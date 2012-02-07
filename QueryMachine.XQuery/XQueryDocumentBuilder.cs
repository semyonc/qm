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
using System.IO;
using System.Globalization;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.XQuery.DocumentModel;
using DataEngine.XQuery.MS;

namespace DataEngine.XQuery
{    
    public class XQueryDocumentBuilder: XmlWriter
    {
        private string xmlns;
        
        internal PageFile m_pageFile;
        private DmNode _parent;
        private DmNode _last;
        
        private bool _normalize;
        private bool _stripNamespace;
        
        private HashSet<object> hs;

        public IXmlSchemaInfo SchemaInfo { get; set; }
        public XmlNamespaceManager NamespaceManager { get; private set; }
        public NamespaceInheritanceMode NamespaceInheritanceMode { get; set; }

        private class BuilderState 
        {
            public const int BeginDocument  = 0x1;
            public const int Element        = 0x2;
            public const int BeginElement   = 0x4;
            public const int BeginAttribute = 0x8;
            public const int BeginNamespace = 0x10;
        }

        private int _state;

        private class Namespace
        {
            public String name;
            public String value;
        }
        
        private class Attribute
        {
            public String prefix;
            public String localName;
            public String namespaceUri;
            public String value;
            public IXmlSchemaInfo schemaInfo;
        }

        private String _elementPrefix;
        private String _elementLocalName;
        private String _elementNamespaceUri;
        private bool _elementHasNamespaces;

        private String _namespaceName;
        private String _namespaceValue;

        private String _attributePrefix;
        private String _attributeLocalName;
        private String _attributeNamespaceUri;
        private String _attributeValue;
        
        private LinkedList<Attribute> _attributes;
        private LinkedList<Namespace> _namespaces;
        
        private List<Action> _postComplete;
       
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
            _state = BuilderState.BeginDocument;
            if (doc.input == null)
            {
                _normalize = true;
                hs = new HashSet<object>();
                NamespaceManager = new XmlNamespaceManager(NameTable);
            }
            SchemaInfo = null;
            DocumentRoot = new DmRoot();
            _parent = DocumentRoot;
            _parent._builder_pos = -1;
            _parent._builder_prior_pos = -1;
        }

        private DmContainer GetContainer()
        {
            return (DmContainer)_parent;
        }

        private void CheckState(int stateMask)
        {
            if ((_state & stateMask) == 0)
                throw new XQueryException(Properties.Resources.InvalidAttributeSequence);
        }

        private void UpdateParent()
        {
            if (m_pageFile.Count > 0)
            {
                if (_parent._builder_pos != -1)
                    m_pageFile.Update(_parent._builder_pos, Position);
                if (_parent._builder_prior_pos != -1)
                    m_pageFile[_parent._builder_prior_pos] = Position;
            }
            _parent._builder_prior_pos = Position;
        }

        private void CloseChain()
        {
            if (m_document.input == null)
                m_pageFile[Position - 1] = -1;
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
            if (NamespaceManager != null)
                return NamespaceManager.LookupPrefix(ns);
            return "";
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
            CheckState(BuilderState.BeginDocument | BuilderState.Element);
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
            CheckState(BuilderState.BeginDocument | BuilderState.Element);
            UpdateParent();
            DmComment comment = (DmComment)GetContainer().CreateChildComment();
            comment.IndexNode(Position);
            m_pageFile.AddNode(_parent._builder_pos, comment, text);
            CloseChain();
            _last = null;
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
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
            CheckState(BuilderState.BeginDocument | BuilderState.Element);
            UpdateParent();
            DmPI pi = (DmPI)GetContainer().CreateChildPI(name, NameTable);
            pi.IndexNode(Position);
            m_pageFile.AddNode(_parent._builder_pos, pi, text);
            CloseChain();
            _last = null;
        }

        public override void WriteRaw(string data)
        {
            throw new NotImplementedException();
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        private String GeneratePrefix()
        {
            String prefix;
            int k = 1;
            do
            {
                prefix = String.Format("ns{0}", k++);
            }
            while (NamespaceManager.LookupNamespace(prefix) != null);
            return prefix;
        }

        public override void WriteStartAttribute(string prefix, string localName, string namespaceUri)
        {
            if (prefix == null)
                prefix = String.Empty;
            if (localName == null)
                throw new ArgumentException("localName");
            if (namespaceUri == null)
                namespaceUri = String.Empty;
            if (_state != BuilderState.BeginElement)
                throw new XQueryException(Properties.Resources.XQTY0024, 
                    new XmlQualifiedName(localName, namespaceUri));            
            if (namespaceUri == xmlns)
            {
                _namespaceName = String.IsNullOrEmpty(prefix) ?
                    String.Empty : localName;
                _namespaceValue = "";
                _elementHasNamespaces = true;
                _state = BuilderState.BeginNamespace;
            }
            else
            {
                _attributePrefix = prefix;
                _attributeLocalName = localName;
                _attributeNamespaceUri = namespaceUri;                
                _state = BuilderState.BeginAttribute;
            }
        }

        public override void WriteEndAttribute()
        {
            CheckState(BuilderState.BeginAttribute | BuilderState.BeginNamespace);
            if (_state == BuilderState.BeginNamespace)
            {
                if (NamespaceManager == null)
                {
                    if (_namespaces == null)
                        _namespaces = new LinkedList<Namespace>();
                    Namespace ns = new Namespace();
                    ns.name = _namespaceName;
                    ns.value = _namespaceValue;
                    _namespaces.AddLast(ns);
                }
                else
                {
                    if (_elementPrefix == _namespaceName && _elementNamespaceUri != _namespaceValue)
                        _elementNamespaceUri = _namespaceValue;
                    if (NamespaceInheritanceMode == NamespaceInheritanceMode.Inherit ||
                            NamespaceManager.LookupNamespace(_namespaceName) != _namespaceValue)
                        NamespaceManager.AddNamespace(_namespaceName, _namespaceValue);
                }
                _namespaceName = null;
                _namespaceValue = null;
            }
            else
            {
                if (_normalize)
                {
                    if ((_attributeLocalName == "id" && _attributeNamespaceUri == XmlReservedNs.NsXml) ||
                        (SchemaInfo != null && SchemaInfo.SchemaType.TypeCode == XmlTypeCode.Id))
                        _attributeValue = XQueryFuncs.NormalizeSpace(_attributeValue);
                }
                if (NamespaceManager != null && _attributePrefix != "")
                {
                    string ns = NamespaceManager.LookupNamespace(_attributePrefix);
                    if (ns == null)
                    {
                        NamespaceManager.AddNamespace(_attributePrefix, _attributeNamespaceUri);
                        _elementHasNamespaces = true;
                    }
                    else if (ns != _attributeNamespaceUri)
                    {
                        _attributePrefix = NamespaceManager.LookupPrefix(_attributeNamespaceUri);
                        if (_attributePrefix == null)
                        {
                            _attributePrefix = GeneratePrefix();
                            NamespaceManager.AddNamespace(_attributePrefix, _attributeNamespaceUri);
                            _elementHasNamespaces = true;
                        }
                    }
                }
                Attribute atr = new Attribute();
                atr.prefix = _attributePrefix;
                atr.localName = _attributeLocalName;
                atr.namespaceUri = _attributeNamespaceUri;
                atr.value = _attributeValue;
                if (SchemaInfo != null && SchemaInfo.SchemaAttribute != null)
                    atr.schemaInfo = new DmSchemaInfo(SchemaInfo);
                if (_attributes == null)
                    _attributes = new LinkedList<Attribute>();
                _attributes.AddLast(atr);
                _attributePrefix = null;
                _attributeLocalName = null;
                _attributeNamespaceUri = null;
                _attributeValue = null;
            }            
            _state = BuilderState.BeginElement;
        }

        internal void CompleteElement()
        {
            CompleteElement(false);
        }

        private void CompleteElement(bool isElemEnd)
        {
            if (_state == BuilderState.BeginElement)
            {
                if (NamespaceManager != null)
                {
                    if (_elementPrefix != null)                        
                    {
                        String ns = NamespaceManager.LookupNamespace(_elementPrefix);
                        if (ns != _elementNamespaceUri)
                        {
                            NamespaceManager.AddNamespace(_elementPrefix, _elementNamespaceUri);
                            _elementHasNamespaces = true;
                        }
                    }
                    if (_stripNamespace && _elementHasNamespaces)
                    {
                        IDictionary<String, String> local = NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local);
                        foreach (KeyValuePair<String, String> ns in local)
                        {
                            bool found = (ns.Value == _elementNamespaceUri);
                            if (_attributes != null && !found)
                                foreach (Attribute atr in _attributes)
                                    if (atr.prefix == ns.Key)
                                    {
                                        found = true;
                                        break;
                                    }
                            if (!found)
                                NamespaceManager.RemoveNamespace(ns.Key, ns.Value);
                        }
                   }
                    if (NamespaceInheritanceMode == NamespaceInheritanceMode.Inherit)
                    {
                        IDictionary<String, String> allExcludeXml = NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml);
                        IDictionary<String, String> local = NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local);
                        foreach (KeyValuePair<string, string> kvp in allExcludeXml)
                        {
                            if (kvp.Key != "" && !local.ContainsKey(kvp.Key))
                            {
                                NamespaceManager.AddNamespace(kvp.Key, kvp.Value);
                                _elementHasNamespaces = true;
                            }
                        }
                    }
                    if (_elementHasNamespaces)
                    {
                        _namespaces = new LinkedList<Namespace>();
                        foreach (KeyValuePair<String, String> local in
                            NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.Local))
                        {
                            Namespace ns = new Namespace();
                            ns.name = local.Key;
                            ns.value = local.Value;
                            _namespaces.AddFirst(ns);
                        }
                    }
                }
                DmElement element = (DmElement)GetContainer().CreateChild(
                    new DmQName(_elementPrefix, _elementLocalName, _elementNamespaceUri, NameTable));
                element._builder_prior_pos = -1;
                element._builder_pos = Position;
                if (_namespaces != null)
                    element._builder_pos += _namespaces.Count;
                if (_attributes != null)
                    element._builder_pos += _attributes.Count;
                if (_namespaces != null)
                {
                    for (LinkedListNode<Namespace> listNode = _namespaces.Last; listNode != null; listNode = listNode.Previous)
                    {
                        Namespace ns = listNode.Value;
                        DmNamespace node = (DmNamespace)element.CreateNamespace(ns.name);
                        m_pageFile.AddNode(element._builder_pos, node, ns.value);
                    }
                    _namespaces = null;
                }
                if (_attributes != null)
                {
                    for (LinkedListNode<Attribute> listNode = _attributes.Last; listNode != null; listNode = listNode.Previous)
                    {
                        Attribute atr = listNode.Value;
                        DmQName name = new DmQName(atr.prefix, atr.localName, atr.namespaceUri, NameTable);
                        if (hs != null)
                        {
                            if (hs.Contains(name))
                                throw new XQueryException(Properties.Resources.XQDY0025,
                                    new XmlQualifiedName(element.LocalName, element.NamespaceURI),
                                    new XmlQualifiedName(atr.localName, atr.namespaceUri));
                            hs.Add(name);
                        }
                        DmAttribute node = (DmAttribute)element.CreateAttribute(name);
                        if (atr.schemaInfo != null)
                            node.SchemaInfo = atr.schemaInfo;
                        node.IndexNode(Position);
                        m_pageFile.AddNode(element._builder_pos, node, atr.value);
                    }
                    _attributes = null;
                    if (hs != null)
                        hs.Clear();
                }                
                UpdateParent();
                m_pageFile.AddNode(_parent._builder_pos, element, null);
                element.IndexNode(element._builder_pos);
                CloseChain();
                _parent = element;
                _elementPrefix = null;
                _elementLocalName = null;
                _elementNamespaceUri = null;
                _elementHasNamespaces = false;    
                _state = BuilderState.Element;
                PerformPostCompleteAction();
            }
        }

        public override void WriteStartDocument(bool standalone)
        {
            CheckState(BuilderState.BeginDocument);
            DocumentRoot.Standalone = standalone;
            DocumentRoot.IndexNode(Position);
            m_pageFile.AddNode(-1, DocumentRoot, null);
            m_pageFile[0] = 0;
            _parent._builder_pos = 0;
        }

        public override void WriteStartDocument()
        {
            WriteStartDocument(false);
        }

        public override void WriteEndDocument()
        {
            m_pageFile[0] = -1;
            if (_parent._builder_prior_pos != -1 && m_pageFile[_parent._builder_prior_pos] == 0)
                m_pageFile[_parent._builder_prior_pos] = -1;
            _last = null;
        }

        public bool IsEmptyElement { get; set; }
        internal int LastElementEnd { get; private set; }

        public override void WriteStartElement(string prefix, string localName, string namespaceUri)
        {
            if (prefix == null)
                prefix = String.Empty;
            if (localName == null)
                throw new ArgumentException("localName");
            if (namespaceUri == null)
                namespaceUri = String.Empty;
            CompleteElement(false);
            CheckState(BuilderState.BeginDocument | BuilderState.Element);
            _elementPrefix = prefix;
            _elementLocalName = localName;
            _elementNamespaceUri = namespaceUri;
            _state = BuilderState.BeginElement;
            if (NamespaceManager != null)
                NamespaceManager.PushScope();
            _last = null;
        }

        public override void WriteEndElement()
        {
            CompleteElement(true);
            if (SchemaInfo != null && SchemaInfo.SchemaElement != null)
                _parent.SchemaInfo = SchemaInfo;
            LastElementEnd = _parent._builder_pos;
            if (_parent._builder_prior_pos != -1 && m_pageFile[_parent._builder_prior_pos] == 0)
                m_pageFile[_parent._builder_prior_pos] = -1;
            _parent = _parent.ParentNode;
            if (NamespaceManager != null)
                NamespaceManager.PopScope();
            _last = null;
        }        

        public override WriteState WriteState
        {
            get 
            {
                switch (_state)
                {
                    case BuilderState.BeginDocument:
                        return WriteState.Prolog;
                    case BuilderState.BeginElement:
                    case BuilderState.Element:
                        return WriteState.Element;
                    case BuilderState.BeginAttribute:
                    case BuilderState.BeginNamespace:
                        return WriteState.Attribute;
                    default:
                        return WriteState.Error;
                }
            }
        }

        internal int Position
        {
            get
            {
                return m_pageFile.Count;
            }
        }

        public override void WriteString(string text)
        {
            switch (_state)
            {
                case BuilderState.BeginAttribute:
                    _attributeValue += text;
                    break;
                
                case BuilderState.BeginNamespace:
                    _namespaceValue += text;
                    break;

                default:
                    CompleteElement();
                    if (_last != null && _last is DmText)
                        m_pageFile.LastNodeAppendValue(text);
                    else
                    {
                        UpdateParent();
                        _last = (DmText)GetContainer().CreateChildText();
                        _last.IndexNode(Position);
                        m_pageFile.AddNode(_parent._builder_pos, _last, text);
                        CloseChain();
                    }
                    break;
            }
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new NotImplementedException();
        }

        public override void WriteWhitespace(string ws)
        {
            switch (_state)
            {
                case BuilderState.BeginAttribute:
                    _attributeValue += ws;
                    break;

                default:
                    CompleteElement();
                    if (_last != null && _last is DmWhitespace)
                        m_pageFile.LastNodeAppendValue(ws);
                    else
                    {
                        UpdateParent();
                        _last = (DmWhitespace)GetContainer().CreateChildWhitespace();
                        _last.IndexNode(Position);
                        m_pageFile.AddNode(_parent._builder_pos, _last, ws);
                        CloseChain();
                    }
                    break;
            }
        }

        public void WriteNode(XPathNavigator navigator, NamespacePreserveMode preserveMode, 
            ElementConstructionMode constructMode)
        {
            CompleteElement(false);
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

        public XQueryDocument Document
        {
            get { return m_document; }
        }

        public XmlNameTable NameTable { get; private set; }

        internal void AddCompleteElementAction(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (_postComplete == null)
                _postComplete = new List<Action>();
            _postComplete.Add(action);
        }

        private void PerformPostCompleteAction()
        {
            if (_postComplete != null)
            {
                foreach (Action action in _postComplete)
                    action();
                _postComplete = null;
            }
        }
    }
}
