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

using DataEngine.CoreServices;

namespace DataEngine.XQuery.Util
{
    public class YearMonthDurationValue: DurationValue, IComparable
    {
        public YearMonthDurationValue(TimeSpan value)
            : base(value, TimeSpan.Zero)
        {
        }

        public override string ZeroStringValue()
        {
            return "P0M";
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            YearMonthDurationValue other = obj as YearMonthDurationValue;
            if (other == null)
                throw new ArgumentException("obj");
            return HighPartValue.CompareTo(other.HighPartValue);
        }

        #endregion

        private static int DaysToMonth(int days)
        {
            return days / 365 * 12 + days % 365 / 30;
        }

        private static int MonthToDays(int month)
        {
            return month / 12 * 365 + month % 12 * 30;
        }

        private static YearMonthDurationValue Multiply(YearMonthDurationValue a, double b)
        {
            if (Double.IsNaN(b) || Double.IsNegativeInfinity(b) || Double.IsPositiveInfinity(b))
                throw new XQueryException(Properties.Resources.FOCA0005);
            //int month = (int)Math.Round(DaysToMonth(a.Value.Days) * b, MidpointRounding.AwayFromZero);
            int month = (int)Math.Floor(0.5 + DaysToMonth(a.HighPartValue.Days) * b);
            return new YearMonthDurationValue(new TimeSpan(MonthToDays(month), 0, 0, 0));
        }

        public static YearMonthDurationValue Divide(YearMonthDurationValue a, double b)
        {
            if (b == 0.0)
                throw new XQueryException(Properties.Resources.FOAR0001);
            if (Double.IsNaN(b))
                throw new XQueryException(Properties.Resources.FOCA0005);
            //int month = (int)Math.Round(DaysToMonth(a.Value.Days) / b, MidpointRounding.AwayFromZero);
            int month = (int)Math.Floor(0.5 + DaysToMonth(a.HighPartValue.Days) / b);
            return new YearMonthDurationValue(new TimeSpan(MonthToDays(month), 0, 0, 0));
        }

        public static decimal Divide(YearMonthDurationValue a, YearMonthDurationValue b)
        {
            int month1 = DaysToMonth(a.HighPartValue.Days);
            int month2 = DaysToMonth(b.HighPartValue.Days);
            if (month2 == 0)
                throw new XQueryException(Properties.Resources.FOAR0001);
            return (decimal)month1 / (decimal)month2;
        }

        public static YearMonthDurationValue operator -(YearMonthDurationValue d)
        {
            return new YearMonthDurationValue(-d.HighPartValue);
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
                DurationValue duration = arg1 as DurationValue;
                if (duration == null)
                    throw new InvalidCastException();
                return new YearMonthDurationValue(duration.HighPartValue);
            }

            public override object Neg(object arg1)
            {
                throw new OperatorMismatchException(Funcs.Neg, arg1, null);
            }

            public override object Add(object arg1, object arg2)
            {
                if (arg1 is YearMonthDurationValue)
                {
                    if (arg2 is YearMonthDurationValue)
                        return new YearMonthDurationValue(((YearMonthDurationValue)arg1).HighPartValue + ((YearMonthDurationValue)arg2).HighPartValue);
                    if (arg2 is DateTimeValue)
                        return DateTimeValue.Add((DateTimeValue)arg2, (YearMonthDurationValue)arg1);
                    if (arg2 is DateValue)
                        return DateValue.Add((DateValue)arg2, (YearMonthDurationValue)arg1);
                }
                throw new OperatorMismatchException(Funcs.Add, arg1, arg2);
            }

            public override object Sub(object arg1, object arg2)
            {
                if (arg1 is YearMonthDurationValue && arg2 is YearMonthDurationValue)
                    return new YearMonthDurationValue(((YearMonthDurationValue)arg1).HighPartValue - ((YearMonthDurationValue)arg2).HighPartValue);
                else
                    throw new OperatorMismatchException(Funcs.Sub, arg1, arg2);
            }

            public override object Mul(object arg1, object arg2)
            {
                if (arg1 is YearMonthDurationValue && TypeConverter.IsNumberType(arg2.GetType()))
                    return YearMonthDurationValue.Multiply((YearMonthDurationValue)arg1, Convert.ToDouble(arg2));
                else if (TypeConverter.IsNumberType(arg1.GetType()) && arg2 is YearMonthDurationValue)
                    return YearMonthDurationValue.Multiply((YearMonthDurationValue)arg2, Convert.ToDouble(arg1));
                else
                    throw new OperatorMismatchException(Funcs.Mul, arg1, arg2);
            }

            public override object Div(object arg1, object arg2)
            {
                if (arg1 is YearMonthDurationValue && TypeConverter.IsNumberType(arg2.GetType()))
                    return YearMonthDurationValue.Divide((YearMonthDurationValue)arg1, Convert.ToDouble(arg2));
                else if (arg1 is YearMonthDurationValue && arg2 is YearMonthDurationValue)
                    return YearMonthDurationValue.Divide((YearMonthDurationValue)arg1, (YearMonthDurationValue)arg2);
                else
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
    }
}
