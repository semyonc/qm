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
