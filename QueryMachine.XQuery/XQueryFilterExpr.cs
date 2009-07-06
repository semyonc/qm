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
    internal class ContextProvider : IContextProvider
    {
        private XQueryNodeIterator m_iter;

        public ContextProvider(XQueryNodeIterator iter)
        {
            m_iter = iter;
        }

        #region IContextProvider Members

        public XPathItem Context
        {
            get
            {
                return m_iter.Current;
            }
        }

        public int CurrentPosition
        {
            get
            {
                return m_iter.CurrentPosition + 1;
            }
        }

        public int LastPosition
        {
            get
            {
                return m_iter.Count;
            }
        }

        #endregion
    }

    class XQueryFilterExpr: XQueryExprBase
    {
        private XQueryExprBase m_filter;        
        
        public XQueryFilterExpr(XQueryContext queryContext, XQueryExprBase filter)
            : base(queryContext)
        {
            m_filter = filter;         
        }

        private IEnumerable<XPathItem> CreateEnumerator(XQueryNodeIterator baseIter)
        {
            XQueryNodeIterator iter = baseIter.Clone();
            XQueryExpr expr = m_filter as XQueryExpr;
            if (expr != null && expr.m_expr[0] is int)
            {
                int pos = (int)expr.m_expr[0];
                foreach (XPathItem item in iter)
                {
                    if (pos == 1)
                    {
                        yield return item;
                        break;
                    }
                    else
                        pos--;
                }
            }
            else
            {
                ContextProvider provider = new ContextProvider(iter);                
                while (iter.MoveNext())
                {
                    QueryContext.EnterContext(provider);
                    XQueryNodeIterator res = m_filter.Execute(null);
                    if (res.MoveNext())
                    {
                        XPathItem v = res.Current;
                        if (v is XPathNavigator)
                            yield return iter.Current;
                        else
                        {
                            if (res.MoveNext())
                                yield return iter.Current;
                            else
                                if (v.ValueType == typeof(System.Int32))
                                {
                                    if (provider.CurrentPosition == (int)v.TypedValue)
                                    {
                                        yield return iter.Current;
                                        break;
                                    }
                                }
                                else
                                    if (Core.BooleanValue(v))
                                        yield return iter.Current;
                        }
                    }
                    QueryContext.LeaveContext();
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
            sb.Append(m_filter.ToString());
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }
}
