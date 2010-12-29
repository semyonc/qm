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
    public class CsvWriter : AbstractWriter
    {
        private string m_fileName;

        public CsvWriter(string fileName, string commaSeparator, bool createSchemaIni)
        {
            m_fileName = fileName;
            CommaSeparator = commaSeparator;
            CreateSchemaIni = createSchemaIni;
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
            for (int k = 0; k < rs.RowType.Fields.Length; k++)
            {
                if (k > 0)
                    sw.Write(CommaSeparator);
                sw.Write(Util.UnquoteName(rs.RowType.Fields[k].Name));
            }
            sw.WriteLine();
            while (rs.Begin != null)
            {
                Row row = rs.Dequeue();
                for (int k = 0; k < row.Length; k++)
                {
                    if (k > 0)
                        sw.Write(CommaSeparator);
                    Object value = row.GetValue(k);
                    if (value != DBNull.Value)
                    {
                        String data = value.ToString();
                        if (data.Contains(CommaSeparator) || data.Contains("\""))
                            data = String.Format("\"{0}\"", data.Replace("\"", "\"\""));
                        sw.Write(data);
                    }
                }
                sw.WriteLine();
                RowProceded();
            }
            sw.Close();
        }    
        
        public String CommaSeparator { get; set; }

        public bool CreateSchemaIni { get; set; }

        public bool ExportNlsSettings { get; set; }
    }
}
