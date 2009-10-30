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
    internal abstract class XQueryFLWORBase : XQueryExprBase
    {
        internal object m_var;
        internal XQuerySequenceType m_varType;
        internal object m_expr;
        internal XQueryExprBase m_bodyExpr;
        protected SymbolLink m_compiledExpr;

        public XQueryFLWORBase(XQueryContext context, object var, XQuerySequenceType varType, object expr, XQueryExprBase bodyExpr)
            : base(context)
        {
            m_var = var;
            m_varType = varType;
            m_expr = expr;
            m_bodyExpr = bodyExpr;
        }

        public override IEnumerable<SymbolLink> EnumDynamicFuncs()
        {
            if (m_compiledExpr == null)
                throw new InvalidOperationException();
            List<SymbolLink> res = new List<SymbolLink>();
            res.Add(m_compiledExpr);
            res.AddRange(m_bodyExpr.EnumDynamicFuncs());
            return res;
        }
    }

    internal class XQueryFLWOR : XQueryFLWORBase
    {
        private object m_pos;                
        private SymbolLink m_value;
        private SymbolLink m_posValue;
        private Type m_itemType;

        public XQueryFLWOR(XQueryContext context, object var, XQuerySequenceType varType, object pos, object expr, XQueryExprBase bodyExpr)
            : base(context, var, varType, expr, bodyExpr)
        {
            m_var = var;
            m_varType = varType;            
            m_pos = pos;
            m_value = new SymbolLink(varType.ValueType);
            m_itemType = varType.ItemType;
            if (m_pos != null)
                m_posValue = new SymbolLink(typeof(System.Int32));
        }

        private IEnumerable<XPathItem> CreateEnumerator(IContextProvider provider, object[] args, XQueryNodeIterator baseIter)
        {
            int index = 1;
            XQueryNodeIterator iter = baseIter.Clone();
            while(iter.MoveNext())
            {
                XPathItem curr = iter.Current;
                if (m_varType != XQuerySequenceType.Item)
                {
                    if (curr.ValueType != m_value.Type && !Core.InstanceOf(QueryContext.Engine, curr, m_varType))
                        throw new XQueryException(Properties.Resources.XPTY0004,
                            new XQuerySequenceType(curr.XmlType.TypeCode), m_varType);
                    object value = curr;
                    if (m_varType.IsNumeric)
                        value = curr.ValueAs(m_itemType);
                    else
                        if (!curr.IsNode)
                            value = curr.TypedValue;
                    if (m_varType.Cardinality == XmlTypeCardinality.ZeroOrMore ||
                        m_varType.Cardinality == XmlTypeCardinality.OneOrMore)
                        value = Core.CreateSequence(QueryContext.Engine, value);
                    m_value.Value = value;
                }
                else
                {
                    if (curr.IsNode)
                        m_value.Value = curr;
                    else
                        m_value.Value = curr.TypedValue;
                }
                if (m_pos != null)
                    m_posValue.Value = index++;
                XQueryNodeIterator res = m_bodyExpr.Execute(provider, args);
                res = res.Clone();
                while (res.MoveNext())
                    yield return res.Current;
            }
        }

        public override void Bind(Executive.Parameter[] parameters)
        {
            m_compiledExpr = new SymbolLink();
            QueryContext.Engine.Compile(parameters, m_expr, m_compiledExpr);
            object data = QueryContext.Resolver.GetCurrentStack();
            QueryContext.Resolver.SetValue(m_var, m_value);
            if (m_pos != null)
                QueryContext.Resolver.SetValue(m_pos, m_posValue);
            m_bodyExpr.Bind(parameters);
            QueryContext.Resolver.RevertToStack(data);
        }

        public override XQueryNodeIterator Execute(IContextProvider provider, Object[] args)
        {
            return new NodeIterator(CreateEnumerator(provider, args, Core.CreateSequence(QueryContext.Engine, 
                QueryContext.Engine.Apply(null, null, m_expr, args, m_compiledExpr))));
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            sb.Append(Lisp.Format(m_var));
            sb.Append(" as ");
            sb.Append(m_varType.ToString());
            if (m_pos != null)
            {
                sb.Append(" at $");
                sb.Append(Lisp.Format(m_pos));
            }
            sb.Append(" := ");
            sb.Append(Lisp.Format(m_expr));
            sb.Append(" return ");
            sb.Append(m_bodyExpr.ToString());
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }       
}
