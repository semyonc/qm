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
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class DataSorter: QueryNode
    {
        public class Column
        {
            public Object Name { get; set; }
            public SortDirection Direction { get; set; }

            public Column()
            {

            }

            public Column(Object name, SortDirection direction)
            {
                Name = name;
                Direction = direction;
            }
        }

        protected class SorterContext : DemandProcessingContext
        {
            private Resultset _source;
            RowComparer _comparer;

            public SorterContext(Resultset source, RowComparer comparer)
                : base(new Resultset[] { source })
            {
                _source = source;
                _comparer = comparer;
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                _source.Fill();
                _source.Sort(_comparer);
                while (_source.Begin != null)
                    rs.Enqueue(_source.Dequeue());                    
                return false;
            }
        }

        private Column[] _columns;

        public DataSorter(Column[] columns)
        {
            _columns = columns;
            _childs = new QueryNodeCollection(this);           
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            Resultset source = ChildNodes[0].Get(queryContext, parameters);
            RowType.Locator[] locator = new RowType.Locator[_columns.Length];
            int[] way = new int[_columns.Length];
            Binder binder = new Binder(source);
            for (int k = 0; k < _columns.Length; k++)
            {
                Column col = _columns[k];
                locator[k] = col.Name is String ? binder.GetLocator((string)col.Name) :
                    binder.GetLocator(((int)col.Name) - 1);
                way[k] = col.Direction ==
                   SortDirection.Descending ? -1 : 1;
            }
            return new Resultset(source.RowType, new SorterContext(source, 
                new ComplexComparer(locator, way)));
        }

#if DEBUG
        protected override void Dump(System.IO.TextWriter w, int padding)
        {
            OutlineNode(w, padding);
            w.Write("DataSorter ");
            for (int k = 0; k < _columns.Length; k++)
            {
                if (k > 0)
                    w.Write(", ");
                w.Write(Lisp.Format(_columns[k].Name));
                if (_columns[k].Direction == SortDirection.Descending)
                    w.Write(" DESC");
            }
            w.WriteLine();
        }
#endif
    }
}
