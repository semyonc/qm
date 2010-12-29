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
using System.Data.Common;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    public class AdoTableAccessor: QueryNode
    {
        private DataTable _dataTable;

        protected class ProcessingContext : DemandProcessingContext
        {
            private DbDataReader _dataReader;

            public ProcessingContext(DbDataReader dataReader)
                : base(null)
            {
                _dataReader = dataReader;
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                if (!_dataReader.Read())
                {
                    _dataReader.Close();
                    return false;
                }
                Row row = rs.NewRow();
                object[] values = new object[_dataReader.FieldCount];
                _dataReader.GetValues(values);
                for (int i = 0; i < _dataReader.FieldCount; i++)
                    row.SetValue(i, values[i]);
                rs.Enqueue(row);
                return true;
            }
        }

        public AdoTableAccessor(DataTable dataTable)
        {
            _dataTable = dataTable;
        }

        public AdoTableAccessor(string fileName)
        {
            _dataTable = new DataTable();
            _dataTable.ReadXml(fileName);
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            DbDataReader reader = _dataTable.CreateDataReader();
            ProcessingContext context = new ProcessingContext(reader);
            return new Resultset(new RowType(reader.GetSchemaTable()), context);
        }
    }
}
