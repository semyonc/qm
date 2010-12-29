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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine.ADO
{
    public class DataReader: DbDataReader
    {
        private Resultset _rs;
        private QueryContext _context;
        private RowType _rct;        
        private Row _current;
        private bool _result_f;

        private Binder _binder;

        public DataReader(Resultset rs, QueryContext context)
        {
            _rs = rs;
            _context = context;
            _binder = new Binder(rs);
            _rct = rs.RowType;
            _result_f = true;
        }

        private void CheckRecord()
        {
            if (_rs == null || _current == null)
                throw new Exception(Properties.Resources.NoDataFound);
        }

        public bool PreserveResults { get; set; }

        public Resultset Source
        {
            get
            {
                return _rs;
            }
        }

        public override void Close()
        {
            _rs = null;
            _current = null;
            if (!PreserveResults && _context != null)
                _context.Close();
            _context = null;
        }

        public override int Depth
        {
            get { return 1; }
        }

        public override int FieldCount
        {
            get { return _rct.Fields.Length; }
        }

        public override bool GetBoolean(int ordinal)
        {
            CheckRecord();
            return _current.GetBoolen(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            CheckRecord();
            return _current.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            CheckRecord();
            return _current.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            CheckRecord();
            return _current.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            CheckRecord();
            return _current.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _rct.Fields[ordinal].DataType.Name;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            CheckRecord();
            return _current.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            CheckRecord();
            return _current.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            CheckRecord();
            return _current.GetDouble(ordinal);
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this);
        }

        public override Type GetFieldType(int ordinal)
        {
            return _rct.Fields[ordinal].DataType;
        }

        public override float GetFloat(int ordinal)
        {
            CheckRecord();
            return _current.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            CheckRecord();
            return _current.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            CheckRecord();
            return _current.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            CheckRecord();
            return _current.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return Util.UnquoteName(_rct.Fields[ordinal].Name);
        }

        public override int GetOrdinal(string name)
        {
            CheckRecord();
            ColumnBinding b = _binder.Get(null, name);
            if (b == null)
                throw new ArgumentException(name);
            return b.rnum;
        }

        public override DataTable GetSchemaTable()
        {
            return _rct.GetSchemaTable();
        }

        public override string GetString(int ordinal)
        {
            CheckRecord();
            return _current.GetString(ordinal);
        }

        protected override DbDataReader GetDbDataReader(int ordinal)
        {
            if (IsDBNull(ordinal))
                return null;
            else
            {
                DataReader res = new DataReader((Resultset)GetValue(ordinal), _context);
                res.PreserveResults = true;
                return res;
            }
        }

        public override object GetValue(int ordinal)
        {
            CheckRecord();
            return _current.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            CheckRecord();
            int count = (FieldCount < values.Length ? FieldCount : values.Length);
            for (int i = 0; i < count; i++)
                values[i] = _current.GetValue(i);
            return count;
        }

        public override bool HasRows
        {
            get { return true; }
        }

        public override bool IsClosed
        {
            get { return _rs == null; }
        }

        public override bool IsDBNull(int ordinal)
        {
            CheckRecord();
            return _current.IsDbNull(ordinal);
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            if (IsClosed)
                throw new InvalidOperationException(Properties.Resources.DataReaderNotOpen);
            else
                if (_result_f)
                {
                    if (_current == null)
                        _current = _rs.Begin;
                    else
                    {
                        _current = _rs.NextRow(_current);
                        if (!PreserveResults)
                            _rs.Truncate();
                    }
                    if (_current == null)
                        _result_f = false;
                }
            return _result_f;
        }

        public override int RecordsAffected
        {
            get { return 0; }
        }

        public override object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        public override object this[int ordinal]
        {
            get { return GetValue(ordinal); }
        }
    }
}