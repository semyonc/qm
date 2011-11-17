//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Xml;
using System.Xml.XPath;

namespace DataEngine.XQuery
{
    public sealed class XmlNodeListIterator : XQueryNodeIterator
    {
        private XmlNodeList m_nodeList;

        public XmlNodeListIterator(XmlNodeList nodeList)
        {
            m_nodeList = nodeList;
        }

        public override XQueryNodeIterator Clone()
        {
            return new XmlNodeListIterator(m_nodeList);
        }

        protected override XPathItem NextItem()
        {
            int index = CurrentPosition + 1;
            if (index < m_nodeList.Count)
                m_nodeList.Item(index).CreateNavigator();
            return null;
        }

        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return Clone();
        }
    }
}
