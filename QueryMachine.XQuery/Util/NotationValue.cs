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
using System.Xml;
using System.Xml.Schema;
using System.Globalization;

namespace DataEngine.XQuery.Util
{
    public class NotationValue : IXmlConvertable
    {
        public NotationValue(QNameValue name)
        {
            Prefix = name.Prefix;
            LocalName = name.LocalName;
            NamespaceUri = name.NamespaceUri;
        }

        public String Prefix { get; private set; }
        public String LocalName { get; private set; }
        public String NamespaceUri { get; private set; }

        public override bool Equals(object obj)
        {
            NotationValue other = obj as NotationValue;
            if (other != null)
            {
                if (LocalName == other.LocalName &&
                    NamespaceUri == other.NamespaceUri)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return LocalName.GetHashCode() ^ NamespaceUri.GetHashCode() << 8; 
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Prefix != "")
            {
                sb.Append(Prefix);
                sb.Append(':');
            }
            sb.Append(LocalName);
            return sb.ToString();
        }

        #region IXmlConvertable Members

        object IXmlConvertable.ValueAs(XQuerySequenceType type, XmlNamespaceManager nsmgr)
        {
            switch (type.TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.Notation:
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

        public static NotationValue Parse(string name, XmlNamespaceManager resolver)
        {
            return new NotationValue(QNameValue.Parse(name, resolver));
        }
    }
}
