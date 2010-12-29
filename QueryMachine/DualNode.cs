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
    class DualNode: QueryNode
    {
        public DualNode()
            : base()
        {
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            DataTable dt = RowType.CreateSchemaTable();
            DataRow r = dt.NewRow();
            r["ColumnName"] = "dummy";
            r["ColumnOrdinal"] = 0;
            r["DataType"] = typeof(System.String);
            r["ColumnSize"] = 1;
            dt.Rows.Add(r);
            Resultset rs = new Resultset(new RowType(dt), null);
            Row row = rs.NewRow();
            row.SetString(0, "X");
            rs.Enqueue(row);
            return rs;
        }
    }
}
