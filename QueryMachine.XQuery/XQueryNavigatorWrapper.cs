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

using DataEngine.CoreServices;
using DataEngine.XQuery.Util;

namespace DataEngine.XQuery
{
    public class XQueryNavigatorWrapper : XPathNavigator, IConvertible, IHasXmlNode
    {
        private class UnderlyingObjectDecorate
        {
            private XPathNavigator m_inner;

            public UnderlyingObjectDecorate(XPathNavigator inner)
            {
                m_inner = inner;
            }

            public override bool Equals(object obj)
            {                
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return XPathNavigator.NavigatorComparer.GetHashCode(m_inner);
            }
        }

        internal XPathNavigator m_inner;
        private object m_typedValue;
        private object m_underlyingObjectDecorate;

        public XQueryNavigatorWrapper(XPathNavigator inner)
        {
            m_inner = inner;
        }

        public override object UnderlyingObject
        {
            get
            {
                if (m_underlyingObjectDecorate == null)
                    m_underlyingObjectDecorate = new UnderlyingObjectDecorate(m_inner);
                return m_underlyingObjectDecorate;
            }
        }

        private XPathNavigator Unwrap(XPathNavigator nav)
        {
            XQueryNavigatorWrapper wrapper = nav as XQueryNavigatorWrapper;
            if (wrapper != null)
                return wrapper.m_inner;
            return nav;
        }

        public override string BaseURI
        {
            get 
            {
                return m_inner.BaseURI;
            }
        }

        public override XPathNavigator Clone()
        {
            return new XQueryNavigatorWrapper(m_inner.Clone());
        }

        public override bool IsEmptyElement
        {
            get 
            {
                return m_inner.IsEmptyElement;
            }
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            return m_inner.IsSamePosition(Unwrap(other));
        }

        public override XmlNodeOrder ComparePosition(XPathNavigator nav)
        {
            return m_inner.ComparePosition(Unwrap(nav));
        }

        public override string LocalName
        {
            get 
            {
                return m_inner.LocalName;
            }
        }

        public override bool MoveTo(XPathNavigator other)
        {
            return m_inner.MoveTo(Unwrap(other));
        }

        public override bool MoveToFirstAttribute()
        {
            return m_inner.MoveToFirstAttribute();
        }

        public override bool MoveToChild(string localName, string namespaceURI)
        {
            return m_inner.MoveToChild(localName, namespaceURI);
        }

        public override bool MoveToChild(XPathNodeType type)
        {
            return m_inner.MoveToChild(type);
        }

        public override bool MoveToFirstChild()
        {
            return m_inner.MoveToFirstChild();
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            return m_inner.MoveToFirstNamespace(namespaceScope);
        }

        public override bool MoveToId(string id)
        {
            return m_inner.MoveToId(id);
        }

        public override bool MoveToNext()
        {
            return m_inner.MoveToNext();
        }

        public override bool MoveToNextAttribute()
        {
            return m_inner.MoveToNextAttribute();
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            return m_inner.MoveToNextNamespace(namespaceScope);
        }

        public override bool MoveToParent()
        {
            return m_inner.MoveToParent();
        }

        public override bool MoveToPrevious()
        {
            return m_inner.MoveToPrevious();
        }

        public override bool CheckValidity(XmlSchemaSet schemas, ValidationEventHandler validationEventHandler)
        {
            return m_inner.CheckValidity(schemas, validationEventHandler);
        }
        public override string GetAttribute(string localName, string namespaceURI)
        {
            return m_inner.GetAttribute(localName, namespaceURI);
        }

        public override bool IsDescendant(XPathNavigator nav)
        {
            return m_inner.IsDescendant(Unwrap(nav));
        }

        public override string GetNamespace(string name)
        {
            return m_inner.GetNamespace(name);
        }

        public override IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
        {
            return m_inner.GetNamespacesInScope(scope);
        }

        public override string LookupNamespace(string prefix)
        {
            return m_inner.LookupNamespace(prefix);
        }

        public override bool HasAttributes
        {
            get
            {
                return m_inner.HasAttributes;
            }
        }

        public override bool HasChildren
        {
            get
            {
                return m_inner.HasChildren;
            }
        }

        public override string LookupPrefix(string namespaceURI)
        {
            return m_inner.LookupPrefix(namespaceURI);
        }

        public override bool MoveToAttribute(string localName, string namespaceURI)
        {
            return m_inner.MoveToAttribute(localName, namespaceURI);
        }

        public override bool MoveToFirst()
        {
            return m_inner.MoveToFirst();
        }

        public override string InnerXml
        {
            get
            {
                return m_inner.InnerXml;
            }
            set
            {
                m_inner.InnerXml = value;
            }
        }

        public override bool MoveToFollowing(string localName, string namespaceURI)
        {
            return m_inner.MoveToFollowing(localName, namespaceURI);
        }

        public override bool MoveToFollowing(string localName, string namespaceURI, XPathNavigator end)
        {
            return m_inner.MoveToFollowing(localName, namespaceURI, Unwrap(end));
        }

        public override bool MoveToFollowing(XPathNodeType type)
        {
            return m_inner.MoveToFollowing(type);
        }

        public override bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
        {
            return m_inner.MoveToFollowing(type, Unwrap(end));
        }

        public override bool MoveToNamespace(string name)
        {
            return m_inner.MoveToNamespace(name);
        }

        public override bool MoveToNext(string localName, string namespaceURI)
        {
            return m_inner.MoveToNext(localName, namespaceURI);
        }

        public override bool MoveToNext(XPathNodeType type)
        {
            return m_inner.MoveToNext(type);
        }

        public override void MoveToRoot()
        {
            m_inner.MoveToRoot();
        }

        public override string OuterXml
        {
            get
            {
                return m_inner.OuterXml;
            }
            set
            {
                m_inner.OuterXml = value;
            }
        }

        public override XmlReader ReadSubtree()
        {
            return m_inner.ReadSubtree();
        }

        public override string ToString()
        {
            return m_inner.ToString();
        }

        public override IXmlSchemaInfo SchemaInfo
        {
            get
            {
                return m_inner.SchemaInfo;
            }
        }

        public override string Name
        {
            get 
            {
                return m_inner.Name;
            }
        }

        public override XmlNameTable NameTable
        {
            get 
            {
                return m_inner.NameTable;
            }
        }

        public override string NamespaceURI
        {
            get 
            {
                return m_inner.NamespaceURI;
            }
        }

        public override XPathNodeType NodeType
        {
            get 
            {
                return m_inner.NodeType;
            }
        }

        public override string Prefix
        {
            get 
            {
                return m_inner.Prefix;
            }
        }

        public override string Value
        {
            get 
            {
                return m_inner.Value;
            }
        }

        public override void WriteSubtree(XmlWriter writer)
        {
            m_inner.WriteSubtree(writer);
        }

        public override object ValueAs(Type returnType)
        {
            return m_inner.ValueAs(returnType);
        }

        public override object ValueAs(Type returnType, IXmlNamespaceResolver nsResolver)
        {
            return m_inner.ValueAs(returnType, nsResolver);
        }

        private object GetNavigatorTypedValue(XPathNavigator nav)
        {
            IXmlSchemaInfo schemaInfo = nav.SchemaInfo;
            if (schemaInfo == null || schemaInfo.SchemaType == null)
            {
                switch (nav.NodeType)
                {
                    case XPathNodeType.Comment:
                    case XPathNodeType.ProcessingInstruction:
                    case XPathNodeType.Namespace:
                        return nav.Value;
                    default:
                        return new UntypedAtomic(nav.Value);
                }
            }
            switch (schemaInfo.SchemaType.TypeCode)
            {
                case XmlTypeCode.UntypedAtomic:
                    return new UntypedAtomic(nav.Value);
                case XmlTypeCode.Integer:
                case XmlTypeCode.PositiveInteger:
                case XmlTypeCode.NegativeInteger:
                case XmlTypeCode.NonPositiveInteger:
                    return (Integer)(decimal)base.TypedValue;
                case XmlTypeCode.Date:
                    return DateValue.Parse(nav.Value);
                case XmlTypeCode.DateTime:
                    return DateTimeValue.Parse(nav.Value);
                case XmlTypeCode.Time:
                    return TimeValue.Parse(nav.Value);
                case XmlTypeCode.Duration:
                    return DurationValue.Parse(nav.Value);
                case XmlTypeCode.DayTimeDuration:
                    return new DayTimeDurationValue((TimeSpan)nav.TypedValue);
                case XmlTypeCode.YearMonthDuration:
                    return new YearMonthDurationValue((TimeSpan)nav.TypedValue);
                case XmlTypeCode.GDay:
                    return GDayValue.Parse(nav.Value);
                case XmlTypeCode.GMonth:
                    return GMonthValue.Parse(nav.Value);
                case XmlTypeCode.GMonthDay:
                    return GMonthDayValue.Parse(nav.Value);
                case XmlTypeCode.GYear:
                    return GYearValue.Parse(nav.Value);
                case XmlTypeCode.GYearMonth:
                    return GYearMonthValue.Parse(nav.Value);
                case XmlTypeCode.QName:
                    {
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);
                        XQueryFuncs.ScanLocalNamespaces(nsmgr, nav.Clone(), true);
                        if (schemaInfo.SchemaType.TypeCode == XmlTypeCode.Notation)
                            return NotationValue.Parse(nav.Value, nsmgr);
                        else
                            return QNameValue.Parse(nav.Value, nsmgr);
                    }
                case XmlTypeCode.AnyUri:
                    return new AnyUriValue(nav.Value);
                case XmlTypeCode.HexBinary:
                    return new HexBinaryValue((byte[])nav.TypedValue);
                case XmlTypeCode.Base64Binary:
                    return new Base64BinaryValue((byte[])nav.TypedValue);
                case XmlTypeCode.Idref:
                    if (schemaInfo.SchemaType == XQuerySequenceType.XmlSchema.IDREFS)
                        return new IDREFSValue((string[])nav.TypedValue);
                    goto default;
                case XmlTypeCode.NmToken:
                    if (schemaInfo.SchemaType == XQuerySequenceType.XmlSchema.NMTOKENS)
                        return new NMTOKENSValue((string[])nav.TypedValue);
                    goto default;
                case XmlTypeCode.Entity:
                    if (schemaInfo.SchemaType == XQuerySequenceType.XmlSchema.ENTITIES)
                        return new ENTITIESValue((string[])nav.TypedValue);
                    goto default;
                default:
                    return null;
            }
        }

        public override object TypedValue
        {
            get
            {
                if (m_typedValue == null)
                {
                    m_typedValue = GetNavigatorTypedValue(m_inner);
                    if (m_typedValue == null)
                        m_typedValue = m_inner.TypedValue;
                }
                return m_typedValue;
            }
        }

        public override bool ValueAsBoolean
        {
            get
            {
                return m_inner.ValueAsBoolean;
            }
        }

        public override DateTime ValueAsDateTime
        {
            get
            {
                return m_inner.ValueAsDateTime;
            }
        }

        public override double ValueAsDouble
        {
            get
            {
                return m_inner.ValueAsDouble;
            }
        }

        public override int ValueAsInt
        {
            get
            {
                return m_inner.ValueAsInt;
            }
        }

        public override long ValueAsLong
        {
            get
            {
                return m_inner.ValueAsLong;
            }
        }

        public override Type ValueType
        {
            get
            {
                return XPathFactory.GetNavigatorValueType(m_inner, m_inner.ValueType);
            }
        }

        public override string XmlLang
        {
            get
            {
                return m_inner.XmlLang;
            }
        }

        public override XmlSchemaType XmlType
        {
            get
            {
                XmlSchemaType xmlType = m_inner.XmlType;
                if (xmlType == null)
                    return XQuerySequenceType.XmlSchema.UntypedAtomic;
                return xmlType;
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

        #region IHasXmlNode Members

        public XmlNode GetNode()
        {
            IHasXmlNode hasNode = m_inner as IHasXmlNode;
            if (hasNode != null)
                return hasNode.GetNode();
            return null;
        }

        #endregion
    }
}
