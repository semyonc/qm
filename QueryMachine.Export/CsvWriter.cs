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
using System.IO;

using DataEngine;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine.Export
{
    public enum CsvWriterQuotePolicy
    {
        Auto,
        Always,
        Literal
    }

    public class CsvWriter : AbstractWriter
    {
        private string m_fileName;

        public CsvWriterQuotePolicy QuotePolicy { get; set; }

        public CsvWriter(string fileName, string commaSeparator, bool createSchemaIni)
        {
            m_fileName = fileName;
            CommaSeparator = commaSeparator;
            CreateSchemaIni = createSchemaIni;
            QuotePolicy = CsvWriterQuotePolicy.Auto;
        }

        public override void Write(Resultset rs)
        {
            if (CreateSchemaIni)
            {
                string iniFileName = Path.Combine(Path.GetDirectoryName(m_fileName), "schema.ini");
                string sectionName = Path.GetFileName(m_fileName);
                IniFile schema = new IniFile(iniFileName);
                schema.RemoveSection(sectionName);
                if (CommaSeparator != "\t")
                    schema.SetValue(sectionName, "Format", String.Format("Delimited({0})", CommaSeparator));
                else
                    schema.SetValue(sectionName, "Format", "TabDelimited");
                schema.SetValue(sectionName, "ColNameHeader", true);
                if (ExportNlsSettings)
                    TextDataAccessor.ExportTextDataFormat(schema, sectionName,
                        TextDataAccessor.GetCurrentTextDataFormat());
                int n = 1;
                foreach (RowType.TypeInfo ti in rs.RowType.Fields)
                {
                    if (ti.DataType == typeof(String) && ti.Size > 0)
                        schema.SetValue(sectionName, String.Format("Col{0}", n++),
                           String.Format("{0} Text Width {1}", ti.Name, ti.Size));
                    else
                        schema.SetValue(sectionName, String.Format("Col{0}", n++),
                            String.Format("{0} {1}", ti.Name, TextDataAccessor.GetSchemaDataType(ti.DataType)));
                }
            }
            TextWriter sw = new StreamWriter(m_fileName, false, Encoding.Default);
            try
            {
                for (int k = 0; k < rs.RowType.Fields.Length; k++)
                {
                    if (k > 0)
                        sw.Write(CommaSeparator);
                    sw.Write(Util.UnquoteName(rs.RowType.Fields[k].Name));
                }
                sw.WriteLine();
                RowType.TypeInfo[] fields = rs.RowType.Fields;
                while (rs.Begin != null)
                {
                    Row row = rs.Dequeue();
                    for (int k = 0; k < row.Length; k++)
                    {
                        if (k > 0)
                            sw.Write(CommaSeparator);
                        WriteValue(sw, fields[k], row.GetValue(k));
                    }
                    sw.WriteLine();
                    RowProceded();
                }
            }
            finally
            {
                sw.Close();
            }
        }

        private bool NeedQuotes(String data, RowType.TypeInfo ti)
        {
            switch (QuotePolicy)
            {
                case CsvWriterQuotePolicy.Auto:
                    if (data.Contains(CommaSeparator) || data.Contains("\""))
                        return true;
                    break;

                case CsvWriterQuotePolicy.Always:
                    return true;

                case CsvWriterQuotePolicy.Literal:
                    switch (Type.GetTypeCode(ti.DataType))
                    {
                        case TypeCode.Decimal:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Byte:
                        case TypeCode.SByte:
                            return false;

                        default:
                            return true;
                    }
            }
            return false;
        }

        protected virtual void WriteValue(TextWriter sw, RowType.TypeInfo ti, object value)
        {
            if (value != DBNull.Value)
            {
                String data = FormatValue(ti, value);
                if (NeedQuotes(data, ti))
                    data = String.Format("\"{0}\"", data.Replace("\"", "\"\""));
                sw.Write(data);
            }
        }

        protected virtual string FormatValue(RowType.TypeInfo ti, object value)
        {
            return value.ToString();
        }
        
        public String CommaSeparator { get; set; }

        public bool CreateSchemaIni { get; set; }

        public bool ExportNlsSettings { get; set; }
    }
}
