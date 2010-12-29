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
using System.Data.Common;

using DataEngine;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine.Export
{
    public class AdoProviderWriter : AbstractWriter
    {
        private BatchMove m_batchMove;

        public AdoProviderWriter(BatchMove batchMove)
        {
            m_batchMove = batchMove;
        }

        public override void Write(Resultset rs)
        {
            DataProviderHelper helper =
                new DataProviderHelper(m_batchMove.ProviderInvariantName, m_batchMove.ConnectionString);

            DataTable dt = new DataTable();
            dt.TableName = m_batchMove.TableName;
            for (int k = 0; k < m_batchMove.FieldNames.Count; k++)
            {
                DataColumn column = new DataColumn();
                column.ColumnName = m_batchMove.FieldNames[k];
                column.DataType = rs.RowType.Fields[k].DataType;
                dt.Columns.Add(column);
            }

            DbDataAdapter adapter = m_batchMove.CreateDataAdapter();
            while (rs.Begin != null)
            {
                Row row = rs.Dequeue();
                DataRow dr = dt.NewRow();
                for (int k = 0; k < row.Length; k++)
                    dr[k] = row[k];
                dt.Rows.Add(dr);
                RowProceded();
            }
            adapter.Update(dt);
        }
    }
}
