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
using System.Runtime.InteropServices;
using System.Diagnostics;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using DataEngine.CoreServices;
using DataEngine.XQuery.Util;
using DataEngine.XQuery.DocumentModel;

namespace DataEngine.XQuery
{
    public class XQueryNavigator : XPathNavigator, IConvertible
    {
        private XQueryDocument _doc;
        private PageFile _pf;

        private String _value;
        private object _typedValue;
        
        private int _pos;
        private DmNode _head;
        private int _parent;

        private XQueryNavigator()
        {
        }

        internal XQueryNavigator(XQueryDocument doc)
            : this()
        {
            _doc = doc;
            _pf = doc.pagefile;
            _typedValue = null;
            MoveToRoot();
        }

        public override void MoveToRoot()
        {            
            DmNode node;
            int parent;
            _doc.ExpandPageFile(1);
            if (_pf.Count > 0)
            {
                _pf.Get(0, out node, out parent);
                Read(node.NodeType == XPathNodeType.Attribute ||
                    node.NodeType == XPathNodeType.Namespace 
                        ? parent : 0);
            }
        }

        public XQueryDocument Document
        {
            get
            {
                return _doc;
            }
        }

        public override string BaseURI
        {
            get
            {
                if (NodeType == XPathNodeType.Root)
                    return _doc.baseUri;
                return GetAttribute("base", XmlReservedNs.NsXml);
            }
        }

        [DebuggerStepThrough]
        public override XPathNavigator Clone()
        {
            XQueryNavigator clone = new XQueryNavigator();
            clone._head = _head;
            clone._pos = _pos;
            clone._parent = _parent;
            clone._doc = _doc;
            clone._pf = _pf;
            clone._typedValue = _typedValue;
            clone._value = _value;
            return clone;
        }

        public override bool IsEmptyElement
        {
            get
            {
                _doc.ExpandPageFile(_pos + 1);
                return _pf.Get(_pos) == -1;
            }
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            if (other is XQueryNavigator)
            {
                XQueryNavigator nav = (XQueryNavigator)other;
                return nav._pf == _pf && nav._pos == _pos;
            }
            else
                return false;
        }

        public override XmlNodeOrder ComparePosition(XPathNavigator other)
        {
            XQueryNavigator nav = other as XQueryNavigator;
            if (nav != null && nav._pf == _pf)
            {
                if (_pos < nav._pos)
                    return XmlNodeOrder.Before;
                else if (_pos > nav._pos)
                    return XmlNodeOrder.After;
                else
                    return XmlNodeOrder.Same;
            }
            return XmlNodeOrder.Unknown;
        }

        public override bool MoveTo(XPathNavigator other)
        {
            XQueryNavigator nav = other as XQueryNavigator;
            if (nav != null)
            {
                if (nav._pf != _pf || nav._doc != _doc)
                    throw new InvalidOperationException();
                _pos = nav._pos;
                _head = nav._head;
                _parent = nav._parent;
                _typedValue = null;
                _value = null;
                return true;
            }
            else
                return false;
        }

        public override bool MoveToFirstAttribute()
        {
            if (NodeType == XPathNodeType.Element)
            {
                int p = _pos - 1;
                if (p >= 0)
                {
                    DmNode node;
                    int parent;
                    _pf.Get(p, out node, out parent);
                    if (parent == _pos && node.NodeType == XPathNodeType.Attribute)
                    {
                        Read(p);
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            if (NodeType == XPathNodeType.Element)
            {
                int p = _pos - 1;
                while (p >= 0)
                {
                    DmNode node;
                    int parent;
                    _pf.Get(p, out node, out parent);
                    if (parent != _pos)
                        break;
                    if (node.NodeType == XPathNodeType.Namespace)
                    {
                        Read(p);
                        return true;
                    }
                    p--;
                }
            }
            return false;
        }

        public override bool MoveToNextAttribute()
        {
            if (NodeType == XPathNodeType.Attribute)
            {
                int p = _pos - 1;
                if (p >= 0)
                {
                    DmNode node;
                    int parent;
                    _pf.Get(p, out node, out parent);
                    if (parent == _parent && node.NodeType == XPathNodeType.Attribute)
                    {
                        Read(p);
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            if (NodeType == XPathNodeType.Namespace)
            {
                int p = _pos - 1;
                if (p >= 0)
                {
                    DmNode node;
                    int parent;
                    _pf.Get(p, out node, out parent);
                    if (parent == _parent && node.NodeType == XPathNodeType.Namespace)
                    {
                        Read(p);
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool MoveToFirstChild()
        {
            if (NodeType == XPathNodeType.Root ||
                NodeType == XPathNodeType.Element)
            {
                _doc.ExpandPageFile(_pos + 1);
                int child = _pf.Get(_pos);
                if (child != -1)
                {
                    _doc.ExpandPageFile(child);
                    Read(child);
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToId(string id)
        {
            Document.Fill();
            Dictionary<string, int> idTable = _doc.IdTable;
            if (idTable != null)
            {
                int p;
                if (idTable.TryGetValue(id, out p))
                {
                    Position = p;
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToParent()
        {
            if (_parent == -1)
                return false;
            Read(_parent);
            return true;
        }

        public override bool MoveToNext()
        {
            if (NodeType == XPathNodeType.Attribute ||
                NodeType == XPathNodeType.Namespace)
                return false;
            int p = _pf[_pos];
            if (p == 0)
            {
                _doc.ExpandUtilElementEnd(_pos);
                Read();
                p = _pf[_pos];
            }
            if (p != -1)
            {
                _doc.ExpandPageFile(p);
                Read(p);
                return true;
            }
            return false;
        }

        public override bool MoveToPrevious()
        {
            int p;
            if (_parent == -1 ||
                NodeType == XPathNodeType.Attribute ||
                NodeType == XPathNodeType.Namespace)
                return false;
            p = _pf.Get(_parent);
            if (p != _pos)
            {
                while (_pf[p] != _pos && p != -1)
                    p = _pf[p];
                if (p == -1)
                    throw new InvalidOperationException();
                Read(p);
                return true;
            }
            return false;
        }

        private void Read(int pos)
        {
            _value = null;
            _typedValue = null;
            _pos = pos;
            _pf.Get(_pos, out _head, out _parent);
        }

        private void Read()
        {
            Read(_pos);
        }

        public override string Name
        {
            get
            {
                return _head.Name;
            }
        }

        public override XmlNameTable NameTable
        {
            get
            {
                return _doc.nameTable;
            }
        }

        public override string NamespaceURI
        {
            get
            {
                return _head.NamespaceURI;
            }
        }

        public override XPathNodeType NodeType
        {
            get
            {
                return _head.NodeType;
            }
        }

        public override string LocalName
        {
            get
            {
                return _head.LocalName;
            }
        }

        public override string Prefix
        {
            get
            {
                return _head.Prefix;
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
                    return DateTimeValue.Parse(Value);
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
                    return new HexBinaryValue((byte[])base.TypedValue);
                case XmlTypeCode.Base64Binary:
                    return new Base64BinaryValue((byte[])base.TypedValue);
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

        public override string Value
        {
            get
            {
                if (_value == null)
                {
                    if (NodeType == XPathNodeType.Root ||
                        NodeType == XPathNodeType.Element)
                    {
                        StringBuilder sb = new StringBuilder();
                        XPathNodeIterator iter = SelectDescendants(XPathNodeType.Text, false);
                        while (iter.MoveNext())
                            sb.Append(iter.Current.Value);
                        _value = sb.ToString();
                    }
                    else
                        _value = _pf.GetValue(_pos);
                }
                return _value;
            }
        }

        public override IXmlSchemaInfo SchemaInfo
        {
            get
            {
                IXmlSchemaInfo schemaInfo = _head.SchemaInfo;
                if (schemaInfo == null && _pf.HasSchemaInfo)
                {
                    if (_pf[_pos] == 0)
                    {
                        _doc.ExpandUtilElementEnd(_pos);
                        Read();
                    }
                    schemaInfo = _head.SchemaInfo;
                }
                return schemaInfo;
            }
        }

        #region Indexing properties

        internal bool IsCompleted
        {
            get
            {
                if ((NodeType == XPathNodeType.Element ||
                     NodeType ==  XPathNodeType.Root) && _pf[_pos] == 0)
                   return false;
                return true;
            }
        }

        internal int Position
        {
            get
            {
                return _pos;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException();
                _doc.ExpandPageFile(value);
                if (value >= _pf.Count)
                    throw new ArgumentException();
                Read(value);
            }
        }

        private int GetLength()
        {
            if (NodeType == XPathNodeType.Root)
            {
                _doc.Fill();
                return _pf.Count;
            }
            else
            {
                if (_pf[_pos] == 0)
                {
                    _doc.ExpandUtilElementEnd(_pos);
                    Read();
                }
                int p = _pf[_pos];
                if (p == -1)
                {
                    XQueryNavigator nav = (XQueryNavigator)Clone();
                    if (nav.MoveToParent())
                        return nav.GetLength() - ( _pos - nav.Position );
                    return _pf.Count - _pos;
                }
                return p - _pos;
            }
        }

        internal int GetBegin()
        {
            if (NodeType == XPathNodeType.Element)
            {
                int p = _pos - 1;
                while (p >= 0)
                {
                    DmNode node;
                    int parent;
                    _pf.Get(p, out node, out parent);
                    if (parent != _pos || node.NodeType != XPathNodeType.Attribute)
                        return p + 1;
                    p--;
                }
            }
            return _pos;
        }

        internal int GetEnd()
        {
            int p = _pos + GetLength() - 1;
            while (p > _pos)
            {
                DmNode node;
                int parent;
                _pf.Get(p, out node, out parent);
                if (node.NodeType != XPathNodeType.Attribute && parent <= p)
                    return p - _pos + 1;
                p--;
            }
            return 0;
        }

        internal DmNode DmNode
        {
            get
            {
                return _head;
            }
        }

        #endregion


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

    public class XPathComparer : IComparer<XPathItem>
    {
        #region IComparer<XPathItem> Members

        public int Compare(XPathItem x, XPathItem y)
        {
            XPathNavigator nav1 = x as XPathNavigator;
            XPathNavigator nav2 = y as XPathNavigator;
            if (nav1 != null && nav2 != null)
                switch (nav1.ComparePosition(nav2))
                {
                    case XmlNodeOrder.Before:
                        return -1;

                    case XmlNodeOrder.After:
                        return 1;

                    case XmlNodeOrder.Same:
                        return 0;

                    default:
                        {
                            XQueryNavigator xnav1 = nav1 as XQueryNavigator;
                            XQueryNavigator xnav2 = nav2 as XQueryNavigator;
                            int hashCode1 = (nav1 == null) ? nav1.GetHashCode() : xnav1.Document.GetHashCode();
                            int hashCode2 = (nav2 == null) ? nav2.GetHashCode() : xnav2.Document.GetHashCode();
                            if (hashCode1 < hashCode2)
                                return -1;
                            else if (hashCode1 > hashCode2)
                                return 1;
                            else
                                throw new InvalidOperationException();
                        }
                }
            else
                throw new XQueryException(Properties.Resources.XPTY0004,
                    "xs:anyAtomicType", "node()* in function op:union,op:intersect and op:except");
        }
        #endregion
    }

    public class XPathNavigatorEqualityComparer : IEqualityComparer<XPathItem>
    {
        #region IEqualityComparer<XPathItem> Members

        public bool Equals(XPathItem x, XPathItem y)
        {
            XPathNavigator nav1 = x as XPathNavigator;
            XPathNavigator nav2 = y as XPathNavigator;
            if (nav1 != null && nav2 != null)
                return nav1.IsSamePosition(nav2);
            else
                throw new XQueryException(Properties.Resources.XPTY0004, "xs:anyAtomicType",
                    "node()* in function op:union,op:intersect and op:except");
        }

        public int GetHashCode(XPathItem obj)
        {
            if (obj.IsNode)
            {
                XQueryNavigator nav = obj as XQueryNavigator;
                if (nav != null)
                    return nav.Document.GetHashCode() << 8 ^ nav.Position;
                else
                    return XPathNavigator.NavigatorComparer.GetHashCode(obj);
            }
            else
                throw new XQueryException(Properties.Resources.XPTY0004, "xs:anyAtomicType",
                    "node()* in function op:union,op:intersect and op:except");
        }

        #endregion
    }
}
