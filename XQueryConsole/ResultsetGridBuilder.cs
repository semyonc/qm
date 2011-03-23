//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using System.Data;

using DataEngine;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace WmHelp.XmlGrid
{
    public class ResultsetGridBuilder
    {
        public class DataValueCell : GridCell
        {
            public object Value { get; private set; }

            public DataValueCell(object value)
            {
                Value = value;
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
                    if (Value == DBNull.Value || Value == null)
                        return String.Empty;
                    else
                        return Value.ToString();
                }
                set
                {
                    return;
                }
            }
        }

        public class RootCell : GridCellGroup
        {
            public RowType RowType { get; private set; }

            public object Document { get; internal set; }

            public RootCell(RowType rowType)
                : base()
            {
                RowType = rowType;
            }
        }

        public class ResultsetColumnLabel : GridColumnLabel
        {
            private String _text;

            public ResultsetColumnLabel(String text)
                : base()
            {
                _text = text;
            }

            public override string Text
            {
                get
                {
                    return _text;
                }
            }

            public override int ImageIndex
            {
                get
                {
                    return -1;
                }
            }
        }

        public class ResultsetRowLabel : GridRowLabel
        {
            public ResultsetRowLabel(Row row, int index)
                : base()
            {
                RowNum = index;
                ResultsetRow = row;
            }

            public Row ResultsetRow { get; private set; }
        }

        public class ResultsetCell : GridCellGroup
        {
            public ResultsetCell(Resultset rs) :
                base()
            {
                Value = rs;
            }

            public override string Text
            {
                get
                {
                    return Parent.Table[Col, 0].Text;
                }
                set
                {
                    return;
                }
            }

            public Resultset Value { get; private set; }

            private bool _first_expand = false;

            public override void BeforeExpand()
            {
                if (!_first_expand)
                {
                    ResultsetGridBuilder binder = new ResultsetGridBuilder();
                    for (int s = 1; s < Table.Height; s++)
                    {
                        ResultsetRowLabel label = (ResultsetRowLabel)Table[0, s];
                        for (int k = 1; k < Table.Width; k++)
                            Table[k, s] = binder.GetCell(label.ResultsetRow.GetValue(k - 1));
                    }
                    _first_expand = true;
                }
            }
        }

        public class ArrayCell : GridCellGroup
        {
            private string _text;

            public ArrayCell(Type type)
            {
                _text = type.Name;
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

        public class ValueTupleCell : GridCellGroup
        {
            public ValueTuple Tuple { get; private set; }

            public ValueTupleCell(ValueTuple tuple)
            {
                Tuple = tuple;
            }

            public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font, Brush brush, 
                StringFormat format, XmlGridView.DrawInfo drawInfo, Rectangle rect)
            {
                StringFormat sf = new StringFormat(format);
                Font f = new Font(font, FontStyle.Italic);
                Brush textBrush = new SolidBrush(SystemColors.GrayText);
                sf.LineAlignment = StringAlignment.Center;
                rect.Height = drawInfo.cyChar;
                graphics.DrawString(Text, f, brush, rect, sf);              
            }

            public override string Text
            {
                get
                {
                    return String.Format("({0})", Tuple.Name);
                }
                set
                {
                    return;
                }
            }
        }

        public class EmptyTupleCell : GridCell
        {
            public ValueTuple Tuple { get; private set; }

            public EmptyTupleCell(ValueTuple tuple)
            {
                Tuple = tuple;
            }

            public override StringFormat GetStringFormat()
            {
                StringFormat sf = base.GetStringFormat();
                sf.LineAlignment = StringAlignment.Near;
                sf.FormatFlags = 0;
                return sf;
            }

            public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font, Brush brush,
                 StringFormat format, XmlGridView.DrawInfo drawInfo, Rectangle rect)
            {
                StringFormat sf = new StringFormat(format);
                Font f = new Font(font, FontStyle.Italic);
                Brush textBrush = new SolidBrush(SystemColors.GrayText);
                sf.LineAlignment = StringAlignment.Center;
                rect.Height = drawInfo.cyChar;
                graphics.DrawString(Text, f, brush, rect, sf);
            }

            public override string Text
            {
                get
                {
                    return String.Format("({0})", Tuple.Name);
                }
                set
                {
                    return;
                }
            }
        }

        public class GridCellXmlGroup : GridCellGroup
        {
            public Object Node { get; private set; }

            public GridCellXmlGroup(object node)
            {
                Node = node;
            }
        }

        public bool ShowColumnHeader { get; private set; }
        public bool CanExportXml { get; private set; }
        public bool CanExportDS { get; private set; }
        public int TableLimit { get; set; }
        public bool IsTruncated { get; set; }

        public GridCellGroup CreateResultsetCell(Resultset rs)
        {
            rs.Fill();
            GridCellGroup group = new ResultsetCell(rs);
            group.Flags = group.Flags | GroupFlags.TableView;
            group.Table.SetBounds(rs.RowType.Fields.Length + 1, rs.Count + 1);
            group.Table[0, 0] = new GridRowLabel();
            for (int k = 0; k < rs.RowType.Fields.Length; k++)
                group.Table[k + 1, 0] = new ResultsetColumnLabel(rs.RowType.Fields[k].Name);
            int s = 1;
            for (Row r = rs.Begin; r != null; r = rs.NextRow(r), s++)
            {
                group.Table[0, s] = new ResultsetRowLabel(r, s);
                for (int k = 0; k < rs.RowType.Fields.Length; k++)
                    group.Table[k + 1, s] = new DataValueCell(null);
            }
            return group;
        }

        protected GridCell GetCell(Object value)
        {
            if (value != null)
            {
                if (value is XmlElement &&
                    !XmlDataAccessor.IsPairNode((XmlNode)value))
                    return new XmlGroupCell((XmlNode)value);
                else if (value is XmlNodeList || value is XmlNode || value is XmlDocument)
                {
                    GridBuilder builder = new GridBuilder();
                    GridCellGroup group = new GridCellXmlGroup(value);
                    group.Flags = GroupFlags.Expanded | GroupFlags.Overlapped;
                    if (value is XmlDocument)
                        builder.ParseNodes(group, null, ((XmlNode)value).ChildNodes);
                    else if (value is XmlNode)
                    {
                        XmlDataAccessor.NodeList nodeList =
                            new XmlDataAccessor.NodeList();
                        nodeList.Add((XmlNode)value);
                        builder.ParseNodes(group, null, nodeList);
                    }
                    else
                        builder.ParseNodes(group, null, (XmlNodeList)value);
                    return group;
                }
                else if (value is XPathItem)
                {
                    XPathGridBuilder builder = new XPathGridBuilder();
                    GridCellGroup group = new GridCellXmlGroup(value);
                    group.Flags = GroupFlags.Expanded | GroupFlags.Overlapped;
                    XPathNavigator nav = value as XPathNavigator;
                    if (nav != null && nav.NodeType == XPathNodeType.Root)
                        builder.ParseChildNodes(group, (XPathNavigator)value);
                    else
                    {
                        List<XPathItem> list = new List<XPathItem>();
                        list.Add((XPathItem)value);
                        builder.ParseNodes(group, list);
                    }
                    return group;
                }
                else if (value is Resultset)
                    return CreateResultsetCell((Resultset)value);
                else if (value is Array)
                {
                    Array array = (Array)value;
                    GridCellGroup group = new ArrayCell(array.GetType());
                    if (array.Rank == 1 || array.Rank > 2)
                        group.Table.SetBounds(1, array.Length);
                    else
                        group.Table.SetBounds(array.GetUpperBound(0) - array.GetLowerBound(0),
                            array.GetUpperBound(1) - array.GetLowerBound(1));
                    int k = 0;
                    int s = 0;
                    foreach (Object item in array)
                    {
                        group.Table[k++, s] = GetCell(item);
                        if (k == group.Table.Width)
                        {
                            k = 0;
                            s++;
                        }
                    }
                    return group;
                }
                else if (value is ValueTuple)
                {
                    ValueTuple tuple = (ValueTuple)value;
                    if (tuple.Empty)
                        return new EmptyTupleCell(tuple);
                    else
                    {
                        GridCellGroup group = new ValueTupleCell(tuple);
                        group.Table.SetBounds(2, tuple.Values.Count);
                        int s = 0;
                        foreach (DictionaryEntry entry in tuple.Values)
                        {
                            group.Table[0, s] = new DataValueCell(entry.Key);
                            group.Table[1, s++] = GetCell(entry.Value);
                        }
                        return group;
                    }
                }
                else
                    return new DataValueCell(value);
            }
            else
                return new DataValueCell(null);
        }

        public GridCellGroup Parse(Resultset rs)
        {
            GridBuilder builder = new GridBuilder();
            RootCell rootCell = new RootCell(rs.RowType);
            rs.Fill(2);
            CanExportDS = CanExportXml = false;
            if (rs.Count == 1 && rs.RowType.Fields.Length == 1 &&
                 (rs.Begin.GetValue(0) is XmlNode || rs.Begin.GetValue(0) is XPathNavigator))
            {
                rootCell.Document = rs.Begin.GetValue(0);
                ShowColumnHeader = false;
                rootCell.Table.SetBounds(1, 1);
                rootCell.Table[0, 0] = (GridCellGroup)GetCell(rootCell.Document);
                rs.Truncate();
                CanExportXml = true;
            }
            else
            {
                if (TableLimit == 0)
                    rs.Fill();
                else
                    rs.Fill(TableLimit - 1);
                ShowColumnHeader = true;
                CanExportDS = true;
                foreach (RowType.TypeInfo ti in rs.RowType.Fields)
                    if (ti.DataType == typeof(System.Object) ||
                        ti.DataType == typeof(Resultset))
                    {
                        CanExportDS = false;
                        break;
                    }
                rootCell.Table.SetBounds(rs.RowType.Fields.Length, rs.Count + 1);
                for (int k = 0; k < rootCell.Table.Width; k++)
                {
                    GridHeadLabel label = new GridHeadLabel();
                    label.Text = Util.UnquoteName(rs.RowType.Fields[k].Name);
                    rootCell.Table[k, 0] = label;
                }
                int s = 1;
                while (rs.Begin != null)
                {
                    if (TableLimit > 0 && s > TableLimit)
                    {
                        IsTruncated = true;
                        break;
                    }
                    Row r = rs.Dequeue();
                    for (int k = 0; k < rootCell.Table.Width; k++)
                        rootCell.Table[k, s] = GetCell(r.GetValue(k));
                    s++;
                }
            }
            return rootCell;
        }

        private class GridDemandContext : QueryNode.DemandProcessingContext
        {
            private GridCellTable m_cellTable;
            private int m_pos;

            public GridDemandContext(GridCellTable cellTable)
                : base(null)
            {
                m_cellTable = cellTable;
                m_pos = 1;
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                if (m_pos < m_cellTable.Height)
                {
                    Row row = rs.NewRow();
                    for (int k = 0; k < m_cellTable.Width; k++)
                    {
                        GridCell baseCell = m_cellTable[k, m_pos];
                        if (baseCell is ResultsetGridBuilder.DataValueCell)
                        {
                            ResultsetGridBuilder.DataValueCell cell = (ResultsetGridBuilder.DataValueCell)baseCell;
                            if (cell.Value != null)
                                row.SetValue(k, cell.Value);
                        }
                        else if (baseCell is ResultsetGridBuilder.ResultsetCell)
                        {
                            ResultsetGridBuilder.ResultsetCell cell = (ResultsetGridBuilder.ResultsetCell)baseCell;
                            row.SetObject(k, cell.Value);
                        }
                        else if (baseCell is GridCellXmlGroup)
                        {
                            GridCellXmlGroup group = (GridCellXmlGroup)baseCell;
                            row.SetObject(k, group.Node);
                        }
                        else if (baseCell is ArrayCell)
                        {
                            GridCellGroup group = (ArrayCell)baseCell;
                            Array arr = Array.CreateInstance(rs.RowType.Fields[k].DataType.GetElementType(),
                                group.Table.Height * group.Table.Width);
                            int index = 0;
                            for (int s = 0; s < group.Table.Height; s++)
                                for (int p = 0; p < group.Table.Width; p++)
                                {
                                    GridCell cell = group.Table[p, s];
                                    if (cell is ResultsetGridBuilder.DataValueCell)
                                        arr.SetValue(((ResultsetGridBuilder.DataValueCell)cell).Value, index);
                                    index++;
                                }
                            row.SetObject(k, arr);
                        }
                        else if (baseCell is ValueTupleCell)
                            row.SetObject(k, ((ValueTupleCell)baseCell).Tuple);
                        else if (baseCell is EmptyTupleCell)
                            row.SetObject(k, ((EmptyTupleCell)baseCell).Tuple);
                    }
                    m_pos++;
                    rs.Enqueue(row);
                    return true;
                }
                else
                    return false;
            }
        }

        public Resultset CreateResultset(RootCell rootCell)
        {
            if (rootCell.Document != null)
            {
                DataTable dt = RowType.CreateSchemaTable();
                DataRow r = dt.NewRow();
                r["ColumnName"] = "node";
                r["ColumnOrdinal"] = 0;
                r["DataType"] = typeof(System.Object);
                r["IsContainer"] = true;
                dt.Rows.Add(r);
                Resultset rs = new Resultset(new RowType(dt), null);
                Row row = rs.NewRow();
                row[0] = rootCell.Document;
                rs.Enqueue(row);
                return rs;
            }
            return new Resultset(rootCell.RowType, new GridDemandContext(rootCell.Table));
        }
    }
}
