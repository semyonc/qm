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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using DataEngine.CoreServices;
using DataEngine.XQuery.DTD;
using DataEngine.XQuery.DocumentModel;

namespace DataEngine.XQuery
{
    public class XQueryDocument: IXPathNavigable
    {
        internal XmlNameTable nameTable;
        internal PageFile pagefile;        
        internal XmlReader input;
        internal DmRoot documentRoot;
        internal bool lookahead;

        internal XQueryDocumentBuilder builder = null;
        internal string baseUri = String.Empty;       
        internal object internalLockObject = new object();
        
        internal Dictionary<string, string> elemIdTable = null;
        internal Dictionary<string, int> IdTable = null;
        internal Dictionary<string, string[]> elemIdRefTable = null;

        internal int sequenceNumber;
        internal bool preserveSpace;
        internal static int s_docNumberSequence = 0;

        public static long LargeFileLength = 154533888;

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
        }

        public XQueryDocument(XmlNameTable nameTable)
        {
            sequenceNumber = s_docNumberSequence++;
            this.nameTable = nameTable;
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
            settings.ProhibitDtd = false;
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

        public void Open(Uri uri, XmlReaderSettings settings, XmlSpace space)
        {
            bool large = false;
            if (uri.Scheme == "file")
            {
                FileInfo fi = new FileInfo(uri.LocalPath);
                if (fi.Exists && fi.Length > LargeFileLength)
                    large = true;
            }
            pagefile = new PageFile(large);
            input = XmlReader.Create(uri.AbsoluteUri, settings);
            nameTable = input.NameTable;
            builder = new XQueryDocumentBuilder(this);
            builder.SchemaInfo = input.SchemaInfo;
            pagefile.HasSchemaInfo = (input.SchemaInfo != null);
            baseUri = input.BaseURI;
            preserveSpace = (space == XmlSpace.Preserve);
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
            lock (internalLockObject)
            {
                if (input != null)
                    input.Close();
                pagefile.Close();
            }
        }

        private void Read()
        {
#if PARALLEL
            XQueryNodeIterator.CheckThreadCanceled();
#endif
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
                                    if (IdTable == null)
                                        IdTable = new Dictionary<string, int>();
                                    if (!IdTable.ContainsKey(input.Value))
                                        IdTable.Add(input.Value, builder.Position);
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
                                        if (IdTable == null)
                                            IdTable = new Dictionary<string, int>();
                                        if (!IdTable.ContainsKey(input.Value))
                                            IdTable.Add(input.Value, builder.Position);
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
                input = null;
                builder = null;
            }
        }

        internal void ExpandPageFile(int pos)
        {
            lock (internalLockObject)
                while (pos >= pagefile.Count && input != null)
                    Read();
        }

        internal void ExpandUtilElementEnd(int pos)
        {
            lock (internalLockObject)
                while (input != null && pos != builder.LastElementEnd)
                    Read();
        }

        public void Fill()
        {
            lock (internalLockObject)
                while (input != null)
                    Read();
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
