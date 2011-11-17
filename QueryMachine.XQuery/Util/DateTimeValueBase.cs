//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

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

        public bool IsLocal { get; set; }

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
