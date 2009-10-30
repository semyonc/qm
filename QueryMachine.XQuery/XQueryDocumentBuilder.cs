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
using System.Xml.XPath;
using System.Globalization;

namespace DataEngine.XQuery
{    
    public class XQueryDocumentBuilder: XmlWriter
    {
        private PageFile m_pageFile;
        private Stack<ElementContext> m_stack = new Stack<ElementContext>();
        private string xmlns;
        private WriteState _state;
        private bool _normalize;
        private bool _stripNamespace;
        
        private StringBuilder _sb;

        private XdmAttribute attr;        
        private XdmNamespace ns;
        private HashSet<object> hs;
        private bool nsflag;

        public IXmlSchemaInfo SchemaInfo { get; set; }
        public XmlNamespaceManager NamespaceManager { get; private set; }
        public NamespaceInheritanceMode NamespaceInheritanceMode { get; set; }
       
        internal XQueryDocument m_document;
        
        public XQueryDocumentBuilder(XQueryDocument doc)
        {
            m_document = doc;
            m_pageFile = doc.pagefile;
            NameTable = doc.nameTable;
            xmlns = NameTable.Get(XmlReservedNs.NsXmlNs);
            _state = WriteState.Start;
            if (doc.input == null)
            {
                _normalize = true;
                hs = new HashSet<object>();
                NamespaceManager = new XmlNamespaceManager(NameTable);
            }
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
            CompleteElement();
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
            if (m_stack.Count == 0)
                throw new XQueryException(Properties.Resources.InvalidAttributeSequence);
            if (prefix == null)
                prefix = String.Empty;
            if (localName == null)
                throw new ArgumentException();
            if (namespaceUri == null)
                namespaceUri = String.Empty;
            ElementContext context = m_stack.Peek();
            if (context.completed)
                throw new XQueryException(Properties.Resources.XQTY0024, new XmlQualifiedName(localName, namespaceUri));
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
                if (hs != null)
                {
                    if (hs.Contains(ns._name))
                        throw new XQueryException(Properties.Resources.XQDY0025,
                            new XmlQualifiedName(element._nodeInfo.localName, element._nodeInfo.namespaceUri), ns._name);
                    hs.Add(ns._name);
                }
                if (NamespaceManager != null)
                {
                    if (!context.scopeFlag)
                    {
                        NamespaceManager.PushScope();
                        context.scopeFlag = true;
                    }
                }
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
                if (NamespaceManager != null && prefix != "")
                {
                    string ns = NamespaceManager.LookupNamespace(prefix);
                    if (ns != null && ns != namespaceUri)
                    {
                        prefix = NamespaceManager.LookupPrefix(namespaceUri);
                        if (prefix == null)
                        {
                            // Generate new prefix
                            int k = 1;
                            do
                            {
                                prefix = String.Format("ns{0}", k++);
                            }
                            while (NamespaceManager.LookupNamespace(prefix) != null);
                            NamespaceManager.AddNamespace(prefix, namespaceUri);
                        }
                    }
                }
                attr._nodeInfo = 
                    m_pageFile.NodeInfoTable.Add(prefix, localName, namespaceUri);
                nsflag = false;
                if (hs != null)
                {
                    if (hs.Contains(attr._nodeInfo.handle))
                        throw new XQueryException(Properties.Resources.XQDY0025,
                            new XmlQualifiedName(element._nodeInfo.localName, element._nodeInfo.namespaceUri), 
                            new XmlQualifiedName(attr._nodeInfo.localName, attr._nodeInfo.namespaceUri));
                    hs.Add(attr._nodeInfo.handle);
                }
            }
        }

        public override void WriteEndAttribute()
        {
            string value = _sb.ToString();
            if (nsflag)
            {      
                ns._value = value;
                ElementContext context = m_stack.Peek();
                if (context.scopeFlag)
                    NamespaceManager.AddNamespace(ns._name, ns._value);
                if ((context.element._nodeInfo.prefix == ns._name) &&
                    (context.element._nodeInfo.namespaceUri != ns._value))
                    context.element._nodeInfo = m_pageFile.NodeInfoTable.Add(
                        context.element._nodeInfo.prefix, context.element._nodeInfo.localName, ns._value);
            }
            else
            {
                if (_normalize)
                {
                    if ((attr._nodeInfo.localName == "id" && attr._nodeInfo.namespaceUri == XmlReservedNs.NsXml) ||
                        (SchemaInfo != null && SchemaInfo.SchemaType.TypeCode == XmlTypeCode.Id))
                        value = XQueryFuncs.NormalizeSpace(value);
                }
                attr._value = value;
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
                    if (NamespaceManager != null)
                    {
                        IDictionary<string, string> dict = NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local);
                        string prefix = context.element._nodeInfo.prefix;
                        if (prefix != "" && !dict.ContainsKey(prefix))
                        {
                            WriteStartAttribute("xmlns", prefix, xmlns);
                            WriteString(context.element._nodeInfo.namespaceUri);
                            WriteEndAttribute();
                        }
                        XdmAttribute curr = context.element._attributes;
                        while (curr != null)
                        {
                            prefix = curr._nodeInfo.prefix;
                            if (prefix != "")
                            {
                                dict = NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local);
                                if (!dict.ContainsKey(prefix))
                                {
                                    WriteStartAttribute("xmlns", prefix, xmlns);
                                    WriteString(curr._nodeInfo.namespaceUri);
                                    WriteEndAttribute();
                                }
                            }
                            curr = curr._next;
                        }
                        if (_stripNamespace)
                        {
                            List<XdmNamespace> nodes = new List<XdmNamespace>();
                            for (XdmNamespace ns1 = context.element._ns; ns1 != null; ns1 = ns1._next)
                            {
                                if (ns1._value == context.element._nodeInfo.namespaceUri ||
                                    (NamespaceInheritanceMode == NamespaceInheritanceMode.Inherit &&
                                        context.inheritedScope.ContainsKey(ns1._name)))
                                    nodes.Add(ns1);
                                else
                                {
                                    for (XdmAttribute atr = context.element._attributes; atr != null; atr = atr._next)
                                    {
                                        if (atr._nodeInfo.namespaceUri == ns1._value)
                                        {
                                            nodes.Add(ns1);
                                            break;
                                        }
                                    }
                                }
                            }
                            context.element._ns = ns = null;                            
                            foreach (XdmNamespace ns1 in nodes)
                            {
                                if (ns == null)
                                    context.element._ns = ns = ns1;
                                else
                                    ns._next = ns1;
                                ns1._next = null;
                            }
                        }
                        if (NamespaceInheritanceMode == NamespaceInheritanceMode.Inherit)
                        {
                            dict = NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local);
                            foreach (KeyValuePair<string, string> kvp in context.inheritedScope)
                            {
                                if (kvp.Key != "" && !(context.scopeFlag && dict.ContainsKey(kvp.Key)))
                                {
                                    WriteStartAttribute("xmlns", kvp.Key, xmlns);
                                    WriteString(kvp.Value);
                                    WriteEndAttribute();
                                }
                            }
                        }
                    }
                    attr = null;
                    ns = null;
                    if (hs != null)
                        hs.Clear();
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
            if (prefix == null)
                prefix = String.Empty;
            if (localName == null)
                throw new ArgumentException();
            if (ns == null)
                ns = String.Empty;
            element._nodeInfo = m_pageFile.NodeInfoTable.Add(prefix, localName, ns);
            context.element = element;
            context.pos = m_pageFile.Count;
            m_stack.Push(context);
            context.isEmptyElement = IsEmptyElement;
            if (NamespaceManager != null && NamespaceInheritanceMode == NamespaceInheritanceMode.Inherit)
                context.inheritedScope = NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local);
            m_pageFile.AddNode(element);
        }

        public override void WriteEndElement()
        {
            CompleteElement();
            ElementContext context = m_stack.Pop();
            if (context.scopeFlag)
                NamespaceManager.PopScope();
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
            {
                _sb.Append(text);
            }
            else
            {
                CompleteElement();
                XdmText node = m_pageFile.LastNode as XdmText;
                if (node != null)
                    node._text = node._text + text;
                else
                {
                    node = new XdmText();
                    node._text = text;
                    m_pageFile.AddNode(node);
                }
            }
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new NotImplementedException();
        }

        public override void WriteWhitespace(string ws)
        {
            if (_state == WriteState.Attribute)
                _sb.Append(ws);
            else
            {
                CompleteElement();
                XdmWhitespace node = m_pageFile.LastNode as XdmWhitespace;
                if (node != null)
                    node._text = node._text + ws;
                else
                {
                    node = new XdmWhitespace();
                    node._text = ws;
                    m_pageFile.AddNode(node);
                }
            }
        }

        public void WriteNode(XPathNavigator navigator, NamespacePreserveMode preserveMode, 
            ElementConstructionMode constructMode)
        {
            _stripNamespace = (preserveMode == NamespacePreserveMode.NoPreserve);
            try
            {                
                base.WriteNode(navigator, false);
            }
            finally
            {
                _stripNamespace = false;
            }
        }

        public XmlNameTable NameTable { get; private set; }

        internal protected class ElementContext
        {
            internal XdmElementStart element;
            internal int pos;
            internal bool isEmptyElement;
            internal bool completed;
            internal bool scopeFlag;
            internal IDictionary<string, string> inheritedScope;
        }
    }
}
