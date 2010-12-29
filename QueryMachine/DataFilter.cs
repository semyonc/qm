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
    public class DataFilter: QueryNode
    {
        protected class FilterContext : LispProcessingContext
        {
            private Resultset _source;
            private DataResolver _resolver;
            private Object _filterPredicate;
            private FunctionLink _compiledBody;

            public FilterContext(DataFilter owner, Resultset source, Object filterPredicate, QueryContext queryContext, Object[] parameters)
                : base(owner, source, queryContext, parameters)
            {
                _source = source;
                _compiledBody = new FunctionLink();
                List<ColumnBinding> bindings = new List<ColumnBinding>();
                ExpressionTransformer transformer = new ExpressionTransformer(bindings);
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
                            b.data = new SymbolLink(nested_ti.DataType);
                            bindings.Add(b);
                            if (nested_ti.IsContainer)
                                transformer.NeedTransform = true;
                        }
                LispExecutive.Enter(_resolver = new DataResolver(new Binder(bindings)));
                if (transformer.NeedTransform)
                    _filterPredicate = transformer.Process(filterPredicate);
                else
                    _filterPredicate = filterPredicate;
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                while (_source.Begin != null)
                {
                    Row row = _source.Dequeue();
                    foreach (ColumnBinding b in _resolver.Bindings)
                        b.Set(row, LispExecutive);
                    object res = LispExecutive.Apply(null, null, _filterPredicate, null, _compiledBody, LispExecutive.DefaultPool);
                    if (res != null && res != Undefined.Value)
                    {
                        rs.Enqueue(row);
                        return true;
                    }
                }
                return false;
            }
        }

        public DataFilter(object filterPredicate)
        {
            FilterPredicate = filterPredicate;
            _childs = new QueryNodeCollection(this);
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            if (ChildNodes.Count < 1)
                throw new InvalidOperationException();

            Resultset rs = ChildNodes[0].Get(queryContext, parameters);
            FilterContext context = new FilterContext(this, rs, FilterPredicate, queryContext, parameters);
            return new Resultset(rs.RowType, context);
        }

        public Object FilterPredicate { get; set; }

#if DEBUG
        protected override void Dump(System.IO.TextWriter w, int padding)
        {
            OutlineNode(w, padding);
            w.Write("DataFilter ");
            w.Write(Lisp.Format(FilterPredicate));
            w.WriteLine();
        }
#endif
    }
}
