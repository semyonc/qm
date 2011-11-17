//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;

namespace DataEngine.XQuery
{
    public static class XConverter
    {
        public static XObject ToXNode(XPathNavigator nav)
        {
            switch (nav.NodeType)
            {
                case XPathNodeType.Root:
                    {
                        XDocument doc = XDocument.Load(nav.ReadSubtree());
                        doc.AddFirst(new XDeclaration("1.0", "utf-8", null));
                        return doc;
                    }

                case XPathNodeType.Element:
                    return XElement.Load(nav.ReadSubtree());
                
                case XPathNodeType.Attribute:
                    return new XAttribute(XName.Get(nav.LocalName, nav.NamespaceURI), nav.Value);

                case XPathNodeType.Namespace:
                    {
                        if (nav.Name == "")
                            return new XAttribute("xmlns", nav.Value);
                        else
                            return new XAttribute(XNamespace.Xmlns + nav.Name, nav.Value);
                    }

                case XPathNodeType.Comment:
                    return new XComment(nav.Value);

                case XPathNodeType.Text:
                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                    return new XText(nav.Value);

                case XPathNodeType.ProcessingInstruction:
                    return new XProcessingInstruction(nav.Name, nav.Value);

                default:
                    throw new ArgumentException("nav");                    
            }
        }
    }
}
