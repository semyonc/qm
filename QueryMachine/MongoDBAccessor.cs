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
using System.Globalization;
using System.Text.RegularExpressions;

using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

using DataEngine.CoreServices.Data;
using DataEngine.CoreServices;

namespace DataEngine
{
    class MongoDBAccessor : QueryNode, SmartTableAccessor
    {
        public TableType TableType { get; private set; }

        public int TopRows { get; set; }

        public SortColumn[] SortColumns { get; set; }

        public String[] AccessPredicate { get; set; }

        public Object[][] AccessPredicateValues { get; set; }

        private object _filterPredicate;
        
        private int[] _parameterBindings;

        public Object FilterPredicate
        {
            get
            {
                return _filterPredicate;
            }
            set
            {
                _filterPredicate = value;
                List<int> bindings = new List<int>();
                Binder.GetParameterBindings(value, bindings);
                if (bindings.Count == 0)
                    _parameterBindings = null;
                else
                    _parameterBindings = bindings.ToArray();
            }
        }

        protected class ProcessingContext : DemandProcessingContext
        {
            private MongoDBAccessor _owner;
            private object[] _parameters;
            private MongoServer _server;
            private IEnumerator<BsonDocument> _enum;
            private BsonLoader _loader;

            public ProcessingContext(MongoDBAccessor owner, Object[] parameters)
                : base(null)
            {
                _owner = owner;
                _parameters = parameters;
                _loader = new BsonLoader();
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                if (_server == null)
                    _enum = _owner.CreateCursor(_owner.Connect(out _server), 
                        rs.RowType, _parameters).GetEnumerator();
                if (_enum != null)
                {
                    if (!_enum.MoveNext())
                    {
                        _enum = null;
                        _server.Disconnect();
                    }
                    else
                    {
                        _loader.LoadRecord(rs, _enum.Current);
                        return true;
                    }
                }
                return false;
            }
        }

        public MongoDBAccessor(TableType tableType)
            : base()
        {
            TableType = tableType;
            TopRows = -1;
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            DataTable dt = TableType.TableRowType.GetSchemaTable();
            ProcessingContext context = new ProcessingContext(this, parameters) { RecordLimit = queryContext.LimitInputQuery };
            return new Resultset(new RowType(dt), context);       
        }

        private MongoDatabase Connect(out MongoServer server)
        {
            MongoConnectionStringBuilder csb = new MongoConnectionStringBuilder();
            csb.ConnectionString = TableType.DataSource.ConnectionString;
            server = MongoServer.Create(csb);
            server.Connect();
            MongoDatabaseSettings settings = server.CreateDatabaseSettings(csb.DatabaseName);
            if (csb.Username != null)
                settings.Credentials = new MongoCredentials(csb.Username, csb.Password);
            return server.GetDatabase(settings);
        }

        public int GetTableEstimate(int threshold)
        {
            MongoServer server;
            MongoDatabase database = Connect(out server);
            try
            {
                MongoCollection<BsonDocument> collection =
                    database.GetCollection<BsonDocument>(TableType.TableName);
                return (int)collection.Count();
            }
            finally
            {
                server.Disconnect();
            }
        }

        private MongoCursor<BsonDocument> CreateCursor(MongoDatabase database, RowType rt, object[] parameters)
        {
            MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(TableType.TableName);
            MongoCursor<BsonDocument> cursor;
            if (FilterPredicate != null || AccessPredicate != null)
            {
                IMongoQuery query;
                Binder binder = new Binder(rt.Fields);
                if (FilterPredicate != null && AccessPredicate != null)
                    query = Query.And(BuildQuery(binder, FilterPredicate, parameters), BuildAccessPredicate());
                else
                    if (FilterPredicate != null)
                        query = BuildQuery(binder, FilterPredicate, parameters);
                    else
                        query = BuildAccessPredicate();
                cursor = collection.Find(query);                    
            }
            else
                cursor = collection.FindAll();
            if (TopRows != -1)
                cursor.SetLimit(TopRows);
            if (SortColumns != null && SortColumns.Length > 0)
            {
                SortByDocument sortBy = new SortByDocument();
                foreach (SortColumn sortCol in SortColumns)
                    sortBy.Add(sortCol.ColumnName,
                        sortCol.Direction == SortDirection.Ascending ? 1 : -1);
                cursor.SetSortOrder(sortBy);
            }
            return cursor;
        }

        private IMongoQuery BuildAccessPredicate()
        {
            IMongoQuery res = null;
            for (int s = 0; s < AccessPredicateValues.Length; s++)
            {
                IMongoQuery q = null;
                for (int k = 0; k < AccessPredicate.Length; k++)
                {                    
                    Object predicateValue = AccessPredicateValues[s][k];
                    if (q == null)
                        q = Query.EQ(AccessPredicate[k], 
                            BsonValue.Create(predicateValue == DBNull.Value 
                                ? null : predicateValue));
                    else
                        q = Query.And(q, Query.EQ(AccessPredicate[k], 
                            BsonValue.Create(predicateValue == DBNull.Value 
                                ? null : predicateValue)));
                }
                if (res != null)
                    res = Query.Or(res, q);
                else
                    res = q;
            }
            return res;
        }

        private bool IsBsonValue(object value)
        {
            return Lisp.IsFunctor(value, ID.ParamRef) ||
                (Lisp.IsNode(value) && !Lisp.IsAtom(value));
        }

        private BsonValue GetBsonValue(object value, Object[] parameters)
        {
            if (Lisp.IsFunctor(value, ID.ParamRef))
                return BsonValue.Create(parameters[(int)Lisp.Arg1(value) -1]);
            return BsonValue.Create(value);
        }

        private string GetMongoFieldName(Binder binder, object atom)
        {
            ATOM a = (ATOM)atom;
            string name;
            if (a.parts.Length == 1)
                name = a.parts[0];
            else
                name = a.parts[1];
            ColumnBinding b = binder.Get(name);
            if (b == null)
                throw new ESQLException(Properties.Resources.InvalidIdentifier, name);
            RowType.TypeInfo ti = TableType.TableRowType.Fields[b.rnum];
            if (ti.ProviderColumnName != null)
                return ti.ProviderColumnName;
            else
                return ti.Name;
        }

        // http://bytes.com/topic/c-sharp/answers/253519-using-regex-create-sqls-like-like-function
        private String CreateRegExpr(object tail)
        {
            /* Turn "off" all regular expression related syntax in
             * the pattern string. */
            string pattern = Regex.Escape(Lisp.Nth(tail, 0).ToString());
            
            /* Replace the SQL LIKE wildcard metacharacters with the
             * equivalent regular expression metacharacters. */

            pattern = pattern.Replace("%", ".*?").Replace("_", ".");

            /* The previous call to Regex.Escape actually turned off
             * too many metacharacters, i.e. those which are recognized by
             * both the regular expression engine and the SQL LIKE
             * statement ([...] and [^...]). Those metacharacters have
             * to be manually unescaped here. */
            pattern = pattern.Replace(@"\[", "[").Replace(@"\]", "]").Replace(@"\^", "^");

            return pattern;
        }

        private IMongoQuery BuildQuery(Binder binder, object lval, Object[] parameters)
        {
            if (IsValidFilterExpr(lval))
                return BuildFilterExpr(binder, lval, parameters);
            string jsFilter = BuildJavaScript(binder, lval, parameters);
            return Query.Where(new BsonJavaScript(jsFilter));
        }

        private bool IsValidFilterExpr(object lval)
        {
            if (Lisp.IsFunctor(lval, Funcs.And) || Lisp.IsFunctor(lval, Funcs.Or))
                return IsValidFilterExpr(Lisp.Arg1(lval)) && IsValidFilterExpr(Lisp.Arg2(lval));
            else
                if (Lisp.IsFunctor(lval, ID.EQ) ||
                    Lisp.IsFunctor(lval, ID.NE) ||
                    Lisp.IsFunctor(lval, ID.GT) ||
                    Lisp.IsFunctor(lval, ID.GE) ||
                    Lisp.IsFunctor(lval, ID.LT) ||
                    Lisp.IsFunctor(lval, ID.LE))
                    return (Lisp.IsAtom(Lisp.Arg1(lval)) && IsBsonValue(Lisp.Arg2(lval))) ||
                        (IsBsonValue(Lisp.Arg1(lval)) && Lisp.IsAtom(Lisp.Arg2(lval)));
            else 
                if (Lisp.IsFunctor(lval, ID.IsNull))
                    return Lisp.IsAtom(Lisp.Arg1(lval));
            else
                if (Lisp.IsFunctor(lval, Funcs.Not))
                    return IsValidFilterExpr(Lisp.Arg1(lval));
            else
                if (Lisp.IsFunctor(lval, ID.Like1))
                    return Lisp.IsAtom(Lisp.Arg1(lval)) && IsBsonValue(Lisp.Arg2(lval));
            else
                if (Lisp.IsFunctor(lval, ID.Between))
                    return Lisp.IsAtom(Lisp.Arg1(lval)) && 
                        IsBsonValue(Lisp.Arg2(lval)) && IsBsonValue(Lisp.Arg3(lval));
            else
                if (Lisp.IsFunctor(lval, ID.Member))
                {
                    object[] list = Lisp.ToArray(Lisp.Cdr(Lisp.Arg2(lval)));
                    foreach (object item in list)
                        if (!IsBsonValue(item))
                            return false;
                    return Lisp.IsAtom(Lisp.Arg1(lval));
                }
            return false;
        }

        private IMongoQuery BuildFilterExpr(Binder binder, object lval, Object[] parameters)
        {
            if (Lisp.IsFunctor(lval, Funcs.And))
                return Query.And(
                    BuildFilterExpr(binder, Lisp.Arg1(lval), parameters),
                    BuildFilterExpr(binder, Lisp.Arg2(lval), parameters));
            else 
                if (Lisp.IsFunctor(lval, Funcs.Or))
                    return Query.Or(
                        BuildFilterExpr(binder, Lisp.Arg1(lval), parameters),
                        BuildFilterExpr(binder, Lisp.Arg2(lval), parameters));
            else 
                if (Lisp.IsFunctor(lval, ID.EQ))
                    if (Lisp.IsAtom(Lisp.Arg1(lval)))
                        return Query.EQ(GetMongoFieldName(binder, Lisp.Arg1(lval)),
                            GetBsonValue(Lisp.Arg2(lval), parameters));
                    else
                        return Query.EQ(GetMongoFieldName(binder, Lisp.Arg2(lval)),
                            GetBsonValue(Lisp.Arg1(lval), parameters));
            else
                if (Lisp.IsFunctor(lval, ID.NE))
                    if (Lisp.IsAtom(Lisp.Arg1(lval)))
                        return Query.NE(GetMongoFieldName(binder, Lisp.Arg1(lval)),
                            GetBsonValue(Lisp.Arg2(lval), parameters));
                    else
                        return Query.NE(GetMongoFieldName(binder, Lisp.Arg2(lval)),
                            GetBsonValue(Lisp.Arg1(lval), parameters));
            else 
                if (Lisp.IsFunctor(lval, ID.GT))
                    if (Lisp.IsAtom(Lisp.Arg1(lval)))
                        return Query.GT(GetMongoFieldName(binder, Lisp.Arg1(lval)),
                            GetBsonValue(Lisp.Arg2(lval), parameters));
                    else
                        return Query.LT(GetMongoFieldName(binder, Lisp.Arg2(lval)),
                            GetBsonValue(Lisp.Arg1(lval), parameters));
            else 
                if (Lisp.IsFunctor(lval, ID.GE))
                    if (Lisp.IsAtom(Lisp.Arg1(lval)))
                        return Query.GTE(GetMongoFieldName(binder, Lisp.Arg1(lval)),
                            GetBsonValue(Lisp.Arg2(lval), parameters));
                    else
                        return Query.LTE(GetMongoFieldName(binder, Lisp.Arg2(lval)),
                            GetBsonValue(Lisp.Arg1(lval), parameters));
            else 
                if (Lisp.IsFunctor(lval, ID.LT))
                    if (Lisp.IsAtom(Lisp.Arg1(lval)))
                        return Query.LT(GetMongoFieldName(binder, Lisp.Arg1(lval)),
                            GetBsonValue(Lisp.Arg2(lval), parameters));
                    else
                        return Query.GT(GetMongoFieldName(binder, Lisp.Arg2(lval)),
                            GetBsonValue(Lisp.Arg1(lval), parameters));
            else 
                if (Lisp.IsFunctor(lval, ID.LE))
                    if (Lisp.IsAtom(Lisp.Arg1(lval)))
                        return Query.LTE(GetMongoFieldName(binder, Lisp.Arg1(lval)),
                            GetBsonValue(Lisp.Arg2(lval), parameters));
                    else
                        return Query.GTE(GetMongoFieldName(binder, Lisp.Arg2(lval)),
                            GetBsonValue(Lisp.Arg1(lval), parameters));
            else
                if (Lisp.IsFunctor(lval, ID.IsNull))
                    return  Query.EQ(GetMongoFieldName(binder, Lisp.Arg1(lval)), BsonNull.Value);
            else
                if (Lisp.IsFunctor(lval, ID.Member))
                {
                    object[] list = Lisp.ToArray(Lisp.Cdr(Lisp.Arg2(lval)));
                    BsonValue[] values = new BsonValue[list.Length];
                    for (int k = 0; k < list.Length; k++)
                        values[k] = GetBsonValue(list[k], parameters);
                    return Query.In(GetMongoFieldName(binder, Lisp.Arg1(lval)), values);
                }
            else
                if (Lisp.IsFunctor(lval, ID.Like1))
                    return Query.Matches(GetMongoFieldName(binder, Lisp.Arg1(lval)),
                        new BsonRegularExpression(CreateRegExpr(Lisp.Cddr(lval))));
            else
                if (Lisp.IsFunctor(lval, Funcs.Not))
                    return Query.Nor(BuildFilterExpr(binder, Lisp.Arg1(lval), parameters));
            else
                if (Lisp.IsFunctor(lval, ID.Between))
                {
                    string fieldName = GetMongoFieldName(binder, Lisp.Arg1(lval));
                    return Query.And(Query.GTE(fieldName, GetBsonValue(Lisp.Arg2(lval), parameters)),
                        Query.LTE(fieldName, GetBsonValue(Lisp.Arg3(lval), parameters)));
                }
            else
                throw new UnproperlyFormatedExpr(Lisp.Format(lval));
        }

        private string BuildJavaScript(Binder binder, object lval, object[] parameters)
        {
            if (Lisp.IsNode(lval))
            {
                if (Lisp.IsAtom(lval))
                {
                    ATOM a = (ATOM)lval;
                    string name;
                    if (a.parts.Length == 1)
                        name = a.parts[0];
                    else
                        name = a.parts[1];
                    ColumnBinding b = binder.Get(name);
                    if (b == null)
                        throw new ESQLException(Properties.Resources.InvalidIdentifier, name);
                    RowType.TypeInfo ti = TableType.TableRowType.Fields[b.rnum];
                    string fieldName;
                    if (ti.ProviderColumnName != null)
                        fieldName = ti.ProviderColumnName;
                    else
                        fieldName = ti.Name;
                    if (DataProviderHelper.RequiresQuotingDefault(fieldName, true))
                        return String.Format("this['{0}']", fieldName);
                    else
                        return String.Format("this.{0}", fieldName);
                }
                else
                {
                    object value;
                    if (Lisp.IsFunctor(lval, ID.ParamRef))
                        value = parameters[(int)Lisp.Arg1(lval) - 1];
                    else
                        value = lval;
                    switch (Type.GetTypeCode(value.GetType()))
                    {
                        case TypeCode.DateTime:
                            {
                                DateTime dateTime = (DateTime)value;
                                return String.Format("new Date({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                                    dateTime.Year, dateTime.Month, dateTime.Date,
                                    dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
                            }

                        case TypeCode.Int32:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            {
                                double n = Convert.ToDouble(value);
                                return n.ToString(CultureInfo.InvariantCulture);
                            }

                        case TypeCode.Int64:
                            return String.Format("new NumberLong({0})", value);

                        default:
                            return String.Format("'{0}'", value);
                    }
                }
            }
            else
            {
                object head = Lisp.Car(lval);
                if (Lisp.IsCons(head))
                {
                    object[] list = Lisp.ToArray(lval);
                    StringBuilder sb = new StringBuilder();
                    for (int k = 0; k < list.Length; k++)
                    {
                        if (k > 0)
                            sb.Append(" && ");
                        sb.Append("(");
                        sb.Append(BuildJavaScript(binder, list[k], parameters));
                        sb.Append(")");
                    }
                    return sb.ToString();
                }
                else
                {
                    object[] args = Lisp.ToArray(Lisp.Cdr(lval));
                    if (head.Equals(ID.Like1))
                        return String.Format("new RegExp('{1}').test({0})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                            CreateRegExpr(Lisp.Cddr(lval)));
                    else if (head.Equals(ID.Between))
                        return String.Format("({1}) <= ({0}) && ({0}) <= ({2})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                            BuildJavaScript(binder, Lisp.Arg2(lval), parameters),
                            BuildJavaScript(binder, Lisp.Arg3(lval), parameters));
                    else if (head.Equals(Funcs.Not))
                        return String.Format("!({0})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters));
                    else if (head.Equals(ID.IsNull))
                        return String.Format("(({0}) == null || ({0}) == undefined)",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters));
                    else if (head.Equals(Funcs.And))
                        return String.Format(" ({0}) && ({1})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                            BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(Funcs.Or))
                        return String.Format(" ({0}) || ({1})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                            BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(ID.LT))
                        return String.Format(" ({0}) < ({1})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                            BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(ID.GT))
                        return String.Format(" ({0}) > ({1})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                            BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(ID.EQ))
                        return String.Format(" ({0}) == ({1})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                            BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(ID.NE))
                        return String.Format(" ({0}) != ({1})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                            BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(ID.LE))
                        return String.Format(" ({0}) <= ({1})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                            BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(ID.GE))
                        return String.Format(" ({0}) >= ({1})",
                            BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                            BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(ID.Concat))
                        return String.Format(" ({0}).toString() + ({1}).toString()",
                             BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                             BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(Funcs.Add))
                        return String.Format(" ({0}) + ({1})",
                             BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                             BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(Funcs.Sub))
                        return String.Format(" ({0}) - ({1})",
                             BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                             BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(Funcs.Mul))
                        return String.Format(" ({0}) * ({1})",
                             BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                             BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else if (head.Equals(Funcs.Div))
                        return String.Format(" ({0}) / ({1})",
                             BuildJavaScript(binder, Lisp.Arg1(lval), parameters),
                             BuildJavaScript(binder, Lisp.Arg2(lval), parameters));
                    else
                        throw new UnproperlyFormatedExpr(Lisp.Format(lval));
                }
            }
        }
    }
}
