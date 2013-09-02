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
using System.Xml;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;
using System.Globalization;

namespace DataEngine
{
    public enum AggregateFunctor
    {        
        Count,        
        Min,
        Max,
        Sum,
        Avg,
        RowCount,
        XMLAgg
    }

    public enum AggregateType
    {
        Index,
        Stream
    }

    public class DataAggregator: QueryNode
    {
        public class Column
        {
            public String Name { get; set; }
            public Symbol Expr { get; set; }
            public String ColumnName { get; set; }
            public AggregateFunctor Functor { get; set; }
            public bool Distinct { get; set; }
            public Object[] OrderFields { get; set; }
            public SortDirection[] Direction { get; set; }

            public Column()
            {
            }

            public Column(String columnName, AggregateFunctor functor, bool distinct)
            {
                ColumnName = columnName;
                Functor = functor;
                Distinct = distinct;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                switch(Functor)
                {
                    case AggregateFunctor.Avg:
                        sb.Append("AVG(");
                        break;

                    case AggregateFunctor.Count:
                    case AggregateFunctor.RowCount:
                        sb.Append("COUNT(");
                        break;

                    case AggregateFunctor.Max:
                        sb.Append("MAX(");
                        break;

                    case AggregateFunctor.Min:
                        sb.Append("MIN(");
                        break;

                    case AggregateFunctor.Sum:
                        sb.Append("SUM(");
                        break;

                    case AggregateFunctor.XMLAgg:
                        sb.Append("XMLAgg(");
                        break;
                }
                if (Functor == AggregateFunctor.RowCount)
                    sb.Append("*");
                else
                {
                    if (Distinct)
                        sb.Append("DISTINCT ");
                    sb.Append(ColumnName);
                }
                sb.Append(")");
                return sb.ToString();
            }
        }

        public abstract class AggregatorFactory
        {
            protected RowType.Locator locator;
            protected Column column;

            public AggregatorFactory(Binder binder, Column column)
            {
                if (column.ColumnName != null)
                    locator = binder.GetLocator(column.ColumnName);                
                this.column = column;
            }

            public abstract Aggregator CreateAgregator();
        }
        

        public abstract class Aggregator
        {
            private RowType.Locator locator;
            private HashSet<Object> hash = null;

            public Aggregator(RowType.Locator locator, Column column)
            {
                this.locator = locator;
                if (column.Distinct)
                    hash = new HashSet<object>();
            }

            public virtual void Reset()
            {
                if (hash != null)
                    hash.Clear();
            }            

            public virtual void Set(Row row)
            {
                Object value = DBNull.Value;
                if (locator.detail == null)
                    value = row.GetValue(locator.master);
                else
                {
                    Row nested_row = (Row)row.GetObject(locator.master);
                    if (nested_row != null)
                        value = nested_row.GetValue(locator.detail.Value);
                }
                if (value != DBNull.Value && (hash == null || !hash.Contains(value)))
                {
                    Aggregate(value, row);
                    if (hash != null)
                        hash.Add(value);
                }
            }

            public abstract void Aggregate(Object value, Row row);

            public abstract Object Value { get; }
        }

        #region aggregators

        private sealed class RowCountFactory : AggregatorFactory
        {
            public RowCountFactory(Binder binder, Column column)
                : base(binder, column)
            {
            }

            public override Aggregator CreateAgregator()
            {
                return new RowCount(locator, column);
            }
        }

        /// <summary>
        /// Special stuff for count(*)
        /// </summary>       
        private sealed class RowCount : Aggregator
        {
            private int _count;

            public RowCount(RowType.Locator locator, Column column)
                : base(locator, column)
            {
            }

            public override void Reset()
            {
                base.Reset();
                _count = 0;
            }

            public override void Set(Row row)
            {
                _count++;    
            }

            public override void Aggregate(object value, Row row)
            {
                throw new NotImplementedException();
            }

            public override object Value
            {
                get { return _count; }
            }
        }

        private sealed class CountFactory : AggregatorFactory
        {
            public CountFactory(Binder binder, Column column)
                : base(binder, column)
            {
            }

            public override Aggregator CreateAgregator()
            {
                return new Count(locator, column);
            }
        }

        private sealed class Count : Aggregator
        {
            public int _count;

            public Count(RowType.Locator locator, Column column)
                : base(locator, column)
            {
            }

            public override void Reset()
            {
                base.Reset();
                _count = 0;
            }

            public override void  Aggregate(object value, Row row)
            {
 	            _count++;
            }

            public override object Value
            {
                get
                {
                    return _count;
                }
            }
        }

        private sealed class MinFactory : AggregatorFactory
        {
            public MinFactory(Binder binder, Column column)
                : base(binder, column)
            {
            }

            public override Aggregator CreateAgregator()
            {
                return new Min(locator, column);
            }
        }

        private sealed class Min : Aggregator
        {
            Object _value;

            public Min(RowType.Locator locator, Column column)
                : base(locator, column)
            {
            }

            public override void Reset()
            {
                base.Reset();
                _value = DBNull.Value;
            }

            public override void Aggregate(object value, Row row)
            {
                if (_value == DBNull.Value || ((IComparable)_value).CompareTo(value) == 1)
                    _value = value;
            }

            public override object Value
            {
                get { return _value; }
            }
        }

        private sealed class MaxFactory : AggregatorFactory
        {
            public MaxFactory(Binder binder, Column column)
                : base(binder, column)
            {
            }

            public override Aggregator CreateAgregator()
            {
                return new Max(locator, column);
            }
        }

        private sealed class Max : Aggregator
        {
            Object _value;

            public Max(RowType.Locator locator, Column column)
                : base(locator, column)
            {
            }

            public override void Reset()
            {
                base.Reset();
                _value = DBNull.Value;
            }

            public override void Aggregate(object value, Row row)
            {
                if (_value == DBNull.Value || ((IComparable)_value).CompareTo(value) == -1)
                    _value = value;
            }

            public override object Value
            {
                get { return _value; }
            }
        }

        private sealed class SumFactory : AggregatorFactory
        {
            public SumFactory(Binder binder, Column column)
                : base(binder, column)
            {
            }

            public override Aggregator CreateAgregator()
            {
                return new Sum(locator, column);
            }
        }

        private sealed class Sum : Aggregator
        {
            private double _value;
            private bool _hasValue;

            public Sum(RowType.Locator locator, Column column)
                : base(locator, column)
            {
            }

            public override void Reset()
            {
                base.Reset();
                _value = 0.0;
                _hasValue = false;
            }

            public override void Aggregate(object value, Row row)
            {
                _value += Convert.ToDouble(value);
                _hasValue = true;
            }

            public override object Value
            {
                get { return _hasValue ? (Object)_value : DBNull.Value; }
            }
        }

        private sealed class AvgFactory : AggregatorFactory
        {
            public AvgFactory(Binder binder, Column column)
                : base(binder, column)
            {
            }

            public override Aggregator CreateAgregator()
            {
                return new Avg(locator, column);
            }
        }

        private sealed class Avg : Aggregator
        {
            private double _value;
            private int _count;

            public Avg(RowType.Locator locator, Column column)
                : base(locator, column)
            {
            }

            public override void Reset()
            {
                base.Reset();
                _value = 0.0;
                _count = 0;
            }

            public override void Aggregate(object value, Row row)
            {
                _value += Convert.ToDouble(value);
                _count++;
            }

            public override object Value
            {
                get
                {
                    if (_count > 0)
                        return _value / _count;
                    else
                        return DBNull.Value;
                }
            }
        }

        private sealed class XMLAggFactory : AggregatorFactory
        {
            private QueryContext queryContext;

            public XMLAggFactory(Binder binder, Column column, QueryContext queryContext)
                : base(binder, column)
            {
                this.queryContext = queryContext;
            }

            public override Aggregator CreateAgregator()
            {
                return new XMLAgg(locator, column, queryContext);
            }
        }


        private sealed class XMLAgg : Aggregator
        {
            private XmlDataAccessor.NodeList nodes;
            private QueryContext queryContext;

            public XMLAgg(RowType.Locator locator, Column column, QueryContext queryContext)
                : base(locator, column)
            {
                this.queryContext = queryContext;
                nodes = new XmlDataAccessor.NodeList();
            }

            public override void Aggregate(object value, Row row)
            {
                if (value is XmlNode)
                    nodes.Add((XmlNode)value);
                else if (value is XmlNodeList)
                {
                    XmlNodeList l = (XmlNodeList)value;
                    foreach (XmlNode node in l)
                        nodes.Add(node);
                }
                else
                    nodes.Add(XmlDataAccessor.Serialize(queryContext.XmlResult, value));
            }

            public override object Value
            {
                get 
                { 
                    return nodes; 
                }
            }
        }

        private sealed class SortedXMLAggFactory : AggregatorFactory
        {
            private QueryContext queryContext;
            private ComplexComparer comparer;

            public SortedXMLAggFactory(Binder binder, Column column, QueryContext queryContext)
                : base(binder, column)
            {
                this.queryContext = queryContext;
                RowType.Locator[] orderFields = new RowType.Locator[column.OrderFields.Length];
                int[] way = new int[column.Direction.Length];
                for (int k = 0; k < orderFields.Length; k++)
                {
                    if (column.OrderFields[k] is String)
                        orderFields[k] = binder.GetLocator((String)column.OrderFields[k]);
                    else
                        orderFields[k] = binder.GetLocator((int)column.OrderFields[k]);
                }
                for (int k = 0; k < way.Length; k++)
                    way[k] = column.Direction[k] ==
                       SortDirection.Descending ? -1 : 1;
                comparer = new ComplexComparer(orderFields, way);
            }

            public override Aggregator CreateAgregator()
            {
                return new SortedXMLAgg(locator, column, comparer, queryContext);
            }
        }

        private sealed class SortedXMLAgg : Aggregator
        {

            private class Item
            {
                public readonly Row row;
                public readonly XmlNode node;

                public Item(Row row, XmlNode node)
                {
                    this.row = row;
                    this.node = node;
                }
            }

            private List<Item> nodes;

            private QueryContext queryContext;
            private ComplexComparer comparer;

            public SortedXMLAgg(RowType.Locator locator, Column column, ComplexComparer comparer, QueryContext queryContext)
                : base(locator, column)
            {
                nodes = new List<Item>();
                this.queryContext = queryContext;
                this.comparer = comparer;
            }

            public override void Aggregate(object value, Row row)
            {
                if (value is XmlNode)
                    nodes.Add(new Item(row, (XmlNode)value));                    
                else if (value is XmlNodeList)
                {
                    XmlNodeList l = (XmlNodeList)value;
                    foreach (XmlNode node in l)
                        nodes.Add(new Item(row, node));
                }
                else
                    nodes.Add(new Item(row, 
                        XmlDataAccessor.Serialize(queryContext.XmlResult, value)));
            }

            private int Comparison(Item x, Item y)
            {
                return comparer.Compare(x.row, y.row);
            }

            public override object Value
            {
                get
                {
                    XmlDataAccessor.NodeList res = new XmlDataAccessor.NodeList();
                    nodes.Sort(new Comparison<Item>(Comparison));
                    foreach (Item item in nodes)
                        res.Add(item.node);
                    return res;
                }
            }
        }

        #endregion
   
        private Column[] _columns;
        private String[] _groups;

        public bool StreamAggregate { get; set; }

        public Column[] Columns { get { return _columns; } }

        public DataAggregator(Column[] columns, String[] groups)
        {
            _columns = columns;
            _groups = groups;
            _childs = new QueryNodeCollection(this);
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            if (ChildNodes.Count != 1)
                throw new InvalidOperationException();
            
            Resultset rs = ChildNodes[0].Get(queryContext, parameters);
            Binder binder = new Binder(rs);
            DataTable dt = RowType.CreateSchemaTable();
            for (int k = 0; k < _columns.Length; k++)
            {
                DataRow r = dt.NewRow();
                r["ColumnOrdinal"] = k;
                r["ColumnName"] = _columns[k].Name;                
                r["IsExpression"] = true;
                if (_columns[k].ColumnName != null)
                {
                    RowType.Locator loc = binder.GetLocator(_columns[k].ColumnName);
                    RowType.TypeInfo t = rs.GetFieldType(loc);
                    if (_columns[k].Functor == AggregateFunctor.Min ||
                        _columns[k].Functor == AggregateFunctor.Max)
                    {
                        r["DataType"] = t.DataType;
                        r["ColumnSize"] = t.Size;
                    }
                    else if (_columns[k].Functor == AggregateFunctor.Count)
                        r["DataType"] = typeof(System.Int32);
                    else
                        if (_columns[k].Functor == AggregateFunctor.XMLAgg)
                            r["DataType"] = typeof(System.Object);
                        else
                            r["DataType"] = typeof(System.Double);
                    r["BaseServerName"] = t.BaseServerName;
                    r["BaseSchemaName"] = t.BaseSchemaName;
                    r["BaseCatalogName"] = t.BaseCatalogName;
                    r["BaseColumnName"] = t.BaseColumnName;
                    r["BaseTableName"] = t.BaseTableName;
                }
                else
                    r["DataType"] = typeof(System.Int32);
                dt.Rows.Add(r);    
            }
            
            RowType type = new RowType(dt);
            dt.Clear();
            
            DataRow r0 = dt.NewRow();
            r0["ColumnName"] = "$result";
            r0["ColumnOrdinal"] = 0;
            r0["DataType"] = typeof(Row);
            r0["NestedType"] = type;
            dt.Rows.Add(r0);

            foreach (DataRow r2 in rs.RowType.GetSchemaTable().Select())
            {
                DataRow r1 = dt.NewRow();
                r1.ItemArray = r2.ItemArray;
                dt.Rows.Add(r1);
            }

            AggregatorFactory[] aggregators = new AggregatorFactory[_columns.Length];
            for (int k = 0; k < _columns.Length; k++)
            {
                switch (_columns[k].Functor)
                {
                    case AggregateFunctor.RowCount:
                        aggregators[k] = new RowCountFactory(binder, _columns[k]);
                        break;

                    case AggregateFunctor.Count:
                        aggregators[k] = new CountFactory(binder, _columns[k]);
                        break;

                    case AggregateFunctor.Min:
                        aggregators[k] = new MinFactory(binder, _columns[k]);
                        break;

                    case AggregateFunctor.Max:
                        aggregators[k] = new MaxFactory(binder, _columns[k]);
                        break;

                    case AggregateFunctor.Sum:
                        aggregators[k] = new SumFactory(binder, _columns[k]);
                        break;

                    case AggregateFunctor.Avg:
                        aggregators[k] = new AvgFactory(binder, _columns[k]);
                        break;

                    case AggregateFunctor.XMLAgg:
                        if (_columns[k].OrderFields != null)
                            aggregators[k] = new SortedXMLAggFactory(binder, _columns[k], queryContext);
                        else
                            aggregators[k] = new XMLAggFactory(binder, _columns[k], queryContext);
                        break;
                }
            }

            RowType rs_type = new RowType(dt);                       
            if (_groups != null && _groups.Length > 0)
            {
                RowType.Locator[] groups = new RowType.Locator[_groups.Length];
                for (int k = 0; k < _groups.Length; k++)
                    groups[k] = binder.GetLocator(_groups[k]);
                
                EnumeratorProcessingContext enucontext = new EnumeratorProcessingContext(new Resultset[] { rs });
                if (StreamAggregate)
                    enucontext.Iterator = CreateStreamIterator(rs, rs_type, type, aggregators, groups);
                else
                    enucontext.Iterator = CreateHashIterator(rs, rs_type, type, aggregators, groups);
                
                return new Resultset(rs_type, enucontext);            
            }
            else
                return new Resultset(rs_type, new ScalarAggregator(rs, type, aggregators));            
        }

        private class ScalarAggregator : DemandProcessingContext
        {
            private Resultset _source;
            private Aggregator[] _aggregators;
            private RowType _type;

            public ScalarAggregator(Resultset source, RowType type, AggregatorFactory[] factory)
                : base(new Resultset[] { source })
            {
                _source = source;
                _type = type;
                _aggregators = new Aggregator[factory.Length];
                for (int k = 0; k < factory.Length; k++)
                    _aggregators[k] = factory[k].CreateAgregator();
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                for (int k = 0; k < _aggregators.Length; k++)
                    _aggregators[k].Reset();
                while (_source.Begin != null)
                {
                    Row curr = _source.Dequeue();
                    for (int k = 0; k < _aggregators.Length; k++)
                        _aggregators[k].Set(curr);
                }
                Row row = new Row(_type);
                for (int k = 0; k < _aggregators.Length; k++)
                    row.SetValue(k, _aggregators[k].Value);
                Row res = rs.NewRow();
                res.SetObject(0, row);
                rs.Enqueue(res);
                return false;
            }
        }

        private sealed class HashEntry
        {
            private Aggregator[] m_aggregators;

            public HashEntry(AggregatorFactory[] factory)
            {
                m_aggregators = new Aggregator[factory.Length];
                for (int k = 0; k < factory.Length; k++)
                {
                    m_aggregators[k] = factory[k].CreateAgregator();
                    m_aggregators[k].Reset();
                }
            }

            public void Put(Row row)
            {
                for (int k = 0; k < m_aggregators.Length; k++)
                    m_aggregators[k].Set(row);
            }

            public void Get(Row row)
            {
                for (int k = 0; k < m_aggregators.Length; k++)
                    row.SetValue(k, m_aggregators[k].Value);
            }
        }

        private IEnumerator<Row> CreateHashIterator(Resultset source, RowType rs_type, RowType type,
            AggregatorFactory[] aggregators, RowType.Locator[] groups)
        {
            Dictionary<Row, HashEntry> hs = new Dictionary<Row, HashEntry>(new RowComplexEqualityComparer(groups));            
            while (source.Begin != null)
            {
                HashEntry entry;
                Row row = source.Dequeue();
                if (!hs.TryGetValue(row, out entry))
                {
                    entry = new HashEntry(aggregators);
                    hs.Add(row, entry);
                }
                entry.Put(row);
            }
            foreach (KeyValuePair<Row, HashEntry> kvp in hs)
            {
                Row row = new Row(type);
                kvp.Value.Get(row);
                Row res = new Row(rs_type);
                res.SetObject(0, row);
                for (int k = 0; k < kvp.Key.Length; k++)
                    res.SetObject(k + 1, (Row)kvp.Key.GetObject(k));
                yield return res;
            }
        }

        private IEnumerator<Row> CreateStreamIterator(Resultset source, RowType rs_type, RowType type,
            AggregatorFactory[] aggregators, RowType.Locator[] groups)
        {
            //RowFactory factory = RowFactoryManager.GetFactory(type);
            //RowFactory rs_factory = RowFactoryManager.GetFactory(rs_type);
            //ComplexComparer comparer = new ComplexComparer(groups);
            //Row dest = source.Begin;
            //while (source.Begin != null)
            //{
            //    Row curr = source.Dequeue();                
            //    if (comparer.Compare(dest, curr) == 0)
            //        for (int k = 0; k < aggregators.Length; k++)
            //            aggregators[k].Set(curr);
            //    else
            //    {
            //        Row row = factory.CreateRow();
            //        for (int k = 0; k < aggregators.Length; k++)
            //        {
            //            row.SetValue(k, aggregators[k].Value);
            //            aggregators[k].Reset();
            //            aggregators[k].Set(curr);
            //        }
            //        Row res = rs_factory.CreateRow();
            //        res.SetObject(0, row);
            //        for (int k = 0; k < dest.Length; k++)
            //            res.SetObject(k + 1, (Row)dest.GetObject(k));
            //        yield return res;
            //        dest = curr;
            //    }
            //}
            throw new NotImplementedException();
        }

#if DEBUG
        protected override void Dump(System.IO.TextWriter w, int padding)
        {
            OutlineNode(w, padding);
            w.WriteLine("DataAggregator");
            foreach (Column column in Columns)
            {
                OutlineNode(w, padding);
                w.Write(" >");
                w.Write(column.ToString());
                w.WriteLine();
            }
        }
#endif
    }
}
