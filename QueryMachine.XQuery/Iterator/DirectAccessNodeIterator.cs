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
using DataEngine.XQuery.DocumentModel;
using DataEngine.XQuery.MS;

namespace DataEngine.XQuery.Iterator
{
    class DirectAccessNodeIterator: ExprNodeIterator
    {
        protected DirectAccessNodeIterator()
        {
        }

        public DirectAccessNodeIterator(XQueryPathExpr expr, object[] args, MemoryPool pool, XQueryNodeIterator baseIter)
            : base(expr, args, pool, baseIter)
        {
        }

        public override XQueryNodeIterator Clone()
        {
            DirectAccessNodeIterator res = new DirectAccessNodeIterator();
            res.AssignForm(this);            
            return res;
        }

        protected override XQueryNodeIterator Eval(XQueryExprBase expr, IContextProvider provider, object[] args, MemoryPool pool)
        {
            XQueryNavigator nav = provider.Context as XQueryNavigator;
            if (nav == null || (nav.NodeType != XPathNodeType.Root && nav.NodeType != XPathNodeType.Element))
                return base.Eval(expr, provider, args, pool);
            return new NodeIterator(Iterator((XQueryPathExpr)expr, (XQueryNavigator)nav.Clone()));
        }

        private IEnumerable<XPathItem> Iterator(XQueryPathExpr path, XQueryNavigator src)
        {
            int inf;
            int sup;
            int bound;
            bool buffered;
            XQueryDocument doc = src.Document;
            PageFile pf = doc.pagefile;
            int index = src.GetBegin();
            DmNode node = src.DmNode;
            int[] buffer = new int[XQueryLimits.DirectAcessBufferSize];
            XQueryNavigator res = (XQueryNavigator)src.Clone();
            do
            {
                buffered = src.IsCompleted;
                if (!buffered)
                {
                    if (index == pf.Count)
                    {
                        src.Document.ExpandUtilElementEnd(src.Position, XQueryLimits.DirectAccessDelta);
                        buffered = src.IsCompleted;
                    }
                }
                if (buffered)
                    bound = src.Position + src.GetEnd() - 1;
                else
                    bound = pf.Count - 1;
                NodeSet nset;
                lock (pf)
                {
                    nset = node.GetNodeSet(this);
                    if (nset == null)
                        nset = node.CreateNodeSet(this, path);
                    nset.GetBounds(out inf, out sup);
                }
                if (index < inf)
                    index = inf;
                int length = Math.Min(bound - index, sup - inf) + 1;
                while (length > 0)
                {
                    int count = pf.Select(nset.hindex, ref index, ref length, buffer);
                    for (int k = 0; k < count; k++)
                    {
                        res.Position = buffer[k];
                        yield return res;
                    }
                }
            }
            while (!buffered);
        }
    }
}
