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
    public class HexBinaryValue: IXmlConvertable
    {
        public HexBinaryValue(byte[] binaryValue)
        {
            if (binaryValue == null)
                throw new NullReferenceException();
            BinaryValue = binaryValue;
        }

        public byte[] BinaryValue { get; private set; }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = new XmlTextWriter(sw);
            tw.WriteBinHex(BinaryValue, 0, BinaryValue.Length);
            tw.Close();
            return sw.ToString();
        }

        public override bool Equals(object obj)
        {
            HexBinaryValue other = obj as HexBinaryValue;
            if (other != null && BinaryValue.Length == other.BinaryValue.Length)
            {
                for (int k = 0; k < BinaryValue.Length; k++)
                    if (BinaryValue[k] != other.BinaryValue[k])
                        return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return BinaryValue.GetHashCode();
        }

        #region IXmlConvertable Members

        object IXmlConvertable.ValueAs(XQuerySequenceType type, XmlNamespaceManager nsmgr)
        {
            switch (type.TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.HexBinary:
                    return this;
                case XmlTypeCode.String:
                    return ToString();
                case XmlTypeCode.UntypedAtomic:
                    return new UntypedAtomic(ToString());
                case XmlTypeCode.Base64Binary:
                    return new Base64BinaryValue(BinaryValue);
                default:
                    throw new InvalidCastException();
            }
        }

        #endregion
    }
}
