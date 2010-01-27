using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System.IO;

using DataEngine.CoreServices;
using DataEngine.XQuery;
using System.Diagnostics;

namespace SimpleTestConsole
{
    public class OpenXML
    {

        [XQuerySignature("doc", NamespaceUri = "http://www.wmhelp.com/ext",
            Return = XmlTypeCode.Node, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetOpenXmlDocument([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string name)
        {
            return GetOpenXmlDocument(engine, name, "");
        }

        [XQuerySignature("doc", NamespaceUri="http://www.wmhelp.com/ext", 
            Return = XmlTypeCode.Node, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetOpenXmlDocument([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.String, Cardinality=XmlTypeCardinality.ZeroOrOne)] string name,
            [XQueryParameter(XmlTypeCode.String, Cardinality=XmlTypeCardinality.ZeroOrOne)] string id)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            string filename = context.GetFileName(name);
            if (filename == null)
                throw new Exception(String.Format("File {0} is not found", name));
            string ext = Path.GetExtension(filename);
            ContentKey key = new ContentKey(name, id);
            XQueryDocument doc;
            if (context.ExtraProperties.ContainsKey(key))
                doc = (XQueryDocument)context.ExtraProperties[key];
            else
            {
                if (ext == ".docx")
                    doc = OpenDocx(context, filename, id);
                else if (ext == ".xlsx")
                    doc = OpenXlsx(context, filename, id);
                else
                    throw new Exception(String.Format("File {0} is unknown type. Supported .docx & .xlsx file types", name));
                context.ExtraProperties.Add(key, doc);
            }            
            return doc.CreateNavigator();
        }

        private static XQueryDocument OpenDocx(XQueryContext context, string filename, string id)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filename, false))
            {
                OpenXmlPart part;
                if (id == "")
                    part = doc.MainDocumentPart;
                else
                    part = doc.GetPartById(id);
                Stream content = part.GetStream();
                XQueryDocument res = new XQueryDocument(content, context.GetSettings());
                res.Fill();
                doc.Close();
                return res;
            }
        }

        private static XQueryDocument OpenXlsx(XQueryContext context, string filename, string id)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filename, false))
            {
                OpenXmlPart part;
                if (id == "")
                    part = doc.WorkbookPart;
                else
                    part = doc.WorkbookPart.GetPartById(id);
                Stream content = part.GetStream();
                XQueryDocument res = new XQueryDocument(content, context.GetSettings());
                res.Fill();
                doc.Close();
                return res;
            }
        }

        private class ContentKey
        {
            public string FileName { get; private set; }

            public string PartID { get; private set; }

            public ContentKey(string filename, string id)
            {
                FileName = filename;
                PartID = id;
            }

            public override bool Equals(object obj)
            {
                ContentKey other = obj as ContentKey;
                if (obj != null)
                    return other.FileName == FileName &&
                        other.PartID == PartID;
                return false;
            }

            public override int GetHashCode()
            {
                return (FileName.GetHashCode() << 6) ^ PartID.GetHashCode();
            }
        }
    }
}
