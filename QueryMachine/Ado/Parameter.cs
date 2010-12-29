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
using System.Text;
using System.Data;
using System.Data.Common;

namespace DataEngine.ADO
{
    public class Parameter: DbParameter
    {
        private String _parameterName;
        private DbType _dbType;
        private int _size;
        private String _sourceColumn;
        private bool _sourceColumnMapping;
        private DataRowVersion _sourceVersion;
        private Object _value;
        private bool _init;

        public Parameter()
        {
            _value = DBNull.Value;
            _dbType = DbType.String;
            _sourceVersion = DataRowVersion.Default;
        }

        public Parameter(String parameterName, Object value)
        {
            _parameterName = parameterName;
            _value = value;
            InferDbType(value);
            _sourceVersion = DataRowVersion.Default;
        }

        public Parameter(String parameterName, DbType dbtype)
        {
            _parameterName = parameterName;
            _value = DBNull.Value;
            _dbType = dbtype;
            _sourceVersion = DataRowVersion.Default;
        }

        public Parameter(String parameterName, DbType dbtype, int size)
        {
            _parameterName = parameterName;
            _value = DBNull.Value;
            _dbType = dbtype;
            _size = size;
            _sourceVersion = DataRowVersion.Default;
        }

        public override DbType DbType
        {
            get
            {
                return _dbType;
            }
            set
            {
                _dbType = value;
                _init = true;
            }
        }

        public override ParameterDirection Direction
        {
            get
            {
                return ParameterDirection.Input;
            }
            set
            {
                if (value != ParameterDirection.Input)
                    throw new NotImplementedException();
            }
        }

        public override bool IsNullable
        {
            get
            {
                return true;
            }
            set
            {
                if (!value)
                    throw new NotImplementedException();
            }
        }

        public override string ParameterName
        {
            get
            {
                return _parameterName;
            }
            set
            {
                _parameterName = value;
            }
        }

        public override void ResetDbType()
        {
            _init = false;
        }

        public override int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        public override string SourceColumn
        {
            get
            {
                return _sourceColumn;
            }
            set
            {
                _sourceColumn = value;
            }
        }

        public override bool SourceColumnNullMapping
        {
            get
            {
                return _sourceColumnMapping;
            }
            set
            {
                _sourceColumnMapping = value;
            }
        }

        public override DataRowVersion SourceVersion
        {
            get
            {
                return _sourceVersion;
            }
            set
            {
                _sourceVersion = value;
            }
        }

        public override object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!_init && value != null && value != DBNull.Value)
                {
                    InferDbType(value);
                    _init = true;
                }
                _value = value;
            }
        }

        private void InferDbType(object value)
        {
            TypeCode tc = Type.GetTypeCode(value.GetType());
            switch (tc)
            {
                case TypeCode.SByte:
                    _dbType = DbType.SByte;
                    break;
                case TypeCode.Byte:
                    _dbType = DbType.Byte;
                    break;
                case TypeCode.Int16:
                    _dbType = DbType.Int16;
                    break;
                case TypeCode.UInt16:
                    _dbType = DbType.UInt16;
                    break;
                case TypeCode.Int32:
                    _dbType = DbType.Int32;
                    break;
                case TypeCode.UInt32:
                    _dbType = DbType.UInt32;
                    break;
                case TypeCode.Int64:
                    _dbType = DbType.Int64;
                    break;
                case TypeCode.UInt64:
                    _dbType = DbType.UInt64;
                    break;
                case TypeCode.DateTime:
                    _dbType = DbType.DateTime;
                    break;
                case TypeCode.String:
                    _dbType = DbType.String;
                    break;
                case TypeCode.Single:
                    _dbType = DbType.Single;
                    break;
                case TypeCode.Double:
                    _dbType = DbType.Double;
                    break;
                case TypeCode.Decimal:
                    _dbType = DbType.Decimal;
                    break;
                case TypeCode.Object:
                default:
                    _dbType = DbType.Object;
                    break;
            }
        }
    }
}
