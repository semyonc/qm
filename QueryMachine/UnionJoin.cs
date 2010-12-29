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

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    public class UnionJoin: QueryNode
    {
        public UnionJoin()
            : base()
        {
            _childs = new QueryNodeCollection(this);
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            Resultset rs1 = null;
            Resultset rs2 = null;

            Iterator.Invoke(new Action[] {
                () => rs1 = ChildNodes[0].Get(queryContext, parameters),
                () => rs2 = ChildNodes[1].Get(queryContext, parameters)
            });

            DataTable dt1 = rs1.RowType.GetSchemaTable();
            DataTable dt2 = rs2.RowType.GetSchemaTable();

            foreach (DataRow r2 in dt2.Select())
            {
                DataRow r1 = dt1.NewRow();
                r1.ItemArray = r2.ItemArray;
                dt1.Rows.Add(r1);
            }

            EnumeratorProcessingContext context = new EnumeratorProcessingContext(new Resultset[] { rs1, rs2 });
            Resultset rs = new Resultset(new RowType(dt1), context);
            context.Iterator = DataIterator(rs, rs1, rs2);
            return rs;
        }

        protected IEnumerator<Row> DataIterator(Resultset rs, Resultset src1, Resultset src2)
        {
            while (src1.Begin != null)
            {
                Row row1 = src1.Dequeue();
                Row row = rs.NewRow();
                for (int k = 0; k < row1.Length; k++)
                    row.SetObject(k, (Row)row1.GetObject(k));
                yield return row;
            }
            while (src2.Begin != null)
            {
                Row row2 = src2.Dequeue();
                Row row = rs.NewRow();
                for (int k = 0; k < row2.Length; k++)
                    row.SetObject(k + src1.RowType.Fields.Length, (Row)row2.GetObject(k));
                yield return row;
            }
        }
    }
}
