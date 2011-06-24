//        Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
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

using DataEngine.CoreServices.Data;
using DataEngine.XL;
using System.IO;


namespace DataEngine
{
    public class XLDataAccessor : QueryNode
    {
        private CellRef tableRef;
        private bool hasHeader;

        protected class XLDataContext : DemandProcessingContext
        {
            private WorksheetData worksheet;
            private int currentRow;
            private int endRow;
            private int startCol;
            private int endCol;

            public XLDataContext(WorksheetData worksheet,
                int startRow, int startCol, int endRow, int endCol)
                : base(null)
            {
                this.worksheet = worksheet;
                currentRow = startRow;
                this.endRow = endRow;
                this.startCol = startCol;
                this.endCol = endCol;
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                if (currentRow <= endRow)
                {
                    Row row = rs.NewRow();
                    WorksheetData.SparseRow sparseRow = worksheet.GetRow(currentRow++);
                    if (sparseRow != null)
                    {
                        int k = 0;
                        for (int s = startCol; s <= endCol; s++, k++)
                        {
                            object value = sparseRow[s];
                            if (value != null)
                                row.SetValue(k, value);
                        }
                    }
                    rs.Enqueue(row);
                    return true;
                }
                return false;
            }
        }

        public XLDataAccessor(DatabaseDictionary dict, string tableName, bool hasHeader)
            : base()
        {
            this.hasHeader = hasHeader;
            if (tableName.Contains("[") && tableName.Contains("]"))
            {
                tableRef = CellRef.Parse(tableName);
                if (tableRef == null)
                    throw new ESQLException(Properties.Resources.XlTableNameInvalid, tableName);
            }
            else
            {
                tableRef = new CellRef();
                tableRef.FileName = tableName;
            }
            tableRef.FileName = dict.GetFilePath(tableRef.FileName, "xlsx");
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            if (tableRef.FileName.IndexOfAny(new char[] { '?', '*' }) != -1)
            {
                EnumeratorProcessingContext context = new EnumeratorProcessingContext(null);
                Resultset rs = new Resultset(RowType.CreateContainerType(typeof(Resultset)), context);
                context.Iterator = NextFile(rs, queryContext);
                return rs;
            }
            else
                using (Loader loader = new Loader(tableRef.FileName))
                    return CreateResultset(loader, tableRef.WorksheetName);
        }

        private IEnumerator<Row> NextFile(Resultset rs, QueryContext queryContext)
        {
            bool found = false;
            string[] pathset = queryContext.DatabaseDictionary.SearchPath.Split(new char[] { ';' });
            HashSet<String> hs = new HashSet<string>();
            foreach (string baseDir in pathset)
            {
                string fileName = Path.Combine(baseDir, tableRef.FileName);
                string filePath = Path.GetDirectoryName(fileName);
                DirectoryInfo di = new DirectoryInfo(filePath);
                foreach (FileInfo fi in di.GetFiles(Path.GetFileName(tableRef.FileName)))
                {
                    if (hs.Contains(fi.FullName))
                        continue;
                    Row row = rs.NewRow();
                    using (Loader loader = new Loader(fi.FullName))
                    {
                        loader.MultiFile = true;
                        Resultset xrs = CreateResultset(loader, tableRef.WorksheetName);
                        if (xrs == null)
                            continue;
                        row.SetObject(0, xrs);
                    }
                    row.SetString(1, fi.FullName);
                    found = true;
                    yield return row;
                    hs.Add(fi.FullName);
                }
            }
            if (!found)
                throw new ESQLException(Properties.Resources.NoOneFileWasFound, tableRef.FileName);
        }

        private Resultset CreateResultset(Loader loader, string worksheetName)
        {
            WorksheetData worksheet = loader.Load(worksheetName);
            DataTable dt = RowType.CreateSchemaTable();
            int minRow = Math.Max(worksheet.MinRow, tableRef.R1);
            int maxRow = Math.Min(worksheet.MaxRow, tableRef.R2);
            int minCol = 0;
            int maxCol = 0;
            foreach (WorksheetData.SparseRow b in worksheet.GetRows())
            {
                int locMin = Math.Max(tableRef.C1, b.MinColumn);
                int locMax = Math.Min(tableRef.C2, b.MaxColumn);
                if (minCol == 0 || minCol > locMin)
                    minCol = locMin;
                if (maxCol == 0 || maxCol < locMax)
                    maxCol = locMax;
            }
            if (minCol > maxCol)
                return new Resultset(new RowType(dt), null);
            WorksheetData.SparseRow header = null;
            bool useHeader = false;
            if ((hasHeader || loader.HasHeader) &&
                minRow == tableRef.R1 && maxRow > minRow)
            {
                header = worksheet.GetRow(minRow);
                if (header.MinColumn <= minCol && maxCol <= header.MaxColumn)
                {   // Header row is not sparse and have only string cells
                    useHeader = true;
                    for (int s = minCol; s <= maxCol; s++)
                    {
                        object value = header[s];
                        if (value == null ||
                            value.GetType() != typeof(System.String))
                        {
                            useHeader = false;
                            break;
                        }
                    }
                }
            }
            int sz = maxCol - minCol + 1;
            Type[] columnTypes = new Type[sz];
            int[] lenghts = new int[sz];
            worksheet.ForEach(useHeader ? minRow + 1 : minRow, minCol, maxRow, maxCol,
                (r, c, value) =>
                {
                    int index = c - minCol;
                    Type valueType = value.GetType();
                    if (columnTypes[index] == null)
                        columnTypes[index] = valueType;
                    else
                        if (columnTypes[index] != valueType)
                            columnTypes[index] = typeof(System.Object);
                    if (columnTypes[index] == typeof(System.String))
                    {
                        String strValue = (string)value;
                        if (lenghts[index] < strValue.Length)
                            lenghts[index] = strValue.Length;
                    }
                    return true;
                });
            int k = 0;
            for (int j = minCol; j <= maxCol; j++)
            {
                DataRow dr = dt.NewRow();
                if (useHeader)
                    dr["ColumnName"] = (string)header[j];
                else
                    dr["ColumnName"] = CellRef.GetColumnLiteralName(j);
                Type columnType = columnTypes[j - minCol];
                if (columnType != null)
                {
                    dr["DataType"] = columnType;
                    if (columnType == typeof(System.String))
                        dr["ColumnSize"] = lenghts[j - minCol];
                }
                else
                    dr["DataType"] = typeof(System.Object);
                dr["ColumnOrdinal"] = k++;
                dt.Rows.Add(dr);
            }
            return new Resultset(new RowType(dt),
                new XLDataContext(worksheet, useHeader ? minRow + 1 : minRow, minCol, maxRow, maxCol));
        }
    }
}
