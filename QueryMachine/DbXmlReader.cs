using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Xml;
using System.Xml.Schema;

using System.Data;
using System.Data.Common;

using DataEngine.XQuery;


namespace QueryMachine.Export
{
    public class DbXmlReader: XmlReader, IXmlSchemaInfo
    {
        private DbDataReader m_reader;
        private XmlNameTable m_nameTable;
        private XmlSchemaSet m_schemaSet;
        private XmlReaderSettings m_settings;
        
        private bool m_readFlag;
        private SAXRecord[] m_buffer;
        private int m_index;
        private int m_depth;
        private int m_count;
        
        private XmlSchemaElement rootElem;
        private XmlSchemaElement rowElem;
        private XmlSchemaElement[] dataElem;
        private ReadState m_readState;
        private bool m_isEmptyElement;

        private struct SAXRecord
        {
            public XmlNodeType nodeType;
            public string localName;
            public string value;
            public bool isNil;
            public XmlSchemaElement schemaElement;
            public XmlSchemaType schemaType;
        }

        public DbXmlReader(DbDataReader reader)
            : this(reader, "Data", new XmlReaderSettings())
        {
        }

        public DbXmlReader(DbDataReader reader, string name, XmlReaderSettings settings)
        {
            m_reader = reader;
            m_settings = settings;
            m_buffer = new SAXRecord[5 * reader.FieldCount];
            if (settings.NameTable != null)
                m_nameTable = settings.NameTable;
            else
                m_nameTable = new NameTable();
            CreateSchema(name, m_reader.GetSchemaTable());            
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
            for (int k = 0; k < dt_rows.Length; k++)
            {
                DataRow r = dt_rows[k];
                dataElem[k] = new XmlSchemaElement();
                dataElem[k].Name = XmlConvert.EncodeName((String)r["ColumnName"]);
                if (r["AllowDBNull"] != DBNull.Value && (bool)r["AllowDBNull"])
                    dataElem[k].IsNillable = true;
                Type dataType = (Type)r["DataType"];
                XmlTypeCode typeCode = XQuerySequenceType.GetXmlTypeCode(dataType);
                if (typeCode == XmlTypeCode.Item)
                    typeCode = XmlTypeCode.HexBinary;
                dataElem[k].SchemaType = XmlSchemaType.GetBuiltInSimpleType(typeCode);
                s2.Items.Add(dataElem[k]);
            }

            m_schemaSet = new XmlSchemaSet();
            m_schemaSet.Add(schema);
            m_schemaSet.Compile();

            NewNode(XmlNodeType.XmlDeclaration, null, null, null, false);            
            NewNode(XmlNodeType.Element, name, null, null, false);

            m_readState = ReadState.Initial;

            //XmlWriter writer = XmlWriter.Create("c:\\work\\schema.xml");
            //schema.Write(writer);
            //writer.Close();
        }

        private void ImportRow()
        {
            m_count = 0;
            m_readFlag = m_reader.Read();
            if (m_readFlag)
            {
                NewNode(XmlNodeType.SignificantWhitespace, "\n  ");
                NewNode(XmlNodeType.Element, "row", null, null, false);
                for (int k = 0; k < m_reader.FieldCount; k++)
                {
                    NewNode(XmlNodeType.SignificantWhitespace, "\n    ");
                    XmlSchemaElement elem = dataElem[k];
                    NewNode(XmlNodeType.Element, elem.Name, null, null, false);
                    bool isNull = m_reader.IsDBNull(k);
                    if (!isNull)
                        NewNode(XmlNodeType.Text, (String)elem.SchemaType.Datatype
                            .ChangeType(m_reader[k], typeof(System.String)));
                    NewNode(XmlNodeType.EndElement, elem.Name, elem, elem.SchemaType, isNull);
                }
                NewNode(XmlNodeType.EndElement, "row", rowElem, rowElem.SchemaType, false);
            }
            else
                NewNode(XmlNodeType.EndElement, rootElem.Name, rootElem, rootElem.SchemaType, false);
        }

        private void NewNode(XmlNodeType nodeType, string localName, 
            XmlSchemaElement schemaElement, XmlSchemaType schemaType, bool isNil)
        {
            m_buffer[m_count].nodeType = nodeType;
            m_buffer[m_count].localName = localName;
            m_buffer[m_count].schemaElement = schemaElement;
            m_buffer[m_count].schemaType = schemaType;
            m_buffer[m_count].isNil = isNil;
            m_count++;
        }

        private void NewNode(XmlNodeType nodeType, string value)
        {
            m_buffer[m_count].nodeType = nodeType;
            m_buffer[m_count].value = value;
            m_count++;
        }

        public override void Close()
        {
            m_reader.Close();
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
                return !m_readFlag; 
            }
        }

        public override bool HasValue
        {
            get 
            { 
                return m_buffer[m_index].value != null; 
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
                return m_buffer[m_index].localName; 
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
                return m_buffer[m_index].nodeType; 
            }
        }

        public override bool Read()
        {
            if (m_count == 0)
            {
                if (!m_readFlag)
                {
                    m_readState = ReadState.EndOfFile;
                    return false;
                }
                else
                    m_readState = ReadState.Interactive;
                m_index = 0;
                ImportRow();
            }
            else
                m_index++;
            m_isEmptyElement = false;
            if (NodeType == XmlNodeType.Element)
            {
                m_depth++;
                if (m_buffer[m_index + 1].nodeType == XmlNodeType.EndElement)
                    m_isEmptyElement = true;
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
                return m_buffer[m_index].value; 
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
                return m_buffer[m_index].isNil; 
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
                return m_buffer[m_index].schemaElement; 
            }
        }

        public XmlSchemaType SchemaType
        {
            get 
            { 
                return m_buffer[m_index].schemaType; 
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
