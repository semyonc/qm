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
    internal sealed class XQueryLET : XQueryFLWORBase
    {
        private SymbolLink m_value;
        private bool m_convert;

        public XQueryLET(XQueryContext context, object var, XQuerySequenceType varType, XQueryExprBase expr, XQueryExprBase bodyExpr, bool convert)
            : base(context, var, varType, expr, bodyExpr)
        {
            m_value = new SymbolLink(varType.ValueType);
            m_convert = convert;
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            m_expr.Bind(parameters, pool);
            if (!m_expr.IsParameterSensitive())
            {
                m_value.IsStatic = true;
                m_expr.GetValueDependences(null, parameters, false, (SymbolLink s) =>
                {
                    if (!s.IsStatic)
                        m_value.IsStatic = false;
                });
            }
            object data = QueryContext.Resolver.GetCurrentStack();
            pool.Bind(m_value);
            QueryContext.Resolver.SetValue(m_var, m_value);
            if (ConditionExpr != null)
            {
                m_conditionExpr = new FunctionLink();
                QueryContext.Engine.Compile(parameters, ConditionExpr, m_conditionExpr);
            }
            m_bodyExpr.Bind(parameters, pool);
            QueryContext.Resolver.RevertToStack(data);
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            object value = m_expr.Execute(provider, args, pool);
            if (value == null)
                value = DataEngine.CoreServices.Generation.RuntimeOps.False;
            if (m_varType != XQuerySequenceType.Item && m_convert)
                value = Core.TreatAs(QueryContext.Engine, value, m_varType);
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null && !(iter is BufferedNodeIterator))
                value = iter.CreateBufferedIterator();
            pool.SetData(m_value, value);
            if (m_conditionExpr != null &&
                !Core.BooleanValue(QueryContext.Engine.Apply(null, null, ConditionExpr, args, m_conditionExpr, pool)))
                return EmptyIterator.Shared;
            return m_bodyExpr.Execute(provider, args, pool);
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
