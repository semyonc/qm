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
using DataEngine.XQuery.DocumentModel;

namespace DataEngine.XQuery
{   
    internal abstract class XdmNode
    {
        protected XdmNode()
        {
        }

        public virtual String Value
        {
            get
            {
                return String.Empty;
            }
        }

        public virtual void Load(XdmReader reader)
        {
        }

        public virtual void Store(XdmWriter writer)
        {
        }
    }

    internal class XdmDocument : XdmNode
    {
        public XdmDocument()
        {
        }

        public override void Load(XdmReader reader)
        {            
        }

        public override void Store(XdmWriter writer)
        {            
        }
    }

    internal sealed class XdmAttribute : XdmNode
    {
        internal DmAttribute _dm;
        internal String _value;
        internal XdmAttribute _next;

        public XdmAttribute()
        {
        }

        public override string Value
        {
            get
            {
                return _value;
            }
        }

        public override void Load(XdmReader reader)
        {
            _dm = reader.ReadAttributeInfo();
            _value = reader.ReadString();
        }

        public override void Store(XdmWriter writer)
        {
            writer.WriteAttributeInfo(_dm);
            writer.WriteString(_value);
        }
    }

    internal sealed class XdmNamespace : XdmNode
    {
        internal String _name;
        internal String _value;
        internal XdmNamespace _next;

        public XdmNamespace()
        {
        }

        public override string Value
        {
            get
            {
                return _value;
            }
        }

        public override void Load(XdmReader reader)
        {
            base.Load(reader);
            _name = reader.ReadString();
            _value = reader.ReadString();
        }

        public override void Store(XdmWriter writer)
        {
            base.Store(writer);
            writer.WriteString(_name);
            writer.WriteString(_value);
        }
    }

    internal sealed class XdmElement : XdmNode
    {
        internal XdmNamespace _ns;
        internal XdmAttribute _attributes;
        internal string _value = String.Empty;

        public XdmElement()
        {
        }

        public override string Value
        {
            get
            {
                return _value;
            }
        }

        public override void Load(XdmReader reader)
        {
            base.Load(reader);
            XdmNamespace ns = null;
            while (true)
            {
                if (!reader.ReadBoolean())
                    break;
                XdmNamespace curr = new XdmNamespace();
                curr.Load(reader);
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
                if (!reader.ReadBoolean())
                    break;
                XdmAttribute curr = new XdmAttribute();
                curr.Load(reader);
                if (attr == null)
                    _attributes = attr = curr;
                else
                {
                    attr._next = curr;
                    attr = curr;
                }
            }            
        }

        public override void Store(XdmWriter writer)
        {
            base.Store(writer);
            XdmNamespace ns = _ns;
            while (ns != null)
            {
                writer.WriteBoolean(true);
                ns.Store(writer);
                ns = ns._next;
            }
            writer.WriteBoolean(false);
            XdmAttribute attr = _attributes;
            while (attr != null)
            {
                writer.WriteBoolean(true);
                attr.Store(writer);
                attr = attr._next;
            }
            writer.WriteBoolean(false);
        }

        public void LoadTextValue(XdmReader reader)
        {
            _value = reader.ReadString();
        }

        public void StoreTextValue(XdmWriter writer)
        {
            writer.WriteString(_value);
        }
    }

    internal sealed class XdmProcessingInstruction : XdmNode
    {
        internal String _value;

        public XdmProcessingInstruction()
        {
        }

        public XdmProcessingInstruction(string value)
        {
            _value = value;
        }

        public override string Value
        {
            get
            {
                return _value;
            }
        }

        public override void Load(XdmReader reader)
        {
            base.Load(reader);
            _value = reader.ReadString();
        }

        public override void Store(XdmWriter writer)
        {
            base.Store(writer);
            writer.WriteString(_value);
        }
    }

    internal sealed class XdmComment : XdmNode
    {
        internal String _text;

        public XdmComment()
        {
        }

        public XdmComment(string text)
        {
            _text = text;
        }

        public override string Value
        {
            get
            {
                return _text;
            }
        }

        public override void Load(XdmReader reader)
        {
            base.Load(reader);
            _text = reader.ReadString();
        }

        public override void Store(XdmWriter writer)
        {
            base.Store(writer);
            writer.WriteString(_text);
        }
    }

    internal sealed class XdmWhitespace : XdmNode
    {
        internal String _text;

        public XdmWhitespace()
        {
        }

        public XdmWhitespace(string text)
        {
            _text = text;
        }

        public override string Value
        {
            get
            {
                return _text;
            }
        }

        public override void Load(XdmReader reader)
        {
            base.Load(reader);
            _text = reader.ReadString();
        }

        public override void Store(XdmWriter writer)
        {
            base.Store(writer);
            writer.WriteString(_text);
        }
    }

    internal sealed class XdmText : XdmNode
    {
        internal String _text;

        public XdmText()
        {
        }

        public XdmText(string text)
        {
            _text = text;
        }

        public override string Value
        {
            get
            {
                return _text;
            }
        }

        public override void Load(XdmReader reader)
        {
            base.Load(reader);
            _text = reader.ReadString();
        }

        public override void Store(XdmWriter writer)
        {
            base.Store(writer);
            writer.WriteString(_text);
        }
    }
}
