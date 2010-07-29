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
using System.Globalization;

using DataEngine.CoreServices;
using DataEngine.XQuery.Util;
using System.Diagnostics;

namespace DataEngine.XQuery
{
    [DebuggerDisplay("Atom|{Value},{ValueType}")]
    public class XQueryItem: XQueryItemBase, IConvertible
    {       
        private object _value;
        private XmlSchemaType _xmlType;

        public XQueryItem()
        {
            _value = Undefined.Value;
        }

        public XQueryItem(object value)
        {
            RawValue = value;
        }

        public XQueryItem(object value, XmlSchemaType xmlType)
        {
            RawValue = value;
            if (xmlType == null)
                xmlType = XQuerySequenceType.XmlSchema.UntypedAtomic;
            _xmlType = xmlType;
        }

        private void InferXmlType()
        {
            if (_value is String)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String);
            else if (_value is UntypedAtomic)
                _xmlType = XQuerySequenceType.XmlSchema.UntypedAtomic;
            else if (_value is Integer)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Integer);
            else if (_value is Boolean)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean);
            else if (_value is DateTimeValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime);
            else if (_value is DateValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Date);
            else if (_value is TimeValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Time);
            else if (_value is Double)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double);
            else if (_value is Int32)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Int);
            else if (_value is Int64)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Long);
            else if (_value is Single)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Float);
            else if (_value is Decimal)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Decimal);
            else if (_value is Int16)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Short);
            else if (_value is UInt16)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedShort);
            else if (_value is UInt32)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt);
            else if (_value is UInt64)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedLong);
            else if (_value is SByte)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Byte);
            else if (_value is Byte)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedByte);
            else if (_value is DayTimeDurationValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DayTimeDuration);
            else if (_value is YearMonthDurationValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.YearMonthDuration);
            else if (_value is DurationValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Duration);
            else if (_value is GYearMonthValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.GYearMonth);
            else if (_value is GYearValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.GYear);
            else if (_value is GDayValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.GDay);
            else if (_value is GMonthValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.GMonth);
            else if (_value is GMonthDayValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.GMonthDay);
            else if (_value is QNameValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.QName);
            else if (_value is AnyUriValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.AnyUri);
            else if (_value is HexBinaryValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.HexBinary);
            else if (_value is Base64BinaryValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Base64Binary);
            else if (_value is NMTOKENSValue)
                _xmlType = XQuerySequenceType.XmlSchema.NMTOKENS;
            else if (_value is IDREFSValue)
                _xmlType = XQuerySequenceType.XmlSchema.IDREFS;
            else if (_value is ENTITIESValue)
                _xmlType = XQuerySequenceType.XmlSchema.ENTITIES;
            else if (_value is NotationValue)
                _xmlType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Notation);
            else
                throw new ArgumentException("value");
        }

        public override XPathItem Clone()
        {
            XQueryItem clone = new XQueryItem();
            clone._value = _value;
            clone._xmlType = _xmlType;
            return clone;
        }

        public override string ToString()
        {
            if (_value != null)
                return Value;
            return String.Empty;
        }

        public object RawValue
        {
            set
            {
                if (value == null)
                    _value = CoreServices.Generation.RuntimeOps.False;
                else if (value is XQueryDocumentBuilder)
                {
                    XQueryDocumentBuilder builder = (XQueryDocumentBuilder)value;
                    _value = builder.m_document.CreateNavigator();
                }
                else
                    _value = value;
                _xmlType = null;
            }
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
                return XQueryConvert.ToString(_value);
            }
        }

        public override object ValueAs(Type returnType, IXmlNamespaceResolver nsResolver)
        {
            return XQueryConvert.ChangeType(_value, returnType);
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
                if (_xmlType == null)
                    InferXmlType();
                return _xmlType;
            }
        }

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
    }
}
