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
using System.IO;

namespace DataEngine
{
    public abstract class BaseDataAccessor: QueryNode
    {
        public BaseDataAccessor()
        {
            _childs = new QueryNodeCollection(this);
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            Resultset rs1 = ChildNodes[0].Get(queryContext, parameters);
            if (rs1.Begin != null)
            {
                if (ChildNodes[0] is FlatFileAccessor &&
                                    ((FlatFileAccessor)ChildNodes[0]).MultiFile)
                {
                    EnumeratorProcessingContext context = new EnumeratorProcessingContext(null);
                    Resultset rs = new Resultset(RowType.CreateContainerType(typeof(Resultset)), context);
                    context.Iterator = NextFile(rs, rs1, queryContext);
                    return rs;
                }
                else
                {
                    string fileName = "";
                    Row row = rs1.Dequeue();
                    Stream stream = (Stream)row.GetObject(0);
                    if (!row.IsDbNull(1))
                        fileName = row.GetString(1);
                    return CreateResultset(stream, fileName, queryContext);
                }
            }
            else
                return new Resultset(RowType.CreateContainerType(typeof(Resultset)), null);
        }

        protected IEnumerator<Row> NextFile(Resultset rs, Resultset src, QueryContext queryContext)
        {
            while (src.Begin != null)
            {
                string fileName = "";
                Row row = src.Dequeue();
                Stream stream = (Stream)row.GetObject(0);
                if (!row.IsDbNull(1))
                    fileName = row.GetString(1);
                row = rs.NewRow();
                row.SetObject(0, CreateResultset(stream, fileName, queryContext));
                if (fileName != null)
                    row.SetString(1, fileName);
                yield return row;
            }
        }

        protected abstract Resultset CreateResultset(Stream stream, string fileName, QueryContext queryContext);
    }
}
