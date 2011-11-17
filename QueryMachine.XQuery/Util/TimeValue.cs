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
using System.Xml;
using System.Xml.Schema;

using DataEngine.CoreServices;

namespace DataEngine.XQuery.Util
{
    public class TimeValue: IComparable, IXmlConvertable
    {
        public const int ProxyValueCode = 14;

        public TimeValue(DateTimeOffset value)
        {
            DateTime today = DateTime.Today;
            Value = new DateTimeOffset(new DateTime(today.Year, today.Month, today.Day, value.DateTime.Hour, 
                value.DateTime.Minute, value.DateTime.Second, value.DateTime.Millisecond), value.Offset);
            IsLocal = false;
        }

        public TimeValue(DateTime value)
        {
            DateTime today = DateTime.Today;
            Value = new DateTimeOffset(new DateTime(today.Year, today.Month, today.Day, value.Hour,
                value.Minute, value.Second, value.Millisecond));            
            IsLocal = true;
        }        

        public DateTimeOffset Value { get; private set; }

        public bool IsLocal { get; private set; }

        public override bool Equals(object obj)
        {
            TimeValue other = obj as TimeValue;
            if (other != null)
                return Value.Equals(other.Value);
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
            if (Value.DateTime.Millisecond != 0)
            {
                sb.Append('.');
                sb.Append(Math.Abs(Value.DateTime.Millisecond).ToString("000").TrimEnd('0'));
            }
            if (!IsLocal)
            {
                if (Value.Offset == TimeSpan.Zero)
                    sb.Append("Z");
                else
                    sb.Append(Value.ToString("zzz", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        private static string[] TimeFormats = new string[] {             
            "HH:mm:ss", 
            "HH:mm:ss.f",
            "HH:mm:ss.ff",
            "HH:mm:ss.fff",
            "HH:mm:ss.ffff",
            "HH:mm:ss.fffff",
            "HH:mm:ss.fffffff",
            "HH:mm:ss.ffffffff"
        };

        private static string[] TimeOffsetFormats = new string[] {             
            "HH:mm:sszzz",
            "HH:mm:ss.fzzz",
            "HH:mm:ss.ffzzz",
            "HH:mm:ss.fffzzz",
            "HH:mm:ss.ffffzzz",
            "HH:mm:ss.fffffzzz",
            "HH:mm:ss.fffffffzzz",
            "HH:mm:ss.ffffffffzzz"
        };

        public static TimeValue Parse(string text)
        {
            DateTimeOffset dateTimeOffset;
            DateTime dateTime;
            text = text.Trim();
            int p = text.IndexOf("24:00:00");
            if (p != -1)
            {
                if (p + 8 < text.Length && text[p + 8] == '.')
                    throw new XQueryException(Properties.Resources.InvalidFormat, text, "xs:dateTime");
                text = text.Substring(0, p) + "00:00:00" + text.Substring(p + 8);
            }
            if (text.EndsWith("Z"))
            {
                if (!DateTimeOffset.TryParseExact(text.Substring(0, text.Length - 1), TimeFormats,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | 
                        DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out dateTimeOffset))
                    throw new XQueryException(Properties.Resources.InvalidFormat, text, "xs:time");
                return new TimeValue(dateTimeOffset);
            }
            else
            {
                if (DateTime.TryParseExact(text, TimeFormats, CultureInfo.InvariantCulture, 
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out dateTime))
                    return new TimeValue(dateTime);
                if (!DateTimeOffset.TryParseExact(text, TimeOffsetFormats, CultureInfo.InvariantCulture, 
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out dateTimeOffset))
                    throw new XQueryException(Properties.Resources.InvalidFormat, text, "xs:time");
                return new TimeValue(dateTimeOffset);
            }
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            TimeValue other = obj as TimeValue;
            if (other == null)
                throw new ArgumentException("obj");
            return Value.CompareTo(other.Value);
        }

        #endregion

        public static TimeValue Add(TimeValue tm, DayTimeDurationValue duration)
        {
            DateTimeValue dat;
            if (tm.IsLocal)
                dat = new DateTimeValue(false, DateTime.Today, tm.Value.DateTime);
            else
                dat = new DateTimeValue(false, DateTime.Today, tm.Value.DateTime, tm.Value.Offset);
            DateTime dt = DateTimeValue.Add(dat, duration).Value.DateTime;
            DateTime today = DateTime.Today;
            dt = new DateTime(today.Year, today.Month, today.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
            if (tm.IsLocal)
                return new TimeValue(dt);
            else
                return new TimeValue(new DateTimeOffset(dt, tm.Value.Offset));
        }

        public static DayTimeDurationValue Sub(TimeValue tm1, TimeValue tm2)
        {
            return new DayTimeDurationValue(tm1.Value - tm2.Value);
        }

        internal class ProxyFactory : ValueProxyFactory
        {
            public override ValueProxy Create(object value)
            {
                return new Proxy((TimeValue)value);
            }

            public override int GetValueCode()
            {
                return ProxyValueCode;
            }

            public override Type GetValueType()
            {
                return typeof(TimeValue);
            }

            public override bool IsNumeric
            {
                get { return false; }
            }

            public override int Compare(ValueProxyFactory other)
            {
                return 0;
            }
        }


        internal class Proxy : ValueProxy
        {
            private TimeValue _value;

            public Proxy(TimeValue value)
            {
                _value = value;
            }

            public override int GetValueCode()
            {
                return ProxyValueCode;
            }

            public override object Value
            {
                get 
                {
                    return _value;
                }
            }

            protected override bool Eq(ValueProxy val)
            {
                if (val.GetValueCode() != ProxyValueCode)
                    throw new OperatorMismatchException(Funcs.Eq, _value, val.Value);
                return _value.Equals(((Proxy)val)._value);
            }

            protected override bool Gt(ValueProxy val)
            {
                if (val.GetValueCode() != ProxyValueCode)
                    throw new OperatorMismatchException(Funcs.Gt, _value, val.Value);
                return ((IComparable)_value).CompareTo(((Proxy)val)._value) > 0;
            }

            protected override ValueProxy Promote(ValueProxy val)
            {
                throw new NotImplementedException();
            }

            protected override ValueProxy Neg()
            {
                throw new OperatorMismatchException(Funcs.Neg, _value, null);
            }

            protected override ValueProxy Add(ValueProxy value)
            {
                switch (value.GetValueCode())
                {
                    case DayTimeDurationValue.ProxyValueCode:
                        return new Proxy(TimeValue.Add(_value, (DayTimeDurationValue)value.Value));

                    default:
                        throw new OperatorMismatchException(Funcs.Add, _value, value.Value);
                }
            }

            protected override ValueProxy Sub(ValueProxy value)
            {
                switch (value.GetValueCode())
                {
                    case TimeValue.ProxyValueCode:
                        return new DayTimeDurationValue.Proxy(TimeValue.Sub(_value, (TimeValue)value.Value));
                    case DayTimeDurationValue.ProxyValueCode:
                        return new Proxy(TimeValue.Add(_value, -(DayTimeDurationValue)value.Value));

                    default:
                        throw new OperatorMismatchException(Funcs.Sub, _value, value.Value);
                }
            }

            protected override ValueProxy Mul(ValueProxy value)
            {
                throw new OperatorMismatchException(Funcs.Mul, _value, value.Value);
            }

            protected override ValueProxy Div(ValueProxy value)
            {
                throw new OperatorMismatchException(Funcs.Div, _value, value.Value);
            }

            protected override Integer IDiv(ValueProxy value)
            {
                throw new OperatorMismatchException(Funcs.IDiv, _value, value.Value);
            }

            protected override ValueProxy Mod(ValueProxy value)
            {
                throw new OperatorMismatchException(Funcs.Div, _value, value.Value);
            }
        }

        #region IXmlConvertable Members

        object IXmlConvertable.ValueAs(XQuerySequenceType type, XmlNamespaceManager nsmgr)
        {
            switch (type.TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.Time:
                    return this;

                case XmlTypeCode.String:
                    return ToString();

                case XmlTypeCode.UntypedAtomic:
                    return new UntypedAtomic(ToString());

                default:
                    throw new InvalidCastException();
            }
        }

        #endregion
    }
}
