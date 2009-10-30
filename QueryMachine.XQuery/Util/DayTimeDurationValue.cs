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
    public class DayTimeDurationValue: DurationValue, IComparable, IRuntimeExtension
    {
        public DayTimeDurationValue(TimeSpan value)
            : base(TimeSpan.Zero, value)
        {
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            DayTimeDurationValue other = obj as DayTimeDurationValue;
            if (other == null)
                throw new ArgumentException("obj");
            return LowPartValue.CompareTo(other.LowPartValue);
        }

        #endregion

        private static DayTimeDurationValue Multiply(DayTimeDurationValue a, double b)
        {
            if (Double.IsNaN(b) || Double.IsNegativeInfinity(b) || Double.IsPositiveInfinity(b))
                throw new XQueryException(Properties.Resources.FOCA0005);
            long timespan = (long)(a.LowPartValue.Ticks * b);
            return new DayTimeDurationValue(new TimeSpan(timespan));
        }

        private static DayTimeDurationValue Divide(DayTimeDurationValue a, double b)
        {
            if (b == 0.0)
                throw new XQueryException(Properties.Resources.FOAR0001);
            if (Double.IsNaN(b))
                throw new XQueryException(Properties.Resources.FOCA0005);
            long timespan = (long)(a.LowPartValue.Ticks / b);
            return new DayTimeDurationValue(new TimeSpan(timespan));
        }

        private static decimal Divide(DayTimeDurationValue a, DayTimeDurationValue b)
        {
            if (b.LowPartValue == TimeSpan.Zero)
                throw new XQueryException(Properties.Resources.FOAR0001);
            return (decimal)a.LowPartValue.Ticks / (decimal)b.LowPartValue.Ticks;
        }

        public static DayTimeDurationValue operator -(DayTimeDurationValue d)
        {
            return new DayTimeDurationValue(-d.LowPartValue);
        }

        #region IRuntimeExtension Members

        object IRuntimeExtension.OperatorAdd(object arg1, object arg2)
        {
            if (arg1 is DayTimeDurationValue)
            {
                if (arg2 is DayTimeDurationValue)
                    return new DayTimeDurationValue(((DayTimeDurationValue)arg1).LowPartValue + ((DayTimeDurationValue)arg2).LowPartValue);
                if (arg2 is DateTimeValue)
                    return DateTimeValue.Add((DateTimeValue)arg2, (DayTimeDurationValue)arg1);
                if (arg2 is DateValue)
                    return DateValue.Add((DateValue)arg2, (DayTimeDurationValue)arg1);
                if (arg2 is TimeValue)
                    return TimeValue.Add((TimeValue)arg2, (DayTimeDurationValue)arg1);
            }
            throw new Runtime.OperatorMismatchException(Funcs.Add, arg1, arg2);
        }

        object IRuntimeExtension.OperatorSub(object arg1, object arg2)
        {
            if (arg1 is DayTimeDurationValue && arg2 is DayTimeDurationValue)
                return new DayTimeDurationValue(((DayTimeDurationValue)arg1).LowPartValue - ((DayTimeDurationValue)arg2).LowPartValue);
            else
                throw new Runtime.OperatorMismatchException(Funcs.Sub, arg1, arg2);
        }

        object IRuntimeExtension.OperatorMul(object arg1, object arg2)
        {
            if (arg1 is DayTimeDurationValue && TypeConverter.IsNumberType(arg2.GetType()))
                return Multiply((DayTimeDurationValue)arg1, Convert.ToDouble(arg2));
            else if (TypeConverter.IsNumberType(arg1.GetType()) && arg2 is DayTimeDurationValue)
                return Multiply((DayTimeDurationValue)arg2, Convert.ToDouble(arg1));
            else
                throw new Runtime.OperatorMismatchException(Funcs.Mul, arg1, arg2);
        }

        object IRuntimeExtension.OperatorDiv(object arg1, object arg2)
        {
            if (arg1 is DayTimeDurationValue && TypeConverter.IsNumberType(arg2.GetType()))
                return Divide((DayTimeDurationValue)arg1, Convert.ToDouble(arg2));
            else if (arg1 is DayTimeDurationValue && arg2 is DayTimeDurationValue)
                return Divide((DayTimeDurationValue)arg1, (DayTimeDurationValue)arg2);
            else
                throw new Runtime.OperatorMismatchException(Funcs.Div, arg1, arg2);
        }

        #endregion
    }
}
