using System;
using System.Xml;
using System.Xml.XPath;

namespace DataEngine.XQuery
{
    internal sealed class XmlNodeListIterator : XQueryNodeIterator
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
