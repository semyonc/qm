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

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

using DataEngine.CoreServices;
using System.Diagnostics;


namespace DataEngine.XQuery
{
    public enum XQueryOrder
    {
        Default,
        Ordered,
        Unordered
    }

    public sealed class XQueryExpr : XQueryExprBase
    {
        internal object[] m_expr;
        internal FunctionLink[] m_compiledBody;
        private FunctionLink[] m_compiledAnnotation;
        private SymbolLink m_context;

        public XQueryOrder QueryOrder { get; set; }
        public object[] Annotation { get; set; }
        public bool Parallel { get; set; }

        public XQueryExpr(XQueryContext context, object[] expr)
            : base(context)
        {
            m_expr = expr;
            m_compiledBody = null;
            QueryOrder = XQueryOrder.Default;
        }

        public XQueryExpr(XQueryContext context, object[] expr, object[] annotation)
            : this(context, expr)
        {
            Annotation = annotation;
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            m_context = new SymbolLink(typeof(IContextProvider));            
            pool.Bind(m_context);
            object data = QueryContext.Resolver.GetCurrentStack();
            QueryContext.Resolver.SetValue(ID.Context, m_context);
            m_compiledBody = new FunctionLink[m_expr.Length];
            for (int k = 0; k < m_expr.Length; k++)
            {
                m_compiledBody[k] = new FunctionLink();
                QueryContext.Engine.Compile(parameters, m_expr[k], m_compiledBody[k]);
            }
            if (Annotation != null)
            {
                m_compiledAnnotation = new FunctionLink[Annotation.Length];
                for (int k = 0; k < Annotation.Length; k++)
                {
                    m_compiledAnnotation[k] = new FunctionLink();
                    QueryContext.Engine.Compile(parameters, Annotation[k], m_compiledAnnotation[k]);
                }
            }
            QueryContext.Resolver.RevertToStack(data);
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            List<FunctionLink> res = new List<FunctionLink>();
            res.AddRange(m_compiledBody);
            if (m_compiledAnnotation != null)
                res.AddRange(m_compiledAnnotation);
            return res;
        }

        public override bool IsContextSensitive(Executive.Parameter[] parameters)
        {
            for (int k = 0; k < m_expr.Length; k++)
            {
                SymbolLink[] dps = QueryContext.Engine.GetValueDependences(null, parameters,
                    m_expr[k], m_compiledBody[k], false);
                foreach (SymbolLink s in dps)
                    if (s == m_context)
                        return true;
            }
            if (Annotation != null)
            {
                for (int k = 0; k < Annotation.Length; k++)
                {
                    SymbolLink[] dps = QueryContext.Engine.GetValueDependences(null, parameters,
                       Annotation[k], m_compiledAnnotation[k], false);
                    foreach (SymbolLink s in dps)
                        if (s == m_context)
                            return true;
                }
            }
            return false;
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            pool.SetData(m_context, provider);
            if (Annotation != null)
            {
                object[] annotation = new object[Annotation.Length];
                for (int k = 0; k < Annotation.Length; k++)
                {
                    object res = QueryContext.Engine.Apply(null, null,
                        Annotation[k], args, m_compiledAnnotation[k], pool);
                    if (res != Undefined.Value)
                        if (res == null)
                            annotation[k] = false;
                        else
                            annotation[k] = Core.Atomize(res);
                }
                if (Parallel && QueryContext.EnableHPC)
                    return new XQueryExprIteratorHPC(this, args, annotation, pool);
                return new XQueryExprIterator(this, args, annotation, pool);
            }
            if (m_expr.Length == 1)
                return QueryContext.Engine.Apply(null, null, m_expr[0], args, m_compiledBody[0], pool);
            if (Parallel && QueryContext.EnableHPC)
                return new XQueryExprIteratorHPC(this, args, null, pool);
            return new XQueryExprIterator(this, args, null, pool);
        }

        public override object ToLispFunction()
        {
            if (m_expr.Length == 1)
                return m_expr[0];
            return base.ToLispFunction();
        }

        #region Iterators

        private abstract class XQueryExprIteratorBase : XQueryNodeIterator
        {
            protected XQueryExpr owner;
            protected object[] args;
            protected MemoryPool pool;
            protected XQueryNodeIterator childIter;
            protected object[] annotation;
            
            private int index = 0;
            private XQueryItem currItem = new XQueryItem();

            public XQueryExprIteratorBase (XQueryExpr owner, object[] args, object[] annotation, MemoryPool pool)
            {
                this.owner = owner;
                this.args = args;
                this.annotation = annotation;
                this.pool = pool;
            }

            public override XQueryNodeIterator CreateBufferedIterator()
            {
                return new BufferedNodeIterator(this);
            }

            protected abstract object GetItemAtIndex(int index);

            protected override XPathItem NextItem()
            {
                while (true)
                {
                    if (childIter != null)
                    {
                        if (childIter.MoveNext())
                        {
                            if (annotation != null)
                                return new XQueryWrappedValue(childIter.Current, annotation);
                            else
                                return childIter.Current;
                        }
                        else
                            childIter = null;
                    }
                    if (index == owner.m_expr.Length)
                        return null;
                    object res = GetItemAtIndex(index++);
                    if (res != Undefined.Value)
                    {
                        childIter = res as XQueryNodeIterator;
                        if (childIter == null)
                        {
                            XPathItem item = res as XPathItem;
                            if (item == null)
                            {
                                item = currItem;
                                currItem.RawValue = res;
                            }
                            if (annotation != null)
                                return new XQueryWrappedValue(item, annotation);
                            else
                                return item;
                        }
                    }
                }
            }
        }

        private sealed class XQueryExprIterator : XQueryExprIteratorBase
        {
            public XQueryExprIterator(XQueryExpr owner, object[] args, object[] annotation, MemoryPool pool)
                : base(owner, args, annotation, pool)
            {
            }

            public override XQueryNodeIterator Clone()
            {
                return new XQueryExprIterator(owner, args, annotation, pool);
            }

            protected override object GetItemAtIndex(int index)
            {
                return owner.QueryContext.Engine.Apply(null, null,
                    owner.m_expr[index], args, owner.m_compiledBody[index], pool);
            }
        }

        private sealed class XQueryExprIteratorHPC : XQueryExprIteratorBase
        {
            private ConcurrentDictionary<int, Object> orderedBag = new ConcurrentDictionary<int, object>();

            public XQueryExprIteratorHPC(XQueryExpr owner, object[] args, object[] annotation, MemoryPool pool)
                : base(owner, args, annotation, pool)
            {
            }

            public override XQueryNodeIterator Clone()
            {
                return new XQueryExprIteratorHPC(owner, args, annotation, pool);
            }

            protected override void Init()
            {
                ParallelOptions options = new ParallelOptions();
                options.CancellationToken = owner.QueryContext.Token;
                ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();
                ParallelLoopResult result = System.Threading.Tasks.Parallel.For(0, owner.m_expr.Length - 1, options, 
                    () => pool.Fork(),
                    (int index, ParallelLoopState state, MemoryPool localPool) =>
                    {
                        try
                        {
                            orderedBag[index] = owner.QueryContext.Engine.Apply(null, null,
                                owner.m_expr[index], args, owner.m_compiledBody[index], localPool);
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(ex);
                            state.Break();
                        }
                        return localPool;
                    },
                    (MemoryPool localPool) => { });
                if (exceptions.Count > 0)
                    owner.QueryContext.AggregateMultiplyException(exceptions.ToArray());
            }

            protected override object GetItemAtIndex(int index)
            {
                return orderedBag[index];
            }
        }
        #endregion

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            if (m_expr.Length == 0)
                sb.Append("()");
            else
            {
                for (int k = 0; k < m_expr.Length; k++)
                {
                    if (k > 0)
                        sb.Append(", ");
                    sb.Append(Lisp.Format(m_expr[k]));
                }
            }
            sb.Append("]");
            if (Parallel)
                sb.Append("{PARALLEL}");
            return sb.ToString();
        }
#endif

        public static XQueryExprBase Create(XQueryContext context, params object[] expr)
        {
            if (expr.Length == 1 && Lisp.IsFunctor(expr[0], ID.DynExecuteExpr, 4))
              return (XQueryExprBase)Lisp.Arg1(expr[0]);
            return new XQueryExpr(context, expr);
        }
    }
}
