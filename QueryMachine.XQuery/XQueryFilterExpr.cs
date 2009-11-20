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
        private XQueryExprBase[] m_filter;
        
        public XQueryExprBase SourceExpr { get; set; }
        
        public XQueryFilterExpr(XQueryContext queryContext, XQueryExprBase[] filter)
            : base(queryContext)
        {
            m_filter = filter;
        }

        private IEnumerable<XPathItem> CreateEnumerator(object[] args,  
            XQueryExpr expr, XQueryNodeIterator baseIter)
        {
            XQueryNodeIterator iter = baseIter.Clone();
            if (expr != null && expr.m_expr.Length == 1 
                && expr.m_expr[0] is int)
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
                    object res = expr.Execute(provider, args);
                    if (res == Undefined.Value)
                        continue;
                    XQueryNodeIterator iter2 = res as XQueryNodeIterator;
                    XPathItem item;
                    if (iter2 != null)
                    {
                        if (!iter2.MoveNext())
                            continue;
                        item = iter2.Current.Clone();
                        if (!item.IsNode && iter2.MoveNext())
                            throw new XQueryException(Properties.Resources.FORG0006, "fn:boolean()",
                                new XQuerySequenceType(XmlTypeCode.AnyAtomicType, XmlTypeCardinality.OneOrMore));
                    }
                    else
                    {
                        item = res as XPathItem;
                        if (item == null)
                            item = new XQueryItem(res);
                    }
                    if (item.IsNode)
                        yield return iter.Current;
                    else
                    {
                        if (TypeConverter.IsNumberType(item.ValueType))
                        {
                            if (QueryContext.Engine.OperatorEq(iter.CurrentPosition + 1, item.TypedValue) != null)
                                yield return iter.Current;
                        }
                        else
                            if (Core.BooleanValue(item))
                                yield return iter.Current;
                    }
                }                
            }
        }

        public override void Bind(Executive.Parameter[] parameters)
        {
            SourceExpr.Bind(parameters);
            foreach (XQueryExprBase expr in m_filter)
                expr.Bind(parameters);
        }

        public override IEnumerable<SymbolLink> EnumDynamicFuncs()
        {
            List<SymbolLink> res = new List<SymbolLink>();
            res.AddRange(SourceExpr.EnumDynamicFuncs());
            foreach (XQueryExprBase expr in m_filter)
                res.AddRange(expr.EnumDynamicFuncs());
            return res;
        }

        public override object Execute(IContextProvider provider, object[] args)
        {
            if (SourceExpr == null)
                return EmptyIterator.Shared;
            XQueryNodeIterator iter = XQueryNodeIterator.Create(SourceExpr.Execute(provider, args));
            for (int k = 0; k < m_filter.Length; k++)
                iter = new NodeIterator(CreateEnumerator(args, (XQueryExpr)m_filter[k], iter));
            return iter;
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            for (int k = 0; k < m_filter.Length; k++)
            {
                if (k > 0)
                    sb.Append(", ");
                sb.Append(m_filter[k].ToString());
            }
            if (SourceExpr != null)
            {
                sb.Append(" ");
                sb.Append(SourceExpr.ToString());
            }
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }
}
