//        Copyright (c) 2009-2010, Semyon A. Chertkov (semyonc@gmail.com)
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
using DataEngine.XQuery;
using DataEngine.XQuery.Util;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace DataEngine.XQuery.OpenXML
{
    public class OpenXmlNavigator: XPathNavigator
    {
        private NavigatorAdapter _adapter;
        private object _typedValue;

        internal OpenXmlNavigator(NavigatorAdapter adapter)
        {
            _adapter = adapter;
        }

        private OpenXmlNavigator(NavigatorAdapter adapter, object typedValue)
        {
            _adapter = adapter;
            _typedValue = typedValue;
        }

        public OpenXmlElement Element
        {
            get
            {
                return _adapter.Element;
            }
        }

        public override string BaseURI
        {
            get 
            {
                RootAdapter adapt = _adapter as RootAdapter;
                if (adapt != null)
                    return adapt.Document.BaseUri;
                return GetAttribute("base", XmlReservedNs.NsXml);
            }
        }

        public override XPathNavigator Clone()
        {
            return new OpenXmlNavigator(_adapter.Clone(), _typedValue);
        }

        public override bool IsEmptyElement
        {
            get 
            {
                return _adapter.IsEmptyElement;
            }
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            OpenXmlNavigator nav = other as OpenXmlNavigator;
            if (nav != null)
                return _adapter.Equals(nav._adapter);
            return false;
        }

        public override bool MoveTo(XPathNavigator other)
        {
            OpenXmlNavigator nav = other as OpenXmlNavigator;
            if (nav != null)
            {
                _adapter = nav._adapter.Clone();
                _typedValue = nav._typedValue;
                return true;
            }
            else
                return false;
        }

        public override bool MoveToFirstAttribute()
        {
            NavigatorAdapter adapter = _adapter.MoveToFirstAttribute();
            if (adapter != null)
            {
                _adapter = adapter;
                return true;
            }
            return false;
        }

        public override bool MoveToFirstChild()
        {
            NavigatorAdapter adapter = _adapter.MoveToFirstChild();
            if (adapter != null)
            {
                _adapter = adapter;
                return true;
            }
            return false;
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            NavigatorAdapter adapter = _adapter.MoveToFirstNamespace(namespaceScope);
            if (adapter != null)
            {
                _adapter = adapter;
                return true;
            }
            return false;
        }

        public override bool MoveToParent()
        {
            NavigatorAdapter adapter = _adapter.MoveToParent();
            if (adapter != null)
            {
                _adapter = adapter;
                return true;
            }
            return false;
        }

        public override bool MoveToId(string id)
        {
            return false;
        }

        public override bool MoveToNext()
        {
            return _adapter.MoveToNext();
        }

        public override bool MoveToNextAttribute()
        {
            return _adapter.MoveToNextAttribute();
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            return _adapter.MoveToNextNamespace(namespaceScope);
        }

        public override bool MoveToPrevious()
        {
            return _adapter.MoveToPrevious();
        }

        public override string LocalName
        {
            get
            {
                return _adapter.LocalName;
            }
        }

        public override string Name
        {
            get 
            {
                string prefix = _adapter.Prefix;
                if (prefix == String.Empty)
                    return _adapter.LocalName;
                StringBuilder sb = new StringBuilder();
                sb.Append(prefix);
                sb.Append(':');
                sb.Append(_adapter.LocalName);
                return sb.ToString();
            }
        }

        public override XmlNameTable NameTable
        {
            get 
            {
                return _adapter.NameTable;
            }
        }

        public override string NamespaceURI
        {
            get 
            {
                return _adapter.NamespaceURI;
            }
        }

        public override XPathNodeType NodeType
        {
            get 
            {
                return _adapter.NodeType;
            }
        }

        public override string Prefix
        {
            get 
            {
                return _adapter.Prefix;
            }
        }

        public override string Value
        {
            get 
            {
                return _adapter.Value;
            }
        }

        public override object UnderlyingObject
        {
            get
            {
                return _adapter;
            }
        }

        public override XmlSchemaType XmlType
        {
            get
            {
                XmlSchemaType xmlType = base.XmlType;
                if (xmlType == null)
                    return XQuerySequenceType.XmlSchema.UntypedAtomic;
                return xmlType;
            }
        }

        private object GetNavigatorTypedValue()
        {
            IXmlSchemaInfo schemaInfo = SchemaInfo;
            if (schemaInfo == null || schemaInfo.SchemaType == null)
            {
                switch (NodeType)
                {
                    case XPathNodeType.Comment:
                    case XPathNodeType.ProcessingInstruction:
                    case XPathNodeType.Namespace:
                        return Value;
                    default:
                        return new UntypedAtomic(Value);
                }
            }
            switch (schemaInfo.SchemaType.TypeCode)
            {
                case XmlTypeCode.UntypedAtomic:
                    return new UntypedAtomic(Value);
                case XmlTypeCode.Integer:
                case XmlTypeCode.PositiveInteger:
                case XmlTypeCode.NegativeInteger:
                case XmlTypeCode.NonPositiveInteger:
                    return (Integer)(decimal)base.TypedValue;
                case XmlTypeCode.Date:
                    return DateValue.Parse(Value);
                case XmlTypeCode.DateTime:
                    return Util.DateTimeValue.Parse(Value);
                case XmlTypeCode.Time:
                    return TimeValue.Parse(Value);
                case XmlTypeCode.Duration:
                    return DurationValue.Parse(Value);
                case XmlTypeCode.DayTimeDuration:
                    return new DayTimeDurationValue((TimeSpan)base.TypedValue);
                case XmlTypeCode.YearMonthDuration:
                    return new YearMonthDurationValue((TimeSpan)base.TypedValue);
                case XmlTypeCode.GDay:
                    return GDayValue.Parse(Value);
                case XmlTypeCode.GMonth:
                    return GMonthValue.Parse(Value);
                case XmlTypeCode.GMonthDay:
                    return GMonthDayValue.Parse(Value);
                case XmlTypeCode.GYear:
                    return GYearValue.Parse(Value);
                case XmlTypeCode.GYearMonth:
                    return GYearMonthValue.Parse(Value);
                case XmlTypeCode.QName:
                case XmlTypeCode.Notation:
                    {
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(NameTable);
                        XQueryFuncs.ScanLocalNamespaces(nsmgr, Clone(), true);
                        if (schemaInfo.SchemaType.TypeCode == XmlTypeCode.Notation)
                            return NotationValue.Parse(Value, nsmgr);
                        else
                            return QNameValue.Parse(Value, nsmgr);
                    }
                case XmlTypeCode.AnyUri:
                    return new AnyUriValue(Value);
                case XmlTypeCode.HexBinary:
                    return new Util.HexBinaryValue((byte[])base.TypedValue);
                case XmlTypeCode.Base64Binary:
                    return new Util.Base64BinaryValue((byte[])base.TypedValue);
                case XmlTypeCode.Idref:
                    if (schemaInfo.SchemaType == XQuerySequenceType.XmlSchema.IDREFS)
                        return new IDREFSValue((string[])base.TypedValue);
                    goto default;
                case XmlTypeCode.NmToken:
                    if (schemaInfo.SchemaType == XQuerySequenceType.XmlSchema.NMTOKENS)
                        return new NMTOKENSValue((string[])base.TypedValue);
                    goto default;
                case XmlTypeCode.Entity:
                    if (schemaInfo.SchemaType == XQuerySequenceType.XmlSchema.ENTITIES)
                        return new ENTITIESValue((string[])base.TypedValue);
                    goto default;
                default:
                    return base.TypedValue;
            }
        }

        public override object TypedValue
        {
            get
            {
                if (_typedValue == null)
                    _typedValue = GetNavigatorTypedValue();
                return _typedValue;
            }
        }

        public override Type ValueType
        {
            get
            {
                return XPathFactory.GetNavigatorValueType(this, base.ValueType);
            }
        }
    }
}
