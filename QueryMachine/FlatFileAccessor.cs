//        Copyright (c) 2008 - 2012, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    public class FlatFileAccessor: QueryNode
    {
        public string FileName { get; private set; }
        public bool Compressed { get; set; }
        
        public bool MultiFile { get; private set; }

        public FlatFileAccessor(string fileName)
        {            
            FileName = fileName;
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            DataTable dt = RowType.CreateSchemaTable();
            DataRow r = dt.NewRow();
            r["ColumnName"] = "Stream";
            r["ColumnOrdinal"] = 0;
            r["DataType"] = typeof(System.Object);
            dt.Rows.Add(r);
            r = dt.NewRow();
            r["ColumnName"] = "FileName";
            r["ColumnOrdinal"] = 1;
            r["DataType"] = typeof(System.String);
            dt.Rows.Add(r);
            EnumeratorProcessingContext context = null;
            if (FileName.IndexOfAny(new char[] { '?', '*' }) != -1)
                context = new EnumeratorProcessingContext(null);
            Resultset rs = new Resultset(new RowType(dt), context);
            if (context == null)
            {
                if (Compressed)
                {
                    ZipInputStream zipStream = new ZipInputStream(
                        new FileStream(FileName, FileMode.Open, FileAccess.Read));
                    zipStream.IsStreamOwner = true;
                    ZipEntry entry = zipStream.GetNextEntry();
                    if (entry != null)
                    {
                        Row row = rs.NewRow();
                        row.SetObject(0, zipStream);
                        row.SetString(1, FileName);
                        rs.Enqueue(row);
                    }
                    else
                        zipStream.Close();
                }
                else
                {
                    Stream stm = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                    Row row = rs.NewRow();
                    row.SetObject(0, stm);
                    row.SetString(1, FileName);
                    rs.Enqueue(row);
                }
            }
            else
            {
                MultiFile = true;
                context.Iterator = SearchLoop(queryContext, rs);
            }
            return rs;
        }

        protected IEnumerator<Row> SearchLoop(QueryContext queryContext, Resultset rs)
        {
            bool found = false;
            string[] pathset = queryContext.DatabaseDictionary.SearchPath.Split(new char[] { ';' });
            HashSet<string> hs = new HashSet<string>();
            foreach (string baseDir in pathset)
            {
                string fileName = Path.Combine(baseDir, FileName);
                string filePath = Path.GetDirectoryName(fileName);
                DirectoryInfo di = new DirectoryInfo(filePath);
                foreach (FileInfo fi in di.GetFiles(Path.GetFileName(FileName)))
                {
                    if (hs.Contains(fi.FullName))
                        continue;
                    if (Compressed)
                    {
                        ZipInputStream zipStream = new ZipInputStream(
                            new FileStream(fi.FullName, FileMode.Open, FileAccess.Read));
                        zipStream.IsStreamOwner = true;
                        ZipEntry entry = zipStream.GetNextEntry();
                        if (entry != null)
                        {
                            Row row = rs.NewRow();
                            row.SetObject(0, zipStream);
                            row.SetString(1, fi.FullName);
                            found = true;
                            yield return row;
                        }
                        else
                            zipStream.Close();
                    }
                    else
                    {
                        Stream stm = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);
                        Row row = rs.NewRow();
                        row.SetObject(0, stm);
                        row.SetString(1, fi.FullName);
                        found = true;
                        yield return row;
                    }
                    hs.Add(fi.FullName);
                }
            }
            if (!found)
                throw new ESQLException(Properties.Resources.NoOneFileWasFound, FileName);
        }

#if DEBUG
        protected override void Dump(TextWriter w, int padding)
        {
            OutlineNode(w, padding);
            if (MultiFile)
                w.WriteLine("FlatFileAccessor {0} [MULTIFILE]", FileName);
            else
                w.WriteLine("FlatFileAccessor {0}", FileName);
        }
#endif

    }
}
