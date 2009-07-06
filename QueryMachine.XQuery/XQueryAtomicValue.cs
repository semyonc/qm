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

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

namespace DataEngine.XQuery
{
    public class XQueryAtomicValue: XPathItem, IConvertible
    {       
        private object _value;
        private XmlSchemaType _xmlType;

        public XQueryAtomicValue(object value)
        {
            _value = value;
            if (value is String)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String);
            else if (value is Boolean)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean);
            else if (value is DateTime)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime);
            else if (value is Double)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double);
            else if (value is Int32)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Integer);
            else if (value is Int64)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Long);
            else if (value is Single)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Float);
            else if (value is Decimal)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Decimal);
            else if (value is Int16)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Short);
            else if (value is UInt16)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedShort);
            else if (value is UInt32)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt);
            else if (value is UInt64)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedLong);
            else if (value is SByte)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Byte);
            else if (value is Byte)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedByte);
        }

        public XQueryAtomicValue(object value, XQuerySequenceType destType)
        {
            _value = value;
            _xmlType = destType.SchemaType;
        }
        
        public XQueryAtomicValue(object value, XmlSchemaType xmlType)
        {
            _value = value;
            if (xmlType == null)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UntypedAtomic);
            else
                _xmlType = xmlType;
        }

        public override bool IsNode
        {
            get 
            { 
                return false; 
            }
        }

        public override object TypedValue
        {
            get 
            {
                return _value;
            }
        }

        public override string Value
        {
            get
            {
                if (_xmlType == null)
                    return _value.ToString();
                else
                    return (string)_xmlType.Datatype.ChangeType(_value, typeof(String));
            }
        }

        public override object ValueAs(Type returnType, IXmlNamespaceResolver nsResolver)
        {
            return _xmlType.Datatype.ChangeType(_value, returnType, nsResolver);
        }

        public override bool ValueAsBoolean
        {
            get 
            {
                return Convert.ToBoolean(_value);
            }
        }

        public override DateTime ValueAsDateTime
        {
            get 
            {
                return Convert.ToDateTime(_value);
            }
        }

        public override double ValueAsDouble
        {
            get 
            {
                return Convert.ToDouble(_value);
            }
        }

        public override int ValueAsInt
        {
            get 
            {
                return Convert.ToInt32(_value);
            }
        }

        public override long ValueAsLong
        {
            get 
            {
                return Convert.ToInt64(_value);
            }
        }

        public override Type ValueType
        {
            get 
            {
                return _value.GetType();
            }
        }

        public override XmlSchemaType XmlType
        {
            get 
            {
                return _xmlType;
            }
        }

        public static XmlSchemaType AnyAtomicType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.AnyAtomicType);
        public static XmlSchemaType UntypedAtomic = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UntypedAtomic);

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            return Type.GetTypeCode(ValueType);
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(TypedValue, provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(TypedValue, provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(TypedValue, provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(TypedValue, provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(TypedValue, provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(TypedValue, provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(TypedValue, provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(TypedValue, provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(TypedValue, provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(TypedValue, provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(TypedValue, provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return Convert.ToString(TypedValue, provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(TypedValue, conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(TypedValue, provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(TypedValue, provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(TypedValue, provider);
        }

        #endregion

        public object[] Annotation { get; set; }
    }
}
