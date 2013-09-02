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

namespace DataEngine.CoreServices.Data
{
    public class Row : IComparable, ICloneable
    {
        internal Row _next;
        internal object[] _itemArray;
        internal RowType _type;

        internal Row(RowType type)
        {
            _type = type;
            _itemArray = new object[type.Fields.Length];
        }


        public int Length
        {
            get
            {
                return _itemArray.Length;
            }
        }

        public RowType Type
        {
            get
            {
                return _type;
            }
        }

        public void Clear()
        {
            for (int k = 0; k < Length; k++)
                SetDbNull(k);
        }

        public object Clone()
        {
            Row row = new Row(_type);
            for (int k = 0; k < Length; k++)
                row.SetValue(k, GetValue(k));
            return row;
        }

        public bool HasNullValues
        {
            get
            {
                foreach (object o in _itemArray)
                    if (o == null)
                        return true;
                return false;
            }
        }

        public bool IsDbNull(int ordinal)
        {            
            return _itemArray[ordinal] == null;
        }

        public void SetDbNull(int ordinal)
        {
            _itemArray[ordinal] = null;
        }

        public bool GetBoolen(int ordinal)
        {
            return (bool)_itemArray[ordinal];
        }

        public void SetBoolean(int ordinal, bool value)
        {
            SetValue(ordinal, value);
        }

        public byte GetByte(int ordinal)
        {
            return (byte)_itemArray[ordinal];
        }

        public void SetByte(int ordinal, byte value)
        {
            SetValue(ordinal, value);
        }

        public sbyte GetSByte(int ordinal)
        {
            return (sbyte)_itemArray[ordinal];
        }

        public void SetSByte(int ordinal, sbyte value)
        {
            SetValue(ordinal, value);
        }

        public long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            byte[] value = (byte[])_itemArray[ordinal];
            if (buffer == null)
                return value.Length;
            int copylen = length > value.Length ? value.Length : length;
            Array.Copy(value, dataOffset, buffer, bufferOffset, copylen);
            return copylen;
        }

        public void SetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int ordinal)
        {
            return (char)_itemArray[ordinal];
        }

        public void SetChar(int ordinal, char value)
        {
            SetValue(ordinal, value);
        }

        public long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            char[] value = (char[])_itemArray[ordinal];
            if (buffer == null)
                return value.Length;
            int copylen = length > value.Length ? value.Length : length;
            Array.Copy(value, dataOffset, buffer, bufferOffset, copylen);
            return copylen;
        }

        public void SetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int ordinal)
        {
            return (DateTime)_itemArray[ordinal];
        }

        public void SetDateTime(int ordinal, DateTime value)
        {
            SetValue(ordinal, value);
        }

        public decimal GetDecimal(int ordinal)
        {
            return (Decimal)_itemArray[ordinal];
        }

        public void SetDecimal(int ordinal, decimal value)
        {
            SetValue(ordinal, value);
        }

        public double GetDouble(int ordinal)
        {
            return (Double)_itemArray[ordinal];
        }

        public void SetDouble(int ordinal, double value)
        {
            SetValue(ordinal, value);
        }

        public float GetFloat(int ordinal)
        {
            return (Single)_itemArray[ordinal];
        }

        public void SetFloat(int ordinal, float value)
        {
            SetValue(ordinal, value);
        }

        public short GetInt16(int ordinal)
        {
            return (Int16)_itemArray[ordinal];
        }

        public void SetInt16(int ordinal, short value)
        {
            SetValue(ordinal, value);
        }

        public ushort GetUInt16(int ordinal)
        {
            return (UInt16)_itemArray[ordinal];
        }

        public void SetUInt16(int ordinal, ushort value)
        {
            SetValue(ordinal, value);
        }

        public int GetInt32(int ordinal)
        {
            return (Int32)_itemArray[ordinal];
        }

        public void SetInt32(int ordinal, int value)
        {
            SetValue(ordinal, value);
        }

        public uint GetUInt32(int ordinal)
        {
            return (UInt32)_itemArray[ordinal];
        }

        public void SetUInt32(int ordinal, uint value)
        {
            SetValue(ordinal, value);
        }

        public long GetInt64(int ordinal)
        {
            return (Int64)_itemArray[ordinal];
        }

        public void SetInt64(int ordinal, long value)
        {
            SetValue(ordinal, value);
        }

        public ulong GetUInt64(int ordinal)
        {
            return (UInt64)_itemArray[ordinal];
        }

        public void SetUInt64(int ordinal, ulong value)
        {
            SetValue(ordinal, value);
        }

        public string GetString(int ordinal)
        {
            return (String)_itemArray[ordinal];
        }

        public void SetString(int ordinal, string value)
        {
            SetValue(ordinal, value);
        }

        public object GetObject(int ordinal)
        {            
            if (IsDbNull(ordinal))
                return null;
            else
                return GetValue(ordinal);
        }

        public void SetObject(int ordinal, object value)
        {
            SetValue(ordinal, value);
        }

        public object GetValue(int ordinal)
        {
            if (IsDbNull(ordinal))
                return DBNull.Value;
            else
                return _itemArray[ordinal];
        }

        public void SetValue(int ordinal, object value)
        {
            if (value == null || value == DBNull.Value)
                _itemArray[ordinal] = null;
            else if (value.GetType() == _type.Fields[ordinal].DataType ||
                     _type.Fields[ordinal].DataType == typeof(System.Object))
                _itemArray[ordinal] = value;
            else
                throw new InvalidCastException();
        }

        public Object this[int index]
        {
            get
            {
                return GetValue(index);
            }
            set
            {
                SetValue(index, value);
            }
        }

        public object[] ItemArray
        {
            get
            {
                return _itemArray;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int k = 0; k < Length; k++)
            {
                if (k > 0)
                    sb.Append(", ");
                object value = GetValue(k);
                if (value == null || value == DBNull.Value)
                    sb.Append("null");
                else
                    sb.Append(value.ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }

        public static int CompareValue(Object a, Object b)
        {
            if (Object.ReferenceEquals(a, b))
                return 0;

            if (a == DBNull.Value || a == null)
                return -1;

            if (b == DBNull.Value || b == null)
                return 1;

            TypeCode typecode = TypeConverter.GetTypeCode(a, b);
            object val1 = TypeConverter.ChangeType(a, typecode);
            object val2 = TypeConverter.ChangeType(b, typecode);
            return ((IComparable)val1).CompareTo(val2);
            
            //return ((IComparable)a).CompareTo(b);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is Row)
            {
                int res;
                Row dest = (Row)obj;
                if (Length < dest.Length)
                    return -1;
                else if (Length > dest.Length)
                    return 0;
                else
                {
                    for (int k = 0; k < Length; k++)
                    {
                        res = CompareValue(GetValue(k), dest.GetValue(k));
                        if (res != 0)
                            return res;
                    }
                    return 0;
                }
            }
            else
                throw new ArgumentException();
        }

        #endregion
    }

    public sealed class RowEqualityComparer : IEqualityComparer<Row>
    {
        #region IEqualityComparer<Row> Members

        public bool Equals(Row x, Row y)
        {
            if (x.Length == y.Length)
            {
                for (int k = 0; k < x.Length; k++)
                    if (!x[k].Equals(y[k]))
                        return false;
                return true;
            }
            else
                return false;
        }

        public int GetHashCode(Row obj)
        {
            int hash = 23; // Josh Bloch's "Effective Java"
            for (int k = 0; k < obj.Length; k++)
                hash = hash * 37 + obj[k].GetHashCode();
            return hash;
        }

        #endregion
    }

    public sealed class RowEqualityComplexComparer : IEqualityComparer<Row>
    {
        #region IEqualityComparer<Row> Members

        public bool Equals(Row x, Row y)
        {
            if (x._type == y._type)
            {
                for (int k = 0; k < x.Length; k++)
                {
                    Row r1 = (Row)x.GetObject(k);
                    Row r2 = (Row)y.GetObject(k);
                    if (r1 == null || r2 == null)
                    {
                        if (r1 == null && r2 == null)
                            return true;
                        else
                            return false;
                    }
                    else
                        if (r1.Length == r2.Length)
                        {
                            for (int s = 0; s < r1.Length; s++)
                                if (!r1[k].Equals(r2[k]))
                                    return false;
                        }
                        else
                            return false;
                }
                return true;
            }
            else
                return false;
        }

        public int GetHashCode(Row obj)
        {
            int hash = 23; // Josh Bloch's "Effective Java"
            for (int k = 0; k < obj.Length; k++)
            {
                Row r = (Row)obj.GetObject(k);
                if (r != null)
                    for (int s = 0; s < r.Length; s++)
                        hash = hash * 37 + r[s].GetHashCode();
            }
            return hash;
        }

        #endregion
    }
}
