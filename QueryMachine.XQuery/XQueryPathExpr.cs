//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using DataEngine.CoreServices;
using DataEngine.XQuery.DocumentModel;
using DataEngine.XQuery.Collections;
using DataEngine.XQuery.Util;
using DataEngine.XQuery.Iterator;

namespace DataEngine.XQuery
{
    internal sealed class XQueryPathExpr: XQueryExprBase
    {
        private XQueryPathStep[] _path;        
        
        private bool _isOrdered;
        private bool _isOrderedSet;        
        private bool _isStatic;
        private bool? _isParameterSensitive;

        private SymbolLink _threadCache;
        private volatile XQueryNodeIterator _instanceCache;        

        public bool EnableCaching { get; set; }

        public XQueryPathStep[] Path
        {
            get
            {
                return _path;
            }
        }

        public XQueryPathStep LastStep
        {
            get
            {
                int index = _path.Length;
                while (index > 0)
                {
                    XQueryPathStep t = _path[--index];
                    switch (t.type)
                    {
                        case XPath2ExprType.PositionFilter:
                            continue;

                        case XPath2ExprType.Expr:
                            XQueryFilterExpr filter = t.expr as XQueryFilterExpr;
                            if (filter != null)
                            {
                                XQueryPathExpr sourceExpr = filter.Source as XQueryPathExpr;
                                return sourceExpr.LastStep;
                            }
                            break;

                        case XPath2ExprType.DirectAccess:
                            XQueryPathExpr pathExpr = (XQueryPathExpr)t.nodeTest;
                            return pathExpr.LastStep;

                        case XPath2ExprType.ChildOverDescendants:
                            ChildOverDescendantsNodeIterator.NodeTest[] test = (ChildOverDescendantsNodeIterator.NodeTest[])t.nodeTest;
                            return new XQueryPathStep(test[test.Length - 1]);
                    }
                    return t;
                }
                return null;
            }
        }

        public XQueryPathExpr(XQueryContext context, XQueryPathStep[] path, bool isOrdered)
            : base(context)
        {
            _path = path;
            _isOrderedSet = IsOrderedSet();
            _isOrdered = isOrdered;
            _threadCache = new SymbolLink();
            _isStatic = false;
            EnableCaching = true;
        }

        public override bool IsContextSensitive(Executive.Parameter[] parameters)
        {
            if (_path[0].type == XPath2ExprType.Expr)
                return _path[0].expr.IsContextSensitive(parameters);
            return true;
        }

        public override bool IsParameterSensitive()
        {
            if (_isParameterSensitive == null)
            {
                _isParameterSensitive = false;
                foreach (XQueryPathStep step in _path)
                    if (step.type == XPath2ExprType.Expr && step.expr.IsParameterSensitive())
                    {
                        _isParameterSensitive = true;
                        break;
                    }
            }
            return (bool)_isParameterSensitive;
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            HashSet<SymbolLink> outerValues = new HashSet<SymbolLink>(QueryContext.Resolver.List());
            foreach (XQueryPathStep step in _path)
            {
                switch (step.type)
                {
                    case XPath2ExprType.Expr:
                        step.expr.Bind(parameters, pool);
                        break;
                    case XPath2ExprType.DirectAccess:
                        ((XQueryPathExpr)step.nodeTest).Bind(parameters, pool);
                        break;
                }
            }
            EnableCaching = EnableCaching && !IsContextSensitive(parameters);
            pool.Bind(_threadCache);
            if (EnableCaching)
            {
                _isStatic = !IsParameterSensitive();
                foreach (XQueryPathStep step in _path)
                    if (step.type == XPath2ExprType.Expr) 
                        step.expr.GetValueDependences(null, parameters, false, (SymbolLink value) =>
                        {
                            if (!value.IsStatic && outerValues.Contains(value))
                            {
                                value.OnChange += new ChangeValueAction(OnChangeValue);
                                _isStatic = false;
                            }
                        });
            }
        }

        public override void GetValueDependences(HashSet<object> hs, Executive.Parameter[] parameters, bool reviewLambdaExpr, Action<SymbolLink> callback)
        {
            foreach (XQueryPathStep step in _path)
                if (step.type == XPath2ExprType.Expr)
                    step.expr.GetValueDependences(hs, parameters, reviewLambdaExpr, callback);
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            List<FunctionLink> res = new List<FunctionLink>();
            foreach (XQueryPathStep step in _path)
                if (step.type == XPath2ExprType.Expr) 
                    res.AddRange(step.expr.EnumDynamicFuncs());
            return res;
        }

        private XQueryNodeIterator CreateIterator(IContextProvider provider, object[] args, MemoryPool pool)
        {
            bool orderedSet = _isOrderedSet;
            bool special = QueryContext is XPathContext;
            XQueryNodeIterator tail;
            if (_path[0].type == XPath2ExprType.Expr)
            {
                tail = XQueryNodeIterator.Create(_path[0].expr.Execute(provider, args, pool)).CreateBufferedIterator();
                orderedSet = orderedSet && (tail.IsOrderedSet || tail.IsSingleIterator);
            }
            else
                tail = _path[0].Create(QueryContext, args, pool,
                    XQueryNodeIterator.Create(Core.ContextNode(provider)), special);
            for (int k = 1; k < _path.Length; k++)
                tail = _path[k].Create(QueryContext, args, pool, tail, special);
            if (!orderedSet)
                return new DocumentOrderNodeIterator(tail);
            return tail;
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            //if (_isStatic)
            //{
            //    if (_instanceCache == null)
            //    {
            //        lock (this)
            //        {
            //            if (_instanceCache == null)
            //                // _instanceCache = CreateIterator(provider, args, pool).CreateBufferedIterator();
            //                _instanceCache = BufferedNodeIterator.Preload(CreateIterator(provider, args, pool));
            //        }
            //    }
            //    return _instanceCache.Clone();
            //}
            //else
            //{
            XQueryNodeIterator res;
            res = (XQueryNodeIterator)pool.GetData(_threadCache);
            if (res != null)
                return res.Clone();
            res = CreateIterator(provider, args, pool);
            if (EnableCaching)
            {
                res = res.CreateBufferedIterator();
                pool.SetData(_threadCache, res.Clone());
            }
            return res;
            //}
        }

        private bool IsOrderedSet()
        {
            for (int k = 0; k < _path.Length; k++)
            {
                XPath2ExprType exprType;
                if (_path[k].type == XPath2ExprType.Expr)
                {
                    if (k == 0)
                        continue;
                    XQueryFilterExpr filterExpr = _path[k].expr as XQueryFilterExpr;
                    if (filterExpr == null)
                        return false;
                    XQueryPathExpr pathExpr = filterExpr.Source as XQueryPathExpr;
                    if (pathExpr == null)
                        return false;
                    exprType = pathExpr._path[0].type;
                }
                else
                    exprType = _path[k].type;
                switch (exprType)
                {
                    case XPath2ExprType.Expr:
                    case XPath2ExprType.Parent:
                    case XPath2ExprType.Ancestor:
                    case XPath2ExprType.AncestorOrSelf:
                    case XPath2ExprType.Preceding:
                    case XPath2ExprType.PrecedingSibling:
                        return false;

                    case XPath2ExprType.Descendant:
                    case XPath2ExprType.DescendantOrSelf:
                    case XPath2ExprType.Following:
                    case XPath2ExprType.ChildOverDescendants:
                    case XPath2ExprType.DirectAccess:
                        if (k < _path.Length - 1)
                            for (int s = k + 1; s < _path.Length; s++)
                            {
                                if (_path[s].type != XPath2ExprType.Attribute &&
                                    _path[s].type != XPath2ExprType.Namespace)
                                    return false;
                            }
                        break;
                }
            }
            return true;
        }

        private void OnChangeValue(SymbolLink line, MemoryPool pool)
        {
            pool.SetData(_threadCache, null);
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(": ");
            for (int k = 0; k < _path.Length; k++)
            {
                if (k > 0)
                    sb.Append(", ");
                sb.Append(_path[k].ToString());
            }
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }
}
