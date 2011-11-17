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
    public class GYearValue: DateTimeValueBase
    {
        public GYearValue(bool sign, DateTimeOffset value)
            : base(sign, value)
        {
        }

        public GYearValue(bool sign, DateTime value)
            : base(sign, value)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (S)
                sb.Append('-');
            if (IsLocal)
                sb.Append(Value.ToString("yyyy"));
            else
                if (Value.Offset == TimeSpan.Zero)
                    sb.Append(Value.ToString("yyyy'Z'"));
                else
                    sb.Append(Value.ToString("yyyyzzz"));
            return sb.ToString();
        }

        private static string[] GYearFormats = new string[] {             
            "yyyy",              
            "'-'yyyy"
        };

        private static string[] GYearOffsetFormats = new string[] {             
            "yyyyzzz", 
            "'-'yyyyzzz"
        };

        public static GYearValue Parse(string text)
        {
            DateTimeOffset dateTimeOffset;
            DateTime dateTime;
            text = text.Trim();
            bool s = text.StartsWith("-");
            if (text.EndsWith("Z"))
            {
                if (!DateTimeOffset.TryParseExact(text.Substring(0, text.Length - 1), GYearFormats,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTimeOffset))
                    throw new XQueryException(Properties.Resources.InvalidFormat, text, "xs:gYear");
                return new GYearValue(s, dateTimeOffset);
            }
            else
            {
                if (DateTime.TryParseExact(text, GYearFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return new GYearValue(s, dateTime);
                if (!DateTimeOffset.TryParseExact(text, GYearOffsetFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset))
                    throw new XQueryException(Properties.Resources.InvalidFormat, text, "xs:gYear");
                return new GYearValue(s, dateTimeOffset);
            }
        }    

    }
}
