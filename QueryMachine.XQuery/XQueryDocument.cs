//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using DataEngine.CoreServices;
using DataEngine.XQuery.DTD;
using DataEngine.XQuery.DocumentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DataEngine.XQuery
{
    public class XQueryDocument: IXPathNavigable
    {
        internal const uint DYN_DOCUMENT = 0x1;

        internal XmlReader input;
        internal XmlNameTable nameTable;
        internal PageFile pagefile;                
        internal DmRoot documentRoot;
        internal bool lookahead;
        internal TypedValueCache valueCache;

        internal XQueryDocumentBuilder builder = null;
        internal string baseUri = String.Empty;
        
        internal Dictionary<string, string> elemIdTable = null;
        internal Dictionary<string, int> IdTable = null;
        internal Dictionary<string, string[]> elemIdRefTable = null;

        internal int sequenceNumber;
        internal bool preserveSpace;
        internal static int s_docNumberSequence = 0;
        internal uint flags;

        private CancellationToken token;

        public bool IsIndexed
        {
            get
            {
                return documentRoot != null && builder == null;
            }
        }

        public XQueryDocument()
        {
            sequenceNumber = s_docNumberSequence++;
            nameTable = new NameTable();
            valueCache = new TypedValueCache(XQueryLimits.DefaultValueCacheSize);
        }

        public XQueryDocument(XmlNameTable nameTable)
        {
            sequenceNumber = s_docNumberSequence++;
            this.nameTable = nameTable;
            valueCache = new TypedValueCache(XQueryLimits.DefaultValueCacheSize);
        }

        public XQueryDocument(Stream stream)
            : this()
        {
            input = XmlReader.Create(stream);
            builder = new XQueryDocumentBuilder(this);
        }

        public XQueryDocument(Stream stream, XmlReaderSettings settings)
            : this()
        {
            input = XmlReader.Create(stream, settings);
            builder = new XQueryDocumentBuilder(this);
        }


        public XQueryDocument(TextReader textReader)
            : this()
        {
            input = XmlReader.Create(textReader);
            builder = new XQueryDocumentBuilder(this);
        }

        public XQueryDocument(string uri)    
            : this(uri, XmlSpace.Default)
        {
        }

        public XQueryDocument(string uri, XmlSpace space)
            : this()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = space != XmlSpace.Preserve;
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.XmlResolver = null;
            settings.NameTable = nameTable;
            input = XmlReader.Create(uri, settings);
            builder = new XQueryDocumentBuilder(this);
            builder.SchemaInfo = input.SchemaInfo;
            baseUri = input.BaseURI;
            preserveSpace = (space == XmlSpace.Preserve);
        }


        public XQueryDocument(XmlReader reader, XmlSpace space)
            : this()
        {
            this.input = reader;            
            nameTable = input.NameTable;
            builder = new XQueryDocumentBuilder(this);
            builder.SchemaInfo = input.SchemaInfo;
            baseUri = reader.BaseURI;
            preserveSpace = (space == XmlSpace.Preserve);
        }

        public XQueryDocument(XmlReader reader)
            : this(reader, XmlSpace.Default)
        {
        }

        public void Open(Uri uri, XmlReaderSettings settings, XmlSpace space, CancellationToken token)
        {
            bool large = false;
            if (uri.Scheme == "file")
            {
                FileInfo fi = new FileInfo(uri.LocalPath);
                if (fi.Exists)
                {
                    if (fi.Length > XQueryLimits.LargeFileLength)
                        large = true;
                    int valueCacheSize = (int)Math.Round(fi.Length * 0.0003);
                    valueCache = new TypedValueCache(Math.Max(valueCacheSize,
                        XQueryLimits.DefaultValueCacheSize));
                }
            }
            pagefile = new PageFile(large);
            input = XmlReader.Create(uri.AbsoluteUri, settings);
            nameTable = input.NameTable;
            builder = new XQueryDocumentBuilder(this);
            builder.SchemaInfo = input.SchemaInfo;
            pagefile.HasSchemaInfo = (input.SchemaInfo != null);
            baseUri = input.BaseURI;
            preserveSpace = (space == XmlSpace.Preserve);
            this.token = token;
        }

        public void Open(Stream stream, XmlReaderSettings settings, XmlSpace space, CancellationToken token)
        {
            pagefile = new PageFile(false);
            input = XmlReader.Create(stream, settings);
            nameTable = input.NameTable;
            builder = new XQueryDocumentBuilder(this);
            builder.SchemaInfo = input.SchemaInfo;
            pagefile.HasSchemaInfo = (input.SchemaInfo != null);
            baseUri = input.BaseURI;
            preserveSpace = (space == XmlSpace.Preserve);
            this.token = token;
        }


        XPathNavigator IXPathNavigable.CreateNavigator()
        {
            if (pagefile == null)
                throw new InvalidOperationException();
            return new XQueryNavigator(this);            
        }

        public XQueryNavigator CreateNavigator()
        {
            return new XQueryNavigator(this);
        }

        public override int GetHashCode()
        {
            return sequenceNumber;
        }

        public void Close()
        {
            if (input != null)
                input.Close();
            pagefile.Close();
        }

        private void Read()
        {
            token.ThrowIfCancellationRequested();
            if (lookahead || input.Read())
            {
                if (documentRoot == null)
                {
                    builder.WriteStartDocument();
                    documentRoot = builder.DocumentRoot;
                }
                lookahead = false;
                switch (input.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                        break;

                    case XmlNodeType.DocumentType:
                        try
                        {
                            object documentType;
                            string value = input.Value;
                            if (value == "")
                                documentType = DTDParser.ParseExternal(input.GetAttribute("PUBLIC"), 
                                    input.GetAttribute("SYSTEM"), baseUri);
                            else
                                documentType = DTDParser.ParseInline(value, baseUri);
                            CreateIdTable(documentType);
                        }
                        catch (XQueryException)
                        {
                        }
                        break;

                    case XmlNodeType.Element:
                        builder.IsEmptyElement = input.IsEmptyElement;
                        builder.WriteStartElement(input.Prefix, input.LocalName, input.NamespaceURI);
                        string elemName = input.Name;
                        while (input.MoveToNextAttribute())
                        {
                            builder.WriteStartAttribute(input.Prefix, input.LocalName, input.NamespaceURI);
                            builder.WriteString(input.Value);
                            builder.WriteEndAttribute();
                            if (elemIdTable != null)
                            {
                                string name;
                                if (elemIdTable.TryGetValue(elemName, out name) &&
                                    name == input.Name)
                                {
                                    string id = input.Value;
                                    builder.AddCompleteElementAction(() =>  
                                    {
                                        if (IdTable == null)
                                            IdTable = new Dictionary<string, int>();
                                        if (!IdTable.ContainsKey(id))
                                            IdTable.Add(id, builder.Position -1);
                                    });
                                }
                            }
                            if (pagefile.HasSchemaInfo)
                            {
                                IXmlSchemaInfo schemaInfo = input.SchemaInfo;
                                if (schemaInfo != null && schemaInfo.SchemaType != null)
                                {
                                    XmlTypeCode typeCode = schemaInfo.SchemaType.TypeCode;
                                    if (typeCode == XmlTypeCode.Id)
                                    {
                                        string id = input.Value;
                                        builder.AddCompleteElementAction(() =>
                                        {
                                            if (IdTable == null)
                                                IdTable = new Dictionary<string, int>();
                                            if (!IdTable.ContainsKey(id))
                                                IdTable.Add(id, builder.Position -1);
                                        });
                                    }
                                }
                            }
                        }
                        if (builder.IsEmptyElement)
                            builder.WriteEndElement();
                        else
                        {
                            while (input.Read())
                            {
                                if (input.NodeType == XmlNodeType.Text)
                                    builder.WriteString(input.Value);
                                else if (input.NodeType == XmlNodeType.Whitespace)
                                {
                                    if (preserveSpace)
                                        builder.WriteString(input.Value);
                                }
                                else if (input.NodeType == XmlNodeType.EndElement)
                                {
                                    builder.WriteEndElement();
                                    break;
                                }
                                else
                                {
                                    builder.CompleteElement();
                                    lookahead = true;
                                    break;
                                }
                            }                            
                        }
                        break;

                    case XmlNodeType.EndElement:
                        builder.WriteEndElement();
                        break;

                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        builder.WriteString(input.Value);
                        break;

                    case XmlNodeType.EntityReference:
                        input.ResolveEntity();
                        break;

                    case XmlNodeType.ProcessingInstruction:
                        builder.WriteProcessingInstruction(input.LocalName, input.Value);
                        break;

                    case XmlNodeType.Comment:
                        builder.WriteComment(input.Value);
                        break;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        if (preserveSpace)
                            builder.WriteString(input.Value);
                        break;
                }
            }
            else
            {
                builder.WriteEndDocument();
                input.Close();
                Interlocked.Exchange(ref input, null);
                builder = null;
             }
        }

        internal void ExpandPageFile(int pos)
        {
            if (pos >= pagefile.Count && input != null)
            {
                lock (pagefile)
                {
                    while (pos >= pagefile.Count && input != null)
                        Read();
                }
            }
        }

        internal void ExpandUtilElementEnd(int pos)
        {
            lock (pagefile)
            {
                if (input != null && pagefile[pos] == 0)
                {
                    while (input != null && pagefile[pos] == 0)
                        Read();
                }
            }
        }

        internal void ExpandUtilElementEnd(int pos, int size)
        {
            lock (pagefile)
            {
                if (input != null && pagefile[pos] == 0 && size > 0)
                {
                    int count = pagefile.Count;
                    while (size > pagefile.Count - count && input != null && pagefile[pos] == 0)
                        Read();
                }
            }
        }

        public void Fill()
        {
            lock (pagefile)
            {
                while (input != null)
                    Read();
            }
        }

        private void CreateIdTable(object documentType)
        {
            for (object curr = documentType; curr != null; curr = Lisp.Cdr(curr))
            {
                object decl = Lisp.Car(curr);
                if (Lisp.IsFunctor(decl, DTDID.AttlistDecl))
                {
                    string name = Lisp.SArg1(decl);
                    List<string> idref = null;
                    for (object attr = Lisp.Arg2(decl); attr != null; attr = Lisp.Cdr(attr))
                    {
                        object attdecl = Lisp.Car(attr);
                        string attname = Lisp.SArg1(attdecl);
                        if (Lisp.Arg2(attdecl) == DTDID.ID)
                        {
                            if (elemIdTable == null)
                                elemIdTable = new Dictionary<string, string>();
                            elemIdTable[name] = attname;
                        }
                        else if (Lisp.Arg2(attdecl) == DTDID.IDREF ||
                            Lisp.Arg2(attdecl) == DTDID.IDREFS)
                        {
                            if (idref == null)
                                idref = new List<string>();
                            idref.Add(attname);
                        }
                    }
                    if (idref != null)
                    {
                        if (elemIdRefTable == null)
                            elemIdRefTable = new Dictionary<string, string[]>();
                        elemIdRefTable[name] = idref.ToArray();
                    }
                }
            }
        }
    }
}
