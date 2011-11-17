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
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Data;
using System.Data.Common;


namespace DataEngine.XQuery
{
    public class DbXmlReader : XmlReader, IXmlSchemaInfo
    {
        private IDataReader m_source;
        private XmlNameTable m_nameTable;
        private XmlSchemaSet m_schemaSet;
        private XmlReaderSettings m_settings;
        private string m_tableName;
        private bool m_typed;
        private bool m_finish;

        private Queue<SAXRecord> m_queue;
        private SAXRecord m_curr;
        private int m_depth;

        private XmlSchemaElement rootElem;
        private XmlSchemaElement rowElem;
        private XmlSchemaElement[] dataElem;
        private XmlSchemaSimpleType[] dataContent;
        private ReadState m_readState;
        private bool m_isEmptyElement;

        public bool Typed
        {
            get
            {
                return m_typed;
            }
            set
            {
                if (m_readState != ReadState.Initial)
                    throw new InvalidOperationException();
                m_typed = value;
            }
        }

        private class SAXRecord
        {
            public XmlNodeType nodeType;
            public string localName;
            public string value;
            public XmlSchemaElement schemaElement;
            public XmlSchemaType schemaType;
        }

        public DbXmlReader(IDataReader source)
            : this(source, "Data", new XmlReaderSettings())
        {
        }

        public DbXmlReader(IDataReader source, string name, XmlReaderSettings settings)
        {
            m_source = source;
            m_settings = settings;
            m_queue = new Queue<SAXRecord>();
            if (settings.NameTable != null)
                m_nameTable = settings.NameTable;
            else
                m_nameTable = new NameTable();
            m_tableName = name;
            DataTable dt = m_source.GetSchemaTable();
            CreateSchema(m_tableName, dt);
        }

        private void CreateSchema(String name, DataTable dt)
        {
            XmlSchema schema = new XmlSchema();
            rootElem = new XmlSchemaElement();
            rootElem.Name = XmlConvert.EncodeName(name);
            schema.Items.Add(rootElem);

            XmlSchemaComplexType rootContent = new XmlSchemaComplexType();
            rootElem.SchemaType = rootContent;

            XmlSchemaSequence s1 = new XmlSchemaSequence();
            rootContent.Particle = s1;

            rowElem = new XmlSchemaElement();
            rowElem.Name = "row";
            rowElem.MinOccurs = 0;
            rowElem.MaxOccursString = "unbounded";
            s1.Items.Add(rowElem);

            XmlSchemaComplexType rowContent = new XmlSchemaComplexType();
            rowElem.SchemaType = rowContent;
            XmlSchemaSequence s2 = new XmlSchemaSequence();
            rowContent.Particle = s2;

            DataRow[] dt_rows = dt.Select();
            dataElem = new XmlSchemaElement[dt_rows.Length];
            dataContent = new XmlSchemaSimpleType[dt_rows.Length];
            for (int k = 0; k < dt_rows.Length; k++)
            {
                DataRow r = dt_rows[k];
                dataElem[k] = new XmlSchemaElement();
                dataElem[k].Name = XmlConvert.EncodeName((String)r["ColumnName"]);
                if (r["AllowDBNull"] != DBNull.Value && (bool)r["AllowDBNull"])
                    dataElem[k].MinOccurs = 0;
                Type dataType = (Type)r["DataType"];
                XmlTypeCode typeCode = XmlTypeCode.Item;
                if (m_typed)
                    XQuerySequenceType.GetXmlTypeCode(dataType);
                if (typeCode == XmlTypeCode.Item)
                    dataContent[k] = null;
                else
                {
                    dataContent[k] = XmlSchemaType.GetBuiltInSimpleType(typeCode);
                    dataElem[k].SchemaTypeName = dataContent[k].QualifiedName;
                }
                s2.Items.Add(dataElem[k]);
            }

            m_schemaSet = new XmlSchemaSet();
            m_schemaSet.Add(schema);
            m_schemaSet.Compile();

            NewNode(XmlNodeType.XmlDeclaration, null, null, null);
            NewNode(XmlNodeType.Element, m_tableName, null, null);                  
            m_readState = ReadState.Initial;
            m_finish = false;
        }

        private void NewNode(XmlNodeType nodeType, string localName,
            XmlSchemaElement schemaElement, XmlSchemaType schemaType)
        {
            SAXRecord rec = new SAXRecord();
            rec.nodeType = nodeType;
            rec.localName = localName;
            if (m_typed)
            {
                rec.schemaElement = schemaElement;
                rec.schemaType = schemaType;
            }
            m_queue.Enqueue(rec);
        }

        private void NewNode(XmlNodeType nodeType, string value)
        {
            SAXRecord rec = new SAXRecord();
            rec.nodeType = nodeType;
            rec.value = value;
            m_queue.Enqueue(rec);
        }

        private void CheckState()
        {
            if (m_readState != ReadState.Interactive)
                throw new InvalidOperationException("Invalid state");
        }

        public override void Close()
        {
            m_source.Close();
            m_readState = ReadState.Closed;
        }

        public override string BaseURI
        {
            get
            {
                return String.Empty;
            }
        }

        public override int Depth
        {
            get
            {
                return m_depth;
            }
        }

        public override bool EOF
        {
            get
            {
                return m_readState == ReadState.EndOfFile;
            }
        }

        public override bool HasValue
        {
            get
            {
                return (m_curr != null) && (m_curr.value != null);
            }
        }

        public override bool IsEmptyElement
        {
            get
            {
                return m_isEmptyElement;
            }
        }

        public override string Prefix
        {
            get
            {
                return String.Empty;
            }
        }

        public override string LocalName
        {
            get
            {
                CheckState();
                return m_curr.localName;
            }
        }

        public override string NamespaceURI
        {
            get
            {
                return String.Empty;
            }
        }

        public override XmlNameTable NameTable
        {
            get
            {
                return m_nameTable;
            }
        }

        public override XmlNodeType NodeType
        {
            get
            {
                CheckState();
                return m_curr.nodeType;
            }
        }

        private void ImportRow()
        {            
            NewNode(XmlNodeType.SignificantWhitespace, "\n  ");
            NewNode(XmlNodeType.Element, "row", null, null);
            for (int k = 0; k < m_source.FieldCount; k++)
            {
                if (!m_source.IsDBNull(k))
                {
                    NewNode(XmlNodeType.SignificantWhitespace, "\n    ");
                    XmlSchemaElement elem = dataElem[k];
                    NewNode(XmlNodeType.Element, elem.Name, null, null);
                    if (m_typed && dataContent[k] != null)
                        NewNode(XmlNodeType.Text, (String)dataContent[k].Datatype
                                .ChangeType(m_source[k], typeof(System.String)));
                    else
                        NewNode(XmlNodeType.Text, XQueryConvert.ToString(m_source[k]));
                    NewNode(XmlNodeType.EndElement, elem.Name, elem, dataContent[k]);
                }
            }
            NewNode(XmlNodeType.EndElement, "row", rowElem, rowElem.SchemaType);
        }

        public override bool Read()
        {
            if (m_readState == ReadState.Initial)
                m_readState = ReadState.Interactive;
            if (m_queue.Count == 0)
            {
                if (m_finish)
                {
                    m_readState = ReadState.EndOfFile;
                    return false;
                }
                if (m_source.Read())
                    ImportRow();
                else
                {
                    NewNode(XmlNodeType.EndElement, rootElem.Name, rootElem, rootElem.SchemaType);
                    m_finish = true;
                }
            }
            m_isEmptyElement = false;
            m_curr = m_queue.Dequeue();
            if (NodeType == XmlNodeType.Element)
            {
                m_depth++;
                if (m_queue.Count > 0)
                {
                    SAXRecord rec = m_queue.Peek();
                    if (rec.nodeType == XmlNodeType.EndElement)
                        m_isEmptyElement = true;
                }
            }
            else if (NodeType == XmlNodeType.EndElement)
                m_depth--;
            return true;
        }

        public override ReadState ReadState
        {
            get
            {
                return m_readState;
            }
        }

        public override string Value
        {
            get
            {
                CheckState();
                return m_curr.value;
            }
        }

        public override void ResolveEntity()
        {
            return;
        }

        public override string LookupNamespace(string prefix)
        {
            return null;
        }

        public override XmlReaderSettings Settings
        {
            get
            {
                return m_settings;
            }
        }

        #region IXmlSchemaInfo Members

        public bool IsNil
        {
            get
            {
                return false;
            }
        }

        public XmlSchemaSimpleType MemberType
        {
            get
            {
                return null;
            }
        }

        public XmlSchemaAttribute SchemaAttribute
        {
            get
            {
                return null;
            }
        }

        public XmlSchemaElement SchemaElement
        {
            get
            {
                CheckState();
                return m_curr.schemaElement;
            }
        }

        public XmlSchemaType SchemaType
        {
            get
            {
                CheckState();
                return m_curr.schemaType;
            }
        }

        public XmlSchemaValidity Validity
        {
            get
            {
                return XmlSchemaValidity.Valid;
            }
        }

        #endregion

        #region Not used attribute support

        public override bool MoveToElement()
        {
            return false;
        }

        public override bool ReadAttributeValue()
        {
            return false;
        }

        public override string GetAttribute(int i)
        {
            throw new ArgumentOutOfRangeException("i");
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            throw new ArgumentOutOfRangeException("name");
        }

        public override string GetAttribute(string name)
        {
            throw new ArgumentOutOfRangeException("name");
        }

        public override bool MoveToFirstAttribute()
        {
            return false;
        }

        public override bool MoveToNextAttribute()
        {
            return false;
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return false;
        }

        public override bool MoveToAttribute(string name)
        {
            return false;
        }

        public override int AttributeCount
        {
            get { return 0; }
        }

        #endregion

    }
}
