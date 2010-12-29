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
using System.Data;

using DataEngine;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;


namespace DataEngine.Export
{
    public class AdoNetWriter : AbstractWriter
    {
        private string m_fileName;

        public AdoNetWriter(string fileName)
        {
            m_fileName = fileName;
        }

        public override void Write(Resultset rs)
        {
            DataTable dt = new DataTable();
            dt.TableName = "Result";
            foreach (RowType.TypeInfo ti in rs.RowType.Fields)
            {
                DataColumn column = new DataColumn();
                column.ColumnName = ti.Name;
                column.DataType = ti.DataType;
                dt.Columns.Add(column);
            }
            while (rs.Begin != null)
            {
                Row row = rs.Dequeue();
                DataRow dr = dt.NewRow();
                for (int k = 0; k < row.Length; k++)
                    dr[k] = row[k];
                dt.Rows.Add(dr);
                RowProceded();
            }
            dt.WriteXml(m_fileName, XmlWriteMode.WriteSchema);
        }
    }
}
