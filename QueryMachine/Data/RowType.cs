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

namespace DataEngine.CoreServices.Data
{   
    public class RowType
    {
        protected class ValueConverter
        {
            public T To<T>(Object value)
            {
                if (value == null || value == DBNull.Value)
                    return default(T);
                else
                    return (T)Convert.ChangeType(value, typeof(T));
            }

            public T To<T>(DataRow r, String columnName)
            {
                if (r.Table.Columns.Contains(columnName))
                    return this.To<T>(r[columnName]);
                else
                    return default(T);
            }

            public Nullable<T> ToNullable<T>(DataRow r, String columnName)
                where T : struct
            {
                if (r.Table.Columns.Contains(columnName))
                    return this.To<T>(r[columnName]);
                else
                    return null;
            }

            public Object ToDBNullable<T>(Nullable<T> value)
                where T : struct
            {
                if (value.HasValue)
                    return value.Value;
                else
                    return DBNull.Value;
            }

            public Object ToDBNullable(String value)
            {
                if (value != null)
                    return value;
                else
                    return DBNull.Value;
            }
        }

        public class TypeInfo
        {
            public readonly int Ordinal;
            public readonly string Name;
            public readonly Type DataType;
            public readonly RowType NestedType;
            public readonly int Size;
            public readonly short Precision;
            public readonly short Scale;
            public readonly bool IsLong;
            public readonly bool AllowDBNull;
            public readonly bool? IsUnique;
            public readonly bool? IsKey;
            public readonly bool? IsReadOnly;
            public readonly bool? IsRowVersion;
            public readonly bool? IsAutoIncrement;
            public readonly bool? IsAliased;
            public readonly bool? IsExpression;
            public bool IsHidden;
            public bool IsNatural;
            public readonly bool? IsIdentity;
            public readonly string BaseServerName;
            public readonly string BaseCatalogName;
            public readonly string BaseSchemaName;
            public readonly string BaseTableName;
            public readonly string BaseColumnName;
            public readonly string ProviderColumnName;
            public readonly bool IsContainer;
            public readonly bool IsCaseSensitive;
            public bool ExportAsPk;
            public bool ExportAsUnique;
            public String ExportDataType;

            public TypeInfo(String name, Type dataType, int size)
            {
                Ordinal = 0;
                Name = name;
                DataType = dataType;
                NestedType = null;
                IsExpression = true;
                Size = size;
                Precision = 0;
                Scale = 0;
                IsLong = false;
                AllowDBNull = true;
                IsUnique = null;
                IsKey = null;
                IsReadOnly = null;
                IsRowVersion = null;
                IsAutoIncrement = null;
                IsAliased = null;
                IsExpression = name == null ? 
                    (bool?)true : null;
                IsHidden = false;
                IsNatural = false;
                IsIdentity = null;
                BaseServerName = null;
                BaseCatalogName = null;
                BaseSchemaName = null;
                BaseTableName = null;
                BaseColumnName = null;
                IsContainer = false;
                IsCaseSensitive = false;
            }

            public TypeInfo(int ordinal, String name, TypeInfo src)
            {
                Ordinal = ordinal;
                Name = name;
                DataType = src.DataType;
                NestedType = src.NestedType;
                IsExpression = src.IsExpression;
                Size = src.Size;
                Precision = src.Precision;
                Scale = src.Scale;
                IsLong = src.IsLong;
                AllowDBNull = src.AllowDBNull;
                IsUnique = src.IsUnique;
                IsKey = src.IsKey;
                IsReadOnly = src.IsReadOnly;
                IsRowVersion = src.IsRowVersion;
                IsAutoIncrement = src.IsAutoIncrement;
                IsAliased = src.IsAliased;
                IsExpression = src.IsExpression;
                IsHidden = src.IsHidden;
                IsNatural = src.IsNatural;
                IsIdentity = src.IsIdentity;
                BaseServerName = src.BaseServerName;
                BaseCatalogName = src.BaseCatalogName;
                BaseSchemaName = src.BaseSchemaName;
                BaseTableName = src.BaseTableName;
                BaseColumnName = src.BaseColumnName;
                ProviderColumnName = src.ProviderColumnName;
                IsContainer = src.IsContainer;
                IsCaseSensitive = src.IsCaseSensitive;
            }

            public TypeInfo(int ordinal, TypeInfo src1, TypeInfo src2)
            {
                if (src1.DataType != src2.DataType)
                    DataType = TypeConverter.GetType(src1.DataType, src2.DataType);
                    //throw new ESQLException(Properties.Resources.InconsistentDatatypes, src1.DataType, src2.DataType);
                else
                    DataType = src1.DataType;
                if (src1.NestedType != src2.NestedType)
                    throw new ESQLException(Properties.Resources.InconsistentNestedTypes);                
                Ordinal = ordinal;
                Name = src1.Name;                
                NestedType = src1.NestedType;
                Size = Math.Min(src1.Size, src2.Size);
                Precision = Math.Min(src1.Precision, src2.Precision);
                Scale = Math.Min(src1.Scale, src2.Scale);
                IsLong = src1.IsLong || src2.IsLong;
                AllowDBNull = src1.AllowDBNull || src2.AllowDBNull;
                if (src1.IsUnique != null && src2.IsUnique != null)
                    IsUnique = src1.IsUnique.Value && src2.IsUnique.Value;
                else
                    IsUnique = null;
                if (src1.IsKey != null && src2.IsKey != null)
                    IsKey = src1.IsKey.Value && src2.IsKey.Value;
                else
                    IsKey = null;
                if (src1.IsReadOnly != null && src2.IsReadOnly != null)
                    IsReadOnly = src1.IsReadOnly.Value && src2.IsReadOnly.Value;
                else
                    IsReadOnly = null;
                if (src1.IsRowVersion != null && src2.IsRowVersion != null)
                    IsRowVersion = src1.IsRowVersion.Value && src2.IsRowVersion.Value;
                else
                    IsRowVersion = null;
                if (src1.IsAutoIncrement != null && src2.IsAutoIncrement != null)
                    IsAutoIncrement = src1.IsAutoIncrement.Value && src2.IsAutoIncrement.Value;
                else
                    IsAutoIncrement = null;
                IsAliased = src1.IsAliased;
                if (src1.IsExpression != null && src2.IsExpression != null)
                    IsExpression = src1.IsExpression.Value || src2.IsExpression.Value;
                IsHidden = src1.IsHidden && src2.IsHidden;
                IsNatural = src1.IsNatural && src2.IsNatural;
                if (src1.IsIdentity != null && src2.IsIdentity != null)
                    IsIdentity = src1.IsIdentity.Value && src2.IsIdentity.Value;
                else
                    IsIdentity = null;
                if (src1.BaseServerName == src2.BaseServerName)
                    BaseServerName = src1.BaseServerName;
                else
                    BaseServerName = null;
                if (src1.BaseCatalogName == src2.BaseCatalogName)
                    BaseCatalogName = src1.BaseCatalogName;
                else
                    BaseCatalogName = null;
                if (src1.BaseSchemaName == src2.BaseSchemaName)
                    BaseSchemaName = src1.BaseSchemaName;
                else
                    BaseSchemaName = null;
                if (src1.BaseColumnName == src2.BaseColumnName)
                    BaseColumnName = src1.BaseColumnName;
                else
                    BaseColumnName = null;
                if (src1.ProviderColumnName == src2.ProviderColumnName)
                    ProviderColumnName = src1.ProviderColumnName;
                else
                    ProviderColumnName = null;
                if (src1.IsContainer == src2.IsContainer)
                    IsContainer = src1.IsContainer;
                else
                    IsContainer = false;
                if (src1.IsCaseSensitive == src2.IsCaseSensitive)
                    IsCaseSensitive = src1.IsCaseSensitive;
                else
                    IsCaseSensitive = false;
            }
            
            public TypeInfo(TypeInfo src)
                : this(0, src.Name, src)
            {

            }

            public TypeInfo(int ordinal, String columnName, DataRow r)
            {
                ValueConverter convert = new ValueConverter();
                
                Ordinal = ordinal;
                Name = columnName; 
                
                DataType = (Type)r["DataType"];
                if (r.Table.Columns.Contains("NestedType") && !r.IsNull("NestedType"))
                    NestedType = (RowType)r["NestedType"];
                else
                    NestedType = null;

                Size = convert.To<Int32>(r["ColumnSize"]);
                Precision = convert.To<Int16>(r["NumericPrecision"]);
                Scale = convert.To<Int16>(r["NumericScale"]);                
                
                AllowDBNull = convert.To<Boolean>(r, "AllowDBNull");                
                IsLong = convert.To<Boolean>(r, "IsLong");
                IsUnique = convert.ToNullable<Boolean>(r, "IsUnique");
                IsKey = convert.ToNullable<Boolean>(r, "IsKey");
                IsReadOnly = convert.ToNullable<Boolean>(r, "IsReadOnly");
                IsRowVersion = convert.ToNullable<Boolean>(r, "IsRowVersion");
                IsAutoIncrement = convert.ToNullable<Boolean>(r, "IsAutoIncrement");
                IsAliased = convert.ToNullable<Boolean>(r, "IsAliased");
                IsExpression = convert.ToNullable<Boolean>(r, "IsExpression");
                IsHidden = convert.ToNullable<Boolean>(r, "IsHidden") ?? false;
                IsNatural = convert.ToNullable<Boolean>(r, "IsNatural") ?? false;
                IsIdentity = convert.ToNullable<Boolean>(r, "IsIdentity");

                BaseServerName = convert.To<String>(r, "BaseServerName");
                BaseSchemaName = convert.To<String>(r, "BaseSchemaName");
                BaseCatalogName = convert.To<String>(r, "BaseCatalogName");
                BaseTableName = convert.To<String>(r, "BaseTableName");
                BaseColumnName = convert.To<String>(r, "BaseColumnName");

                ProviderColumnName = convert.To<String>(r, "ProviderColumnName");
                IsContainer = convert.ToNullable<Boolean>(r, "IsContainer") ?? false;
                IsCaseSensitive = convert.ToNullable<Boolean>(r, "IsCaseSensitive") ?? false;
            }
        }

        private TypeInfo[] _fields;
        private bool _hasNestedRows;

        public struct Locator
        {
            public int master;
            public int? detail;

            public static Locator Create(int master, int detail)
            {
                Locator loc = new Locator();
                loc.master = master;
                loc.detail = detail;
                return loc;
            }
        }

        public RowType(DataTable dt)
        {
            List<TypeInfo> list = new List<TypeInfo>();
            List<string> fieldNames = new List<string>();
            foreach (DataRow row in dt.Select())
            {
                TypeInfo t = new TypeInfo(list.Count, 
                    Util.CreateUniqueName(fieldNames, row["ColumnName"].ToString()), row);
                if (t.NestedType != null)
                    _hasNestedRows = true;
                list.Add(t);                
            }
            _fields = list.ToArray();
        }

        public RowType(TypeInfo[] fields)
        {
            _fields = fields;
            foreach(TypeInfo t in _fields)
                if (t.NestedType != null)
                {
                    _hasNestedRows = true;
                    break;
                }
        }

        public static RowType CreateContainerType(Type type)
        {
            DataTable dt = RowType.CreateSchemaTable();
            DataRow r = dt.NewRow();
            r["ColumnName"] = "node";
            r["ColumnOrdinal"] = 0;
            r["DataType"] = type;
            r["IsContainer"] = true;
            dt.Rows.Add(r);
            r = dt.NewRow();
            r["ColumnName"] = "name";
            r["ColumnOrdinal"] = 1;
            r["DataType"] = typeof(System.String);
            r["IsContainer"] = true;
            dt.Rows.Add(r);
            return new RowType(dt);
        }

        public static DataTable CreateSchemaTable()
        {
            DataTable dt = new DataTable("SchemaTable");
            dt.Columns.Add("ColumnName", typeof(String));
            dt.Columns.Add("ColumnOrdinal", typeof(Int32));
            dt.Columns.Add("ColumnSize", typeof(Int32));
            dt.Columns.Add("NumericPrecision", typeof(Int16));
            dt.Columns.Add("NumericScale", typeof(Int16));
            dt.Columns.Add("DataType", typeof(Type));
            dt.Columns.Add("NestedType", typeof(RowType));
            //dt.Columns.Add("ProviderType", typeof(Int32));
            dt.Columns.Add("IsLong", typeof(Boolean));
            dt.Columns.Add("AllowDBNull", typeof(Boolean));
            dt.Columns.Add("IsUnique", typeof(Boolean));
            dt.Columns.Add("IsKey", typeof(Boolean));
            dt.Columns.Add("IsReadOnly", typeof(Boolean));
            dt.Columns.Add("IsRowVersion", typeof(Boolean));
            dt.Columns.Add("IsAutoIncrement", typeof(Boolean));
            dt.Columns.Add("IsAliased", typeof(Boolean));
            dt.Columns.Add("IsExpression", typeof(Boolean));
            dt.Columns.Add("IsHidden", typeof(Boolean));
            dt.Columns.Add("IsNatural", typeof(Boolean));
            dt.Columns.Add("IsIdentity", typeof(Boolean));
            dt.Columns.Add("BaseServerName", typeof(String));
            dt.Columns.Add("BaseSchemaName", typeof(String));
            dt.Columns.Add("BaseCatalogName", typeof(String));
            dt.Columns.Add("BaseTableName", typeof(String));
            dt.Columns.Add("BaseColumnName", typeof(String));
            dt.Columns.Add("ProviderColumnName", typeof(String));
            dt.Columns.Add("IsContainer", typeof(Boolean));
            dt.Columns.Add("IsCaseSensitive", typeof(Boolean));
            return dt;
        }

        public DataTable GetSchemaTable()
        {
            DataTable dt = CreateSchemaTable();
            ValueConverter convert = new ValueConverter();
           
            foreach (TypeInfo ti in _fields)
            {
                DataRow r = dt.NewRow();
                r["ColumnName"] = ti.Name;
                r["ColumnOrdinal"] = ti.Ordinal;
                r["ColumnSize"] = ti.Size;
                r["NumericPrecision"] = ti.Precision;
                r["NumericScale"] = ti.Scale;
                r["DataType"] = ti.DataType;
                r["NestedType"] = ti.NestedType;
                r["IsLong"] = ti.IsLong;
                r["AllowDBNull"] = ti.AllowDBNull;
                r["IsUnique"] = convert.ToDBNullable<Boolean>(ti.IsUnique);
                r["IsKey"] = convert.ToDBNullable<Boolean>(ti.IsKey);
                r["IsReadOnly"] = convert.ToDBNullable<Boolean>(ti.IsReadOnly);
                r["IsRowVersion"] = convert.ToDBNullable<Boolean>(ti.IsRowVersion);
                r["IsAutoIncrement"] = convert.ToDBNullable<Boolean>(ti.IsAutoIncrement);
                r["IsAliased"] = convert.ToDBNullable<Boolean>(ti.IsAliased);
                r["IsExpression"] = convert.ToDBNullable<Boolean>(ti.IsExpression);
                r["IsHidden"] = ti.IsHidden;
                r["IsNatural"] = ti.IsNatural;
                r["IsIdentity"] = convert.ToDBNullable<Boolean>(ti.IsIdentity);
                r["BaseServerName"] = convert.ToDBNullable(ti.BaseServerName);
                r["BaseSchemaName"] = convert.ToDBNullable(ti.BaseSchemaName);
                r["BaseCatalogName"] = convert.ToDBNullable(ti.BaseCatalogName);
                r["BaseTableName"] = convert.ToDBNullable(ti.BaseTableName);
                r["BaseColumnName"] = convert.ToDBNullable(ti.BaseColumnName);
                r["ProviderColumnName"] = convert.ToDBNullable(ti.ProviderColumnName);
                r["IsContainer"] = ti.IsContainer;
                r["IsCaseSensitive"] = ti.IsCaseSensitive;
                dt.Rows.Add(r);
            }

            return dt;
        }

        public bool RowTypeEquals(RowType dest)
        {
            if (Object.ReferenceEquals(dest, this))
                return true;
            else
                if (dest.Fields.Length == Fields.Length)
                {
                    for (int k = 0; k < Fields.Length; k++)
                        if (Fields[k].DataType != dest.Fields[k].DataType)
                            return false;
                    return true;
                }
                else
                    return false;
        }

        public int GetOrdinal(String name)
        {
            foreach (TypeInfo t in Fields)
                if (t.Name.Equals(name))
                    return t.Ordinal;
            throw new ESQLException(Properties.Resources.InvalidIdentifier, name);
        }

        public TypeInfo[] Fields
        {
            get
            {
                return _fields;
            }
        }

        public bool HasNestedRows
        {
            get
            {
                return _hasNestedRows;
            }
        }
    }
}
