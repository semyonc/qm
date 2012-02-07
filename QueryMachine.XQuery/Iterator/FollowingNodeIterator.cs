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
    sealed class FollowingNodeIterator : AxisNodeIterator
    {
        private XPathNodeType kind;

        public FollowingNodeIterator(XQueryContext context, object nodeTest, XQueryNodeIterator iter)
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

        private FollowingNodeIterator(FollowingNodeIterator src)
        {
            AssignFrom(src);
            kind = src.kind;
        }

        public override XQueryNodeIterator Clone()
        {
            return new FollowingNodeIterator(this);
        }


        protected override XPathItem NextItem()
        {
            while (true)
            {
                if (!accept)
                {
                    if (!MoveNextIter())
                        return null;
                }
                accept = curr.MoveToFollowing(kind);
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
