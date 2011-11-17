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
    internal class DmAttribute : DmNode
    {
        private DmQName _name;
        private IXmlSchemaInfo _schemaInfo;

        public DmAttribute(DmQName name)
        {
            _name = name;
        }


        public override XPathNodeType NodeType
        {
            get 
            {
                return XPathNodeType.Attribute;
            }
        }

        public DmQName QName
        {
            get
            {
                return _name;
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

        public bool IsId { get; set; }
    }
}