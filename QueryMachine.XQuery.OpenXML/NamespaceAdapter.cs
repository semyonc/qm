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
using System.Linq;

using System.Xml;
using System.Xml.XPath;
using DocumentFormat.OpenXml;

namespace DataEngine.XQuery.OpenXML
{
    class NamespaceAdapter: NavigatorAdapter
    {
        protected NavigatorAdapter _parent;
        protected KeyValuePair<string, string>[] _ns;
        protected int _index;

        public NamespaceAdapter(IEnumerable<KeyValuePair<string, string>> ns, NavigatorAdapter parent)
            : base(parent.Element)
        {
            _index = -1;
            _ns = ns.ToArray();
            _parent = parent;
        }

        private NamespaceAdapter(int index, KeyValuePair<string, string>[] ns, NavigatorAdapter parent)
            : base(parent.Element)
        {
            _index = index;
            _ns = ns;
            _parent = parent;
        }

        public override bool Equals(object obj)
        {
            NamespaceAdapter other = obj as NamespaceAdapter;
            if (other != null)
                return _elem == other._elem && _index == other._index;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override NavigatorAdapter Clone()
        {
            return new NamespaceAdapter(_index, _ns, _parent);
        }

        public override NavigatorAdapter MoveToParent()
        {
            return _parent;
        }

        public override NavigatorAdapter MoveToFirstAttribute()
        {
            return null;
        }

        public override NavigatorAdapter MoveToFirstChild()
        {
            return null;
        }

        public override NavigatorAdapter MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            if (namespaceScope != XPathNamespaceScope.Local)
                throw new NotImplementedException();
            _index = 0;
            if (_index < _ns.Length)
                return this;
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
            if (namespaceScope != XPathNamespaceScope.Local)
                throw new NotImplementedException();
            if (_index == -1)
                throw new InvalidOperationException();
            if (_index + 1 < _ns.Length)
            {
                _index++;
                return true;
            }
            return false;
        }

        public override bool MoveToPrevious()
        {
            return false;
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
                return XPathNodeType.Namespace;
            }
        }

        public override string LocalName
        {
            get 
            {
                return _ns[_index].Key;
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
                return _ns[_index].Value;
            }
        }

        public override XmlNameTable NameTable
        {
            get 
            {
                return _parent.NameTable;
            }
        }
    }
}
