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
    internal class DmElement : DmContainer
    {
        private DmQName _name;
        private IXmlSchemaInfo _schemaInfo;

        private Dictionary<DmQName, DmNode> _attributes;

        public DmElement(DmQName name)
        {
            _name = name;
        }

        public override XPathNodeType NodeType
        {
            get
            {
                return XPathNodeType.Element;
            }
        }

        public DmNode CreateAttribute(DmQName name)
        {
            if (_attributes == null)
                _attributes = new Dictionary<DmQName, DmNode>();
            DmNode node;
            if (_attributes.TryGetValue(name, out node))
                return node;
            node = new DmAttribute(name);
            node._parent = this;
            _attributes.Add(name, node);
            return node;
        }

        public override XdmNode CreateNode()
        {
            return new XdmElement(); 
        }

        public override bool HasAttributes
        {
            get
            {
                return _attributes != null;
            }
        }

        public override DmNodeList ChildAttributes
        {
            get
            {
                if (_attributes == null)
                    return null;
                return new DmNodeList(_attributes.Values);
            }
        }

        public override string Prefix
        {
            get
            {
                return _name.prefix;
            }
        }

        public override string LocalName
        {
            get
            {
                return _name.localName;
            }
        }

        public override string NamespaceURI
        {
            get
            {
                return _name.ns;
            }
        }

        public override IXmlSchemaInfo SchemaInfo
        {
            get
            {
                return _schemaInfo;
            }
            set
            {
                _schemaInfo = new DmSchemaInfo(value);
            }
        }
    }
}