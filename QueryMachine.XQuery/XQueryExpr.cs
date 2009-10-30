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

    class XQueryExpr : XQueryExprBase
    {
        internal object[] m_expr;
        internal SymbolLink[] m_compiledBody;
        private SymbolLink[] m_compiledAnnotation;        
        private SymbolLink m_context;

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

        private IEnumerable<XPathItem> CreateEnumerator(object[] args)
        {
            object[] annotation;
            if (Annotation != null)
            {
                annotation = new object[Annotation.Length];                
                for (int k = 0; k < Annotation.Length; k++)
                {
                    object res = QueryContext.Engine.Apply(null, null,
                        Annotation[k], args, m_compiledAnnotation[k]);
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
                    m_expr[k], args, m_compiledBody[k]);
                if (res != Undefined.Value)
                {
                    XQueryNodeIterator iter = res as XQueryNodeIterator;
                    if (iter != null)
                    {
                        iter = iter.Clone();
                        while (iter.MoveNext())
                        {
                            if (annotation != null)
                                yield return new XQueryWrappedValue(iter.Current, annotation);
                            else
                                yield return iter.Current;
                        }
                    }
                    else
                    {
                        XPathItem item = res as XPathItem;
                        if (item == null)
                            item = QueryContext.CreateItem(res);
                        if (annotation != null)
                            yield return new XQueryWrappedValue(item, annotation);
                        else
                            yield return item;
                    }
                }
            }
        }

        public override void Bind(Executive.Parameter[] parameters)
        {
            m_context = new SymbolLink(typeof(IContextProvider));
            QueryContext.Resolver.SetValue(ID.Context, m_context);
            m_compiledBody = new SymbolLink[m_expr.Length];
            for (int k = 0; k < m_expr.Length; k++)
            {
                m_compiledBody[k] = new SymbolLink();
                QueryContext.Engine.Compile(parameters, m_expr[k], m_compiledBody[k]);
            }
            if (Annotation != null)
            {
                m_compiledAnnotation = new SymbolLink[Annotation.Length];
                for (int k = 0; k < Annotation.Length; k++)
                {
                    m_compiledAnnotation[k] = new SymbolLink();
                    QueryContext.Engine.Compile(parameters, Annotation[k], m_compiledAnnotation[k]);
                }
            }            
        }

        public override IEnumerable<SymbolLink> EnumDynamicFuncs()
        {
            return m_compiledBody;
        }

        public override XQueryNodeIterator Execute(IContextProvider provider, object[] args)
        {
            m_context.Value = provider;
            return new NodeIterator(CreateEnumerator(args));
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            if (m_expr.Length == 0)
                sb.Append("()");
            else
            {
                for (int k = 0; k < m_expr.Length; k++)
                {
                    if (k > 0)
                        sb.Append(", ");
                    sb.Append(Lisp.Format(m_expr[k]));
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }
}
