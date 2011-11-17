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
    public class GMonthValue: DateTimeValueBase
    {
        public GMonthValue(DateTimeOffset value)
            : base(false, value)
        {
        }

        public GMonthValue(DateTime value)
            : base(false, value)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IsLocal)
                sb.Append(Value.ToString("--MM"));
            else
                if (Value.Offset == TimeSpan.Zero)
                    sb.Append(Value.ToString("--MM'Z'"));
                else
                    sb.Append(Value.ToString("--MMzzz"));
            return sb.ToString();
        }

        public static GMonthValue Parse(string text)
        {
            DateTimeOffset dateTimeOffset;
            DateTime dateTime;
            text = "2008" + text.Trim();
            if (text.EndsWith("Z"))
            {
                if (!DateTimeOffset.TryParseExact(text.Substring(0, text.Length - 1), "yyyy--MM",
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTimeOffset))
                    throw new XQueryException(Properties.Resources.InvalidFormat, text, "xs:gMonth");
                return new GMonthValue(dateTimeOffset);
            }
            else
            {
                if (DateTime.TryParseExact(text, "yyyy--MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return new GMonthValue(dateTime);
                if (!DateTimeOffset.TryParseExact(text, "yyyy--MMzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset))
                    throw new XQueryException(Properties.Resources.InvalidFormat, text, "xs:gMonth");
                return new GMonthValue(dateTimeOffset);
            }
        }    

    }
}
