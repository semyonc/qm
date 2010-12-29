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
    public class DataCollector: QueryNode
    {

        protected class CollectorContext : DemandProcessingContext
        {
            private DataCollector _owner;
            private Resultset _source;

            public CollectorContext(DataCollector owner, Resultset source, object[] parameters)
                : base(new Resultset[] { source })
            {
                _owner = owner;
                _source = source;
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                if (_source.Begin == null)
                    return false;
                else
                {
                    Row row = rs.NewRow();
                    row.SetObject(0, _source.Dequeue());
                    rs.Enqueue(row);
                    return true;
                }
            }
        }

        private string _tableName;

        public DataCollector(String tableName)
        {
            _tableName = tableName;
            _childs = new QueryNodeCollection(this);
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            if (ChildNodes.Count != 1)
                throw new InvalidOperationException();

            Resultset source = ChildNodes[0].Get(queryContext, parameters);

            DataTable dt = RowType.CreateSchemaTable();
            DataRow r = dt.NewRow();
            r["ColumnName"] = _tableName;
            r["ColumnOrdinal"] = 0; 
            r["DataType"] = typeof(Row);
            r["NestedType"] = source.RowType;
            dt.Rows.Add(r);

            CollectorContext context = new CollectorContext(this, source, parameters);
            return new Resultset(new RowType(dt), context);
        }
    }
}
