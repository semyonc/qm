//        Copyright (c) 2009-2010, Semyon A. Chertkov (semyonc@gmail.com)
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

using System.Xml;
using System.Xml.XPath;
using DocumentFormat.OpenXml;

namespace DataEngine.XQuery.OpenXML
{
    class ElementAdapter: NavigatorAdapter
    {
        protected RootAdapter _root;

        public ElementAdapter(OpenXmlElement elem, RootAdapter root)
            : base(elem)
        {
            _root = root;
        }

        public override bool Equals(object obj)
        {
            ElementAdapter other = obj as ElementAdapter;
            if (other != null)
                return _elem == other._elem;
            return false;
        }

        public override int GetHashCode()
        {
            return _elem.GetHashCode();
        }

        public override NavigatorAdapter Clone()
        {
            return new ElementAdapter(_elem, _root);
        }

        public override NavigatorAdapter MoveToParent()
        {
            if (_elem.Parent == null)
                return _root;
            else
            {
                _elem = _elem.Parent;
                return this;
            }
        }

        public override NavigatorAdapter MoveToFirstAttribute()
        {
            if (_elem.HasAttributes)
                return new AttributeAdapter(_elem.GetAttributes(), this).MoveToFirstAttribute();
            return null;
        }

        public override NavigatorAdapter MoveToFirstChild()
        {
            if (_elem is OpenXmlLeafTextElement)
                return new LeafTextElementAdapter(_elem, this);
            if (_elem.HasChildren)
            {
                _elem = _elem.FirstChild;
                return this;
            }
            return null;
        }

        public override NavigatorAdapter MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            NavigatorAdapter adapter = new NamespaceAdapter(_elem.NamespaceDeclarations, this);
            return adapter.MoveToFirstNamespace(namespaceScope);
        }

        public override bool MoveToNext()
        {
            OpenXmlElement nextSibling = _elem.NextSibling();
            if (nextSibling != null)
            {
                _elem = nextSibling;
                return true;
            }
            return false;
        }

        public override bool MoveToNextAttribute()
        {
            return false;
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        public override bool MoveToPrevious()
        {
            OpenXmlElement previosSibling = _elem.PreviousSibling();
            if (previosSibling != null)
            {
                _elem = previosSibling;
                return true;
            }
            return false;
        }

        public override XmlNameTable NameTable
        {
            get
            {
                return _root.NameTable;
            }
        }

        public override bool IsEmptyElement
        {
            get 
            {
                if (_elem is OpenXmlLeafTextElement)
                    return false;
                return !_elem.HasChildren;
            }
        }

        public override XPathNodeType NodeType
        {
            get 
            {
                OpenXmlMiscNode misc = _elem as OpenXmlMiscNode;
                if (misc != null)
                    switch (misc.XmlNodeType)
                    {
                        case XmlNodeType.CDATA:
                        case XmlNodeType.Text:
                            return XPathNodeType.Text;
                        case XmlNodeType.Whitespace:
                            return XPathNodeType.Whitespace;
                        case XmlNodeType.SignificantWhitespace:
                            return XPathNodeType.SignificantWhitespace;
                        case XmlNodeType.Comment:
                            return XPathNodeType.Comment;
                        case XmlNodeType.ProcessingInstruction:
                            return XPathNodeType.ProcessingInstruction;                                                
                    }
                return XPathNodeType.Element;
            }
        }

        public override string LocalName
        {
            get 
            {
                return _elem.LocalName;
            }
        }

        public override string NamespaceURI
        {
            get 
            {
                return _elem.NamespaceUri;
            }
        }

        public override string Prefix
        {
            get 
            {
                return _elem.Prefix;
            }
        }

        public override string Value
        {
            get 
            {
                return _elem.InnerText;
            }
        }
    }
}
