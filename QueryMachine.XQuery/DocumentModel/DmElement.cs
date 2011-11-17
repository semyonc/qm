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
        private Dictionary<String, DmNode> _namespaces;

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
            UpdateNodeSet();
            return node;
        }

        public DmNode CreateNamespace(string name)
        {
            if (_namespaces == null)
                _namespaces = new Dictionary<String, DmNode>();
            DmNode node;
            if (_namespaces.TryGetValue(name, out node))
                return node;            
            node = new DmNamespace(name);
            node._parent = this;
            _namespaces.Add(name, node);
            return node;
        }

        public override bool HasAttributes
        {
            get
            {
                return _attributes != null;
            }
        }

        public override bool HasNamespaces
        {
            get
            {
                return _namespaces != null;
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