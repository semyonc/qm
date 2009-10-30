//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace DataEngine.CoreServices
{
    public struct Integer: IFormattable, IComparable, IConvertible, IComparable<Integer>, IEquatable<Integer>
    {
        private decimal _value;

        internal Integer(Decimal value)
        {
            _value = value;
        }

        public Integer(Int32 value)
        {
            _value = new Decimal(value);
        }

        public Integer(UInt32 value)
        {
            _value = new Decimal(value);
        }

        public Integer(Int64 value)
        {
            _value = new Decimal(value);
        }

        public Integer(UInt64 value)
        {
            _value = new Decimal(value);
        }

        public Integer(Integer other)
        {
            _value = other._value;
        }

        public static implicit operator Integer(byte value)
        {
            return (new Integer(value));
        }

        public static implicit operator Integer(sbyte value)
        {
            return (new Integer(value));
        }

        public static implicit operator Integer(short value)
        {
            return (new Integer(value));
        }

        public static implicit operator Integer(ushort value)
        {
            return (new Integer(value));
        }

        public static implicit operator Integer(long value)
        {
            return (new Integer(value));
        }

        public static implicit operator Integer(ulong value)
        {
            return (new Integer(value));
        }

        public static implicit operator Integer(int value)
        {
            return (new Integer(value));
        }

        public static implicit operator Integer(uint value)
        {
            return (new Integer(value));
        }

        public static explicit operator SByte(Integer i1)
        {
            return (SByte)i1._value;
        }

        public static explicit operator Byte(Integer i1)
        {
            return (Byte)i1._value;
        }

        public static explicit operator Char(Integer i1)
        {
            return (Char)i1._value;
        }

        public static explicit operator Int16(Integer i1)
        {
            return (Int16)i1._value;
        }

        public static explicit operator UInt16(Integer i1)
        {
            return (UInt16)i1._value;
        }

        public static explicit operator Int32(Integer i1)
        {
            return (Int32)i1._value;
        }

        public static explicit operator UInt32(Integer i1)
        {
            return (UInt32)i1._value;
        }

        public static explicit operator Int64(Integer i1)
        {
            return (Int64)i1._value;
        }

        public static explicit operator UInt64(Integer i1)
        {
            return (UInt64)i1._value;
        }

        public static explicit operator Single(Integer i1)
        {
            return (Single)i1._value;
        }

        public static explicit operator Double(Integer i1)
        {
            return (Double)i1._value;
        }

        public static explicit operator Decimal(Integer i1)
        {
            return i1._value;
        }

        public static explicit operator Integer(Decimal value)
        {
            return new Integer(Decimal.Truncate(value));
        }

        public static explicit operator Integer(Single value)
        {
            return new Integer(Decimal.Truncate(new Decimal(value)));
        }

        public static explicit operator Integer(Double value)
        {
            return new Integer(Decimal.Truncate(new Decimal(value)));
        }

        public override bool Equals(object obj)
        {
            if (obj is Integer)
                return _value.Equals(((Integer)obj)._value);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static string ToString(Integer value)
        {
            return value.ToString();
        }

        public static Integer operator +(Integer i1, Integer i2)
        {
            return new Integer(i1._value + i2._value);
        }

        public static Integer operator ++(Integer i1)
        {
            return new Integer(i1._value + 1);
        }

        public static Integer operator -(Integer i1)
        {
            return new Integer(0 -i1._value);
        }

        public static Integer operator -(Integer i1, Integer i2)
        {
            return new Integer(i1._value - i2._value);
        }

        public static Integer operator --(Integer i1)
        {
            return new Integer(i1._value - 1);
        }

        public static Integer operator *(Integer i1, Integer i2)
        {
            return new Integer(i1._value * i2._value);
        }

        public static Integer operator /(Integer i1, Integer i2)
        {
            return new Integer(Decimal.Truncate(i1._value / i2._value));
        }

        public static Integer operator %(Integer i1, Integer i2)
        {
            return new Integer(i1._value % i2._value);
        }

        public static bool operator ==(Integer i1, Integer i2)
        {
            return i1._value.Equals(i2._value);
        }

        public static bool operator !=(Integer i1, Integer i2)
        {
            return !i1._value.Equals(i2._value);
        }

        public static bool operator >(Integer i1, Integer i2)
        {
            return i1._value > i2._value;
        }

        public static bool operator <(Integer i1, Integer i2)
        {
            return i1._value < i2._value;
        }

        public static bool operator >=(Integer i1, Integer i2)
        {
            return (i1 == i2 || i1 > i2);
        }


        public static bool operator <=(Integer i1, Integer i2)
        {
            return (i1 == i2 || i1 < i2);
        }

        public static bool IsDerivedSubtype(object value)
        {
            NumericCode code = TypeConverter.GetNumericCode(value.GetType());
            switch (code)
            {
                case NumericCode.Unknown:
                case NumericCode.Float:
                case NumericCode.Double:               
                    return false;

                case NumericCode.Decimal:
                    return Decimal.Truncate((decimal)value) == (decimal)value;

                default:
                    return true;
            }
        }

        public static Integer ToInteger(object value)
        {
             return (Integer)Convert.ToDecimal(value);
        }

        #region IFormattable Members

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return _value.ToString(format, formatProvider);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (!(obj is Integer))
                throw new ArgumentException("Object type must be a DataEngine.CoreServices.Integer");
            return _value.CompareTo(((Integer)obj)._value);
        }

        #endregion

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(_value, provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(_value, provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(_value, provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(_value, provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(_value, provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(_value, provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(_value, provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(_value, provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(_value, provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(_value, provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(_value, provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return Convert.ToString(_value, provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(_value, conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(_value, provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(_value, provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(_value, provider);
        }

        #endregion

        #region IComparable<Integer> Members

        int IComparable<Integer>.CompareTo(Integer other)
        {
            return _value.CompareTo(other._value);
        }

        #endregion

        #region IEquatable<Integer> Members

        bool IEquatable<Integer>.Equals(Integer other)
        {
            return _value.Equals(other._value);
        }

        #endregion
    }
}
