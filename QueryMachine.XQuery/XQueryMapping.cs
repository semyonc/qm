//        Copyright (c) 2009-2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
