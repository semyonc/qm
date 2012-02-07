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

using DataEngine.XQuery.MS;

namespace DataEngine.XQuery.Iterator
{
    sealed class ChildOverDescendantsNodeIterator: XQueryNodeIterator
    {
        public class NodeTest
        {
            public XmlQualifiedNameTest nameTest;
            public XQuerySequenceType typeTest;

            public NodeTest(object test)
            {
                if (test is XmlQualifiedNameTest)
                    nameTest = (XmlQualifiedNameTest)test;
                else if (test is XQuerySequenceType && test != XQuerySequenceType.Node)
                    typeTest = (XQuerySequenceType)test;
            }
        }

        private XPathNodeType kind;
        private XQueryContext context;
        private NodeTest[] nodeTest;
        private NodeTest lastTest;
        private XQueryNodeIterator iter;
        private XPathNavigator curr;

        public ChildOverDescendantsNodeIterator(XQueryContext context, NodeTest[] nodeTest, XQueryNodeIterator iter)
        {
            this.context = context;
            this.nodeTest = nodeTest;
            this.iter = iter;         
            lastTest = nodeTest[nodeTest.Length - 1];
            kind = XPathNodeType.All;
            if (lastTest.nameTest != null ||
                 (lastTest.typeTest != null && lastTest.typeTest.GetNodeKind() == XPathNodeType.Element))
                kind = XPathNodeType.Element;
        }

        private ChildOverDescendantsNodeIterator(ChildOverDescendantsNodeIterator src)
        {
            context = src.context;
            nodeTest = src.nodeTest;
            iter = src.iter.Clone();
            lastTest = src.lastTest;
            kind = src.kind;
        }

        public override XQueryNodeIterator Clone()
        {
            return new ChildOverDescendantsNodeIterator(this);
        }

        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return new BufferedNodeIterator(Clone());
        }

        private bool TestItem(XPathNavigator nav, NodeTest nodeTest)
        {
            XmlQualifiedNameTest nameTest = nodeTest.nameTest;
            XQuerySequenceType typeTest = nodeTest.typeTest;
            if (nameTest != null)
            {

                return (nav.NodeType == XPathNodeType.Element || nav.NodeType == XPathNodeType.Attribute) &&
                    (nameTest.IsNamespaceWildcard || context.StringEquals(nameTest.Namespace, nav.NamespaceURI)) &&
                    (nameTest.IsNameWildcard || context.StringEquals(nameTest.Name, nav.LocalName));
            }
            else
                if (typeTest != null)
                    return typeTest.Match(nav, context);
            return true;
        }

        private int depth;
        private bool accept;
        private XPathNavigator nav;
        private int sequentialPosition;

        protected override XPathItem NextItem()
        {
        MoveNextIter:
            if (!accept)
            {
                if (!iter.MoveNext())
                    return null;
                XPathNavigator current = iter.Current as XPathNavigator;
                if (current == null)
                    throw new XQueryException(Properties.Resources.XPTY0019, iter.Current.Value);
                if (curr == null || !curr.MoveTo(current))
                    curr = current.Clone();
                sequentialPosition = 0;
                accept = true;
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
            if (depth < nodeTest.Length || !TestItem(curr, lastTest))
                goto MoveToFirstChild;
            if (nav == null || !nav.MoveTo(curr))
                nav = curr.Clone();            
            for (int k = nodeTest.Length - 2; k >= 0; k--)
                if (!(nav.MoveToParent() && TestItem(nav, nodeTest[k])))
                    goto MoveToFirstChild;
            sequentialPosition++;
            return curr;
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
            accept = false;
        }
    }
}
