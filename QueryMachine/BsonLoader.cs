//        Copyright (c) 2012, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.


using System;
using System.Collections.Generic;
using System.Data;
using MongoDB.Bson;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;


namespace DataEngine
{
    class BsonLoader
    {
        public BsonLoader()
        {
        }

        public void LoadRecord(Resultset rs, BsonDocument doc)
        {
            Row row = rs.NewRow();
            for (int k = 0; k < rs.RowType.Fields.Length; k++)
            {
                RowType.TypeInfo ti = rs.RowType.Fields[k];
                BsonValue value;
                if (doc.TryGetValue(ti.ProviderColumnName, out value))
                    row.SetValue(k, GetValue(ti.ProviderColumnName, value));
            }
            rs.Enqueue(row);
        }

        public object GetValue(string itemName, BsonValue value)
        {
            switch (value.BsonType)
            {
                case BsonType.Null:
                    return DBNull.Value;

                case BsonType.Boolean:
                    return value.AsBoolean;

                case BsonType.Binary:
                    return value.AsByteArray;

                case BsonType.DateTime:
                    return value.AsDateTime;

                case BsonType.Int32:
                case BsonType.Double:
                    return Convert.ToDouble(value.RawValue);

                case BsonType.Int64:
                    return value.AsInt64;

                case BsonType.Timestamp:
                    return Service.ConvertTimestamp(value.AsBsonTimestamp.Value);

                case BsonType.Document:
                    return CreateTupleFromDocument(value.AsBsonDocument);

                case BsonType.Array:
                    if (value.AsBsonArray.Count == 0)
                        return DBNull.Value;
                    return CreateNestedTable(itemName, value.AsBsonArray);

                default:
                    return value.ToString();
            }
        }

        public ValueTuple CreateTupleFromDocument(BsonDocument doc)
        {
            Dictionary<String, Object> values = new Dictionary<string, object>();
            foreach (BsonElement element in doc.Elements)
            {
                switch (element.Value.BsonType)
                {
                    case BsonType.ObjectId:
                    case BsonType.String:
                    case BsonType.Symbol:
                    case BsonType.RegularExpression:
                    case BsonType.JavaScript:
                    case BsonType.JavaScriptWithScope:
                        values.Add(element.Name, element.Value.ToString());
                        break;

                    case BsonType.Boolean:
                        values.Add(element.Name, element.Value.AsBoolean);
                        break;

                    case BsonType.Binary:
                        values.Add(element.Name, element.Value.AsByteArray);
                        break;

                    case BsonType.DateTime:
                        values.Add(element.Name, element.Value.AsDateTime);
                        break;

                    case BsonType.Int32:
                    case BsonType.Double:
                        values.Add(element.Name, Convert.ToDouble(element.Value.RawValue));
                        break;

                    case BsonType.Int64:
                        values.Add(element.Name, element.Value.AsInt64);
                        break;

                    case BsonType.Timestamp:
                        values.Add(element.Name, Service.ConvertTimestamp(
                            element.Value.AsBsonTimestamp.Value));
                        break;

                    case BsonType.Document:
                        values.Add(element.Name,
                            CreateTupleFromDocument(element.Value.AsBsonDocument));
                        break;

                    case BsonType.Array:
                        if (element.Value.AsBsonArray.Count == 0)
                            values.Add(element.Name, DBNull.Value);
                        else
                            values.Add(element.Name,
                                CreateNestedTable(element.Name, element.Value.AsBsonArray));
                        break;

                    default:
                        values.Add(element.Name, DBNull.Value);
                        break;
                }
            }
            return new ValueTuple("BsonDocument", values);
        }

        private void CreateNestedField(string name, BsonType valueType, List<String> fieldNames,
            Dictionary<String, BsonType> bsonTypes)
        {
            BsonType bt;
            if (!bsonTypes.TryGetValue(name, out bt))
            {
                bsonTypes.Add(name, valueType);
                fieldNames.Add(name);
            }
            else
            {
                if (bt != BsonType.Undefined && bt != valueType)
                {
                    if (bt == BsonType.Null)
                        bsonTypes[name] = valueType;
                    else
                        if (valueType != BsonType.Null)
                            bsonTypes[name] = BsonType.Undefined;
                }
            }
        }

        private Resultset CreateNestedTable(string itemName, BsonArray array)
        {
            List<String> fieldNames = new List<string>();
            Dictionary<string, BsonType> bsonTypes = new Dictionary<string, BsonType>();
            foreach (BsonValue curr in array.Values)
            {
                BsonDocument doc = curr as BsonDocument;
                if (doc == null)
                    CreateNestedField(itemName, curr.BsonType, fieldNames, bsonTypes);
                else
                    foreach (BsonElement elem in doc)
                        CreateNestedField(elem.Name, elem.Value.BsonType, fieldNames, bsonTypes);
            }
            DataTable dt = RowType.CreateSchemaTable();
            for (int k = 0; k < fieldNames.Count; k++)
            {
                string name = fieldNames[k];
                BsonType bt = bsonTypes[name];
                Type dataType;
                switch (bt)
                {
                    case BsonType.ObjectId:
                    case BsonType.String:
                    case BsonType.Symbol:
                    case BsonType.RegularExpression:
                    case BsonType.JavaScript:
                    case BsonType.JavaScriptWithScope:
                        dataType = typeof(String);
                        break;

                    case BsonType.Boolean:
                        dataType = typeof(Boolean);
                        break;

                    case BsonType.Binary:
                        dataType = typeof(byte[]);
                        break;

                    case BsonType.DateTime:
                        dataType = typeof(DateTime);
                        break;

                    case BsonType.Int32:
                    case BsonType.Double:
                        dataType = typeof(Double);
                        break;

                    case BsonType.Int64:
                        dataType = typeof(Int64);
                        break;

                    case BsonType.Timestamp:
                        dataType = typeof(TimeSpan);
                        break;

                    default:
                        dataType = typeof(System.Object);
                        break;
                }
                DataRow r = dt.NewRow();
                r["ColumnName"] = name;
                r["ProviderColumnName"] = name;
                r["ColumnOrdinal"] = k;
                r["DataType"] = dataType;
                r["AllowDBNull"] = true;
                r["IsCaseSensitive"] = false;
                dt.Rows.Add(r);
            }
            int indexOf = -1;
            Resultset rs = new Resultset(new RowType(dt), null);
            rs.Persistent = true;
            foreach (BsonValue value in array)
            {
                BsonDocument doc = value as BsonDocument;
                if (doc == null)
                {
                    Row row = rs.NewRow();
                    if (indexOf == -1)
                        indexOf = fieldNames.IndexOf(itemName);
                    row.SetValue(indexOf, GetValue(itemName, value));
                    rs.Enqueue(row);
                }
                else
                    LoadRecord(rs, doc);
            }
            return rs;
        }
    }    
}
