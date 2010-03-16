﻿//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
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

using DataEngine.XQuery.Util;
using DataEngine.XQuery.DocumentModel;

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

        public Type ParameterType { get; set; }

        public Type ItemType { get; private set; }

        public bool IsNode { get; private set; }

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
            ParameterType = clrType;
            IsNode = TypeCodeIsNodeType(TypeCode);
            if (TypeCode != XmlTypeCode.Item && !IsNode)
                SchemaType = XmlSchemaType.GetBuiltInSimpleType(TypeCode);
        }

        public XQuerySequenceType(XmlTypeCode typeCode, XmlQualifiedNameTest nameTest)
        {
            TypeCode = typeCode;
            Cardinality = XmlTypeCardinality.One;
            NameTest = nameTest;
            IsNode = TypeCodeIsNodeType(TypeCode);
            if (TypeCode != XmlTypeCode.Item && !IsNode)
                SchemaType = XmlSchemaType.GetBuiltInSimpleType(TypeCode);
            ItemType = TypeCodeToItemType(TypeCode, SchemaType);
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
            IsNode = TypeCodeIsNodeType(TypeCode);
            ItemType = TypeCodeToItemType(TypeCode, SchemaType);
        }

        public XQuerySequenceType(XmlSchemaElement schemaElement)
        {
            TypeCode = XmlTypeCode.Element;
            SchemaElement = schemaElement;
            IsNode = true;
            ItemType = TypeCodeToItemType(TypeCode, SchemaType);
        }

        public XQuerySequenceType(XmlSchemaAttribute schemaAttribute)
        {
            TypeCode = XmlTypeCode.Attribute;
            SchemaAttribute = schemaAttribute;
            IsNode = true;
            ItemType = TypeCodeToItemType(TypeCode, SchemaType);
        }


        public XQuerySequenceType(XmlSchemaType schemaType, XmlTypeCardinality cardinality, Type clrType)
        {
            TypeCode = schemaType.TypeCode;
            SchemaType = schemaType;
            Cardinality = cardinality;
            ParameterType = clrType;
            IsNode = TypeCodeIsNodeType(TypeCode);
            ItemType = TypeCodeToItemType(TypeCode, SchemaType);
        }

        public XQuerySequenceType(Type clrType, XmlTypeCardinality cardinality)
        {
            TypeCode = GetXmlTypeCode(clrType);
            if (TypeCode != XmlTypeCode.Item && !IsNode)
                SchemaType = XmlSchemaType.GetBuiltInSimpleType(TypeCode);
            ParameterType = clrType;
            Cardinality = cardinality;
            IsNode = TypeCodeIsNodeType(TypeCode);
            ItemType = TypeCodeToItemType(TypeCode, SchemaType);
        }

        public XQuerySequenceType(XmlTypeCode typeCode, IXmlSchemaInfo schemaInfo, Type clrType)
        {
            TypeCode = typeCode;
            NameTest = XmlQualifiedNameTest.Wildcard;
            Cardinality = XmlTypeCardinality.One;
            SchemaType = schemaInfo.SchemaType;
            SchemaElement = schemaInfo.SchemaElement;
            SchemaAttribute = schemaInfo.SchemaAttribute;
            ParameterType = clrType;
            IsNode = TypeCodeIsNodeType(TypeCode);
            ItemType = TypeCodeToItemType(TypeCode, SchemaType);
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
            ParameterType = src.ParameterType;
            IsNode = src.IsNode;
            ItemType = src.ItemType;
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
                if (Cardinality == XmlTypeCardinality.One)
                    return ItemType;
                return typeof(System.Object);
            }
        }

        public Type AtomizedValueType
        {
            get
            {
                if (IsNode)
                {
                    switch (TypeCode)
                    {
                        case XmlTypeCode.Text:
                        case XmlTypeCode.ProcessingInstruction:
                        case XmlTypeCode.Comment:
                        case XmlTypeCode.UntypedAtomic:
                            return typeof(UntypedAtomic);

                        default:
                            if (SchemaType != null)
                                return SchemaType.Datatype.ValueType;
                            else if (SchemaElement != null)
                            {
                                if (SchemaElement.ElementSchemaType != null &&
                                    SchemaElement.ElementSchemaType.Datatype != null)
                                    return SchemaElement.ElementSchemaType.Datatype.ValueType;
                            }
                            else if (SchemaAttribute != null)
                            {
                                if (SchemaAttribute.AttributeSchemaType != null &&
                                    SchemaAttribute.AttributeSchemaType.Datatype != null)
                                    return SchemaAttribute.AttributeSchemaType.Datatype.ValueType;
                            }
                            return typeof(UntypedAtomic);
                    }
                }
                else
                    return ItemType;
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

        public bool IsUntypedAtomic
        {
            get
            {
                return TypeCode == XmlTypeCode.UntypedAtomic;
            }
        }

        private bool MatchName(XPathNavigator nav, XQueryContext context)
        {
            return (NameTest.IsNamespaceWildcard || context.StringEquals(NameTest.Namespace, nav.NamespaceURI)) &&
               (NameTest.IsNameWildcard || context.StringEquals(NameTest.Name, nav.LocalName));
        }

        private bool MatchName(DmNode node)
        {
            return (NameTest.IsNamespaceWildcard || NameTest.Namespace == node.NamespaceURI) &&
               (NameTest.IsNameWildcard || NameTest.Name == node.LocalName);
        }

        public bool Match(XPathItem item, XQueryContext context)
        {
            switch (TypeCode)
            {
                case XmlTypeCode.None:
                    return false;

                case XmlTypeCode.Item:
                    return true;

                case XmlTypeCode.Node:
                    return item.IsNode;

                case XmlTypeCode.AnyAtomicType:
                    return !item.IsNode;

                case XmlTypeCode.UntypedAtomic:
                    return !item.IsNode && item.XmlType == XmlSchema.UntypedAtomic;

                case XmlTypeCode.Document:
                    {
                        XPathNavigator nav = item as XPathNavigator;
                        if (nav != null)
                        {
                            if (nav.NodeType == XPathNodeType.Root)
                            {
                                XPathNavigator cur = nav.Clone();                                
                                if (SchemaElement == null)
                                {
                                    if (cur.MoveToChild(XPathNodeType.Element) && MatchName(cur, context))
                                    {
                                        if (SchemaType == null || SchemaType == XmlSchema.UntypedAtomic)
                                            return true;
                                        IXmlSchemaInfo schemaInfo = cur.SchemaInfo;
                                        if (schemaInfo != null)
                                        {
                                            if (XmlSchemaType.IsDerivedFrom(schemaInfo.SchemaType, SchemaType, XmlSchemaDerivationMethod.Empty))
                                                return !schemaInfo.IsNil || Nillable;
                                        }
                                        else
                                            return XmlSchemaType.IsDerivedFrom(XmlSchema.UntypedAtomic, SchemaType, XmlSchemaDerivationMethod.Empty);
                                    }
                                }
                                else
                                {
                                    if (!cur.MoveToChild(XPathNodeType.Element))
                                        return false;
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
                                if (MatchName(nav, context))
                                {
                                    if (SchemaType == null || SchemaType == XmlSchema.UntypedAtomic)
                                        return true;
                                    IXmlSchemaInfo schemaInfo = nav.SchemaInfo;
                                    if (schemaInfo != null)
                                    {
                                        if (XmlSchemaType.IsDerivedFrom(schemaInfo.SchemaType, SchemaType, XmlSchemaDerivationMethod.Empty))
                                            return !schemaInfo.IsNil || Nillable;
                                    }
                                    else
                                        return XmlSchemaType.IsDerivedFrom(XmlSchema.UntypedAtomic, SchemaType, XmlSchemaDerivationMethod.Empty);
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
                                if (MatchName(nav, context))
                                {
                                    if (SchemaType == null || SchemaType == XmlSchema.UntypedAtomic) 
                                        return true;
                                    IXmlSchemaInfo schemaInfo = nav.SchemaInfo;
                                    if (schemaInfo == null)
                                        return XmlSchemaType.IsDerivedFrom(XmlSchema.UntypedAtomic, SchemaType, XmlSchemaDerivationMethod.Empty);
                                    else
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
                            return (nav.NodeType == XPathNodeType.ProcessingInstruction &&
                                (NameTest.IsNameWildcard || NameTest.Name == nav.Name));
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

                case XmlTypeCode.PositiveInteger:
                    switch (item.XmlType.TypeCode)
                    {
                        case XmlTypeCode.Byte:
                        case XmlTypeCode.Short:
                        case XmlTypeCode.Int:
                        case XmlTypeCode.Long:
                        case XmlTypeCode.Integer:
                            return (decimal)item.ValueAs(typeof(System.Decimal)) > 0;
                    }
                    break;

                case XmlTypeCode.NegativeInteger:
                    switch (item.XmlType.TypeCode)
                    {
                        case XmlTypeCode.Byte:
                        case XmlTypeCode.Short:
                        case XmlTypeCode.Int:
                        case XmlTypeCode.Long:
                        case XmlTypeCode.Integer:
                            return (decimal)item.ValueAs(typeof(System.Decimal)) < 0;
                    }
                    break;

                case XmlTypeCode.NonPositiveInteger:
                    switch (item.XmlType.TypeCode)
                    {
                        case XmlTypeCode.Byte:
                        case XmlTypeCode.Short:
                        case XmlTypeCode.Int:
                        case XmlTypeCode.Long:
                        case XmlTypeCode.Integer:
                            return (decimal)item.ValueAs(typeof(System.Decimal)) <= 0;
                    }
                    break;

                case XmlTypeCode.NonNegativeInteger:
                    switch (item.XmlType.TypeCode)
                    {
                        case XmlTypeCode.Byte:
                        case XmlTypeCode.Short:
                        case XmlTypeCode.Int:
                        case XmlTypeCode.Long:
                        case XmlTypeCode.Integer:
                            return (decimal)item.ValueAs(typeof(System.Decimal)) >= 0;

                        case XmlTypeCode.UnsignedByte:
                        case XmlTypeCode.UnsignedShort:
                        case XmlTypeCode.UnsignedInt:
                        case XmlTypeCode.UnsignedLong:
                            return true;
                    }
                    break;

                case XmlTypeCode.Integer:
                    switch (item.XmlType.TypeCode)
                    {
                        case XmlTypeCode.Byte:
                        case XmlTypeCode.Short:
                        case XmlTypeCode.Int:
                        case XmlTypeCode.Long:
                        case XmlTypeCode.Integer:
                        case XmlTypeCode.UnsignedByte:
                        case XmlTypeCode.UnsignedShort:
                        case XmlTypeCode.UnsignedInt:
                        case XmlTypeCode.UnsignedLong:
                            return true;

                        case XmlTypeCode.Decimal:
                            decimal value = (decimal)item.ValueAs(typeof(System.Decimal));
                            return value == Math.Truncate(value);
                    }
                    break;

                case XmlTypeCode.Entity:
                    return (item.XmlType.TypeCode == XmlTypeCode.String) ||
                           (item.XmlType.TypeCode == XmlTypeCode.Entity);

                default:
                    {
                        if (item.XmlType != null)
                            return XmlSchemaType.IsDerivedFrom(item.XmlType, SchemaType, XmlSchemaDerivationMethod.Empty);
                    }
                    break;
            }
            return false;
        }

        internal bool Match(DmNode node)
        {
            switch (TypeCode)
            {
                case XmlTypeCode.Item:
                case XmlTypeCode.Node:
                    return true;

                case XmlTypeCode.Document:
                    if (node.NodeType == XPathNodeType.Root)
                    {
                        DmElement documentElement = ((DmRoot)node).DocumentElement;
                        if (documentElement != null)
                        {
                            if (SchemaElement == null)
                            {
                                if (MatchName(documentElement))
                                {
                                    if (SchemaType == null || SchemaType == XmlSchema.UntypedAtomic)
                                        return true;
                                    IXmlSchemaInfo schemaInfo = documentElement.SchemaInfo;
                                    if (schemaInfo != null)
                                    {
                                        if (XmlSchemaType.IsDerivedFrom(schemaInfo.SchemaType, SchemaType, XmlSchemaDerivationMethod.Empty))
                                            return !schemaInfo.IsNil || Nillable;
                                    }
                                    else
                                        return XmlSchemaType.IsDerivedFrom(XmlSchema.UntypedAtomic, SchemaType, XmlSchemaDerivationMethod.Empty);
                                }
                            }
                            else
                            {
                                IXmlSchemaInfo schemaInfo = documentElement.SchemaInfo;
                                if (schemaInfo != null)
                                    return schemaInfo.SchemaElement.QualifiedName == SchemaElement.QualifiedName;
                            }
                        }
                    }
                    break;

                case XmlTypeCode.Element:
                    {
                        if (node.NodeType == XPathNodeType.Element)
                        {
                            if (SchemaElement == null)
                            {
                                if (MatchName(node))
                                {
                                    if (SchemaType == null || SchemaType == XmlSchema.UntypedAtomic)
                                        return true;
                                    IXmlSchemaInfo schemaInfo = node.SchemaInfo;
                                    if (schemaInfo != null)
                                    {
                                        if (XmlSchemaType.IsDerivedFrom(schemaInfo.SchemaType, SchemaType, XmlSchemaDerivationMethod.Empty))
                                            return !schemaInfo.IsNil || Nillable;
                                    }
                                    else
                                        return XmlSchemaType.IsDerivedFrom(XmlSchema.UntypedAtomic, SchemaType, XmlSchemaDerivationMethod.Empty);
                                }
                            }
                            else
                            {
                                IXmlSchemaInfo schemaInfo = node.SchemaInfo;
                                if (schemaInfo != null)
                                    return schemaInfo.SchemaElement.QualifiedName == SchemaElement.QualifiedName;
                            }
                        }
                    }
                    break;

                case XmlTypeCode.Attribute:
                    {
                        if (node.NodeType == XPathNodeType.Attribute)
                        {
                            if (SchemaAttribute == null)
                            {
                                if (MatchName(node))
                                {
                                    if (SchemaType == null || SchemaType == XmlSchema.UntypedAtomic)
                                        return true;
                                    IXmlSchemaInfo schemaInfo = node.SchemaInfo;
                                    if (schemaInfo == null)
                                        return XmlSchemaType.IsDerivedFrom(XmlSchema.UntypedAtomic, SchemaType, XmlSchemaDerivationMethod.Empty);
                                    else
                                        return XmlSchemaType.IsDerivedFrom(schemaInfo.SchemaType, SchemaType, XmlSchemaDerivationMethod.Empty);
                                }
                            }
                            else
                            {
                                IXmlSchemaInfo schemaInfo = node.SchemaInfo;
                                if (schemaInfo != null)
                                    return schemaInfo.SchemaAttribute.QualifiedName == SchemaAttribute.QualifiedName;
                            }
                        }
                    }
                    break;

                case XmlTypeCode.ProcessingInstruction:
                    return (node.NodeType == XPathNodeType.ProcessingInstruction &&
                         (NameTest.IsNameWildcard || NameTest.Name == node.Name));

                case XmlTypeCode.Comment:
                    return node.NodeType == XPathNodeType.Comment;

                case XmlTypeCode.Text:
                    return node.NodeType == XPathNodeType.Text ||
                       node.NodeType == XPathNodeType.SignificantWhitespace;
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
                    sb.Append("empty-sequence()");
                    break;

                case XmlTypeCode.Item:
                    sb.Append("item()");
                    break;

                case XmlTypeCode.Node:
                    sb.Append("node()");
                    break;

                case XmlTypeCode.Document:
                    sb.Append("document-node(");
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
                            else
                                sb.Append(", xs:untyped");
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
                        else
                            sb.Append(", xs:untyped");
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
                    if (SchemaType == XmlSchema.NMTOKENS)
                        sb.Append("xs:NMTOKENS");
                    else
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
                    if (SchemaType == XmlSchema.IDREFS)
                        sb.Append("xs:IDREFS");
                    else
                        sb.Append("xs:IDREF");
                    break;

                case XmlTypeCode.Entity:
                    if (SchemaType == XmlSchema.ENTITIES)
                        sb.Append("xs:ENTITYS");
                    else
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

                case XmlTypeCode.DayTimeDuration:
                    sb.Append("xs:dayTimeDuration");
                    break;

                case XmlTypeCode.YearMonthDuration:
                    sb.Append("xs:yearMonthDuration");
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
            if (value is XPathItem)
            {
                XPathItem item = (XPathItem)value;
                if (item.XmlType == null)
                    return XmlTypeCode.UntypedAtomic;
                return item.XmlType.TypeCode;
            }
            return GetXmlTypeCode(value.GetType());
        }

        public static bool TypeCodeIsNodeType(XmlTypeCode typeCode)
        {
            switch (typeCode)
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

        public static Type TypeCodeToItemType(XmlTypeCode typeCode, XmlSchemaType schemaType)
        {
            switch (typeCode)
            {
                case XmlTypeCode.Boolean:
                    return typeof(System.Boolean);
                case XmlTypeCode.Short:
                    return typeof(System.Int16);
                case XmlTypeCode.Int:
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
                case XmlTypeCode.Integer:
                case XmlTypeCode.PositiveInteger:
                case XmlTypeCode.NegativeInteger:
                case XmlTypeCode.NonPositiveInteger:
                case XmlTypeCode.NonNegativeInteger:
                    return typeof(CoreServices.Integer);
                case XmlTypeCode.Double:
                    return typeof(System.Double);
                case XmlTypeCode.DateTime:
                    return typeof(DateTimeValue);
                case XmlTypeCode.Date:
                    return typeof(DateValue);
                case XmlTypeCode.Time:
                    return typeof(TimeValue);
                case XmlTypeCode.AnyUri:
                    return typeof(AnyUriValue);
                case XmlTypeCode.String:
                case XmlTypeCode.NormalizedString:
                case XmlTypeCode.Token:
                case XmlTypeCode.Language:
                case XmlTypeCode.Name:
                case XmlTypeCode.NCName:
                case XmlTypeCode.Id:
                case XmlTypeCode.Idref:
                    if (schemaType == XmlSchema.IDREFS)
                        return typeof(IDREFSValue);
                    else
                        return typeof(String);
                case XmlTypeCode.NmToken:
                    if (schemaType == XmlSchema.NMTOKENS)
                        return typeof(NMTOKENSValue);
                    else
                        return typeof(String);
                case XmlTypeCode.Entity:
                    if (schemaType == XmlSchema.ENTITIES)
                        return typeof(ENTITIESValue);
                    else
                        return typeof(System.String);
                case XmlTypeCode.UntypedAtomic:
                    return typeof(UntypedAtomic);
                case XmlTypeCode.Duration:
                    return typeof(DurationValue);
                case XmlTypeCode.DayTimeDuration:
                    return typeof(DayTimeDurationValue);
                case XmlTypeCode.YearMonthDuration:
                    return typeof(YearMonthDurationValue);
                case XmlTypeCode.GYearMonth:
                    return typeof(GYearMonthValue);
                case XmlTypeCode.GYear:
                    return typeof(GYearValue);
                case XmlTypeCode.GMonth:
                    return typeof(GMonthValue);
                case XmlTypeCode.GMonthDay:
                    return typeof(GMonthDayValue);
                case XmlTypeCode.GDay:
                    return typeof(GDayValue);
                case XmlTypeCode.QName:
                    return typeof(QNameValue);
                case XmlTypeCode.HexBinary:
                    return typeof(HexBinaryValue);
                case XmlTypeCode.Base64Binary:
                    return typeof(Base64BinaryValue);
                default:
                    return typeof(System.Object);
            }
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
                default:
                    if (type == typeof(XPathNavigator))
                        return XmlTypeCode.Node;
                    if (type == typeof(UntypedAtomic))
                        return XmlTypeCode.UntypedAtomic;
                    if (type == typeof(CoreServices.Integer))
                        return XmlTypeCode.Integer;
                    if (type == typeof(DateTimeValue))
                        return XmlTypeCode.DateTime;
                    if (type == typeof(DateValue))
                        return XmlTypeCode.Date;
                    if (type == typeof(TimeValue))
                        return XmlTypeCode.Time;
                    if (type == typeof(DurationValue))
                        return XmlTypeCode.Duration;
                    if (type == typeof(YearMonthDurationValue))
                        return XmlTypeCode.YearMonthDuration;
                    if (type == typeof(DayTimeDurationValue))
                        return XmlTypeCode.DayTimeDuration;
                    if (type == typeof(GYearMonthValue))
                        return XmlTypeCode.GYearMonth;
                    if (type == typeof(GYearValue))
                        return XmlTypeCode.GYear;
                    if (type == typeof(GDayValue))
                        return XmlTypeCode.GDay;
                    if (type == typeof(GMonthValue))
                        return XmlTypeCode.GMonth;
                    if (type == typeof(GMonthDayValue))
                        return XmlTypeCode.GMonthDay;
                    if (type == typeof(QNameValue))
                        return XmlTypeCode.QName;
                    if (type == typeof(AnyUriValue))
                        return XmlTypeCode.AnyUri;
                    if (type == typeof(HexBinaryValue))
                        return XmlTypeCode.HexBinary;
                    if (type == typeof(Base64BinaryValue))
                        return XmlTypeCode.Base64Binary;
                    if (type == typeof(IDREFSValue))
                        return XmlTypeCode.Idref;
                    if (type == typeof(ENTITIESValue))
                        return XmlTypeCode.Entity;
                    if (type == typeof(NMTOKENSValue))
                        return XmlTypeCode.NmToken;
                    return XmlTypeCode.Item;
            }
        }

        public bool IsDerivedFrom(XQuerySequenceType src)
        {
            switch (src.TypeCode)
            {
                case XmlTypeCode.Node:
                    if (!IsNode)
                        return false;
                    break;

                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.UntypedAtomic:
                    if (IsNode)
                        return false;
                    break;

                case XmlTypeCode.Document:
                    if (TypeCode != XmlTypeCode.Document ||
                        SchemaElement != src.SchemaElement)
                        return false;
                    break;

                case XmlTypeCode.Element:
                    if (TypeCode != XmlTypeCode.Element ||
                        SchemaElement != src.SchemaElement)
                        return false;
                    break;

                case XmlTypeCode.Attribute:
                    if (TypeCode != XmlTypeCode.Attribute ||
                        SchemaAttribute != src.SchemaAttribute)
                        return false;
                    break;

                case XmlTypeCode.ProcessingInstruction:
                    if (TypeCode != XmlTypeCode.ProcessingInstruction)
                        return false;
                    break;

                case XmlTypeCode.Comment:
                    if (TypeCode != XmlTypeCode.Comment)
                        return false;
                    break;

                case XmlTypeCode.Text:
                    if (TypeCode != XmlTypeCode.Text)
                        return false;
                    break;
            }
            if (SchemaType != null || src.SchemaType != null)
            {
                if (SchemaType != null && src.SchemaType != null)
                {
                    if (!XmlSchemaType.IsDerivedFrom(SchemaType, src.SchemaType, XmlSchemaDerivationMethod.Empty))
                        return false;
                }
                else
                    return false;
            }

            if (Cardinality != src.Cardinality)
            {
                if ((Cardinality == XmlTypeCardinality.ZeroOrOne ||
                     Cardinality == XmlTypeCardinality.ZeroOrMore) &&
                     (src.Cardinality == XmlTypeCardinality.One ||
                      src.Cardinality == XmlTypeCardinality.OneOrMore))
                    return false;
                if (Cardinality == XmlTypeCardinality.One &&
                    src.Cardinality == XmlTypeCardinality.OneOrMore)
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            XQuerySequenceType dest = obj as XQuerySequenceType;
            if (!Object.ReferenceEquals(obj, null))
                return TypeCode == dest.TypeCode &&
                    SchemaElement == dest.SchemaElement &&
                    SchemaAttribute == dest.SchemaAttribute &&
                    SchemaType == dest.SchemaType &&
                    Cardinality == dest.Cardinality;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static XQuerySequenceType Create(string name)
        {
            if (name == String.Empty ||
                name == "empty-sequence()")
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

        public static class XmlSchema
        {
            public static XmlSchemaType AnySimpleType = XmlSchemaType.GetBuiltInSimpleType(new XmlQualifiedName("anySimpleType", XmlReservedNs.NsXs));
            public static XmlSchemaType AnyType = XmlSchemaType.GetBuiltInComplexType(new XmlQualifiedName("anyType", XmlReservedNs.NsXs));
            public static XmlSchemaType AnyAtomicType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.AnyAtomicType);
            public static XmlSchemaType UntypedAtomic = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UntypedAtomic);
            public static XmlSchemaType Integer = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Integer);
            public static XmlSchemaType DateTime = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime);
            public static XmlSchemaType Date = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Date);
            public static XmlSchemaType Time = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Time);
            public static XmlSchemaType Duration = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Duration);
            public static XmlSchemaType YearMonthDuration = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.YearMonthDuration);
            public static XmlSchemaType DayTimeDuration = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DayTimeDuration);
            public static XmlSchemaType GYearMonth = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.GYearMonth);
            public static XmlSchemaType GYear = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.GYear);
            public static XmlSchemaType GDay = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.GDay);
            public static XmlSchemaType GMonth = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.GMonth);
            public static XmlSchemaType GMonthDay = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.GMonthDay);
            public static XmlSchemaType QName = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.QName);
            public static XmlSchemaType HexBinary = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.HexBinary);
            public static XmlSchemaType Base64Binary = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Base64Binary);
            public static XmlSchemaType IDREFS = XmlSchemaType.GetBuiltInSimpleType(new XmlQualifiedName("IDREFS", XmlReservedNs.NsXs));
            public static XmlSchemaType NMTOKENS = XmlSchemaType.GetBuiltInSimpleType(new XmlQualifiedName("NMTOKENS", XmlReservedNs.NsXs));
            public static XmlSchemaType ENTITIES = XmlSchemaType.GetBuiltInSimpleType(new XmlQualifiedName("ENTITIES", XmlReservedNs.NsXs));
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
        internal static XQuerySequenceType AnyAtomicType = new XQuerySequenceType(XmlTypeCode.AnyAtomicType);
        internal static XQuerySequenceType AnyAtomicTypeO = new XQuerySequenceType(XmlTypeCode.AnyAtomicType, XmlTypeCardinality.ZeroOrOne);        

        #endregion

    }
}
