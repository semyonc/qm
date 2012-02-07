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

namespace DataEngine.XQuery.Iterator
{
    sealed class PrecedingNodeIterator : AxisNodeIterator
    {
        private XPathNodeType kind;

        public PrecedingNodeIterator(XQueryContext context, object nodeTest, XQueryNodeIterator iter)
            : base(context, nodeTest, false, iter)
        {
            if (typeTest == null)
            {
                if (nameTest == null)
                    kind = XPathNodeType.All;
                else
                    kind = XPathNodeType.Element;
            }
            else
                kind = typeTest.GetNodeKind();
        }

        private PrecedingNodeIterator(PrecedingNodeIterator src)
        {
            AssignFrom(src);
            kind = src.kind;
        }

        public override XQueryNodeIterator Clone()
        {
            return new PrecedingNodeIterator(this);
        }

        private XPathNavigator nav = null;

        protected override XPathItem NextItem()
        {
            while (true)
            {
                if (!accept)
                {
                    if (!MoveNextIter())
                        return null;
                    nav = curr.Clone();
                    curr.MoveToRoot();
                }
                accept = curr.MoveToFollowing(kind, nav);
                if (accept)
                {
                    if (TestItem())
                    {
                        sequentialPosition++;
                        return curr;
                    }
                }
            } 
        }
    }
}
