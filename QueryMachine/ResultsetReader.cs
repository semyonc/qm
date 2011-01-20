using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Xml;
using System.Xml.Schema;

using System.Data;
using System.Data.Common;

using DataEngine.CoreServices.Data;
using System.Diagnostics;

namespace DataEngine
{
    public class ResultsetReader : XmlReader, IXmlSchemaInfo
    {
        private class SAXRecord
        {
            public XmlNodeType nodeType;
            public string localName;
            public string value;
            public XmlSchemaElement schemaElement;
            public XmlSchemaType schemaType;
        }

        private XmlReader m_inner;

        private Resultset m_source;
        private XmlNameTable m_nameTable;
        private XmlSchemaSet m_schemaSet;
        private XmlReaderSettings m_settings;

        private Queue<object> m_queue;
        private SAXRecord m_curr;
        private int m_depth;

        private XmlSchemaElement rootElem;
        private XmlSchemaElement rowElem;
        private XmlSchemaElement[] dataElem;
        private XmlSchemaSimpleType[] dataContent;
        private ReadState m_readState;
        private bool m_isEmptyElement;
        private bool m_createRoot;

        private ResultsetReader(Resultset source, ResultsetReader parent)
        {
            m_source = source;
            m_settings = parent.m_settings;
            m_nameTable = parent.m_nameTable;
            m_queue = new Queue<object>();
            m_createRoot = false;
            CreateSchema("dummy", source.RowType.GetSchemaTable());
        }

        public ResultsetReader(Resultset source, string name, XmlReaderSettings settings)
        {
            m_source = source;
            m_settings = settings;
            m_queue = new Queue<object>();
            if (settings.NameTable != null)
                m_nameTable = settings.NameTable;
            else
                m_nameTable = new NameTable();            
            m_createRoot = true;
            NewNode(XmlNodeType.XmlDeclaration, null, null, null);
            NewNode(XmlNodeType.Element, name, null, null);            
            CreateSchema(name, source.RowType.GetSchemaTable());
        }

        internal bool IsInReadingStates()
        {
            return (m_readState == ReadState.Interactive); 
        }

        private void CreateSchema(String name, DataTable dt)
        {
            XmlSchema schema = new XmlSchema();
            rootElem = new XmlSchemaElement();
            rootElem.Name = name;
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
                dataElem[k].Name = XmlConvert.EncodeLocalName(
                    DataEngine.CoreServices.Util.UnquoteName((String)r["ColumnName"]));
                if (r["AllowDBNull"] != DBNull.Value && (bool)r["AllowDBNull"])
                    dataElem[k].MinOccurs = 0;
                Type dataType = (Type)r["DataType"];
                XmlTypeCode typeCode = XmlTypeCode.Item;
                switch (Type.GetTypeCode(dataType))
                {
                    case System.TypeCode.Boolean:
                        typeCode = XmlTypeCode.Boolean;
                        break;
                    case System.TypeCode.Int16:
                        typeCode = XmlTypeCode.Short;
                        break;
                    case System.TypeCode.Int32:
                        typeCode = XmlTypeCode.Int;
                        break;
                    case System.TypeCode.Int64:
                        typeCode = XmlTypeCode.Long;
                        break;
                    case System.TypeCode.UInt16:
                        typeCode = XmlTypeCode.UnsignedShort;
                        break;
                    case System.TypeCode.UInt32:
                        typeCode = XmlTypeCode.UnsignedInt;
                        break;
                    case System.TypeCode.UInt64:
                        typeCode = XmlTypeCode.UnsignedLong;
                        break;
                    case System.TypeCode.SByte:
                        typeCode = XmlTypeCode.Byte;
                        break;
                    case System.TypeCode.Byte:
                        typeCode = XmlTypeCode.UnsignedByte;
                        break;
                    case System.TypeCode.Single:
                        typeCode = XmlTypeCode.Float;
                        break;
                    case System.TypeCode.Decimal:
                        typeCode = XmlTypeCode.Decimal;
                        break;
                    case System.TypeCode.Double:
                        typeCode = XmlTypeCode.Double;
                        break;
                    case System.TypeCode.Char:
                    case System.TypeCode.String:
                        typeCode = XmlTypeCode.String;
                        break;
                }
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

            m_readState = ReadState.Initial;
        }

        protected void NewNode(XmlNodeType nodeType, string localName,
            XmlSchemaElement schemaElement, XmlSchemaType schemaType)
        {
            SAXRecord rec = new SAXRecord();
            rec.nodeType = nodeType;
            rec.localName = localName;
            rec.schemaElement = schemaElement;
            rec.schemaType = schemaType;
            m_queue.Enqueue(rec);
        }

        protected void NewNode(XmlNodeType nodeType, string value)
        {
            SAXRecord rec = new SAXRecord();
            rec.nodeType = nodeType;
            rec.value = value;
            m_queue.Enqueue(rec);
        }

        protected void NewInnerReader(XmlReader reader)
        {
            m_queue.Enqueue(reader);
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
                            if (row[k] is XmlNode)
                                NewInnerReader(new XmlNodeReader((XmlNode)row[k]));
                            else if (row[k] is XmlNodeList)
                            {
                                XmlNodeList nodeList = (XmlNodeList)row[k];
                                foreach (XmlNode node in nodeList)
                                    NewInnerReader(new XmlNodeReader(node));
                            }
                            else if (row[k] is Resultset)
                                NewInnerReader(new ResultsetReader((Resultset)row[k], this));
                            else
                                NewNode(XmlNodeType.Text, row[k].ToString());
                        NewNode(XmlNodeType.EndElement, elem.Name, elem, dataContent[k]);
                    }
                }
                NewNode(XmlNodeType.EndElement, "row", rowElem, rowElem.SchemaType);
                if (m_source.Begin == null && m_createRoot)
                    NewNode(XmlNodeType.EndElement, rootElem.Name, rootElem, rootElem.SchemaType);
            }
        }

        public override int AttributeCount
        {
            get 
            {
                if (m_inner == null)
                    return 0;
                else
                    return m_inner.AttributeCount; 
            }
        }

        public override string BaseURI
        {
            get 
            {
                if (m_inner == null)
                    return String.Empty;
                else
                    return m_inner.BaseURI;
            }
        }

        public override void Close()
        {
            while (m_queue.Count > 0)
            {
                XmlReader reader = m_queue.Dequeue() as XmlReader;
                if (reader != null)
                    reader.Close();
            }
            if (m_inner != null)
            {
                m_inner.Close();
                m_inner = null;
            }
            m_source.Cancel();
            m_readState = ReadState.Closed;            
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

        public override string GetAttribute(int i)
        {
            if (m_inner != null)
                return m_inner.GetAttribute(i);
            else
                throw new ArgumentOutOfRangeException("i");
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            if (m_inner != null)
                return m_inner.GetAttribute(name, namespaceURI);
            else
                throw new ArgumentOutOfRangeException("name");
        }

        public override string GetAttribute(string name)
        {
            if (m_inner != null)
                return m_inner.GetAttribute(name);
            else
                throw new ArgumentOutOfRangeException("name");
        }

        public override bool HasValue
        {
            get 
            {
                if (m_inner != null)
                    return m_inner.HasValue;
                else
                    return (m_curr != null) && (m_curr.value != null); 
            }
        }

        public override bool IsEmptyElement
        {
            get 
            {
                if (m_inner != null)
                    return m_inner.IsEmptyElement;
                else
                    return m_isEmptyElement;
            }
        }

        public override string LocalName
        {
            get 
            {
                if (m_inner != null)
                    return m_inner.LocalName;
                else
                {
                    if (!IsInReadingStates())
                        return String.Empty;
                    return m_curr.localName;
                }
            }
        }

        public override string LookupNamespace(string prefix)
        {
            if (m_inner != null)
                return m_inner.LookupNamespace(prefix);
            else
                return null;
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            if (m_inner != null)
                return m_inner.MoveToAttribute(name, ns);
            else
                return false;
        }

        public override bool MoveToAttribute(string name)
        {
            if (m_inner != null)
                return m_inner.MoveToAttribute(name);
            else
                return false;
        }

        public override bool MoveToElement()
        {
            if (m_inner != null)
                return m_inner.MoveToElement();
            else
                return false;
        }

        public override bool MoveToFirstAttribute()
        {
            if (m_inner != null)
                return m_inner.MoveToFirstAttribute();
            else
                return false;
        }

        public override bool MoveToNextAttribute()
        {
            if (m_inner != null)
                return m_inner.MoveToNextAttribute();
            else
                return false;
        }

        public override XmlNameTable NameTable
        {
            get 
            {
                return m_nameTable;
            }
        }

        public override string NamespaceURI
        {
            get 
            {
                if (m_inner != null)
                    return m_inner.NamespaceURI;
                else
                    return String.Empty;
            }
        }

        public override XmlNodeType NodeType
        {
            get 
            {
                if (m_inner != null)
                    return m_inner.NodeType;
                else
                {
                    if (!IsInReadingStates())
                        return XmlNodeType.None;
                    return m_curr.nodeType;
                }
            }
        }

        public override string Prefix
        {
            get 
            {
                if (m_inner != null)
                    return m_inner.Prefix;
                else
                    return String.Empty;
            }
        }

        public override bool Read()
        {
            if (m_readState == ReadState.Initial)
                m_readState = ReadState.Interactive;
            if (m_inner != null)
            {
                if (m_inner.Read())
                {
                    if (NodeType == XmlNodeType.Element)
                        m_depth++;
                    else if (NodeType == XmlNodeType.EndElement)
                        m_depth--;
                    return true;
                }
                m_inner.Close();
                m_inner = null;
            }
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
            object tail = m_queue.Dequeue();
            if (tail is XmlReader)
            {
                m_inner = (XmlReader)tail;
                Debug.Assert(m_inner.Read());
            }
            else
                m_curr = (SAXRecord)tail;
            if (NodeType == XmlNodeType.Element)
            {
                m_depth++;
                if (m_queue.Count > 0)
                {
                    object peek = m_queue.Peek();
                    if (peek is SAXRecord && ((SAXRecord)peek).nodeType == XmlNodeType.EndElement)
                        m_isEmptyElement = true;
                }
            }
            else if (NodeType == XmlNodeType.EndElement)
                m_depth--;
            return true;
        }

        public override bool ReadAttributeValue()
        {
            if (m_inner != null)
                return m_inner.ReadAttributeValue();
            else
                return false;
        }

        public override ReadState ReadState
        {
            get 
            {
                return m_readState;
            }
        }

        public override void ResolveEntity()
        {
            if (m_inner != null)
                m_inner.ResolveEntity();
        }

        public override string Value
        {
            get 
            {
                if (m_inner != null)
                    return m_inner.Value;
                else
                {
                    if (!IsInReadingStates())
                        return String.Empty;
                    return m_curr.value;
                }
            }
        }

        #region IXmlSchemaInfo Members

        public bool IsNil
        {
            get 
            {
                if (m_inner != null)
                {
                    IXmlSchemaInfo schemaInfo = m_inner as IXmlSchemaInfo;
                    if (schemaInfo == null)
                        return false;
                    return schemaInfo.IsNil;
                }
                return false;
            }
        }

        public XmlSchemaSimpleType MemberType
        {
            get 
            {
                if (m_inner != null)
                {
                    IXmlSchemaInfo schemaInfo = m_inner as IXmlSchemaInfo;
                    if (schemaInfo == null)
                        return null;
                    return schemaInfo.MemberType;
                }
                return null;
            }
        }

        public XmlSchemaAttribute SchemaAttribute
        {
            get 
            {
                if (m_inner != null)
                {
                    IXmlSchemaInfo schemaInfo = m_inner as IXmlSchemaInfo;
                    if (schemaInfo == null)
                        return null;
                    return schemaInfo.SchemaAttribute;
                }
                return null;
            }
        }

        public XmlSchemaElement SchemaElement
        {
            get 
            {
                if (m_inner != null)
                {
                    IXmlSchemaInfo schemaInfo = m_inner as IXmlSchemaInfo;
                    if (schemaInfo == null)
                        return null;
                    return schemaInfo.SchemaElement;
                }
                if (!IsInReadingStates())
                    return null;
                return m_curr.schemaElement;
            }
        }

        public XmlSchemaType SchemaType
        {
            get 
            {
                if (m_inner != null)
                {
                    IXmlSchemaInfo schemaInfo = m_inner as IXmlSchemaInfo;
                    if (schemaInfo == null)
                        return null;
                    return schemaInfo.SchemaType;
                }
                if (!IsInReadingStates())
                    return null;
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
    }
}
