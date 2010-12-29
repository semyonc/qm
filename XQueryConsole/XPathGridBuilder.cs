//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace WmHelp.XmlGrid
{
    public class XPathItemCell : GridCell
    {
        public string Value { get; private set; }

        public XPathItemCell(XPathItem item)
        {
            Value = item.Value;
        }

        public override StringFormat GetStringFormat()
        {
            StringFormat sf = base.GetStringFormat();
            sf.LineAlignment = StringAlignment.Near;
            sf.FormatFlags = 0;
            return sf;
        }

        public override string Text
        {
            get
            {
                return Value;
            }
            set
            {
                return;
            }
        }
    }

    public class XPathLabelCell : GridCell
    {
        private XPathNodeType _nodeType;
        private String _text;

        public XPathLabelCell(XPathNavigator nav)
        {
            _nodeType = nav.NodeType;
            if (_nodeType == XPathNodeType.Comment ||
                _nodeType == XPathNodeType.Text)
                _text = nav.Value;
            else
                if (_nodeType == XPathNodeType.Root)
                    _text = "xml";
                else 
                    _text = nav.Name;
        }

        public override int ImageIndex
        {
            get
            {
                switch (_nodeType)
                {
                    case XPathNodeType.Attribute:
                    case XPathNodeType.Namespace:
                        return 2;
                    case XPathNodeType.Element:
                        return 3;
                    case XPathNodeType.Text:
                        return 4;
                    case XPathNodeType.Comment:
                        return 6;
                    default:
                        return -1;
                }
            }
        }

        public override string Text
        {
            get
            {
                return _text;
            }
            set
            {
                return;
            }
        }

        public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font, Brush brush,
            StringFormat sf, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            if (gridView.AutoHeightCells && _nodeType == XPathNodeType.Text)
                sf.FormatFlags = sf.FormatFlags & ~StringFormatFlags.NoWrap;
            if (_nodeType != XPathNodeType.Attribute && _nodeType != XPathNodeType.Element)
                font = new Font(font, FontStyle.Italic);
            base.DrawCellText(gridView, graphics, font, brush, sf, drawInfo, rect);
        }
    }

    public class XPathValueCell : GridCell
    {
        private string _text;

        public XPathValueCell(XPathNavigator nav)
        {
            _text = nav.Value;
        }

        public override string Text
        {
            get
            {
                return _text;
            }
            set
            {
                return;
            }
        }

        public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font, Brush brush,
            StringFormat sf, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            if (gridView.AutoHeightCells)
                sf.FormatFlags = sf.FormatFlags & ~StringFormatFlags.NoWrap;
            base.DrawCellText(gridView, graphics, font, brush, sf, drawInfo, rect);
        }

        public override int GetTextHeight(XmlGridView gridView, Graphics graphics,
            Font font, XmlGridView.DrawInfo drawInfo, int Width)
        {
            if (String.IsNullOrEmpty(Text))
                return drawInfo.cyChar;
            else
            {
                StringFormat sf = GetStringFormat();
                sf.FormatFlags = 0;
                SizeF sz = graphics.MeasureString(Text, font, Width, sf);
                int height = Math.Max((int)sz.Height, drawInfo.cyChar);
                if (height > drawInfo.cyChar)
                    height += 4;
                return height;
            }
        }
    }

    public class XPathDeclarationCell : GridCell
    {
        private string _text;

        public XPathDeclarationCell(XPathNavigator nav)
        {
            if (nav.NodeType == XPathNodeType.Root)
                _text = "version=\"1.0\" encoding=\"utf-8\"";
            else 
                _text = nav.Value;
        }

        public override string Text
        {
            get
            {
                return _text;
            }
            set
            {
                return;
            }
        }
    }

    public class XPathGroupCell : GridCellGroup
    {
        public XPathNavigator Navigator { get; private set; }

        public XPathGroupCell(XPathNavigator nav)
        {
            Navigator = nav.Clone();
        }

        public override string Text
        {
            get
            {
                return Navigator.Name;
            }
            set
            {
                return;
            }
        }

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (Navigator.HasAttributes)
                {
                    XPathNavigator curr = Navigator.Clone();
                    curr.MoveToFirstAttribute();
                    do
                    {
                        if (sb.Length > 150)
                        {
                            sb.Append("..");
                            break;
                        }
                        if (sb.Length > 0)
                            sb.Append(" ");
                        sb.AppendFormat("{0}={1}", curr.Name, curr.Value);
                    } while (curr.MoveToNextAttribute());
                }
                return sb.ToString();
            }
        }

        public override void BeforeExpand()
        {
            if (Table.IsEmpty)
            {
                XPathGridBuilder builder = new XPathGridBuilder();
                builder.ParseChildNodes(this, Navigator);
            }
        }

        public override void CopyToClipboard()
        {
            DataFormats.Format fmt = DataFormats.GetFormat("EXML Fragment");
            DataObject data = new DataObject();
            MemoryStream stream = new MemoryStream();
            StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
            sw.Write("<doc>");
            if (TableView)
            {
                for (int s = 1; s < Table.Height; s++)
                {
                    XPathRowLabelCell cell = (XPathRowLabelCell)Table[0, s];
                    sw.Write(cell.Navigator.OuterXml);
                }
            }
            else
                sw.Write(Navigator.OuterXml);
            sw.Write("</doc>");
            sw.Flush();
            stream.WriteByte(0);
            stream.WriteByte(0);
            data.SetData(fmt.Name, false, stream);
            if (TableView)
            {
                stream = new MemoryStream();
                sw = new StreamWriter(stream, Encoding.Default);
                String separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                for (int s = 0; s < Table.Height; s++)
                {
                    if (s > 0)
                        sw.WriteLine();
                    for (int k = 1; k < Table.Width; k++)
                    {
                        if (k > 1)
                            sw.Write(separator);
                        if (Table[k, s] != null && Table[k, s].Text != null)
                        {
                            String text = Table[k, s].Text;
                            if (text.Contains(separator) || text.Contains("\""))
                                text = String.Format("\"{0}\"", text.Replace("\"", "\"\""));
                            sw.Write(text);
                        }
                    }
                }
                sw.Flush();
                stream.WriteByte(0);
                stream.WriteByte(0);
                data.SetData(DataFormats.CommaSeparatedValue, false, stream);
            }
            data.SetText(Text);
            Clipboard.SetDataObject(data);
        }
    }

    public class XPathColumnLabelCell : GridColumnLabel
    {
        public XPathNodeType NodeType { get; private set; }
        public String NodeName { get; private set; }
        public String LocalName { get; private set; }
        public String NamespaceUri { get; private set; }
        public int NodePos { get; private set; }

        public XPathColumnLabelCell(XPathNodeType nodeType, String nodeName, String localName, String namespaceUri, int nodePos)
        {
            NodeType = nodeType;
            NodeName = nodeName;
            LocalName = localName;
            NamespaceUri = namespaceUri;
            NodePos = nodePos;
        }

        public override int ImageIndex
        {
            get
            {
                switch (NodeType)
                {
                    case XPathNodeType.Attribute:
                        return 2;
                    case XPathNodeType.Element:
                        return 3;
                    case XPathNodeType.Text:
                        return 4;
                    default:
                        return -1;
                }
            }
        }

        public XPathNavigator GetNodeAtColumn(XPathNavigator nav)
        {
            int nodePos = NodePos;
            if (NodeType == XPathNodeType.Attribute)
            {
                XPathNavigator curr = nav.Clone();
                if (curr.MoveToAttribute(LocalName, NamespaceUri))
                    return curr;
            }
            else
                if (nav.HasChildren)
                {
                    XPathNavigator curr = nav.Clone();
                    curr.MoveToFirstChild();
                    do
                    {
                        if (nav.NodeType == NodeType &&
                            ((NodeType != XPathNodeType.Element || 
                                (LocalName == nav.LocalName && NamespaceUri == nav.NamespaceURI))))
                            if (nodePos == 0)
                                return curr;
                            else
                                nodePos--;
                    }
                    while (curr.MoveToNext());
                }
            return null;
        }

        public override string Text
        {
            get
            {
                switch (NodeType)
                {
                    case XPathNodeType.Text:
                        return "#text";
                    default:
                        return NodeName;
                }
            }
            set
            {
                base.Text = value;
            }
        }

        public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font,
            Brush brush, StringFormat sf, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            if (NodeType != XPathNodeType.Element && NodeType != XPathNodeType.Attribute)
                font = new Font(font, FontStyle.Italic);
            else
                font = new Font(font, FontStyle.Bold);
            base.DrawCellText(gridView, graphics, font, brush, sf, drawInfo, rect);
        }
    }

    public class XPathRowLabelCell : GridRowLabel
    {
        public XPathNavigator Navigator { get; private set; }

        public XPathRowLabelCell(int row, XPathNavigator nav)
        {
            RowNum = row;
            Navigator = nav.Clone();
        }
    }

    public class XPathGridBuilder
    {
        protected class TableColumn
        {
            public XPathNodeType nodeType;
            public Type dataType;
            public String name;
            public String localName;
            public String ns;
            public int pos;
            public int count;
            public bool marked;

            public override bool Equals(object obj)
            {
                if (obj is TableColumn)
                {
                    TableColumn c = (TableColumn)obj;
                    return nodeType == c.nodeType && name == c.name &&
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

        protected class TableColumns : IEnumerable<TableColumn>
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
                    if (last.nodeType == col.nodeType && last.localName == col.localName && 
                        last.ns == col.ns && last.pos == col.pos)
                        last.count++;
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


        protected enum ItemType
        {
            Values,
            List,
            Table
        };

        protected class Item
        {
            public ItemType type;
            public List<XPathNavigator> nodes;

            public Item()
            {
                nodes = new List<XPathNavigator>();
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

            public XPathNavigator LastNode()
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

            public void Add(ItemType type, XPathNavigator node)
            {
                Item item = GetItem(type);
                item.nodes.Add(node.Clone());
            }

            public void Fork()
            {
                Item item = Last();
                if (item.nodes.Count > 1)
                {
                    ItemType type = item.type;
                    XPathNavigator node = LastNode();
                    item.nodes.RemoveAt(item.nodes.Count - 1);
                    item = new Item();
                    _list.Add(item);
                    item.type = type;
                    item.nodes.Add(node);
                }
            }

            public int CountCells()
            {
                int res = 0;
                for (int i = 0; i < Length; i++)
                    if (this[i].type == ItemType.List)
                        res += this[i].nodes.Count;
                    else
                        res++;
                return res;
            }
        }        

        public static bool IsPairNode(XPathNavigator nav)
        {
            if (nav.NodeType == XPathNodeType.Text || nav.NodeType == XPathNodeType.Attribute ||
                nav.NodeType == XPathNodeType.Namespace)
                return true;
            else
                if (nav.NodeType == XPathNodeType.Element)
                {
                    if (!nav.HasAttributes)
                    {
                        XPathNavigator curr = nav.Clone();
                        if (!curr.MoveToFirstChild())
                            return true;
                        if (curr.NodeType == XPathNodeType.Text)
                            return !curr.MoveToNext();
                    }
                }
            return false;
        }

        protected bool CanGroupNodes(XPathNavigator nav1, XPathNavigator nav2)
        {
            if (nav1 != null && nav1.NodeType == XPathNodeType.Element &&
                nav2 != null && nav2.NodeType == XPathNodeType.Element)
            {
                if (nav1.LocalName == nav2.LocalName &&
                    nav1.NamespaceURI == nav2.NamespaceURI)
                    return true;
            }
            return false;
        }

        protected List<XPathNavigator> SelectChilds(XPathNavigator nav)
        {
            List<XPathNavigator> res = new List<XPathNavigator>();
            XPathNavigator curr = nav.Clone();
            if (curr.MoveToFirstChild())
            {
                do
                {
                    if (curr.NodeType != XPathNodeType.SignificantWhitespace)
                        res.Add(curr.Clone());
                } while (curr.MoveToNext());
            }
            return res;
        }

        protected TableColumns CreateColumns(XPathNavigator nav)
        {
            TableColumns columns = new TableColumns();
            if (nav.NodeType == XPathNodeType.Element)
            {
                XPathNavigator curr = nav.Clone();
                if (curr.MoveToFirstAttribute())
                {
                    do
                    {
                        TableColumn col = columns.Add();
                        col.nodeType = curr.NodeType;
                        col.name = curr.Name;
                        col.localName = curr.LocalName;
                        col.ns = curr.NamespaceURI;
                    }
                    while (curr.MoveToNextAttribute());
                }
            }
            if (nav.HasChildren)
            {
                int i = 0;
                List<XPathNavigator> childs = SelectChilds(nav);
                while (i < childs.Count)
                {
                    XPathNavigator cur = childs[i];
                    TableColumn col = columns.Add();
                    col.nodeType = cur.NodeType;
                    col.localName = cur.LocalName;
                    col.ns = cur.NamespaceURI;
                    col.name = cur.Name;
                    col.pos = 0;
                    col.count = 1;
                    int k;
                    for (k = 0; k < i; k++)
                    {
                        cur = childs[k];
                        if (cur.NodeType == col.nodeType && (cur.NodeType != XPathNodeType.Element || 
                            (cur.LocalName == col.localName && cur.NamespaceURI == col.ns)))
                            col.pos++;
                    }
                    k = i + 1;
                    while (k < childs.Count)
                    {
                        cur = childs[k];
                        if (cur.NodeType == col.nodeType && (cur.NodeType != XPathNodeType.Element ||
                            (cur.LocalName == col.localName && cur.NamespaceURI == col.ns)))
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

        protected TableColumns GroupNode(XPathNavigator nav, TableColumns columns)
        {
            TableColumns nodeColumns = CreateColumns(nav);
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
                            for (int s = 0; s < k - 1; s++)
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

        protected List<XPathNavigator> GetNodeAtColumn(XPathNavigator nav, TableColumn col)
        {
            List<XPathNavigator> res = new List<XPathNavigator>();
            if (col.nodeType == XPathNodeType.Attribute)
            {
                XPathNavigator curr = nav.Clone();
                if (curr.HasAttributes)
                {
                    if (curr.MoveToAttribute(col.localName, col.ns))
                        res.Add(curr);
                }
            }
            else
                if (nav.HasChildren)
                {
                    int pos = col.pos;
                    int count = col.count;
                    List<XPathNavigator> childs = SelectChilds(nav);
                    for (int k = 0; k < childs.Count; k++)
                    {
                        XPathNavigator cur = childs[k];
                        if (cur.NodeType == col.nodeType && (cur.NodeType != XPathNodeType.Element ||
                                (cur.LocalName == col.localName && cur.NamespaceURI == col.ns)))
                            if (pos == 0)
                            {
                                int s = k;
                                while (count > 0 && s < childs.Count)
                                {
                                    cur = childs[s];
                                    if (cur.NodeType == col.nodeType && (cur.NodeType != XPathNodeType.Element ||
                                        (cur.LocalName == col.localName && cur.NamespaceURI == col.ns)))
                                    {
                                        res.Add(cur);
                                        count--;
                                    }
                                    else
                                        break;
                                    s++;
                                }
                            }
                            else
                                pos--;
                    }
                }
            return res;
        }

        public void ParseChildNodes(GridCellGroup cell, XPathNavigator parent)
        {
            ItemList items = new ItemList();
            XPathNavigator curr = parent.Clone();
            bool iterf;
            if (curr.NodeType == XPathNodeType.Root)
            {
                items.Add(ItemType.Values, curr);
                iterf = curr.MoveToFirstChild();                
            }
            else
            {
                if (curr.MoveToFirstNamespace(XPathNamespaceScope.Local))
                {
                    do
                    {
                        items.Add(ItemType.Values, curr.Clone());
                    }
                    while (curr.MoveToNextNamespace(XPathNamespaceScope.Local));
                    curr.MoveToParent();
                }
                if (curr.MoveToFirstAttribute())
                {
                    do
                    {
                        items.Add(ItemType.Values, curr.Clone());
                    }
                    while (curr.MoveToNextAttribute());
                    curr.MoveToParent();
                }
                iterf = curr.MoveToFirstChild();
            }
            if (iterf)
                do
                {
                    if (curr.NodeType == XPathNodeType.SignificantWhitespace)
                        continue;
                    if (CanGroupNodes(items.LastNode(), curr))
                    {
                        if (items.Last().type != ItemType.Table)
                        {
                            items.Fork();
                            items.Last().type = ItemType.Table;
                        }
                        items.Add(ItemType.Table, curr);
                    }
                    else
                        if ((curr.NodeType != XPathNodeType.Text && IsPairNode(curr)) ||
                            curr.NodeType == XPathNodeType.ProcessingInstruction)
                            items.Add(ItemType.Values, curr);
                        else
                            items.Add(ItemType.List, curr);
                } while (curr.MoveToNext());
            ParseNodes(cell, items);
        }

        public void ParseNodes(GridCellGroup cell, List<XPathNavigator> nodeList)
        {
            ItemList items = new ItemList();
            foreach (XPathNavigator curr in nodeList)
            {
                if (curr.NodeType == XPathNodeType.SignificantWhitespace)
                    continue;
                if (CanGroupNodes(items.LastNode(), curr))
                {
                    if (items.Last().type != ItemType.Table)
                    {
                        items.Fork();
                        items.Last().type = ItemType.Table;
                    }
                    items.Add(ItemType.Table, curr);
                }
                else
                    if ((curr.NodeType != XPathNodeType.Text && IsPairNode(curr)) ||
                        curr.NodeType == XPathNodeType.ProcessingInstruction)
                        items.Add(ItemType.Values, curr);
                    else
                        items.Add(ItemType.List, curr);
            }
            ParseNodes(cell, items);
        }

        public void ParseNodes(GridCellGroup cell, List<XPathItem> itemList)
        {
            cell.Table.SetBounds(1, itemList.Count);
            for (int k = 0; k < itemList.Count; k++)
            {
                XPathItem item = itemList[k];
                if (item.IsNode)
                {
                    XPathNavigator nav = (XPathNavigator)item;
                    GridCellGroup group = new XPathGroupCell(nav);
                    group.Flags = GroupFlags.Expanded | GroupFlags.Overlapped;
                    if (nav.NodeType == XPathNodeType.Root)
                        ParseChildNodes(group, nav);
                    else
                    {
                        ItemList items = new ItemList();
                        if ((nav.NodeType != XPathNodeType.Text && IsPairNode(nav)) ||
                            nav.NodeType == XPathNodeType.ProcessingInstruction)
                            items.Add(ItemType.Values, nav);
                        else
                            items.Add(ItemType.List, nav);
                        ParseNodes(group, items);
                    }
                    cell.Table[0, k] = group;
                }
                else
                    cell.Table[0, k] = new XPathItemCell(item);
            }
        }

        private void ParseNodes(GridCellGroup cell, ItemList items)
        {            
            if (items.Length == 1 && items[0].type == ItemType.Values)
            {
                cell.Table.SetBounds(2, items[0].nodes.Count);
                for (int s = 0; s < items[0].nodes.Count; s++)
                {
                    XPathNavigator nav = items[0].nodes[s];
                    cell.Table[0, s] = new XPathLabelCell(nav);
                    cell.Table[1, s] = new XPathValueCell(nav);
                }
            }
            else
            {
                int k = 0;
                cell.Table.SetBounds(1, items.CountCells());
                for (int i = 0; i < items.Length; i++)
                {
                    Item item = items[i];
                    switch (item.type)
                    {
                        case ItemType.Values:
                            {
                                GridCellGroup group = new GridCellGroup();
                                group.Flags = GroupFlags.Expanded | GroupFlags.Overlapped;
                                group.Table.SetBounds(2, item.nodes.Count);
                                for (int s = 0; s < item.nodes.Count; s++)
                                {
                                    XPathNavigator nav = item.nodes[s];
                                    group.Table[0, s] = new XPathLabelCell(nav);
                                    if (nav.NodeType == XPathNodeType.Root)
                                        group.Table[1, s] = new XPathDeclarationCell(nav);
                                    else
                                        group.Table[1, s] = new XPathValueCell(nav);
                                }
                                cell.Table[0, k++] = group;
                            }
                            break;

                        case ItemType.List:
                            for (int s = 0; s < item.nodes.Count; s++)
                                if (item.nodes[s].NodeType == XPathNodeType.Element)
                                    cell.Table[0, k++] = new XPathGroupCell(item.nodes[s]);
                                else
                                    cell.Table[0, k++] = new XPathLabelCell(item.nodes[s]);
                            break;

                        case ItemType.Table:
                            {
                                GridCellGroup group = new XPathGroupCell(item.nodes[0]);
                                group.Flags = group.Flags | GroupFlags.TableView;
                                TableColumns tableColumns = new TableColumns();
                                for (int s = 0; s < item.nodes.Count; s++)
                                    tableColumns = GroupNode(item.nodes[s], tableColumns);
                                group.Table.SetBounds(tableColumns.Length + 1, item.nodes.Count + 1);
                                group.Table[0, 0] = new GridRowLabel();
                                for (int s = 0; s < tableColumns.Length; s++)
                                    group.Table[s + 1, 0] = new XPathColumnLabelCell(tableColumns[s].nodeType,
                                        tableColumns[s].name, tableColumns[s].localName, tableColumns[s].ns, tableColumns[s].pos);
                                for (int s = 0; s < item.nodes.Count; s++)
                                {
                                    XPathNavigator node = item.nodes[s];
                                    group.Table[0, s + 1] = new XPathRowLabelCell(s + 1, node);
                                    for (int p = 0; p < tableColumns.Length; p++)
                                    {
                                        List<XPathNavigator> nodeList = GetNodeAtColumn(node, tableColumns[p]);
                                        if (nodeList.Count == 0)
                                            group.Table[p + 1, s + 1] = new XmlValueCell(null);
                                        else
                                        {
                                            XPathNavigator child = nodeList[0];
                                            if (nodeList.Count == 1)
                                            {
                                                if (child.NodeType != XPathNodeType.Element || IsPairNode(child))
                                                    group.Table[p + 1, s + 1] = new XPathValueCell(child);
                                                else
                                                    group.Table[p + 1, s + 1] = new XPathGroupCell(child);
                                            }
                                            else
                                            {
                                                XPathGroupCell childGroup = new XPathGroupCell(child);
                                                childGroup.Flags = GroupFlags.Overlapped | GroupFlags.Expanded;
                                                group.Table[p + 1, s + 1] = childGroup;                                                
                                                ParseNodes(childGroup, nodeList);
                                            }
                                        }
                                    }
                                }
                                cell.Table[0, k++] = group;
                            }
                            break;
                    }
                }
            }
        }
    }
}
