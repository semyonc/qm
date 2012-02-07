//        Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using DataEngine.CoreServices;

namespace DataEngine.XQuery.Iterator
{
    class ExprNodeIterator : XQueryNodeIterator
    {
        private XQueryExprBase expr;
        private object[] args;
        private MemoryPool pool;
        private XQueryNodeIterator baseIter;
        private XQueryNodeIterator iter;
        private int sequentialPosition;
        private IContextProvider provider;

        protected ExprNodeIterator()
        {
        }

        public ExprNodeIterator(XQueryExprBase expr, object[] args, MemoryPool pool, XQueryNodeIterator baseIter)
        {
            this.expr = expr;
            this.args = args;
            this.pool = pool;
            this.baseIter = baseIter;
        }

        protected void AssignForm(ExprNodeIterator src)
        {
            expr = src.expr;
            args = src.args;
            pool = src.pool;
            baseIter = src.baseIter.Clone();
        }

        public override XQueryNodeIterator Clone()
        {
            ExprNodeIterator res = new ExprNodeIterator();
            res.AssignForm(this);            
            return res;
        }

        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return new BufferedNodeIterator(this);
        }

        protected override void Init()
        {
            provider = new ContextProvider(baseIter);
        }

        protected virtual XQueryNodeIterator Eval(XQueryExprBase expr, IContextProvider provider, object[] args, MemoryPool pool)
        {
            return XQueryNodeIterator.Create(expr.Execute(provider, args, pool));
        }

        protected override XPathItem NextItem()
        {
            while (true)
            {
                if (iter == null)
                {
                    if (!baseIter.MoveNext())
                        return null;
                    sequentialPosition = 0;
                    if (!baseIter.Current.IsNode)
                        throw new XQueryException(Properties.Resources.XPTY0019, baseIter.Current.Value);
                    iter = Eval(expr, provider, args, pool);
                }
                if (iter.MoveNext())
                {
                    sequentialPosition++;
                    return iter.Current;
                }
                iter = null;
            }
        }

        public override int SequentialPosition
        {
            get
            {
                return sequentialPosition;
            }
        }

        public override void ResetSequentialPosition()
        {
            iter = null;
        }
    }

}
