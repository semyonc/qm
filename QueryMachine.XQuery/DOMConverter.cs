//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

using DataEngine.XQuery;

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
