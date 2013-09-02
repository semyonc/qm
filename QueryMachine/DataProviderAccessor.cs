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
    public abstract class DataProviderAccessor: QueryNode
    {
        protected class ProcessingContext : DemandProcessingContext
        {
            private DataProviderAccessor _owner;
            private DbDataReader _reader;
            private Object[] _parameters;
            private int _rownum;

            public ProcessingContext(DataProviderAccessor owner, Object[] parameters)
                : base(null)
            {
                _owner = owner;
                _parameters = parameters;
                _rownum = 0;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    if (_reader != null)
                    {
                        _reader.Close();
                        _reader = null;
                    }
                }
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                CheckDisposed();
                if (_reader == null)
                {
                    DbConnection connection = DataProviderHelper.CreateDbConnection(_owner._providerInvariantName, _owner._x86Connection);
                    connection.ConnectionString = _owner._connectionString;
                    connection.Open();
                    DbCommand command = connection.CreateCommand();
                    _owner.PrepareCommand(rs.RowType.Fields, command, _parameters);                
                    _reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                if (!_reader.Read() || 
                        (_owner.TopRows != -1 && _rownum > _owner.TopRows))
                {                   
                    _reader.Close();
                    _reader = null;
                    return false;
                }
                Row row = rs.NewRow();
                object[] values = new object[_reader.FieldCount];
                _reader.GetValues(values);
                for (int i = 0; i < _reader.FieldCount; i++)
                        row.SetValue(i, values[i]);
                rs.Enqueue(row);
                _rownum++;
                return true;
            }
        }

        protected String _providerInvariantName;
        protected bool _x86Connection;
        protected String _connectionString;

        public int TopRows { get; set; }

        public DataProviderAccessor() :
            base()
        {
            TopRows = -1;
        }

        protected abstract void PrepareCommand(RowType.TypeInfo[] fields, 
            DbCommand command, Object[] parameters);

        protected abstract RowType CreateRowType();

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            RowType rt = CreateRowType();
            ProcessingContext context = new ProcessingContext(this, parameters);
            context.RecordLimit = queryContext.LimitInputQuery;
            return new Resultset(rt, context);            
        }        
    }
}
