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
using System.Text;
using System.Collections;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace DataEngine.XQuery.DocumentModel
{
    internal struct DmQName
    {
        public string prefix;
        public string localName;
        public string ns;

        public DmQName(string prefix, string localName, string ns, XmlNameTable nameTable)
        {
            this.prefix = nameTable.Add(prefix);
            this.localName = nameTable.Add(localName);
            this.ns = nameTable.Add(ns);
        }

        public override int GetHashCode()
        {
            int hashCode = localName.GetHashCode();
            hashCode = (hashCode << 7) ^ prefix.GetHashCode();
            hashCode = (hashCode << 7) ^ ns.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {            
            if (obj is DmQName)
            {
                DmQName other = (DmQName)obj;
                return Object.ReferenceEquals(localName, other.localName) &&
                    Object.ReferenceEquals(prefix, other.prefix) &&
                    Object.ReferenceEquals(ns, other.ns);
            }
            return false;
        }
    }

    abstract class DmContainer: DmNode
    {
        private Dictionary<DmQName, DmNode> _childs;

        private DmComment _comment;
        private DmText _text;
        private DmWhitespace _whitespace;

        public DmNode CreateChild(DmQName name)
        {
            if (_childs == null)
                _childs = new Dictionary<DmQName, DmNode>();
            DmNode node;
            if (_childs.TryGetValue(name, out node))
                return node;
            node = new DmElement(name);
            node._parent = this;
            _childs.Add(name, node);
            return node;
        }

        public DmNode CreateChildPI(string name, XmlNameTable nameTable)
        {
            DmQName id = new DmQName(String.Empty, name, DmPI.InternalNs, nameTable);
            if (_childs == null)
                _childs = new Dictionary<DmQName, DmNode>();
            DmNode node;
            if (_childs.TryGetValue(id, out node))
                return node;
            node = new DmPI(id.localName);
            node._parent = this;
            _childs.Add(id, node);
            return node;
        }

        public DmNode CreateChildText()
        {
            if (_text == null)
                _text = new DmText(this);
            return _text;
        }

        public DmNode CreateChildComment()
        {
            if (_comment == null)
                _comment = new DmComment(this);
            return _comment;
        }

        public DmNode CreateChildWhitespace()
        {
            if (_whitespace == null)
                _whitespace = new DmWhitespace(this);
            return _whitespace;
        }

        public override bool HasChildNodes
        {
            get
            {
                return _childs != null;
            }
        }

        public override DmNodeList ChildNodes
        {
            get
            {
                if (_childs == null)
                    return null;
                return new DmNodeList(_childs.Values);
            }
        }

        public DmText ChildText
        {
            get
            {
                return _text;
            }
        }

        public DmComment ChildComment
        {
            get
            {
                return _comment;
            }
        }

        public DmWhitespace ChildWhitespace
        {
            get
            {
                return _whitespace;
            }
        }
    }
}
