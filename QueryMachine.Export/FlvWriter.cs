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
    public class FlvWriter : AbstractWriter
    {
        private string m_fileName;

        public FlvWriter(string fileName, bool createSchemaIni)
        {
            m_fileName = fileName;
            CreateSchemaIni = createSchemaIni;
        }

        public override void Write(Resultset rs)
        {
            int[] width = new int[rs.RowType.Fields.Length];
            foreach (Row row in rs)
            {
                for (int k = 0; k < row.Length; k++)
                {
                    String data = "";
                    Object value = row.GetValue(k);
                    if (value != DBNull.Value)
                        data = value.ToString().TrimEnd();
                    if (width[k] < data.Length)
                        width[k] = data.Length;
                }
            }
            if (CreateSchemaIni)
            {
                string iniFileName = Path.Combine(Path.GetDirectoryName(m_fileName), "schema.ini");
                string sectionName = Path.GetFileName(m_fileName);
                IniFile schema = new IniFile(iniFileName);
                schema.RemoveSection(sectionName);
                schema.SetValue(sectionName, "Format", "FixedLength");
                schema.SetValue(sectionName, "ColNameHeader", true);
                if (ExportNlsSettings)
                    TextDataAccessor.ExportTextDataFormat(schema, sectionName,
                        TextDataAccessor.GetCurrentTextDataFormat());
                for (int k = 0; k < rs.RowType.Fields.Length; k++)
                {
                    RowType.TypeInfo ti = rs.RowType.Fields[k];
                    schema.SetValue(sectionName, String.Format("Col{0}", k + 1),
                       String.Format("{0} {1} Width {2}", ti.Name,
                            TextDataAccessor.GetSchemaDataType(ti.DataType), width[k]));
                }
            }
            TextWriter sw = new StreamWriter(m_fileName, false, Encoding.Default);
            for (int k = 0; k < rs.RowType.Fields.Length; k++)
            {
                string name = Util.UnquoteName(rs.RowType.Fields[k].Name);
                if (name.Length < width[k])
                    sw.Write(name.PadRight(width[k]));
                else
                    sw.Write(name.Substring(0, width[k]));
            }
            sw.WriteLine();
            while (rs.Begin != null)
            {
                Row row = rs.Dequeue();
                for (int k = 0; k < row.Length; k++)
                {
                    String data = "";
                    Object value = row.GetValue(k);
                    if (value != DBNull.Value)
                        data = value.ToString().TrimEnd();
                    if (data.Length < width[k])
                        sw.Write(data.PadRight(width[k]));
                    else
                        sw.Write(data.Substring(0, width[k]));
                }
                sw.WriteLine();
                RowProceded();
            }
            sw.Close();
        }

        public bool CreateSchemaIni { get; set; }

        public bool ExportNlsSettings { get; set; }
    }
}
