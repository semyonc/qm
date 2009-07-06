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
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public enum XQueryOrder
    {
        Default,
        Ordered,
        Unordered
    }

    class XQueryExpr : XQueryExprBase, Resolver
    {
        internal object[] m_expr;
        private SymbolLink[] m_compiledBody;
        private SymbolLink[] m_compiledAnnotation;        
        private Executive.Parameter[] m_parameter;
        private SymbolLink[] m_parameterValues;

        public XQueryOrder QueryOrder { get; set; }
        public object[] Annotation { get; set; }

        public XQueryExpr(XQueryContext context, object[] expr)
            : base(context)
        {
            m_expr = expr;
            m_compiledBody = null;
            QueryOrder = XQueryOrder.Default;
        }

        public XQueryExpr(XQueryContext context, object[] expr, object[] annotation)
            : this(context, expr)
        {
            Annotation = annotation;
        }

        private IEnumerable<XPathItem> CreateEnumerator(object[] parameters)
        {
            object[] oldParameters = null;
            if (parameters != null)
            {
                QueryContext.Engine.Enter(this);
                oldParameters = new object[m_parameterValues.Length];
                for (int k = 0; k < m_parameterValues.Length; k++)
                {
                    oldParameters[k] = m_parameterValues[k].Value;
                    m_parameterValues[k].Value = parameters[k];
                }
            }
            if (QueryOrder != XQueryOrder.Default)
                QueryContext.EnterOrdering(QueryOrder);
            if (m_compiledBody == null)
            {
                m_compiledBody = new SymbolLink[m_expr.Length];
                for (int k = 0; k < m_expr.Length; k++)
                {
                    m_compiledBody[k] = new SymbolLink();
                    QueryContext.Engine.Compile(null, m_expr[k], m_compiledBody[k]);
                }
                if (Annotation != null)
                {
                    m_compiledAnnotation = new SymbolLink[Annotation.Length];
                    for (int k = 0; k < Annotation.Length; k++)
                    {
                        m_compiledAnnotation[k] = new SymbolLink();
                        QueryContext.Engine.Compile(null, Annotation[k], m_compiledAnnotation[k]);
                    }
                }
            }
            object[] annotation;
            if (Annotation != null)
            {
                annotation = new object[Annotation.Length];                
                for (int k = 0; k < Annotation.Length; k++)
                {
                    object res = QueryContext.Engine.Apply(null, null,
                        Annotation[k], null, m_compiledAnnotation[k]);
                    if (res != Undefined.Value)
                        if (res == null)
                            annotation[k] = false;
                        else
                            annotation[k] = Core.Atomize(res);
                }                
            }
            else
                annotation = null;
            for (int k = 0; k < m_expr.Length; k++)
            {
                object res = QueryContext.Engine.Apply(null, null,
                    m_expr[k], null, m_compiledBody[k]);
                if (res != Undefined.Value)
                {
                    XQueryNodeIterator iter = res as XQueryNodeIterator;
                    if (iter != null)
                    {
                        foreach (XPathItem item in iter)
                        {
                            if (annotation != null)
                            {
                                XPathItem item2 = item.Clone();
                                item2.SetAnnotation(annotation);
                                yield return item2;
                            }
                            else
                                yield return item;
                        }
                    }
                    else
                    {
                        XPathItem item = res as XPathItem;
                        if (item == null)
                            item = QueryContext.CreateItem(res);
                        if (annotation != null)
                        {
                            XPathItem item2 = item.Clone();
                            item2.SetAnnotation(annotation);
                            yield return item2;
                        }
                        else
                            yield return item;
                    }
                }
            }
            if (QueryOrder != XQueryOrder.Default)
                QueryContext.LeaveOrdering();
            if (parameters != null)
            {
                QueryContext.Engine.Leave();
                for (int k = 0; k < m_parameterValues.Length; k++)
                    m_parameterValues[k].Value = oldParameters[k];
            }
        }

        public override XQueryNodeIterator Execute(object[] parameters)
        {
            object[] currentValues = null;
            if (QueryContext.Engine.CurrentLambda != null &&
                QueryContext.Engine.CurrentLambda.Arity > 0)
            {
                CompiledLambda lambda = QueryContext.Engine.CurrentLambda;
                if (m_parameter == null)
                {
                    m_parameter = lambda.Parameters;
                    m_parameterValues = new SymbolLink[lambda.Arity];
                    for (int k = 0; k < lambda.Arity; k++)
                        m_parameterValues[k] = new SymbolLink(lambda.Values[k].Type);                
                }
                currentValues = new object[lambda.Arity];
                for (int k = 0; k < lambda.Arity; k++)
                    currentValues[k] = lambda.Values[k].Value;
            }
            return new NodeIterator(CreateEnumerator(currentValues));
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            for (int k = 0; k < m_expr.Length; k++)
            {
                if (k > 0)
                    sb.Append(", ");
                sb.Append(Lisp.Format(m_expr[k]));
            }
            sb.Append("]");
            return sb.ToString();
        }
#endif

        #region Resolver Members

        public bool Get(object atom, out SymbolLink result)
        {
            if (m_parameter != null)
            {
                for (int k = 0; k < m_parameter.Length; k++)
                    if (m_parameter[k].ID == atom)
                    {
                        result = m_parameterValues[k];
                        return true;
                    }
            }
            result = null;
            return false;
        }

        #endregion
    }
}
