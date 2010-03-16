//        Copyright (c) 2009-2010, Semyon A. Chertkov (semyonc@gmail.com)
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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

using DataEngine.CoreServices;
using DataEngine.XQuery.Util;

namespace DataEngine.XQuery.OpenXML
{
    internal enum OpenXmlDocumentType
    {
        Unknown,
        PresentationDocument,
        SpreadsheetDocument,
        WordprocessingDocument
    }

    public class OpenXmlQueryCommand: XQueryCommand
    {
        protected class OpenXmlContext : WorkContext
        {
            private Dictionary<String, OpenXmlPackage> _packages;

            public OpenXmlContext(OpenXmlQueryCommand command, XmlNameTable nameTable)
                : base(command, nameTable)
            {
                _packages = new Dictionary<String, OpenXmlPackage>();
            }

            public override void InitNamespaces()
            {
                base.InitNamespaces();
                if (NamespaceManager.LookupNamespace("oxml") == null)
                    AddNamespace("oxml", "http://schemas.openxmlformats.org/package");
            }

            public override bool StringEquals(string s1, string s2)
            {
                return String.CompareOrdinal(s1, s2) == 0;
            }

            public override void Close()
            {
                base.Close();
                foreach (OpenXmlPackage package in _packages.Values)
                    package.Close();
            }

            public OpenXmlPackage OpenPackage(string fileName)
            {
                OpenXmlPackage package;
                if (!_packages.TryGetValue(fileName, out package))
                {
                    OpenXmlDocumentType docType = OpenXmlQueryCommand.GetDocumentTypeByFileName(fileName);
                    switch (docType)
                    {
                        case OpenXmlDocumentType.PresentationDocument:
                            package = PresentationDocument.Open(fileName, false);
                            break;
                        case OpenXmlDocumentType.SpreadsheetDocument:
                            package = SpreadsheetDocument.Open(fileName, false);
                            break;
                        case OpenXmlDocumentType.WordprocessingDocument:
                            package = WordprocessingDocument.Open(fileName, false);
                            break;
                        default:
                            throw new OpenXmlException(Properties.Resources.FileTypeUnknown, fileName);
                    }
                    _packages.Add(fileName, package);
                }
                return package;
            }

            public IXPathNavigable OpenPart(string fileName, Uri partUri)
            {
                OpenXmlPackage package = OpenPackage(fileName);
                if (partUri == null)
                {
                    if (package is PresentationDocument)
                        return new OpenXmlDocument(((PresentationDocument)package).PresentationPart, NameTable);
                    else if (package is WordprocessingDocument)
                        return new OpenXmlDocument(((WordprocessingDocument)package).MainDocumentPart, NameTable);
                    else if (package is SpreadsheetDocument)
                        return new OpenXmlDocument(((SpreadsheetDocument)package).WorkbookPart, NameTable);
                }
                else
                {
                    OpenXmlPart part = GetPartList(package).SingleOrDefault(p => p.Uri == partUri);
                    if (part != null)
                    {
                        if (part.RootElement == null)
                        { // Some parts has a null RootElement
                            Stream content = part.GetStream();
                            XQueryDocument res = new XQueryDocument(content, GetSettings());
                            res.Fill();
                            return res;
                        }
                        else
                            return new OpenXmlDocument(part, NameTable);
                    }                
                }
                throw new OpenXmlException(Properties.Resources.PartNotFound, partUri, fileName);
            }
            
            private void AddPart(HashSet<OpenXmlPart> partList, OpenXmlPart part)
            {   // see http://blogs.msdn.com/brian_jones/archive/2010/01/27/open-xml-sdk-code-behind-the-custom-xml-markup-detection-tool.aspx
                if (partList.Contains(part))
                    return;

                //only add parts that are xml based
                if (part.ContentType.EndsWith("+xml"))
                    partList.Add(part);

                foreach (IdPartPair p in part.Parts)
                    AddPart(partList, p.OpenXmlPart);
            }
 
            public List<OpenXmlPart> GetPartList(OpenXmlPackage package)
            {
                HashSet<OpenXmlPart> partList = new HashSet<OpenXmlPart>();
                foreach (IdPartPair p in package.Parts)
                    AddPart(partList, p.OpenXmlPart);
                return partList.ToList();
            }
        }

        public static class OpenXmlFuncs
        {
            static OpenXmlFuncs()
            {
                XQueryFunctionTable.Register(typeof(OpenXmlFuncs));
            }

            public static void Init()
            {
            }

            [XQuerySignature("doc", NamespaceUri = "http://schemas.openxmlformats.org/package",
                Return = XmlTypeCode.Node, Cardinality = XmlTypeCardinality.ZeroOrOne)]
            public static object GetOpenXmlDocument([Implict] Executive engine,
                [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string name)
            {
                return GetOpenXmlDocument(engine, name, new AnyUriValue(String.Empty));
            }

            [XQuerySignature("doc", NamespaceUri = "http://schemas.openxmlformats.org/package",
                Return = XmlTypeCode.Node, Cardinality = XmlTypeCardinality.ZeroOrOne)]
            public static object GetOpenXmlDocument([Implict] Executive engine,
                [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string name,
                [XQueryParameter(XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)] AnyUriValue partUri)
            {
                OpenXmlContext context = (OpenXmlContext)engine.Owner;
                string filename = context.GetFileName(name);
                if (filename == null)
                    throw new OpenXmlException(Properties.Resources.FileNotFound, name);
                IXPathNavigable doc;
                if (partUri.Value == String.Empty)
                    doc = context.OpenPart(filename, null);
                else
                    doc = context.OpenPart(filename, new Uri(partUri.Value, UriKind.Relative));
                return doc.CreateNavigator();
            }

            [XQuerySignature("doc-parts", NamespaceUri = "http://schemas.openxmlformats.org/package",
                Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrMore)]
            public static XQueryNodeIterator GetOpenXmlDocumentParts([Implict] Executive engine,
                [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string name)
            {
                OpenXmlContext context = (OpenXmlContext)engine.Owner;
                string filename = context.GetFileName(name);
                if (filename == null)
                    throw new OpenXmlException(Properties.Resources.FileNotFound, name);
                OpenXmlPackage package = context.OpenPackage(filename);
                List<OpenXmlPart> list = context.GetPartList(package);
                return new NodeIterator(EnumParts(list));
            }

            private static IEnumerable<XPathItem> EnumParts(List<OpenXmlPart> list)
            {
                XQueryItem currItem = new XQueryItem();
                foreach (OpenXmlPart part in list)
                {
                    currItem.RawValue = new AnyUriValue(part.Uri.ToString());
                    yield return currItem;
                }
            }
        }

        public OpenXmlQueryCommand()
            : this(new NameTable())
        {
        }

        public OpenXmlQueryCommand(XmlNameTable nameTable)
            : base((WorkContext)null)
        {
            OpenXmlFuncs.Init();
            m_context = new OpenXmlContext(this, nameTable);
        }

        #region Static
        private struct FileDesc
        {
            public readonly OpenXmlDocumentType docType;
            public readonly string fileExt;

            public FileDesc(OpenXmlDocumentType docType, string fileExt)
            {
                this.docType = docType;
                this.fileExt = fileExt;
            }
        }

        private static FileDesc[] s_knownDocuments =
            new FileDesc[] { 
                new FileDesc(OpenXmlDocumentType.PresentationDocument, ".ppam"),  
                new FileDesc(OpenXmlDocumentType.PresentationDocument, ".pptm"),
                new FileDesc(OpenXmlDocumentType.PresentationDocument, ".ppsm"),
                new FileDesc(OpenXmlDocumentType.PresentationDocument, ".potm"),
                new FileDesc(OpenXmlDocumentType.PresentationDocument, ".pptx"),
                new FileDesc(OpenXmlDocumentType.PresentationDocument, ".ppsx"),
                new FileDesc(OpenXmlDocumentType.PresentationDocument, ".potx"),
                new FileDesc(OpenXmlDocumentType.SpreadsheetDocument, ".xlam"),
                new FileDesc(OpenXmlDocumentType.SpreadsheetDocument, ".xltm"),
                new FileDesc(OpenXmlDocumentType.SpreadsheetDocument, ".xlsm"),
                new FileDesc(OpenXmlDocumentType.SpreadsheetDocument, ".xltx"),
                new FileDesc(OpenXmlDocumentType.SpreadsheetDocument, ".xlsx"),
                new FileDesc(OpenXmlDocumentType.WordprocessingDocument, ".docx"),
                new FileDesc(OpenXmlDocumentType.WordprocessingDocument, ".docm"),
                new FileDesc(OpenXmlDocumentType.WordprocessingDocument, ".dotm"),
                new FileDesc(OpenXmlDocumentType.WordprocessingDocument, ".dotx")
            };

        internal static OpenXmlDocumentType GetDocumentTypeByFileName(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            foreach (FileDesc desc in s_knownDocuments)
                if (desc.fileExt == ext)
                    return desc.docType;
            return OpenXmlDocumentType.Unknown;
        }

        #endregion
    }
}
