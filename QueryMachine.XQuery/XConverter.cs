//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
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
