using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using System.Drawing;
using DataEngine.CoreServices;
using WmHelp.XmlGrid;


namespace SimpleTestConsole
{
    class ResultBuilder
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

        public class RootCell : GridCellGroup
        {
            public object Document { get; internal set; }

            public RootCell()
                : base()
            {
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

        public bool ShowColumnHeader { get; private set; }
        public bool CanExportXml { get; private set; }

        protected GridCell GetCell(Object value)
        {
            if (value != null)
            {
                if (value is XmlElement &&
                    !GridBuilder.IsPairNode((XmlNode)value))
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
                        NodeList nodeList = new NodeList();
                        nodeList.Add((XmlNode)value);
                        builder.ParseNodes(group, null, nodeList);
                    }
                    else
                        builder.ParseNodes(group, null, (XmlNodeList)value);
                    return group;
                }
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
                else
                    return new DataValueCell(value);
            }
            else
                return new DataValueCell(null);
        }

        public GridCellGroup Parse(List<object> nodes)
        {
            GridBuilder builder = new GridBuilder();
            RootCell rootCell = new RootCell();
            if (nodes.Count == 1 && nodes[0] is XmlNode)
            {
                rootCell.Document = nodes[0];
                ShowColumnHeader = false;
                rootCell.Table.SetBounds(1, 1);
                rootCell.Table[0, 0] = (GridCellGroup)GetCell(rootCell.Document);
                CanExportXml = true;
            }
            else
            {
                ShowColumnHeader = true;
                rootCell.Table.SetBounds(1, nodes.Count + 1);
                GridHeadLabel label = new GridHeadLabel();
                label.Text = String.Format("XQueryNodeIterator({0})", nodes.Count);
                rootCell.Table[0, 0] = label;
                int s = 1;
                foreach (object node in nodes)
                {
                    rootCell.Table[0, s] = GetCell(node);
                    s++;
                }
            }
            return rootCell;
        }
    }
}
