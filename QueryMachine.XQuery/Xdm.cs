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
using System.IO;

using System.Xml;
using System.Xml.Schema;

namespace DataEngine.XQuery
{
    public enum XdmNodeType
    {        
        Document,        
        ElementStart,
        ElementEnd,        
        Attribute, 
        Namespace,
        Text,        
        Pi,  
        Comment,
        Whitespace,
        Cdata
    }
    
    internal abstract class XdmNode
    {        
        public abstract XdmNodeType NodeType { get; }

        public virtual bool Completed
        {
            get
            {
                return true;
            }
        }

        public virtual String Value
        {
            get
            {
                return String.Empty;
            }
        }

        public abstract void Load(PageFile pagefile);

        public abstract void Store(PageFile pagefile);

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            switch (NodeType)
            {
                case XdmNodeType.Document:
                    {
                        XdmDocument node = (XdmDocument)this;
                        sb.AppendFormat("Document standalone = {0}", node.standalone);
                    }
                    break;

                case XdmNodeType.ElementStart:
                    {
                        XdmElementStart node = (XdmElementStart)this;
                        sb.AppendFormat("Element {0} [{1}]", node._nodeInfo.name, node._linkNext);
                        XdmAttribute attr = node._attributes;
                        while (attr != null)
                        {
                            sb.Append(" ");
                            sb.AppendFormat("{0} = '{1}'", attr._nodeInfo.name, attr._value);
                            attr = attr._next;
                        }
                    }
                    break;

                case XdmNodeType.Attribute:
                    {
                        XdmAttribute node = (XdmAttribute)this;
                        sb.AppendFormat("{0} = '{1}'", node._nodeInfo.name, node._value);
                    }
                    break;

                case XdmNodeType.ElementEnd:
                    {
                        XdmElementEnd node = (XdmElementEnd)this;
                        sb.AppendFormat("ElementEnd {0}", node._linkHead);
                    }
                    break;

                case XdmNodeType.Comment:
                    {
                        XdmComment node = (XdmComment)this;
                        sb.AppendFormat("Comment '{0}'", node._text);
                    }
                    break;

                case XdmNodeType.Pi:
                    {
                        XdmProcessingInstruction node = (XdmProcessingInstruction)this;
                        sb.AppendFormat("Pi {0} {1}", node._name, node._value);
                    }
                    break;

                case XdmNodeType.Text:
                    sb.AppendFormat("Text '{0}'", Value);
                    break;

                case XdmNodeType.Whitespace:
                    sb.Append("S");
                    break;

                case XdmNodeType.Cdata:
                    sb.AppendFormat("Cdata '{0}'", Value);
                    break;
            }
            return sb.ToString();
        }
    }

    internal class XdmDocument : XdmNode
    {
        internal bool standalone;

        public override XdmNodeType NodeType
        {
            get { return XdmNodeType.Document; }
        }

        public override void Load(PageFile pagefile)
        {
            standalone = pagefile.ReadBoolean();
        }

        public override void Store(PageFile pagefile)
        {
            pagefile.WriteBoolean(standalone);
        }
    }

    internal class XdmAttribute : XdmNode
    {
        internal XQueryNodeInfo _nodeInfo;
        internal String _value;
        internal IXmlSchemaInfo _schemaInfo;
        internal XdmAttribute _next;

        public override XdmNodeType NodeType
        {
            get { return XdmNodeType.Attribute; }
        }

        public override string Value
        {
            get
            {
                return _value;
            }
        }

        public override void Load(PageFile pagefile)
        {
            _nodeInfo = pagefile.ReadNodeInfo();
            _value = pagefile.ReadString();
            if (pagefile.HasSchemaInfo)
                _schemaInfo = pagefile.ReadSchemaInfo();
        }

        public override void Store(PageFile pagefile)
        {
            pagefile.WriteNodeInfo(_nodeInfo);
            pagefile.WriteString(_value);
            if (pagefile.HasSchemaInfo)
                pagefile.WriteSchemaInfo(_schemaInfo);
        }
    }

    internal class XdmNamespace : XdmNode
    {
        internal String _name;
        internal String _value;
        internal XdmNamespace _next;

        public override XdmNodeType NodeType
        {
            get { return XdmNodeType.Namespace; }
        }

        public override string Value
        {
            get
            {
                return _value;
            }
        }

        public override void Load(PageFile pagefile)
        {
            _name = pagefile.ReadString();
            _value = pagefile.ReadString();
        }

        public override void Store(PageFile pagefile)
        {
            pagefile.WriteString(_name);
            pagefile.WriteString(_value);
        }
    }

    internal class XdmElementStart : XdmNode
    {
        internal int _linkNext;
        internal XQueryNodeInfo _nodeInfo;
        internal XdmNamespace _ns;
        internal XdmAttribute _attributes;         

        public override XdmNodeType NodeType
        {
            get { return XdmNodeType.ElementStart; }
        }

        public override bool Completed
        {
            get
            {
                return _linkNext != 0;
            }
        }

        public override void Load(PageFile pagefile)
        {
            _linkNext = pagefile.ReadInt32();
            _nodeInfo = pagefile.ReadNodeInfo();
            XdmNamespace ns = null;
            while (true)
            {
                if (!pagefile.ReadBoolean())
                    break;
                XdmNamespace curr = new XdmNamespace();
                curr.Load(pagefile);
                if (ns == null)
                    _ns = ns = curr;
                else
                {
                    ns._next = curr;
                    ns = curr;
                }
            }
            XdmAttribute attr = null;
            while (true)
            {
                if (!pagefile.ReadBoolean())
                    break;
                XdmAttribute curr = new XdmAttribute();
                curr.Load(pagefile);
                if (attr == null)
                    _attributes = attr = curr;
                else
                {
                    attr._next = curr;
                    attr = curr;
                }
            }
        }

        public override void Store(PageFile pagefile)
        {
            pagefile.WriteInt32(_linkNext);
            pagefile.WriteNodeInfo(_nodeInfo);
            XdmNamespace ns = _ns;
            while (ns != null)
            {
                pagefile.WriteBoolean(true);
                ns.Store(pagefile);
                ns = ns._next;
            }
            pagefile.WriteBoolean(false);
            XdmAttribute attr = _attributes;
            while (attr != null)
            {
                pagefile.WriteBoolean(true);
                attr.Store(pagefile);
                attr = attr._next;
            }
            pagefile.WriteBoolean(false);
        }
    }

    internal class XdmElementEnd : XdmNode
    {
        internal int _linkHead;
        internal IXmlSchemaInfo _schemaInfo;

        public override XdmNodeType NodeType
        {
            get { return XdmNodeType.ElementEnd; }
        }

        public override void Load(PageFile pagefile)
        {
            _linkHead = pagefile.ReadInt32();
            if (pagefile.HasSchemaInfo)
                _schemaInfo = pagefile.ReadSchemaInfo();
        }

        public override void Store(PageFile pagefile)
        {
            pagefile.WriteInt32(_linkHead);
            if (pagefile.HasSchemaInfo)
                pagefile.WriteSchemaInfo(_schemaInfo);
        }
    }

    internal class XdmProcessingInstruction : XdmNode
    {
        internal String _name;
        internal String _value;

        public override XdmNodeType NodeType
        {
            get { return XdmNodeType.Pi; }
        }

        public override string Value
        {
            get
            {
                return _value;
            }
        }

        public override void Load(PageFile pagefile)
        {
            _name = pagefile.ReadString();
            _value = pagefile.ReadString();
        }

        public override void Store(PageFile pagefile)
        {
            pagefile.WriteString(_name);
            pagefile.WriteString(_value);
        }
    }

    internal class XdmComment : XdmNode
    {
        internal String _text;

        public override XdmNodeType NodeType
        {
            get { return XdmNodeType.Comment; }
        }

        public override string Value
        {
            get
            {
                return _text;
            }
        }

        public override void Load(PageFile pagefile)
        {
            _text = pagefile.ReadString();
        }

        public override void Store(PageFile pagefile)
        {
            pagefile.WriteString(_text);
        }
    }

    internal class XdmWhitespace : XdmNode
    {
        internal String _text;

        public override string Value
        {
            get
            {
                return _text;
            }
        }

        public override XdmNodeType NodeType
        {
            get { return XdmNodeType.Whitespace; }
        }

        public override void Load(PageFile pagefile)
        {
            _text = pagefile.ReadString();
        }

        public override void Store(PageFile pagefile)
        {
            pagefile.WriteString(_text);
        }
    }

    internal class XdmText : XdmNode
    {
        internal String _text;

        public override string Value
        {
            get
            {
                return _text;
            }
        }

        public override XdmNodeType NodeType
        {
            get { return XdmNodeType.Text; }
        }

        public override void Load(PageFile pagefile)
        {
            _text = pagefile.ReadString();
        }

        public override void Store(PageFile pagefile)
        {
            pagefile.WriteString(_text);
        }
    }

    internal class XdmCdata : XdmNode
    {
        internal String _text;

        public override string Value
        {
            get
            {
                return _text;
            }
        }

        public override XdmNodeType NodeType
        {
            get { return XdmNodeType.Cdata; }
        }

        public override void Load(PageFile pagefile)
        {
            _text = pagefile.ReadString();
        }

        public override void Store(PageFile pagefile)
        {
            pagefile.WriteString(_text);
        }
    }
}
