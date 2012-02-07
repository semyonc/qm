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
    sealed class SpecialDescendantNodeIterator: AxisNodeIterator
    {
        private XPathNodeType kind;

        public SpecialDescendantNodeIterator(XQueryContext context, object nodeTest, bool matchSelf, XQueryNodeIterator iter)
            : base(context, nodeTest, matchSelf, iter)
        {
            kind = XPathNodeType.All;
            if (nameTest != null ||
                 (typeTest != null && typeTest.GetNodeKind() == XPathNodeType.Element))
                kind = XPathNodeType.Element;
        }

        private SpecialDescendantNodeIterator(SpecialDescendantNodeIterator src)
        {
            AssignFrom(src);
            kind = src.kind;
        }

        public override XQueryNodeIterator Clone()
        {
            return new SpecialDescendantNodeIterator(this);
        }

        private int depth;

        protected override XPathItem NextItem()
        {
        MoveNextIter:
            if (!accept)
            {
                if (!MoveNextIter())
                    return null;
                if (matchSelf && TestItem())
                {
                    sequentialPosition++;
                    return curr;
                }
            }

        MoveToFirstChild:
            if (curr.MoveToChild(kind))
            {
                depth++;
                goto TestItem;
            }

        MoveToNext:
            if (depth == 0)
            {
                accept = false;
                goto MoveNextIter;
            }
            if (!curr.MoveToNext(kind))
            {
                curr.MoveToParent();
                depth--;
                goto MoveToNext;
            }

        TestItem:
            if (!TestItem())
                goto MoveToFirstChild;
            sequentialPosition++;
            return curr;
        }
    }
}
