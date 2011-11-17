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
using System.IO;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace DataEngine.XQuery.Util
{
    public class AnyUriValue: IXmlConvertable, IComparable
    {
        public AnyUriValue(string value)
        {
            if (value == null)
                throw new InvalidOperationException();
            Value = value;
        }

        public AnyUriValue(Uri uri)
        {
            Value = uri.OriginalString;
        }

        public string Value { get; private set; }

        public override bool Equals(object obj)
        {
            AnyUriValue other = obj as AnyUriValue;
            if (other != null)
                return Value == other.Value;
            if (obj is String)
                return Value == obj.ToString();
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        #region IXmlConvertable Members

        public object ValueAs(XQuerySequenceType type, XmlNamespaceManager nsmgr)
        {
            switch (type.TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.AnyUri:
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

        #region IComparable Members

        public int CompareTo(object obj)
        {
            AnyUriValue other = obj as AnyUriValue;
            if (other == null)
                throw new ArgumentException();
            return String.CompareOrdinal(Value, other.Value);
        }

        #endregion
    }
}
