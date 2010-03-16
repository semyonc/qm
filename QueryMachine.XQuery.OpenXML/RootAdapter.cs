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
    class RootAdapter: NavigatorAdapter
    {
        protected OpenXmlDocument _doc;

        public RootAdapter(OpenXmlElement elem, OpenXmlDocument doc)
            : base(elem)
        {
            _doc = doc;
        }

        public override bool Equals(object obj)
        {
            RootAdapter other = obj as RootAdapter;
            if (other != null)
                return _doc == other._doc;
            return false;
        }

        public override int GetHashCode()
        {
            return _doc.Part.GetHashCode();
        }

        public override NavigatorAdapter Clone()
        {
            return this;
        }

        public override NavigatorAdapter MoveToParent()
        {
            return null;
        }

        public override NavigatorAdapter MoveToFirstAttribute()
        {
            return null;
        }

        public override NavigatorAdapter MoveToFirstChild()
        {
            return new ElementAdapter(_elem, this);
        }

        public override NavigatorAdapter MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            return null;
        }

        public override bool MoveToNext()
        {
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
            return false;
        }

        public OpenXmlDocument Document
        {
            get
            {
                return _doc;
            }
        }

        public override XmlNameTable NameTable
        {
            get 
            {
                return _doc.NameTable;
            }
        }

        public override bool IsEmptyElement
        {
            get 
            {
                return false;
            }
        }

        public override XPathNodeType NodeType
        {
            get 
            {
                return XPathNodeType.Root;
            }
        }

        public override string LocalName
        {
            get 
            {
                return String.Empty;
            }
        }

        public override string NamespaceURI
        {
            get
            {
                return String.Empty;
            }
        }

        public override string Prefix
        {
            get
            {
                return String.Empty;
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
