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
using System.Xml.Schema;
using System.Xml.XPath;

namespace DataEngine.XQuery
{
    public enum XmlTypeCardinality
    {
        One,
        ZeroOrOne,
        OneOrMore,
        ZeroOrMore
    }

    public class XQuerySequenceType
    {
        public XmlTypeCode TypeCode { get; set; }

        public XmlQualifiedNameTest NameTest { get; set; }

        public XmlTypeCardinality Cardinality { get; set;  }

        public XmlSchemaType SchemaType { get; set; }

        public XmlSchemaElement SchemaElement { get; set; }

        public XmlSchemaAttribute SchemaAttribute { get; set; }

        public bool Nillable { get; set; }

        public Type OriginalType { get; set; }

        public XQuerySequenceType(XmlTypeCode typeCode)
            : this(typeCode, XmlQualifiedNameTest.Wildcard)
        {
        }

        public XQuerySequenceType(XmlTypeCode typeCode, XmlTypeCardinality cardinality)
            : this(typeCode, XmlQualifiedNameTest.Wildcard)
        {
            Cardinality = cardinality;
        }

        public XQuerySequenceType(XmlTypeCode typeCode, XmlTypeCardinality cardinality, Type clrType)
            : this(typeCode, XmlQualifiedNameTest.Wildcard)
        {
            Cardinality = cardinality;
            OriginalType = clrType;
            if (TypeCode != XmlTypeCode.Item && !IsNode)
                SchemaType = XmlSchemaType.GetBuiltInSimpleType(TypeCode);
        }

        public XQuerySequenceType(XmlTypeCode typeCode, XmlQualifiedNameTest nameTest)
        {
            TypeCode = typeCode;
            Cardinality = XmlTypeCardinality.One;
            NameTest = nameTest;
            if (TypeCode != XmlTypeCode.Item && !IsNode)
                SchemaType = XmlSchemaType.GetBuiltInSimpleType(TypeCode);
        }

        public XQuerySequenceType(XmlTypeCode typeCode, XmlQualifiedNameTest nameTest, XmlSchemaType schemaType)
            : this(typeCode, nameTest, schemaType, false)
        {
        }

        public XQuerySequenceType(XmlTypeCode typeCode, XmlQualifiedNameTest nameTest, XmlSchemaType schemaType, bool isOptional)
        {
            TypeCode = typeCode;
            Cardinality = XmlTypeCardinality.One;
            NameTest = nameTest;
            SchemaType = schemaType;
            Nillable = Nillable;
        }

        public XQuerySequenceType(XmlSchemaElement schemaElement)
        {
            TypeCode = XmlTypeCode.Element;
            SchemaElement = schemaElement;
        }

        public XQuerySequenceType(XmlSchemaAttribute schemaAttribute)
        {
            TypeCode = XmlTypeCode.Attribute;
            SchemaAttribute = schemaAttribute;
        }


        public XQuerySequenceType(XmlSchemaType schemaType, XmlTypeCardinality cardinality, Type clrType)
        {
            TypeCode = schemaType.TypeCode;
            SchemaType = schemaType;
            Cardinality = cardinality;
        }

        public XQuerySequenceType(Type clrType, XmlTypeCardinality cardinality)
        {
            TypeCode = GetXmlTypeCode(clrType);
            if (TypeCode != XmlTypeCode.Item && !IsNode)
                SchemaType = XmlSchemaType.GetBuiltInSimpleType(TypeCode);
            OriginalType = clrType;
            Cardinality = cardinality;
        }

        public XQuerySequenceType(XmlTypeCode typeCode, IXmlSchemaInfo schemaInfo, Type clrType)
        {
            TypeCode = typeCode;
            NameTest = XmlQualifiedNameTest.Wildcard;
            Cardinality = XmlTypeCardinality.One;
            SchemaType = schemaInfo.SchemaType;
            SchemaElement = schemaInfo.SchemaElement;
            SchemaAttribute = schemaInfo.SchemaAttribute;
            OriginalType = clrType;
        }

        public XQuerySequenceType(XQuerySequenceType src)
        {
            TypeCode = src.TypeCode;
            NameTest = src.NameTest;
            Cardinality = src.Cardinality;
            SchemaType = src.SchemaType;
            SchemaElement = src.SchemaElement;
            SchemaAttribute = src.SchemaAttribute;
            Nillable = src.Nillable;
            OriginalType = src.OriginalType;
        }

        public Type ValueType
        {
            get
            {
                if (Cardinality == XmlTypeCardinality.ZeroOrMore ||
                    Cardinality == XmlTypeCardinality.OneOrMore)
                    return typeof(XQueryNodeIterator);
                if (IsNode)
                    return typeof(XPathNavigator);
                return ItemType;
            }
        }

        public Type ItemType
        {
            get
            {
                switch (TypeCode)
                {
                    case XmlTypeCode.Boolean:
                        return typeof(System.Boolean);
                    case XmlTypeCode.Short:
                        return typeof(System.Int16);
                    case XmlTypeCode.Int:
                    case XmlTypeCode.Integer:
                        return typeof(System.Int32);
                    case XmlTypeCode.Long:
                        return typeof(System.Int64);
                    case XmlTypeCode.UnsignedShort:
                        return typeof(System.UInt16);
                    case XmlTypeCode.UnsignedInt:
                        return typeof(System.UInt32);
                    case XmlTypeCode.UnsignedLong:
                        return typeof(System.UInt64);
                    case XmlTypeCode.Byte:
                        return typeof(System.SByte);
                    case XmlTypeCode.UnsignedByte:
                        return typeof(System.Byte);
                    case XmlTypeCode.Float:
                        return typeof(System.Single);
                    case XmlTypeCode.Decimal:
                        return typeof(System.Decimal);
                    case XmlTypeCode.Double:
                        return typeof(System.Double);
                    case XmlTypeCode.DateTime:
                    case XmlTypeCode.Date:
                    case XmlTypeCode.Time:
                        return typeof(System.DateTime);
                    case XmlTypeCode.String:
                    case XmlTypeCode.UntypedAtomic:
                        return typeof(System.String);
                    case XmlTypeCode.Duration:
                    case XmlTypeCode.DayTimeDuration:
                        return typeof(System.TimeSpan);
                    default:
                        return typeof(System.Object);
                }
            }
        }

        public bool IsNode
        {
            get
            {
                switch (TypeCode)
                {
                    case XmlTypeCode.Node:
                    case XmlTypeCode.Element:
                    case XmlTypeCode.Attribute:
                    case XmlTypeCode.Document:
                    case XmlTypeCode.Comment:
                    case XmlTypeCode.Text:
                    case XmlTypeCode.ProcessingInstruction:
                        return true;
                }
                return false;
            }
        }

        public bool IsNumeric
        {
            get
            {
                switch (TypeCode)
                {
                    case XmlTypeCode.Decimal:
                    case XmlTypeCode.Float:
                    case XmlTypeCode.Double:
                    case XmlTypeCode.Integer:
                    case XmlTypeCode.NonPositiveInteger:
                    case XmlTypeCode.NegativeInteger:
                    case XmlTypeCode.Long:
                    case XmlTypeCode.Int:
                    case XmlTypeCode.Short:
                    case XmlTypeCode.Byte:
                    case XmlTypeCode.NonNegativeInteger:
                    case XmlTypeCode.UnsignedLong:
                    case XmlTypeCode.UnsignedInt:
                    case XmlTypeCode.UnsignedShort:
                    case XmlTypeCode.UnsignedByte:
                    case XmlTypeCode.PositiveInteger:
                        return true;
                }
                return false;
            }
        }

        public bool Match(XPathItem item)
        {
            switch (TypeCode)
            {
                case XmlTypeCode.Item:
                    return true;

                case XmlTypeCode.Node:
                    return item.IsNode;

                case XmlTypeCode.AnyAtomicType:
                    return !item.IsNode;

                case XmlTypeCode.UntypedAtomic:
                    return !item.IsNode && item.XmlType == XQueryAtomicValue.UntypedAtomic;

                case XmlTypeCode.Document:
                    {
                        XPathNavigator nav = item as XPathNavigator;
                        if (nav != null)
                        {
                            if (nav.NodeType == XPathNodeType.Root)
                            {
                                XPathNavigator cur = nav.Clone();
                                cur.MoveToFirstChild();
                                if (SchemaElement == null)
                                {
                                    if ((NameTest.IsNamespaceWildcard || NameTest.Namespace == cur.NamespaceURI) &&
                                        (NameTest.IsNameWildcard || NameTest.Name == cur.LocalName))
                                    {
                                        if (SchemaType == null)
                                            return true;
                                        IXmlSchemaInfo schemaInfo = cur.SchemaInfo;
                                        if (schemaInfo != null)
                                        {
                                            if (XmlSchemaType.IsDerivedFrom(schemaInfo.SchemaType, SchemaType, XmlSchemaDerivationMethod.Empty))
                                                return !schemaInfo.IsNil || Nillable;
                                        }
                                    }
                                }
                                else
                                {
                                    IXmlSchemaInfo schemaInfo = cur.SchemaInfo;
                                    if (schemaInfo != null)
                                        return schemaInfo.SchemaElement.QualifiedName == SchemaElement.QualifiedName;
                                }
                            }
                        }
                    }
                    break;

                case XmlTypeCode.Element:
                    {
                        XPathNavigator nav = item as XPathNavigator;
                        if (nav != null && nav.NodeType == XPathNodeType.Element)
                        {
                            if (SchemaElement == null)
                            {
                                if ((NameTest.IsNamespaceWildcard || NameTest.Namespace == nav.NamespaceURI) &&
                                    (NameTest.IsNameWildcard || NameTest.Name == nav.LocalName))
                                {
                                    if (SchemaType == null)
                                        return true;
                                    IXmlSchemaInfo schemaInfo = nav.SchemaInfo;
                                    if (schemaInfo != null)
                                    {
                                        if (XmlSchemaType.IsDerivedFrom(schemaInfo.SchemaType, SchemaType, XmlSchemaDerivationMethod.Empty))
                                            return !schemaInfo.IsNil || Nillable;
                                    }
                                }
                            }
                            else
                            {
                                IXmlSchemaInfo schemaInfo = nav.SchemaInfo;
                                if (schemaInfo != null)
                                    return schemaInfo.SchemaElement.QualifiedName == SchemaElement.QualifiedName;
                            }
                        }
                    }
                    break;

                case XmlTypeCode.Attribute:
                    {
                        XPathNavigator nav = item as XPathNavigator;
                        if (nav != null && nav.NodeType == XPathNodeType.Attribute)
                        {
                            if (SchemaAttribute == null)
                            {
                                if ((NameTest.IsNamespaceWildcard || NameTest.Namespace == nav.NamespaceURI) &&
                                    (NameTest.IsNameWildcard || NameTest.Name == nav.LocalName))
                                {
                                    if (SchemaType == null)
                                        return true;
                                    IXmlSchemaInfo schemaInfo = nav.SchemaInfo;
                                    if (schemaInfo != null)
                                        return XmlSchemaType.IsDerivedFrom(schemaInfo.SchemaType, SchemaType, XmlSchemaDerivationMethod.Empty);
                                }
                            }
                            else
                            {
                                IXmlSchemaInfo schemaInfo = nav.SchemaInfo;
                                if (schemaInfo != null)
                                    return schemaInfo.SchemaAttribute.QualifiedName == SchemaAttribute.QualifiedName;
                            }
                        }
                    }
                    break;

                case XmlTypeCode.ProcessingInstruction:
                    {
                        XPathNavigator nav = item as XPathNavigator;
                        if (nav != null)
                            return nav.NodeType == XPathNodeType.ProcessingInstruction;
                    }
                    break;

                case XmlTypeCode.Comment:
                    {
                        XPathNavigator nav = item as XPathNavigator;
                        if (nav != null)
                            return nav.NodeType == XPathNodeType.Comment;
                    }
                    break;

                case XmlTypeCode.Text:
                    {
                        XPathNavigator nav = item as XPathNavigator;
                        if (nav != null)
                            return nav.NodeType == XPathNodeType.Text ||
                                nav.NodeType == XPathNodeType.SignificantWhitespace;
                    }
                    break;

                default:
                    {
                        if (item.XmlType != null)
                            return XmlSchemaType.IsDerivedFrom(SchemaType, item.XmlType, XmlSchemaDerivationMethod.Empty);
                    }
                    break;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            switch (TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                    sb.Append("AnyAtomicType");
                    break;

                case XmlTypeCode.UntypedAtomic:
                    sb.Append("UntypedAtomic");
                    break;

                case XmlTypeCode.None:
                    sb.Append("void()");
                    break;

                case XmlTypeCode.Item:
                    sb.Append("item()");
                    break;

                case XmlTypeCode.Node:
                    sb.Append("node()");
                    break;

                case XmlTypeCode.Document:
                    sb.Append("document(");
                    if (SchemaElement == null)
                    {
                        if (!NameTest.IsWildcard || SchemaType != null)
                        {
                            sb.Append("element(");
                            sb.Append(NameTest.ToString());
                            if (SchemaType != null)
                            {
                                sb.Append(",");
                                sb.Append(SchemaType.ToString());
                                if (Nillable)
                                    sb.Append("?");
                            }
                            sb.Append(")");
                        }
                    }
                    else
                    {
                        sb.Append("schema-element(");
                        sb.Append(SchemaElement.QualifiedName);
                        sb.Append(")");
                    }
                    sb.Append(")");
                    break;

                case XmlTypeCode.Element:
                    if (SchemaElement == null)
                    {
                        sb.Append("element(");
                        sb.Append(NameTest.ToString());
                        if (SchemaType != null)
                        {
                            sb.Append(",");
                            sb.Append(SchemaType.ToString());
                            if (Nillable)
                                sb.Append("?");
                        }
                    }
                    else
                    {
                        sb.Append("schema-element(");                        
                        sb.Append(SchemaElement.QualifiedName);
                    }
                    sb.Append(")");
                    break;

                case XmlTypeCode.Attribute:
                    if (SchemaAttribute == null)
                    {
                        sb.Append("attribute(");
                        sb.Append(NameTest.ToString());
                        if (SchemaType != null)
                        {
                            sb.Append(",");
                            sb.Append(SchemaType.ToString());
                        }
                    }
                    else
                    {
                        sb.Append("schema-attribute(");
                        sb.Append(SchemaAttribute.QualifiedName);
                    }
                    sb.Append(")");
                    break;

                case XmlTypeCode.ProcessingInstruction:
                    sb.Append("processing-instruction(");
                    break;

                case XmlTypeCode.Comment:
                    sb.Append("comment()");
                    break;

                case XmlTypeCode.Text:
                    sb.Append("text()");
                    break;

                case XmlTypeCode.String:
                    sb.Append("xs:string");
                    break;

                case XmlTypeCode.Boolean:
                    sb.Append("xs:boolean");
                    break;

                case XmlTypeCode.Decimal:
                    sb.Append("xs:decimal");
                    break;

                case XmlTypeCode.Float:
                    sb.Append("xs:float");
                    break;

                case XmlTypeCode.Double:
                    sb.Append("xs:double");
                    break;

                case XmlTypeCode.Duration:
                    sb.Append("xs:Duration");
                    break;

                case XmlTypeCode.DateTime:
                    sb.Append("xs:dateTime");
                    break;

                case XmlTypeCode.Time:
                    sb.Append("xs:time");
                    break;

                case XmlTypeCode.Date:
                    sb.Append("xs:date");
                    break;

                case XmlTypeCode.GYearMonth:
                    sb.Append("xs:gYearMonth");
                    break;

                case XmlTypeCode.GYear:
                    sb.Append("xs:gYear");
                    break;

                case XmlTypeCode.GMonthDay:
                    sb.Append("xs:gMonthDay");
                    break;

                case XmlTypeCode.GDay:
                    sb.Append("xs:gDay");
                    break;

                case XmlTypeCode.GMonth:
                    sb.Append("xs:gMonth");
                    break;

                case XmlTypeCode.HexBinary:
                    sb.Append("xs:hexBinary");
                    break;

                case XmlTypeCode.Base64Binary:
                    sb.Append("xs:base64Binary");
                    break;

                case XmlTypeCode.AnyUri:
                    sb.Append("xs:anyURI");
                    break;

                case XmlTypeCode.QName:
                    sb.Append("xs:QName");
                    break;

                case XmlTypeCode.Notation:
                    sb.Append("xs:NOTATION");
                    break;

                case XmlTypeCode.NormalizedString:
                    sb.Append("xs:normalizedString");
                    break;

                case XmlTypeCode.Token:
                    sb.Append("xs:token");
                    break;

                case XmlTypeCode.Language:
                    sb.Append("xs:language");
                    break;

                case XmlTypeCode.NmToken:
                    sb.Append("xs:NMTOKEN");
                    break;

                case XmlTypeCode.Name:
                    sb.Append("xs:Name");
                    break;

                case XmlTypeCode.NCName:
                    sb.Append("xs:NCName");
                    break;

                case XmlTypeCode.Id:
                    sb.Append("xs:ID");
                    break;

                case XmlTypeCode.Idref:
                    sb.Append("xs:IDREF");
                    break;

                case XmlTypeCode.Entity:
                    sb.Append("xs:ENTITY");
                    break;

                case XmlTypeCode.Integer:
                    sb.Append("xs:integer");
                    break;

                case XmlTypeCode.NonPositiveInteger:
                    sb.Append("xs:nonPositiveInteger");
                    break;

                case XmlTypeCode.NegativeInteger:
                    sb.Append("xs:negativeInteger");
                    break;

                case XmlTypeCode.Long:
                    sb.Append("xs:long");
                    break;

                case XmlTypeCode.Int:
                    sb.Append("xs:int");
                    break;

                case XmlTypeCode.Short:
                    sb.Append("xs:short");
                    break;

                case XmlTypeCode.Byte:
                    sb.Append("xs:byte");
                    break;

                case XmlTypeCode.NonNegativeInteger:
                    sb.Append("xs:nonNegativeInteger");
                    break;

                case XmlTypeCode.UnsignedLong:
                    sb.Append("xs:unsignedLong");
                    break;

                case XmlTypeCode.UnsignedInt:
                    sb.Append("xs:unsignedInt");
                    break;

                case XmlTypeCode.UnsignedShort:
                    sb.Append("xs:unsignedShort");
                    break;

                case XmlTypeCode.UnsignedByte:
                    sb.Append("xs:unsignedByte");
                    break;

                case XmlTypeCode.PositiveInteger:
                    sb.Append("xs:positiveInteger");
                    break;

                default:
                    sb.Append("[]");
                    break;
            }
            switch (Cardinality)
            {
                case XmlTypeCardinality.OneOrMore:
                    sb.Append("+");
                    break;

                case XmlTypeCardinality.ZeroOrMore:
                    sb.Append("*");
                    break;

                case XmlTypeCardinality.ZeroOrOne:
                    sb.Append("?");
                    break;
            }
            return sb.ToString();
        }

        static public XmlTypeCode GetXmlTypeCode(object value)
        {
            if (value is XPathNavigator)
            {
                XPathNavigator nav = (XPathNavigator)value;
                switch (nav.NodeType)
                {
                    case XPathNodeType.Attribute:
                        return XmlTypeCode.Attribute;
                    case XPathNodeType.Comment:
                        return XmlTypeCode.Comment;
                    case XPathNodeType.Element:
                        return XmlTypeCode.Element;
                    case XPathNodeType.Namespace:
                        return XmlTypeCode.Namespace;
                    case XPathNodeType.ProcessingInstruction:
                        return XmlTypeCode.ProcessingInstruction;
                    case XPathNodeType.Root:
                        return XmlTypeCode.Document;
                    case XPathNodeType.SignificantWhitespace:
                    case XPathNodeType.Whitespace:
                    case XPathNodeType.Text:
                        return XmlTypeCode.Text;
                    default:
                        return XmlTypeCode.None;
                }
            }
            return GetXmlTypeCode(value.GetType());
        }

        public static XmlTypeCode GetXmlTypeCode(Type type)
        {
            TypeCode typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case System.TypeCode.Boolean:
                    return XmlTypeCode.Boolean;
                case System.TypeCode.Int16:
                    return XmlTypeCode.Short;
                case System.TypeCode.Int32:
                    return XmlTypeCode.Int;
                case System.TypeCode.Int64:
                    return XmlTypeCode.Long;
                case System.TypeCode.UInt16:
                    return XmlTypeCode.UnsignedShort;
                case System.TypeCode.UInt32:
                    return XmlTypeCode.UnsignedInt;
                case System.TypeCode.UInt64:
                    return XmlTypeCode.UnsignedLong;
                case System.TypeCode.SByte:
                    return XmlTypeCode.Byte;
                case System.TypeCode.Byte:
                    return XmlTypeCode.UnsignedByte;
                case System.TypeCode.Single:
                    return XmlTypeCode.Float;
                case System.TypeCode.Decimal:
                    return XmlTypeCode.Decimal;
                case System.TypeCode.Double:
                    return XmlTypeCode.Double;
                case System.TypeCode.Char:
                case System.TypeCode.String:
                    return XmlTypeCode.String;
                case System.TypeCode.DateTime:
                    return XmlTypeCode.DateTime;
                default:
                    return XmlTypeCode.Item;
            }
        }

        // See XQuery & XPath 2.0 functions & operators section 17.
        public static bool CanConvert(XmlTypeCode src, XmlTypeCode dst)
        {
            // Notation cannot be converted from other than Notation
            if (src == XmlTypeCode.Notation && dst != XmlTypeCode.Notation)
                return false;

            // untypedAtomic and string are convertable unless source type is QName.
            switch (dst)
            {
                case XmlTypeCode.UntypedAtomic:
                case XmlTypeCode.String:
                    return src != XmlTypeCode.QName;
            }

            switch (src)
            {
                case XmlTypeCode.None:
                case XmlTypeCode.Item:
                case XmlTypeCode.Node:
                case XmlTypeCode.Document:
                case XmlTypeCode.Element:
                case XmlTypeCode.Attribute:
                case XmlTypeCode.Namespace:
                case XmlTypeCode.ProcessingInstruction:
                case XmlTypeCode.Comment:
                case XmlTypeCode.Text:
                    return src == dst;

                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.UntypedAtomic:
                case XmlTypeCode.String:
                    switch (dst)
                    {
                        case XmlTypeCode.Boolean:
                        case XmlTypeCode.Short:
                        case XmlTypeCode.Int:
                        case XmlTypeCode.Long:
                        case XmlTypeCode.UnsignedShort:
                        case XmlTypeCode.UnsignedInt:
                        case XmlTypeCode.UnsignedLong:
                        case XmlTypeCode.Byte:
                        case XmlTypeCode.UnsignedByte:
                        case XmlTypeCode.Float:
                        case XmlTypeCode.Decimal:
                        case XmlTypeCode.Double:
                        case XmlTypeCode.String:
                        case XmlTypeCode.DateTime:
                            return true;
                    }
                    return false;

                case XmlTypeCode.Boolean:
                case XmlTypeCode.Decimal:
                    switch (dst)
                    {
                        case XmlTypeCode.Float:
                        case XmlTypeCode.Double:
                        case XmlTypeCode.Decimal:
                        case XmlTypeCode.Boolean:
                            return true;
                    }
                    return false;

                case XmlTypeCode.Float:
                case XmlTypeCode.Double:
                    goto case XmlTypeCode.Decimal;

                case XmlTypeCode.Duration:
                    switch (dst)
                    {
                        case XmlTypeCode.Duration:
                        case XmlTypeCode.YearMonthDuration:
                        case XmlTypeCode.DayTimeDuration:
                            return true;
                    }
                    return false;

                case XmlTypeCode.DateTime:
                    switch (dst)
                    {
                        case XmlTypeCode.DateTime:
                        case XmlTypeCode.Time:
                        case XmlTypeCode.Date:
                        case XmlTypeCode.GYearMonth:
                        case XmlTypeCode.GYear:
                        case XmlTypeCode.GMonthDay:
                        case XmlTypeCode.GDay:
                        case XmlTypeCode.GMonth:
                            return true;
                    }
                    return false;

                case XmlTypeCode.Time:
                    switch (dst)
                    {
                        case XmlTypeCode.Time:
                        case XmlTypeCode.Date:
                            return true;
                    }
                    return false;

                case XmlTypeCode.Date:
                    if (dst == XmlTypeCode.Time)
                        return false;
                    goto case XmlTypeCode.DateTime;

                case XmlTypeCode.GYearMonth:
                case XmlTypeCode.GYear:
                case XmlTypeCode.GMonthDay:
                case XmlTypeCode.GDay:
                case XmlTypeCode.GMonth:
                    return src == dst;

                case XmlTypeCode.HexBinary:
                case XmlTypeCode.Base64Binary:
                    if (src == dst)
                        return true;
                    switch (dst)
                    {
                        case XmlTypeCode.HexBinary:
                        case XmlTypeCode.Base64Binary:
                            return true;
                    }
                    return false;

                case XmlTypeCode.AnyUri:
                case XmlTypeCode.QName:
                case XmlTypeCode.Notation:
                    return src == dst;

                case XmlTypeCode.NormalizedString:
                case XmlTypeCode.Token:
                case XmlTypeCode.Language:
                case XmlTypeCode.NmToken:
                case XmlTypeCode.Name:
                case XmlTypeCode.NCName:
                case XmlTypeCode.Id:
                case XmlTypeCode.Idref:
                case XmlTypeCode.Entity:
                case XmlTypeCode.Integer:
                case XmlTypeCode.NonPositiveInteger:
                case XmlTypeCode.NegativeInteger:
                case XmlTypeCode.Long:
                case XmlTypeCode.Int:
                case XmlTypeCode.Short:
                case XmlTypeCode.Byte:
                case XmlTypeCode.NonNegativeInteger:
                case XmlTypeCode.UnsignedLong:
                case XmlTypeCode.UnsignedInt:
                case XmlTypeCode.UnsignedShort:
                case XmlTypeCode.UnsignedByte:
                case XmlTypeCode.PositiveInteger:
                    throw new NotImplementedException();

                // xdt:*
                case XmlTypeCode.YearMonthDuration:
                    if (dst == XmlTypeCode.DayTimeDuration)
                        return false;
                    goto case XmlTypeCode.Duration;
                case XmlTypeCode.DayTimeDuration:
                    if (dst == XmlTypeCode.YearMonthDuration)
                        return false;
                    goto case XmlTypeCode.Duration;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            XQuerySequenceType dest = obj as XQuerySequenceType;
            if (dest != null)
                return TypeCode == dest.TypeCode &&
                    SchemaElement == dest.SchemaElement &&
                    SchemaAttribute == dest.SchemaAttribute &&
                    SchemaType == dest.SchemaType &&
                    Cardinality == dest.Cardinality &&
                    OriginalType == dest.OriginalType;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static XQuerySequenceType Create(string name)
        {
            if (name == String.Empty ||
                name == "void()")
                return null;
            else
            {
                XmlTypeCardinality cardinality = XmlTypeCardinality.One;
                if (name.EndsWith("?"))
                {
                    cardinality = XmlTypeCardinality.ZeroOrOne;
                    name = name.Substring(1, name.Length - 1);
                }
                else if (name.EndsWith("+"))
                {
                    cardinality = XmlTypeCardinality.OneOrMore;
                    name = name.Substring(1, name.Length - 1);
                }
                else if (name.EndsWith("*"))
                {
                    cardinality = XmlTypeCardinality.ZeroOrMore;
                    name = name.Substring(1, name.Length - 1);
                }
                if (name.Equals("xs:AnyAtomicType"))
                    return new XQuerySequenceType(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.AnyAtomicType),
                        cardinality, null);
                else if (name.Equals("item()"))
                    return new XQuerySequenceType(XmlTypeCode.Item, cardinality);
                else if (name.Equals("node()"))
                    return new XQuerySequenceType(XmlTypeCode.Node, cardinality);
                else
                {
                    string prefix;
                    string localName;
                    QNameParser.Split(name, out prefix, out localName);
                    if (prefix != "xs")
                        throw new ArgumentException("name");
                    XmlSchemaType schemaType = XmlSchemaType.GetBuiltInSimpleType(new XmlQualifiedName(localName, XmlReservedNs.NsXs));
                    if (schemaType == null)
                        throw new ArgumentException("name");
                    return new XQuerySequenceType(schemaType, cardinality, null);
                }
            }
        }

        #region build-in

        internal static XQuerySequenceType Void = new XQuerySequenceType(XmlTypeCode.None);       
        internal static XQuerySequenceType Item = new XQuerySequenceType(XmlTypeCode.Item);
        internal static XQuerySequenceType ItemS = new XQuerySequenceType(XmlTypeCode.Item, XmlTypeCardinality.ZeroOrMore);       
        internal static XQuerySequenceType Node = new XQuerySequenceType(XmlTypeCode.Node);
        internal static XQuerySequenceType ProcessingInstruction = new XQuerySequenceType(XmlTypeCode.ProcessingInstruction);
        internal static XQuerySequenceType Text = new XQuerySequenceType(XmlTypeCode.Text);
        internal static XQuerySequenceType Comment = new XQuerySequenceType(XmlTypeCode.Comment);       
        internal static XQuerySequenceType Element = new XQuerySequenceType(XmlTypeCode.Element);
        internal static XQuerySequenceType Attribute = new XQuerySequenceType(XmlTypeCode.Attribute);
        internal static XQuerySequenceType Document = new XQuerySequenceType(XmlTypeCode.Document);

        internal static XQuerySequenceType Boolean = new XQuerySequenceType(XmlTypeCode.Boolean);

        #endregion

    }
}
