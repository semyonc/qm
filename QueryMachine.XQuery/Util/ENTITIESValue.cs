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
    public class ENTITIESValue
    {
        public ENTITIESValue(string[] value)
        {
            if (value == null)
                throw new ArgumentException("value");
            ValueList = value;
        }

        public string[] ValueList { get; private set; }

        public override bool Equals(object obj)
        {
            ENTITIESValue other = obj as ENTITIESValue;
            if (other != null && other.ValueList.Length == ValueList.Length)
            {
                for (int k = 0; k < ValueList.Length; k++)
                    if (ValueList[k] != other.ValueList[k])
                        return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int k = 0; k < ValueList.Length; k++)
                hashCode = hashCode << 7 ^ ValueList[k].GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < ValueList.Length; k++)
            {
                if (k > 0)
                    sb.Append(" ");
                sb.Append(ValueList[k]);
            }
            return sb.ToString();
        }
    }
}
