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

namespace DataEngine.XQuery
{
    public class XQueryNavigator : XPathNavigator, IConvertible
    {
        protected class ElementContext
        {
            public ElementContext(ElementContext parent, int pos)
            {
                this.parent = parent;
                this.pos = pos;
            }

            public int pos;
            public ElementContext parent;
        }

        private struct Properties
        {
            public XPathNodeType nodeType;
            public string name;
            public int index;
            public string prefix;
            public string localName;
            public string namespaceUri;
        }

        internal int _pos;
        private XdmNode _curr;
        private XQueryDocument _document;
        private PageFile _pf;
        private Properties _props;
        private ElementContext _context;


        private XQueryNavigator()
        {
        }

        internal XQueryNavigator(XQueryDocument doc)
            : this()
        {
            _document = doc;
            _pf = _document.pagefile;
            _context = null;
            _pos = 0;            
            Read();
            EnterElement();
        }

        public XQueryDocument Document
        {
            get
            {
                return _document;
            }
        }

        public override string BaseURI
        {
            get 
            { 
                return _document.baseUri; 
            }
        }

        public override XPathNavigator Clone()
        {
            XQueryNavigator clone = new XQueryNavigator();
            clone._document = _document;
            clone._pf = _pf;
            clone._pos = _pos;
            clone._curr = _curr;
            clone._props = _props;
            clone._context = _context;            
            return clone;
        }

        public override bool IsEmptyElement
        {
            get
            {
                if (_curr.NodeType == XdmNodeType.ElementStart)
                {
                    int p = _pos + 1;
                    _document.ExpandPageFile(p);
                    if (p < _pf.Count)
                        return _pf[p].NodeType == XdmNodeType.ElementEnd;
                }
                return false;
            }
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            if (other is XQueryNavigator)
            {
                XQueryNavigator nav = (XQueryNavigator)other;
                return nav._document == _document &&
                    (nav._pos == _pos) && (nav._props.index == _props.index);
            }
            else
                return false;
        }

        public override XmlNodeOrder ComparePosition(XPathNavigator other)
        {
            XQueryNavigator nav = other as XQueryNavigator;
            if (nav != null && nav._document == _document)
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

        public override string LocalName
        {
            get
            {
                return _props.localName;
            }
        }

        public override bool MoveTo(XPathNavigator other)
        {
            XQueryNavigator nav = other as XQueryNavigator;
            if (nav != null)
            {
                if (nav._document != _document)
                    throw new InvalidOperationException();
                _context = nav._context;
                _pos = nav._pos;
                _curr = nav._curr;
                _props = nav._props;
                return true;
            }
            else
                return false;
        }
        
        public override bool MoveToFirstAttribute()
        {
            if (_curr.NodeType == XdmNodeType.ElementStart)
            {
                XdmElementStart element = (XdmElementStart)_curr;
                if (element._attributes != null)
                {
                    _curr = element._attributes;
                    _props.index = 1;
                    Update();
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            if (_curr.NodeType == XdmNodeType.ElementStart)
            {
                if (namespaceScope != XPathNamespaceScope.Local)
                    throw new NotImplementedException();
                XdmElementStart element = (XdmElementStart)_curr;
                if (element._ns != null)
                {
                    _curr = element._ns;
                    Update();                    
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToNextAttribute()
        {
            if (_curr.NodeType == XdmNodeType.Attribute)
            {
                XdmAttribute attr = (XdmAttribute)_curr;
                if (attr._next != null)
                {
                    _curr = attr._next;
                    _props.index++;
                    Update();
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            if (_curr.NodeType == XdmNodeType.Namespace)
            {
                if (namespaceScope != XPathNamespaceScope.Local)
                    throw new NotImplementedException();
                XdmNamespace ns = (XdmNamespace)_curr;
                if (ns._next != null)
                {
                    _curr = ns._next;
                    Update();
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToFirstChild()
        {
            if (_curr.NodeType == XdmNodeType.Document ||
                _curr.NodeType == XdmNodeType.ElementStart)
            {
                int p = _pos + 1;
                _document.ExpandPageFile(p);
                if (p < _pf.Count && _pf[p].NodeType != XdmNodeType.ElementEnd)
                {
                    _pos = p;
                    Read();
                    if (NodeType == XPathNodeType.Element)
                        EnterElement();
                    return true;
                }

            }
            return false;
        }

        public override bool MoveToId(string id)
        {
            return false;
        }

        public override bool MoveToNext()
        {
            if (_props.nodeType == XPathNodeType.Element ||
                _props.nodeType == XPathNodeType.Comment ||
                _props.nodeType == XPathNodeType.ProcessingInstruction ||
                _props.nodeType == XPathNodeType.Text ||
                _props.nodeType == XPathNodeType.SignificantWhitespace)
            {
                if (_curr.NodeType == XdmNodeType.ElementStart)
                {
                    XdmElementStart node = (XdmElementStart)_curr;
                    if (node._linkNext == 0)
                        _document.ExpandUtilElementEnd(_pos);
                    _document.ExpandPageFile(node._linkNext);
                    if (node._linkNext < _pf.Count)
                    {
                        XdmNode node2 = _pf[node._linkNext];
                        if (node2.NodeType != XdmNodeType.ElementEnd)
                        {
                            _pos = node._linkNext;
                            LeaveElement();
                            Read();
                            if (NodeType == XPathNodeType.Element)
                                EnterElement();
                            return true;
                        }
                    }
                }
                else
                {
                    int p = _pos + 1;
                    _document.ExpandPageFile(p);
                    if (p < _pf.Count)
                    {
                        XdmNode node = _pf[p];
                        if (node.NodeType != XdmNodeType.ElementEnd)
                        {
                            if (NodeType == XPathNodeType.Element)
                                LeaveElement();
                            _pos = p;
                            Read();
                            if (NodeType == XPathNodeType.Element)
                                EnterElement();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override bool MoveToParent()
        {
            if (NodeType == XPathNodeType.Element ||
                NodeType == XPathNodeType.Root)
            {
                if (_context.parent == null)
                    return false;
                LeaveElement();
            }
            _pos = _context.pos;
            Read();
            return true;
        }

        public override bool MoveToPrevious()
        {
            if (_props.nodeType == XPathNodeType.Element ||
                _props.nodeType == XPathNodeType.Comment ||
                _props.nodeType == XPathNodeType.ProcessingInstruction ||
                _props.nodeType == XPathNodeType.Text ||
                _props.nodeType == XPathNodeType.SignificantWhitespace)
            {
                int p = _pos - 1;
                if (p > 0)
                {
                    XdmNode node = _pf[p];
                    if (node.NodeType != XdmNodeType.ElementStart &&
                        node.NodeType != XdmNodeType.Document)
                    {
                        if (_props.nodeType == XPathNodeType.Element)
                            LeaveElement();
                        if (node.NodeType == XdmNodeType.ElementEnd)
                            _pos = ((XdmElementEnd)node)._linkHead;
                        else
                            _pos = p;
                        Read();
                        if (NodeType == XPathNodeType.Element)
                            EnterElement();
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual void EnterElement()
        {
            _context = new ElementContext(_context, _pos);
        }

        protected virtual void LeaveElement()
        {
            _context = _context.parent;
        }

        private void Read()
        {
            _document.ExpandPageFile(_pos);
            _curr = _pf[_pos];
            Update();
        }

        private void Update()
        {
            ClearProps();
            if (_curr.NodeType != XdmNodeType.Attribute)
                _props.index = 0;
            if (_curr.NodeType == XdmNodeType.Document)
                _props.nodeType = XPathNodeType.Root;
            else if (_curr.NodeType == XdmNodeType.ElementStart)
            {
                XdmElementStart elem = (XdmElementStart)_curr;
                _props.nodeType = XPathNodeType.Element;                
                _props.name = elem._nodeInfo.name;
                _props.prefix = elem._nodeInfo.prefix;
                _props.localName = elem._nodeInfo.localName;
                _props.namespaceUri = elem._nodeInfo.namespaceUri;
            }
            else if (_curr.NodeType == XdmNodeType.Attribute)
            {
                XdmAttribute attr = (XdmAttribute)_curr;
                _props.nodeType = XPathNodeType.Attribute;
                _props.name = attr._nodeInfo.name;
                _props.prefix = attr._nodeInfo.prefix;
                _props.localName = attr._nodeInfo.localName;
                _props.namespaceUri = attr._nodeInfo.namespaceUri;
            }
            else if (_curr.NodeType == XdmNodeType.Namespace)
            {
                XdmNamespace ns = (XdmNamespace)_curr;
                _props.nodeType = XPathNodeType.Namespace;
                _props.name = ns._name;
                _props.localName = ns._name;
            }
            else if (_curr.NodeType == XdmNodeType.Document)
                _props.nodeType = XPathNodeType.Root;
            else if (_curr.NodeType == XdmNodeType.Comment)
                _props.nodeType = XPathNodeType.Comment;
            else if (_curr.NodeType == XdmNodeType.Pi)
            {
                XdmProcessingInstruction node = (XdmProcessingInstruction)_curr;
                _props.nodeType = XPathNodeType.ProcessingInstruction;
                _props.localName = node._name;
                _props.name = node._name;                
            }
            else if (_curr.NodeType == XdmNodeType.Text ||
                     _curr.NodeType == XdmNodeType.Cdata)
                _props.nodeType = XPathNodeType.Text;
            else if (_curr.NodeType == XdmNodeType.Whitespace)
                _props.nodeType = XPathNodeType.SignificantWhitespace;
        }        

        private void ClearProps()
        {
            _props.nodeType = XPathNodeType.All;
            _props.name = String.Empty;
            _props.prefix = String.Empty;
            _props.localName = String.Empty;
            _props.namespaceUri = String.Empty;            
        }

        public override string Name
        {
            get
            {
                return _props.name;
            }
        }

        public override XmlNameTable NameTable
        {
            get
            {
                return _document.nameTable;
            }
        }

        public override string NamespaceURI
        {
            get
            {
                return _props.namespaceUri;
            }
        }

        public override XPathNodeType NodeType
        {
            get
            {
                return _props.nodeType;
            }
        }

        public override string Prefix
        {
            get
            {
                return _props.prefix;
            }
        }

        public override string Value
        {
            get
            {
                if (_curr.NodeType == XdmNodeType.ElementStart)
                {
                    StringBuilder sb = new StringBuilder();
                    int p = _pos + 1;
                    _document.ExpandPageFile(p);
                    while (p < _pf.Count)
                    {
                        XdmNode node = _pf[p++];
                        if (node.NodeType == XdmNodeType.ElementEnd &&
                           ((XdmElementEnd)node)._linkHead == _pos)
                            break;
                        else if (node.NodeType == XdmNodeType.Text  ||
                                 node.NodeType == XdmNodeType.Cdata ||
                                 node.NodeType == XdmNodeType.Whitespace)
                            sb.Append(node.Value);
                        _document.ExpandPageFile(p);
                    }
                    return sb.ToString();
                }
                else
                    return _curr.Value;
            }
        }

        public override IXmlSchemaInfo SchemaInfo
        {
            get
            {
                if (_curr.NodeType == XdmNodeType.ElementStart)
                {
                    XdmElementStart node = (XdmElementStart)_curr;
                    if (node._linkNext == 0)
                        _document.ExpandUtilElementEnd(_pos);
                    _document.ExpandPageFile(node._linkNext);
                    XdmElementEnd node2 = (XdmElementEnd)_pf[node._linkNext - 1];
                    return node2._schemaInfo;
                }
                else if (_curr.NodeType == XdmNodeType.Attribute)
                {
                    XdmAttribute attr = (XdmAttribute)_curr;
                    return attr._schemaInfo;
                }                
                return null;
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

        #region XQueryTypedValue Members

        public XQuerySequenceType ItemType
        {
            get 
            {
                IXmlSchemaInfo schemaInfo = SchemaInfo;
                switch (NodeType)
                {
                    case XPathNodeType.Root:
                        return new XQuerySequenceType(XmlTypeCode.Document);
                    
                    case XPathNodeType.Element:
                        if (schemaInfo == null)
                            return new XQuerySequenceType(XmlTypeCode.Element);
                        else
                            return new XQuerySequenceType(XmlTypeCode.Element, schemaInfo, ValueType);

                    case XPathNodeType.Attribute:
                        if (schemaInfo == null)
                            return new XQuerySequenceType(XmlTypeCode.Attribute);
                        else
                            return new XQuerySequenceType(XmlTypeCode.Attribute, schemaInfo, ValueType);

                    case XPathNodeType.Comment:
                        return new XQuerySequenceType(XmlTypeCode.Comment);
                    
                    case XPathNodeType.ProcessingInstruction:
                        return new XQuerySequenceType(XmlTypeCode.ProcessingInstruction);
                    
                    case XPathNodeType.Whitespace:
                    case XPathNodeType.SignificantWhitespace:
                    case XPathNodeType.Text:
                        return new XQuerySequenceType(XmlTypeCode.Text);

                    default:
                        return XQuerySequenceType.Void;
                }
            }
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
                            if (nav1.GetHashCode() < nav2.GetHashCode())
                                return -1;
                            else if (nav1.GetHashCode() > nav2.GetHashCode())
                                return 1;
                            else
                                return 0;
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

    public class XPathItemEqualityComparer : IEqualityComparer<XPathItem>
    {
        #region IEqualityComparer<XPathItem> Members

        public bool Equals(XPathItem x, XPathItem y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode(XPathItem obj)
        {
            return obj.Value.GetHashCode();
        }

        #endregion
    }
}
