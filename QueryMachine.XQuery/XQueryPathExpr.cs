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
    internal class XQueryPathExpr: XQueryExprBase
    {
        private XQueryExprBase[] _stepExpr;        
        private bool _isOrdered;
        private bool _isOrderedSet;

        public XQueryExprBase LastStep
        {
            get
            {
                XQueryExprBase t = _stepExpr[_stepExpr.Length - 1];
                XQueryFilterExpr filter = t as XQueryFilterExpr;
                if (filter != null)
                    return filter.SourceExpr;
                return t;
            }
        }

        public XQueryPathExpr(XQueryContext context, XQueryExprBase[] stepExpr, bool isOrdered)
            : base(context)
        {
            _stepExpr = stepExpr;
            _isOrderedSet = IsOrderedSet();
            _isOrdered = isOrdered;
        }

        public override void Bind(Executive.Parameter[] parameters)
        {
            foreach (XQueryExprBase expr in _stepExpr)
                expr.Bind(parameters);
        }

        public override IEnumerable<SymbolLink> EnumDynamicFuncs()
        {
            List<SymbolLink> res = new List<SymbolLink>();
            foreach (XQueryExprBase expr in _stepExpr)
                res.AddRange(expr.EnumDynamicFuncs());
            return res;
        }

        public override object Execute(IContextProvider provider, object[] args)
        {
            XQueryNodeIterator rootIter = 
                XQueryNodeIterator.Create(_stepExpr[0].Execute(provider, args)).CreateBufferedIterator();
            return new ResultIterator(this, provider, _isOrderedSet, rootIter, args);
        }

        private bool IsOrderedSet()
        {
            for (int k = 1; k < _stepExpr.Length; k++)
            {
                XQueryStepExpr stepExpr;
                if (_stepExpr[k] is XQueryFilterExpr)
                    stepExpr = ((XQueryFilterExpr)_stepExpr[k]).SourceExpr as XQueryStepExpr;
                else
                    stepExpr = _stepExpr[k] as XQueryStepExpr;
                if (stepExpr == null)
                    return false;
                switch (stepExpr.ExprType)
                {
                    case XQueryPathExprType.Parent:
                    case XQueryPathExprType.Ancestor:
                    case XQueryPathExprType.AncestorOrSelf:
                    case XQueryPathExprType.Preceding:
                    case XQueryPathExprType.PrecedingSibling:
                        return false;

                    case XQueryPathExprType.Descendant:
                    case XQueryPathExprType.DescendantOrSelf:
                    case XQueryPathExprType.Following:
                        if (k < _stepExpr.Length - 1)
                            return false;
                        break;
                }
            }
            return true;
        }

        private bool MoveNext(object[] args, XQueryNodeIterator[] iter, int index)
        {
            if (iter[index] != null && iter[index].MoveNext())
                return true;
            else
                if (index > 0)
                {
                    while (MoveNext(args, iter, index - 1))
                    {
                        ContextProvider provider = new ContextProvider(iter[index - 1]);
                        if (!provider.Context.IsNode)
                            throw new XQueryException(Properties.Resources.XPTY0019, provider.Context.Value);
                        iter[index] = XQueryNodeIterator.Create(_stepExpr[index].Execute(provider, args));
                        if (iter[index].MoveNext())
                            return true;
                    }
                }
            return false;
        }

        private IEnumerator<XPathItem> Iterator(ResultIterator res)
        {
            XQueryNodeIterator[] iter = new XQueryNodeIterator[_stepExpr.Length];
            iter[0] = res.rootIter.Clone();
            if (MoveNext(res.args, iter, _stepExpr.Length - 1))
            {
                bool isNode = iter[_stepExpr.Length - 1].Current.IsNode;
                if (!isNode || res.orderedSet)
                    do
                    {
                        XPathItem curr = iter[_stepExpr.Length - 1].Current;
                        if (curr.IsNode != isNode)
                            throw new XQueryException(Properties.Resources.XPTY0018, curr.Value);
                        yield return curr;
                    }
                    while (MoveNext(res.args, iter, _stepExpr.Length - 1));
                else
                {
                    if (_isOrdered)
                    {
                        bool needSort = false;
                        XPathNavigator last_node;                        
                        if (res.buffer == null)
                        {
                            last_node = null;                        
                            res.buffer = new List<XPathItem>();
                            do
                            {
                                XPathItem item = iter[_stepExpr.Length - 1].Current;
                                if (!item.IsNode)
                                    throw new XQueryException(Properties.Resources.XPTY0018, item.Value);
                                XPathNavigator node = (XPathNavigator)item;
                                if (!needSort)
                                {
                                    if (last_node != null)
                                        needSort = last_node.ComparePosition(node) == XmlNodeOrder.Before;
                                }
                                last_node = node.Clone();
                                res.buffer.Add(last_node);
                            }
                            while (MoveNext(res.args, iter, _stepExpr.Length - 1));
                            if (needSort)
                                res.buffer.Sort(new XPathComparer());
                        }
                        last_node = null;
                        for (int k = 0; k < res.buffer.Count; k++)
                        {
                            XPathNavigator node = (XPathNavigator)res.buffer[k];
                            if (last_node != null && last_node.IsSamePosition(node))
                                continue;
                            yield return node;
                            last_node = node;
                        }
                    }
                    else
                    {
                        HashSet<XPathItem> hs = new HashSet<XPathItem>(new XPathNavigatorEqualityComparer());
                        do
                        {
                            XPathItem curr = iter[_stepExpr.Length - 1].Current;
                            if (!curr.IsNode)
                                throw new XQueryException(Properties.Resources.XPTY0018, curr.Value);
                            if (!hs.Contains(curr))
                            {
                                yield return curr;
                                hs.Add(curr.Clone());
                            }
                        }
                        while (MoveNext(res.args, iter, _stepExpr.Length - 1));
                    }
                }
            }
        }

        private sealed class ResultIterator : XQueryNodeIterator
        {
            private XQueryPathExpr owner;
            
            public List<XPathItem> buffer;
            public IContextProvider provider;
            public bool orderedSet;
            public XQueryNodeIterator rootIter;
            public object[] args;
            public IEnumerator<XPathItem> iter;

            public ResultIterator(XQueryPathExpr owner, IContextProvider provider, bool orderedSet, 
                XQueryNodeIterator rootIter, object[] args)
            {
                this.owner = owner;
                this.provider = provider;
                this.orderedSet = orderedSet;
                this.rootIter = rootIter;
                this.args = args;
            }

            [DebuggerStepThrough]
            public override XQueryNodeIterator Clone()
            {
                ResultIterator clone = new ResultIterator(owner, provider, orderedSet, rootIter, args);
                clone.buffer = buffer;
                return clone;
            }

            public override void Init()
            {
                iter = owner.Iterator(this);
            }

            public override XPathItem NextItem()
            {
                if (iter.MoveNext())
                    return iter.Current;
                return null;
            }

            public override XQueryNodeIterator CreateBufferedIterator()
            {
                return new BufferedNodeIterator(this);
            }
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            for (int k = 0; k < _stepExpr.Length; k++)
            {
                if (k > 0)
                    sb.Append(", ");
                sb.Append(_stepExpr[k].ToString());
            }
            sb.Append("]");
            return sb.ToString();
            
        }
#endif
    }
}
