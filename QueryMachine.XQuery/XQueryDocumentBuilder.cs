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
using DataEngine.XQuery.DocumentModel;

namespace DataEngine.XQuery
{    
    public class XQueryDocumentBuilder: XmlWriter
    {
        private string xmlns;
        
        private PageFile m_pageFile;
        private ElementContext context;
        private bool cflag;

        private WriteState _state;
        private DmContainer _parent;
        private bool _normalize;
        private bool _stripNamespace;
        
        private String _text;
        private String _attr_text;

        private XdmAttribute attr;        
        private XdmNamespace ns;
        private XdmNamespace ns2;
        private HashSet<object> hs;
        private bool nsflag;        

        public IXmlSchemaInfo SchemaInfo { get; set; }
        public XmlNamespaceManager NamespaceManager { get; private set; }
        public NamespaceInheritanceMode NamespaceInheritanceMode { get; set; }
       
        internal XQueryDocument m_document;
       
        public XQueryDocumentBuilder(XQueryDocument doc)
        {            
            m_document = doc;
            if (doc.pagefile == null)
            {
                m_pageFile = new PageFile(false);
                m_pageFile.HasSchemaInfo = true;
                doc.pagefile = m_pageFile;
            }
            else
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
            DocumentRoot = new DmRoot();
            _parent = DocumentRoot;
            _text = null;
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
            WriteString(text);
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
            int pos = -1;
            if (context != null)
                pos = context.pos;
            DmComment comment = (DmComment)_parent.CreateChildComment();
            comment.IndexNode(m_pageFile.Count);
            m_pageFile.AddNode(pos, comment, new XdmComment(text));
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
            int pos = -1;
            if (context != null)
                pos = context.pos;
            DmPI pi = (DmPI)_parent.CreateChildPI(name, NameTable);
            pi.IndexNode(m_pageFile.Count);
            m_pageFile.AddNode(pos, pi, new XdmProcessingInstruction(text));
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
            if (context == null)
                throw new XQueryException(Properties.Resources.InvalidAttributeSequence);
            if (prefix == null)
                prefix = String.Empty;
            if (localName == null)
                throw new ArgumentException();
            if (namespaceUri == null)
                namespaceUri = String.Empty;
            if (!cflag || _text != null)
                throw new XQueryException(Properties.Resources.XQTY0024, new XmlQualifiedName(localName, namespaceUri));
            _state = WriteState.Attribute;
            _attr_text = null;
            DmNode node = context.node;
            XdmElement element = context.element;
            if (namespaceUri == xmlns)
            {
                ns2 = new XdmNamespace();
                ns2._name = String.IsNullOrEmpty(prefix) ? 
                    String.Empty : localName;
                nsflag = true;
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
                        {   // Generate new prefix
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
                DmQName name = new DmQName(prefix, localName, namespaceUri, NameTable);
                attr._dm = (DmAttribute)context.node.CreateAttribute(name);
                nsflag = false;
                if (hs != null)
                {
                    if (hs.Contains(name))
                        throw new XQueryException(Properties.Resources.XQDY0025,
                            new XmlQualifiedName(node.LocalName, node.NamespaceURI), 
                            new XmlQualifiedName(localName, namespaceUri));
                    hs.Add(name);
                }
            }
        }

        public override void WriteEndAttribute()
        {
            _state = WriteState.Element;
            string value = _attr_text != null ? 
                _attr_text : String.Empty;
            _attr_text = null;
            if (nsflag)
            {      
                ns2._value = value;
                if (context.node.Prefix == ns2._name && context.node.NamespaceURI != ns2._value)
                {
                    context.node = (DmElement)_parent.CreateChild(new DmQName(context.node.Prefix, 
                        context.node.LocalName, ns2._value, NameTable));
                    for (XdmAttribute attr = context.element._attributes; attr != null; attr = attr._next)
                    {
                        DmAttribute src = attr._dm;
                        attr._dm = (DmAttribute)context.node.CreateAttribute(src.QName);
                        attr._dm.SchemaInfo = src.SchemaInfo;
                        attr._dm.IsId = src.IsId;
                    }
                }
                if (context.scopeFlag)
                {
                    if (NamespaceInheritanceMode != NamespaceInheritanceMode.Inherit && 
                        NamespaceManager.LookupNamespace(ns2._name) == ns2._value)
                        return;
                    NamespaceManager.AddNamespace(ns2._name, ns2._value);
                }
                if (hs != null)
                {
                    if (hs.Contains(ns2._name))
                        throw new XQueryException(Properties.Resources.XQDY0025,
                            new XmlQualifiedName(context.node.LocalName, context.node.NamespaceURI), ns2._name);
                    hs.Add(ns2._name);
                }
                if (ns == null)
                    context.element._ns = ns2;
                else
                    ns._next = ns2;
                ns = ns2;
            }
            else
            {
                if (_normalize)
                {
                    if ((attr._dm.LocalName == "id" && attr._dm.NamespaceURI == XmlReservedNs.NsXml) ||
                        (SchemaInfo != null && SchemaInfo.SchemaType.TypeCode == XmlTypeCode.Id))
                        value = XQueryFuncs.NormalizeSpace(value);
                }
                attr._value = value;
                if (SchemaInfo != null && SchemaInfo.SchemaAttribute != null)
                    attr._dm.SchemaInfo = SchemaInfo;

            }            
        }

        internal void CompleteElement()
        {
            CompleteElement(false);
        }

        private void CompleteElement(bool isElemEnd)
        {
            if (cflag)
            {
                int pos = -1;
                if (context.parent != null)
                    pos = context.parent.pos;
                context.node.IndexNode(m_pageFile.Count);
                m_pageFile.AddNode(pos, _parent = context.node, context.element);
                if (isElemEnd)
                {
                    if (_text != null)
                    {
                        context.element._value = _text;
                        m_pageFile[context.pos] = PageFile.MixedLeaf;
                        _parent.CreateChildText();
                        _text = null;
                    }
                    else
                        m_pageFile[context.pos] = PageFile.Leaf;
                }
                else
                {
                    m_pageFile[context.pos] = 0;
                    if (_text != null)
                    {
                        DmText node = (DmText)_parent.CreateChildText();
                        node.IndexNode(m_pageFile.Count);
                        m_pageFile.AddNode(context.pos, node, new XdmText(_text));
                        _text = null;
                    }
                }
                if (NamespaceManager != null)
                {
                    IDictionary<string, string> dict = NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local);
                    string prefix = context.node.Prefix;
                    if (prefix != "" && !dict.ContainsKey(prefix))
                    {
                        WriteStartAttribute("xmlns", prefix, xmlns);
                        WriteString(context.node.NamespaceURI);
                        WriteEndAttribute();
                    }
                    XdmAttribute curr = context.element._attributes;
                    while (curr != null)
                    {
                        prefix = curr._dm.Prefix;
                        if (prefix != "")
                        {
                            dict = NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local);
                            if (!dict.ContainsKey(prefix))
                            {
                                WriteStartAttribute("xmlns", prefix, xmlns);
                                WriteString(curr._dm.NamespaceURI);
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
                            if (ns1._value == context.node.NamespaceURI ||
                                (NamespaceInheritanceMode == NamespaceInheritanceMode.Inherit &&
                                    context.inheritedScope.ContainsKey(ns1._name)))
                                nodes.Add(ns1);
                            else
                            {
                                for (XdmAttribute atr = context.element._attributes; atr != null; atr = atr._next)
                                {
                                    if (atr._dm.NamespaceURI == ns1._value)
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
                cflag = false;
            }
        }

        public override void WriteStartDocument(bool standalone)
        {
            DocumentRoot.Standalone = standalone;
            DocumentRoot.IndexNode(m_pageFile.Count);
            m_pageFile.AddNode(-1, DocumentRoot, new XdmDocument());
            m_pageFile[0] = 0;
            context = new ElementContext(null);
            context.pos = 0;
        }

        public override void WriteStartDocument()
        {
            WriteStartDocument(false);
        }

        public bool IsEmptyElement { get; set; }
        internal int LastElementEnd { get; private set; }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            CompleteElement(false);
            int pos = -1;
            cflag = true;
            if (context != null)
                pos = context.pos;
            context = new ElementContext(context);
            if (prefix == null)
                prefix = String.Empty;
            if (localName == null)
                throw new ArgumentException();
            if (ns == null)
                ns = String.Empty;
            context.node = (DmElement)_parent.CreateChild(new DmQName(prefix, localName, ns, NameTable));
            context.pos = m_pageFile.Count;
            context.element = new XdmElement();
            context.isEmptyElement = IsEmptyElement;
            if (NamespaceManager != null && NamespaceInheritanceMode == NamespaceInheritanceMode.Inherit)
                context.inheritedScope = NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local);            
        }

        public override void WriteEndElement()
        {
            bool isLeaf = cflag;
            CompleteElement(true);
            if (context.scopeFlag)
                NamespaceManager.PopScope();
            if (SchemaInfo != null && SchemaInfo.SchemaElement != null)
                context.node.SchemaInfo = SchemaInfo;
            LastElementEnd = context.pos;
            if (!isLeaf)
            {
                m_pageFile[context.pos] = m_pageFile.Count + 1;                
                m_pageFile.AddNode(-1, null, null);
                m_pageFile[m_pageFile.Count - 1] = LastElementEnd;
            }
            _parent = (DmContainer)context.node.ParentNode;
            context = context.parent;
        }        

        public override WriteState WriteState
        {
            get { return _state; }
        }

        internal int Position
        {
            get
            {
                if (context == null)
                    throw new InvalidOperationException();
                return context.pos;
            }
        }

        public override void WriteString(string text)
        {
            if (_state == WriteState.Attribute)
            {
                if (_attr_text == null)
                    _attr_text = text;
                else
                    _attr_text = _attr_text + text;
            }
            else
            {
                if (cflag)
                {
                    if (_text == null)
                        _text = text;
                    else
                        _text = _text + text;
                }
                else
                {
                    XdmText data = m_pageFile.LastNode as XdmText;
                    if (data != null)
                        data._text = data._text + text;
                    else
                    {
                        int pos = -1;
                        if (context != null)
                            pos = context.pos;
                        DmText node = (DmText)_parent.CreateChildText();
                        node.IndexNode(m_pageFile.Count);
                        m_pageFile.AddNode(pos, node, new XdmText(text));
                    }
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
            {
                if (_attr_text == null)
                    _attr_text = ws;
                else
                    _attr_text = _attr_text + ws;
            }
            else
            {
                CompleteElement();
                XdmWhitespace data = m_pageFile.LastNode as XdmWhitespace;
                if (data != null)
                    data._text = data._text + ws;
                else
                {
                    int pos = -1;
                    if (context != null)
                        pos = context.pos;
                    DmWhitespace node = (DmWhitespace)_parent.CreateChildWhitespace();
                    node.IndexNode(m_pageFile.Count);
                    m_pageFile.AddNode(pos, node, new XdmWhitespace(ws));
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

        internal DmRoot DocumentRoot { get; private set; }

        public XmlNameTable NameTable { get; private set; }

        internal protected class ElementContext
        {
            public ElementContext parent;
            internal DmElement node;
            internal XdmElement element;
            public int pos;
            public bool isEmptyElement;
            public bool scopeFlag;
            public IDictionary<string, string> inheritedScope;

            public ElementContext(ElementContext parent)
            {
                this.parent = parent;
            }
        }
    }
}
