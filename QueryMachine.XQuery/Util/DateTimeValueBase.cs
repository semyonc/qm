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

namespace DataEngine.XQuery.Util
{
    public abstract class DateTimeValueBase: IComparable, IConvertible
    {
        public DateTimeValueBase(bool sign, DateTime value)
        {
            S = sign;
            Value = new DateTimeOffset(value);
            IsLocal = true;
        }

        public DateTimeValueBase(bool sign, DateTimeOffset value)
        {
            S = sign;
            Value = value;
            IsLocal = false;
        }

        public DateTimeValueBase(bool sign, DateTime date, DateTime time)
        {
            S = sign;
            IsLocal = true;
            Value = new DateTimeOffset(new DateTime(date.Year, date.Month,
                date.Day, time.Hour, time.Minute, time.Second, time.Millisecond));
        }

        public DateTimeValueBase(bool sign, DateTime date, DateTime time, TimeSpan offset)
        {
            S = sign;
            IsLocal = false;
            Value = new DateTimeOffset(new DateTime(date.Year, date.Month,
                date.Day, time.Hour, time.Minute, time.Second, time.Millisecond), offset);
        }

        public bool S { get; protected set; }

        public DateTimeOffset Value { get; protected set; }

        public bool IsLocal { get; protected set; }

        public override bool Equals(object obj)
        {
            DateTimeValueBase other = obj as DateTimeValueBase;
            if (other != null)
                return Value.Equals(other.Value);
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            DateTimeValueBase other = obj as DateTimeValueBase;
            if (other == null)
                throw new ArgumentException("obj");
            return Value.CompareTo(other.Value);
        }

        #endregion     

        public GYearMonthValue ToGYearMonth()
        {
            if (IsLocal)
                return new GYearMonthValue(S, new DateTime(Value.Year, Value.Month, 1));
            else
                return new GYearMonthValue(S, new DateTimeOffset(new DateTime(Value.Year, Value.Month, 1), Value.Offset));
        }

        public GYearValue ToGYear()
        {
            DateTime today = DateTime.Today;
            if (IsLocal)
                return new GYearValue(S, new DateTime(Value.Year, 1, 1));
            else
                return new GYearValue(S, new DateTimeOffset(new DateTime(Value.Year, 1, 1), Value.Offset));
        }

        public GDayValue ToGDay()
        {            
            if (IsLocal)
                return new GDayValue(new DateTime(2008, 1, Value.Day));
            else
                return new GDayValue(new DateTimeOffset(new DateTime(2008, 1, Value.Day), Value.Offset));
        }

        public GMonthValue ToGMonth()
        {
            if (IsLocal)
                return new GMonthValue(new DateTime(2008, Value.Month, 1));
            else
                return new GMonthValue(new DateTimeOffset(new DateTime(2008, Value.Month, 1), Value.Offset));
        }

        public GMonthDayValue ToGMonthDay()
        {
            if (IsLocal)
                return new GMonthDayValue(new DateTime(2008, Value.Month, Value.Day));
            else
                return new GMonthDayValue(new DateTimeOffset(new DateTime(2008, Value.Month, Value.Day), Value.Offset));
        }

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.DateTime;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(Value.DateTime, provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(Value.DateTime, provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(Value.DateTime, provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(Value.DateTime, provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(Value.DateTime, provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(Value.DateTime, provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(Value.DateTime, provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(Value.DateTime, provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(Value.DateTime, provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(Value.DateTime, provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(Value.DateTime, provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return Convert.ToString(Value.DateTime, provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Value.DateTime, conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Value.DateTime, provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Value.DateTime, provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Value.DateTime, provider);
        }

        #endregion
    }
}
