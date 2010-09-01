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
    public class DateTimeValue: DateTimeValueBase, IXmlConvertable
    {
        public const int ProxyValueCode = 10;

        public DateTimeValue(bool sign, DateTime value)
            : base(sign, value)
        {
        }

        public DateTimeValue(bool sign, DateTimeOffset value)
            : base(sign, value)
        {
        }

        public DateTimeValue(bool sign, DateTime date, DateTime time)
            : base(sign, date, time)
        {
        }

        public DateTimeValue(bool sign, DateTime date, DateTime time, TimeSpan offset)
            : base(sign, date, time, offset)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (S)
                sb.Append("-");            
            sb.Append(Value.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture));
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

        private static string[] DateTimeFormats = new string[] {             
            "yyyy-MM-dd'T'HH:mm:ss", 
            "yyyy-MM-dd'T'HH:mm:ss.f",
            "yyyy-MM-dd'T'HH:mm:ss.ff",
            "yyyy-MM-dd'T'HH:mm:ss.fff",
            "yyyy-MM-dd'T'HH:mm:ss.ffff",
            "yyyy-MM-dd'T'HH:mm:ss.fffff",
            "yyyy-MM-dd'T'HH:mm:ss.fffffff",
            "yyyy-MM-dd'T'HH:mm:ss.ffffffff",
            "'-'yyyy-MM-dd'T'HH:mm:ss", 
            "'-'yyyy-MM-dd'T'HH:mm:ss.f",
            "'-'yyyy-MM-dd'T'HH:mm:ss.ff",
            "'-'yyyy-MM-dd'T'HH:mm:ss.fff",
            "'-'yyyy-MM-dd'T'HH:mm:ss.ffff",
            "'-'yyyy-MM-dd'T'HH:mm:ss.fffff",
            "'-'yyyy-MM-dd'T'HH:mm:ss.fffffff",
            "'-'yyyy-MM-dd'T'HH:mm:ss.ffffffff"
        };

        private static string[] DateTimeOffsetFormats = new string[] {             
            "yyyy-MM-dd'T'HH:mm:sszzz",
            "yyyy-MM-dd'T'HH:mm:ss.fzzz",
            "yyyy-MM-dd'T'HH:mm:ss.ffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.fffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.ffffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.fffffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.fffffffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.ffffffffzzz",
            "'-'yyyy-MM-dd'T'HH:mm:sszzz",
            "'-'yyyy-MM-dd'T'HH:mm:ss.fzzz",
            "'-'yyyy-MM-dd'T'HH:mm:ss.ffzzz",
            "'-'yyyy-MM-dd'T'HH:mm:ss.fffzzz",
            "'-'yyyy-MM-dd'T'HH:mm:ss.ffffzzz",
            "'-'yyyy-MM-dd'T'HH:mm:ss.fffffzzz",
            "'-'yyyy-MM-dd'T'HH:mm:ss.fffffffzzz",
            "'-'yyyy-MM-dd'T'HH:mm:ss.ffffffffzzz"
        };

        public static DateTimeValue Parse(string text)
        {
            DateTimeOffset dateTimeOffset;
            DateTime dateTime;
            text = text.Trim();
            int p = text.IndexOf("24:00:00");
            bool add_day = false;
            if (p != -1)
            {
                if (p + 8 < text.Length && text[p + 8] == '.')
                    throw new XQueryException(Properties.Resources.InvalidFormat, text, "xs:dateTime");
                text = text.Substring(0, p) + "00:00:00" + text.Substring(p + 8);
                add_day = true;
            }
            bool s = text.StartsWith("-");
            DateTimeValue dat;
            if (text.EndsWith("Z"))
            {
                if (!DateTimeOffset.TryParseExact(text.Substring(0, text.Length - 1), DateTimeFormats,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTimeOffset))
                    throw new XQueryException(Properties.Resources.InvalidFormat, text, "xs:dateTime");
                dat = new DateTimeValue(s, dateTimeOffset);
            }
            else
            {
                if (DateTime.TryParseExact(text, DateTimeFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTime))
                    dat = new DateTimeValue(s, dateTime);
                else
                {
                    if (!DateTimeOffset.TryParseExact(text, DateTimeOffsetFormats, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out dateTimeOffset))
                        throw new XQueryException(Properties.Resources.InvalidFormat, text, "xs:dateTime");
                    dat = new DateTimeValue(s, dateTimeOffset);
                }
            }
            if (add_day)
                return DateTimeValue.Add(dat, new DayTimeDurationValue(new TimeSpan(1, 0, 0, 0)));
            return dat;
        }
    
        private static short[] monthData = {306, 337, 0, 31, 61, 92, 122, 153, 184, 214, 245, 275};

        // http://vsg.cape.com/~pbaum/date/jdalg.htm 
        // http://vsg.cape.com/~pbaum/date/jdalg2.htm
        public static int GetJulianDayNumber(int year, int month, int day)
        {
            int z = year - (month < 3 ? 1 : 0);
            short f = monthData[month - 1];
            if (z >= 0)
                return day + f + 365 * z + z / 4 - z / 100 + z / 400 + 1721118;
            else
            {
                // for negative years, add 12000 years and then subtract the days!
                z += 12000;
                int j = day + f + 365 * z + z / 4 - z / 100 + z / 400 + 1721118;
                return j - (365 * 12000 + 12000 / 4 - 12000 / 100 + 12000 / 400);  // number of leap years in 12000 years
            }
        }

        // http://www.hermetic.ch/cal_stud/jdn.htm#comp
        public static DateTimeValue GetDateFromJulianDayNumber(int julianDayNumber)
        {
            if (julianDayNumber >= 0)
            {
                int L = julianDayNumber + 68569 + 1;    // +1 adjustment for days starting at noon
                int n = (4 * L) / 146097;
                L = L - (146097 * n + 3) / 4;
                int i = (4000 * (L + 1)) / 1461001;
                L = L - (1461 * i) / 4 + 31;
                int j = (80 * L) / 2447;
                int d = L - (2447 * j) / 80;
                L = j / 11;
                int m = j + 2 - (12 * L);
                int y = 100 * (n - 49) + i + L;
                if (y > 0)
                    return new DateTimeValue(false, new DateTime(y, m, d));
                else
                    return new DateTimeValue(true, new DateTime(Math.Abs(y) + 1, m, d));
            }
            else
            {
                // add 12000 years and subtract them again...
                DateTime dt = GetDateFromJulianDayNumber(julianDayNumber +
                        (365 * 12000 + 12000 / 4 - 12000 / 100 + 12000 / 400)).Value.DateTime;
                return new DateTimeValue(true, new DateTime(Math.Abs(dt.Year - 12000), dt.Month, dt.Day));
            }
        }

        public decimal ToJulianInstant()
        {
            int sign = S ? -1 : 1;
            DateTime dt = Value.DateTime;
            int julianDay = GetJulianDayNumber(sign * dt.Year, dt.Month, dt.Day);
            long julianSecond = julianDay * (24L * 60L * 60L);
            julianSecond += (((dt.Hour * 60L + dt.Minute) * 60L) + dt.Second);
            decimal j = julianSecond;
            if (dt.Millisecond != 0)
                j += Math.Round((decimal)dt.Millisecond / 1000000, 6, MidpointRounding.ToEven);
            return j;
        }

        public static DateTimeValue CreateFromJulianInstant(decimal instant)
        {
            long js = (long)Decimal.Truncate(instant);
            decimal microseconds = (instant - js) * 1000000;
            long jd = js / (24L * 60L * 60L);
            DateTimeValue dt = GetDateFromJulianDayNumber((int)jd);
            js = js % (24L * 60L * 60L);
            int hour = (int)(js / (60L * 60L));
            js = js % (60L * 60L);
            int minute = (int)(js / 60L);
            js = js % (60L);
            return new DateTimeValue(dt.S, dt.Value.Date, 
                new DateTime(1, 1, 1, hour, minute, (int)js, (int)microseconds));            
        }

        public static DateTimeValue Add(DateTimeValue dat, YearMonthDurationValue duration)
        {
            try
            {

                Calendar calender = CultureInfo.InvariantCulture.Calendar;
                DateTime dt = dat.Value.DateTime;
                int year = dat.S ? -dt.Year : dt.Year - 1;
                int m = (dt.Month - 1) + duration.Months;
                year = year + duration.Years + m / 12;
                if (year >= 0)
                    year = year + 1;
                m = m % 12;
                if (m < 0)
                {
                    m += 12;
                    year -= 1;
                }
                m++;
                int day = Math.Min(dt.Day, calender.GetDaysInMonth(Math.Abs(year), m));
                if (year < 0)
                    dt = new DateTime(-year, m, day);
                else
                    dt = new DateTime(year, m, day);
                if (dat.IsLocal)
                    return new DateTimeValue(year < 0, dt, dat.Value.DateTime);
                else
                    return new DateTimeValue(year < 0, dt, dat.Value.DateTime, dat.Value.Offset);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new XQueryException(Properties.Resources.FODT0001);
            }
        }

        public static DateTimeValue Add(DateTimeValue dat, DayTimeDurationValue duration)
        {
            try
            {
                decimal seconds = (decimal)duration.LowPartValue.Ticks / TimeSpan.TicksPerSecond;
                decimal julian = dat.ToJulianInstant();
                julian += seconds;
                DateTimeValue dt = CreateFromJulianInstant(julian);
                if (dat.IsLocal)
                    return dt;
                else
                    return new DateTimeValue(dt.S, new DateTimeOffset(dt.Value.DateTime, dat.Value.Offset));
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new XQueryException(Properties.Resources.FODT0001);
            }
        }

        private static DayTimeDurationValue Sub(DateTimeValue dat1, DateTimeValue dat2)
        {
            try
            {
                TimeSpan ts1;
                TimeSpan ts2;
                if (dat1.IsLocal && dat2.IsLocal)
                {
                    ts1 = dat1.Value.DateTime - DateTime.MinValue;
                    ts2 = dat2.Value.DateTime - DateTime.MinValue;
                }
                else
                {
                    ts1 = dat1.Value.ToUniversalTime().DateTime - DateTime.MinValue;
                    ts2 = dat2.Value.ToUniversalTime().DateTime - DateTime.MinValue;
                }
                if (dat1.S)
                    ts1 = -ts1;
                if (dat2.S)
                    ts2 = -ts2;
                return new DayTimeDurationValue(ts1 - ts2);
            }
            catch (OverflowException)
            {
                throw new XQueryException(Properties.Resources.FODT0001);
            }
        }

        internal class ProxyFactory : ValueProxyFactory
        {
            public override ValueProxy Create(object value)
            {
                return new Proxy((DateTimeValue)value);
            }

            public override int GetValueCode()
            {
                return ProxyValueCode;
            }

            public override Type GetValueType()
            {
                return typeof(DateTimeValue);
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
            private DateTimeValue _value;

            public Proxy(DateTimeValue value)
            {
                _value = value;
            }

            public override object Value
            {
                get 
                {
                    return _value;
                }
            }

            public override int GetValueCode()
            {
                return ProxyValueCode;
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
                    case YearMonthDurationValue.ProxyValueCode:
                        return new Proxy(DateTimeValue.Add(_value, (YearMonthDurationValue)value.Value));
                    case DayTimeDurationValue.ProxyValueCode:
                        return new Proxy(DateTimeValue.Add(_value, (DayTimeDurationValue)value.Value));
                    
                    default:
                        throw new OperatorMismatchException(Funcs.Add, _value, value.Value);
                }
            }

            protected override ValueProxy Sub(ValueProxy value)
            {
                switch (value.GetValueCode())
                {
                    case DateTimeValue.ProxyValueCode:
                        return new DayTimeDurationValue.Proxy(DateTimeValue.Sub(_value, (DateTimeValue)value.Value));
                    case YearMonthDurationValue.ProxyValueCode:
                        return new Proxy(DateTimeValue.Add(_value, -(YearMonthDurationValue)value.Value));
                    case DayTimeDurationValue.ProxyValueCode:
                        return new Proxy(DateTimeValue.Add(_value, -(DayTimeDurationValue)value.Value));
                    
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
                case XmlTypeCode.DateTime:
                    return this;
                
                case XmlTypeCode.Date:
                    if (IsLocal)
                        return new DateValue(S, Value.Date);
                    else
                        return new DateValue(S,  
                            new DateTimeOffset(DateTime.SpecifyKind(Value.Date, DateTimeKind.Unspecified), Value.Offset));

                case XmlTypeCode.Time:
                    if (IsLocal)
                        return new TimeValue(Value.DateTime);
                    else
                        return new TimeValue(Value);

                case XmlTypeCode.GYear:
                    return ToGYear();

                case XmlTypeCode.GYearMonth:
                    return ToGYearMonth();

                case XmlTypeCode.GMonth:
                    return ToGMonth();

                case XmlTypeCode.GMonthDay:
                    return ToGMonthDay();

                case XmlTypeCode.GDay:
                    return ToGDay();

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
