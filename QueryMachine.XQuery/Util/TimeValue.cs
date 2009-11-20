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
using System.Xml;
using System.Xml.Schema;

using DataEngine.CoreServices;

namespace DataEngine.XQuery.Util
{
    public class TimeValue: IComparable, IXmlConvertable
    {
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

        internal class Proxy : TypeProxy
        {
            public override bool Eq(object arg1, object arg2)
            {
                return arg1.Equals(arg2);
            }

            public override bool Gt(object arg1, object arg2)
            {
                return ((IComparable)arg1).CompareTo(arg2) > 0;
            }

            public override object Promote(object arg1)
            {
                TimeValue time = arg1 as TimeValue;
                if (time == null)
                    throw new InvalidCastException();
                return time;
            }

            public override object Neg(object arg1)
            {
                throw new OperatorMismatchException(Funcs.Neg, arg1, null);
            }

            public override object Add(object arg1, object arg2)
            {
                if (arg1 is TimeValue && arg2 is DayTimeDurationValue)
                    return TimeValue.Add((TimeValue)arg1, (DayTimeDurationValue)arg2);
                else
                    throw new OperatorMismatchException(Funcs.Add, arg1, arg2);
            }

            public override object Sub(object arg1, object arg2)
            {
                if (arg1 is TimeValue && arg2 is TimeValue)
                    return TimeValue.Sub((TimeValue)arg1, (TimeValue)arg2);
                else if (arg1 is TimeValue && arg2 is DayTimeDurationValue)
                    return TimeValue.Add((TimeValue)arg1, -(DayTimeDurationValue)arg2);
                else
                    throw new OperatorMismatchException(Funcs.Sub, arg1, arg2);
            }

            public override object Mul(object arg1, object arg2)
            {
                throw new OperatorMismatchException(Funcs.Mul, arg1, arg2);
            }

            public override object Div(object arg1, object arg2)
            {
                throw new OperatorMismatchException(Funcs.Div, arg1, arg2);
            }

            public override Integer IDiv(object arg1, object arg2)
            {
                throw new OperatorMismatchException(Funcs.IDiv, arg1, arg2);
            }

            public override object Mod(object arg1, object arg2)
            {
                throw new OperatorMismatchException(Funcs.Div, arg1, arg2);
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
