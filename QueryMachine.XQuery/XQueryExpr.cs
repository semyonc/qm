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

    sealed class XQueryExpr : XQueryExprBase
    {
        internal object[] m_expr;
        internal FunctionLink[] m_compiledBody;
        private FunctionLink[] m_compiledAnnotation;        
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

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            m_context = new SymbolLink(typeof(IContextProvider));            
            pool.Bind(m_context);
            object data = QueryContext.Resolver.GetCurrentStack();
            QueryContext.Resolver.SetValue(ID.Context, m_context);
            m_compiledBody = new FunctionLink[m_expr.Length];
            for (int k = 0; k < m_expr.Length; k++)
            {
                m_compiledBody[k] = new FunctionLink();
                QueryContext.Engine.Compile(parameters, m_expr[k], m_compiledBody[k]);
            }
            if (Annotation != null)
            {
                m_compiledAnnotation = new FunctionLink[Annotation.Length];
                for (int k = 0; k < Annotation.Length; k++)
                {
                    m_compiledAnnotation[k] = new FunctionLink();
                    QueryContext.Engine.Compile(parameters, Annotation[k], m_compiledAnnotation[k]);
                }
            }
            QueryContext.Resolver.RevertToStack(data);
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            List<FunctionLink> res = new List<FunctionLink>();
            res.AddRange(m_compiledBody);
            if (m_compiledAnnotation != null)
                res.AddRange(m_compiledAnnotation);
            return res;
        }

        public override bool IsContextSensitive(Executive.Parameter[] parameters)
        {
            for (int k = 0; k < m_expr.Length; k++)
            {
                SymbolLink[] dps = QueryContext.Engine.GetValueDependences(null, parameters,
                    m_expr[k], m_compiledBody[k], false);
                foreach (SymbolLink s in dps)
                    if (s == m_context)
                        return true;
            }
            if (Annotation != null)
            {
                for (int k = 0; k < Annotation.Length; k++)
                {
                    SymbolLink[] dps = QueryContext.Engine.GetValueDependences(null, parameters,
                       Annotation[k], m_compiledAnnotation[k], false);
                    foreach (SymbolLink s in dps)
                        if (s == m_context)
                            return true;
                }
            }
            return false;
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            pool.SetData(m_context, provider);
            if (Annotation != null)
            {
                object[] annotation = new object[Annotation.Length];
                for (int k = 0; k < Annotation.Length; k++)
                {
                    object res = QueryContext.Engine.Apply(null, null,
                        Annotation[k], args, m_compiledAnnotation[k], pool);
                    if (res != Undefined.Value)
                        if (res == null)
                            annotation[k] = false;
                        else
                            annotation[k] = Core.Atomize(res);
                }
                return new XQueryExprIterator(this, args, annotation, pool);
            }
            if (m_expr.Length == 1)
                return QueryContext.Engine.Apply(null, null, m_expr[0], args, m_compiledBody[0], pool);
            return new XQueryExprIterator(this, args, null, pool);
        }

        public override object ToLispFunction()
        {
            if (m_expr.Length == 1)
                return m_expr[0];
            return base.ToLispFunction();
        }

        private sealed class XQueryExprIterator : XQueryNodeIterator
        {
            private XQueryExpr owner;
            private object[] args;
            private MemoryPool pool;
            private XQueryNodeIterator childIter;
            private object[] annotation;
            private int index;

            private XQueryItem currItem = new XQueryItem();

            public XQueryExprIterator(XQueryExpr owner, object[] args, object[] annotation, MemoryPool pool)
            {
                this.owner = owner;
                this.args = args;
                this.annotation = annotation;
                this.pool = pool;
            }

            public override XQueryNodeIterator Clone()
            {
                return new XQueryExprIterator(owner, args, annotation, pool);
            }

            public override XQueryNodeIterator CreateBufferedIterator()
            {
                return new BufferedNodeIterator(this);
            }

            protected override void Init()
            {
                index = 0;
            }

            protected override XPathItem NextItem()
            {
                while (true)
                {
                    if (childIter != null)
                    {
                        if (childIter.MoveNext())
                        {
                            if (annotation != null)
                                return new XQueryWrappedValue(childIter.Current, annotation);
                            else
                                return childIter.Current;
                        }
                        else
                            childIter = null;
                    }
                    if (index == owner.m_expr.Length)
                        return null;
                    object res = owner.QueryContext.Engine.Apply(null, null,
                        owner.m_expr[index], args, owner.m_compiledBody[index], pool);
                    index++;
                    if (res != Undefined.Value)
                    {
                        childIter = res as XQueryNodeIterator;
                        if (childIter == null)
                        {
                            XPathItem item = res as XPathItem;
                            if (item == null)
                            {
                                item = currItem;
                                currItem.RawValue = res;
                            }
                            if (annotation != null)
                                return new XQueryWrappedValue(item, annotation);
                            else
                                return item;
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

        public static XQueryExprBase Create(XQueryContext context, object[] expr)
        {
            if (expr.Length == 1 && Lisp.IsFunctor(expr[0], ID.DynExecuteExpr, 4))
              return (XQueryExprBase)Lisp.Arg1(expr[0]);
            return new XQueryExpr(context, expr);
        }
    }
}
