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
    public class DataProviderQueryAccessor: DataProviderAccessor
    {
        protected class SqlQueryWriter : SqlWriter
        {
            public SqlQueryWriter(Notation notation)
                : base(notation)
            {
                Bindings = new List<int>();
            }

            protected override void WriteQualifiedNamePrefix(Qname qname)
            {
                return; // Ignore prefix
            }

            protected override void WritePlaceholder(Placeholder placeholder)
            {
                base.WriteText(ProviderHelper.FormatParameter(
                    String.Format("p{0}", Bindings.Count)));
                Bindings.Add(placeholder.Index -1);
            }

            public List<int> Bindings { get; private set; }
        }

        protected override void PrepareCommand(RowType.TypeInfo[] fields, DbCommand command, Object[] parameters)
        {
            command.CommandText = _commandText;
            for (int k = 0; k < _parameterBindings.Length; k++)
            {
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = _helper.FormatParameter(
                    String.Format("p{0}", command.Parameters.Count));
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = parameters[_parameterBindings[k]];
                command.Parameters.Add(parameter);
            }
        }

        protected override RowType CreateRowType()
        {
            if (_rowType == null)
            {
                DataProviderHelper helper = new DataProviderHelper(_providerInvariantName, _connectionString, _x86Connection);
                DbConnection connection = DataProviderHelper.CreateDbConnection(_providerInvariantName, _x86Connection);
                connection.ConnectionString = _connectionString;
                connection.Open();
                DbCommand command = connection.CreateCommand();
                command.CommandText = _commandText;
                for (int k = 0; k < _parameterBindings.Length; k++)
                {
                    DbParameter parameter = command.CreateParameter();
                    parameter.ParameterName = _helper.FormatParameter(
                        String.Format("p{0}", command.Parameters.Count));
                    parameter.Direction = ParameterDirection.Input;
                    command.Parameters.Add(parameter);
                }
                DbDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.CloseConnection);
                try
                {
                    DataTable dt_src = reader.GetSchemaTable();
                    DataTable dt = RowType.CreateSchemaTable();
                    int n = 0;
                    foreach (DataRow r in dt_src.Rows)
                    {
                        DataRow r1 = dt.NewRow();
                        foreach (DataColumn col in dt_src.Columns)
                        {
                            DataColumn dest = dt.Columns[col.ColumnName];
                            if (dt.Columns.IndexOf(col.ColumnName) != -1 && col.ColumnName != "ColumnName")
                                r1[dest] = r[col];
                        }
                        r1["ColumnOrdinal"] = n++;
                        string columnName = (string)r["ColumnName"];
                        r1["ColumnName"] = helper.NativeFormatIdentifier(columnName);
                        r1["ProviderColumnName"] = columnName;
                        r1["IsCaseSensitive"] =
                            helper.IdentifierCase == IdentifierCase.Sensitive;
                        dt.Rows.Add(r1);
                    }
                    _rowType = new RowType(dt);
                }
                finally
                {
                    reader.Close();
                }
            }
            return _rowType;
        }

        private DataProviderHelper _helper;
        private RowType _rowType;
        private string _commandText;
        private int[] _parameterBindings;

        public DataProviderQueryAccessor(DataSourceInfo dataSourceInfo, Notation notation, Symbol squery)
            : base()
        {
            _rowType = null;
            _connectionString = dataSourceInfo.ConnectionString;
            _providerInvariantName = dataSourceInfo.ProviderInvariantName;
            _x86Connection = dataSourceInfo.X86Connection;
            _helper = new DataProviderHelper(_providerInvariantName, _connectionString, _x86Connection);
            SqlQueryWriter writer = new SqlQueryWriter(notation);
            writer.ProviderHelper = _helper;
            if (squery.Tag == Tag.Stmt)
                writer.WriteStmt(squery);
            else
                writer.WriteQueryExp(squery);
            _commandText = writer.ToString();
            _parameterBindings = writer.Bindings.ToArray();
        }

#if DEBUG
        protected override void Dump(System.IO.TextWriter w, int padding)
        {
            OutlineNode(w, padding);
            w.WriteLine("DataProviderQueryAccessor {0}", _providerInvariantName);
            w.WriteLine(_commandText);
        }
#endif
    }
}
