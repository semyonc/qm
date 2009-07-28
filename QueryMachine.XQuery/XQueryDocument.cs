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

namespace DataEngine.XQuery
{
    public class XQueryDocument: IXPathNavigable
    {
        internal XmlNameTable nameTable;
        internal XQueryNodeInfoTable nodeInfoTable;
        internal XQuerySchemaInfoTable schemaInfoTable;
        internal PageFile pagefile;        
        internal XmlReader input;
        internal bool documentStarted;

        internal XQueryDocumentBuilder builder = null;
        internal string baseUri = String.Empty;
        
        internal object internalLockObject = new object();

        public XQueryDocument()
        {
            nameTable = new NameTable();
            nodeInfoTable = new XQueryNodeInfoTable();
            schemaInfoTable = new XQuerySchemaInfoTable();
            pagefile = new PageFile(nodeInfoTable, schemaInfoTable);
        }

        internal XQueryDocument(XmlNameTable nameTable, 
            XQueryNodeInfoTable nodeInfoTable, XQuerySchemaInfoTable schemaInfoTable)
        {
            this.nameTable = nameTable;
            this.nodeInfoTable = nodeInfoTable;
            this.schemaInfoTable = schemaInfoTable;
            pagefile = new PageFile(nodeInfoTable, schemaInfoTable);
        }

        public XQueryDocument(Stream stream)
            : this()
        {
            input = XmlReader.Create(stream);
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
        }

        public XQueryDocument(XmlReader reader)
            : this()
        {
            this.input = reader;            
            nameTable = input.NameTable;
            builder = new XQueryDocumentBuilder(this);
            builder.SchemaInfo = input.SchemaInfo;
            baseUri = reader.BaseURI;

        }

        public void Open(Uri uri, XmlReaderSettings settings, XmlSpace space)
        {
            input = XmlReader.Create(uri.AbsoluteUri, settings);
            builder = new XQueryDocumentBuilder(this);
            builder.SchemaInfo = input.SchemaInfo;
            baseUri = input.BaseURI;
        }

        XPathNavigator IXPathNavigable.CreateNavigator()
        {
            return new XQueryNavigator(this);            
        }

        public XQueryNavigator CreateNavigator()
        {
            return new XQueryNavigator(this);
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
            if (input.Read())
                switch (input.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                        builder.WriteStartDocument();
                        documentStarted = true;
                        break;

                    case XmlNodeType.DocumentType:
                        if (!documentStarted)
                        {
                            builder.WriteStartDocument();
                            documentStarted = true;
                        }
                        break;

                    case XmlNodeType.Element:
                        if (!documentStarted)
                        {
                            builder.WriteStartDocument();
                            documentStarted = true;
                        }
                        builder.IsEmptyElement = input.IsEmptyElement;
                        builder.WriteStartElement(input.Prefix, input.LocalName, input.NamespaceURI);
                        while (input.MoveToNextAttribute())
                        {
                            builder.WriteStartAttribute(input.Prefix, input.LocalName, input.NamespaceURI);
                            builder.WriteString(input.Value);
                            builder.WriteEndAttribute();
                        }
                        builder.CompleteElement();
                        if (builder.IsEmptyElement)
                            builder.WriteEndElement();
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
                        //builder.WriteString(input.Value);
                        break;
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
    }
}
