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
using System.Data;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    class DetailJoin: QueryNode
    {
        private class JoinNode
        {
            public Row row;
            public RowType type;
            public Resultset nested_rs;
        }

        public bool Outer { get; set; }

        public DetailJoin()
        {
            _childs = new QueryNodeCollection(this);
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            DynatableAccessor accessor = (DynatableAccessor)ChildNodes[1];
            return CreateDyntable(accessor, queryContext, parameters);
        }

        private Resultset CreateDyntable(DynatableAccessor accessor, QueryContext queryContext, object[] parameters)
        {
            List<JoinNode> nodes = new List<JoinNode>();
            Resultset src1 = ChildNodes[0].Get(queryContext, parameters);
            if (accessor.Strong)
                while (src1.Begin != null)
                {
                    Row row1 = src1.Dequeue();
                    Resultset src2 = accessor.Get(src1, row1, queryContext, parameters);
                    if (src2 != null)
                    {
                        JoinNode node = new JoinNode();
                        node.row = row1;
                        node.type = src2.RowType.Fields[0].NestedType;
                        if (node.type.Fields.Length == 0)
                            throw new ESQLException(Properties.Resources.DyntableEmpty, src2.RowType.Fields[0].Name);
                        node.nested_rs = src2;
                        nodes.Add(node);
                        break;
                    }
                    else
                        if (Outer)
                        {
                            JoinNode node = new JoinNode();
                            node.row = row1;
                            node.nested_rs = null;
                            nodes.Add(node);
                        }
                }
            else
            {
                Iterator.ForEach<Row>(src1, row1 =>
                {
                    Resultset src2 = accessor.Get(src1, row1, queryContext, parameters);
                    if (src2 != null)
                    {
                        JoinNode node = new JoinNode();
                        node.row = row1;
                        node.type = src2.RowType.Fields[0].NestedType;
                        if (node.type.Fields.Length == 0)
                            throw new ESQLException(Properties.Resources.DyntableEmpty, src2.RowType.Fields[0].Name);
                        node.nested_rs = src2;
                        lock (nodes)
                            nodes.Add(node);
                    }
                    else
                        if (Outer)
                        {
                            JoinNode node = new JoinNode();
                            node.row = row1;
                            node.nested_rs = null;
                            lock (nodes)
                                nodes.Add(node);
                        }
                });
                src1.Clear();
            }
            Dictionary<RowType.TypeInfo, RowType.TypeInfo> fields_set =
                new Dictionary<RowType.TypeInfo, RowType.TypeInfo>(new FieldComparer());
            foreach (JoinNode node in nodes)
                foreach (RowType.TypeInfo nested_ti in node.type.Fields)
                    if (!nested_ti.IsHidden)
                    {
                        RowType.TypeInfo src_ti;
                        if (fields_set.TryGetValue(nested_ti, out src_ti))
                        {
                            if (src_ti.DataType == typeof(DBNull) && nested_ti.DataType != typeof(DBNull))
                                fields_set[nested_ti] = nested_ti;
                        }
                        else
                            fields_set[nested_ti] = nested_ti;
                    }
            List<string> fieldNames = new List<string>();
            RowType.TypeInfo[] fields = new RowType.TypeInfo[fields_set.Count];
            Dictionary<RowType.TypeInfo, int> map = new Dictionary<RowType.TypeInfo, int>(new FieldComparer());
            int k = 0;
            foreach (RowType.TypeInfo ti in fields_set.Values)
            {
                fields[k] = new RowType.TypeInfo(k, Util.CreateUniqueName(fieldNames, ti.Name), ti);
                map.Add(ti, k);
                k++;
            }
            RowType nested_type = new RowType(fields);
            DataTable dt = src1.RowType.GetSchemaTable();
            DataRow r = dt.NewRow();
            r["ColumnName"] = accessor.Name;
            r["ColumnOrdinal"] = dt.Rows.Count;
            r["DataType"] = typeof(Row);
            r["NestedType"] = nested_type;
            dt.Rows.Add(r);

            EnumeratorProcessingContext context = new EnumeratorProcessingContext(new Resultset[] { src1 });
            Resultset rs = new Resultset(new RowType(dt), context);
            context.Iterator = NextRow(rs, NextNode(nodes, src1, accessor, queryContext, parameters), nested_type, map);
            return rs;
        }

        private IEnumerable<JoinNode> NextNode(List<JoinNode> nodes, Resultset src, 
            DynatableAccessor accessor, QueryContext queryContext, object[] parameters)
        {
            foreach (JoinNode node in nodes)
                yield return node;
            while (src.Begin != null)
            {
                Row row1 = src.Dequeue();
                Resultset src2 = accessor.Get(src, row1, queryContext, parameters);
                if (src2 != null)
                {
                    JoinNode node = new JoinNode();
                    node.row = row1;
                    node.type = src2.RowType.Fields[0].NestedType;
                    if (node.type.Fields.Length == 0)
                        throw new ESQLException(Properties.Resources.DyntableEmpty, src2.RowType.Fields[0].Name);
                    node.nested_rs = src2;
                    yield return node;
                }
                else
                    if (Outer)
                    {
                        JoinNode node = new JoinNode();
                        node.row = row1;
                        node.nested_rs = null;
                        yield return node;
                    }
            }
        }

        private IEnumerator<Row> NextRow(Resultset rs, IEnumerable<JoinNode> nodes, 
            RowType nested_type, Dictionary<RowType.TypeInfo, int> map)
        {
            foreach (JoinNode node in nodes)
                if (node.nested_rs != null)
                {
                    while (node.nested_rs.Begin != null)
                    {
                        Row row = node.nested_rs.Dequeue();
                        Row dest = rs.NewRow();
                        for (int k = 0; k < node.row.Length; k++)
                            dest.SetObject(k, node.row.GetObject(k));
                        Row detail_row = new Row(nested_type);
                        dest.SetObject(node.row.Length, detail_row);

                        Row data = (Row)row.GetObject(0);
                        for (int k = 0; k < node.type.Fields.Length; k++)
                        {
                            int pos = map[node.type.Fields[k]];
                            detail_row.SetValue(pos, data.GetValue(k));
                        }
                        yield return dest;
                    }
                }
                else
                {
                    Row dest = rs.NewRow();
                    for (int k = 0; k < node.row.Length; k++)
                        dest.SetObject(k, node.row.GetObject(k));
                    yield return dest;
                }
        }

        private class FieldComparer : IEqualityComparer<RowType.TypeInfo>
        {
            #region IEqualityComparer<TypeInfo> Members

            public bool Equals(RowType.TypeInfo x, RowType.TypeInfo y)
            {
                return x.Name.Equals(y.Name) && (x.DataType == y.DataType || 
                    x.DataType == typeof(DBNull) || y.DataType == typeof(DBNull));
            }

            public int GetHashCode(RowType.TypeInfo obj)
            {
                return obj.Name.GetHashCode();
            }

            #endregion
        }        
    }
}
