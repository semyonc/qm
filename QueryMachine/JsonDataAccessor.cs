//        Copyright (c) 2012, Semyon A. Chertkov (semyonc@gmail.com)
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
using System.IO;
using System.Text.RegularExpressions;

using MongoDB.Bson;
using MongoDB.Bson.IO;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;


namespace DataEngine
{
    public class JsonDataAccessor: BaseDataAccessor
    {
        class JsonDataFormat
        {
            public bool IgnoreFormatError { get; set; }
        }

        class SimpleContext : DemandProcessingContext
        {
            private StreamReader _reader;
            private BsonLoader _loader;

            public SimpleContext(StreamReader reader)
                : base(null)
            {
                _reader = reader;
                _loader = new BsonLoader();
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    if (_reader != null)
                    {
                        _reader.Close();
                        _reader = null;
                    }
                }
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                CheckDisposed();
                if (!_reader.EndOfStream)
                {
                    JsonBuffer buffer = new JsonBuffer(_reader.ReadLine());
                    JsonReader jsonReader = 
                        new JsonReader(buffer, JsonReaderSettings.Defaults);
                    BsonDocument doc = BsonDocument.ReadFrom(jsonReader);
                    jsonReader.Close();
                    ValueTuple tuple = _loader.CreateTupleFromDocument(doc);
                    Row r = rs.NewRow();
                    r.SetObject(0, tuple);
                    rs.Enqueue(r);
                    return true;
                }
                _reader.Close();
                _reader = null;
                return false;
            }
        }

        class TypedContext : DemandProcessingContext
        {
            private StreamReader _reader;
            private BsonLoader _loader;
            private Dictionary<String, int> _lookup;

            public bool IgnoreConvertionErrors { get; set; }

            public TypedContext(StreamReader reader, DataTable dt)
                : base(null)
            {
                _reader = reader;
                _loader = new BsonLoader();
                _lookup = new Dictionary<string, int>();
                foreach (DataRow dr in dt.Rows)
                    _lookup[(string)dr["ProviderColumnName"]] = (int)dr["ColumnOrdinal"];
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    if (_reader != null)
                    {
                        _reader.Close();
                        _reader = null;
                    }
                }
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                CheckDisposed();
                if (!_reader.EndOfStream)
                {
                    JsonBuffer buffer = new JsonBuffer(_reader.ReadLine());
                    JsonReader jsonReader =
                        new JsonReader(buffer, JsonReaderSettings.Defaults);
                    BsonDocument doc = BsonDocument.ReadFrom(jsonReader);
                    jsonReader.Close();
                    Row r = rs.NewRow();
                    RowType rt = rs.RowType;
                    foreach (BsonElement elem in doc.Elements)
                    {
                        int ordinal = -1;
                        if (elem.Value.BsonType != BsonType.Null && 
                            _lookup.TryGetValue(elem.Name, out ordinal))
                        {
                            Type typ = rt.Fields[ordinal].DataType;
                            if (typ != typeof(System.Object))
                                try
                                {
                                    r.SetValue(ordinal, Convert.ChangeType(elem.Value, typ));
                                }
                                catch (Exception)
                                {
                                    if (!IgnoreConvertionErrors)
                                        throw;
                                }
                            else
                                r.SetValue(ordinal, _loader.GetValue(elem.Name, elem.Value));
                        }
                    }
                    rs.Enqueue(r);
                    return true;
                }
                _reader.Close();
                _reader = null;
                return false;
            }
        }

        private static string[] FieldTypeNames = new string[] {
            "boolean", "int", "long", "number", "date", "string", "objectid", "timestamp", "binary", "object" };
        
        private static Type[] FieldTypes = new Type[] {
            typeof(System.Boolean),    // boolean
            typeof(System.Int32),      // int
            typeof(System.Int64),      // long
            typeof(System.Double),     // number
            typeof(System.DateTime),   // date
            typeof(System.String),     // string
            typeof(System.String),     // objectid
            typeof(System.TimeSpan),   // timestamp
            typeof(byte[]),            // binary
            typeof(System.Object)      // object
        };

        public static string GetSchemaDataType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return "boolean";                
                
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                    return "int";

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return "long";

                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return "number";               

                case TypeCode.DateTime:
                    return "date";

                case TypeCode.String:
                    return "string";

                default:
                    if (type == typeof(TimeSpan))
                        return "timestamp";
                    else if (type == typeof(byte[]))
                        return "binary";
                    else
                        return "object";
            }
        }

        private object DecodeDataType(string name)
        {
            int index = Array.IndexOf<String>(FieldTypeNames, name);
            if (index != -1)
                return FieldTypes[index];
            else
                return typeof(System.String);
        }

        private DataTable CreateRowType(IniFile ini, string section, JsonDataFormat format)
        {
            format.IgnoreFormatError = ini.GetIniBoolean(section, "IgnoreFormatError", false);

            List<string> names = new List<string>();
            List<int> length = new List<int>();
            DataProviderHelper helper = new DataProviderHelper();
            DataTable dt = RowType.CreateSchemaTable();

            string[] keys = ini.GetEntryNames(section);
            Regex regex = new Regex("^Col[0-9]+$");
            StringBuilder sb = new StringBuilder();
            string[] values = new string[4];
            int ordinal = 0;
            for (int k = 0; k < keys.Length; k++)
                if (regex.IsMatch(keys[k]))
                {
                    CsvParser parser = new CsvParser(' ');
                    if (parser.Get((string)ini.GetValue(section, keys[k]), values) > 0)
                    {
                        DataRow dr = dt.NewRow();
                        dr["ColumnOrdinal"] = ordinal++;
                        dr["ColumnName"] = Util.CreateUniqueName(names, helper.NativeFormatIdentifier(values[0]));
                        dr["DataType"] = DecodeDataType(values[1]);
                        dr["ProviderColumnName"] = values[0];
                        dt.Rows.Add(dr);
                    }
                }
            return dt;
        }        

        private DataTable CreateRowType(string fileName, JsonDataFormat format)
        {
            string iniFileName = Path.Combine(Path.GetDirectoryName(fileName), "schema.ini");
            if (File.Exists(iniFileName))
            {
                IniFile ini = new IniFile(iniFileName);
                string[] sections = ini.GetSectionNames();
                string name = Path.GetFileName(fileName);
                foreach (string s in sections)
                    if (s.IndexOfAny(new char[] { '?', '*' }) != -1)
                    {
                        String[] files = Directory.GetFiles(Path.GetDirectoryName(fileName), s);
                        for (int k = 0; k < files.Length; k++)
                            files[k] = Path.GetFileName(files[k]);
                        if (Array.IndexOf<String>(files, name) != -1)
                            return CreateRowType(ini, s, format);
                    }
                    else
                        if (String.Compare(s, name, true) == 0)
                            return CreateRowType(ini, s, format);
            }
            return null;
        }

        protected override Resultset CreateResultset(Stream stream, string fileName, QueryContext queryContext)
        {
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            JsonDataFormat format = new JsonDataFormat();
            DataTable dt = null;
            if (fileName != "")
                dt = CreateRowType(fileName, format);
            if (dt == null)
            {
                dt = RowType.CreateSchemaTable();
                DataRow dr = dt.NewRow();
                dr["ColumnOrdinal"] = 0;
                dr["ColumnName"] = "node";
                dr["IsContainer"] = true;
                dr["DataType"] = typeof(System.Object);
                dt.Rows.Add(dr);
                return new Resultset(new RowType(dt), 
                    new SimpleContext(reader) { RecordLimit = queryContext.LimitInputQuery });
            }
            return new Resultset(new RowType(dt), 
                new TypedContext(reader, dt) { RecordLimit = queryContext.LimitInputQuery, 
                    IgnoreConvertionErrors = format.IgnoreFormatError });
        }

        public static object OpenFile(QueryNode node, QueryContext context, string fileName, bool compressed)
        {
            var fileAccessor = new FlatFileAccessor(fileName);
            fileAccessor.Compressed = compressed;
            var accessor = new JsonDataAccessor();            
            accessor.ChildNodes.Add(fileAccessor);
            return accessor.Get(context, null);
        }
    }
}
