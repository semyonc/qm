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
    sealed class XQueryFilterExpr: XQueryExprBase
    {
        private XQueryExprBase[] m_filter;
        private bool m_contextSensitive;
        private XQueryExprBase m_src;
        
        public XQueryExprBase Source 
        {
            get
            {
                return m_src;
            }
            set
            {
                m_src = value;
                XQueryPathExpr pathExpr = m_src as XQueryPathExpr;
                if (pathExpr != null)
                    pathExpr.EnableCaching = false;
            }
        }
        
        public XQueryFilterExpr(XQueryContext queryContext, XQueryExprBase[] filter)
            : base(queryContext)
        {
            m_filter = filter;
        }

        private IEnumerable<XPathItem> CreateEnumerator(object[] args,  MemoryPool pool,
            XQueryExprBase expr, XQueryNodeIterator baseIter)
        {
            XQueryNodeIterator iter = baseIter.Clone();
            XQueryExpr numexpr = expr as XQueryExpr;
            if (numexpr != null && numexpr.m_expr.Length == 1
                && numexpr.m_expr[0] is Integer)
            {
                Integer pos = (Integer)numexpr.m_expr[0];
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
                object res = Undefined.Value;
                while (iter.MoveNext())
                {
                    if (m_contextSensitive || res == Undefined.Value)
                        res = expr.Execute(provider, args, pool);
                    if (res == Undefined.Value)
                    {
                        if (!m_contextSensitive)
                            break;
                        continue;
                    }                            
                    XQueryNodeIterator iter2 = res as XQueryNodeIterator;
                    XPathItem item;
                    if (iter2 != null)
                    {
                        iter2 = iter2.Clone();
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
                            {
                                yield return iter.Current;
                                if (!m_contextSensitive)
                                    break;
                            }
                        }
                        else
                            if (Core.BooleanValue(item))
                                yield return iter.Current;
                    }
                }                
            }
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            Source.Bind(parameters, pool);
            m_contextSensitive = false;
            foreach (XQueryExprBase expr in m_filter)
            {
                expr.Bind(parameters, pool);
                if (!m_contextSensitive)
                    m_contextSensitive = expr.IsContextSensitive(parameters);
            }
        }

        public override bool IsContextSensitive(Executive.Parameter[] parameters)
        {
            return Source.IsContextSensitive(parameters);
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            List<FunctionLink> res = new List<FunctionLink>();
            res.AddRange(Source.EnumDynamicFuncs());
            foreach (XQueryExprBase expr in m_filter)
                res.AddRange(expr.EnumDynamicFuncs());
            return res;
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            if (Source == null)
                return EmptyIterator.Shared;
            XQueryNodeIterator iter = XQueryNodeIterator.Create(Source.Execute(provider, args, pool));
            for (int k = 0; k < m_filter.Length; k++)
                iter = new NodeIterator(CreateEnumerator(args, pool, m_filter[k], iter));
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
            if (Source != null)
            {
                sb.Append(" ");
                sb.Append(Source.ToString());
            }
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }
}
