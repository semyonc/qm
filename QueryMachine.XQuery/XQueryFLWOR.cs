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
        internal object m_body;

        public XQueryFLWORBase(XQueryContext context, object var, XQuerySequenceType varType, object body)
            : base(context)
        {
            m_var = var;
            m_varType = varType;
            m_body = body;
        }
    }

    internal class XQueryFLWOR : XQueryFLWORBase
    {
        private object m_pos;        
        private SymbolLink m_compiledBody;
        private SymbolLink m_value;
        private SymbolLink m_posValue;

        public XQueryFLWOR(XQueryContext context, object var, XQuerySequenceType varType, object pos, object body)
            : base(context, var, varType, body)
        {
            m_var = var;
            m_varType = varType;            
            m_pos = pos;

            m_body = body;            
            m_value = new SymbolLink(varType.ValueType);
            if (m_pos != null)
                m_posValue = new SymbolLink(typeof(System.Int32));
        }

        private IEnumerable<XPathItem> CreateEnumerator(XQueryNodeIterator baseIter, object exprStack)
        {
            int index = 1;
            foreach (XPathItem curr in baseIter)
            {
                m_value.Value = Core.CastTo(QueryContext.Engine, curr, m_varType);
                if (m_pos != null)
                    m_posValue.Value = index++;
                if (m_compiledBody == null)
                {
                    m_compiledBody = new SymbolLink();
                    QueryContext.Resolver.RevertToStack(exprStack);
                    QueryContext.Resolver.SetValue(m_var, m_value);
                    if (m_pos != null)
                        QueryContext.Resolver.SetValue(m_pos, m_posValue);
                }
                object res = QueryContext.Engine.Apply(null, null,
                    m_body, null, m_compiledBody);
                if (res != Undefined.Value)
                {
                    XQueryNodeIterator iter = res as XQueryNodeIterator;
                    if (iter != null)
                        foreach (XPathItem item in iter)
                            yield return item;
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

        public override XQueryNodeIterator Execute(Object[] parameters)
        {
            if (parameters.Length != 1)
                throw new InvalidOperationException();     
            if (m_compiledBody == null)
                return new NodeIterator(CreateEnumerator(Core.CreateSequence(QueryContext.Engine, parameters[0]), 
                    QueryContext.Resolver.GetCurrentStack()));
            else
                return new NodeIterator(CreateEnumerator(Core.CreateSequence(QueryContext.Engine, parameters[0]), null));
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
            sb.Append(" return ");
            sb.Append(Lisp.Format(m_body));
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }       
}
