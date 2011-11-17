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

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    public class DataSelector: QueryNode
    {        
        public class Column
        {
            public object Expr { get; set; }
            public String Alias { get; set; }

            public Column()
            {
            }

            public Column(object expr, String alias)
            {
                Expr = expr;
                Alias = alias;
            }

            public Column(object expr)
                : this(expr, null)
            { }
        }

        public static object Table = ATOM.Create("$table");

        private class SelectorResolver : DataResolver
        {
            public SelectorResolver(Binder binder)
                : base(binder)
            {
                Resultsets = new List<object>();
            }

            public List<object> Resultsets { get; private set; }

            public override bool Get(object atom, out SymbolLink result)
            {
                bool res = base.Get(atom, out result);
                if (res && result.Type == typeof(Resultset))
                    Resultsets.Add(atom);
                return res;
            }
        }
        
        public class SelectorContext : LispProcessingContext
        {
            private DataResolver _resolver;
            private Column[] _columns;
            private Resultset _source;
            private HashSet<Row> _distinctSet;
            FunctionLink[] _compiledBody;
            private int _topRows;

            public SelectorContext(DataSelector owner, bool distinct, Column[] columns, DataResolver resolver,
                Resultset source, QueryContext queryContext, object[] parameters, FunctionLink[] compiledBody)
                : base(owner, source, queryContext, parameters)
            {
                _columns = columns;
                _source = source;
                _resolver = resolver;
                _topRows = owner.TopRows;
                _compiledBody = compiledBody;
                RowNum = 1;
                if (distinct)
                    _distinctSet = new HashSet<Row>(new RowEqualityComparer());
                LispExecutive.Enter(resolver);
            }

            public int RowNum { get; private set; }

            public override bool ProcessNextPiece(Resultset rs)            
            {
                if (_topRows > 0 && RowNum > _topRows)
                {
                    _source.Cancel();
                    return false;
                }
                while (_source.Begin != null)
                {
                    Row src = _source.Dequeue();
                    foreach (ColumnBinding b in _resolver.Bindings)
                        b.Set(src, LispExecutive);
                    Row row = rs.NewRow();
                    for (int k = 0; k < _columns.Length; k++)
                    {
                        object res = LispExecutive.Apply(null, null, _columns[k].Expr,
                            null, _compiledBody[k], LispExecutive.DefaultPool);
                        if (res != null && res != Undefined.Value)
                            row.SetValue(k, res);
                    }
                    if (_distinctSet != null)
                    {
                        if (!_distinctSet.Contains(row))
                        {
                            _distinctSet.Add(row);
                            rs.Enqueue(row);
                        }
                        else
                            continue;
                    }
                    else
                        rs.Enqueue(row);
                    RowNum++;
                    return true;                    
                }
                return false;
            }
        }

        protected Column[] _targets;
        protected bool _distinct;

        public int TopRows { get; set; }

        public DataSelector(bool distinct, Column[] columns)
        {
            _distinct = distinct;
            _targets = columns;
            _childs = new QueryNodeCollection(this);
            TopRows = -1;
        }
     
        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            // Prepare bindings
            Resultset source = ChildNodes[0].Get(queryContext, parameters);
            List<ColumnBinding> bindings = new List<ColumnBinding>();            
            ExpressionTransformer transformer = new ExpressionTransformer(bindings);
            foreach (RowType.TypeInfo ti in source.RowType.Fields)
                foreach (RowType.TypeInfo nested_ti in ti.NestedType.Fields)
                    if (!nested_ti.IsHidden && nested_ti.IsNatural)
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
            foreach (RowType.TypeInfo ti in source.RowType.Fields)
                foreach (RowType.TypeInfo nested_ti in ti.NestedType.Fields)
                    if (!nested_ti.IsHidden && !nested_ti.IsNatural)
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
            // Expand columns.
            // On this step we transform select table.* and select * to select field1,...
            List<Column> columns = new List<Column>();
            if (_targets == null) // Select fields from all selected tables
            {
                foreach (ColumnBinding b in bindings)
                    columns.Add(new Column(ATOM.Create(null, new string[] { b.TableName, b.Name }, false)));
            }
            else
                foreach (Column col in _targets)
                {
                    if (Lisp.IsFunctor(col.Expr, Table)) // Select fields from specified table
                    {
                        bool found = false;
                        String name = (String)Lisp.Second(col.Expr);
                        var tableBindings =
                                from b in bindings
                                where b.TableName == name
                                select b;
                        foreach (ColumnBinding b in tableBindings)
                        {
                            columns.Add(new Column(ATOM.Create(null, new string[] { b.TableName, b.Name }, false)));
                            found = true;
                        }
                        if (!found)
                            throw new ESQLException(Properties.Resources.InvalidIdentifier, name);
                    }
                    else // Select expression specified
                        if (transformer.NeedTransform)
                        {
                            Column c = new Column();
                            c.Alias = col.Alias;
                            c.Expr = transformer.Process(col.Expr);
                            columns.Add(c);
                        }
                        else
                            columns.Add(col);
                }
            // Create demand context   
            FunctionLink[] compiledBody = new FunctionLink[columns.Count];
            SelectorResolver resolver = new SelectorResolver(new Binder(bindings));
            SelectorContext context = new SelectorContext(this, _distinct, columns.ToArray(),
                resolver, source, queryContext, parameters, compiledBody);
            // Create columns type info and generate unique column names by field name and alias
            RowType.TypeInfo[] fields = new RowType.TypeInfo[columns.Count];
            List<String> fieldNames = new List<string>();
            int p = 1;
            for (int k = 0; k < fields.Length; k++)
            {
                String name;
                RowType.TypeInfo fieldType = null;                
                object expr = columns[k].Expr;
                compiledBody[k] = new FunctionLink();
                Type resType = context.LispExecutive.Compile(null, expr, compiledBody[k]);                
                if (Lisp.IsAtom(expr))
                {
                    ColumnBinding b = resolver.GetBinding(expr);
                    if (b != null)
                        fieldType = b.fieldType;
                }
                else if (Lisp.IsNode(expr) && expr is String)
                {
                    fieldType = new RowType.TypeInfo(null, typeof(String),
                        TypeConverter.GetValueSize(expr));
                }
                else if (Lisp.IsFunctor(expr, ID.ParamRef))
                {
                    object value = parameters[(int)Lisp.Arg1(expr)];
                    fieldType = new RowType.TypeInfo(null, value.GetType(),
                        TypeConverter.GetValueSize(value));
                }
                else if (Lisp.IsFunctor(expr, ID.ToString))
                {
                    if (Lisp.Length(expr) == 3)
                        fieldType = new RowType.TypeInfo(null, typeof(String), (int)Lisp.Arg2(expr));
                    else
                        fieldType = new RowType.TypeInfo(null, typeof(String), 0);
                }
                else if (Lisp.IsFunctor(expr, ID.ToInt16))
                    fieldType = new RowType.TypeInfo(null, typeof(Int16), 0);
                else if (Lisp.IsFunctor(expr, ID.ToInt32))
                    fieldType = new RowType.TypeInfo(null, typeof(Int32), 0);
                else if (Lisp.IsFunctor(expr, ID.ToInt64))
                    fieldType = new RowType.TypeInfo(null, typeof(Int64), 0);
                else if (Lisp.IsFunctor(expr, ID.ToSingle))
                    fieldType = new RowType.TypeInfo(null, typeof(Single), 0);
                else if (Lisp.IsFunctor(expr, ID.ToDouble))
                    fieldType = new RowType.TypeInfo(null, typeof(Double), 0);
                else if (Lisp.IsFunctor(expr, ID.ToDecimal))
                    fieldType = new RowType.TypeInfo(null, typeof(Decimal), 0);
                else if (Lisp.IsFunctor(expr, ID.ToDateTime))
                    fieldType = new RowType.TypeInfo(null, typeof(DateTime), 0);
                else if (Lisp.IsFunctor(expr, ID.ConvertTimestamp))
                    fieldType = new RowType.TypeInfo(null, typeof(DateTime), 0);
                else
                    fieldType = new RowType.TypeInfo(null, resType, 0);
                if (!String.IsNullOrEmpty(columns[k].Alias))
                    name = columns[k].Alias;
                else
                    if (fieldType.Name == null)
                        name = String.Format("Expr{0}", p++);
                    else
                        name = fieldType.Name;                
                fields[k] = new RowType.TypeInfo(k, 
                    Util.CreateUniqueName(fieldNames, name), fieldType);                
            }
            ScanDynatableAccessors(ChildNodes[0], resolver);
            return new Resultset(new RowType(fields), context);            
        }        

        private void ScanDynatableAccessors(QueryNode queryNode, SelectorResolver resolver)
        {
            if (queryNode is DataFilter || queryNode is DataAggregator || queryNode is DataSorter)
                ScanDynatableAccessors(queryNode.ChildNodes[0], resolver);
            else if (queryNode is DataJoin || queryNode is DetailJoin)
            {
                ScanDynatableAccessors(queryNode.ChildNodes[0], resolver);
                ScanDynatableAccessors(queryNode.ChildNodes[1], resolver);
            }
            else if (queryNode is DynatableAccessor)
            {
                DynatableAccessor accessor = (DynatableAccessor)queryNode;
                object[] terms = Lisp.GetTerms(accessor.AccessPredicate);
                foreach (object o1 in resolver.Resultsets)
                    foreach(object o2 in terms)
                        if (o1 == o2)
                        {
                            accessor.CopyContext = true;
                            break;
                        }
            }
        }

#if DEBUG
        protected override void Dump(System.IO.TextWriter w, int deep)
        {
            OutlineNode(w, deep);
            w.Write("DataSelector");
            if (_distinct)
                w.Write(":DISTINCT");
            w.WriteLine();
            if (_targets == null)
            {
                OutlineNode(w, deep);
                w.WriteLine(" *");
            }
            else
                foreach (Column col in _targets)
                {
                    OutlineNode(w, deep);
                    w.Write(" >");
                    if (col.Alias != null)
                    {
                        w.Write("[");
                        w.Write(col.Alias);
                        w.Write("]");
                        w.Write(":");
                    }
                    w.WriteLine(Lisp.Format(col.Expr));
                }
        }
#endif
    }
}
