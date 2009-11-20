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
    internal class XQueryLET : XQueryFLWORBase
    {
        private SymbolLink m_value;
        private bool m_convert;

        public XQueryLET(XQueryContext context, object var, XQuerySequenceType varType, object expr, XQueryExprBase bodyExpr, bool convert)
            : base(context, var, varType, expr, bodyExpr)
        {
            m_value = new SymbolLink(varType.ValueType);
            m_convert = convert;
        }

        public override void Bind(Executive.Parameter[] parameters)
        {
            m_valueExpr = new SymbolLink();
            QueryContext.Engine.Compile(parameters, m_expr, m_valueExpr);
            object data = QueryContext.Resolver.GetCurrentStack();
            QueryContext.Resolver.SetValue(m_var, m_value);
            if (ConditionExpr != null)
            {
                m_conditionExpr = new SymbolLink();
                QueryContext.Engine.Compile(parameters, ConditionExpr, m_conditionExpr);
            }
            m_bodyExpr.Bind(parameters);
            QueryContext.Resolver.RevertToStack(data);
        }

        public override object Execute(IContextProvider provider, object[] args)
        {
            object value = QueryContext.Engine.Apply(null, null, m_expr, args, m_valueExpr);
            if (value == null)
                value = DataEngine.CoreServices.Generation.RuntimeOps.False;
            if (m_varType != XQuerySequenceType.Item && m_convert)
                value = Core.TreatAs(QueryContext.Engine, value, m_varType);
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
                value = iter.CreateBufferedIterator();
            m_value.Value = value;
            if (m_conditionExpr != null &&
                !Core.BooleanValue(QueryContext.Engine.Apply(null, null, ConditionExpr, args, m_conditionExpr)))
                return EmptyIterator.Shared;
            return m_bodyExpr.Execute(provider, args);
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
            sb.Append(" := ");
            sb.Append(m_expr.ToString());
            if (ConditionExpr != null)
            {
                sb.Append(" where ");
                sb.Append(ConditionExpr.ToString());
            }
            sb.Append(" return ");
            sb.Append(m_bodyExpr.ToString());
            sb.Append("]");
            return sb.ToString();
        }
#endif
    
    }
}
