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
using System.Xml;

using DataEngine.CoreServices;
using DataEngine.Parser;
using DataEngine.CoreServices.Data;
using System.IO;

namespace DataEngine
{
    public class Optimizer
    {
        public Optimizer(QueryContext context)
        {
            Context = context;
            EnableServerQuery = true;
            EnableDSFilter = true;
        }

        #region Analyzers
        private class SubqueryAnalyzer : QueryWalker
        {
            private List<Symbol> queries = new List<Symbol>();

            public SubqueryAnalyzer(Notation notation)
                : base(notation)
            {
            }

            public override void WalkQuerySpec(Symbol squery)
            {
                queries.Add(squery);
                base.WalkQuerySpec(squery);
            }

            public Symbol[] Queries
            {
                get
                {
                    return queries.ToArray();
                }
            }
        }

        private class RankAnalyzer : QueryWalker
        {
            public int Rank { get; private set; }

            public int _cur_rank = 0;

            public RankAnalyzer(Notation notation)
                : base(notation)
            {
            }

            public override void WalkQuerySpec(Symbol squery)
            {
                _cur_rank++;
                if (_cur_rank > Rank)
                    Rank = _cur_rank;
                base.WalkQuerySpec(squery);
                _cur_rank--;
            }
        }

        private class SubQueryComparer : IComparer<Symbol>
        {
            private Notation notation;

            public SubQueryComparer(Notation notation)
            {
                this.notation = notation;
            }

            public int Compare(Symbol x, Symbol y)
            {
                return GetRank(x) - GetRank(y);
            }

            private int GetRank(Symbol x)
            {
                RankAnalyzer analyzer = new RankAnalyzer(notation);
                analyzer.WalkSubQuery(x);
                return analyzer.Rank;
            }
        }

        private class AggregationAnalyzer : QueryWalker
        {
            private List<Symbol> groupFuncList = new List<Symbol>();
            private List<Qname> groupByList = new List<Qname>();
            private List<Qname> columnList = new List<Qname>();

            private bool ignoreFlag = false;
            private bool whereClauseFlag = false;

            public AggregationAnalyzer(Notation notation)
                : base(notation)
            {
            }

            public override void WalkSubQuery(Symbol squery)
            {
                return; 
            }

            public override void WalkQualifiedName(Qname qname)
            {
                base.WalkQualifiedName(qname);
                if (!ignoreFlag)
                    columnList.Add(qname);
            }

            public override void WalkAlias(Qname qname)
            {
                return;
            }

            public override void WalkSetFuncSpec(Symbol sym)
            {
                if (whereClauseFlag)
                    throw new ESQLException(Properties.Resources.GroupFunctionNotAllowed);
                ignoreFlag = true;
                base.WalkSetFuncSpec(sym);
                ignoreFlag = false;
                groupFuncList.Add(sym);
            }

            public override void WalkWhereClause(Symbol sym)
            {
                whereClauseFlag = ignoreFlag = true;
                base.WalkWhereClause(sym);
                whereClauseFlag = ignoreFlag = false;
            }

            public override void WalkFromClause(Symbol sym)
            {
                ignoreFlag = true;
                base.WalkFromClause(sym);
                ignoreFlag = false;
            }

            public override void WalkGroupByClause(Symbol sym)
            {
                Notation.Record[] recs = notation.Select(sym, Descriptor.GroupBy, 1);
                if (recs.Length > 0)
                {
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                    for (int k = 0; k < arr.Length; k++)
                        groupByList.Add((Qname)arr[k]);
                }                
            }

            public List<Symbol> GroupFuncs { get { return groupFuncList; } }

            public List<Qname> GroupByColumns { get { return groupByList; } }

            public List<Qname> Columns { get { return columnList; } }

            public bool hasAggregation
            {
                get
                {
                    return groupFuncList.Count > 0;
                }
            }

            public bool hasComplexExpr
            {
                get
                {
                    foreach (Symbol sym in groupFuncList)
                    {
                        Notation.Record[] recs = notation.Select(sym, Descriptor.Aggregate, 2);
                        if (recs.Length > 0)
                        {
                            if (!(recs[0].Arg1 is Qname) || 
                                  ((TokenWrapper)recs[0].Arg0).Data == Token.XMLAGG)
                                return true;
                        }
                    }
                    return false;
                }
            }
        }

        private class JoinAnalyzer : QueryWalker
        {
            private List<Symbol> exprList = new List<Symbol>();
            private Stack<Symbol> joinStack = new Stack<Symbol>();

            public JoinAnalyzer(Notation notation)
                : base(notation)
            {
            }

            public override void WalkJoinSpec(Symbol sym)
            {
                joinStack.Push(sym);
                base.WalkJoinSpec(sym);
                joinStack.Pop();
            }

            public override void WalkJoinSearchCondition(Symbol sym)
            {                
                base.WalkJoinSearchCondition(sym);
                exprList.Add(joinStack.Peek());
            }

            public List<Symbol> QualifiedJoins { get { return exprList; } }
        }

        private class QnameAnalyzer : QueryWalker
        {

            private class Context
            {
                public Context owner;
                public List<String> tables;                

                public Context(Context owner)
                {
                    this.owner = owner;
                    this.tables = new List<string>();
                }
            }
            
            public class QnameRecord
            {
                public Notation.Record rec;
                public bool qualified = false;                
            }

            private Stack<Context> st;
            private List<QnameRecord> qrecs;            


            public QnameAnalyzer(Notation notation)
                : base(notation)
            {
                st = new Stack<Context>();
                qrecs = new List<QnameRecord>();
            }

            public override void WalkStmt()
            {
                st.Push(new Context(null));
                base.WalkStmt();
            }

            public override void WalkQuerySpec(Symbol sym)
            {
                st.Push(new Context(st.Peek()));
                base.WalkQuerySpec(sym);
                st.Pop();
            }

            public override void WalkTableRefSimple(Symbol sym)
            {
                string alias = null;
                Context context = st.Peek();
                Notation.Record[] recs = notation.Select(sym, Descriptor.Alias, 1);
                if (recs.Length > 0)
                {
                    Qname qname = (Qname)recs[0].Arg0;
                    alias = qname.Name;
                }
                if (sym.Tag == Tag.Qname)
                {
                    if (alias == null)
                        alias = ((Qname)sym).Name;
                }
                if (alias != null)
                    context.tables.Add(alias);
                base.WalkTableRefSimple(sym);
            }

            public override void WalkTableQualifiedName(Qname qname)
            {                
            }

            public override void WalkDref(Notation.Record[] recs)
            {
                if (recs[0].Arg0.Tag == Tag.Qname)
                {
                    Qname qname = (Qname)recs[0].Arg0;
                    if (qname.IsNonQualifiedName)
                    {
                        QnameRecord qr = new QnameRecord();
                        qr.rec = recs[0];
                        for (Context context = st.Peek(); context != null; context = context.owner)
                            if (context.tables.Contains(qname.Name))
                            {
                                qr.qualified = true;
                                break;
                            }
                        if (qr.qualified)
                        {
                            qrecs.Add(qr);
                            return;
                        }
                    }
                }
                base.WalkDref(recs);
            }

            public List<QnameRecord> Records { get { return qrecs; } }
        }

        public class TableAnalyzer : QueryWalker
        {
            public TableAnalyzer(Notation notation)
                : base(notation)
            {
                Tables = new List<Symbol>();
            }

            public override void WalkTableRefSimple(Symbol sym)
            {
                base.WalkTableRefSimple(sym);
                Tables.Add(sym);
            }

            public override void WalkSubQuery(Symbol squery)
            {
                return;
            }

            public List<Symbol> Tables { get; private set; }
        }

        public class PredicateAnalyzer : QueryWalker
        {
            public PredicateAnalyzer(Notation notation)
                : base(notation)
            {
            }

            public override void WalkSubQuery(Symbol squery)
            {
                IsTablePredicate = false;
            }

            public override void WalkPredicate(Symbol sym)
            {
                TableName = null;
                _tableName_set = false;
                IsTablePredicate = true;
                
                base.WalkPredicate(sym);
            }

            public override void WalkCaseExp(Symbol sym)
            {
                IsTablePredicate = false;
                base.WalkCaseExp(sym);
            }

            public override void WalkFuncall(Notation.Record rec)
            {
                IsTablePredicate = false;
                base.WalkFuncall(rec);
            }

            public override void WalkXmlValueFunc(Symbol sym)
            {
                IsTablePredicate = false;
                base.WalkXmlValueFunc(sym);
            }

            public override void WalkQualifiedName(Qname qname)
            {
                if (_tableName_set)
                {
                    if (qname.Qualifier != TableName)
                    {
                        TableName = null;
                        IsTablePredicate = false;
                    }
                }
                else
                {
                    TableName = qname.Qualifier;
                    _tableName_set = true;
                }
            }

            public override void WalkDescriptor(Descriptor descriptor)
            {
                switch (descriptor)
                {
                    case Descriptor.RowValue:
                    case Descriptor.Overlaps:
                    case Descriptor.Match:
                    case Descriptor.PosString:
                    case Descriptor.NullIf:
                    case Descriptor.StringConvert:
                    case Descriptor.StringLower:
                    case Descriptor.StringUpper:
                    case Descriptor.StringTrim:
                    case Descriptor.Substring:
                    case Descriptor.XMLAgg:
                    case Descriptor.Dref:
                    case Descriptor.Wref:
                    case Descriptor.At:
                    case Descriptor.Cast:
                        IsTablePredicate = false;
                        break;
                }
            }

            public bool _tableName_set;

            public String TableName { get; private set; }

            public bool IsTablePredicate { get; private set; }

        }

        public class ServerQueryAnalyzer : QueryWalker
        {

            public ServerQueryAnalyzer(Notation notation)
                : base(notation)
            {
            }

            protected bool _provider_key_set;

            public override void WalkStmt()
            {
                IsServerSubquery = true;
                base.WalkStmt();
            }

            public override void WalkSubQuery(Symbol squery)
            {
                IsServerSubquery = false;
            }

            public override void WalkDescriptor(Descriptor descriptor)
            {
                switch (descriptor)
                {
                    case Descriptor.NaturalJoin:
                    case Descriptor.CrossJoin:
                    case Descriptor.UnionJoin:
                    case Descriptor.QualifiedJoin:
                    case Descriptor.Except:
                    case Descriptor.Intersect:            
                    case Descriptor.Explicit:
                    case Descriptor.TableValue:
                    case Descriptor.RowValue:
                    case Descriptor.Overlaps:
                    case Descriptor.Match:
                    case Descriptor.PosString:
                    case Descriptor.NullIf:
                    case Descriptor.Concat:
                    case Descriptor.StringConvert:
                    case Descriptor.StringLower:
                    case Descriptor.StringUpper:
                    case Descriptor.StringTrim:
                    case Descriptor.Substring:
                    case Descriptor.XMLAgg:
                    case Descriptor.Dynatable:
                    case Descriptor.Tuple:
                    case Descriptor.Dref:
                    case Descriptor.Wref:
                    case Descriptor.At:
                    case Descriptor.Top:
                    case Descriptor.Cast:
                        IsServerSubquery = false;
                        break;
                }
            }

            public override void WalkCaseExp(Symbol sym)
            {
                IsServerSubquery = false;
                base.WalkCaseExp(sym);
            }

            public override void WalkFuncall(Notation.Record rec)
            {
                IsServerSubquery = false;
                base.WalkFuncall(rec);
            }

            public override void WalkXmlValueFunc(Symbol sym)
            {
                IsServerSubquery = false;
                base.WalkXmlValueFunc(sym);
            }

            public override void WalkTableQualifiedName(Qname qname)
            {
                if (IsServerSubquery)
                {
                    string prefix = null;
                    Notation.Record[] recs = notation.Select(qname, Descriptor.Prefix, 1);
                    if (recs.Length > 0)
                    {
                        Literal lit = (Literal)recs[0].Arg0;
                        prefix = lit.Data;
                    }
                    if (_provider_key_set)
                    {
                        if (prefix != ProviderKey)
                            IsServerSubquery = false;
                    }
                    else
                    {
                        _provider_key_set = true;
                        ProviderKey = prefix;
                    }
                }
            }

            public bool IsServerSubquery { get; protected set; }

            public String ProviderKey { get; protected set; }
        }

        public class ServerSubQueryAnalyzer : ServerQueryAnalyzer
        {
            public ServerSubQueryAnalyzer(Notation notation)
                : base(notation)
            {
            }

            private int _level = 0;

            public override void WalkSubQuery(Symbol squery)
            {
                if (_level == 0)
                {
                    ProviderKey = null;
                    IsServerSubquery = true;
                    _provider_key_set = false;
                }
                else
                    IsServerSubquery = false;
                _level++;
                base.WalkSubQuery(squery);
                _level--;
            }
        }

        #endregion

        public QueryContext Context { get; private set; }
        public bool IsServerQuery { get; private set; }
        public bool EnableServerQuery { get; set; }
        public bool EnableDSFilter { get; set; }
        public string TraceFileName { get; set; }

        public void Process(Notation notation)
        {
            CompileHints(notation);            
            //GenerateColumnRef(notation);
            ExpandExplitTable(notation);
            EvaluteConstant(notation);
            if (EnableServerQuery && Context != null)
            {
                ServerQueryAnalyzer sa = new ServerQueryAnalyzer(notation);
                sa.WalkStmt();
                IsServerQuery = sa.IsServerSubquery;
                if (IsServerQuery && CheckProvider(sa.ProviderKey, Context.DatabaseDictionary))
                {
                    Notation.Record[] recs = notation.Select(Descriptor.Root, 1);
                    Notation.Record[] recs1 = notation.Select(recs[0].Arg0, Descriptor.From, 1);
                    if (recs1.Length > 0)
                        notation.Confirm(recs[0].sym, Descriptor.HintServerSubquery, sa.ProviderKey);
                    return;
                }
            }
            PrepareEachSubquery(notation);
            TransformJoins(notation);
        }

        private bool CheckProvider(string ProviderKey, DatabaseDictionary dictionary)
        {
            DataSourceInfo dsi = dictionary.GetDataSource(ProviderKey);
            if (dsi != null && dsi.TableAccessor == AccessorType.DataProvider)
            {
                DataProviderHelper helper = new DataProviderHelper(dsi);
                if (helper.Smart)
                    return true;
            }
            return false;
        }

        public void PostProcess(Notation notation)
        {
            Notation.Record[] recs = notation.Select(Descriptor.Root, 1);
            SubqueryAnalyzer analyzer = new SubqueryAnalyzer(notation);
            analyzer.WalkStmt();
            Symbol[] qexpr = analyzer.Queries;
            Array.Sort<Symbol>(qexpr, new SubQueryComparer(notation));
            ServerSubQueryAnalyzer sa = new ServerSubQueryAnalyzer(notation);
            foreach (Symbol q in qexpr)
                if (recs[0].Arg0 != q)
                {
                    sa.WalkSubQuery(q);
                    if (sa.IsServerSubquery)
                        notation.Confirm(q, Descriptor.HintServerSubquery, sa.ProviderKey);
                    else
                    {
                        if (EnableDSFilter)
                            SelectFilters(notation, q);
                        ExtractJoinPredicates(notation, q);
                    }
                }
                else
                {
                    if (EnableDSFilter)
                        SelectFilters(notation, q);
                    ExtractJoinPredicates(notation, q);
                }
#if DEBUG
            if (TraceFileName != null)
            {
                Context._traceWriter = new StreamWriter(
                    new FileStream(TraceFileName, FileMode.Create));
            }
#endif            
        }

        protected struct DataMapping
        {
            public string column;
            public string dataType;
        }

        private DataMapping[] _hintMapping = null;
        private string[] _hintPk = null;
        private string[] _hintUnique = null;

        public void ProcessResults(Resultset rs)
        {
            foreach (RowType.TypeInfo rt in rs.RowType.Fields)
            {
                if (_hintPk != null && Array.IndexOf(_hintPk, rt.Name) != -1)
                    rt.ExportAsPk = true;
                if (_hintUnique != null && Array.IndexOf(_hintUnique, rt.Name) != -1)
                    rt.ExportAsUnique = true; 
                if (_hintMapping != null)
                    foreach(DataMapping mapping in _hintMapping)
                        if (mapping.column == rt.Name)
                        {
                            rt.ExportDataType = mapping.dataType;
                            break;
                        }
            }
        }

        private void CompileHints(Notation notation)
        {
            Notation.Record[] recs = notation.Select(Descriptor.OptimizerHint, 1);
            foreach (Notation.Record rec in recs)
            {
                String[] arr = Lisp.ToArray<String>(recs[0].args[0]);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<options>");
                foreach (String s in arr)
                    sb.AppendLine(s);
                sb.AppendLine("</options>");
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(sb.ToString());
                XmlNodeList nodeList = xmldoc.SelectNodes("//namespace");
                foreach (XmlElement elem in nodeList)
                    notation.Confirm(rec.sym, Descriptor.HintNamespace,
                        elem.GetAttribute("name"), elem.GetAttribute("href"));
                nodeList = xmldoc.SelectNodes("//column");
                foreach (XmlElement elem in nodeList)
                    notation.Confirm(rec.sym, Descriptor.HintColumn, elem.GetAttribute("name"), 
                        elem.GetAttribute("namespaceUri"), elem.GetAttribute("type"));
                nodeList = xmldoc.SelectNodes("//LdapSearchLimit");
                if (nodeList.Count > 0)
                {
                    XmlElement elem = (XmlElement)nodeList.Item(0);
                    if (elem.HasAttribute("value"))                        
                        Context.LdapSearchLimit = Int32.Parse(elem.GetAttribute("value"));
                }
                nodeList = xmldoc.SelectNodes("//ServerQuery");
                if (nodeList.Count > 0)
                {
                    XmlElement elem = (XmlElement)nodeList.Item(0);
                    if (elem.HasAttribute("enable"))
                        EnableServerQuery = ParseBool(elem.GetAttribute("enable"), EnableServerQuery);
                }
                nodeList = xmldoc.SelectNodes("//LocalCache");
                if (nodeList.Count > 0)
                {
                    XmlElement elem = (XmlElement)nodeList.Item(0);
                    if (elem.HasAttribute("enable"))
                    {
                        if (elem.HasAttribute("enable"))
                            Context.CacheEnabled = ParseBool(elem.GetAttribute("enable"), Context.CacheEnabled);
                    }
                }
                nodeList = xmldoc.SelectNodes("//Filter");
                if (nodeList.Count > 0)
                {
                    XmlElement elem = (XmlElement)nodeList.Item(0);
                    if (elem.HasAttribute("enable"))
                    {
                        if (elem.HasAttribute("enable"))
                            EnableDSFilter = ParseBool(elem.GetAttribute("enable"), EnableDSFilter);
                    }
                }
                nodeList = xmldoc.SelectNodes("//Export");
                if (nodeList.Count > 0)
                {
                    List<DataMapping> map = new List<DataMapping>();
                    foreach (XmlElement elem in nodeList)
                    {
                        DataMapping mapping;
                        mapping.column = elem.GetAttribute("column");
                        mapping.dataType = elem.GetAttribute("dataType");
                        map.Add(mapping);
                    }
                    _hintMapping = map.ToArray();
                }
                nodeList = xmldoc.SelectNodes("//PrimaryKey");
                if (nodeList.Count > 0)
                {
                    XmlElement elem = (XmlElement)nodeList.Item(0);
                    _hintPk = elem.InnerXml.Split(',');
                }
                nodeList = xmldoc.SelectNodes("//Unique");
                if (nodeList.Count > 0)
                {
                    XmlElement elem = (XmlElement)nodeList.Item(0);
                    _hintUnique = elem.InnerXml.Split(',');
                }
                nodeList = xmldoc.SelectNodes("//useSampleData");
                if (nodeList.Count > 0)
                    Context.UseSampleData = true;
                nodeList = xmldoc.SelectNodes("//set");
                if (nodeList.Count > 0)
                {
                    XmlElement elem = (XmlElement)nodeList.Item(0);
                    Context.DatabaseDictionary.SearchPath = elem.GetAttribute("path");
                }
#if DEBUG                
                nodeList = xmldoc.SelectNodes("//ILDump");
                if (nodeList.Count > 0)
                {
                    XmlElement elem = (XmlElement)nodeList.Item(0);
                    TraceFileName = elem.GetAttribute("file");
                }
#endif
                nodeList = xmldoc.SelectNodes("//limit");
                if (nodeList.Count > 0)
                {
                    XmlElement elem = (XmlElement)nodeList.Item(0);
                    if (elem.HasAttribute("value") && !Context.DisableLimitInput)
                        Context.LimitInputQuery = Int32.Parse(elem.GetAttribute("value"));
                }
            }
            notation.Remove(recs);
        }

        private void GenerateColumnRef(Notation notation)
        {
            QnameAnalyzer analyzer = new QnameAnalyzer(notation);
            analyzer.WalkStmt();
            foreach (QnameAnalyzer.QnameRecord qr in analyzer.Records)
            {
                Qname tableName = (Qname)qr.rec.Arg0;
                Qname qname = new Qname(new string[] { 
                    tableName.Name, ((Literal)qr.rec.Arg1).Data });                
                notation.Replace(qr.rec.sym, qname);
                Notation.Record[] recs = notation.Select(qr.rec.sym, Descriptor.Alias, 1);
                if (recs.Length > 0)
                {
                    Notation.Record rec1 = recs[0];
                    rec1.sym = qname;
                    notation.Replace(rec1, recs[0]);
                }
            }
        }

        private void ExpandExplitTable(Notation notation)
        {
            Notation.Record[] recs = notation.Select(Descriptor.Explicit, 1);
            foreach (Notation.Record rec in recs)
            {
                Symbol qexpr = new Symbol(Tag.SQuery);
                notation.Confirm(qexpr, Descriptor.Select);
                notation.Confirm(qexpr, Descriptor.TableName, rec.Arg0);
                notation.Confirm(qexpr, Descriptor.From, Lisp.Cons(rec.Arg0));
                notation.Replace(rec.sym, qexpr);                
            }
            notation.Pack();
        }

        private void PrepareEachSubquery(Notation notation)
        {
            SubqueryAnalyzer analyzer = new SubqueryAnalyzer(notation);
            analyzer.WalkStmt();
            Symbol[] qexpr = analyzer.Queries;
            Array.Sort<Symbol>(qexpr, new SubQueryComparer(notation));
            foreach (Symbol q in qexpr)
                PrepareAggregates(notation, q);
        }

        private void SelectFilters(Notation notation, Symbol qexpr)
        {
            Notation.Record[] where = notation.Select(qexpr, Descriptor.Where, 1);
            if (where.Length == 0)
                return;
            List<ExprNode> leaf = new List<ExprNode>();
            ExprNode rootNode = ExprNode.Create(notation, where[0].Arg0, leaf);
            PredicateAnalyzer analyzer = new PredicateAnalyzer(notation);
            foreach (ExprNode node in leaf)
            {
                analyzer.WalkPredicate(node.sym);
                if (node.isPredicate = analyzer.IsTablePredicate)
                    node.t = analyzer.TableName;
            }
            TableAnalyzer ta = new TableAnalyzer(notation);
            ta.WalkFromClause(qexpr);
            foreach (Symbol t in ta.Tables)
            {
                String alias;
                if (t.Tag == Tag.Qname)
                {
                    Notation.Record[] recs = notation.Select(t, Descriptor.Alias, 1);
                    if (recs.Length > 0)
                    {
                        Qname qname = (Qname)recs[0].Arg0;
                        alias = qname.Name;
                    }
                    else
                        alias = ((Qname)t).Name;
                }
                else
                    continue;
                if (ta.Tables.Count == 1)
                {
                    foreach (ExprNode node in leaf)
                        if (String.IsNullOrEmpty(node.t) && node.isPredicate)
                            node.t = alias;
                }
                ExprNode projection = rootNode.Probe(alias);
                if (projection != null)
                    notation.Confirm(t, Descriptor.HintFilter, projection.Compile(notation));
            }
        }

        private class JoinLink
        {
            public Qname qname1;
            public Qname qname2;            
        }

        private class JoinedTable
        {
            public Symbol t;
            public String alias;
            public List<JoinLink> links;

            public JoinedTable(Symbol t, String alias)
            {
                links = new List<JoinLink>();
                this.t = t;
                this.alias = alias;
            }
        }

        private void ExtractJoinPredicates(Notation notation, Symbol qexpr)
        {
            Notation.Record[] where = notation.Select(qexpr, Descriptor.Where, 1);
            if (where.Length == 0)
                return;
            List<JoinLink> links = new List<JoinLink>();
            Symbol tail = where[0].Arg0;
            while (tail != null)
            {
                Notation.Record[] recs = notation.Select(tail, Descriptor.LogicalAND, 2);
                if (recs.Length > 0)
                {
                    Notation.Record[] recs1 = notation.Select(recs[0].Arg1, Descriptor.Pred, 3);
                    if (recs1.Length > 0)
                    {
                        string op = (String)recs1[0].args[1];
                        if (op.Equals("=") && recs1[0].Arg0.Tag == Tag.Qname &&
                            recs1[0].Arg2.Tag == Tag.Qname)
                        {
                            JoinLink j = new JoinLink();
                            j.qname1 = (Qname)recs1[0].Arg0;
                            j.qname2 = (Qname)recs1[0].Arg2;
                            links.Add(j);
                        }
                    }
                    tail = recs[0].Arg0;
                }
                else
                {
                    recs = notation.Select(tail, Descriptor.Pred, 3);
                    if (recs.Length > 0)
                    {
                        string op = (String)recs[0].args[1];
                        if (op.Equals("=") && recs[0].Arg0.Tag == Tag.Qname &&
                            recs[0].Arg2.Tag == Tag.Qname)
                        {
                            JoinLink j = new JoinLink();
                            j.qname1 = (Qname)recs[0].Arg0;
                            j.qname2 = (Qname)recs[0].Arg2;
                            links.Add(j);
                        }                        
                    }
                    break;
                }
            }
            int query_name_index = 1;
            Notation.Record[] from = notation.Select(qexpr, Descriptor.From, 1);
            if (from.Length == 0)
                throw new InvalidOperationException();
            Symbol[] tables = Lisp.ToArray<Symbol>(from[0].args[0]);
            List<JoinedTable> joinedTables = new List<JoinedTable>();
            foreach (Symbol t in tables)
            {
                String alias = null;
                Notation.Record[] recs = notation.Select(t, Descriptor.Alias, 1);
                if (recs.Length > 0)
                {
                    Qname qname = (Qname)recs[0].Arg0;
                    alias = qname.Name;
                } else 
                    if (t.Tag == Tag.Qname)
                        alias = ((Qname)t).Name;
                if (alias == null)
                    alias = String.Format("query{0}", query_name_index++);
                joinedTables.Add(new JoinedTable(t, alias));
            }
            foreach (JoinLink j in links)
                for (int k = joinedTables.Count - 1; k >= 0; k--)
                {
                    JoinedTable jt = joinedTables[k];
                    if (j.qname1.Qualifier == jt.alias ||
                        j.qname2.Qualifier == jt.alias)
                    {
                        jt.links.Add(j);
                        break;
                    }
                }
            foreach (JoinedTable jt in joinedTables)
                if (jt.links.Count > 0)
                {
                    string[] k1 = new string[jt.links.Count];
                    string[] k2 = new string[jt.links.Count];
                    for (int k = 0; k < jt.links.Count; k++)
                        if (jt.links[k].qname2.Qualifier == jt.alias)
                        {
                            k1[k] = jt.links[k].qname1.ToString();
                            k2[k] = jt.links[k].qname2.ToString();
                        }
                        else
                        {
                            k1[k] = jt.links[k].qname2.ToString();
                            k2[k] = jt.links[k].qname1.ToString();
                        }
                    notation.Confirm(jt.t, Descriptor.HintKeyPair, k1, k2);
                }
        }

        private void PrepareAggregates(Notation notation, Symbol qexpr)
        {
            AggregationAnalyzer analyzer = new AggregationAnalyzer(notation);
            analyzer.WalkQueryExp(qexpr);
            if (analyzer.hasAggregation)
            {
                foreach (Qname qname in analyzer.Columns)
                {
                    bool found = false;
                    foreach (Qname g in analyzer.GroupByColumns)
                        if (g.Equals(qname))
                        {
                            found = true;
                            break;
                        }
                    if (!found)
                        throw new ESQLException(Properties.Resources.NotSingleGroupFunc, qname);
                }
                if (analyzer.hasComplexExpr)
                {
                    Notation.Record[] recs;
                    Dictionary<Symbol, Symbol> map = new Dictionary<Symbol, Symbol>();
                    recs = notation.Select(qexpr, Descriptor.From, 1);
                    recs = notation.ConnectivitySelect(recs[0]);
                    Notation.Record[] from = notation.Clone(map, recs);
                    notation.Remove(recs);
                    notation.Confirm(qexpr, Descriptor.From, Lisp.Cons(map[qexpr]));
                    notation.Confirm(from);
                    Notation.Record[] where = null;
                    recs = notation.Select(qexpr, Descriptor.Where, 1);
                    if (recs.Length > 0)
                    {
                        recs = notation.ConnectivitySelect(recs[0]);
                        where = notation.Clone(map, recs);
                        notation.Remove(recs);
                    }
                    object select_list = null;
                    List<string> fieldNames = new List<string>();
                    for (int k = 0; k < analyzer.GroupByColumns.Count; k++)
                    {                        
                        Qname qname = analyzer.GroupByColumns[k];
                        Qname alias = new Qname(Util.CreateUniqueName(fieldNames, qname.Name));
                        select_list = Lisp.Append(select_list, Lisp.Cons(qname));
                        notation.Confirm(qname, Descriptor.Alias, alias);
                        notation.Replace(qname, alias);
                    }
                    if (where != null)
                        notation.Confirm(where);
                    for (int k = 0; k < analyzer.GroupFuncs.Count; k++)
                    {                        
                        recs = notation.Select(analyzer.GroupFuncs[k], Descriptor.Aggregate, 2);
                        TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                        string prefix;
                        switch (w.Data)
                        {
                            case Token.COUNT:
                                prefix = "count";
                                break;
                            case Token.SUM:
                                prefix = "sum";
                                break;
                            case Token.AVG:
                                prefix = "avg";
                                break;
                            case Token.MIN:
                                prefix = "min";
                                break;
                            case Token.MAX:
                                prefix = "max";
                                break;
                            case Token.XMLAGG:
                                {
                                    prefix = "xmlagg";
                                    Notation.Record[] recs_s =
                                        notation.Select(analyzer.GroupFuncs[k], Descriptor.Order, 1);
                                    if (recs_s.Length > 0)
                                    {
                                        Symbol[] arr = Lisp.ToArray<Symbol>(recs_s[0].args[0]);
                                        for (int p = 0; p < arr.Length; p++)
                                            if (arr[p].Tag == Tag.Qname)
                                                select_list = Lisp.Append(select_list, Lisp.Cons(arr[p]));
                                            else
                                                throw new ESQLException(Properties.Resources.InvalidOrdinalInXMLAgg);
                                    }
                                }
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                        Qname alias = new Qname(String.Format("{0}{1}", prefix, k + 1));
                        if (recs[0].Arg1 is Value)
                        {
                            select_list = Lisp.Append(select_list, Lisp.Cons(recs[0].Arg1));
                            notation.Confirm(recs[0].Arg1, Descriptor.Alias, alias);
                        }
                        else
                        {
                            foreach (Notation.Record curr in notation.Select(recs[0].Arg1))
                            {
                                Notation.Record[] expr = notation.ConnectivitySelect(curr);
                                notation.Confirm(notation.Clone(map, expr));                         
                            }
                            select_list = Lisp.Append(select_list, Lisp.Cons(map[recs[0].Arg1]));
                            notation.Confirm(map[recs[0].Arg1], Descriptor.Alias, alias);
                        }
                        notation.Replace(recs[0].Arg1, alias);
                    }
                    notation.Confirm(map[qexpr], Descriptor.Select, select_list);
                    recs = notation.Select(qexpr, Descriptor.TableName, 1);
                    foreach (Notation.Record rec in recs)
                        rec.sym = map[qexpr];
                    notation.Pack();
                }
            }
            else
                if (analyzer.GroupByColumns.Count > 0)
                    throw new ESQLException(Properties.Resources.NotAGroupByExpr, analyzer.GroupByColumns[0]);
        }

        private void TransformJoins(Notation notation)
        {
            List<string> k1 = new List<string>();
            List<string> k2 = new List<string>();
            JoinAnalyzer analyzer = new JoinAnalyzer(notation);
            analyzer.WalkStmt();
            foreach (Symbol expr in analyzer.QualifiedJoins)
            {
                Notation.Record[] recs = notation.Select(expr, Descriptor.JoinSpec, 1);
                if (recs.Length > 0)
                {
                    if (ExtractJoinColumns(notation, k1, k2, recs[0].Arg0))
                    {
                        notation.Confirm(expr, Descriptor.HintKeyPair, k1.ToArray(), k2.ToArray());
                        notation.Remove(recs);
                    }
                    k1.Clear();
                    k2.Clear();
                }
            }
        }

        private bool IsNumberValue(object a, int v)
        {
            if (a is Value)
            {
                decimal n;
                if (a is Literal && Decimal.TryParse(((Literal)a).Data, out n) && n == v)
                    return true;
                else if (a is DecimalValue && ((DecimalValue)a).Data == v)
                    return true;
                else if (a is IntegerValue && ((IntegerValue)a).Data == v)
                    return true;
            }
            return false;
        }

        private Symbol NumberOperator(Descriptor desc, Symbol a, Symbol b)
        {
            if (a is Value || b is Value)
            {
                if (a is Value && !(a is Qname) && b is Value && !(b is Qname))
                {
                    Value v1 = (Value)a;
                    Value v2 = (Value)b;
                    TypeCode typecode = TypeConverter.GetTypeCode(v1.Data, v2.Data);
                    switch (typecode)
                    {
                        case TypeCode.Int32:
                            {
                                switch (desc)
                                {
                                    case Descriptor.Add:
                                        return new IntegerValue(Convert.ToInt32(v1.Data) + Convert.ToInt32(v2.Data));

                                    case Descriptor.Sub:
                                        return new IntegerValue(Convert.ToInt32(v1.Data) - Convert.ToInt32(v2.Data));

                                    case Descriptor.Mul:
                                        return new IntegerValue(Convert.ToInt32(v1.Data) * Convert.ToInt32(v2.Data));

                                    case Descriptor.Div:
                                        return new IntegerValue(Convert.ToInt32(v1.Data) / Convert.ToInt32(v2.Data));
                                }
                            }
                            break;

                        case TypeCode.Decimal:
                            {
                                switch (desc)
                                {
                                    case Descriptor.Add:
                                        return new DecimalValue(Convert.ToDecimal(v1.Data) + Convert.ToDecimal(v2.Data));

                                    case Descriptor.Sub:
                                        return new DecimalValue(Convert.ToDecimal(v1.Data) - Convert.ToDecimal(v2.Data));

                                    case Descriptor.Mul:
                                        return new DecimalValue(Convert.ToDecimal(v1.Data) * Convert.ToDecimal(v2.Data));

                                    case Descriptor.Div:
                                        return new DecimalValue(Convert.ToDecimal(v1.Data) / Convert.ToDecimal(v2.Data));
                                }
                            }
                            break;
                    }
                }
                else
                    switch (desc)
                    {   // Some simple check:
                        // 0 + a = a, a + 0 = a           
                        case Descriptor.Add: 
                            if (IsNumberValue(a, 0))   
                                return b;
                            else if (IsNumberValue(b, 0))
                                return a;
                            break;

                        // 1 * a = a, a * 1 = a
                        // 0 * a = 0, a * 0 = 0 
                        case Descriptor.Mul: 
                            if (IsNumberValue(a, 0) || IsNumberValue(b, 0)) 
                                return new IntegerValue(0);
                            else if (IsNumberValue(a, 1))
                                return b;
                            else if (IsNumberValue(b, 1))
                                return a;
                            break;
                    }
            }
            return null;
        }

        private void EvaluteConstant(Notation notation)
        {
            Symbol subst;
            do
            {
                subst = null;
                Notation.Record[] recs = notation.Select();
                for (int k = 0; k < recs.Length; k++)
                {
                    switch (recs[k].descriptor)
                    {
                        case Descriptor.Add:
                        case Descriptor.Sub:
                        case Descriptor.Mul:
                        case Descriptor.Div:
                            subst = NumberOperator(recs[k].descriptor, recs[k].Arg0, recs[k].Arg1);
                            break;

                        case Descriptor.Branch:
                            if (recs[k].Arg0 is Value)
                                subst = recs[k].Arg0;
                            break;
                    }
                    if (subst != null)
                    {
                        notation.Replace(recs[k].sym, subst);
                        notation.Remove(new Notation.Record[] { recs[k] });
                        break;
                    }
                }
            } while (subst != null);            
        }

        private bool ExtractJoinColumns(Notation notation, List<String> k1, List<String> k2, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.LogicalAND, 2);
            if (recs.Length > 0)
                return ExtractJoinColumns(notation, k1, k2, recs[0].Arg0) &&
                       ExtractJoinColumns(notation, k1, k2, recs[0].Arg1);
            else
            {
                recs = notation.Select(sym, Descriptor.Pred, 3);
                if (recs.Length > 0) // Canonical join predicate
                {
                    string op = (String)recs[0].args[1];
                    if (op.Equals("=") && recs[0].Arg0.Tag == Tag.Qname && 
                        recs[0].Arg2.Tag == Tag.Qname)
                    {
                        Qname qname1 = (Qname)recs[0].Arg0;
                        Qname qname2 = (Qname)recs[0].Arg2;
                        k1.Add(qname1.ToString());
                        k2.Add(qname2.ToString());
                        return true;
                    }
                }
            }
            return false;
        }

        private class ExprNode
        {
            public Descriptor desc;
            public ExprNode arg0;
            public ExprNode arg1;

            public Symbol sym;
            public String t;
            public bool isPredicate;

            public ExprNode(Symbol sym)
            {
                this.sym = sym;
            }

            public static ExprNode Create(Notation notation, Symbol expr, List<ExprNode> leaf)
            {
                ExprNode node = new ExprNode(expr);
                Notation.Record[] recs = notation.Select(expr, new Descriptor[] { 
                Descriptor.LogicalOR, Descriptor.LogicalAND });
                if (recs.Length > 0)
                {
                    node.desc = recs[0].descriptor;
                    node.arg0 = Create(notation, recs[0].Arg0, leaf);
                    node.arg1 = Create(notation, recs[0].Arg1, leaf);
                }
                else
                {
                    recs = notation.Select(expr, new Descriptor[] {
                        Descriptor.Inverse, Descriptor.Branch });
                    if (recs.Length > 0 && recs[0].args.Length > 0)
                    {
                        node.desc = recs[0].descriptor;
                        node.arg0 = Create(notation, recs[0].Arg0, leaf);
                    }
                    else
                    {
                        node.desc = Descriptor.Z;
                        leaf.Add(node);
                    }
                }
                return node;
            }

            public ExprNode Probe(String alias)
            {
                if (desc == Descriptor.Z)
                {
                    if (t == alias)
                        return this;
                    else
                        return null;
                }
                else
                    switch (desc)
                    {
                        case Descriptor.LogicalAND:
                        case Descriptor.LogicalOR:
                            {
                                ExprNode node1 = arg0.Probe(alias);
                                ExprNode node2 = arg1.Probe(alias);
                                if (node1 != null && node2 != null)
                                {
                                    ExprNode res = new ExprNode(sym);
                                    res.desc = desc;
                                    res.arg0 = node1;
                                    res.arg1 = node2;
                                    return res;
                                }
                                else
                                    if (desc == Descriptor.LogicalOR)
                                        return null;
                                    else
                                    {
                                        if (node1 != null)
                                            return node1;
                                        else
                                            return node2;
                                    }
                            }
                        
                        case Descriptor.Inverse:
                        case Descriptor.Branch:
                            {
                                ExprNode node = arg0.Probe(alias);
                                if (node == null)
                                    return node;
                                else
                                {
                                    ExprNode res = new ExprNode(sym);
                                    res.desc = desc;
                                    res.arg0 = node;
                                    return res;
                                }
                            }
                      

                        default:
                            throw new InvalidOperationException();
                    }
            }

            public Symbol Compile(Notation notation)
            {
                if (desc == Descriptor.Z)
                    return sym;
                else
                    switch (desc)
                    {
                        case Descriptor.LogicalAND:
                        case Descriptor.LogicalOR:
                            return notation.Confirm(new Symbol(Tag.BooleanExpr), desc,
                               arg0.Compile(notation), arg1.Compile(notation));

                        case Descriptor.Inverse:
                        case Descriptor.Branch:
                            return notation.Confirm(new Symbol(Tag.BooleanExpr), 
                                desc, arg0.Compile(notation));

                        default:
                            throw new InvalidOperationException();
                    }
            }
        }

        private bool ParseBool(string val, bool defval)
        {
            if (val == "1" || val == "T" || val == "True" ||
                            val == "t" || val == "true")
                return true;
            else
                if (val == "0" || val == "F" || val == "False" ||
                    val == "f" || val == "false")
                    return false;
            return defval;
        }
    }
}
