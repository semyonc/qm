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
    abstract class NavigatorAdapter
    {                
        protected OpenXmlElement _elem;

        public NavigatorAdapter(OpenXmlElement elem)
        {
            _elem = elem;
        }

        public abstract NavigatorAdapter Clone();

        public abstract NavigatorAdapter MoveToParent();
        public abstract NavigatorAdapter MoveToFirstAttribute();
        public abstract NavigatorAdapter MoveToFirstChild();
        public abstract NavigatorAdapter MoveToFirstNamespace(XPathNamespaceScope namespaceScope);        
        public abstract bool MoveToNext();
        public abstract bool MoveToNextAttribute();
        public abstract bool MoveToNextNamespace(XPathNamespaceScope namespaceScope);
        public abstract bool MoveToPrevious();

        public abstract bool IsEmptyElement { get; }
        public abstract XPathNodeType NodeType { get; }
        public abstract string LocalName { get; }
        public abstract string NamespaceURI { get; }
        public abstract string Prefix { get; }
        public abstract string Value { get; }
        public abstract XmlNameTable NameTable { get; }
        
        public OpenXmlElement Element
        {
            get
            {
                return _elem;
            }
        }

        public int Depth
        {
            get
            {
                int num = 0;
                for (NavigatorAdapter curr = Clone().MoveToParent(); curr != null; curr = curr.MoveToParent())
                    num++;
                return num;
            }
        }
    }
}
