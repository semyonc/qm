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
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Data;
using System.Data.Common;

using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataEngine
{
    public enum AccessorType
    {
        DataProvider,
        DataTable,  
        DataSet,
        XMLFile,
        FlatFile,
        XlFile,
        XlFileTable,
        MongoDb,
        Json,
        ZipJson,
        CustomAccessor
    }

    [Serializable]
    public class DataSourceInfo
    {
        public readonly Guid UUID;
        public readonly String Prefix;
        public readonly bool Default;
        public readonly AccessorType TableAccessor;
        public readonly bool X86Connection;
        public readonly String ProviderInvariantName;
        public readonly String ConnectionString;
        public readonly Object DataContext;
        [NonSerialized] 
        public readonly FormatIdentifierDelegate FormatIdentifier;
        [NonSerialized]
        public readonly DataAccessorFactory DataAccessorFactory;

        internal object Tag { get; set; }

        public DataSourceInfo(string prefix, bool isDefault, AccessorType tableAccessor, String providerInvariantName, bool x86Connection,
            String connectionString, Object dataContext)
        {
            UUID = Guid.Empty;
            Prefix = prefix;
            Default = isDefault;
            TableAccessor = tableAccessor;
            ProviderInvariantName = providerInvariantName;
            X86Connection = x86Connection;
            ConnectionString = connectionString;
            FormatIdentifier = null;
            DataContext = dataContext;
        }

        public DataSourceInfo(Guid uuid, string prefix, bool isDefault, AccessorType tableAccessor, String providerInvariantName, bool x86Connection,
            String connectionString, FormatIdentifierDelegate formatIdentifier)
        {
            UUID = uuid;
            Prefix = prefix;
            Default = isDefault;
            TableAccessor = tableAccessor;
            ProviderInvariantName = providerInvariantName;
            X86Connection = x86Connection;
            ConnectionString = connectionString;
            FormatIdentifier = formatIdentifier;
        }

        public DataSourceInfo(DataAccessorFactory dataAccessorFactory)
        {
            DataAccessorFactory = dataAccessorFactory;
            TableAccessor = AccessorType.CustomAccessor;
        }

        public DbConnection CreateConnection()
        {
            if (ProviderInvariantName == null)
                throw new InvalidOperationException();
            DbConnection connection = DataProviderHelper.CreateDbConnection(ProviderInvariantName, X86Connection);
            connection.ConnectionString = ConnectionString;
            return connection;
        }
    }

    [Serializable]
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

        public TableType(String qualifiedName, String tableName, DataSourceInfo dataSource, RowType tableRowType, bool smart)
        {
            QualifiedName = qualifiedName;
            TableName = tableName;
            DataSource = dataSource;
            TableRowType = tableRowType;
            Smart = smart;
        }

        public TableType(String qualifiedName, String tableName, DataSourceInfo dataSource, RowType tableRowType)
            : this(qualifiedName, tableName, dataSource, tableRowType, false)
        {
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
        
        public AccessorType TableAccessor { get; set; }
        public String ProviderInvariantName { get; set; }
        public bool X86Connection { get; set; }
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
        public static readonly int MongoDbFastMRLimit = 150;

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

        public string SchemaCacheDir { get; set; }

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
            _xmldsi = new DataSourceInfo(null, false, AccessorType.XMLFile, null, false, null, null);            
        }

        public TableType GetTableType(String prefix, String[] identifierPart, string[] selectFields)
        {
            return GetTableType(prefix, identifierPart, true, selectFields);
        }

        public TableType GetTableType(String prefix, String[] identifierPart, bool lookupCache, string[] selectFields)
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
                    case AccessorType.DataProvider:
                        tableType = GetDataProviderTableType(dsi, GetProviderTableName(dsi, identifierPart));
                        break;

                    case AccessorType.XMLFile:
                        {
                            string fullPath = GetFilePath(qualifiedName, "xml");
                            tableType = new TableType(qualifiedName, fullPath, dsi, null);
                        }
                        break;

                    case AccessorType.DataTable:
                        {
                            string fullPath = GetFilePath(qualifiedName, "xml");
                            tableType = new TableType(qualifiedName, fullPath, dsi, null);
                        }
                        break;

                    case AccessorType.FlatFile:
                        {
                            string fullPath = GetFilePath(qualifiedName, "txt");
                            tableType = new TableType(qualifiedName, fullPath, dsi, null);
                        }
                        break;

                    case AccessorType.XlFile:
                    case AccessorType.XlFileTable:
                        tableType = new TableType(qualifiedName, qualifiedName, dsi, null);
                        break;

                    case AccessorType.MongoDb:
                        tableType = GetMongoDbTableType(dsi, qualifiedName, selectFields);
                        break;

                    case AccessorType.Json:
                        {
                            string fullPath = GetFilePath(qualifiedName, "json");
                            tableType = new TableType(qualifiedName, fullPath, dsi, null);
                        }
                        break;

                    case AccessorType.ZipJson:
                        {
                            string fullPath = GetFilePath(qualifiedName, "zip");
                            tableType = new TableType(qualifiedName, fullPath, dsi, null);
                        }
                        break;

                    case AccessorType.CustomAccessor:
                        tableType = dsi.DataAccessorFactory.CreateTableType(this, qualifiedName, dsi);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                if (tableType == null)
                    throw new ESQLException(Properties.Resources.TableDoesNotExists, qualifiedName);

                if (dsi.TableAccessor == AccessorType.DataProvider && lookupCache)
                    _catalogCache.Add(key, new TableTypeItem(tableType));
                return tableType;
            }
        }

        protected virtual String GetProviderTableName(DataSourceInfo dsi, String[] identifierPart)
        {
            StringBuilder sb = new StringBuilder();
            DataProviderHelper helper = new DataProviderHelper(dsi);
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
            DataProviderHelper helper = new DataProviderHelper(dsi);                       
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
                        return new TableType(identifier, !helper.IgnoreCatalog ? rtype.Fields[0].BaseCatalogName : null,
                            !helper.IgnoreSchema ? rtype.Fields[0].BaseSchemaName : null, rtype.Fields[0].BaseTableName, dsi, rtype, helper.Smart);
                    else
                    {
                        string schema = null;
                        string catalog = null;
                        string tableName = null;
                        
                        string[] identifierPart = helper.SplitIdentifier(identifier);                        
                        int length = identifierPart.Length;

                        if (length == 3 && !helper.IgnoreCatalog)
                            catalog = identifierPart[identifierPart.Length - length--];

                        if (length == 2 && !helper.IgnoreSchema)
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

        private String GetMongoCacheTableName(DataSourceInfo dsi, String collectionName)
        {
            if (SchemaCacheDir == null)
                return null;
            return Path.Combine(SchemaCacheDir,
                dsi.UUID.ToString(), Uri.EscapeDataString(collectionName));            
        }

        private TableType GetCachedTableType(string cacheTableName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = File.OpenRead(cacheTableName);
            try
            {
                return (TableType)formatter.Deserialize(stream);
            }
            finally
            {
                stream.Close();
            }
        }

        private void PutCacheTableType(string cacheTableName, TableType tableType)
        {
            string directoryName = Path.GetDirectoryName(cacheTableName);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = File.OpenWrite(cacheTableName);
            try
            {
                formatter.Serialize(stream, tableType);
            }
            finally
            {
                stream.Close();
            }
        }

        private TableType GetMongoDbTableType(DataSourceInfo dsi, String collectionName, String[] selectFields)
        {
            string cacheTableName = GetMongoCacheTableName(dsi, collectionName);
            if (cacheTableName != null && File.Exists(cacheTableName))
                return GetCachedTableType(cacheTableName);
            return ScanMongoDbCollection(dsi, collectionName, selectFields, MongoDbFastMRLimit);
        }

        public void CacheMongoDbTableType(DataSourceInfo dsi, String collectionName)
        {
            if (dsi.TableAccessor != AccessorType.MongoDb)
                throw new ArgumentException("dsi");
            string cacheTableName = GetMongoCacheTableName(dsi, collectionName);
            if (cacheTableName != null)
            {
                TableType tableType = ScanMongoDbCollection(dsi, collectionName, null, -1);
                PutCacheTableType(cacheTableName, tableType);
            }
        }

        private TableType ScanMongoDbCollection(DataSourceInfo dsi, String collectionName, String[] selectFields, int rowlimit)
        {
            try
            {
                MongoConnectionStringBuilder csb = new MongoConnectionStringBuilder();
                csb.ConnectionString = dsi.ConnectionString;
                MongoServer mongo = MongoServer.Create(csb);
                mongo.Connect();
                if (!mongo.DatabaseExists(csb.DatabaseName))
                    throw new ESQLException("Database '{0}' is not exists", csb.DatabaseName);
                MongoDatabaseSettings settings = mongo.CreateDatabaseSettings(csb.DatabaseName);
                if (csb.Username != null)
                    settings.Credentials = new MongoCredentials(csb.Username, csb.Password);
                MongoDatabase database = mongo.GetDatabase(settings);
                if (!database.CollectionExists(collectionName))
                    return null;
                List<string> fieldNames = new List<string>();
                DataTable dt = RowType.CreateSchemaTable();
                MongoCollectionSettings<BsonDocument> collectionSettings =
                    database.CreateCollectionSettings<BsonDocument>(collectionName);
                MongoCollection<BsonDocument> collection = database.GetCollection(collectionSettings);
                MapReduceResult res = ExecuteMapReduce(collection, rowlimit);
                if (res.Ok)
                {
                    Dictionary<String, List<BsonDocument>> map = new Dictionary<String, List<BsonDocument>>();
                    foreach (BsonDocument inlineRes in res.GetInlineResultsAs<BsonDocument>())
                    {
                        BsonDocument key = inlineRes[0].AsBsonDocument;
                        String name = key[0].AsString;
                        List<BsonDocument> fields;
                        if (!map.TryGetValue(name, out fields))
                        {
                            fields = new List<BsonDocument>();
                            map.Add(name, fields);
                        }
                        fields.Add(inlineRes);
                    }
                    List<List<BsonDocument>> order = new List<List<BsonDocument>>(map.Values);
                    order.Sort((a, b) =>
                    {
                        int min1 = -1;
                        for (int k = 0; k < a.Count; k++)
                            if (min1 == -1 || Convert.ToInt32(a[k][1]) < min1)
                                min1 = Convert.ToInt32(a[k][1]);
                        int min2 = -1;
                        for (int k = 0; k < b.Count; k++)
                            if (min2 == -1 || Convert.ToInt32(b[k][1]) < min2)
                                min2 = Convert.ToInt32(b[k][1]);
                        if (min1 < min2)
                            return -1;
                        else if (min1 == min2)
                            return 0;
                        return 1;
                    });
                    for (int i = 0; i < order.Count; i++)
                    {
                        List<BsonDocument> field = order[i];
                        string name = field[0][0].AsBsonDocument[0].AsString;
                        string[] types = new string[field.Count];
                        for (int s = 0; s < field.Count; s++)
                            types[s] = field[s][0].AsBsonDocument[1].AsString;
                        CreateFieldType(collectionName, dt, i + 1, name, types, fieldNames);
                    }
                    return new TableType(collectionName, collectionName, dsi, new RowType(dt), true);
                }
                mongo.Disconnect();
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("GetMongoDbTableType: {0}", ex.Message);
                if (ex is ESQLException)
                    throw ex;
            }
            return null;
        }
        
        private MapReduceResult ExecuteMapReduce(MongoCollection<BsonDocument> collection, int rowlimit = -1)
        {
            MapReduceOptionsBuilder builder = new MapReduceOptionsBuilder();
            builder.SetOutput(MapReduceOutput.Inline);
            if (rowlimit != -1)
                builder.SetLimit(rowlimit);
            return collection.MapReduce(
                new BsonJavaScript(@"
                        function Map() {
                            var k = 1;
                            for (var prop in this) {
                                var value = this[prop];
                                var t;
                                if (value == null)
                                    t = 'null';
                                else if (value instanceof Date)
                                    t = 'date';
                                else if (value instanceof Timestamp)
                                    t = 'timestamp';
                                else if (value instanceof ObjectId)
                                    t = 'objectid';
                                else if (value instanceof BinData)
                                    t = 'binary';
                                else if (value instanceof NumberInt)
                                    t = 'long';
                                else if (value instanceof NumberLong)
                                    t = 'long';
                                else
          	                        t = typeof value;
                                emit({ 'prop': prop, 't': t }, k++);
                            }                        
                        }"),
                new BsonJavaScript(@"
                        function Reduce(key, values) {
                            return values[0];
                        }"), builder);            
        }
        
        private void CreateFieldType(string tableName, DataTable dt, int ordinal, string columnName, 
            String[] types, List<String> fieldNames)
        {
            Type dataType = null;
            bool allowDbNull = false;
            bool isKey = false;
            foreach (String t in types)
            {
                Type currType = null;
                if (t == "date")
                    currType = typeof(System.DateTime);
                else if (t == "timestamp")
                    currType = typeof(System.TimeSpan);
                else if (t == "string")
                    currType = typeof(System.String);
                else if (t == "objectid")
                {
                    currType = typeof(System.String);
                    if (ordinal == 1)
                        isKey = true;
                }
                else if (t == "binary")
                    currType = typeof(byte[]);
                else if (t == "int")
                    currType = typeof(System.Int32);
                else if (t == "long")
                    currType = typeof(System.Int64);
                else if (t == "number")
                    currType = typeof(System.Double);
                else if (t == "boolean")
                    currType = typeof(System.Boolean);
                else if (t == "object")
                    currType = typeof(System.Object);
                else if (t == "null")
                    allowDbNull = true;
                if (currType != null)
                {
                    if (dataType == null)
                        dataType = currType;
                    else
                        if (dataType != currType)
                        {
                            dataType = typeof(System.Object);
                            break;
                        }
                }
            }
            if (dataType == null)
                dataType = typeof(System.Object);
            DataRow r = dt.NewRow();
            r["ColumnName"] = Util.CreateUniqueName(fieldNames, columnName);
            r["ProviderColumnName"] = columnName;
            r["BaseTableName"] = tableName;
            r["BaseColumnName"] = columnName;
            r["ColumnOrdinal"] = ordinal -1;
            r["DataType"] = dataType;
            r["AllowDBNull"] = allowDbNull;
            r["IsKey"] = isKey;
            r["IsCaseSensitive"] = true;
            r["IsRowID"] = isKey;
            dt.Rows.Add(r);
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
                    return new DataSourceInfo(null, false, arg.TableAccessor, arg.ProviderInvariantName, arg.X86Connection,
                        arg.ConnectionString, arg.Context);
            }

            foreach (DataSourceInfo dsi in _rdsi)
            {
                if (prefix == dsi.Prefix || (String.IsNullOrEmpty(prefix) && dsi.Default))
                    return dsi;
            }

            if (prefix != null)
            {
                DataAccessorFactory dataAccessorFactory;
                if (prefix.Equals("XML"))
                    return _xmldsi;
                else if (prefix.Equals("ADO"))
                    return new DataSourceInfo(null, false, AccessorType.DataTable, null, false, null, null);
                else if (prefix.Equals("TXT"))
                    return new DataSourceInfo(null, false, AccessorType.FlatFile, null, false, null, null);
                else if (prefix.Equals("XL"))
                    return new DataSourceInfo(null, false, AccessorType.XlFile, null, false, null, null);
                else if (prefix.Equals("XLT"))
                    return new DataSourceInfo(null, false, AccessorType.XlFileTable, null, false, null, null);
                else if (prefix.Equals("JSON"))
                    return new DataSourceInfo(null, false, AccessorType.Json, null, false, null, null);
                else if (prefix.Equals("JSONZ"))
                    return new DataSourceInfo(null, false, AccessorType.ZipJson, null, false, null, null);
                else if (AccessorCatalog.Instance.TryGet(prefix, out dataAccessorFactory))
                    return new DataSourceInfo(dataAccessorFactory);
                else
                {
                    string fileName = GetFilePath(identifierPart[0], prefix.ToLowerInvariant());
                    string baseDir = Path.GetDirectoryName(fileName);
                    if (prefix.Equals("DBF"))
                        return new DataSourceInfo(null, false, AccessorType.DataProvider, "System.Data.OleDb", true,
                           String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;User ID=Admin;Password=;", baseDir), null);
                    else if (prefix.Equals("CSV"))
                        return new DataSourceInfo(null, false, AccessorType.DataProvider, "System.Data.OleDb", true,
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
                            return new DataSourceInfo(null, false, AccessorType.DataProvider, "System.Data.OleDb", true,
                               String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\";", fileName), del);
                        else if (identifierPart[0].EndsWith(".xlsb"))
                            return new DataSourceInfo(null, false, AccessorType.DataProvider, "System.Data.OleDb", true,
                               String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=YES\";", fileName), del);
                        else if (identifierPart[0].EndsWith(".xlsm"))
                            return new DataSourceInfo(null, false, AccessorType.DataProvider, "System.Data.OleDb", true,
                               String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Macro;HDR=YES\";", fileName), del);
                        else
                            return new DataSourceInfo(null, false, AccessorType.DataProvider, "System.Data.OleDb", true,
                               String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes\";", fileName), del);
                    }
                }
            }

            return null;
        }

        public void RegisterDataProvider(Guid uuid, string prefix, bool isDefault, AccessorType accessorType, 
            String providerInvariantName, bool x86Connection, String connectionString)
        {
            _rdsi.Add(new DataSourceInfo(uuid, prefix, isDefault, accessorType,
                providerInvariantName, x86Connection, connectionString, null));
        }
        
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

    public abstract class DataAccessorFactory
    {
        public virtual TableType CreateTableType(DatabaseDictionary databaseDictionary,
            String qualifiedName, DataSourceInfo dataSource)
        {
            return new TableType(qualifiedName, qualifiedName, dataSource, null);
        }

        public abstract QueryNode CreateAccessor(TableType tableType);
    }

    public class AccessorCatalog
    {
        public static AccessorCatalog Instance { get; private set; }

        private Dictionary<String, DataAccessorFactory> m_factories = new Dictionary<string, DataAccessorFactory>();

        private AccessorCatalog()
        {
        }

        public void RegisterFactory(string prefix, DataAccessorFactory factory)
        {
            try
            {
                m_factories.Add(prefix, factory);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(
                    String.Format(Properties.Resources.AccessorPrefixAlreadyRegistered, prefix), ex);
            }
        }

        public bool TryGet(string prefix, out DataAccessorFactory result)
        {
            return m_factories.TryGetValue(prefix, out result);
        }

        static AccessorCatalog()
        {
            Instance = new AccessorCatalog();
        }
    }
}
