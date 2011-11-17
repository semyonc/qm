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
using System.Xml.XPath;
using System.Xml.Schema;

using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    internal sealed class XQueryMapping : XQueryExprBase
    {
        private bool m_root;
        private object m_expr;
        private FunctionLink m_compiledExpr;
        private XQueryExprBase m_bodyExpr;

        public XQueryMapping(XQueryContext context, object expr, XQueryExprBase bodyExpr, bool root)
            : base(context)
        {
            m_root = root;
            m_expr = expr;
            m_bodyExpr = bodyExpr;
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            m_compiledExpr = new FunctionLink();
            QueryContext.Engine.Compile(parameters, m_expr, m_compiledExpr);
            m_bodyExpr.Bind(parameters, pool);
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            List<FunctionLink> res = new List<FunctionLink>();
            res.Add(m_compiledExpr);
            res.AddRange(m_bodyExpr.EnumDynamicFuncs());
            return res;
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            XQueryNodeIterator iter = XQueryNodeIterator.Create(
                QueryContext.Engine.Apply(null, null, m_expr, args, m_compiledExpr, pool));
            if (m_root)
                return new XQueryMappingIterator(args, pool, iter, m_bodyExpr);
            else
            {
                ContextProvider contextProvider = new ContextProvider(iter);
                object res = Undefined.Value;
                while (iter.MoveNext())
                    res = m_bodyExpr.Execute(contextProvider, args, pool);
                return res;
            }
        }

        private sealed class XQueryMappingIterator : XQueryNodeIterator
        {
            private IContextProvider provider;
            private object[] args;
            private MemoryPool pool;
            private XQueryNodeIterator baseIter;
            private XQueryExprBase bodyExpr;

            public XQueryMappingIterator(object[] args, MemoryPool pool, XQueryNodeIterator iter, XQueryExprBase bodyExpr)
            {
                this.args = args;
                this.pool = pool;
                this.baseIter = iter.Clone();
                this.bodyExpr = bodyExpr;
                provider = new ContextProvider(baseIter);
            }

            public override XQueryNodeIterator Clone()
            {
                return new XQueryMappingIterator(args, pool, baseIter, bodyExpr);
            }

            public override XQueryNodeIterator CreateBufferedIterator()
            {
                return new BufferedNodeIterator(this);
            }

            protected override XPathItem NextItem()
            {
                if (baseIter.MoveNext())
                    return (XPathItem)bodyExpr.Execute(provider, args, pool);
                return null;
            }
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            sb.Append(Lisp.Format(m_expr));
            sb.Append(" return ");
            sb.Append(m_bodyExpr.ToString());
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }
}
