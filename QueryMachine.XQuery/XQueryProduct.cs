//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
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
    class XQueryProduct : XQueryExprBase
    {
        private object m_expr;
        private SymbolLink m_compiledBody;
        private SymbolLink m_context;

        public XQueryProduct(XQueryContext queryContext, object expr)
            : base(queryContext)
        {
            m_expr = expr;
            m_context = new SymbolLink(typeof(IContextProvider));
        }

        private IEnumerable<XPathItem> CreateEnumerator(XQueryNodeIterator baseIter)
        {
            XQueryNodeIterator iter = baseIter.Clone();
            ContextProvider provider = new ContextProvider(iter);
            while (iter.MoveNext())
            {
                if (m_compiledBody == null)
                {
                    m_compiledBody = new SymbolLink();
                    QueryContext.Resolver.SetValue(ID.Context, m_context);                    
                }
                m_context.Value = provider;
                object res = QueryContext.Engine.Apply(null, null,
                    m_expr, null, m_compiledBody);
                if (res != Undefined.Value)
                {
                    XQueryNodeIterator iter2 = res as XQueryNodeIterator;
                    if (iter2 != null)
                    {
                        foreach (XPathItem item in iter2)
                            yield return item;
                    }
                    else
                    {
                        XPathItem item = res as XPathItem;
                        if (item == null)
                            item = QueryContext.CreateItem(res);
                        yield return item;
                    }
                }
            }            
        }

        public override XQueryNodeIterator Execute(object[] parameters)
        {
            object value = parameters[0];
            if (value == null)
                return EmptyIterator.Shared;
            return new NodeIterator(CreateEnumerator(Core.CreateSequence(QueryContext.Engine, value)));
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            sb.Append(m_expr.ToString());
            sb.Append("]");
            return sb.ToString();
        }
#endif

    }
}
