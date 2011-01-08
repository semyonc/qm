//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
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
using System.Data.Common;

using DataEngine;
using DataEngine.CoreServices.Data;
using DataEngine.CoreServices;
using Data.Remote;
using Data.Remote.Proxy;

namespace DataEngine.Export
{
    public class BatchMove
    {
        public BatchMove()
        {
            FieldNames = new List<string>();
        }

        public bool IsTableExists()
        {
            DataProviderHelper helper = new DataProviderHelper(ProviderInvariantName, ConnectionString);
            using(DbConnection conn = CreateConnection())
            {
                conn.Open();
                DbCommand command = conn.CreateCommand();
                command.CommandText = String.Format("SELECT 1 FROM {0} WHERE 0=1", 
                    helper.FormatIdentifier(Util.SplitName(TableName)));
                try
                {
                    DbDataReader r = command.ExecuteReader();
                    r.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public void DropTable()
        {
            DataProviderHelper helper = new DataProviderHelper(ProviderInvariantName, ConnectionString);
            using (DbConnection conn = CreateConnection())
            {
                conn.Open();
                DbCommand command = conn.CreateCommand();
                command.CommandText = String.Format("DROP TABLE {0}", 
                    helper.FormatIdentifier(Util.SplitName(TableName)));
                command.ExecuteNonQuery();
            }
        }

        public void CleanTable()
        {
            DataProviderHelper helper = new DataProviderHelper(ProviderInvariantName, ConnectionString);
            using (DbConnection conn = CreateConnection())
            {
                conn.Open();
                DbCommand command = conn.CreateCommand();
                command.CommandText = String.Format("DELETE FROM {0}",
                    helper.FormatIdentifier(Util.SplitName(TableName)));
                command.ExecuteNonQuery();
            }
        }

        public String GetCreateTableDDL()
        {
            DataProviderHelper helper = new DataProviderHelper(ProviderInvariantName, ConnectionString);
            FieldNames.Clear();
            HasUnknownDatatype = false;
            RowType rowType = Source.RowType;
            int[] size = new int[rowType.Fields.Length];
            using (DbConnection conn = CreateConnection())
            {
                conn.Open();
                DataTable dt = conn.GetSchema("DataTypes");
                StringBuilder sb = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                StringBuilder sb3 = new StringBuilder();                
                for (int k = 0; k < rowType.Fields.Length; k++)
                {
                    if (k > 0)
                    {
                        sb.Append(",");
                        sb.AppendLine();
                    }
                    RowType.TypeInfo ti = rowType.Fields[k];
                    String name = helper.NormalizeIdentifier(FieldNames, Util.UnquoteName(ti.Name));
                    sb.Append(name);
                    sb.Append(" ");
                    if (ti.ExportAsPk)
                    {
                        if (sb2.Length > 0)
                            sb2.Append(", ");
                        sb2.Append(name);
                    }
                    if (ti.ExportAsUnique)
                    {
                        if (sb3.Length > 0)
                            sb3.Append(", ");
                        sb3.Append(name);
                    }
                    if (String.IsNullOrEmpty(ti.ExportDataType))
                    {
                        DataRow[] rows;
                        if (ti.DataType == typeof(String) ||
                            ti.DataType == typeof(byte[]))
                        {
                            size[k] = ti.Size;
                            if (size[k] == 0)
                            {
                                foreach (Row data in Source)
                                    if (!data.IsDbNull(k))
                                        size[k] = Math.Max(size[k], data.GetString(k).Length);
                            }
                            if (size[k] > 0)
                            {
                                rows = SelectDataType(dt, ti.DataType, size[k],
                                    new string[] { "size", "max length" }, true, false, false);
                                if (rows.Length == 0)
                                {
                                    rows = SelectDataType(dt, ti.DataType, size[k],
                                        new string[] { "size", "max length" }, false, false, false);
                                    if (rows.Length == 0)
                                    {
                                        rows = SelectDataType(dt, ti.DataType, size[k],
                                            new string[] { "size", "max length" }, null, false, null);
                                        if (rows.Length == 0)
                                        {
                                            rows = SelectDataType(dt, ti.DataType, null, null, true, true, null);
                                            if (rows.Length == 0)
                                                rows = SelectDataType(dt, ti.DataType, null, null, true, false, null);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                rows = SelectDataType(dt, ti.DataType, null, null, true, true, null);
                                if (rows.Length == 0)
                                    rows = SelectDataType(dt, ti.DataType, null, null, true, false, null);
                            }
                        }
                        else if (ti.DataType == typeof(Decimal))
                        {
                            rows = SelectDataType(dt, ti.DataType, null,
                                new string[] { "precision,scale" }, true, false, null);
                            if (rows.Length == 0)
                                rows = SelectDataType(dt, ti.DataType, null, null, false, false, null);
                        }
                        else if (ti.DataType == typeof(Double))
                        {
                            rows = SelectDataType(dt, ti.DataType, null,
                                new string[] { "precision,scale" }, true, false, null);
                            if (rows.Length == 0)
                            {
                                rows = SelectDataType(dt, ti.DataType, null,
                                    new string[] { "precision", "number of bits used to store the mantissa" }, true, null, null);
                                if (rows.Length == 0)
                                {
                                    rows = SelectDataType(dt, ti.DataType, null, null, null, null, null);
                                    if (rows.Length == 0)
                                    {
                                        rows = SelectDataType(dt, typeof(Decimal), null,
                                            new string[] { "precision,scale" }, true, false, null);
                                        if (rows.Length == 0)
                                            rows = SelectDataType(dt, typeof(Decimal), null, null, false, false, null);
                                    }
                                }
                            }
                        }
                        else if (ti.DataType == typeof(Single))
                        {
                            rows = SelectDataType(dt, ti.DataType, null,
                                new string[] { "precision,scale" }, true, false, null);
                            if (rows.Length == 0)
                            {
                                rows = SelectDataType(dt, ti.DataType, null,
                                    new string[] { "precision", "number of bits used to store the mantissa" }, true, null, null);
                                if (rows.Length == 0)
                                {
                                    rows = SelectDataType(dt, ti.DataType, null, null, null, null, null);
                                    if (rows.Length == 0)
                                    {
                                        rows = SelectDataType(dt, typeof(Decimal), null,
                                            new string[] { "precision,scale" }, true, false, null);
                                        if (rows.Length == 0)
                                            rows = SelectDataType(dt, typeof(Decimal), null, null, false, false, null);
                                    }
                                }
                            }
                        }
                        else if (ti.DataType == typeof(Int16) || 
                            ti.DataType == typeof(Int32) || ti.DataType == typeof(Int64))
                        {
                            rows = SelectDataType(dt, ti.DataType, null, new string[] { null }, true, null, null);
                            if (rows.Length == 0)
                            {
                                rows = SelectDataType(dt, ti.DataType, null, new string[] { null }, false, null, null);
                                if (rows.Length == 0)
                                {
                                    rows = SelectDataType(dt, typeof(Decimal), null,
                                        new string[] { "precision,scale" }, true, false, null);
                                    if (rows.Length == 0)
                                        rows = SelectDataType(dt, typeof(Decimal), null, null, false, false, null);
                                }
                            }
                        }
                        else
                        {
                            rows = SelectDataType(dt, ti.DataType, null, new string[] { null }, true, null, null);
                            if (rows.Length == 0)
                            {
                                rows = SelectDataType(dt, ti.DataType, null, new string[] { null }, false, null, null);
                                if (rows.Length == 0)
                                    rows = SelectDataType(dt, ti.DataType, null, null, null, null, null);
                            }
                        }
                        if (rows.Length == 0)
                        {
                            sb.Append("<Unknown>");
                            HasUnknownDatatype = true;
                        }
                        else
                        {
                            DataRow dr = rows[rows.Length - 1];
                            string typeName = (String)dr["TypeName"];
                            string createParams = "";
                            if (!dr.IsNull("CreateParameters"))
                                createParams = (string)dr["CreateParameters"];
                            string createFormat = "";
                            if (!dr.IsNull("CreateFormat"))
                                createFormat = (string)dr["CreateFormat"];
                            if (createFormat == "" || createParams == "")
                                sb.Append(typeName);
                            else
                            {
                                if ((createParams == "size" || createParams == "max length"))
                                    sb.AppendFormat(createFormat, size[k] > 0 ? size[k] : 255);
                                else if (createParams == "precision,scale" || createParams == "precision" ||
                                    createParams == "number of bits used to store the mantissa")
                                    sb.AppendFormat(typeName);
                                else
                                    sb.Append(typeName);
                            }
                        }
                    }
                    else
                        sb.Append(ti.ExportDataType);
                }
                if (sb2.Length > 0)
                {
                    sb.Append(",");
                    sb.AppendLine();
                    sb.AppendFormat("PRIMARY KEY ({0})", sb2.ToString());
                }
                if (sb3.Length > 0)
                {
                    sb.Append(",");
                    sb.AppendLine();
                    sb.AppendFormat("UNIQUE ({0})", sb3.ToString());
                }
                conn.Close();
                return sb.ToString();
            }
        }

        public string CreateCommandText()
        {
            DataProviderHelper helper = new DataProviderHelper(ProviderInvariantName, ConnectionString);
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            for (int k = 0; k < FieldNames.Count; k++)
            {
                if (k > 0)
                    sb.Append(", ");
                sb.Append(helper.FormatIdentifier(FieldNames[k]));
            }
            sb.AppendLine();
            sb.Append(" FROM ");
            sb.Append(helper.FormatIdentifier(Util.SplitName(TableName)));
            return sb.ToString();
        }

        public DbDataAdapter CreateDataAdapter()
        {
            DataProviderHelper helper = new DataProviderHelper(ProviderInvariantName, ConnectionString);
            DbProviderFactory f = DbProviderFactories.GetFactory(ProviderInvariantName);
            DbConnection conn = f.CreateConnection();
            conn.ConnectionString = ConnectionString;
            DbCommand command = conn.CreateCommand();
            command.CommandText = CreateCommandText();
            DbDataAdapter adapter = f.CreateDataAdapter();
            adapter.UpdateBatchSize = helper.UpdateBatchSize;
            adapter.SelectCommand = command;
            DbCommandBuilder builder = f.CreateCommandBuilder();
            builder.DataAdapter = adapter;
            builder.QuotePrefix = Convert.ToString(helper.LeftQuote);
            builder.QuoteSuffix = Convert.ToString(helper.RightQuote);
            return adapter;             
        }

        public ProxyDataAdapter CreateProxyDataAdapter()
        {
            DataProviderHelper helper = new DataProviderHelper(ProviderInvariantName, ConnectionString);
            RemoteDbProviderFactory f = RemoteDbProviderFactories.GetFactory(ProviderInvariantName);
            DbConnection conn = f.CreateConnection();
            conn.ConnectionString = ConnectionString;
            RemoteCommand command = f.CreateCommand();
            command.Connection = conn;
            command.CommandText = CreateCommandText();
            ProxyDataAdapter proxy = f.CreateProxyDataAdapter();
            proxy.UpdateBatchSize = helper.UpdateBatchSize;
            proxy.SelectCommand = command.InnerCommand;
            proxy.QuotePrefix = Convert.ToString(helper.LeftQuote);
            proxy.QuoteSuffix = Convert.ToString(helper.RightQuote);
            return proxy;
        }

        private DataRow[] SelectDataType(DataTable dt, Type dataType, long? columnSize,
            string[] createParameters, bool? isBestMatch, bool? isLong, bool? isFixedLength)
        {
            List<DataRow> res = new List<DataRow>();
            foreach (DataRow dr in dt.Rows)
                if (dataType.FullName.Equals(dr["DataType"]))
                {
                    if (isBestMatch != null && !dr.IsNull("IsBestMatch") && 
                        Convert.ToBoolean(dr["IsBestMatch"]) != isBestMatch.Value)
                        continue;
                    if (isLong != null && !dr.IsNull("IsLong") && 
                        Convert.ToBoolean(dr["IsLong"]) != isLong.Value)
                        continue;
                    if (isFixedLength != null && !dr.IsNull("IsFixedLength") &&
                        Convert.ToBoolean(dr["IsFixedLength"]) != isFixedLength.Value)
                        continue;
                    if (createParameters != null)
                    {                        
                        string cp = null;
                        if (!dr.IsNull("CreateParameters"))
                            cp = (string)dr["CreateParameters"];
                        bool found = false;
                        foreach (string key in createParameters)
                            if (cp == key)
                            {
                                found = true;
                                break;
                            }
                        if (!found)
                            continue;
                    }
                    if (columnSize != null && Convert.ToInt64(dr["ColumnSize"]) < columnSize.Value)
                        continue;
                    res.Add(dr);
                }
            return res.ToArray();
        }

        private DbConnection CreateConnection()
        {
            DbConnection conn = DataProviderHelper.CreateDbConnection(ProviderInvariantName);
            conn.ConnectionString = ConnectionString;
            return conn;
        }

        public void SetFieldsFromTableType(TableType tableType)
        {
            FieldNames.Clear();
            foreach (RowType.TypeInfo ti in tableType.TableRowType.Fields)
                FieldNames.Add(ti.ProviderColumnName);
        }

        public void ParseDDL(string commandText)
        {
            DataProviderHelper helper = 
                new DataProviderHelper(ProviderInvariantName, ConnectionString);
            char[] chars = commandText.ToCharArray();
            bool bInside = false;
            StringBuilder sb = new StringBuilder();
            List<String> lines = new List<string>();
            List<String> FieldNames = new List<string>();
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                if (ch == '(')
                    bInside = true;
                else
                    if (bInside)
                    {
                        if (ch == ')')
                            break;
                        else if (ch == ',')
                        {
                            lines.Add(sb.ToString());
                            sb = new StringBuilder();
                        }
                        else
                            sb.Append(ch);
                    }
            }
            foreach (string s in lines)
            {
                chars = s.ToCharArray();
                bool bLiteral = false;
                for (int i = 0; i < chars.Length; i++)
                {
                    char ch = chars[i];
                    if (Char.IsLetterOrDigit(ch) || bLiteral)
                        sb.Append(ch);
                    else if (ch == helper.LeftQuote)
                        bLiteral = true;
                    else if (ch == helper.RightQuote)
                        bLiteral = false;
                    else
                        if (Char.IsWhiteSpace(ch) && sb.Length > 0)
                        {
                            string name = sb.ToString();
                            sb = new StringBuilder();
                            if (!helper.IsKeyword(name))
                                FieldNames.Add(name);
                            break;
                        }
                }
            }            
        }

        public bool HasUnknownDatatype { get; private set; }
        public String ProviderInvariantName { get; set; }        
        public String ConnectionString { get; set; }
        public String TableName { get; set; }
        public Resultset Source { get; set; }
        public List<String> FieldNames { get; private set; }
    }
}
