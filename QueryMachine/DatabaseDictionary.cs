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
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data;
using System.Data.Common;

using DataEngine.CoreServices.Data;
using System.IO;
using System.Reflection;

namespace DataEngine
{
    public enum AcessorType
    {
        DataProvider,
        DataTable,  
        DataSet,
        XMLFile,
        FlatFile,
        XlFile,
        XlFileTable
    }

    public class DataSourceInfo
    {
        public readonly String Prefix;
        public readonly bool Default;
        public readonly AcessorType TableAccessor;
        public readonly String ProviderInvariantName;
        public readonly String ConnectionString;
        public readonly Object DataContext;
        public readonly FormatIdentifierDelegate FormatIdentifier;

        public DataSourceInfo(string prefix, bool isDefault, AcessorType tableAccessor, String providerInvariantName, 
            String connectionString, Object dataContext)
        {
            Prefix = prefix;
            Default = isDefault;
            TableAccessor = tableAccessor;
            ProviderInvariantName = providerInvariantName;
            ConnectionString = connectionString;
            FormatIdentifier = null;
            DataContext = dataContext;
        }

        public DataSourceInfo(string prefix, bool isDefault, AcessorType tableAccessor, String providerInvariantName,
            String connectionString, FormatIdentifierDelegate formatIdentifier)
        {
            Prefix = prefix;
            Default = isDefault;
            TableAccessor = tableAccessor;
            ProviderInvariantName = providerInvariantName;
            ConnectionString = connectionString;
            FormatIdentifier = formatIdentifier;
        }

        public DbConnection CreateConnection()
        {            
            DbConnection connection = DataProviderHelper.CreateDbConnection(ProviderInvariantName);
            connection.ConnectionString = ConnectionString;
            return connection;
        }
    }

    public class TableType
    {
        public readonly String QualifiedName;
        public readonly String CatalogName;
        public readonly String SchemaName;
        public readonly String TableName;
        public readonly DataSourceInfo DataSource; 
        public readonly RowType TableRowType;
        public readonly bool Smart;

        public TableType(String qualifiedName, String catalogName, String schemaName, String tableName, 
            DataSourceInfo dataSource, RowType tableRowType, bool smart)
        {
            QualifiedName = qualifiedName;
            CatalogName = catalogName;
            SchemaName = schemaName;
            TableName = tableName;
            DataSource = dataSource;
            TableRowType = tableRowType;
            Smart = smart;
        }

        public TableType(String qualifiedName, String tableName, DataSourceInfo dataSource, RowType tableRowType)
        {
            QualifiedName = qualifiedName;
            TableName = tableName;
            DataSource = dataSource;
            TableRowType = tableRowType;
        }

        public String[] GetIdentifierParts()
        {
            List<String> identifierParts = new List<string>();
            if (CatalogName != null)
                identifierParts.Add(CatalogName);
            if (SchemaName != null)
                identifierParts.Add(SchemaName);
            identifierParts.Add(TableName);
            return identifierParts.ToArray();
        }

        public String ToString(DataProviderHelper helper)
        {
            StringBuilder sb = new StringBuilder();
            string[] identifiers = GetIdentifierParts();
            for (int k = 0; k < identifiers.Length; k++)
            {
                if (k > 0)
                    sb.Append(helper.Qualifer);
                sb.Append(helper.FormatIdentifier(identifiers[k]));
            }
            return sb.ToString();
        }
    }

    public class ResolveArgs : EventArgs
    {
        public readonly String Prefix;
        public readonly String[] QualifiedName;
        
        public AcessorType TableAccessor { get; set; }
        public String ProviderInvariantName { get; set; }
        public String ConnectionString { get; set; }
        public Object Context { get; set; }
        public bool Handled { get; set; }

        public ResolveArgs(String prefix, String[] qualifiedName)
        {
            Prefix = prefix;
            QualifiedName = qualifiedName;
        }
    }

    public delegate void ResolveDataSourceInfoDelegate(Object sender, ResolveArgs arg);
    public delegate string[] FormatIdentifierDelegate(Object sender, string[] identifierParts);

    public class DatabaseDictionary
    {
        protected class TableTypeItem
        {
            public TableType CachedType;
            public long CreateTimestamp;
            public long AccessTimestamp;            

            public TableTypeItem(TableType tableType)
            {
                CachedType = tableType;
                CreateTimestamp = Stopwatch.GetTimestamp();
            }

            public TableType GetTableType()
            {
                AccessTimestamp = Stopwatch.GetTimestamp();
                return CachedType;
            }

            public bool IsActual()
            {
                return true;
            }
        }

        private List<DataSourceInfo> _rdsi;
        private Dictionary<String, TableTypeItem> _catalogCache;
        private DataSourceInfo _xmldsi;
        private String _path;

        public String SearchPath 
        { 
            get { return _path; }
            set
            {
                if (value == null)
                    _path = "";
                else
                    _path = value;
            }
        }

        public String GetFilePath(string fileName, string prefix)
        {
            if (fileName.IndexOfAny(new char[] { '*', '?' }) != -1)
                return fileName;
            else
            {
                string[] pathset = SearchPath.Split(new char[] { ';' });
                if (Path.GetExtension(fileName) == "")
                    fileName = fileName + "." + prefix;
                foreach (string dir in pathset)
                {
                    FileInfo fi = new FileInfo(Path.Combine(dir, fileName));
                    if (fi.Exists)
                        return fi.FullName;
                }
                throw new ESQLException(Properties.Resources.FileNotFound, fileName);
            }
        }

        public DatabaseDictionary()
        {
            SearchPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _catalogCache = new Dictionary<string, TableTypeItem>();
            _rdsi = new List<DataSourceInfo>();
            _xmldsi = new DataSourceInfo(null, false, AcessorType.XMLFile, null, null, null);            
        }

        public TableType GetTableType(String prefix, String[] identifierPart)
        {
            return GetTableType(prefix, identifierPart, true);
        }

        public TableType GetTableType(String prefix, String[] identifierPart, bool lookupCache)
        {
            TableTypeItem tti;
            string qualifiedName = MakeQualifiedName(identifierPart);
            string key = String.Format("{0}!{1}", prefix, qualifiedName);

            lock (_catalogCache)
            {
                if (lookupCache && _catalogCache.TryGetValue(key, out tti) && tti.IsActual())
                    return tti.GetTableType();

                DataSourceInfo dsi = ResolveTable(prefix, identifierPart);
                if (dsi == null)
                    throw new ESQLException(Properties.Resources.UnknownDataSource, qualifiedName);

                TableType tableType;
                switch (dsi.TableAccessor)
                {
                    case AcessorType.DataProvider:
                        tableType = GetDataProviderTableType(dsi, GetProviderTableName(dsi, identifierPart));
                        break;

                    case AcessorType.XMLFile:
                        {
                            string fullPath = GetFilePath(qualifiedName, "xml");
                            tableType = new TableType(qualifiedName, fullPath, dsi, null);
                        }
                        break;

                    case AcessorType.DataTable:
                        {
                            string fullPath = GetFilePath(qualifiedName, "xml");
                            tableType = new TableType(qualifiedName, fullPath, dsi, null);
                        }
                        break;

                    case AcessorType.FlatFile:
                        {
                            string fullPath = GetFilePath(qualifiedName, "txt");
                            tableType = new TableType(qualifiedName, fullPath, dsi, null);
                        }
                        break;

                    case AcessorType.XlFile:
                    case AcessorType.XlFileTable:
                        tableType = new TableType(qualifiedName, qualifiedName, dsi, null);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                if (tableType == null)
                    throw new ESQLException(Properties.Resources.TableDoesNotExists, qualifiedName);

                if (dsi.TableAccessor == AcessorType.DataProvider && lookupCache)
                    _catalogCache.Add(key, new TableTypeItem(tableType));
                return tableType;
            }
        }

        protected virtual String GetProviderTableName(DataSourceInfo dsi, String[] identifierPart)
        {
            StringBuilder sb = new StringBuilder();
            DataProviderHelper helper = new DataProviderHelper(dsi.ProviderInvariantName, dsi.ConnectionString);
            string[] formattedIdentifiers = new String[identifierPart.Length];
            for (int k = 0; k < identifierPart.Length; k++)
                formattedIdentifiers[k] = helper.FormatIdentifier(identifierPart[k]);
            if (dsi.FormatIdentifier != null)
                formattedIdentifiers = dsi.FormatIdentifier(this, formattedIdentifiers);
            for (int k = 0; k < formattedIdentifiers.Length; k++)
            {
                if (k > 0)
                    sb.Append(helper.Qualifer);
                sb.Append(formattedIdentifiers[k]);
            }
            return sb.ToString();
        }

        private TableType GetDataProviderTableType(DataSourceInfo dsi, String identifier)
        {           
            DataProviderHelper helper = new DataProviderHelper(dsi.ProviderInvariantName, dsi.ConnectionString);                       
            using (DbConnection connection = dsi.CreateConnection())
            {                
                DbCommand command = connection.CreateCommand();                
                
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM ");
                sb.Append(identifier);
                if (helper.OrderByColumnsInSelect)
                    sb.Append(" ORDER BY 1");
                command.CommandText = sb.ToString();                
                
                try
                {
                    DbDataReader reader;                    
                    connection.Open();
                    bool hasKeyInfo = false;                    
                    try
                    {
                        reader = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
                        hasKeyInfo = true;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("GetDataProviderTableType.ExecuteReader: {0}", ex.Message);                        
                        reader = command.ExecuteReader(CommandBehavior.SchemaOnly);                        
                    }

                    DataTable dt_src = reader.GetSchemaTable();
                    DataTable dt = RowType.CreateSchemaTable();
                    int n = 0;
                    foreach (DataRow r in dt_src.Rows)
                    {
                        DataRow r1 = dt.NewRow();
                        foreach (DataColumn col in dt_src.Columns)
                        {
                            DataColumn dest = dt.Columns[col.ColumnName];
                            if (dt.Columns.IndexOf(col.ColumnName) != -1 && col.ColumnName != "ColumnName")
                                r1[dest] = r[col];
                        }
                        r1["ColumnOrdinal"] = n++;
                        string columnName = (string)r["ColumnName"];
                        r1["ColumnName"] = helper.NativeFormatIdentifier(columnName);
                        r1["ProviderColumnName"] = columnName;
                        r1["IsCaseSensitive"] = 
                            helper.IdentifierCase == IdentifierCase.Sensitive;
                        dt.Rows.Add(r1);
                    }

                    reader.Close();
                    
                    RowType rtype = new RowType(dt);
                    if (hasKeyInfo && rtype.Fields[0].BaseTableName != null)
                        return new TableType(identifier, rtype.Fields[0].BaseCatalogName,
                            rtype.Fields[0].BaseSchemaName, rtype.Fields[0].BaseTableName, dsi, rtype, helper.Smart);
                    else
                    {
                        string schema = null;
                        string catalog = null;
                        string tableName = null;
                        
                        string[] identifierPart = helper.SplitIdentifier(identifier);                        
                        int length = identifierPart.Length;

                        if (length == 3)
                            catalog = identifierPart[identifierPart.Length - length--];

                        if (length == 2)
                            schema = identifierPart[identifierPart.Length - length--];

                        if (length == 1)
                            tableName = identifierPart[identifierPart.Length - length];
                        
                        return new TableType(identifier, catalog, schema, tableName, dsi, rtype, helper.Smart);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("GetDataProviderTableType: {0}", ex.Message);
                    return null;
                }
            }
        }

        private TableType GetDataSetTableType(DataSourceInfo dsi, String tableName)
        {
            DataSet ds = (DataSet)dsi.DataContext;
            if (ds.Tables.Contains(tableName))
                return GetDataTableType(dsi, ds.Tables[tableName]);
            else
                return null;
        }

        private TableType GetDataTableType(DataSourceInfo dsi, DataTable dt)
        {
            DataTableReader reader = dt.CreateDataReader();
            DataTable schemaTable = reader.GetSchemaTable();
            reader.Close();
            return new TableType(dt.TableName, null, null, 
                dt.TableName, dsi, new RowType(schemaTable), false);
        }

        public void Clear()
        {
            lock(_catalogCache)
                _catalogCache.Clear();
        }

        public void PruneCache()
        {
            lock (_catalogCache)
            {
                List<String> pruned = new List<String>();
                foreach (KeyValuePair<String, TableTypeItem> kv in _catalogCache)
                    if (!kv.Value.IsActual())
                        pruned.Add(kv.Key);
                foreach (String key in pruned)
                    _catalogCache.Remove(key);
            }
        }

        public event ResolveDataSourceInfoDelegate OnResolveDataSource;

        public DataSourceInfo GetDataSource(string prefix)
        {
            foreach (DataSourceInfo dsi in _rdsi)
                if (prefix == dsi.Prefix || (String.IsNullOrEmpty(prefix) && dsi.Default))
                    return dsi;
            return null;
        }

        protected virtual DataSourceInfo ResolveTable(string prefix, string[] identifierPart)
        {
            if (OnResolveDataSource != null)
            {
                ResolveArgs arg = new ResolveArgs(prefix, identifierPart);
                OnResolveDataSource(this, arg);
                if (arg.Handled)
                    return new DataSourceInfo(null, false, arg.TableAccessor, arg.ProviderInvariantName, 
                        arg.ConnectionString, arg.Context);
            }

            foreach (DataSourceInfo dsi in _rdsi)
            {
                if (prefix == dsi.Prefix || (String.IsNullOrEmpty(prefix) && dsi.Default))
                    return dsi;
            }

            if (prefix != null)
            {
                if (prefix.Equals("XML"))
                    return _xmldsi;
                else if (prefix.Equals("ADO"))
                    return new DataSourceInfo(null, false, AcessorType.DataTable, null, null, null);
                else if (prefix.Equals("TXT"))
                    return new DataSourceInfo(null, false, AcessorType.FlatFile, null, null, null);
                else if (prefix.Equals("XL"))
                    return new DataSourceInfo(null, false, AcessorType.XlFile, null, null, null);
                else if (prefix.Equals("XLT"))
                    return new DataSourceInfo(null, false, AcessorType.XlFileTable, null, null, null);
                else
                {
                    string fileName = GetFilePath(identifierPart[0], prefix.ToLowerInvariant());
                    string baseDir = Path.GetDirectoryName(fileName);
                    if (prefix.Equals("DBF"))
                        return new DataSourceInfo(null, false, AcessorType.DataProvider, "System.Data.OleDb",
                           String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;User ID=Admin;Password=;", baseDir), null);
                    else if (prefix.Equals("CSV"))
                        return new DataSourceInfo(null, false, AcessorType.DataProvider, "System.Data.OleDb",
                           String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"text;HDR=Yes\";", baseDir), null);
                    else if (prefix.Equals("XLS"))
                    {
                        FormatIdentifierDelegate del =
                            delegate(object sender, string[] parts)
                            {
                                if (parts.Length != 2)
                                    throw new ESQLException(Properties.Resources.InvalidIdentifierFormat,
                                        "Excel", "FileName.SheetName$");
                                string[] res = new string[1];
                                res[0] = parts[1];
                                return res;
                            };
                        if (identifierPart[0].EndsWith(".xlsx"))
                            return new DataSourceInfo(null, false, AcessorType.DataProvider, "System.Data.OleDb",
                               String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\";", fileName), del);
                        else if (identifierPart[0].EndsWith(".xlsb"))
                            return new DataSourceInfo(null, false, AcessorType.DataProvider, "System.Data.OleDb",
                               String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=YES\";", fileName), del);
                        else if (identifierPart[0].EndsWith(".xlsm"))
                            return new DataSourceInfo(null, false, AcessorType.DataProvider, "System.Data.OleDb",
                               String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Macro;HDR=YES\";", fileName), del);
                        else
                            return new DataSourceInfo(null, false, AcessorType.DataProvider, "System.Data.OleDb",
                               String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes\";", fileName), del);
                    }
                }
            }

            return null;
        }

        public void RegisterDataProvider(string prefix, bool isDefault, String providerInvariantName, String connectionString)
        {
            _rdsi.Add(new DataSourceInfo(prefix, isDefault, AcessorType.DataProvider, 
                providerInvariantName, connectionString, null));
        }

        //public void RegisterDataTable(DataTable dataTable)
        //{
        //    RegisterDataTable(dataTable.TableName, dataTable);
        //}

        //public void RegisterDataSet(DataSet dataSet)
        //{
        //    RegisterDataSet(dataSet.DataSetName, dataSet);
        //}

        //public void RegisterDataTable(string tableName, DataTable dataTable)
        //{
        //    _rdsi.Add(new DataSourceInfo(String.Format("^{0}$", EscapeRegExprString(tableName)),
        //        AcessorType.DataTable, null, null, null, dataTable));
        //}

        //public void RegisterDataSet(string datasetName, DataSet dataSet)
        //{
        //    _rdsi.Add(new DataSourceInfo(String.Format("^{0}\\.[^.]+$", EscapeRegExprString(datasetName)),
        //        AcessorType.DataSet, null, null, null, dataSet));
        //}        

        private string MakeQualifiedName(string[] identifierPart)
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < identifierPart.Length; k++)
            {
                if (k > 0)
                    sb.Append(".");
                sb.Append(identifierPart[k]);
            }
            return sb.ToString();
        }

        private string EscapeRegExprString(string str)
        {
            return str;
        }
    }
}
