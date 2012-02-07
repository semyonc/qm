//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
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

using DataEngine.XQuery;
using DataEngine.XQuery.MS;

namespace DataEngine.CoreServices
{
    public class DOMConverter
    {
        XmlDocument _carrier;

        public DOMConverter()
        {
            _carrier = new XmlDocument();
        }

        public DOMConverter(XmlDocument carrier)
        {
            _carrier = carrier;
        }

        public XmlNode ToXmlNode(XPathNavigator nav)
        {
            IHasXmlNode hasNode = nav as IHasXmlNode;
            if (hasNode != null)
            {
                XmlNode node = hasNode.GetNode();
                if (node != null)
                {
                    if (node.OwnerDocument != _carrier)
                        node = _carrier.ImportNode(node, true);
                    return node;
                }
            }
            switch (nav.NodeType)
            {
                case XPathNodeType.Root:
                    {
                        XmlDocument doc = new XmlDocument(nav.NameTable);
                        doc.Load(nav.ReadSubtree());
                        doc.InsertBefore(doc.CreateXmlDeclaration("1.0", "utf-8", null),
                            doc.FirstChild);
                        return doc;
                    }

                case XPathNodeType.Element:
                    {
                        XmlElement tmp = _carrier.CreateElement("dummy");
                        XPathNavigator dest = tmp.CreateNavigator();
                        dest.AppendChild(nav.ReadSubtree());
                        return tmp.FirstChild;
                    }

                case XPathNodeType.Attribute:
                    {
                        XmlAttribute attr = _carrier.CreateAttribute(nav.Prefix, nav.LocalName, nav.NamespaceURI);
                        attr.Value = nav.Value;
                        return attr;
                    }

                case XPathNodeType.Namespace:
                    {
                        XmlAttribute ns;
                        if (nav.Name == "")
                            ns = _carrier.CreateAttribute(String.Empty, "xmlns", XmlReservedNs.NsXmlNs);
                        else
                            ns = _carrier.CreateAttribute("xmlns", nav.LocalName, XmlReservedNs.NsXmlNs);
                        ns.Value = nav.Value;
                        return ns;
                    }

                case XPathNodeType.Comment:
                    return _carrier.CreateComment(nav.Value);

                case XPathNodeType.Text:
                    return _carrier.CreateTextNode(nav.Value);

                case XPathNodeType.SignificantWhitespace:
                    return _carrier.CreateSignificantWhitespace(nav.Value);

                case XPathNodeType.Whitespace:
                    return _carrier.CreateWhitespace(nav.Value);

                case XPathNodeType.ProcessingInstruction:
                    return _carrier.CreateProcessingInstruction(nav.Name, nav.Value);

                default:
                    throw new ArgumentException("nav");                    
            }
        }

        public XmlNode ToXmlNode(XPathItem item)
        {
            if (item.IsNode)
                return ToXmlNode((XPathNavigator)item);
            else
                return _carrier.CreateTextNode(item.Value);
        }
    }
}
