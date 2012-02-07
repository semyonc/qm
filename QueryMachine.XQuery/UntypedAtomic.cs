﻿//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
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

namespace DataEngine.XQuery
{
    public class UntypedAtomic: ICloneable, IComparable, IConvertible, IEquatable<UntypedAtomic>, IComparable<UntypedAtomic>
    {
        public UntypedAtomic(string value)
        {
            Value = value;
        }

        public String Value { get; private set; }

        private object _doubleValue;

        public override bool Equals(object obj)
        {
            UntypedAtomic src = obj as UntypedAtomic;
            if (src == null)
                return false;
            return src.Value.Equals(Value);
        }

        public override int GetHashCode()
        {
            if (Value == null)
                return 0;
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        private bool CanBeNumber()
        {
            if (!String.IsNullOrEmpty(Value))
            {
                char c = Value[0];
                return Char.IsDigit(c) || c == '-' || c == '.' ||
                    (Value.Length == 3 && (c == 'N' || c == 'I'));
            }
            return false;
        }

        public bool TryParseDouble(out double num)
        {
            if (_doubleValue != null)
            {
                num = (double)_doubleValue;
                return true;
            }
            if (CanBeNumber())
            {
                if (Value == "NaN")
                {
                    num = Double.NaN;
                    _doubleValue = num;
                    return true;
                }
                else if (Value == "INF")
                {
                    num = Double.PositiveInfinity;
                    _doubleValue = num;
                    return true;
                }
                else if (Value == "-INF")
                {
                    num = Double.NegativeInfinity;
                    _doubleValue = num;
                    return true;
                }
                else
                    if (Double.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
                    {
                        if (num == 0.0 & Value.StartsWith("-"))
                            num = -num; // -0, -0.0,... etc
                        _doubleValue = num;
                        return true;
                    }
            }
            num = 0.0;
            return false;
        }

        #region ICloneable Members

        public object Clone()
        {
            return new UntypedAtomic(Value);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            UntypedAtomic src = obj as UntypedAtomic;
            if (src == null)
                throw new ArgumentNullException("obj");
            return Value.CompareTo(src.Value);
        }

        #endregion

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            try
            {
                return Convert.ToBoolean(Value, provider);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:boolean");
            }
        }

        public byte ToByte(IFormatProvider provider)
        {
            try
            {
                return Convert.ToByte(Value, provider);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:unsignedByte");
            }
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            try
            {
                return Convert.ToDecimal(Value, provider);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:decimal");
            }
        }

        public float ToSingle(IFormatProvider provider)
        {
            try
            {
                float num;
                if (Value == "NaN")
                    num = Single.NaN;
                else if (Value == "INF")
                    num = Single.PositiveInfinity;
                else if (Value == "-INF")
                    num = Single.NegativeInfinity;
                else
                    num = Convert.ToSingle(Value, provider);
                if (num == 0.0 && Value[0] == '-')
                    num = -num; // -0, -0.0,... etc
                return num;
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:float");
            }
        }

        public double ToDouble(IFormatProvider provider)
        {
            try
            {
                if (_doubleValue == null)
                {
                    double num;
                    if (Value == "NaN")
                        num = Double.NaN;
                    else if (Value == "INF")
                        num = Double.PositiveInfinity;
                    else if (Value == "-INF")
                        num = Double.NegativeInfinity;
                    else
                        num = Convert.ToDouble(Value, provider);
                    if (num == 0.0 && Value[0] == '-')
                        num = -num; // -0, -0.0,... etc
                    _doubleValue = num;
                }
                return (double)_doubleValue;
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:double");
            }
        }

        public short ToInt16(IFormatProvider provider)
        {
            try
            {
                return Convert.ToInt16(Value, provider);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:short");
            }
        }

        public int ToInt32(IFormatProvider provider)
        {
            try
            {
                return Convert.ToInt32(Value, provider);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:int");
            }
        }

        public long ToInt64(IFormatProvider provider)
        {
            try
            {
                return Convert.ToInt64(Value, provider);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:long");
            }
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            try
            {
                return Convert.ToSByte(Value, provider);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:byte");
            }
        }

        public string ToString(IFormatProvider provider)
        {
            return Value;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Value, conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            try
            {
                return Convert.ToUInt16(Value, provider);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:unsignedShort");
            }
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            try
            {
                return Convert.ToUInt32(Value, provider);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:unsignedInt");
            }
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            try
            {
                return Convert.ToUInt64(Value, provider);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, Value, "xs:unsignedLong");
            }
        }

        #endregion

        #region IEquatable<UntypedAtomic> Members

        bool IEquatable<UntypedAtomic>.Equals(UntypedAtomic other)
        {
            if (other == null)
                return false;
            return Value.Equals(other.Value);
        }

        #endregion

        #region IComparable<UntypedAtomic> Members

        int IComparable<UntypedAtomic>.CompareTo(UntypedAtomic other)
        {
            if (other == null)
                throw new ArgumentNullException("other");
            return Value.CompareTo(other.Value);
        }

        #endregion
    }
}
