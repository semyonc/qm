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
    abstract class AxisNodeIterator: XQueryNodeIterator
    {
        protected XQueryContext context;
        protected XmlQualifiedNameTest nameTest;
        protected XQuerySequenceType typeTest;
        protected bool matchSelf;
        protected XQueryNodeIterator iter;
        protected XPathNavigator curr;
        
        protected int sequentialPosition;
        protected bool accept;

        protected AxisNodeIterator()
        {
        }

        public AxisNodeIterator(XQueryContext context, object nodeTest, bool matchSelf, XQueryNodeIterator iter)
        {
            this.context = context;
            if (nodeTest is XmlQualifiedNameTest)
                nameTest = (XmlQualifiedNameTest)nodeTest;
            else if (nodeTest is XQuerySequenceType && nodeTest != XQuerySequenceType.Node)
                typeTest = (XQuerySequenceType)nodeTest;
            this.matchSelf = matchSelf;
            this.iter = iter;
        }

        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return new BufferedNodeIterator(this);
        }

        protected void AssignFrom(AxisNodeIterator src)
        {
            context = src.context;
            typeTest = src.typeTest;
            nameTest = src.nameTest;
            matchSelf = src.matchSelf;
            iter = src.iter.Clone();
        }

        protected bool TestItem()
        {
            if (nameTest != null)
            {
                return (curr.NodeType == XPathNodeType.Element || curr.NodeType == XPathNodeType.Attribute) &&
                    (nameTest.IsNamespaceWildcard || context.StringEquals(nameTest.Namespace, curr.NamespaceURI)) &&
                    (nameTest.IsNameWildcard || context.StringEquals(nameTest.Name, curr.LocalName));
            }
            else
                if (typeTest != null)
                    return typeTest.Match(curr, context);
            return true;
        }

        protected bool MoveNextIter()
        {
            if (!iter.MoveNext())
                return false;
            XPathNavigator nav = iter.Current as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0019, iter.Current.Value);
            if (curr == null || !curr.MoveTo(nav))
                curr = nav.Clone();
            sequentialPosition = 0;
            accept = true;
            return true;
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
