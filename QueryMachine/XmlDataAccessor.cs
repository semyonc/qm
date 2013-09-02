/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2009  Semyon A. Chertkov (semyonc@gmail.com)

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.IO;
using System.Xml;
using System.Data;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;
using System.Collections;

namespace DataEngine
{
    public class XmlDataAccessor: QueryNode
    {
        protected class AccessorContext : DemandProcessingContext
        {
            private Resultset _src;
            private QueryNode _node;
            private QueryContext _queryContext;

            public AccessorContext(QueryNode node, QueryContext queryContext, Resultset src)
                : base(new Resultset[] { src })
            {
                _src = src;
                _node = node;
                _queryContext = queryContext;
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                if (_src.Begin != null)
                {
                    Row row = _src.Dequeue();
                    Stream stm = (Stream)row.GetObject(0);
                    XmlDocument xmldoc = new XmlDocument(_queryContext.NameTable);
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ValidationType = ValidationType.None;
                    settings.IgnoreWhitespace = true;
                    settings.ProhibitDtd = false;
                    settings.XmlResolver = null;
                    settings.NameTable = _queryContext.NameTable;
                    XmlReader render = XmlReader.Create(stm, settings);
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(render.NameTable);
                    InitNsManager(nsManager, _node);
                    _queryContext.SetNsManager(xmldoc, nsManager);
                    row = rs.NewRow();
                    try
                    {
                        xmldoc.Load(render);
                        row.SetObject(0, xmldoc.DocumentElement);
                        if (stm is FileStream)
                            row.SetString(1, ((FileStream)stm).Name);
                    }
                    catch(Exception ex)
                    {
                        string fileName = "";
                        if (stm is FileStream)
                            fileName = Path.GetFileName(((FileStream)stm).Name) + ": ";                        
                        _queryContext.Output.WriteLine("{0}{1}", fileName, ex.Message);
                    }
                    stm.Close();
                    render.Close();
                    rs.Enqueue(row);
                    return true;
                }
                else
                    return false;
            }
        }

        public XmlDataAccessor()
        {
            _childs = new QueryNodeCollection(this);
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {            
            RowCache cache = queryContext.DataCache.Get(this, null);
            if (cache != null)
                return cache.GetResultset();
            Resultset rs1 = ChildNodes[0].Get(queryContext, parameters);
            if (rs1.Begin != null)
            {
                if (ChildNodes[0] is FlatFileAccessor &&
                    ((FlatFileAccessor)ChildNodes[0]).MultiFile)
                    return new Resultset(RowType.CreateContainerType(typeof(System.Object)), 
                        new AccessorContext(this, queryContext, rs1));
                else
                {
                    Row row = rs1.Dequeue();
                    Stream stm = (Stream)row.GetObject(0);
                    XmlDocument xmldoc = new XmlDocument(queryContext.NameTable);
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreWhitespace = true;
                    settings.ProhibitDtd = false;
                    settings.XmlResolver = null;
                    settings.NameTable = queryContext.NameTable;
                    XmlReader render = XmlReader.Create(stm, settings);
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(render.NameTable);
                    InitNsManager(nsManager, this);
                    queryContext.SetNsManager(xmldoc, nsManager);
                    try
                    {
                        xmldoc.Load(render);
                    }
                    finally
                    {
                        stm.Close();
                        render.Close();
                    }
                    Resultset rs = ParseNodes(xmldoc.DocumentElement.ChildNodes, 
                        /*xmldoc.DocumentElement.Attributes*/ null, this);
                    if (queryContext.CacheEnabled)
                        queryContext.DataCache.Add(this, null, rs);
                    return rs;
                }
            }
            else
                return new Resultset(RowType.CreateContainerType(typeof(System.Object)), null);
        }

        public static object OpenFile(QueryNode node, QueryContext context, string fileName)
        {
            FlatFileAccessor accessor = new FlatFileAccessor(fileName);
            Resultset rs = accessor.Get(context, null);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.None;
            settings.IgnoreWhitespace = true;
            settings.ProhibitDtd = false;
            settings.XmlResolver = null;
            settings.NameTable = context.NameTable;
            if (accessor.MultiFile)
            {
                Resultset dest = new Resultset(RowType.CreateContainerType(typeof(System.Object)), null);
                while (rs.Begin != null)
                {
                    Row row = rs.Dequeue();
                    Stream stm = (Stream)row.GetObject(0);
                    XmlDocument xmldoc = new XmlDocument(context.NameTable);
                    XmlReader render = XmlReader.Create(stm, settings);
                    XmlNamespaceManager nsManager = new XmlNamespaceManager(render.NameTable);
                    try
                    {
                        xmldoc.Load(render);
                    }
                    catch (Exception ex)
                    {
                        string name = "";
                        if (stm is FileStream)
                            name = Path.GetFileName(((FileStream)stm).Name) + ": ";
                        context.Output.WriteLine("{0}{1}", name, ex.Message);
                    }
                    XmlDataAccessor.InitNsManager(nsManager, node);
                    context.SetNsManager(xmldoc, nsManager);
                    row = dest.NewRow();
                    row.SetObject(0, xmldoc.DocumentElement);
                    if (stm is FileStream)
                        row.SetString(1, ((FileStream)stm).Name);
                    stm.Close();
                    render.Close();
                    dest.Enqueue(row);
                }
                return dest;
            }
            else
            {
                Row r = rs.Begin;
                if (r == null)
                    return DBNull.Value;
                Stream stm = (Stream)r.GetObject(0);
                rs.Clear();
                XmlDocument xmldoc = new XmlDocument(context.NameTable);
                XmlReader render = XmlReader.Create(stm, settings);
                XmlNamespaceManager nsManager = new XmlNamespaceManager(render.NameTable);
                try
                {
                    xmldoc.Load(render);
                }
                finally
                {
                    render.Close();
                    stm.Close();
                }
                XmlDataAccessor.InitNsManager(nsManager, node);
                context.SetNsManager(xmldoc, nsManager);
                return xmldoc.DocumentElement;
            }
        }


        protected enum ItemType
        {
            Values,
            List,
            Table
        };

        protected class Item
        {
            public ItemType type;
            public List<XmlNode> nodes;

            public Item()
            {
                nodes = new List<XmlNode>();
            }
        }

        protected class ItemList
        {
            private List<Item> _list;

            public ItemList()
            {
                _list = new List<Item>();
            }

            public int Length
            {
                get
                {
                    return _list.Count;
                }
            }

            public Item this[int index]
            {
                get
                {
                    return _list[index];
                }
            }

            public Item Last()
            {
                if (Length > 0)
                    return _list[Length - 1];
                else
                    return null;
            }

            public XmlNode LastNode()
            {
                Item item = Last();
                if (item != null && item.nodes.Count > 0)
                    return item.nodes[item.nodes.Count - 1];
                else
                    return null;
            }

            private Item GetItem(ItemType type)
            {
                Item item = Last();
                if (item == null || type != item.type)
                {
                    item = new Item();
                    item.type = type;
                    _list.Add(item);
                }
                return item;
            }

            public void Add(ItemType type, XmlNode node)
            {
                Item item = GetItem(type);
                item.nodes.Add(node);
            }

            public void Add(ItemType type, XmlNodeList nodes)
            {
                Item item = GetItem(type);
                for (int k = 0; k < nodes.Count; k++)
                    item.nodes.Add(nodes.Item(k));
            }

            public void Add(ItemType type, XmlAttributeCollection nodes)
            {
                Item item = GetItem(type);
                for (int k = 0; k < nodes.Count; k++)
                    item.nodes.Add(nodes.Item(k));
            }

            public void Fork()
            {
                Item item = Last();
                if (item.nodes.Count > 1)
                {
                    ItemType type = item.type;
                    XmlNode node = LastNode();
                    item.nodes.RemoveAt(item.nodes.Count -1); // Semyon 14.10.2008
                    item = new Item();
                    _list.Add(item);
                    item.type = type;
                    item.nodes.Add(node);
                }
            }
        }

        public const int MaxColumns = 256;

        protected class TableColumn
        {
            public Type type;
            public Type dataType;
            public String name;
            public int pos;
            public int count;
            public bool marked;            

            public override bool Equals(object obj)
            {
                if (obj is TableColumn)
                {
                    TableColumn c = (TableColumn)obj;
                    return type == c.type && name == c.name &&
                        pos == c.pos && count == c.count;
                }
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        protected class TableColumns: IEnumerable<TableColumn>
        {
            private List<TableColumn> _list;

            public TableColumns()
            {
                _list = new List<TableColumn>();
            }

            public int Length
            {
                get
                {
                    return _list.Count;
                }
            }

            public TableColumn this[int index]
            {
                get
                {
                    return _list[index];
                }
            }

            public TableColumn Last()
            {
                if (Length > 0)
                    return _list[Length - 1];
                else
                    return null;
            }

            public TableColumn Add()
            {
                TableColumn res = new TableColumn();
                _list.Add(res);
                return res;
            }

            public void Add(TableColumn col)
            {
                if (Length > 0)
                {
                    TableColumn last = Last();
                    if (last.type == col.type && last.name == col.name &&
                        last.pos == col.pos)
                        last.count += col.count;
                    else
                        _list.Add(col);
                }
                else
                    _list.Add(col);
            }

            #region IEnumerable<TableColumn> Members

            public IEnumerator<TableColumn> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            #endregion
        }

        public class NodeList : XmlNodeList
        {
            private List<XmlNode> _nodes = new List<XmlNode>();

            public override int Count
            {
                get { return _nodes.Count; }
            }

            public override System.Collections.IEnumerator GetEnumerator()
            {
                return _nodes.GetEnumerator();
            }

            public override XmlNode Item(int index)
            {
                return _nodes[index];
            }

            public void Add(XmlNode node)
            {
                _nodes.Add(node);
            }

            public void AddRange(XmlNodeList nodes)
            {
                foreach (XmlNode node in nodes)
                    _nodes.Add(node);
            }
        }

        public static XmlTypeManager GetTypeManager(QueryNode node)
        {
            while (node != null)
                if (node.TypeManager != null)
                    return node.TypeManager;
                else
                    node = node.Parent;
            return null;
        }

        public static void InitNsManager(XmlNamespaceManager nsManager, QueryNode node)
        {
            Stack<QueryNode> st = new Stack<QueryNode>();
            while (node != null)
            {
                if (node.Namespaces != null)
                    st.Push(node);
                node = node.Parent;
            }
            while (st.Count > 0)
            {
                node = st.Pop();
                foreach (string[] pair in node.Namespaces)
                    nsManager.AddNamespace(pair[0], pair[1]);
            }
        }

        public static bool IsSpecialAttribute(XmlAttribute attr)
        {
            return attr.Name == "xmlns" || attr.Prefix == "xmlns" || attr.Prefix == "xml" ||
                attr.NamespaceURI == "http://www.w3.org/2001/XMLSchema-instance";
        }

        public static bool IsSpecialNode(XmlNode node)
        {
            return node.NamespaceURI == "http://www.w3.org/2001/XMLSchema";
        }

        public static bool NodeHasNoAttributes(XmlElement elem)
        {
            if (elem.HasAttributes)
                foreach (XmlAttribute attr in elem.Attributes)
                    if (!IsSpecialAttribute(attr))
                        return false;
                return true;
        }

        public static bool IsPairNode(XmlNode node)
        {
            if (node is XmlText)
                return true;
            else
                if (node is XmlElement)
                {
                    XmlElement elem = (XmlElement)node;
                    if (NodeHasNoAttributes(elem) && (!elem.HasChildNodes || 
                            (elem.ChildNodes.Count == 1 && (elem.FirstChild is XmlText || 
                                elem.FirstChild is XmlSignificantWhitespace))))
                        return true;
                }
            return false;
        }

        public static XmlNode Serialize(XmlDocument document, Object o)
        {
            return document.CreateTextNode(Serialize(o));
        }

        public static String Serialize(Object o)
        {
            if (o is Byte)
                return XmlConvert.ToString((Byte)o);
            if (o is Char)
                return XmlConvert.ToString((Char)o);
            if (o is Int16)
                return XmlConvert.ToString((Int16)o);
            if (o is Int32)
                return XmlConvert.ToString((Int32)o);
            if (o is Int64)
                return XmlConvert.ToString((Int64)o);
            if (o is UInt16)
                return XmlConvert.ToString((UInt16)o);
            if (o is UInt32)
                return XmlConvert.ToString((UInt32)o);
            if (o is UInt64)
                return XmlConvert.ToString((UInt64)o);
            if (o is Single)
                return XmlConvert.ToString((Single)o);
            else if (o is Double)
                return XmlConvert.ToString((Double)o);
            else if (o is Decimal)
                return XmlConvert.ToString((Decimal)o);
            else if (o is DateTime)
            {
                DateTime d = (DateTime)o;                
                return new DateTimeOffset(d).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffzzz", 
                    CultureInfo.InvariantCulture);
            }
            else if (o is TimeSpan)
                return XmlConvert.ToString((TimeSpan)o);
            else if (o is DateTimeOffset)
                return XmlConvert.ToString((DateTimeOffset)o);
            else if (o is Boolean)
                return XmlConvert.ToString((Boolean)o);
            else if (o is Guid)
                return XmlConvert.ToString((Guid)o);
            else
                return o.ToString();
        }

        protected bool CanGroupNodes(XmlNode node1, XmlNode node2)
        {
            if (node1 != null && node1 is XmlElement &&
                node2 != null && node2 is XmlElement)
            {
                XmlElement elem1 = (XmlElement)node1;
                XmlElement elem2 = (XmlElement)node2;
                if (elem1.Name == elem2.Name)
                    return true;
            }
            return false;
        }

        protected string GetNodeName(XmlNode node)
        {
            if (node is XmlText && node.ParentNode.ChildNodes.Count == 1)
                return node.ParentNode.Name;
            else
                if (node is XmlAttribute)
                    return "@" + node.Name;
                else
                    return node.Name;
        }

        static private Regex XSDInteger  = new Regex(@"^[\-+]?[0-9]+$");
        static private Regex Literal     = new Regex(@"^0[0-9]+$");    
        static private Regex XSDDecimal  = new Regex(@"^[+\-]?(\d+)?(\.\d+)?$");
        static private Regex XSDDuration = new Regex(@"^[-]?P(\d*Y)?(\d*M)?(\d*D)?(T(\d*H)?(\d*M)?(\d{2}(\.\d+)?S)?)?$");
        static private Regex XSDFloat    = new Regex(@"^(([+\-]?(\d+)?(\.\d+)?([eE][+\-]?\d+)?)|INF|-INF|NaN)$");
        static private Regex XSDDate     = new Regex(@"^[-]?(\d{4})-([1-9]|(0[1-9])|(1[0-2]))-([0-9]|(0[1-9])|(1[0-9])|(2[0-9])|(30|31))(Z|(([+|-]((0[0-9])|(1[0-2]))):00))?$");
        static private Regex XSDDateTime = new Regex(@"^[-]?(\d{4})-([1-9]|(0[1-9])|(1[0-2]))-([0-9]|(0[1-9])|(1[0-9])|(2[0-9])|(30|31))T((([0]?[1-9]|1[0-2])(:|\.)[0-5][0-9]((:|\.)\d{2}(\.\d+)?)( )?)|(([0]?[0-9]|1[0-9]|2[0-3])(:|\.)[0-5][0-9]((:|\.)\d{2}(\.\d+)?)))(Z|(([+|-]((0[0-9])|(1[0-2]))):00))?$");
        static private Regex XSDBoolean  = new Regex(@"^([T|t]rue|[F|f]alse)$");

        private static Type DecodeTypeByRegex(string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;
            else if (text.Length < 18 && (XSDInteger.IsMatch(text) ||
                XSDDecimal.IsMatch(text) || XSDFloat.IsMatch(text)) && !Literal.IsMatch(text) &&
                    text != "-" && text != "+")
                return typeof(System.Decimal);
            else if (XSDDuration.IsMatch(text))
                return typeof(System.TimeSpan);
            else if (XSDDate.IsMatch(text) || XSDDateTime.IsMatch(text))
                return typeof(System.DateTime);
            else if (XSDBoolean.IsMatch(text))
                return typeof(System.Boolean);
            else
                return typeof(System.String);
        }

        public static Type GetNodeType(XmlNode node, XmlTypeManager manager)
        {
            if (manager != null)
            {
                Type type = manager.GetType(node);
                if (type != null)
                    return type;
            }
            if (node is XmlElement)
            {
                if (IsPairNode(node))
                {
                    XmlElement elem = (XmlElement)node;
                    return DecodeTypeByRegex(elem.InnerText);
                }
                else
                    return typeof(System.Object);
            }
            else if (node is XmlAttribute)
            {
                XmlAttribute atr = (XmlAttribute)node;
                return DecodeTypeByRegex(atr.Value);
            }
            else
                return typeof(System.String);
        }

        public static Object Convert(Type type, XmlNode node)
        {
            if (type == typeof(System.Object))
                return node;
            else
            {
                string value;
                if (node is XmlElement)
                    value = node.InnerText;
                else 
                    value = node.Value;
                if (String.IsNullOrEmpty(value))
                    return DBNull.Value;
                else
                {
                    if (type == typeof(System.Decimal))
                        return Decimal.Parse(value, CultureInfo.InvariantCulture);
                    else if (type == typeof(System.TimeSpan))
                        return SoapDuration.Parse(value).Duration();
                    else if (type == typeof(System.DateTime))
                    {
                        if (XSDDate.IsMatch(value))
                            return SoapDate.Parse(value).Value;
                        else
                            return SoapDateTime.Parse(value);
                    }
                    else if (type == typeof(System.Boolean))
                    {
                        if (value.ToLower().Substring(0, 1) == "t")
                            return true;
                        else
                            return false;
                    }
                    else
                        return value;
                }
            }
        }

        protected List<XmlNode> SelectChilds(XmlNode node)
        {
            List<XmlNode> res = new List<XmlNode>();
            foreach (XmlNode child in node.ChildNodes)
                if (!(child is XmlSignificantWhitespace))
                    res.Add(child);
            return res;
        }

        protected TableColumns CreateColumns(XmlNode node)
        {
            TableColumns columns = new TableColumns();
            if (node is XmlElement)
            {
                XmlElement elem = (XmlElement)node;
                if (elem.HasAttributes)
                    foreach (XmlAttribute attr in elem.Attributes)
                    {
                        TableColumn col = columns.Add();
                        col.type = typeof(XmlAttribute);
                        col.name = attr.Name;
                    }
            }
            if (node.HasChildNodes)
            {
                int i = 0;
                List<XmlNode> childs = SelectChilds(node);
                while (i < childs.Count)
                {
                    XmlNode cur = childs[i];
                    TableColumn col = columns.Add();
                    col.type = cur.GetType();
                    col.name = GetNodeName(cur);
                    col.pos = 0;
                    col.count = 1;
                    int k;
                    for (k = 0; k < i; k++)
                    {
                        cur = childs[k];
                        if (!(cur is XmlSignificantWhitespace))
                            if (cur.GetType() == col.type && (!(cur is XmlElement) || GetNodeName(cur) == col.name))
                                col.pos++;
                    }
                    k = i + 1;
                    while (k < childs.Count)
                    {
                        cur = childs[k];
                        if (!(cur is XmlSignificantWhitespace))
                            if (cur.GetType() == col.type && (!(cur is XmlElement) || GetNodeName(cur) == col.name))
                                col.count++;
                            else
                                break;
                        k++;
                    }
                    i = k;
                }
            }
            return columns;
        }

        protected TableColumns GroupColumns(TableColumns nodeColumns, TableColumns columns)
        {
            TableColumns res = new TableColumns();
            if (columns.Length <= nodeColumns.Length)
            {
                for (int i = 0; i < nodeColumns.Length; i++)
                {
                    for (int k = 0; k < columns.Length; k++)
                        if (!columns[k].marked && columns[k].Equals(nodeColumns[i]))
                        {
                            for (int s = 0; s < k - 1; s++)
                                if (!columns[s].marked)
                                {
                                    res.Add(columns[s]);
                                    columns[s].marked = true;
                                }
                            columns[k].marked = true;
                            break;
                        }
                    res.Add(nodeColumns[i]);
                }
                for (int i = 0; i < columns.Length; i++)
                    if (!columns[i].marked)
                        res.Add(columns[i]);
            }
            else
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    for (int k = 0; k < nodeColumns.Length; k++)
                        if (!nodeColumns[k].marked && nodeColumns[k].Equals(columns[i]))
                        {
                            for (int s = 0; s < k -1; s++)
                                if (!nodeColumns[s].marked)
                                {
                                    res.Add(nodeColumns[s]);
                                    nodeColumns[s].marked = true;
                                }
                            nodeColumns[k].marked = true;
                            break;
                        }
                    res.Add(columns[i]);
                }
                for (int i = 0; i < nodeColumns.Length; i++)
                    if (!nodeColumns[i].marked)
                        res.Add(nodeColumns[i]);
            }
            return res;
        }

        protected NodeList GetNodeAtColumn(XmlNode node, TableColumn col)
        {
            NodeList res = new NodeList();
            if (col.type == typeof(XmlAttribute))
            {
                XmlElement elem = (XmlElement)node;
                if (elem.HasAttributes)
                {
                    XmlNode attr = elem.Attributes.GetNamedItem(col.name);
                    if (attr != null)
                        res.Add(attr);
                }
            }
            else
                if (node.HasChildNodes)
                {
                    int pos = col.pos;
                    int count = col.count;
                    List<XmlNode> childs = SelectChilds(node);
                    for (int k = 0; k < childs.Count; k++)
                    {
                        XmlNode cur = childs[k];
                        if (cur.GetType() == col.type && (!(cur is XmlElement) || cur.Name == col.name))
                            if (pos == 0)
                            {
                                int s = k;
                                while (count > 0 && s < childs.Count)
                                {
                                    cur = childs[s];
                                    if (cur.GetType() == col.type && (!(cur is XmlElement) || cur.Name == col.name))
                                    {
                                        res.Add(cur);
                                        count--;
                                    }
                                    else
                                        break;
                                    s++;
                                }
                                break; 
                            }
                            else
                                pos--;
                    }
                }
            return res;
        }

        private TableColumns GroupRows(XmlNode[] rows, int from, int to)
        {
            TableColumns columns = new TableColumns();
            for (int k = from; k < to; k++)
               columns = GroupColumns(CreateColumns(rows[k]), columns);
            return columns;
        }

        public Resultset ParseNodes(XmlNodeList nodes, XmlNamedNodeMap attrs, QueryNode qn)
        {
            DataProviderHelper helper = new DataProviderHelper();
            XmlTypeManager typeManager = GetTypeManager(qn);
            ItemList items = new ItemList();
            if (attrs != null && attrs.Count > 0)
                foreach(XmlAttribute attr in attrs)
                    if (!IsSpecialAttribute(attr))
                        items.Add(ItemType.Values, attr);
            int count = 0;
            foreach (XmlNode node in nodes)
            {
                if (IsSpecialNode(node))
                    continue;
                if (CanGroupNodes(items.LastNode(), node))
                {
                    if (items.Last().type != ItemType.Table)
                    {
                        items.Fork();
                        items.Last().type = ItemType.Table;
                    }
                    items.Add(ItemType.Table, node);
                }
                else if (IsPairNode(node))
                    items.Add(ItemType.Values, node);
                else
                    items.Add(ItemType.List, node);
                count++;
            }
            DataTable dt = RowType.CreateSchemaTable();
            if (items.Length == 1 && items.Last().type == ItemType.Table)
            {
                int p = 0;
                XmlNode[] rows = new XmlNode[count];
                foreach (XmlNode node in nodes)
                {
                    if (IsSpecialNode(node))
                        continue;
                    rows[p++] = node;
                }
                TableColumns columns1 = null;
                TableColumns columns2 = null;
                Iterator.Invoke(new Action[] {
                    () => columns1 = GroupRows(rows, 0, rows.Length / 2),
                    () => columns2 = GroupRows(rows, rows.Length / 2, rows.Length)
                });
                TableColumns columns = GroupColumns(columns1, columns2);
                object[,] data = new object[columns.Length, rows.Length];
                Iterator.For(0, rows.Length, s =>
                {
                    for (int k = 0; k < columns.Length; k++)
                    {
                        NodeList childs = GetNodeAtColumn(rows[s], columns[k]);
                        if (childs.Count > 0)
                        {
                            if (childs.Count == 1)
                                data[k, s] = childs.Item(0);
                            else
                                data[k, s] = childs;
                        }
                    }
                });
                Iterator.For(0, columns.Length, k =>
                {
                    TableColumn col = columns[k];
                    for (int s = 0; s < rows.Length; s++)
                    {
                        object item = data[k, s];
                        if (item != null)
                        {
                            if (item is NodeList)
                            {
                                col.dataType = typeof(System.Object);
                                break;
                            }
                            else
                            {
                                Type dataType = GetNodeType((XmlNode)item, typeManager);
                                if (col.dataType == null)
                                {
                                    col.dataType = dataType;
                                    if (dataType == typeof(System.Object))
                                        break;
                                }
                                else
                                    if (dataType != null && col.dataType != dataType)
                                    {
                                        if (dataType == typeof(System.Object))
                                            col.dataType = typeof(System.Object);
                                        else
                                            col.dataType = typeof(System.String);
                                        break;
                                    }
                            }
                        }
                    }
                    if (col.dataType == null)
                        col.dataType = typeof(System.Object);
                });
                List<string> fieldNames = new List<string>();
                for (int k = 0; k < columns.Length; k++)
                {
                    TableColumn col = columns[k];
                    DataRow r = dt.NewRow();
                    r["ColumnName"] =
                        Util.CreateUniqueName(fieldNames,
                            helper.NativeFormatIdentifier(col.type == typeof(XmlAttribute) ?
                                "@" + XmlConvert.DecodeName(col.name) : XmlConvert.DecodeName(col.name)));
                    r["IsCaseSensitive"] = true;
                    r["ColumnOrdinal"] = k;
                    r["DataType"] = col.dataType;
                    dt.Rows.Add(r);
                }
                Resultset rs = new Resultset(new RowType(dt), null);
                Iterator.For(0, rows.Length, s =>
                {
                    Row row = rs.NewRow();
                    for (int k = 0; k < columns.Length; k++)
                    {
                        object item = data[k,s];
                        if (item != null)
                        {
                            if (item is NodeList)
                                row.SetObject(k, item);
                            else
                                row.SetValue(k, Convert(columns[k].dataType, (XmlNode)item));
                        }
                    }
                    rs.Enqueue(row);
                });
                return rs;
            }
            else
                if (count <= MaxColumns) // We limit max number of columns in resultset
                {
                    List<string> fieldNames = new List<string>();
                    if (attrs != null)
                        for (int k = 0; k < attrs.Count; k++)
                        {
                            XmlAttribute attr = (XmlAttribute)attrs.Item(k);
                            if (IsSpecialAttribute(attr))
                                continue;
                            DataRow r = dt.NewRow();
                            r["ColumnName"] =
                                Util.CreateUniqueName(fieldNames, 
                                    helper.NativeFormatIdentifier(XmlConvert.DecodeName(GetNodeName(attr))));
                            r["IsCaseSensitive"] = true;
                            r["ColumnOrdinal"] = k;
                            Type dataType = GetNodeType(attr, typeManager);
                            r["DataType"] = dataType != null ? dataType : typeof(System.Object);                            
                            dt.Rows.Add(r);
                        }
                    int s = 0;
                    foreach (XmlNode node in nodes)
                    {
                        if (node is XmlSignificantWhitespace || IsSpecialNode(node))
                            continue;
                        DataRow r = dt.NewRow();
                        r["ColumnName"] = 
                            Util.CreateUniqueName(fieldNames, 
                                helper.NativeFormatIdentifier(XmlConvert.DecodeName(GetNodeName(node))));
                        r["IsCaseSensitive"] = true;
                        r["ColumnOrdinal"] = s++;
                        Type dataType = GetNodeType(node, typeManager);
                        r["DataType"] = dataType != null ? dataType : typeof(System.Object);                        
                        dt.Rows.Add(r);
                    }
                    Resultset rs = new Resultset(new RowType(dt), null);
                    Row row = rs.NewRow();
                    s = 0;
                    if (attrs != null)
                        for (int k = 0; k < attrs.Count; k++)
                        {
                            XmlAttribute attr = (XmlAttribute)attrs.Item(k);
                            if (IsSpecialAttribute(attr))
                                continue;
                            row.SetValue(s, Convert(rs.RowType.Fields[s].DataType, attr));
                            s++;
                        }
                    foreach (XmlNode node in nodes)
                    {
                        if (node is XmlSignificantWhitespace || IsSpecialNode(node))
                            continue;
                        row.SetValue(s, Convert(rs.RowType.Fields[s].DataType, node));
                        s++;
                    }
                    rs.Enqueue(row);
                    return rs;
                }
                else
                {   
                    DataRow r = dt.NewRow();
                    r["ColumnName"] = "node";
                    r["ColumnOrdinal"] = 0;
                    r["DataType"] = typeof(System.Object);
                    dt.Rows.Add(r);
                    Resultset rs = new Resultset(new RowType(dt), null);
                    if (attrs != null)
                        foreach (XmlAttribute attr in attrs)
                        {
                            if (IsSpecialAttribute(attr))
                                continue;
                            Row row = rs.NewRow();
                            row.SetObject(0, attr);
                            rs.Enqueue(row);
                        }
                    foreach (XmlNode node in nodes)
                    {
                        if (node is XmlSignificantWhitespace || IsSpecialNode(node))
                            continue;
                        Row row = rs.NewRow();
                        row.SetObject(0, node);
                        rs.Enqueue(row);
                    }
                    return rs;
                }
        }
    }
}
