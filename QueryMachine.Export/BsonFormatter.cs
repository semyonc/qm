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
using System.Xml;

using MongoDB.Bson;

using DataEngine;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

#if JS
using Noesis.Javascript;
#endif


namespace DataEngine.Export
{
    public class BsonFormatter: IDisposable
    {
        public static string CreateJavascriptTemplate(RowType rowType)
        {
            DataProviderHelper helper = new DataProviderHelper();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            for (int k = 0; k < rowType.Fields.Length; k++)
            {
                if (k > 0)
                {
                    sb.Append(",");
                    sb.AppendLine();
                }
                RowType.TypeInfo ti = rowType.Fields[k];
                String name = Util.UnquoteName(ti.Name);
                sb.Append("'");
                sb.Append(name);
                sb.Append("'");
                sb.Append(": ");
                bool convert = false;
                if (helper.RequiresQuotingNoKeywords(name))
                {
                    sb.Append("this['");
                    sb.Append(name);
                    sb.Append("']");
                }
                else
                {
                    sb.Append("this.");
                    sb.Append(name);
                }
                if (convert)
                    sb.Append(")");
            }
            sb.AppendLine();
            sb.AppendLine("}");
            return sb.ToString();
        }

#if JS            
        private string _jsonScript;
        private JavascriptContext _context;
#endif
        private Dictionary<String, Object> _this;

        public BsonFormatter(string jsonScript)
        {
#if JS
            _context = new JavascriptContext();
            _jsonScript = "(function (){ return " + jsonScript + "; }).apply(curr, [])";
#endif
            _this = new Dictionary<string, object>();
        }

        ~BsonFormatter()
        {
            Dispose(false);
        }

        #region IDisposable Members        

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        protected bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
#if JS
                _context.Dispose();
#endif
                _disposed = true;
            }
        }

        public BsonDocument Format(RowType rt, Row row)
        {            
#if JS
            LoadData(_this, rt, row);
            _context.SetParameter("curr", _this);
            Dictionary<String, Object> obj = _context.Run(_jsonScript) as Dictionary<String, Object>;
            if (obj == null)
                throw new ESQLException("Invalid javascript template block", null);
            _this.Clear();
            return ToBsonDocument(obj);
#else
            LoadData(_this, rt, row);
            return ToBsonDocument(_this);
#endif
        }

        private BsonDocument ToBsonDocument(Dictionary<String, Object> value)
        {
            BsonDocument res = new BsonDocument();
            foreach (KeyValuePair<String, Object> item in value)
                res.Add(item.Key, ToBsonValue(item.Value));
            return res;
        }

        private BsonArray ToBsonArray(List<Object> value)
        {
            BsonArray res = new BsonArray();
            foreach (object item in value)
                res.Add(ToBsonValue(item));
            return res;
        }

        private BsonValue ToBsonValue(object value)
        {
            if (value == null)
                return BsonNull.Value;
            else
                if (value is Dictionary<String, Object>)
                    return ToBsonDocument((Dictionary<String, Object>)value);
                else if (value is List<Object>)
                    return ToBsonArray((List<Object>)value);
                else
                    return BsonValue.Create(value);
        }

        private void LoadData(Dictionary<String, Object> rec, RowType rt, Row row)
        {
            for (int k = 0; k < rt.Fields.Length; k++)
            {
                string name = Util.UnquoteName(rt.Fields[k].Name);
                RowType.TypeInfo ti = rt.Fields[k];
                if (row.IsDbNull(k))
                {
                    rec[name] = null;
                    continue;
                }
                switch (Type.GetTypeCode(ti.DataType))
                {
                    case TypeCode.Boolean:
                    case TypeCode.DateTime:
                    case TypeCode.Int32:
                    case TypeCode.Double:
                    case TypeCode.String:
                    case TypeCode.Char:
                        rec[name] = row.GetValue(k);
                        break;

                    case TypeCode.Single:
                    case TypeCode.Decimal:
                    case TypeCode.Int64:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        rec[name] = Convert.ToDouble(row.GetValue(k));
                        break;

                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                        rec[name] = Convert.ToInt32(row.GetValue(k));
                        break;

                    default:
                        {
                            object value = row.GetValue(k);
                            if (value is Resultset)
                                rec[name] = LoadResultset((Resultset)value);
                            else if (value is ValueTuple)
                                rec[name] = LoadValueTuple((ValueTuple)value);
                            else if (value is XmlNode)
                                rec[name] = ((XmlNode)value).InnerXml;
                            else
                                rec[name] = value.ToString();
                        }
                        break;
                }
            }
        }

        private List<Object> LoadResultset(Resultset rs)
        {
            List<Object> res = new List<object>();
            while (rs.Begin != null)
            {
                Dictionary<String, Object> rec = new Dictionary<string, object>();
                LoadData(rec, rs.RowType, rs.Dequeue());
                res.Add(rec);
            }
            return res;
        }

        private Dictionary<String, Object> LoadValueTuple(ValueTuple tuple)
        {
            Dictionary<String, Object> rec = new Dictionary<string, object>();
            foreach (KeyValuePair<String, Object> kvp in tuple.Values)
            {
                string name = kvp.Key;
                if (kvp.Value == null)
                {
                    rec[name] = null;
                    continue;
                }
                switch (Type.GetTypeCode(kvp.Value.GetType()))
                {
                    case TypeCode.Boolean:
                    case TypeCode.DateTime:
                    case TypeCode.Int32:
                    case TypeCode.Double:
                    case TypeCode.String:
                    case TypeCode.Char:
                        rec[name] = kvp.Value;
                        break;

                    case TypeCode.Single:
                    case TypeCode.Decimal:
                    case TypeCode.Int64:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        rec[name] = Convert.ToDouble(kvp.Value);
                        break;

                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                        rec[name] = Convert.ToInt32(kvp.Value);
                        break;

                    default:
                        {
                            object value = kvp.Value;
                            if (value is Resultset)
                                rec[name] = LoadResultset((Resultset)value);
                            else if (value is ValueTuple)
                                rec[name] = LoadValueTuple((ValueTuple)value);
                            else
                                rec[name] = value.ToString();
                        }
                        break;
                }
            }
            return rec;
        }        
    }

}
