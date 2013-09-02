//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Net;
using System.Diagnostics;

using System.Data.Common;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

using DataEngine;
using DataEngine.CoreServices;
using DataEngine.XQuery;
using DataEngine.XQuery.Util;
using DataEngine.XQuery.MS;

namespace XQueryConsole
{
    internal enum OpenXmlDocumentType
    {
        Unknown,
        PresentationDocument,
        SpreadsheetDocument,
        WordprocessingDocument
    }

    public class XQueryDsContext: XQueryContext
    {
        private DatabaseDictionary dict;
        private Dictionary<string, XQueryDocument> cache;
        private Dictionary<String, OpenXmlPackage> packages;


        public XQueryDsContext(DatabaseDictionary dict, XmlNameTable nameTable)
            : base(nameTable)
        {
            this.dict = dict;
            cache = new Dictionary<string, XQueryDocument>();
            packages = new Dictionary<String, OpenXmlPackage>();
        }

        public XQueryDsContext(DatabaseDictionary dict)
            : this(dict, new NameTable())
        {
        }

        public override void InitNamespaces()
        {
            base.InitNamespaces();
            if (NamespaceManager.LookupNamespace("oxml") == null)
                AddNamespace("oxml", "http://schemas.openxmlformats.org/package");
        }

        public IXPathNavigable OpenDataSource(string name)
        {
            lock (cache)
            {
                XQueryDocument doc;
                if (cache.TryGetValue(name, out doc))
                    return doc;
                string prefix;
                string[] identifierPart;
                Util.ParseCollectionName(name, out prefix, out identifierPart);
                DataSourceInfo dsi = dict.GetDataSource(prefix);
                if (dsi == null)
                {
                    String msg;
                    if (prefix == String.Empty)
                        msg = Properties.Resources.NoDefaultDs;
                    else
                        msg = String.Format(Properties.Resources.UnknownDsPrefix, prefix);
                    throw new XQueryException(msg, null);
                }
                DataProviderHelper helper = new DataProviderHelper(dsi);
                DbConnection connection = DataProviderHelper.CreateDbConnection(dsi.ProviderInvariantName, dsi.X86Connection);
                try
                {
                    connection.ConnectionString = dsi.ConnectionString;
                    connection.Open();
                    DbCommand command = connection.CreateCommand();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * FROM ");
                    sb.Append(GetProviderTableName(helper, identifierPart));
                    if (helper.OrderByColumnsInSelect)
                        sb.Append(" ORDER BY 1");
                    command.CommandText = sb.ToString();
                    DbDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    DbXmlReader xmlReader = new DbXmlReader(reader, GetRootName(identifierPart), GetSettings());
                    doc = new XQueryDocument(xmlReader);
                    cache.Add(name, doc);
                    return doc;
                }
                catch (Exception ex)
                {
                    connection.Dispose();
                    throw ex;
                }
           }
        }

        public XQueryNodeIterator HttpGet(string uri, NetworkCredential credential)
        {
            Stream stream;
            WebClient webClient = new WebClient();
            if (credential != null)
                webClient.Credentials = credential;
            webClient.Proxy.Credentials = CredentialCache.DefaultCredentials;
            webClient.Headers["Accept"] = "*/*";
            webClient.Headers["UserAgent"] = "Mozilla/4.0 (compatible; Win32; QueryMachine.XQuery)";
            try
            {
                stream = webClient.OpenRead(uri);
            }
            catch (Exception ex)
            {
                throw new XQueryException(ex.Message, ex);
            }
            string contentType = webClient.ResponseHeaders["Content-Type"];
            if (contentType == null)
                return EmptyIterator.Shared;
            if (contentType.StartsWith("application/xml") || contentType.StartsWith("text/xml"))
            {
                XQueryDocument doc = new XQueryDocument(stream, GetSettings());
                return new NodeIterator(new XPathItem[] { doc.CreateNavigator() });
            }
            else
            {
                XPathItem item = new XQueryItem(String.Format("{0};Content-Length={1}", contentType,
                    webClient.ResponseHeaders["Content-Length"]));
                return new NodeIterator(new XPathItem[] { item });
            }
        }

        public XQueryNodeIterator HttpPost(string uri, NetworkCredential credential, XQueryNodeIterator request)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            if (credential != null)
                webRequest.Credentials = credential;
            webRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml; Charset=UTF-8";
            webRequest.Accept = "*/*";
            webRequest.UserAgent = "Mozilla/4.0 (compatible; Win32; QueryMachine.XQuery)";
            webRequest.Timeout = 30000;
            XQueryNodeIterator iter = request.Clone();
            if (iter.MoveNext())
            {
                MemoryStream stream = new MemoryStream();
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    ConformanceLevel = ConformanceLevel.Auto,
                    Encoding = Encoding.UTF8
                };
                XmlWriter writer = XmlWriter.Create(stream, settings);
                writer.WriteStartDocument();
                foreach (XPathItem item in request)
                {
                    if (item.IsNode)
                    {
                        XPathNavigator nav = (XPathNavigator)item;
                        nav.WriteSubtree(writer);
                    }
                    else
                        writer.WriteString(item.Value);
                }
                writer.WriteEndDocument();
                writer.Close();
                byte[] buffer = stream.ToArray();
                webRequest.ContentLength = buffer.Length -3;
                Stream input = webRequest.GetRequestStream();
                input.Write(buffer, 3, buffer.Length -3);
                input.Close();
            }
            WebResponse webResponse = webRequest.GetResponse();
            string contentType = webResponse.ContentType;
            if (contentType.StartsWith("application/xml") || contentType.StartsWith("text/xml"))
            {
                XQueryDocument doc = new XQueryDocument(webResponse.GetResponseStream(), GetSettings());
                return new NodeIterator(new XPathItem[] { doc.CreateNavigator() });
            }
            else
            {
                XPathItem item = new XQueryItem(String.Format("{0};Content-Length={1}", contentType,
                    webResponse.ContentLength));
                return new NodeIterator(new XPathItem[] { item });
            }
        }

        public override void Close()
        {
            base.Close();
            foreach (XQueryDocument doc in cache.Values)
                doc.Close();
            foreach (OpenXmlPackage package in packages.Values)
                package.Close();
            cache.Clear();
            packages.Clear();
        }

        private String GetProviderTableName(DataProviderHelper helper, String[] identifierPart)
        {
            StringBuilder sb = new StringBuilder();
            string[] formattedIdentifiers = new String[identifierPart.Length];
            for (int k = 0; k < identifierPart.Length; k++)
                formattedIdentifiers[k] = helper.FormatIdentifier(identifierPart[k]);
            for (int k = 0; k < formattedIdentifiers.Length; k++)
            {
                if (k > 0)
                    sb.Append(helper.Qualifer);
                sb.Append(formattedIdentifiers[k]);
            }
            return sb.ToString();
        }

        private String GetRootName(String[] identifierPart)
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < identifierPart.Length; k++)
            {
                if (k > 0)
                    sb.Append(".");
                sb.Append(XmlConvert.EncodeLocalName(identifierPart[k]));
            }
            return sb.ToString();
        }

        public OpenXmlPackage OpenPackage(string fileName)
        {
            OpenXmlPackage package;
            if (!packages.TryGetValue(fileName, out package))
            {
                OpenXmlDocumentType docType = GetDocumentTypeByFileName(fileName);
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
                        throw new XQueryException(String.Format(Properties.Resources.FileTypeUnknown, fileName), null);
                }
                packages.Add(fileName, package);
            }
            return package;
        }

        public IXPathNavigable OpenPart(string fileName, Uri partUri)
        {
            XQueryDocument doc;
            string cacheKey = fileName;
            if (partUri != null)
                cacheKey = cacheKey + partUri.ToString();
            if (cache.TryGetValue(cacheKey, out doc))
                return doc;
            OpenXmlPackage package = OpenPackage(fileName);
            OpenXmlPart part = null;
            if (partUri == null)
            {
                if (package is PresentationDocument)
                    part = ((PresentationDocument)package).PresentationPart;
                else if (package is WordprocessingDocument)
                    part = ((WordprocessingDocument)package).MainDocumentPart;
                else if (package is SpreadsheetDocument)
                    part = ((SpreadsheetDocument)package).WorkbookPart;
            }
            else
                part = GetPartList(package).SingleOrDefault(p => p.Uri == partUri);
            if (part == null)
                throw new XQueryException(String.Format(Properties.Resources.PartNotFound, partUri, fileName), null);
            doc = CreateDocument();
            doc.Open(part.GetStream(), GetSettings(), XmlSpace.Default, Token);
            cache.Add(cacheKey, doc);
            return doc;
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

    public static class WmhFuncs
    {
        [XQuerySignature("ds", NamespaceUri = XmlReservedNs.NsWmhExt, Return = XmlTypeCode.Node, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetDataSource([Implict] Executive executive,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string name)
        {
            XQueryDsContext context = (XQueryDsContext)executive.Owner;
            IXPathNavigable doc = context.OpenDataSource(name);
            return doc.CreateNavigator();
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
            XQueryDsContext context = (XQueryDsContext)engine.Owner;
            string filename = context.GetFileName(name);
            if (filename == null)
                throw new XQueryException(String.Format(Properties.Resources.FileNotFound, name), null);
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
            XQueryDsContext context = (XQueryDsContext)engine.Owner;
            string filename = context.GetFileName(name);
            if (filename == null)
                throw new XQueryException(String.Format(Properties.Resources.FileNotFound, name), null);
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

        [XQuerySignature("http-get", NamespaceUri = XmlReservedNs.NsWmhExt, Return = XmlTypeCode.Item, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator HttpGet([Implict] Executive executive, 
            [XQueryParameter(XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)] AnyUriValue uri)
        {
            XQueryDsContext context = (XQueryDsContext)executive.Owner;
            return context.HttpGet(uri.Value, null);
        }

        [XQuerySignature("http-get", NamespaceUri = XmlReservedNs.NsWmhExt, Return = XmlTypeCode.Item, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator HttpGet([Implict] Executive executive,
            [XQueryParameter(XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)] AnyUriValue uri, String username, String password)
        {
            XQueryDsContext context = (XQueryDsContext)executive.Owner;
            return context.HttpGet(uri.Value, new NetworkCredential(username, password));
        }


        [XQuerySignature("http-post", NamespaceUri = XmlReservedNs.NsWmhExt, Return = XmlTypeCode.Item, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator HttpPost([Implict] Executive executive,
            [XQueryParameter(XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)] AnyUriValue uri,
            [XQueryParameter(XmlTypeCode.Item, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator request)
        {
            XQueryDsContext context = (XQueryDsContext)executive.Owner;
            return context.HttpPost(uri.Value, null, request);
        }

        [XQuerySignature("http-post", NamespaceUri = XmlReservedNs.NsWmhExt, Return = XmlTypeCode.Item, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator HttpPost([Implict] Executive executive,
            [XQueryParameter(XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)] AnyUriValue uri, String username, String password,
            [XQueryParameter(XmlTypeCode.Item, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator request)
        {
            XQueryDsContext context = (XQueryDsContext)executive.Owner;
            return context.HttpPost(uri.Value, new NetworkCredential(username, password), request);
        }
    }
}
