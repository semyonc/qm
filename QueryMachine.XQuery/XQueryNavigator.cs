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
        protected class ElementContext
        {
            public ElementContext(ElementContext parent, int pos, bool random)
            {
                this.parent = parent;
                this.pos = pos;
                this.random = random;
            }

            public int pos;
            public ElementContext parent;
            public bool random;
        }

        private struct Properties
        {
            public XQueryDocument doc;
            public DmNode head;
            public XdmNode node;
            public ElementContext context;
            public bool random;
            public int index;
            public object typedValue;
        }

        internal int _pos;
        private PageFile _pf;
        private Properties _props;


        private XQueryNavigator()
        {
        }

        internal XQueryNavigator(XQueryDocument doc)
            : this()
        {
            _props.doc = doc;
            _pf = doc.pagefile;
            _pos = 0;
            _props.context = null;
            doc.ExpandPageFile(1);
            Read();            
        }

        public XQueryDocument Document
        {
            get
            {
                return _props.doc;
            }
        }

        public override string BaseURI
        {
            get 
            { 
                if (NodeType == XPathNodeType.Root)
                    return _props.doc.baseUri;
                return GetAttribute("base", XmlReservedNs.NsXml);
            }
        }

        protected virtual void EnterElement()
        {
            ElementContext context = _props.context;
            if (context == null || context.pos != _pos)
                _props.context = new ElementContext(context, _pos, _props.random);
        }

        protected virtual void LeaveElement()
        {
            if (_props.context != null)
            {
                _props.random = _props.context.random;
                _props.context = _props.context.parent;
            }
        }

        protected virtual ElementContext RestoreContext(int pos)
        {            
            if (pos < 0)
                return null;
            else
            {
                DmNode head;
                XdmNode data;
                _pf.Get(pos, true, out head, out data);
                if (head == null)
                    throw new ArgumentException();
                return new ElementContext(RestoreContext(data._parent), pos, false);
            }
        }

        [DebuggerStepThrough]
        public override XPathNavigator Clone()
        {
            XQueryNavigator clone = new XQueryNavigator();
            clone._pos = _pos;
            clone._pf = _pf;
            clone._props = _props;
            return clone;
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}:{1}[{2}]", _pos, NodeType, _pf[_pos]);
            return sb.ToString();
        }
#endif

        private XdmNode GetNode()
        {
            if (_props.node == null)
                _pf.Get(_pos, true, out _props.head, out _props.node);
            return _props.node;
        }

        public override bool IsEmptyElement
        {
            get
            {
                if (NodeType == XPathNodeType.Element)
                {
                    int p = _pos + 1;
                    _props.doc.ExpandPageFile(p);
                    if (p < _pf.Count)
                    {
                        if (_pf.Head(p) == null)
                            return true;
                    }
                }
                return false;
            }
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            if (other is XQueryNavigator)
            {
                XQueryNavigator nav = (XQueryNavigator)other;
                return nav._pf == _pf && nav._pos == _pos && 
                    nav._props.index == _props.index;
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
                    if (_props.index < nav._props.index)
                        return XmlNodeOrder.Before;
                    else if (_props.index > nav._props.index)
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
                if (nav._pf != _pf)
                    throw new InvalidOperationException();
                _pos = nav._pos;
                _props = nav._props;
                return true;
            }
            else
                return false;
        }
        
        public override bool MoveToFirstAttribute()
        {
            if (NodeType == XPathNodeType.Element)
            {
                XdmElement element = (XdmElement)GetNode();
                if (element._attributes != null)
                {
                    _props.node = element._attributes;
                    _props.head = element._attributes._dm;
                    _props.index = 1;
                    _props.typedValue = null;
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            if (NodeType == XPathNodeType.Element)
            {
                if (namespaceScope != XPathNamespaceScope.Local)
                    throw new NotImplementedException();
                XdmElement element = (XdmElement)GetNode();
                if (element._ns != null)
                {
                    _props.node = element._ns;
                    _props.head = new DmNamespace(element._ns._name);
                    _props.index = 1;
                    _props.typedValue = null;
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToNextAttribute()
        {
            if (NodeType == XPathNodeType.Attribute)
            {
                XdmAttribute attr = (XdmAttribute)_props.node;
                if (attr._next != null)
                {
                    _props.node = attr._next;
                    _props.head = attr._next._dm;
                    _props.index++;
                    _props.typedValue = null;
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            if (NodeType == XPathNodeType.Namespace)
            {
                if (namespaceScope != XPathNamespaceScope.Local)
                    throw new NotImplementedException();
                XdmNamespace ns = (XdmNamespace)_props.node;
                if (ns._next != null)
                {
                    _props.node = ns._next;
                    _props.head = new DmNamespace(ns._next._name);
                    _props.index++;
                    _props.typedValue = null;
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToFirstChild()
        {
            if (NodeType == XPathNodeType.Root ||
                NodeType == XPathNodeType.Element)
            {
                int p = _pos + 1;
                _props.doc.ExpandPageFile(p);
                if (p < _pf.Count && _pf.Head(p) != null)
                {
                    _pos = p;
                    Read();
                    return true;
                }

            }
            return false;
        }        

        public override bool MoveToId(string id)
        {
            _props.doc.Fill();
            Dictionary<string, int> idTable = _props.doc.IdTable;
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
            if (NodeType == XPathNodeType.Element ||
                NodeType == XPathNodeType.Root)
            {
                if (_props.context.parent == null)
                {
                    if (_props.context.random)
                        _props.context = RestoreContext(_pos);
                    if (_props.context.parent == null)
                        return false;
                }
                LeaveElement();
            }
            if (_props.context == null)
            {
                if (_props.random)
                    _props.context = RestoreContext(_pos);
                if (_props.context == null)
                    return false;
            }
            _pos = _props.context.pos;
            Read();
            return true;
        }

        public override bool MoveToNext()
        {
            if (_pos == 0)
                return false;
            int p = _pf[_pos];
            if (p == 0)
            {
                _props.doc.ExpandUtilElementEnd(_pos);
                p = _pf[_pos];
            }
            _props.doc.ExpandPageFile(p);
            if (p < _pf.Count && _pf.Head(p) != null)
            {
                if (NodeType == XPathNodeType.Element)
                    LeaveElement();
                _pos = p;
                Read();
                return true;
            }
            return false;
        }

        public override bool MoveToPrevious()
        {
            int p = _pos - 1;
            if (p > 0)
            {
                DmNode head = _pf.Head(p);
                if (head == null)
                    _pos = _pf[p];
                else
                    if (head.NodeType == XPathNodeType.Element)
                        return false;
                    else
                        _pos = p;
                if (NodeType == XPathNodeType.Element)
                    LeaveElement();
                Read();
                return true;
            }
            return false;
        }

        private void Read()
        {
            _pf.Get(_pos, false, out _props.head, out _props.node);
            _props.typedValue = null;
            _props.index = 0;
            if (NodeType == XPathNodeType.Element ||
                NodeType == XPathNodeType.Root)
                EnterElement();
        }

        public override string Name
        {
            get
            {
                return _props.head.Name;
            }
        }

        public override XmlNameTable NameTable
        {
            get
            {
                return _props.doc.nameTable;
            }
        }

        public override string NamespaceURI
        {
            get
            {
                return _props.head.NamespaceURI;
            }
        }

        public override XPathNodeType NodeType
        {
            get
            {
                return _props.head.NodeType;
            }
        }

        public override string LocalName
        {
            get
            {
                return _props.head.LocalName;
            }
        }

        public override string Prefix
        {
            get
            {
                return _props.head.Prefix;
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
                if (_props.typedValue == null)
                    _props.typedValue = GetNavigatorTypedValue();
                return _props.typedValue;
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
                if (NodeType == XPathNodeType.Element ||
                    NodeType == XPathNodeType.Root)
                {
                    StringBuilder sb = new StringBuilder();
                    int p = _pos + 1;
                    _props.doc.ExpandPageFile(p);
                    while (p < _pf.Count)
                    {                        
                        DmNode head;
                        XdmNode node;
                        _pf.Get(p, false, out head, out node);
                        if (head == null)
                        {
                            if (_pf[p] == _pos)
                                break;
                        }
                        else if (head.NodeType == XPathNodeType.Text ||
                            head.NodeType == XPathNodeType.Whitespace)
                        {
                            if (node == null)
                                _pf.Get(p, true, out head, out node);
                            sb.Append(node.Value);
                        }
                        p++;
                        _props.doc.ExpandPageFile(p);
                    }
                    return sb.ToString();
                }
                else
                {
                    XdmNode node = GetNode();
                    return node.Value;
                }
            }
        }

        public override IXmlSchemaInfo SchemaInfo
        {
            get
            {
                IXmlSchemaInfo schemaInfo = _props.head.SchemaInfo;
                if (schemaInfo == null && _pf.HasSchemaInfo)
                {
                    if (NodeType == XPathNodeType.Element && _pf[_pos] == 0)
                        _props.doc.ExpandUtilElementEnd(_pos);
                    schemaInfo = _props.head.SchemaInfo;
                }
                return schemaInfo;
            }
        }

        public int Position
        {
            get
            {
                return _pos;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException();
                _props.doc.ExpandPageFile(value);
                if (value >= _pf.Count)
                    throw new ArgumentException();
                _props.context = null;
                _props.random = true;
                _pos = value;
                Read();
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
                    return nav.Document.GetHashCode() << 8 ^ nav._pos;
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
