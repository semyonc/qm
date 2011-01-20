//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using System.Xml;
using System.Xml.XPath;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;
using DataEngine;

namespace DataEngine.Export
{
    public class XmlFileWriter : AbstractWriter
    {
        private XmlWriter _writer;

        public XmlFileWriter(string fileName)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter _writer = XmlWriter.Create(fileName, settings);
        }

        public XmlFileWriter(XmlWriter writer)
        {
            _writer = writer;
        }

        public override void Write(Resultset rs)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            if (rs.RowType.Fields.Length == 1 &&
                rs.Begin != null && rs.NextRow(rs.Begin) == null)
            {
                object obj = rs.Begin.GetValue(0);
                if (obj is XmlDocument)
                {
                    XmlDocument document = (XmlDocument)obj;
                    document.Save(_writer);
                }
                else if (obj is XmlNode)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.InsertBefore(doc.CreateXmlDeclaration("1.0", "utf-8", null),
                        doc.FirstChild);
                    XmlNode node = doc.ImportNode((XmlNode)obj, true);
                    if (node.NodeType == XmlNodeType.Element)
                        doc.AppendChild(node);
                    else
                    {
                        XmlElement root = doc.CreateElement("result");
                        root.AppendChild(node);
                    }
                    doc.Save(_writer);
                }
                else if (obj is XPathNavigator)
                {
                    XPathNavigator nav = (XPathNavigator)obj;
                    nav.WriteSubtree(_writer);
                }
                else
                {
                    _writer.WriteStartElement("result");
                    WriteRootResultset(rs, _writer);
                    _writer.WriteEndElement();
                }
            }
            else
            {
                _writer.WriteStartElement("result");
                WriteRootResultset(rs, _writer);
                _writer.WriteEndElement();
            }
            _writer.Close();
        }

        private void WriteRootResultset(Resultset rs, XmlWriter w)
        {
            while (rs.Begin != null)
            {
                Row row = rs.Dequeue();
                WriteRow(rs, row, w);
                RowProceded();
            }
        }
        private void WriteNestedResultset(Resultset rs, XmlWriter w)
        {
            foreach (Row row in rs)
                WriteRow(rs, row, w);
        }

        private void WriteRow(Resultset rs, Row row, XmlWriter w)
        {
            w.WriteStartElement("row");
            foreach (RowType.TypeInfo ti in rs.RowType.Fields)
            {
                w.WriteStartElement(XmlConvert.EncodeLocalName(ti.Name));
                object value = row.GetValue(ti.Ordinal);
                if (value != DBNull.Value && value != null)
                {
                    if (value is XmlNode)
                    {
                        XmlNode node = (XmlNode)value;
                        node.WriteTo(w);
                    }
                    else if (value is XPathItem)
                    {
                        XPathNavigator nav = value as XPathNavigator;
                        if (nav != null)
                            nav.WriteSubtree(w);
                        else
                        {
                            XPathItem item = (XPathItem)value;
                            w.WriteString(item.Value);
                        }
                    }
                    else if (value is XmlNodeList)
                    {
                        XmlNodeList nodeList = (XmlNodeList)value;
                        foreach (XmlNode node in nodeList)
                            node.WriteTo(w);
                    }
                    else if (value is Array)
                    {
                        Array array = (Array)value;
                        BinaryFormatter formatter = new BinaryFormatter();
                        MemoryStream ms = new MemoryStream();
                        formatter.Serialize(ms, array);
                        w.WriteBase64(ms.GetBuffer(), 0, (int)ms.Length);
                        ms.Close();
                    }
                    else if (value is Resultset)
                    {
                        Resultset nested_rs = (Resultset)value;
                        WriteNestedResultset(nested_rs, w);
                    }
                    else
                        w.WriteValue(XmlDataAccessor.Serialize(value));
                }
                w.WriteEndElement();
            }
            w.WriteEndElement();
        }
    }
}
