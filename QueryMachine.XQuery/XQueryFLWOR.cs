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
using System.Diagnostics;

namespace DataEngine.XQuery
{
    internal abstract class XQueryFLWORBase : XQueryExprBase
    {
        internal object m_var;
        internal XQuerySequenceType m_varType;
        internal object m_expr;
        internal XQueryExprBase m_bodyExpr;
        
        protected FunctionLink m_valueExpr;
        protected FunctionLink m_conditionExpr;

        public object ConditionExpr { get; set; }

        public XQueryFLWORBase(XQueryContext context, object var, XQuerySequenceType varType, object expr, XQueryExprBase bodyExpr)
            : base(context)
        {
            m_var = var;
            m_varType = varType;
            m_expr = expr;
            m_bodyExpr = bodyExpr;
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            if (m_valueExpr == null)
                throw new InvalidOperationException();
            List<FunctionLink> res = new List<FunctionLink>();
            res.Add(m_valueExpr);
            if (m_conditionExpr != null)
                res.Add(m_conditionExpr);
            res.AddRange(m_bodyExpr.EnumDynamicFuncs());
            return res;
        }
    }


    internal sealed class XQueryFLWOR : XQueryFLWORBase
    {
        private object m_pos;                
        private SymbolLink m_value;
        private SymbolLink m_posValue;
        private Type m_itemType;
        private bool m_convert;

        public XQueryFLWOR(XQueryContext context, object var, XQuerySequenceType varType, object pos, object expr, XQueryExprBase bodyExpr, bool convert)
            : base(context, var, varType, expr, bodyExpr)
        {
            m_var = var;
            m_varType = varType;            
            m_pos = pos;
            m_value = new SymbolLink(varType.ValueType);
            m_itemType = varType.ItemType;
            if (m_pos != null)
                m_posValue = new SymbolLink(typeof(Integer));
            m_convert = convert;
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            m_valueExpr = new FunctionLink();
            QueryContext.Engine.Compile(parameters, m_expr, m_valueExpr);
            object data = QueryContext.Resolver.GetCurrentStack();
            pool.Bind(m_value);
            QueryContext.Resolver.SetValue(m_var, m_value);
            if (m_pos != null)
            {
                pool.Bind(m_posValue);
                QueryContext.Resolver.SetValue(m_pos, m_posValue);
            }
            if (ConditionExpr != null)
            {
                m_conditionExpr = new FunctionLink();
                QueryContext.Engine.Compile(parameters, ConditionExpr, m_conditionExpr);
            }
            m_bodyExpr.Bind(parameters, pool);
            QueryContext.Resolver.RevertToStack(data);
        }

        public override object Execute(IContextProvider provider, Object[] args, MemoryPool pool)
        {
            return new XQueryFLWORIterator(this, provider, args, pool, XQueryNodeIterator.Create(
                QueryContext.Engine.Apply(null, null, m_expr, args, m_valueExpr, pool)));
        }

        private bool MoveNext(IContextProvider provider, object[] args, MemoryPool pool, XPathItem curr, Integer index, out object res)
        {
            object value;
            if (curr.IsNode)
                value = curr;
            else
                value = curr.TypedValue;
            if (m_varType != XQuerySequenceType.Item && m_convert)
            {
                if (m_varType.IsNode && !Core.InstanceOf(QueryContext.Engine, value, m_varType))
                    throw new XQueryException(Properties.Resources.XPTY0004,
                       new XQuerySequenceType(curr.XmlType.TypeCode), m_varType);
                value = XQueryConvert.TreatValueAs(value, m_varType);
                if (m_varType.Cardinality == XmlTypeCardinality.ZeroOrMore ||
                    m_varType.Cardinality == XmlTypeCardinality.OneOrMore)
                    value = XQueryNodeIterator.Create(value);
            }
            pool.SetData(m_value, value);
            if (m_pos != null)
                pool.SetData(m_posValue, index);
            res = null;
            if (m_conditionExpr == null ||
                Core.BooleanValue(QueryContext.Engine.Apply(null, null, ConditionExpr, args, m_conditionExpr, pool)))
            {
                res = m_bodyExpr.Execute(provider, args, pool);
                if (res != Undefined.Value)
                    return true;
            }
            return false;
        }

        private sealed class XQueryFLWORIterator : XQueryNodeIterator
        {
            private XQueryFLWOR owner;
            private IContextProvider provider;
            private object[] args;
            private MemoryPool pool;
            private XQueryNodeIterator baseIter;
            private XQueryNodeIterator iter;
            private XQueryNodeIterator childIter;
            private Integer index;

            private XQueryItem currItem = new XQueryItem();

            public XQueryFLWORIterator(XQueryFLWOR owner, IContextProvider provider, object[] args, MemoryPool pool, XQueryNodeIterator baseIter)
            {
                this.owner = owner;
                this.provider = provider;
                this.args = args;
                this.pool = pool;
                this.baseIter = baseIter;                
            }

            public override XQueryNodeIterator Clone()
            {
                return new XQueryFLWORIterator(owner, provider, args, pool, baseIter);
            }

            public override XQueryNodeIterator CreateBufferedIterator()
            {
                return new BufferedNodeIterator(this);
            }

            protected override void Init()
            {
                index = 1;
                iter = baseIter.Clone();
            }

            protected override XPathItem NextItem()
            {
                while (true)
                {
                    if (childIter != null)
                    {
                        if (childIter.MoveNext())
                            return childIter.Current;
                        else
                            childIter = null;
                    }
                    if (!iter.MoveNext())
                        return null;
                    object res;
                    if (owner.MoveNext(provider, args, pool, iter.Current, index++, out res))
                    {
                        childIter = res as XQueryNodeIterator;
                        if (childIter == null)
                        {
                            XPathItem item = res as XPathItem;
                            if (item != null)
                                return item;
                            currItem.RawValue = res;
                            return currItem;                            
                        }
                    }
                }
            }
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
