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
    public enum JoinMethod
    {
        NestedLoops,
        MergeJoin,
        HashJoin,
        SemiJoin,
        DistributedJoin
    };

    public enum JoinSpec
    {
        Inner,
        LeftOuter,
        RightOuter
    };

    public class DataJoin: QueryNode
    {
        public static int DistributedJoinSize = 64;

        public struct Key
        {
            public int r1;
            public int c1;
            public int r2;
            public int c2;
            public string name1;
            public string name2;
        }

        public abstract class KeyResolver
        {
            public abstract Key[] GetJoinKeys(Binder binder1, Binder binder2, JoinSpec spec);

            public virtual bool IsNatural { get { return false; } }
        }

        public class QualifiedJoin : KeyResolver
        {
            private String[] _k1;
            private String[] _k2;

            public QualifiedJoin(String[] k1, String[] k2)
            {
                _k1 = k1;
                _k2 = k2;

                if (k1.Length != k2.Length)
                    throw new InvalidOperationException();
            }

            public override Key[] GetJoinKeys(Binder binder1, Binder binder2, JoinSpec spec)
            {
                Key[] keys = new Key[_k1.Length];
                for (int k = 0; k < _k1.Length; k++)
                {
                    keys[k] = new Key();
                    RowType.Locator loc1 = new RowType.Locator();
                    RowType.Locator loc2 = new RowType.Locator();
                    if (!(binder1.GetLocator(_k1[k], ref loc1) || binder2.GetLocator(_k1[k], ref loc1)))
                        throw new ESQLException(Properties.Resources.InvalidIdentifier, _k1[k]);
                    if (!(binder1.GetLocator(_k2[k], ref loc2) || binder2.GetLocator(_k2[k], ref loc2)))
                        throw new ESQLException(Properties.Resources.InvalidIdentifier, _k2[k]);
                    if (binder1.GetLocator(_k1[k], ref loc1))
                        loc2 = binder2.GetLocator(_k2[k]);
                    else 
                    {
                        loc1 = binder1.GetLocator(_k2[k]);
                        loc2 = binder2.GetLocator(_k1[k]);
                    }
                    keys[k].r1 = loc1.master;
                    keys[k].c1 = loc1.detail.Value;
                    keys[k].r2 = loc2.master;
                    keys[k].c2 = loc2.detail.Value;
                }
                return keys;
            }

#if DEBUG
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < _k1.Length; k++)
                {
                    if (k > 0)
                        sb.Append(" && ");
                    sb.AppendFormat("{0} == {1}", _k1[k], _k2[k]);
                }
                return sb.ToString();
            }
#endif
        }

        public class NaturalJoin : KeyResolver
        {
            public NaturalJoin()
            {
            }

            public override Key[] GetJoinKeys(Binder binder1, Binder binder2, JoinSpec spec)
            {
                //foreach (RowType.TypeInfo ti in rs1.RowType.Fields)
                //    foreach (RowType.TypeInfo ti_det in ti.NestedType.Fields)
                //        if (!ti_det.IsHidden && !names1.Contains(ti_det.Name))
                //            names1.Add(ti_det.Name);
                List<String> names1 = new List<string>();
                foreach (ColumnBinding b in binder1.Bindings)
                    if (!names1.Contains(b.Name))
                        names1.Add(b.Name);
                List<String> names2 = new List<string>();
                foreach (ColumnBinding b in binder2.Bindings)
                    if (!names2.Contains(b.Name))
                        names2.Add(b.Name);
                List<Key> keys = new List<Key>();
                foreach (String s in names1.Intersect(names2))
                {
                    Key key = new Key();
                    RowType.Locator loc1 = binder1.GetLocator(s);
                    RowType.Locator loc2 = binder2.GetLocator(s);
                    key.r1 = loc1.master;
                    key.c1 = loc1.detail.Value;
                    key.r2 = loc2.master;
                    key.c2 = loc2.detail.Value;
                    key.name1 = binder1.GetName(loc1);
                    key.name2 = binder2.GetName(loc2);
                    keys.Add(key);

                }
                return keys.ToArray();
            }

            public override bool IsNatural
            {
                get
                {
                    return true;
                }
            }
        }

        public class NamedColumnsJoin : KeyResolver
        {
            private String[] _columns;
            
            public NamedColumnsJoin(String[] columns)
            {
                _columns = columns;
            }

            public override Key[] GetJoinKeys(Binder binder1, Binder binder2, JoinSpec spec)
            {
                Key[] keys = new Key[_columns.Length];
                for (int k = 0; k < _columns.Length; k++)
                {
                    keys[k] = new Key();
                    RowType.Locator loc1 = binder1.GetLocator(_columns[k]);
                    RowType.Locator loc2 = binder2.GetLocator(_columns[k]);                    
                    keys[k].r1 = loc1.master;
                    keys[k].c1 = loc1.detail.Value;
                    keys[k].r2 = loc2.master;
                    keys[k].c2 = loc2.detail.Value;
                    keys[k].name1 = binder1.GetName(loc1);
                    keys[k].name2 = binder2.GetName(loc2);
                }
                return keys;
            }

            public override bool IsNatural
            {
                get
                {
                    return true;
                }
            }
        }

        private KeyResolver _resolver;
        private object _filterPredicate;
        private JoinMethod _joinMethod;
        private JoinSpec _joinSpec;
        //private RowType _nested_ti;
        //private bool _isNatural;

        public object FilterPredicate 
        { 
            get 
            { 
                return _filterPredicate; 
            }
            set
            {
                if (_filterPredicate != null && _resolver != null)
                    throw new InvalidOperationException();
                _filterPredicate = value;
            }
        }
       
        public DataJoin(JoinMethod joinMethod, KeyResolver resolver, JoinSpec joinSpec)
        {
            _resolver = resolver;
            _joinSpec = joinSpec;
            _joinMethod = joinMethod;
            _childs = new QueryNodeCollection(this);
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            if (ChildNodes.Count != 2)
                throw new InvalidOperationException();

            Resultset rs1 = null;
            Resultset rs2 = null;

            Iterator.Invoke(new Action[] {
                () => rs1 = ChildNodes[0].Get(queryContext, parameters),
                () => rs2 = ChildNodes[1].Get(queryContext, parameters)
            });

            DataTable dt = RowType.CreateSchemaTable();
            DataTable dt1 = rs1.RowType.GetSchemaTable();
            DataTable dt2 = rs2.RowType.GetSchemaTable();
            
            Key[] keys = null;
            RowType nested_ti = null;
            if (_resolver != null)
            {
                Binder binder1 = new Binder(rs1);
                Binder binder2 = new Binder(rs2);
                keys = _resolver.GetJoinKeys(binder1, binder2, _joinSpec);
                if (_resolver.IsNatural)
                {
                    RowType.TypeInfo[] fields = new RowType.TypeInfo[keys.Length];
                    for (int k = 0; k < keys.Length; k++)
                    {
                        RowType.Locator loc1 = binder1.GetLocator(keys[k].name1);
                        RowType.Locator loc2 = binder2.GetLocator(keys[k].name2);
                        fields[k] = new RowType.TypeInfo(k,
                            rs1.GetFieldType(loc1), rs2.GetFieldType(loc2));
                        fields[k].IsNatural = true;
                    }
                    nested_ti = new RowType(fields);
                    DataRow r = dt.NewRow();
                    r["ColumnName"] = "$natural";
                    r["ColumnOrdinal"] = 0;
                    r["DataType"] = typeof(Row);
                    r["NestedType"] = nested_ti;
                    dt.Rows.Add(r);
                }
            }
            
            if (_joinSpec != JoinSpec.RightOuter)
            {
                foreach (DataRow r1 in dt1.Select())
                {
                    DataRow r = dt.NewRow();
                    r.ItemArray = r1.ItemArray;
                    r["ColumnOrdinal"] = dt.Rows.Count;
                    dt.Rows.Add(r);
                }
                foreach (DataRow r2 in dt2.Select())
                {
                    DataRow r = dt.NewRow();
                    r.ItemArray = r2.ItemArray;
                    r["ColumnOrdinal"] = dt.Rows.Count;
                    dt.Rows.Add(r);
                }
            }
            else
            {
                foreach (DataRow r2 in dt2.Select())
                {
                    DataRow r = dt.NewRow();
                    r.ItemArray = r2.ItemArray;
                    r["ColumnOrdinal"] = dt.Rows.Count;
                    dt.Rows.Add(r);
                }
                foreach (DataRow r1 in dt1.Select())
                {
                    DataRow r = dt.NewRow();
                    r.ItemArray = r1.ItemArray;
                    r["ColumnOrdinal"] = dt.Rows.Count;
                    dt.Rows.Add(r);
                }
            }

            RowType type = new RowType(dt);
            if (FilterPredicate != null)
                return new Resultset(type, new JoinFilterContext(this, rs1, rs2, type, queryContext, parameters));
            else
            {
                EnumeratorProcessingContext context = new EnumeratorProcessingContext(new Resultset[] { rs1, rs2 });
                Resultset rs = new Resultset(type, context);
                if (nested_ti != null)
                    foreach (Key k in keys)
                    {
                        RowType.Locator loc1;
                        RowType.Locator loc2;
                        if (_joinSpec != JoinSpec.RightOuter)
                        {
                            loc1 = RowType.Locator.Create(1 + k.r1, k.c1);
                            loc2 = RowType.Locator.Create(1 + dt1.Rows.Count + k.r2, k.c2);
                        }
                        else
                        {
                            loc1 = RowType.Locator.Create(1 + dt2.Rows.Count + k.r1, k.c1);
                            loc2 = RowType.Locator.Create(1 + k.r2, k.c2);
                        }
                        RowType.TypeInfo ti1 = rs.GetFieldType(loc1);
                        RowType.TypeInfo ti2 = rs.GetFieldType(loc2);
                        ti1.IsHidden = ti2.IsHidden = true;
                    }
                switch (_joinMethod)
                {
                    case JoinMethod.NestedLoops:
                        if (keys != null && (IsSmartTableAccessor(ChildNodes[0]) || IsSmartTableAccessor(ChildNodes[1])))
                            context.Iterator = OptimizedJoin(rs, rs1, rs2, keys, nested_ti, queryContext, parameters);
                        else
                            if (keys != null)
                                context.Iterator = HashJoin(rs, rs1, rs2, keys, nested_ti);
                            else
                                context.Iterator = NestedLoops(rs, rs1, rs2, keys, nested_ti);                            
                        break;

                    case JoinMethod.MergeJoin:
                        context.Iterator = MergeJoin(rs, rs1, rs2, keys, nested_ti);
                        break;

                    case JoinMethod.DistributedJoin:
                        context.Iterator = DistributedJoin(rs, rs1, rs2, keys,nested_ti, queryContext, parameters);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return rs;
            }
        }


        // Runtime join optimization
        private bool IsSmartTableAccessor(QueryNode node)
        {
            return (node is DataCollector) &&
               (node.ChildNodes[0] is DataProviderTableAccessor) &&
                 ((DataProviderTableAccessor)node.ChildNodes[0]).TableType.Smart;
        }

        private const int Threshold1 = 10000;
        private const int Threshold2 = 50000;
        private const int Threshold3 = 150000;

        private int NextThreshold(int threshold)
        {
            switch (threshold)
            {
                case 0:
                    return Threshold1;
                case Threshold1:
                    return Threshold2;
                case Threshold2:
                    return Threshold3;
                default:
                    return -1;
            }
        }

        private void EstimateNodes(QueryContext queryContext, Resultset rs1, out int C1, Resultset rs2, out int C2)
        {
            C1 = C2 = -1;
            int threshold1 = 0;
            int threshold2 = 0;            
            do
            {
                if (C1 == -1)
                {
                    if (IsSmartTableAccessor(ChildNodes[0]))
                    {
                        if (threshold1 == -1)
                            C1 = Threshold3;
                        else
                        {
                            DataProviderTableAccessor node1 = (DataProviderTableAccessor)(ChildNodes[0].ChildNodes[0]);
                            int threshold = threshold1;
                            int C = queryContext.GetTableEstimate(node1.TableType, threshold1 = NextThreshold(threshold1));
                            if (C != threshold1 || (C2 != 0 && C2 < threshold))
                                C1 = C;
                        }
                    }
                    else
                    {
                        rs1.Fill();
                        C1 = rs1.Count;
                    }
                }
                if (C2 == -1)
                {
                    if (IsSmartTableAccessor(ChildNodes[1]))
                    {
                        if (threshold2 == -1)
                            C2 = Threshold3;
                        else
                        {
                            DataProviderTableAccessor node2 = (DataProviderTableAccessor)(ChildNodes[1].ChildNodes[0]);
                            int threshold = threshold2;
                            int C = queryContext.GetTableEstimate(node2.TableType, threshold2 = NextThreshold(threshold2));
                            if (C != threshold2 || (C1 != 0 && C1 < threshold))
                                C2 = C;
                        }
                    }
                    else
                    {
                        rs2.Fill();
                        C2 = rs2.Count;
                    }
                }

            } while (C1 == -1 || C2 == -1);
        }

        protected IEnumerator<Row> OptimizedJoin(Resultset rs, Resultset rs1, Resultset rs2, Key[] keys, RowType nested_ti,
            QueryContext queryContext, object[] parameters)
        {
            int C1;
            int C2;
            EstimateNodes(queryContext, rs1, out C1, rs2, out C2);
            int delta = C2 - C1;
            int lim = (C1 + C2) / 2;
            if (delta > lim && IsSmartTableAccessor(ChildNodes[1]))
                return DistributedJoin(rs, rs1, rs2, keys, nested_ti, queryContext, parameters);
            else if (delta < -lim && _joinSpec == JoinSpec.Inner && IsSmartTableAccessor(ChildNodes[0]))
                return RightDistributedJoin(rs, rs1, rs2, keys, nested_ti, queryContext, parameters);
            else
            {
                if (IsSmartTableAccessor(ChildNodes[0]))
                {
                    DataProviderTableAccessor node1 = (DataProviderTableAccessor)(ChildNodes[0].ChildNodes[0]);
                    node1.SortColumns = new DataProviderTableAccessor.SortColumn[keys.Length];
                    for (int k = 0; k < keys.Length; k++)
                    {
                        node1.SortColumns[k] = new DataProviderTableAccessor.SortColumn();
                        node1.SortColumns[k].ColumnName = rs1.RowType.Fields[keys[k].r1].NestedType.Fields[keys[k].c1].BaseColumnName;
                    }                                
                }
                else
                {
                    RowType.Locator[] locator = new RowType.Locator[keys.Length];
                    for (int k = 0; k < keys.Length; k++)
                    {
                        locator[k] = new RowType.Locator();
                        locator[k].master = keys[k].r1;
                        locator[k].detail = keys[k].c1;
                    }
                    rs1.Sort(new ComplexComparer(locator));
                }
                if (IsSmartTableAccessor(ChildNodes[1]))
                {
                    DataProviderTableAccessor node2 = (DataProviderTableAccessor)(ChildNodes[1].ChildNodes[0]);
                    node2.SortColumns = new DataProviderTableAccessor.SortColumn[keys.Length];
                    for (int k = 0; k < keys.Length; k++)
                    {
                        node2.SortColumns[k] = new DataProviderTableAccessor.SortColumn();
                        node2.SortColumns[k].ColumnName = rs2.RowType.Fields[keys[k].r2].NestedType.Fields[keys[k].c2].BaseColumnName;
                    }
                }
                else
                {
                    RowType.Locator[] locator = new RowType.Locator[keys.Length];
                    for (int k = 0; k < keys.Length; k++)
                    {
                        locator[k] = new RowType.Locator();
                        locator[k].master = keys[k].r2;
                        locator[k].detail = keys[k].c2;
                    }
                    rs2.Sort(new ComplexComparer(locator));
                }
                return MergeJoin(rs, rs1, rs2, keys, nested_ti);
            }
        }

        //protected IEnumerator<Row> NestedLoops(Resultset rs, Resultset src1, Resultset src2, Key[] keys, RowType nested_ti)
        //{
        //    while (src1.Begin != null)
        //    {                
        //        Row row1 = src1.Dequeue();
        //        LinkedList<Row> rows = new LinkedList<Row>();
        //        Iterator.ForEach<Row>(src2, row2 =>
        //        {
        //            if (CompareRow(row1, row2, keys) == 0)
        //            {
        //                Row row = rs.NewRow();
        //                SetRow(keys, nested_ti, row, src1, row1, src2, row2);
        //                lock (rows)
        //                    rows.AddLast(row);
        //            }
        //        });
        //        foreach (Row row in rows)
        //            yield return row;
        //        if (rows.Count == 0 && _joinSpec != JoinSpec.Inner)
        //        {
        //            Row row = rs.NewRow();
        //            SetRow(keys, nested_ti, row, src1, row1, src2, null);
        //            yield return row;
        //        }
        //    }
        //    src2.Clear();
        //}

        protected IEnumerator<Row> NestedLoops(Resultset rs, Resultset src1, Resultset src2, Key[] keys, RowType nested_ti)
        {
            while (src1.Begin != null)
            {
                bool found = false;
                Row row1 = src1.Dequeue();
                for (Row row2 = src2.Begin; row2 != null; row2 = src2.NextRow(row2))
                    if (CompareRow(row1, row2, keys) == 0)
                    {
                        Row row = rs.NewRow();
                        SetRow(keys, nested_ti, row, src1, row1, src2, row2);
                        found = true;
                        yield return row;
                    }
                if (!found && _joinSpec != JoinSpec.Inner)
                {
                    Row row = rs.NewRow();
                    SetRow(keys, nested_ti, row, src1, row1, src2, null);
                    yield return row;
                }
            }
            src2.Clear();
        }

        protected class ItemComparer : IEqualityComparer<object[]>
        {
            public ItemComparer()
            {
            }

            #region IEqualityComparer<Row> Members

            public bool Equals(object[] x, object[] y)
            {
                if (x.Length != y.Length)
                    return false;
                for (int k = 0; k < x.Length; k++)
                    if (Row.CompareValue(x[k], y[k]) != 0)
                        return false;
                return true;
            }

            public int GetHashCode(object[] obj)
            {
                int hash = 23;
                for (int k = 0; k < obj.Length; k++)
                    hash = hash * 37 + obj[k].GetHashCode();
                return hash;
            }

            #endregion
        }

        protected IEnumerator<Row> HashJoin(Resultset rs, Resultset src1, Resultset src2, Key[] keys, RowType nested_ti)
        {
            Dictionary<object[], LinkedList<Row>> hs = 
                new Dictionary<object[], LinkedList<Row>>(new ItemComparer());
            while (src2.Begin != null)
            {
                Row row2 = src2.Dequeue();
                LinkedList<Row> list;
                object[] itemArray = new object[keys.Length];
                for (int k = 0; k < keys.Length; k++)
                {
                    Row row = (Row)row2.GetObject(keys[k].r2);
                    itemArray[k] = row.GetValue(keys[k].c2);
                }
                if (!hs.TryGetValue(itemArray, out list))
                {
                    list = new LinkedList<Row>();
                    hs.Add(itemArray, list);
                }
                list.AddLast(row2);
            }            
            while (src1.Begin != null)
            {
                Row row1 = src1.Dequeue();
                LinkedList<Row> list;
                object[] itemArray = new object[keys.Length];
                for (int k = 0; k < keys.Length; k++)
                {
                    Row row = (Row)row1.GetObject(keys[k].r1);
                    itemArray[k] = row.GetValue(keys[k].c1);
                }
                if (hs.TryGetValue(itemArray, out list))
                {
                    foreach (Row row2 in list)
                    {
                        Row row = rs.NewRow();
                        SetRow(keys, nested_ti, row, src1, row1, src2, row2);
                        yield return row;
                    }
                }
                else
                    if (_joinSpec != JoinSpec.Inner)
                    {
                        Row row = rs.NewRow();
                        SetRow(keys, nested_ti, row, src1, row1, src2, null);
                        yield return row;
                    }
            }
        }


        protected IEnumerator<Row> MergeJoin(Resultset rs, Resultset src1, Resultset src2, Key[] keys, RowType nested_ti)
        {
            bool found = false;
            while (src1.Begin != null && src2.Begin != null)
            {
                int i = CompareRow(src1.Begin, src2.Begin, keys);
                if (i == 0)
                {
                    Row row = rs.NewRow();
                    SetRow(keys, nested_ti, row, src1, src1.Begin, src2, src2.Begin);
                    found = true;
                    yield return row;
                    src2.Dequeue();
                }
                else if (i < 0)
                {
                    if (_joinSpec == JoinSpec.LeftOuter)
                    {
                        if (!found)
                        {
                            Row row = rs.NewRow();
                            SetRow(keys, nested_ti, row, src1, src1.Begin, src2, null);
                            yield return row;
                        }
                        else
                            found = false;
                    }
                    src1.Dequeue();
                }
                else /* i > 0 */
                {
                    if (_joinSpec == JoinSpec.RightOuter)
                    {
                        if (!found)
                        {
                            Row row = rs.NewRow();
                            SetRow(keys, nested_ti, row, src1, null, src2, src2.Begin);
                            yield return row;
                        }
                        else
                            found = false;
                    }
                    src2.Dequeue();
                }
            }
            if (_joinSpec == JoinSpec.LeftOuter)
                while (src1.Begin != null)
                {
                    Row row = rs.NewRow();
                    SetRow(keys, nested_ti, row, src1, src1.Begin, src2, null);
                    yield return row;
                    src1.Dequeue();
                }
            else if (_joinSpec == JoinSpec.RightOuter)
                while (src2.Begin != null)
                {
                    Row row = rs.NewRow();
                    SetRow(keys, nested_ti, row, src1, null, src2, src2.Begin);
                    yield return row;
                    src2.Dequeue();
                }
        }

        protected IEnumerator<Row> DistributedJoin(Resultset rs, Resultset src1, Resultset src2, Key[] keys, RowType nested_ti,
            QueryContext queryContext, Object[] parameters)
        {
            DataProviderTableAccessor node2 = (DataProviderTableAccessor)(ChildNodes[1].ChildNodes[0]);
            node2.AccessPredicate = new string[keys.Length];
            for (int k = 0; k < keys.Length; k++)
                node2.AccessPredicate[k] = src2.RowType.Fields[keys[k].r2].NestedType.Fields[keys[k].c2].BaseColumnName;
            List<Object[]> buffer = new List<Object[]>();
            Row current = src1.Begin;
            while (current != null)
            {
                buffer.Clear();
                for (int s = 0; s < DistributedJoinSize; s++)
                {
                    if (current == null)
                        break;
                    Object[] block = new object[keys.Length];
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Row r = (Row)current.GetObject(keys[k].r1);
                        block[k] = r == null ?
                            DBNull.Value : r.GetValue(keys[k].c1);
                    }
                    buffer.Add(block);
                    current = src1.NextRow(current);
                }
                node2.AccessPredicateValues = buffer.ToArray();
                src2 = ChildNodes[1].Get(queryContext, parameters);
                while (src1.Begin != current)
                {
                    bool found = false;
                    Row row1 = src1.Dequeue();
                    for (Row row2 = src2.Begin; row2 != null; row2 = src2.NextRow(row2))
                        if (CompareRow(row1, row2, keys) == 0)
                        {
                            Row row = rs.NewRow();
                            SetRow(keys, nested_ti, row, src1, row1, src2, row2);
                            found = true;
                            yield return row;
                        }
                    if (!found && _joinSpec != JoinSpec.Inner)
                    {
                        Row row = rs.NewRow();
                        SetRow(keys, nested_ti, row, src1, row1, src2, null);
                        yield return row;
                    }
                }
            }
        }

        protected IEnumerator<Row> RightDistributedJoin(Resultset rs, Resultset src1, Resultset src2, Key[] keys, RowType nested_ti,
            QueryContext queryContext, Object[] parameters)
        {
            DataProviderTableAccessor node1 = (DataProviderTableAccessor)(ChildNodes[0].ChildNodes[0]);
            node1.AccessPredicate = new string[keys.Length];
            for (int k = 0; k < keys.Length; k++)
                node1.AccessPredicate[k] = src1.RowType.Fields[keys[k].r1].NestedType.Fields[keys[k].c1].BaseColumnName;
            List<Object[]> buffer = new List<Object[]>();
            Row current = src2.Begin;
            while (current != null)
            {
                buffer.Clear();
                for (int s = 0; s < DistributedJoinSize; s++)
                {
                    if (current == null)
                        break;
                    Object[] block = new object[keys.Length];
                    for (int k = 0; k < keys.Length; k++)
                    {
                        Row r = (Row)current.GetObject(keys[k].r2);
                        block[k] = r == null ?
                            DBNull.Value : r.GetValue(keys[k].c2);
                    }
                    buffer.Add(block);
                    current = src2.NextRow(current);
                }
                node1.AccessPredicateValues = buffer.ToArray();
                src1 = ChildNodes[0].Get(queryContext, parameters);
                while (src2.Begin != current)
                {                    
                    Row row2 = src2.Dequeue();
                    for (Row row1 = src1.Begin; row1 != null; row1 = src1.NextRow(row1))
                        if (CompareRow(row1, row2, keys) == 0)
                        {
                            Row row = rs.NewRow();
                            SetRow(keys, nested_ti, row, src1, row1, src2, row2);
                            yield return row;
                        }
                }
            }
        }

        protected int CompareRow(Row row1, Row row2, Key[] keys)
        {
            if (keys != null)
                for (int k = 0; k < keys.Length; k++)
                {
                    Key kr = keys[k];
                    Row r1 = (Row)row1.GetObject(kr.r1);
                    Row r2 = (Row)row2.GetObject(kr.r2);
                    if (r1 == r2)
                        return 0;
                    else if (r1 == null)
                        return -1;
                    else if (r2 == null)
                        return 1;
                    else
                    {
                        int i = Row.CompareValue(r1.GetValue(kr.c1), r2.GetValue(kr.c2));
                        if (i != 0)
                            return i;
                    }
                }
            return 0;
        }

        protected void SetRow(Key[] keys, RowType nested_ti, 
            Row row, Resultset rs1, Row row1, Resultset rs2, Row row2)
        {
            int l = 0;
            if (nested_ti != null)
            {
                l = 1;
                Row r = new Row(nested_ti);
                for (int k = 0; k < keys.Length; k++)
                {
                    Row child = (Row)row1.GetObject(keys[k].r1);
                    if (child != null)
                    {
                        object value = child.GetValue(keys[k].c1);
                        if (value != null && value != DBNull.Value)
                            r.SetValue(k, TypeConverter.ChangeType(value, 
                                Type.GetTypeCode(nested_ti.Fields[k].DataType)));
                    }
                }
                row.SetObject(0, r);
            }
            int length1 = rs1.RowType.Fields.Length;
            int length2 = rs2.RowType.Fields.Length;
            if (_joinSpec == JoinSpec.Inner ||
                _joinSpec == JoinSpec.LeftOuter)
            {
                for (int k = 0; k < row1.Length; k++)
                    row.SetObject(l + k, (Row)row1.GetObject(k));
                if (row2 != null)
                    for (int k = 0; k < row2.Length; k++)
                        row.SetObject(l + k + length1, (Row)row2.GetObject(k));
            }
            else
            {
                for (int k = 0; k < row1.Length; k++)
                    row.SetObject(l + k + length2, (Row)row1.GetObject(k));
                if (row2 != null)
                    for (int k = 0; k < row2.Length; k++)
                        row.SetObject(l + k, (Row)row2.GetObject(k));
            }
        }

        protected class JoinFilterContext : LispProcessingContext
        {
            private DataJoin _owner;
            private Resultset _src1;
            private Resultset _src2;
            private DataResolver _resolver;
            private Object _filterPredicate;
            private Row _row1;
            private Row _row2;
            private bool _found;
            private FunctionLink _compiledBody;

            public JoinFilterContext(DataJoin owner, Resultset src1, Resultset src2, RowType rt, QueryContext queryContext, Object[] parameters)
                : base(owner, new Resultset[] {src1, src2}, queryContext, parameters)
            {
                _owner = owner;
                _src1 = src1;
                _src2 = src2;
                _compiledBody = new FunctionLink();

                List<ColumnBinding> bindings = new List<ColumnBinding>();
                ExpressionTransformer transformer = new ExpressionTransformer(bindings);
                foreach (RowType.TypeInfo ti in rt.Fields)
                    foreach (RowType.TypeInfo nested_ti in ti.NestedType.Fields)
                        if (!nested_ti.IsHidden)
                        {
                            ColumnBinding b = new ColumnBinding();
                            b.typecode = Type.GetTypeCode(nested_ti.DataType);
                            b.rnum = ti.Ordinal;
                            b.TableName = ti.Name;
                            b.Name = nested_ti.Name;
                            b.fieldType = nested_ti;
                            b.natural = nested_ti.IsNatural;
                            b.container = nested_ti.IsContainer;
                            b.caseSensitive = nested_ti.IsCaseSensitive;
                            b.data = new SymbolLink(nested_ti.DataType);
                            bindings.Add(b);
                            if (nested_ti.IsContainer)
                                transformer.NeedTransform = true;
                        }
                LispExecutive.Enter(_resolver = new DataResolver(new Binder(bindings)));
                if (transformer.NeedTransform)
                    _filterPredicate = transformer.Process(_owner.FilterPredicate);
                else
                    _filterPredicate = _owner.FilterPredicate;

            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                if (_src1.Begin == null)
                    return false;

                if (_row1 == null)
                {
                    _row1 = _src1.Dequeue();
                    _row2 = _src2.Begin;
                    _found = false;
                }
                
                int count = rs.Count;
                while (_row1 != null && count == rs.Count)
                {
                    Row row = rs.NewRow();
                    _owner.SetRow(null, null, row, _src1, _row1, _src2, _row2);
                    foreach (ColumnBinding b in _resolver.Bindings)
                        b.Set(row, LispExecutive);
                    
                    object status = LispExecutive.Apply(null, null, _filterPredicate, null, 
                        _compiledBody, LispExecutive.DefaultPool);
                    if (status != null && status != Undefined.Value)
                    {
                        _found = true;
                        rs.Enqueue(row);
                    }

                    _row2 = _src2.NextRow(_row2);
                    if (_row2 == null)
                    {
                        if (!_found && _owner._joinSpec != JoinSpec.Inner)
                        {
                            row = rs.NewRow();
                            _owner.SetRow(null, null, row, _src1, _row1, _src2, null);
                            rs.Enqueue(row);
                        }
                        _row1 = _src1.Begin != null ? 
                            _src1.Dequeue() : null;
                        _row2 = _src2.Begin;
                        _found = false;
                    }
                }
                
                return _src1.Begin != null;
            }
        }
#if DEBUG
        protected override void Dump(System.IO.TextWriter w, int padding)
        {
            OutlineNode(w, padding);
            w.Write("DataJoin {0},", _joinSpec);
            if (FilterPredicate != null)
                w.Write(Lisp.Format(FilterPredicate));
            else
            {
                if (_resolver != null)
                    w.Write(_resolver.ToString());
            }
            w.WriteLine();
        }
#endif

    }
}
