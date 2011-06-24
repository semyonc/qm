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
using System.IO;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    public class DynatableAccessor: QueryNode
    {
        DatabaseDictionary _dictionary;
        private object _accessPredicate;
        private string _name;

        protected class DynatableResolver : DataResolver
        {
            private DynatableAccessor _owner;
            private QueryContext _queryContext;
            private MemoryPool _pool;

            public DynatableResolver(DynatableAccessor owner, QueryContext context, Binder binder, MemoryPool pool)
                : base(binder)
            {
                _owner = owner;
                _queryContext = context;
                _pool = pool;
            }

            public override bool Get(object atom, out SymbolLink result)
            {
                ColumnBinding b;
                if (!_bindings_t.TryGetValue(atom, out b))                    
                {
                    ATOM a = (ATOM)atom;
                    if (a.prefix != null)
                    {
                        TableType tableType = _owner._dictionary.GetTableType(a.prefix, Util.UnquoteName(a.parts));
                        switch (tableType.DataSource.TableAccessor)
                        {
                            case AcessorType.DataProvider:
                                result = new SymbolLink(new DataProviderTableAccessor(tableType), _pool);
                                break;

                            case AcessorType.XMLFile:
                                result = new SymbolLink(XmlDataAccessor.OpenFile(_owner, _queryContext, tableType.TableName), _pool);
                                break;

                            case AcessorType.DataSet:
                                {
                                    DataSet ds = (DataSet)tableType.DataSource.DataContext;
                                    result = new SymbolLink(new AdoTableAccessor(ds.Tables[tableType.TableName]), _pool);
                                }
                                break;

                            case AcessorType.DataTable:
                                if (tableType.DataSource.DataContext != null)
                                    result = new SymbolLink(new AdoTableAccessor((DataTable)tableType.DataSource.DataContext), _pool);
                                else
                                    result = new SymbolLink(new AdoTableAccessor(tableType.TableName), _pool);
                                break;

                            default:
                                throw new ESQLException(Properties.Resources.PrefixNotAvailableForDynExpr, a.prefix);
                        }
                        return true;
                    }
                    else
                    {
                        if (a.parts.Length == 1)
                            b = _binder.Get(null, a.parts[0]);
                        else
                            b = _binder.Get(a.parts[0], a.parts[1]);
                        if (b == null)
                            throw new ESQLException(Properties.Resources.InvalidIdentifier, atom);
                        _bindings_t.Add(atom, b);
                    }
                }
                result = b.data;
                return true;
            }
        }

        protected class DynatableContext : LispProcessingContext
        {
            private Resultset _rs = null;
            private Row _curr = null;
            private bool _init = false;

            public DynatableContext(DynatableAccessor owner, Resultset source, Row current,
                Object accessPredicate, QueryContext queryContext, Object[] parameters) 
                : base(owner, source, queryContext, parameters)
            {
                List<ColumnBinding> bindings = new List<ColumnBinding>();
                List<object> names = new List<object>();
                ExpressionTransformer transformer = new ExpressionTransformer(bindings);
                MemoryPool pool = LispExecutive.DefaultPool;
                if (source != null)
                {
                    foreach (RowType.TypeInfo ti in source.RowType.Fields)
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
                                bindings.Add(b);
                                if (nested_ti.IsContainer)
                                    transformer.NeedTransform = true;
                            }
                    if (current != null)
                    {                        
                        foreach (ColumnBinding b in bindings)
                        {
                            Row r = (Row)current.GetObject(b.rnum);
                            if (r != null)
                                b.data = new SymbolLink(r[b.fieldType.Ordinal], pool);
                        }
                    }                    
                }
                LispExecutive.Enter(new DynatableResolver((DynatableAccessor)Node, queryContext, new Binder(bindings), pool));
                if (transformer.NeedTransform)
                    _rs = (Resultset)LispExecutive.Apply(null, null, transformer.Process(accessPredicate), null, null, pool);
                else
                    _rs = (Resultset)LispExecutive.Apply(null, null, accessPredicate, null, null, pool);
            }

            public Resultset Result { get { return _rs; } }

            public new DynatableAccessor Node { get { return (DynatableAccessor)base.Node; } }

            public override bool ProcessNextPiece(Resultset rs)
            {
                if (_rs == null)
                    return false;

                if (Node.CopyContext)
                {
                    if (!_init)
                    {
                        _curr = _rs.Begin;
                        _init = true;
                    }
                    if (_curr != null)
                    {
                        Row row = rs.NewRow();
                        row.SetObject(0, _curr.Clone());
                        rs.Enqueue(row);
                        _curr = _rs.NextRow(_curr);
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    if (_rs.Begin != null)
                    {
                        Row row = rs.NewRow();
                        row.SetObject(0, _rs.Dequeue());
                        rs.Enqueue(row);
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        public String Name { get { return _name; } }
        public bool CopyContext { get; set; }

        public DynatableAccessor(String name, object accessPredicate, DatabaseDictionary dictionary)
        {
            _accessPredicate = accessPredicate;
            _name = name;
            _dictionary = dictionary;
            CopyContext = false;
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            DynatableContext context = 
                new DynatableContext(this, null, null, _accessPredicate, queryContext, parameters);
            if (context.Result == null)
                return null;
            else
            {
                DataTable dt = RowType.CreateSchemaTable();
                DataRow r = dt.NewRow();
                r["ColumnName"] = _name;
                r["ColumnOrdinal"] = 0;
                r["DataType"] = typeof(Row);
                r["NestedType"] = context.Result.RowType;
                dt.Rows.Add(r);
                return new Resultset(new RowType(dt), context);
            }
        }

        public Resultset Get(Resultset source, Row current, QueryContext queryContext, object[] parameters)
        {
            DynatableContext context =
                new DynatableContext(this, source, current, _accessPredicate, queryContext, parameters);
            if (context.Result == null)
                return null;
            else
            {
                DataTable dt = RowType.CreateSchemaTable();
                DataRow r = dt.NewRow();
                r["ColumnName"] = _name;
                r["ColumnOrdinal"] = 0;
                r["DataType"] = typeof(Row);
                r["NestedType"] = context.Result.RowType;
                dt.Rows.Add(r);
                return new Resultset(new RowType(dt), context);
            }
        }

        public object AccessPredicate
        {
            get
            {
                return _accessPredicate;
            }
        }

#if DEBUG
        protected override void Dump(TextWriter w, int padding)
        {
            OutlineNode(w, padding);
            w.WriteLine("DynatableAccessor {0},{1}", _name, _accessPredicate);
        }
#endif

    }
}
