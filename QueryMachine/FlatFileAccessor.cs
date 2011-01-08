/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2009  Semyon A. Chertkov (semyonc@gmail.com)

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    public class FlatFileAccessor: QueryNode
    {
        public string FileName { get; set; }
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
            EnumeratorProcessingContext context = null;
            if (FileName.IndexOfAny(new char[] { '?', '*' }) != -1)
                context = new EnumeratorProcessingContext(null);
            Resultset rs = new Resultset(new RowType(dt), context);
            if (context == null)
            {
                Stream stm = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                Row row = rs.NewRow();
                row.SetObject(0, stm);
                rs.Enqueue(row);
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
            foreach (string baseDir in pathset)
            {
                string fileName = Path.Combine(baseDir, FileName);
                string filePath = Path.GetDirectoryName(fileName);
                DirectoryInfo di = new DirectoryInfo(filePath);
                foreach (FileInfo fi in di.GetFiles(Path.GetFileName(FileName)))
                {
                    Stream stm = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);
                    Row row = rs.NewRow();
                    row.SetObject(0, stm);
                    found = true;
                    yield return row;
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
