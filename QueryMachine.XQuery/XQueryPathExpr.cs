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

namespace DataEngine.XQuery
{
    internal sealed class XQueryPathExpr: XQueryExprBase
    {
        private XQueryExprBase[] _path;        
        
        private bool _isOrdered;
        private bool _isOrderedSet;        
        private bool _isStatic;
        private bool? _isParameterSensitive;

        private SymbolLink _threadCache;
        private volatile XQueryNodeIterator _instanceCache;        

        public bool EnableCaching { get; set; }

        public XQueryExprBase LastStep
        {
            get
            {
                XQueryExprBase t = _path[_path.Length - 1];
                XQueryFilterExpr filter = t as XQueryFilterExpr;
                if (filter != null)
                    return filter.Source;
                DirectAccessPathExpr directAccessPath = t as DirectAccessPathExpr;
                if (directAccessPath != null)
                    return directAccessPath.LastStep;
                return t;
            }
        }

        public XQueryPathExpr(XQueryContext context, XQueryExprBase[] path, bool isOrdered)
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
            return _path[0].IsContextSensitive(parameters);
        }

        public override bool IsParameterSensitive()
        {
            if (_isParameterSensitive == null)
            {
                _isParameterSensitive = false;
                foreach (XQueryExprBase expr in _path)
                    if (expr.IsParameterSensitive())
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
            foreach (XQueryExprBase expr in _path)
                expr.Bind(parameters, pool);
            EnableCaching = EnableCaching && !IsContextSensitive(parameters);
            pool.Bind(_threadCache);
            if (EnableCaching)
            {
                _isStatic = !IsParameterSensitive();
                foreach (XQueryExprBase expr in _path)
                    expr.GetValueDependences(null, parameters, false, (SymbolLink value) =>
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
            foreach (XQueryExprBase expr in _path)
                expr.GetValueDependences(hs, parameters, reviewLambdaExpr, callback);
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            List<FunctionLink> res = new List<FunctionLink>();
            foreach (XQueryExprBase expr in _path)
                res.AddRange(expr.EnumDynamicFuncs());
            return res;
        }

        private XQueryNodeIterator CreateIterator(IContextProvider provider, object[] args, MemoryPool pool)
        {
#if DEBUG
            PerfMonitor.Global.Begin(this);
#endif
            XQueryNodeIterator rootIter = 
                XQueryNodeIterator.Create(_path[0].Execute(provider, args, pool)).CreateBufferedIterator();
            bool orderedSet = _isOrderedSet && rootIter.IsSingleIterator;
            XQueryNodeIterator res = new ResultIterator(this, provider, orderedSet, 
                !orderedSet & QueryContext.EnableHPC, rootIter, args, pool);
#if DEBUG
            PerfMonitor.Global.End(this);
#endif
            return res;
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            if (_isStatic)
            {
                if (_instanceCache == null)
                {
                    lock (this)
                    {
                        if (_instanceCache == null)
                            _instanceCache = CreateIterator(provider, args, pool).CreateBufferedIterator();
                    }
                }
                return _instanceCache.Clone();
            }
            else
            {
                XQueryNodeIterator res = (XQueryNodeIterator)pool.GetData(_threadCache);
                if (res != null)
                    return res.Clone();
                res = CreateIterator(provider, args, pool);
                if (EnableCaching)
                {
                    res = res.CreateBufferedIterator();
                    pool.SetData(_threadCache, res.Clone());
                }
                return res;
            }
        }

        private bool IsOrderedSet()
        {
            for (int k = 1; k < _path.Length; k++)
            {
                if (_path[k] is DirectAccessPathExpr)
                    continue;
                XQueryStepExpr stepExpr;
                if (_path[k] is XQueryFilterExpr)
                    stepExpr = ((XQueryFilterExpr)_path[k]).Source as XQueryStepExpr;
                else
                    stepExpr = _path[k] as XQueryStepExpr;
                if (stepExpr == null)
                    return false;
                switch (stepExpr.ExprType)
                {
                    case XQueryPathExprType.Parent:
                    case XQueryPathExprType.Ancestor:
                    case XQueryPathExprType.AncestorOrSelf:
                    case XQueryPathExprType.Preceding:
                    case XQueryPathExprType.PrecedingSibling:
                        return false;

                    case XQueryPathExprType.Descendant:
                    case XQueryPathExprType.DescendantOrSelf:
                    case XQueryPathExprType.Following:
                        if (k < _path.Length - 1)
                            return false;
                        break;
                }
            }
            return true;
        }

        private void OnChangeValue(SymbolLink line, MemoryPool pool)
        {
            pool.SetData(_threadCache, null);
        }

        private bool MoveNext(object[] args, MemoryPool pool, XQueryNodeIterator[] iter, int index)
        {
            if (iter[index] != null && iter[index].MoveNext())
                return true;
            else
                if (index > 0)
                {
                    while (MoveNext(args, pool, iter, index - 1))
                    {
                        ContextProvider provider = new ContextProvider(iter[index - 1]);
                        if (!provider.Context.IsNode)
                            throw new XQueryException(Properties.Resources.XPTY0019, provider.Context.Value);
                        iter[index] = XQueryNodeIterator.Create(_path[index].Execute(provider, args, pool));
                        if (iter[index].MoveNext())
                            return true;
                    }
                }
            return false;
        }

        private IEnumerator<XPathItem> PushIterator(ResultIterator res)
        {
            if (!res.itemSet.Completed)
            {                
                XQueryNodeIterator[] iter = new XQueryNodeIterator[_path.Length];
                iter[0] = res.rootIter.Clone();
                if (MoveNext(res.args, res.pool, iter, _path.Length - 1))
                {                    
                    bool isNode = iter[_path.Length - 1].Current.IsNode;
                    if (!isNode || res.orderedSet)
                    {
                        do
                        {
                            XPathItem curr = iter[_path.Length - 1].Current;
                            if (curr.IsNode != isNode)
                                throw new XQueryException(Properties.Resources.XPTY0018, curr.Value);
                            yield return curr;
                        }
                        while (MoveNext(res.args, res.pool, iter, _path.Length - 1));            
                    }
                    else
                    {
                        bool needSort = false;
                        XPathNavigator last_node = null;
                        do
                        {
                            XPathItem item = iter[_path.Length - 1].Current;
                            if (!item.IsNode)
                                throw new XQueryException(Properties.Resources.XPTY0018, item.Value);
                            XPathNavigator node = (XPathNavigator)item;
                            if (!needSort)
                            {
                                if (last_node != null)
                                    needSort = last_node.ComparePosition(node) == XmlNodeOrder.Before;
                            }
                            last_node = node.Clone();
                            res.itemSet.Add(last_node);
                        }
                        while (MoveNext(res.args, res.pool, iter, _path.Length - 1));
                        if (needSort)
                            res.itemSet.Sort();
                        res.itemSet.Completed = true;
                    }
                }
            }
            if (res.itemSet.Completed)
            {
                XPathNavigator last_node = null;
                foreach (XPathItem item in res.itemSet)
                {
                    XPathNavigator node = item as XPathNavigator;
                    if (node != null)
                    {
                        if (last_node != null)
                        {
                            if (last_node.IsSamePosition(node))
                                continue;
                        }
                        if (!res.orderedSet)
                            last_node = node;
                    }
                    yield return item;
                }
            }
        }

        private sealed class ResultIterator : XQueryNodeIterator
        {
            private XQueryPathExpr owner;
            
            public ItemSet itemSet;
            public IContextProvider provider;
            public bool orderedSet;
            public bool parallel;
            public XQueryNodeIterator rootIter;
            public object[] args;
            public MemoryPool pool;
            public IEnumerator<XPathItem> iter;

            public ResultIterator(XQueryPathExpr owner, IContextProvider provider, bool orderedSet, bool parallel,
                XQueryNodeIterator rootIter, object[] args, MemoryPool pool)
            {
                itemSet = new ItemSet();
                this.owner = owner;
                this.provider = provider;
                this.orderedSet = orderedSet;
                this.rootIter = rootIter;
                this.args = args;
                this.pool = pool;
                this.parallel = parallel;
            }

            [DebuggerStepThrough]
            public override XQueryNodeIterator Clone()
            {
                ResultIterator clone = new ResultIterator(owner, provider, orderedSet, parallel, rootIter, args, pool);
                clone.itemSet = itemSet;
                return clone;
            }

            protected override void Init()
            {
                iter = owner.PushIterator(this);
            }

            protected override XPathItem NextItem()
            {
#if DEBUG
                try
                {
                    PerfMonitor.Global.Begin(owner);
#endif
                if (iter.MoveNext())
                    return iter.Current;
                return null;
#if DEBUG
                }
                finally
                {
                    PerfMonitor.Global.End(owner);
                }
#endif
            }

            public override XQueryNodeIterator CreateBufferedIterator()
            {
                if (!orderedSet)
                    return Clone();
                return new BufferedNodeIterator(this);
            }
        }        

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
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
       
        private sealed class ItemContext : IContextProvider
        {
            private XQueryNodeIterator iter;

            public ItemContext(XQueryNodeIterator iter)
            {
                this.iter = iter;
                Context = iter.Current.Clone();
                CurrentPosition = iter.CurrentPosition + 1;
            }

            #region IContextProvider Members

            public XPathItem Context { get; private set; }

            public int CurrentPosition { get; private set; }

            public int LastPosition 
            {
                get
                {
                    return iter.Count;
                }
            }
            
            #endregion
        }
    }
}
