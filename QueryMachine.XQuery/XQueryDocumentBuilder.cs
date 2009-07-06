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
    class XQueryDocumentBuilder: XmlWriter
    {
        private PageFile m_pageFile;
        private Stack<ElementContext> m_stack = new Stack<ElementContext>();
        private string xmlns;
        private WriteState _state;
        
        private StringBuilder _sb;

        private XdmAttribute attr;        
        private XdmNamespace ns;
        private bool nsflag;

        public IXmlSchemaInfo SchemaInfo { get; set; }

        internal XQueryDocument m_document;
        
        public XQueryDocumentBuilder(XQueryDocument doc)
        {
            m_document = doc;
            m_pageFile = doc.pagefile;
            NameTable = doc.nameTable;
            xmlns = NameTable.Get(XmlReservedNs.NsXmlNs);
            _state = WriteState.Start;
            SchemaInfo = null;
        }

        public override void Close()
        {
            return;
        }

        public override void Flush()
        {
            return;
        }

        public override string LookupPrefix(string ns)
        {
            throw new NotImplementedException();
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            byte[] input = new byte[count];
            for (int k = 0; k < count; k++)
                input[k] = buffer[index + k];
            WriteString(Convert.ToBase64String(input));
        }

        public override void WriteCData(string text)
        {
            XdmCdata node = new XdmCdata();
            node._text = text;
            m_pageFile.AddNode(node);
        }

        public override void WriteCharEntity(char ch)
        {
            return;
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            StringWriter sw = new StringWriter();
            sw.Write(buffer, index, count);
            WriteString(sw.ToString());
        }

        public override void WriteComment(string text)
        {
            //CompleteElement();
            XdmComment node = new XdmComment();
            node._text = text;
            m_pageFile.AddNode(node);
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            return;
        }

        public override void WriteEndDocument()
        {
            return;
        }

        public override void WriteEntityRef(string name)
        {
            throw new NotImplementedException();
        }

        public override void WriteFullEndElement()
        {
            WriteEndElement();
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            CompleteElement();
            XdmProcessingInstruction node = new XdmProcessingInstruction();
            node._name = name;
            node._value = text;
            m_pageFile.AddNode(node);
        }

        public override void WriteRaw(string data)
        {
            throw new NotImplementedException();
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override void WriteStartAttribute(string prefix, string localName, string namespaceUri)
        {
            ElementContext context = m_stack.Peek();
            if (context.completed)
                throw new InvalidOperationException("Attempt to write element attribute after element child node");
            _state = WriteState.Attribute;
            _sb = new StringBuilder();
            XdmElementStart element = context.element;
            if (namespaceUri == xmlns)
            {
                if (ns == null)
                    element._ns = ns = new XdmNamespace();
                else
                {
                    XdmNamespace tmp = new XdmNamespace();
                    ns._next = tmp;
                    ns = tmp;
                }
                ns._name = String.IsNullOrEmpty(prefix) ? 
                    String.Empty : localName;
                nsflag = true;
            }
            else
            {
                if (attr == null)
                    element._attributes = attr = new XdmAttribute();
                else
                {
                    XdmAttribute tmp = new XdmAttribute();
                    attr._next = tmp;
                    attr = tmp;
                }
                attr._nodeInfo = 
                    m_pageFile.NodeInfoTable.Add(prefix, localName, namespaceUri);
                nsflag = false;
            }
        }

        public override void WriteEndAttribute()
        {
            if (nsflag)
            {             
                ns._value = _sb.ToString();
                ElementContext context = m_stack.Peek();
                if ((context.element._nodeInfo.prefix == ns._name) &&
                    (context.element._nodeInfo.namespaceUri != ns._value))
                    context.element._nodeInfo = m_pageFile.NodeInfoTable.Add(
                        context.element._nodeInfo.prefix, context.element._nodeInfo.localName, ns._value);
            }
            else
            {
                attr._value = _sb.ToString();
                if (SchemaInfo != null && SchemaInfo.SchemaAttribute != null)
                    attr._schemaInfo = m_pageFile.SchemaInfoTable.Add(SchemaInfo);
            }
            _state = WriteState.Element;
        }

        internal void CompleteElement()
        {
            if (m_stack.Count > 0)
            {
                ElementContext context = m_stack.Peek();
                if (!context.completed)
                {
                    attr = null;
                    ns = null;
                    context.completed = true;
                }
            }
        }

        public override void WriteStartDocument(bool standalone)
        {
            XdmDocument node = new XdmDocument();
            node.standalone = standalone;
            m_pageFile.AddNode(node);
        }

        public override void WriteStartDocument()
        {
            XdmDocument node = new XdmDocument();
            node.standalone = false;
            m_pageFile.AddNode(node);
        }

        public bool IsEmptyElement { get; set; }
        internal int LastElementEnd { get; private set; }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            CompleteElement();
            ElementContext context = new ElementContext();
            XdmElementStart element = new XdmElementStart();
            element._nodeInfo = m_pageFile.NodeInfoTable.Add(prefix, localName, ns);
            context.element = element;
            context.pos = m_pageFile.Count;
            m_stack.Push(context);
            context.isEmptyElement = IsEmptyElement;
            m_pageFile.AddNode(element);
        }

        public override void WriteEndElement()
        {
            CompleteElement();
            ElementContext context = m_stack.Pop();
            context.element._linkNext = m_pageFile.Count + 1;
            XdmElementEnd node = new XdmElementEnd();
            node._linkHead = context.pos;
            if (SchemaInfo != null && SchemaInfo.SchemaElement != null)
                node._schemaInfo = m_pageFile.SchemaInfoTable.Add(SchemaInfo);
            LastElementEnd = context.pos;
            m_pageFile.AddNode(node);
        }        

        public override WriteState WriteState
        {
            get { return _state; }
        }

        public override void WriteString(string text)
        {
            if (_state == WriteState.Attribute)
                _sb.Append(text);
            else
            {
                //CompleteElement();
                XdmText node = new XdmText();
                node._text = text;
                m_pageFile.AddNode(node);
            }
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new NotImplementedException();
        }

        public override void WriteWhitespace(string ws)
        {            
            return;
        }

        public XmlNameTable NameTable { get; private set; }

        protected class ElementContext
        {
            public XdmElementStart element;
            public int pos;            
            public bool isEmptyElement;
            public bool completed;
        }
    }
}
