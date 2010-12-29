using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Xml;
using System.Xml.Schema;

using System.Data;
using System.Data.Common;

using DataEngine.CoreServices.Data;
using DataEngine.XQuery;

namespace DataEngine.XQuery
{
    public class ResultsetXmlReader : XmlReader, IXmlSchemaInfo
    {
        private Resultset m_source;
        private XmlNameTable m_nameTable;
        private XmlSchemaSet m_schemaSet;
        private XmlReaderSettings m_settings;

        private Queue<SAXRecord> m_queue;
        private SAXRecord m_curr;
        private int m_depth;
        
        private XmlSchemaElement rootElem;
        private XmlSchemaElement rowElem;
        private XmlSchemaElement[] dataElem;
        private XmlSchemaSimpleType[] dataContent;
        private ReadState m_readState;
        private bool m_isEmptyElement;

        private class SAXRecord
        {
            public XmlNodeType nodeType;
            public string localName;
            public string value;            
            public XmlSchemaElement schemaElement;
            public XmlSchemaType schemaType;
        }

        public ResultsetXmlReader(Resultset source)
            : this(source, "Data", new XmlReaderSettings())
        {
        }

        public ResultsetXmlReader(Resultset source, string name, XmlReaderSettings settings)
        {
            m_source = source;
            m_settings = settings;
            m_queue = new Queue<SAXRecord>();
            if (settings.NameTable != null)
                m_nameTable = settings.NameTable;
            else
                m_nameTable = new NameTable();
            CreateSchema(name, source.RowType.GetSchemaTable());            
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
                XmlTypeCode typeCode = XQuerySequenceType.GetXmlTypeCode(dataType);
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
            NewNode(XmlNodeType.Element, name, null, null);

            m_readState = ReadState.Initial;

            //XmlWriter writer = XmlWriter.Create("c:\\work\\schema.xml");
            //schema.Write(writer);
            //writer.Close();
        }

        private void ImportRow()
        {
            if (m_source.Begin != null)
            {
                Row row = m_source.Dequeue();
                NewNode(XmlNodeType.SignificantWhitespace, "\n  ");
                NewNode(XmlNodeType.Element, "row", null, null);
                for (int k = 0; k < row.Length; k++)
                {
                    if (!row.IsDbNull(k))
                    {
                        NewNode(XmlNodeType.SignificantWhitespace, "\n    ");
                        XmlSchemaElement elem = dataElem[k];
                        NewNode(XmlNodeType.Element, elem.Name, null, null);
                        if (dataContent[k] != null)
                            NewNode(XmlNodeType.Text, (String)dataContent[k].Datatype
                                    .ChangeType(row[k], typeof(System.String)));
                        else
                            NewNode(XmlNodeType.Text, row[k].ToString());
                        NewNode(XmlNodeType.EndElement, elem.Name, elem, dataContent[k]);
                    }
                }
                NewNode(XmlNodeType.EndElement, "row", rowElem, rowElem.SchemaType);
                if (m_source.Begin == null)
                    NewNode(XmlNodeType.EndElement, rootElem.Name, rootElem, rootElem.SchemaType);
            }
        }

        private void NewNode(XmlNodeType nodeType, string localName, 
            XmlSchemaElement schemaElement, XmlSchemaType schemaType)
        {
            SAXRecord rec = new SAXRecord();
            rec.nodeType = nodeType;
            rec.localName = localName;
            rec.schemaElement = schemaElement;
            rec.schemaType = schemaType;
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
            m_source.Cancel();
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
                return m_source.Begin == null; 
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

        public override bool Read()
        {
            if (m_readState == ReadState.Initial)
                m_readState = ReadState.Interactive;
            if (m_queue.Count == 0)
            {
                if (m_source.Begin == null)
                {
                    m_readState = ReadState.EndOfFile;
                    return false;
                }
                ImportRow();
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
