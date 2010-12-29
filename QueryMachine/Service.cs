using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.DirectoryServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{

    public partial class ID
    {

        public static readonly object EQ = ATOM.Create("eqx");
        public static readonly object NE = ATOM.Create("nex");
        public static readonly object LT = ATOM.Create("ltx");
        public static readonly object GT = ATOM.Create("gtx");
        public static readonly object GE = ATOM.Create("gex");
        public static readonly object LE = ATOM.Create("lex");

        public static readonly object Between = ATOM.Create("between");
        public static readonly object Case = ATOM.Create("case");
        public static readonly object NullIf = ATOM.Create("nullif");
        public static readonly object IsTrue = ATOM.Create("IsTrue");
        public static readonly object IsFalse = ATOM.Create("IsFalse");
        public static readonly object IsUnknown = ATOM.Create("IsUnknown");

        public static readonly object FirstDate = ATOM.Create("$fdate");
        public static readonly object GetDay = ATOM.Create("$get_day");
        public static readonly object GetMonth = ATOM.Create("$get_month");
        public static readonly object GetYear = ATOM.Create("$get_year");
        public static readonly object ToDateTime = ATOM.Create("$to_datetime");
        public static readonly object ToDecimal = ATOM.Create("$to_decimal");
        public static readonly object ToDouble = ATOM.Create("$to_double");
        public static readonly object ToInt16 = ATOM.Create("$to_int16");
        public static readonly object ToInt32 = ATOM.Create("$to_int32");
        public static readonly object ToInt64 = ATOM.Create("$to_int64");
        public static readonly object ToSingle = ATOM.Create("$to_float");
        new public static readonly object ToString = ATOM.Create("$to_string");
        public static readonly object Base64Decode = ATOM.Create("$base64decode");
        public static readonly object Base64Encode = ATOM.Create("$base64encode");
        public static readonly object Coalesce = ATOM.Create("coalesce");
        public static readonly object Concat = ATOM.Create("concat");
        public static readonly object Format = ATOM.Create("$format");
        public static readonly object GetFileName = ATOM.Create("$getfilename");
        public static readonly object GetFilePath = ATOM.Create("$getfilepath");
        public static readonly object GetMachineName = ATOM.Create("$machinename");
        public static readonly object GetUserDomainName = ATOM.Create("$userdomain");
        public static readonly object GetUserName = ATOM.Create("$username");
        public static readonly object RegEx = ATOM.Create("$regex");
        public static readonly object Like1 = ATOM.Create("$like1");
        public static readonly object Like2 = ATOM.Create("$like2");
        public static readonly object IsNull = ATOM.Create("IsNull");
        public static readonly object StrConvert = ATOM.Create("strconv");
        public static readonly object StrLower = ATOM.Create("strlow");
        public static readonly object StrUpper = ATOM.Create("strup");
        public static readonly object StrPos = ATOM.Create("strpos");
        public static readonly object Substr = ATOM.Create("substr");
        public static readonly object Trim = ATOM.Create("trim");
        public static readonly object TrimStart = ATOM.Create("ltrim");
        public static readonly object TrimEnd = ATOM.Create("rtrim");
        public static readonly object ForAll = ATOM.Create("fall");
        public static readonly object ForAny = ATOM.Create("fany");
        public static readonly object Exist = ATOM.Create("exist");
        public static readonly object Member = ATOM.Create("member");
        public static readonly object ParamRef = ATOM.Create("pref");
        public static readonly object RowNum = ATOM.Create("$rownum");
        public static readonly object SingleColumn = ATOM.Create("scol");
        public static readonly object SingleRow = ATOM.Create("srow");
        public static readonly object SQuery = ATOM.Create("squery");
        public static readonly object GetSysTab = ATOM.Create("$systab");
        public static readonly object QueryLdapProps = ATOM.Create("$ldap_props");
        public static readonly object QueryLdap = ATOM.Create("$ldap");
        public static readonly object At = ATOM.Create("$at");
        public static readonly object Dref = ATOM.Create("$Dref");
        public static readonly object Wref = ATOM.Create("$Wref");
        public static readonly object Dyncast = ATOM.Create("$dyncat");
        public static readonly object Extract = ATOM.Create("$extract");
        public static readonly object NodeText = ATOM.Create("$text");
        public static readonly object Parse = ATOM.Create("$parse");
        public static readonly object Rval = ATOM.Create("$rval");
    }

    public class Service
    {
        static Service()
        {
            GlobalSymbols.DefineStaticOperator(ID.FirstDate, typeof(Service), "FirstDate");
            GlobalSymbols.DefineStaticOperator(ID.GetDay, typeof(Service), "GetDay");
            GlobalSymbols.DefineStaticOperator(ID.GetMonth, typeof(Service), "GetMonth");
            GlobalSymbols.DefineStaticOperator(ID.GetYear, typeof(Service), "GetYear");
            GlobalSymbols.DefineStaticOperator(ID.ToDateTime, typeof(Service), "ToDateTime");
            GlobalSymbols.DefineStaticOperator(ID.ToDecimal, typeof(Service), "ToDecimal");
            GlobalSymbols.DefineStaticOperator(ID.ToInt16, typeof(Service), "ToInt16");
            GlobalSymbols.DefineStaticOperator(ID.ToInt32, typeof(Service), "ToInt32");
            GlobalSymbols.DefineStaticOperator(ID.ToInt64, typeof(Service), "ToInt64");
            GlobalSymbols.DefineStaticOperator(ID.ToSingle, typeof(Service), "ToSingle");
            GlobalSymbols.DefineStaticOperator(ID.ToString, typeof(Service), "ToString");
            GlobalSymbols.DefineStaticOperator(ID.Base64Decode, typeof(Service), "Base64Decode");
            GlobalSymbols.DefineStaticOperator(ID.Base64Encode, typeof(Service), "Base64Encode");
            GlobalSymbols.DefineStaticOperator(ID.Coalesce, typeof(Service), "Coalesce");
            GlobalSymbols.DefineStaticOperator(ID.Concat, typeof(Service), "Concat");
            GlobalSymbols.DefineStaticOperator(ID.Format, typeof(Service), "Format");
            GlobalSymbols.DefineStaticOperator(ID.GetFileName, typeof(Service), "GetFileName");
            GlobalSymbols.DefineStaticOperator(ID.GetFilePath, typeof(Service), "GetFilePath");
            GlobalSymbols.DefineStaticOperator(ID.GetMachineName, typeof(Service), "GetMachineName");
            GlobalSymbols.DefineStaticOperator(ID.GetUserDomainName, typeof(Service), "GetUserDomainName");
            GlobalSymbols.DefineStaticOperator(ID.GetUserName, typeof(Service), "GetUserName");
            GlobalSymbols.DefineStaticOperator(ID.RegEx, typeof(Service), "RegEx");
            GlobalSymbols.DefineStaticOperator(ID.Like1, typeof(Service), "Like1");
            GlobalSymbols.DefineStaticOperator(ID.Like2, typeof(Service), "Like2");
            GlobalSymbols.DefineStaticOperator(ID.StrConvert, typeof(Service), "StrConvert");
            GlobalSymbols.DefineStaticOperator(ID.StrLower, typeof(Service), "StrLower");
            GlobalSymbols.DefineStaticOperator(ID.StrUpper, typeof(Service), "StrUpper");
            GlobalSymbols.DefineStaticOperator(ID.StrPos, typeof(Service), "StrPos");
            GlobalSymbols.DefineStaticOperator(ID.Substr, typeof(Service), "Substr");
            GlobalSymbols.DefineStaticOperator(ID.Trim, typeof(Service), "Trim");
            GlobalSymbols.DefineStaticOperator(ID.TrimStart, typeof(Service), "TrimStart");
            GlobalSymbols.DefineStaticOperator(ID.TrimEnd, typeof(Service), "TrimEnd");
            GlobalSymbols.DefineStaticOperator(ID.ForAll, typeof(Service), "ForAll");
            GlobalSymbols.DefineStaticOperator(ID.ForAny, typeof(Service), "ForAny");
            GlobalSymbols.DefineStaticOperator(ID.Exist, typeof(Service), "Exist");
            GlobalSymbols.DefineStaticOperator(ID.Member, typeof(Service), "Member");
            GlobalSymbols.DefineStaticOperator(ID.ParamRef, typeof(Service), "ParamRef");
            GlobalSymbols.DefineStaticOperator(ID.RowNum, typeof(Service), "RowNum");
            GlobalSymbols.DefineStaticOperator(ID.SingleColumn, typeof(Service), "SingleColumn");
            GlobalSymbols.DefineStaticOperator(ID.SingleRow, typeof(Service), "SingleRow");
            GlobalSymbols.DefineStaticOperator(ID.SQuery, typeof(Service), "SQuery");
            GlobalSymbols.DefineStaticOperator(ID.GetSysTab, typeof(Service), "GetSysTab");
            GlobalSymbols.DefineStaticOperator(ID.QueryLdapProps, typeof(Service), "QueryLdapProps");
            GlobalSymbols.DefineStaticOperator(ID.QueryLdap, typeof(Service), "QueryLdap");
            GlobalSymbols.DefineStaticOperator(ID.At, typeof(Service), "At");
            GlobalSymbols.DefineStaticOperator(ID.Dref, typeof(Service), "Dref");
            GlobalSymbols.DefineStaticOperator(ID.Wref, typeof(Service), "Wref");
            GlobalSymbols.DefineStaticOperator(ID.Dyncast, typeof(Service), "Dyncast");
            GlobalSymbols.DefineStaticOperator(ID.Extract, typeof(Service), "Extract");
            GlobalSymbols.DefineStaticOperator(ID.NodeText, typeof(Service), "NodeText");
            GlobalSymbols.DefineStaticOperator(ID.Parse, typeof(Service), "Parse");
            GlobalSymbols.DefineStaticOperator(ID.Rval, typeof(Service), "Rval");

            GlobalSymbols.DefineStaticOperator("#cmp", typeof(Service), "ExpandRowConstructor");

            GlobalSymbols.Defmacro(ID.EQ, "(a b)", "(#cmp 'eq a b)");
            GlobalSymbols.Defmacro(ID.NE, "(a b)", "(#cmp 'ne a b)");
            GlobalSymbols.Defmacro(ID.LT, "(a b)", "(#cmp 'lt a b)");
            GlobalSymbols.Defmacro(ID.GT, "(a b)", "(#cmp 'gt a b)");
            GlobalSymbols.Defmacro(ID.GE, "(a b)", "(#cmp 'ge a b)");
            GlobalSymbols.Defmacro(ID.LE, "(a b)", "(#cmp 'le a b)");

            GlobalSymbols.Defun("#ec", "(x l c)", @"
                 (cond ((null l) (cons (list 't c))) 
                        (t (append (cons (list (list 'eq x (caar l)) (cdar (car l)))) (#ec x (cdr l) c)))
                  )");

            GlobalSymbols.Defmacro(ID.Between, "(a b c)", "(list 'and (list 'le b a) (list 'le a c))");
            GlobalSymbols.Defmacro(ID.NullIf, "(a b)", "(list 'cond (list (list 'eq a b) 'DBNull) (list 't a))");
            GlobalSymbols.Defmacro(ID.Case, "(a b c)", "(list 'let (list (list 'x a)) (append (cons 'cond) (#ec 'x b c)))");
            GlobalSymbols.Defmacro(ID.IsNull, "(a)", "(list 'null (list 'weak a))");
            GlobalSymbols.Defmacro(ID.IsTrue, "(a)", "(list 'not (list 'null (list 'lambda-qoute a)))");
            GlobalSymbols.Defmacro(ID.IsFalse, "(a)", "(list 'null (list 'lambda-qoute a))");
            GlobalSymbols.Defmacro(ID.IsUnknown, "(a)", "(list 'eq (list 'lambda-qoute a) 'unknown)");
        }

        public static void Initialize()
        {
            return;
        }

        public static object ExpandRowConstructor(object id, object x, object y)
        {
            if (Util.IsRowConstuctor(x) && Util.IsRowConstuctor(y))
            {
                object[] row1 = Lisp.ToArray(x);
                object[] row2 = Lisp.ToArray(y);
                if (row1.Length != row2.Length)
                    throw new ESQLException(Properties.Resources.NotEnoughValues);
                object expr = null;
                for (int k = 0; k < row1.Length; k++)
                    if (expr == null)
                        expr = Lisp.List(id, row1[k], row2[k]);
                    else
                        expr = Lisp.List(Funcs.And, expr, Lisp.List(id, row1[k], row2[k]));
                return expr;
            }
            else
                return Lisp.List(id, x, y);
        }


        public static DateTime FirstDate(DateTime d)
        {
            return new DateTime(d.Year, d.Month, 1);
        }

        public static int GetDay(DateTime d)
        {
            return d.Day;
        }

        public static int GetMonth(DateTime d)
        {
            return d.Month;
        }

        public static int GetYear(DateTime d)
        {
            return d.Year;
        }

        public static DateTime ToDateTime(object o)
        {
            return Convert.ToDateTime(o);
        }

        public static Decimal ToDecimal(object o)
        {
            return Convert.ToDecimal(o);
        }

        public static Double ToDouble(object o)
        {
            return Convert.ToDouble(o);
        }

        public static Int16 ToInt16(object o)
        {
            return Convert.ToInt16(o);
        }

        public static Int32 ToInt32(object o)
        {
            return Convert.ToInt32(o);
        }

        public static Int64 ToInt64(object o)
        {
            return Convert.ToInt64(o);
        }

        public static Single ToSingle(object o)
        {
            return Convert.ToSingle(o);
        }

        public static String ToString(object o)
        {
            return Convert.ToString(o);
        }

        public static String ToString(object o, int len)
        {
            return Util.Substring(Convert.ToString(o), 0, len);
        }

        public static byte[] Base64Decode(String str)
        {
            return Convert.FromBase64String(str);
        }

        public static String Base64Encode(Byte[] raw)
        {
            return Convert.ToBase64String(raw);
        }

        public static object Coalesce(object[] paramsArray)
        {
            for (int k = 0; k < paramsArray.Length; k++)
                if (paramsArray[k] != DBNull.Value)
                    return paramsArray[k];
            return DBNull.Value;
        }

        public static String Concat(String str1, String str2)
        {
            if (str2 == null)
                return str1;
            else if (str1 == null)
                return str2;
            else 
                return str1.ToString() + str2.ToString();
        }

        public static String Format(String format)
        {
            return String.Format(format);
        }

        public static String Format(String format, params object[] paramsArray)
        {
            return String.Format(format, paramsArray);
        }

        public static String GetFileName(String path)
        {
            return Path.GetFileName(path);
        }

        public static String GetFilePath(String path)
        {
            return Path.GetDirectoryName(path);
        }

        public static String GetMachineName()
        {
            return Environment.MachineName;
        }

        public static String GetUserDomainName()
        {
            return Environment.UserDomainName;
        }

        public static String GetUserName()
        {
            return Environment.UserName;
        }

        public static String RegEx(string input, string pattern)
        {
            if (input != null)
            {
                Regex regex = new Regex(pattern);
                var match = regex.Match(input);
                if (match.Success)
                    return input.Substring(match.Index, match.Length);
            }
            return null;
        }

        public static bool Like1(string str, LikeOperator like)
        {
            if (str == null)
                return false;
            return like.Compare(str);
        }

        public static bool Like2(string str, string pattern, string escape_str)
        {
            if (str == null)
                return false;
            char escape_char = '\0';
            if (escape_str.Length > 1)
               throw new ESQLException(Properties.Resources.EscapeCharToLong);
            escape_char = escape_str[0];
            LikeOperator like = new LikeOperator(pattern, escape_char,
                false, System.Globalization.CultureInfo.CurrentCulture);
            return like.Compare(str);
        }

        public static String StrConvert(string str, string collation)
        {
            throw new NotImplementedException();
        }

        public static String StrLower(string str)
        {
            return str.ToLower();
        }

        public static String StrUpper(string str)
        {
            return str.ToUpper();
        }

        public static object StrPos(string str, string substr)
        {
            int pos = str.IndexOf(substr);
            if (pos == -1)
                return null;
            else
                return pos + 1;
        }

        public static String Substr(string str, int index, int len)
        {
            return Util.Substring(str, index - 1, len);
        }

        public static String Trim(string str, string trim_chars)
        {
            return str.Trim(trim_chars.ToCharArray());
        }

        public static String TrimStart(string str, string trim_chars)
        {
            return str.TrimStart(trim_chars.ToCharArray());
        }

        public static String TrimEnd(string str, string trim_chars)
        {
            return str.TrimStart(trim_chars.ToCharArray());
        }

        public static bool ForAll([Implict] Executive engine, Resultset rs, object param, object body)        
        {
            FunctionLink compiledBody = new FunctionLink();
            Executive.Parameter[] parameters = new Executive.Parameter[1];
            parameters[0].ID = param;
            parameters[0].Type = typeof(System.Object);
            object[] args = new object[1];
            while (rs.Begin != null)
            {
                Row row = rs.Dequeue();
                if (row.HasNullValues)
                {
                    rs.Cancel();
                    return false;
                }
                else
                {
                    if (row.Length == 1)
                        args[0] = row.ItemArray[0];
                    else
                        args[0] = Lisp.List(row.ItemArray);
                    object res = engine.Apply(null, parameters, body, args, compiledBody, engine.DefaultPool);
                    if (res == null || res == Undefined.Value)
                    {
                        rs.Cancel();
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ForAny([Implict] Executive engine, Resultset rs, object param, object body)        
        {
            FunctionLink compiledBody = new FunctionLink();
            Executive.Parameter[] parameters = new Executive.Parameter[1];
            parameters[0].ID = param;
            parameters[0].Type = typeof(System.Object);
            object[] args = new object[1];
            while (rs.Begin != null)
            {
                Row row = rs.Dequeue();
                if (!row.HasNullValues)
                {
                    if (row.Length == 1)
                        args[0] = row.ItemArray[0];
                    else
                        args[0] = Lisp.List(row.ItemArray);
                    object res = engine.Apply(null, parameters, body, args, compiledBody, engine.DefaultPool);
                    if (res != null && res != Undefined.Value)
                    {
                        rs.Cancel();
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool Exist(Resultset rs)
        {
            Row row = rs.Begin;
            rs.Cancel();
            return row != null;
        }

        public static bool Member([Implict] Executive engine, object tuple, Resultset rs)
        {
            object[] value;
            if (Lisp.IsCons(tuple))
                value = Lisp.ToArray(tuple);
            else
            {
                value = new object[1];
                value[0] = tuple;
            }
            if (rs.RowType.Fields.Length != value.Length)
                throw new ESQLException(Properties.Resources.NotEnoughValues);
            while (rs.Begin != null)
            {
                Row row = rs.Dequeue();
                if (!row.HasNullValues)
                {
                    bool found = true;
                    for (int k = 0; k < value.Length; k++)
                        if (Runtime.DynamicEq(engine, value[k], row[k]) == null)
                        {
                            found = false;
                            break;
                        }
                    if (found)
                    {
                        rs.Cancel();
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool Member([Implict] Executive engine, object tuple, object values)
        {
            if (Lisp.IsNode(tuple))
            {
                foreach (object value in Lisp.getIterator(values))
                    if (Runtime.DynamicEq(engine, tuple, value) != null)
                        return true;
                return false;
            }
            else
            {
                foreach (object item in Lisp.getIterator(tuple))
                {
                    bool res = Member(engine, item, values);
                    if (!res)
                        return false;
                }
                return true;
            }
        }

        public static object ParamRef([Implict] Executive engine, int num)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            return owner.Parameters[num - 1];
        }

        public static int RowNum([Implict] Executive engine)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            if (!(owner is DataSelector.SelectorContext))
                throw new ESQLException(Properties.Resources.UnexpectedRownum);
            return ((DataSelector.SelectorContext)owner).RowNum;
        }

        public static object SingleColumn(object value)
        {
            if (Lisp.IsNode(value))
                return value;
            else
                throw new ESQLException(Properties.Resources.TooManyValues);
        }

        public static object SingleRow(Resultset rs)
        {
            if (rs.Begin != null && rs.NextRow(rs.Begin) != null)
                throw new ESQLException(Properties.Resources.SingleRowSubquery);
            if (rs.RowType.Fields.Length == 1)
                return rs.Begin[0];
            else
                return Lisp.List(rs.Begin.ItemArray);
        }

        public static Resultset SQuery([Implict] Executive engine, object nodeID, object paramlist)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            QueryNode node = owner.Node.GetNodeByID(nodeID);
            object[] parameters = null;
            if (paramlist != null)
                parameters = Lisp.ToArray(paramlist);
            return node.Get(owner.QueryContext, parameters);
        }

        public static Resultset GetSysTab([Implict] Executive engine, string dataSource, string collection)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            DataSourceInfo dsi = owner.QueryContext.DatabaseDictionary.GetDataSource(dataSource);
            if (dsi != null)
            {
                DbConnection conn = DataProviderHelper.CreateDbConnection(dsi.ProviderInvariantName);
                conn.ConnectionString = dsi.ConnectionString;
                conn.Open();
                try
                {
                    DataTable dt;
                    if (String.IsNullOrEmpty(collection))
                        dt = conn.GetSchema();
                    else
                        dt = conn.GetSchema(collection);
                    AdoTableAccessor accessor = new AdoTableAccessor(dt);
                    return accessor.Get(owner.QueryContext, owner.Parameters);
                }
                finally
                {
                    conn.Close();
                }
            }
            else
                throw new ESQLException(Properties.Resources.UnknownDataSource, dataSource);
        }

        public static Resultset QueryLdapProps([Implict] Executive engine, String domainName, String filter)
        {
            if (String.IsNullOrEmpty(domainName))
                domainName = String.Format("LDAP://{0}", Environment.UserDomainName);
            DirectoryEntry entry = new DirectoryEntry(domainName);
            DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.CacheResults = true;
            searcher.Filter = filter;
            searcher.PropertyNamesOnly = true;
            DataTable dt = RowType.CreateSchemaTable();
            DataRow dr = dt.NewRow();
            dr["ColumnName"] = "Name";
            dr["ColumnOrdinal"] = 1;
            dr["DataType"] = typeof(System.String);
            dt.Rows.Add(dr);
            SearchResultCollection result = searcher.FindAll();
            HashSet<String> hs = new HashSet<string>();
            foreach (SearchResult sr in result)
            {
                foreach (string name in sr.Properties.PropertyNames)
                    if (!hs.Contains(name))
                        hs.Add(name);
            }
            Resultset rs = new Resultset(new RowType(dt), null);
            foreach (String s in hs)
            {
                Row row = rs.NewRow();
                row.SetString(0, s);
                rs.Enqueue(row);
            }
            return rs;
        }

        public static Resultset QueryLdap([Implict] Executive engine, String domainName, String filter, String properties)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            if (String.IsNullOrEmpty(domainName))
                domainName = String.Format("LDAP://{0}", Environment.UserDomainName);
            DirectoryEntry entry = new DirectoryEntry(domainName);
            DirectorySearcher searcher = new DirectorySearcher(entry);
            if (owner.QueryContext.LdapClientTimeout != TimeSpan.Zero)
                searcher.ClientTimeout = owner.QueryContext.LdapClientTimeout;
            searcher.SizeLimit = owner.QueryContext.LdapSearchLimit;
            searcher.CacheResults = true;
            searcher.Filter = filter;
            string[] properties_array = properties.Split(new char[] { ',' });
            foreach (string prop in properties_array)
                searcher.PropertiesToLoad.Add(prop);
            Type[] dataType = new Type[properties_array.Length];
            bool[] is_array = new bool[properties_array.Length];
            SearchResultCollection result = searcher.FindAll();
            for (int p = 0; p < dataType.Length; p++)
            {
                dataType[p] = null;
                foreach (SearchResult sr in result)
                {
                    ResultPropertyValueCollection value_col = sr.Properties[properties_array[p]];
                    if (value_col.Count > 0)
                    {
                        Type curr;
                        is_array[p] |= value_col.Count > 1;
                        if (value_col[0] != null)
                        {
                            curr = value_col[0].GetType();
                            if (curr != typeof(System.Object) && Type.GetTypeCode(curr) == TypeCode.Object)
                                curr = typeof(System.Object);
                            if (dataType[p] == null || curr == typeof(System.Object))
                                dataType[p] = curr;
                            else
                                if (dataType[p] != curr)
                                    dataType[p] = typeof(System.String);
                        }
                        if (dataType[p] == typeof(System.Object))
                            break;
                    }
                }
                if (dataType[p] == null)
                    dataType[p] = typeof(System.String);
            }

            DataTable dt = RowType.CreateSchemaTable();
            DataProviderHelper helper = new DataProviderHelper();
            for (int k = 0; k < dataType.Length; k++)
            {
                DataRow dr = dt.NewRow();
                dr["ColumnName"] = helper.NativeFormatIdentifier(properties_array[k]);
                dr["IsCaseSensitive"] = true;
                dr["ColumnOrdinal"] = k;
                if (is_array[k])
                    dr["DataType"] = typeof(System.Object);
                else
                    dr["DataType"] = dataType[k];
                dt.Rows.Add(dr);
            }
            Resultset rs = new Resultset(new RowType(dt), null);
            foreach (SearchResult sr in result)
            {
                Row row = rs.NewRow();
                for (int p = 0; p < properties_array.Length; p++)
                {
                    ResultPropertyValueCollection value_col = sr.Properties[properties_array[p]];
                    if (value_col.Count > 0)
                    {
                        if (value_col.Count == 1)
                        {
                            if (dataType[p] == typeof(System.String))
                                row.SetString(p, value_col[0].ToString());
                            else
                                row.SetValue(p, value_col[0]);
                        }
                        else
                        {
                            object[] values = (object[])Array.CreateInstance(dataType[p], value_col.Count);
                            for (int k = 0; k < values.Length; k++)
                                values[k] = value_col[k];
                            row.SetValue(p, values);
                        }
                    }
                }
                rs.Enqueue(row);
            }
            searcher.Dispose();
            entry.Dispose();
            return rs;
        }

        public static XmlNode At(Object arg, int index)
        {
            if (arg == null)
                return null;
            else if (arg is XmlNodeList)
            {
                XmlNodeList nodes = (XmlNodeList)arg;
                if (index > 0 && index <= nodes.Count)
                    return nodes.Item(index - 1);
                else
                    return null;
            }
            else
                throw new ESQLException(Properties.Resources.ArgumentIsNotArray);
        }

        private static void ProcessNode(XmlDataAccessor.NodeList nodeList, XmlNode node, string name)
        {
            if (name.StartsWith("@"))
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    XmlElement elem = (XmlElement)node;
                    if (elem.HasAttributes)
                    {
                        name = name.Substring(1); // Omit '@' prefix
                        foreach (XmlAttribute attr in elem.Attributes)
                            if (attr.Name.Equals(name))
                            {
                                nodeList.Add(attr);
                                break;
                            }
                    }
                }
            }
            else
                foreach (XmlNode child in node.ChildNodes)
                    if (child.Name.Equals(name))
                        nodeList.Add(child);
        }

        public static XmlNodeList Dref(object arg, string name)
        {
            XmlDataAccessor.NodeList nodeList = new XmlDataAccessor.NodeList();
            if (arg != null)
            {
                name = Util.UnquoteName(name);
                if (arg is XmlNode)
                    ProcessNode(nodeList, (XmlNode)arg, name);
                else if (arg is XmlNodeList)
                {
                    XmlNodeList nodes = (XmlNodeList)arg;
                    foreach (XmlNode node in nodes)
                        ProcessNode(nodeList, node, name);
                }
            }
            if (nodeList.Count == 0)
                return null;
            else
                return nodeList;
        }

        public static XmlNodeList Wref(object arg, string name)
        {
            throw new NotImplementedException();
        }

        public static Resultset Dyncast([Implict] Executive engine, object arg)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            XmlDataAccessor accessor = new XmlDataAccessor();
            if (arg == null)
                return null;
            else if (arg is Resultset)
                return (Resultset)arg;
            else if (arg is XmlNode)
            {
                XmlNode node = ((XmlNode)arg);
                if (node is XmlDocument)
                    node = ((XmlDocument)node).DocumentElement;
                XmlDataAccessor.NodeList nodeList = new XmlDataAccessor.NodeList();
                foreach (XmlAttribute attr in node.Attributes)
                    if (!XmlDataAccessor.IsSpecialAttribute(attr))
                        nodeList.Add(attr);
                foreach (XmlNode child in node.ChildNodes)
                    nodeList.Add(child);
                return accessor.ParseNodes(nodeList, null, owner.Node);
            }
            else if (arg is XmlNodeList)
                return accessor.ParseNodes(((XmlNodeList)arg), null, owner.Node);
            else if (arg is Array)
            {
                Array array = (Array)arg;
                DataTable dt = RowType.CreateSchemaTable();
                if (array.Rank > 1)
                    throw new ESQLException("Can't cast multi-diminsion array to resultset");
                else
                {
                    Type elemType = array.GetType().GetElementType();
                    DataRow r = dt.NewRow();
                    r["ColumnName"] = "node";
                    r["ColumnOrdinal"] = 0;
                    if (Type.GetTypeCode(elemType) == TypeCode.Object)
                        r["DataType"] = typeof(System.Object);
                    else
                        r["DataType"] = elemType;
                    dt.Rows.Add(r);
                }
                Resultset rs = new Resultset(new RowType(dt), null);
                foreach (object item in array)
                {
                    Row row = rs.NewRow();
                    row.SetValue(0, item);
                    rs.Enqueue(row);
                }
                return rs;
            }
            else if (arg == DBNull.Value)
                return null;
            else
            {
                DataTable dt = RowType.CreateSchemaTable();
                DataRow r = dt.NewRow();
                r["ColumnName"] = "node";
                r["ColumnOrdinal"] = 0;
                r["DataType"] =
                    TypeConverter.GetTypeByTypeCode(Type.GetTypeCode(arg.GetType()));
                dt.Rows.Add(r);
                Resultset rs = new Resultset(new RowType(dt), null);
                Row row = rs.NewRow();
                row.SetValue(0, arg);
                rs.Enqueue(row);
                return rs;
            }
        }

        public static object Extract([Implict] Executive engine, object arg, string xpath)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            XmlDataAccessor xmlacc = new XmlDataAccessor();
            if (arg == null)
                return null;
            else if (arg is Resultset)
            {
                Resultset rs = (Resultset)arg;
                Resultset dest = new Resultset(RowType.CreateContainerType(typeof(System.Object)), null);
                if (!dest.RowType.RowTypeEquals(rs.RowType))
                    throw new InvalidOperationException();
                while (rs.Begin != null)
                {
                    Row row = rs.Dequeue();
                    XmlNode node = (XmlNode)row.GetObject(0);
                    object name = row.GetValue(1);
                    row = dest.NewRow();
                    if (node != null)
                    {
                        XmlNodeList nodes = node.SelectNodes(xpath,
                            owner.QueryContext.GetNsManager(node.OwnerDocument));
                        if (nodes.Count > 0)
                            row.SetObject(0, nodes);
                    }
                    row.SetValue(1, name);
                    dest.Enqueue(row);
                }
                return dest;
            }
            else if (arg is XmlNode)
            {
                XmlNode node = (XmlNode)arg;
                XmlNodeList nodes = node.SelectNodes(xpath,
                    owner.QueryContext.GetNsManager(node.OwnerDocument));
                if (nodes.Count == 0)
                    return null;
                else
                    return nodes;
            }
            else if (arg is XmlNodeList)
            {
                XmlDataAccessor.NodeList nodeList = new XmlDataAccessor.NodeList();
                foreach (XmlNode node in (XmlNodeList)arg)
                {
                    XmlNodeList nodes = node.SelectNodes(xpath,
                        owner.QueryContext.GetNsManager(node.OwnerDocument));
                    nodeList.AddRange(nodes);
                }
                if (nodeList.Count > 0)
                    return nodeList;
                else
                    return null;
            }
            else
                throw new InvalidOperationException();
        }

        public static String NodeText(object arg)
        {
            if (arg is XmlNode)
            {
                XmlNode node = (XmlNode)arg;
                return node.InnerXml;
            }
            else
                return null;
        }

        public static XmlNode Parse([Implict] Executive engine, object arg)
        {
            if (arg == null)
                return null;
            else
            {
                QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
                XmlDocument xmldoc = new XmlDocument(owner.QueryContext.NameTable);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.None;
                settings.IgnoreWhitespace = true;
                settings.DtdProcessing = DtdProcessing.Parse;
                settings.XmlResolver = null;
                settings.NameTable = owner.QueryContext.NameTable;
                XmlReader render;
                if (arg is Stream)
                    render = XmlReader.Create((Stream)arg, settings);
                else if (arg is TextReader)
                    render = XmlReader.Create((TextReader)arg, settings);
                else
                    render = XmlReader.Create(new StringReader(arg.ToString()), settings);
                XmlNamespaceManager nsManager = new XmlNamespaceManager(render.NameTable);
                XmlDataAccessor.InitNsManager(nsManager, owner.Node);
                owner.QueryContext.SetNsManager(xmldoc, nsManager);
                try
                {
                    xmldoc.Load(render);
                    return xmldoc.DocumentElement;
                }
                finally
                {
                    render.Close();
                    if (arg is Stream)
                        ((Stream)arg).Close();
                    else if (arg is TextReader)
                        ((TextReader)arg).Close();
                }
            }
        }

        private static object ProcessNodeList(QueryNode.LispProcessingContext owner, XmlNodeList nodeList)
        {
            if (nodeList.Count == 0)
                return null;
            else if (nodeList.Count == 1)
            {
                XmlNode node = nodeList.Item(0);
                if (node is XmlElement)
                    if (node.ChildNodes.Count == 1 && node.FirstChild is XmlText)
                        return XmlDataAccessor.Convert(XmlDataAccessor.GetNodeType(node,
                            XmlDataAccessor.GetTypeManager(owner.Node)), node);
                    else
                        return node;
                else
                    return node.Value;
            }
            return nodeList;
        }

        public static object Rval([Implict] Executive engine, object arg)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            if (arg is XmlNode)
                return ProcessNodeList(owner, ((XmlNode)arg).ChildNodes);
            else if (arg is XmlNodeList)
                return ProcessNodeList(owner, (XmlNodeList)arg);
            else
                return arg;
        }

    }
}
