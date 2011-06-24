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
using System.Xml;

using DataEngine.Parser;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    public class Translator
    {
        private QueryContext context;
        private DatabaseDictionary dictionary;
        private Stack<DataAggregator> aggregators;
        private int query_name_index = 1;
        
        public Translator(QueryContext context)
        {
            this.context = context;
            dictionary = context.DatabaseDictionary;
            aggregators = new Stack<DataAggregator>();
        }

        public QueryNode Process(Notation notation)
        {
            DataSourceInfo dsi;
            Notation.Record[] recs = notation.Select(Descriptor.Root, 1);
            if (IsProviderSubquery(notation, recs[0].sym, out dsi))
                return new DataProviderQueryAccessor(dsi, notation, recs[0].sym);
            else
                return ProcessStmt(notation, recs[0].sym);
        }        

        private QueryNode ProcessStmt(Notation notation, Symbol stmt)
        {
            Notation.Record[] recs = notation.Select(stmt, Descriptor.Root, 1);
            Symbol qexpr = recs[0].Arg0;
            QueryNode node = ProcessQueryExp(notation, qexpr);
            ProcessQueryHint(notation, stmt, node);
            recs = notation.Select(stmt, Descriptor.Order, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                DataSorter.Column[] columns = new DataSorter.Column[arr.Length];
                for (int k = 0; k < arr.Length; k++)
                {
                    columns[k] = new DataSorter.Column();
                    if (arr[k] is Qname)
                    {
                        Qname qname = (Qname)arr[k];
                        columns[k].Name = qname.ToString();
                    }
                    else
                    {
                        Value val = (Value)arr[k];
                        columns[k].Name = Convert.ToInt32(val.Data);
                    }
                    if (notation.Flag(arr[k], Descriptor.Desc))
                        columns[k].Direction = SortDirection.Descending;
                }
                DataSorter sorter = new DataSorter(columns);
                sorter.ChildNodes.Add(node);
                return sorter;
            }
            else
                return node;
        }

        private void ProcessQueryHint(Notation notation, Symbol sym, QueryNode node)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.HintNamespace, 2);
            if (recs.Length > 0)
            {
                string[][] namespaces = new string[recs.Length][];
                for (int k = 0; k < recs.Length; k++)                    
                {
                    namespaces[k] = new string[2];
                    namespaces[k][0] = (string)recs[k].args[0];
                    namespaces[k][1] = (string)recs[k].args[1];
                }                
                node.Namespaces = namespaces;
            }
            recs = notation.Select(sym, Descriptor.HintColumn, 3);
            if (recs.Length > 0)
            {
                node.TypeManager = new XmlTypeManager();
                foreach (Notation.Record rec in recs)
                    node.TypeManager.Add((string)rec.args[0], (string)rec.args[1], (string)rec.args[2]);
            }
        }

        private QueryNode ProcessQueryExp(Notation notation, Symbol qexpr)
        {
            DataSourceInfo dsi;
            if (IsProviderSubquery(notation, qexpr, out dsi))
                return new DataProviderQueryAccessor(dsi, notation, qexpr);
            else
            {
                Notation.Record[] recs = notation.Select(qexpr,
                                new Descriptor[] { Descriptor.Union, Descriptor.Except }, 2);
                if (recs.Length > 0)
                {
                    ConnectorType connectorType;
                    switch (recs[0].descriptor)
                    {
                        case Descriptor.Except:
                            connectorType = ConnectorType.Except;
                            break;

                        default:
                            connectorType = ConnectorType.Union;
                            break;
                    }
                    QueryNode node1 = ProcessQueryExp(notation, recs[0].Arg0);
                    QueryNode node2 = ProcessQueryTerm(notation, recs[0].Arg1);
                    DataConnector connector = new DataConnector(connectorType,
                        notation.Flag(qexpr, Descriptor.Distinct));
                    connector.ChildNodes.Add(node1);
                    connector.ChildNodes.Add(node2);
                    return connector;
                }
                else
                    return ProcessQueryTerm(notation, qexpr);
            }
        }

        private QueryNode ProcessQueryTerm(Notation notation, Symbol qterm)
        {
            Notation.Record[] recs = notation.Select(qterm, 
                Descriptor.Intersect, 2);
            if (recs.Length > 0)
            {
                QueryNode node1 = ProcessQueryTerm(notation, recs[0].Arg0);
                QueryNode node2 = ProcessQueryTerm(notation, recs[0].Arg1);
                DataConnector connector = new DataConnector(ConnectorType.Intersect, 
                    notation.Flag(qterm, Descriptor.Distinct));
                connector.ChildNodes.Add(node1);
                connector.ChildNodes.Add(node2);
                return connector;                
            }
            else
                return ProcessSimpleTable(notation, qterm);
        }

        private QueryNode ProcessSimpleTable(Notation notation, Symbol sym)
        {
            switch (sym.Tag)
            {
                case Tag.SQuery:
                    return ProcessQuerySpec(notation, sym);

                case Tag.TableConstructor:
                    return ProcessTableConstructor(notation, sym);

                default:
                    throw new InvalidOperationException();
            }
        }

        private QueryNode ProcessTableConstructor(Notation notation, Symbol sym)
        {
            throw new NotImplementedException();
        }

        private QueryNode ProcessQuerySpec(Notation notation, Symbol sym)
        {
            SubqueryAnalyzer squery_analyzer = new SubqueryAnalyzer(notation);
            AggregationAnalyzer analyzer = new AggregationAnalyzer(notation);
            analyzer.WalkQuerySpec(sym);
            
            QueryNode collector = ProcessTableList(notation, sym);

            Notation.Record[] recs = notation.Select(sym, Descriptor.Where, 1);
            if (recs.Length > 0)
            {
                QueryNode filter = new DataFilter(ProcessSearchCondition(notation, recs[0].Arg0));
                filter.ChildNodes.Add(collector);                
                squery_analyzer.WalkWhereClause(sym);
                foreach (Symbol squery in squery_analyzer.Queries)
                {
                    QueryNode node = ProcessQueryExp(notation, squery);
                    node.NodeID = squery;
                    filter.ChildNodes.Add(node);
                }
                collector = filter;
            }

            if (analyzer.hasAggregation)
            {
                DataAggregator.Column[] columns = new DataAggregator.Column[analyzer.GroupFuncs.Count];
                List<string> fieldNames = new List<string>();
                for (int k = 0; k < analyzer.GroupFuncs.Count; k++)
                {
                    columns[k] = new DataAggregator.Column();
                    columns[k].Expr = analyzer.GroupFuncs[k];
                    if (notation.Flag(columns[k].Expr, Descriptor.AggCount))
                        columns[k].Functor = AggregateFunctor.RowCount;
                    else
                    {
                        recs = notation.Select(columns[k].Expr, Descriptor.Aggregate, 2);
                        if (recs.Length > 0)
                        {
                            columns[k].ColumnName = recs[0].Arg1.ToString();
                            if (notation.Flag(columns[k].Expr, Descriptor.Distinct))
                                columns[k].Distinct = true;
                            TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                            switch (w.Data)
                            {
                                case Token.COUNT:
                                    columns[k].Functor = AggregateFunctor.Count;
                                    break;
                                case Token.SUM:
                                    columns[k].Functor = AggregateFunctor.Sum;
                                    break;
                                case Token.AVG:
                                    columns[k].Functor = AggregateFunctor.Avg;
                                    break;
                                case Token.MIN:
                                    columns[k].Functor = AggregateFunctor.Min;
                                    break;
                                case Token.MAX:
                                    columns[k].Functor = AggregateFunctor.Max;
                                    break;
                                case Token.XMLAGG:
                                    {
                                        columns[k].Functor = AggregateFunctor.XMLAgg;
                                        Notation.Record[] recs_s = notation.Select(columns[k].Expr, Descriptor.Order, 1);
                                        if (recs_s.Length > 0)
                                        {
                                            Symbol[] arr = Lisp.ToArray<Symbol>(recs_s[0].args[0]);
                                            columns[k].OrderFields = new Object[arr.Length];
                                            columns[k].Direction = new SortDirection[arr.Length];
                                            for (int p = 0; p < arr.Length; p++)
                                            {
                                                if (arr[k].Tag == Tag.Qname)
                                                    columns[k].OrderFields[p] = ((Qname)arr[k]).Name;
                                                else
                                                    columns[k].OrderFields[p] = ((IntegerValue)arr[k]).Data;
                                                columns[k].Direction[p] = notation.Flag(arr[k], Descriptor.Desc) ?
                                                    SortDirection.Descending : SortDirection.Ascending;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                        }
                    }
                    columns[k].Name = Util.CreateUniqueName(fieldNames, columns[k].ToString());
                }

                string[] groupByColumns = new string[analyzer.GroupByColumns.Count];
                for (int k = 0; k < groupByColumns.Length; k++)
                    groupByColumns[k] = analyzer.GroupByColumns[k].ToString();
                DataAggregator aggregator = new DataAggregator(columns, groupByColumns);
                aggregator.ChildNodes.Add(collector);
                collector = aggregator;
                aggregators.Push(aggregator);

                recs = notation.Select(sym, Descriptor.Having, 1);
                if (recs.Length > 0)
                {
                    QueryNode filter = new DataFilter(ProcessSearchCondition(notation, recs[0].Arg0));
                    filter.ChildNodes.Add(collector);
                    squery_analyzer.WalkWhereClause(sym);
                    foreach (Symbol squery in squery_analyzer.Queries)
                    {
                        QueryNode node = ProcessQueryExp(notation, squery);
                        node.NodeID = squery;
                        filter.ChildNodes.Add(node);
                    }
                    collector = filter;
                }
            }

            DataSelector selector = ProcessSelectList(notation, sym);  
            selector.ChildNodes.Add(collector);
            squery_analyzer.WalkSelectList(sym);
            foreach (Symbol squery in squery_analyzer.Queries)
            {
                QueryNode node = ProcessQueryExp(notation, squery);
                node.NodeID = squery;
                selector.ChildNodes.Add(node);
            }

            if (analyzer.hasAggregation)
                aggregators.Pop();

            recs = notation.Select(sym, Descriptor.Top, 1);
            if (recs.Length > 0)
                selector.TopRows = (int)recs[0].args[0];
            
            recs = notation.Select(sym, Descriptor.Order, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                DataSorter.Column[] columns = new DataSorter.Column[arr.Length];
                for (int k = 0; k < arr.Length; k++)
                {
                    columns[k] = new DataSorter.Column();
                    if (arr[k] is Qname)
                    {
                        Qname qname = (Qname)arr[k];
                        columns[k].Name = qname.ToString();
                    }
                    else
                    {
                        Value val = (Value)arr[k];
                        columns[k].Name = Convert.ToInt32(val.Data);
                    }
                    if (notation.Flag(arr[k], Descriptor.Desc))
                        columns[k].Direction = SortDirection.Descending;
                }
                DataSorter sorter = new DataSorter(columns);
                sorter.ChildNodes.Add(selector);
                return sorter;
            }
            else
                return selector;
        }

        private QueryNode ProcessTableList(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.From, 1);
            if (recs.Length > 0)
            {
                QueryNode res = null;
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k == 0)
                        res = ProcessTableRef(notation, arr[k]);
                    else
                    {
                        QueryNode node = ProcessTableRef(notation, arr[k]);
                        QueryNode dataJoin;
                        if (ContainsDynatable(node))
                            dataJoin = new DetailJoin();
                        else
                        {
                            DataJoin.KeyResolver keyResolver;
                            recs = notation.Select(arr[k], Descriptor.HintKeyPair, 2);
                            if (recs.Length > 0) // Implict inner join
                            {
                                string[] k1 = (string[])recs[0].args[0];
                                string[] k2 = (string[])recs[0].args[1];
                                keyResolver = new DataJoin.QualifiedJoin(k1, k2);
                            }
                            else
                                keyResolver = null;
                            dataJoin = new DataJoin(JoinMethod.NestedLoops, keyResolver, JoinSpec.Inner);
                        }
                        dataJoin.ChildNodes.Add(res);
                        dataJoin.ChildNodes.Add(node);
                        res = dataJoin;
                    }
                }
                return res;
            }
            else // Short select list support
            {
                recs = notation.Select(sym, new Descriptor[] { Descriptor.Select });
                if (recs.Length > 0 && recs[0].args.Length == 0)
                    throw new ESQLException(Properties.Resources.MissingFromClause);

                QueryNode collector = new DataCollector("$dual");
                collector.ChildNodes.Add(new DualNode());
                return collector;
            } 
        }

        private QueryNode ProcessTableRef(Notation notation, Symbol sym)
        {
            if (sym.Tag == Tag.Join)
                return ProcessJoinedTable(notation, sym);
            else
                return ProcessTableRefSimple(notation, sym);
        }

        private QueryNode ProcessTableRefSimple(Notation notation, Symbol sym)
        {
            bool internal_sort = false;
            QueryNode collector = null;
            string alias = null;            
            Notation.Record[] recs = notation.Select(sym, Descriptor.Alias, 1);
            if (recs.Length > 0)
            {
                Qname qname = (Qname)recs[0].Arg0;
                if (qname.IsNonQualifiedName)
                    alias = qname.Name;
                else
                    throw new ESQLException(Properties.Resources.InvalidTableAlias, qname.ToString());                
            }
            if (sym is Qname)
            {
                QueryNode accessor;
                Qname qname = (Qname)sym;
                if (alias == null)
                    alias = qname.Name;
                string prefix = null;
                recs = notation.Select(qname, Descriptor.Prefix, 1);
                if (recs.Length > 0)
                {
                    Literal lit = (Literal)recs[0].Arg0;
                    prefix = lit.Data;
                }                    
                TableType tableType = dictionary.GetTableType(prefix, Util.UnquoteName(qname.ToArray()));     
                switch (tableType.DataSource.TableAccessor)
                {
                    case AcessorType.DataProvider:
                        {                            
                            accessor = new DataProviderTableAccessor(tableType);
                            if (tableType.Smart)
                            {
                                recs = notation.Select(sym, Descriptor.HintFilter, 1);
                                if (recs.Length > 0)
                                    ((DataProviderTableAccessor)accessor).FilterPredicate =
                                        ProcessSearchCondition(notation, recs[0].Arg0);
                                recs = notation.Select(sym, Descriptor.HintSort, 1);
                                if (recs.Length > 0)
                                {
                                    string[] columns = Lisp.ToArray<String>(recs[0].args[0]);
                                    DataProviderTableAccessor.SortColumn[] sortColumns =
                                        new DataProviderTableAccessor.SortColumn[columns.Length];
                                    for (int k = 0; k < columns.Length; k++)
                                    {
                                        sortColumns[k] = new DataProviderTableAccessor.SortColumn();
                                        sortColumns[k].ColumnName = columns[k];
                                    }
                                    ((DataProviderTableAccessor)accessor).SortColumns = sortColumns;
                                    internal_sort = true;
                                }
                            }
                        }
                        break;

                    case AcessorType.XMLFile:
                        {
                            FlatFileAccessor fileAccessor = new FlatFileAccessor(tableType.TableName);
                            accessor = new XmlDataAccessor();
                            accessor.ChildNodes.Add(fileAccessor);
                        }
                        break;

                    case AcessorType.FlatFile:
                        {
                            FlatFileAccessor fileAccessor = new FlatFileAccessor(tableType.TableName);
                            accessor = new TextDataAccessor();
                            accessor.ChildNodes.Add(fileAccessor);
                        }
                        break;

                    case AcessorType.XlFile:
                        accessor = new XLDataAccessor(dictionary, tableType.QualifiedName, false);
                        break;

                    case AcessorType.XlFileTable:
                        accessor = new XLDataAccessor(dictionary, tableType.QualifiedName, true);
                        break;

                    case AcessorType.DataSet:
                        DataSet ds = (DataSet)tableType.DataSource.DataContext;
                        accessor = new AdoTableAccessor(ds.Tables[tableType.TableName]);
                        break;

                    case AcessorType.DataTable:
                        if (tableType.DataSource.DataContext != null)
                            accessor = new AdoTableAccessor((DataTable)tableType.DataSource.DataContext);
                        else
                            accessor = new AdoTableAccessor(tableType.TableName);
                        break;

                    default:
                        throw new NotImplementedException();
                }
                collector = new DataCollector(alias);
                collector.ChildNodes.Add(accessor);
            }
            else
            {
                recs = notation.Select(sym, Descriptor.Dynatable, 1);
                if (recs.Length > 0)
                {
                    if (alias == null)
                    {
                        SqlWriter writer = new SqlWriter(notation);
                        writer.WriteValueExp(recs[0].Arg0);
                        alias = writer.ToString();
                    }
                    collector = new DynatableAccessor(alias, Lisp.List(ID.Dyncast, 
                        ProcessValueExpr(notation, recs[0].Arg0)), dictionary);
                }
                else
                {
                    if (alias == null)
                        alias = String.Format("query{0}", query_name_index++);
                    QueryNode node = ProcessQueryExp(notation, sym);
                    collector = new DataCollector(alias);
                    collector.ChildNodes.Add(node);
                }
            }
            if (!internal_sort)
            {
                recs = notation.Select(sym, Descriptor.HintSort, 1);
                if (recs.Length > 0)
                {
                    string[] columns = Lisp.ToArray<String>(recs[0].args[0]);
                    DataSorter.Column[] sortColumns = new DataSorter.Column[columns.Length];
                    for (int k = 0; k < columns.Length; k++)
                    {
                        sortColumns[k] = new DataSorter.Column();
                        sortColumns[k].Name = columns[k];
                    }
                    DataSorter sorter = new DataSorter(sortColumns);
                    sorter.ChildNodes.Add(collector);
                    return sorter;
                }
            }
            return collector;
        }

        private QueryNode ProcessJoinedTable(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] {
                Descriptor.CrossJoin,
                Descriptor.UnionJoin,
                Descriptor.NaturalJoin,
                Descriptor.QualifiedJoin,
                Descriptor.Branch });

            switch (recs[0].descriptor)
            {
                case Descriptor.CrossJoin:
                    {
                        QueryNode node = ProcessTableRef(notation, recs[0].Arg1);
                        QueryNode dataJoin;
                        if (ContainsDynatable(node))
                            dataJoin = new DetailJoin();
                        else
                            dataJoin = new DataJoin(JoinMethod.NestedLoops, null, JoinSpec.Inner);
                        dataJoin.ChildNodes.Add(ProcessTableRefSimple(notation, recs[0].Arg0));
                        dataJoin.ChildNodes.Add(node);
                        return dataJoin;
                    }

                case Descriptor.UnionJoin:
                    {
                        //if (recs[0].Arg1.Tag == Tag.Dynatable)
                        //    throw new ESQLException(Properties.Resources.IllegalJoin);
                        UnionJoin unionJoin = new UnionJoin();
                        unionJoin.ChildNodes.Add(ProcessTableRefSimple(notation, recs[0].Arg0));
                        unionJoin.ChildNodes.Add(ProcessTableRef(notation, recs[0].Arg1));
                        return unionJoin;
                    }

                case Descriptor.NaturalJoin:
                    return ProcessJoin(notation, sym, recs[0].Arg0, recs[0].Arg1, new DataJoin.NaturalJoin());

                case Descriptor.QualifiedJoin:
                    return ProcessJoin(notation, sym, recs[0].Arg0, recs[0].Arg1, ProcessJoinSpec(notation, sym));

                case Descriptor.Branch:
                    return ProcessTableRef(notation, recs[0].Arg0);

                default:
                    throw new InvalidOperationException();
            }
        }

        private JoinMethod GetJoinMethod(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.HintJoin, 1);
            if (recs.Length > 0)
                return (JoinMethod)recs[0].args[0];
            else
                return JoinMethod.NestedLoops;
        }

        private int GetJoinType(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.JoinType, 1);
            if (recs.Length > 0)
            {
                TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                return w.Data;
            }
            else
                return Token.INNER;
        }

        private QueryNode ProcessJoin(Notation notation, Symbol sym, 
            Symbol jexpr1, Symbol jexpr2, DataJoin.KeyResolver resolver)
        {
            JoinMethod method;
            //if (jexpr2.Tag == Tag.Dynatable)
            //    throw new ESQLException(Properties.Resources.IllegalJoin);
            method = GetJoinMethod(notation, sym);
            switch (GetJoinType(notation, sym))
            {
                case Token.INNER:
                    {
                        DataJoin dataJoin = new DataJoin(method, resolver, JoinSpec.Inner);
                        dataJoin.ChildNodes.Add(ProcessTableRefSimple(notation, jexpr1));
                        dataJoin.ChildNodes.Add(ProcessTableRef(notation, jexpr2));
                        ProcessJoinCondition(notation, sym, dataJoin);
                        return dataJoin;
                    }

                case Token.LEFT:
                    {
                        DataJoin dataJoin = new DataJoin(method, resolver, JoinSpec.LeftOuter);
                        dataJoin.ChildNodes.Add(ProcessTableRefSimple(notation, jexpr1));
                        dataJoin.ChildNodes.Add(ProcessTableRef(notation, jexpr2));
                        ProcessJoinCondition(notation, sym, dataJoin);
                        return dataJoin;
                    }

                case Token.RIGHT:
                    {
                        DataJoin dataJoin = new DataJoin(method, resolver, JoinSpec.RightOuter);
                        dataJoin.ChildNodes.Add(ProcessTableRef(notation, jexpr2));
                        dataJoin.ChildNodes.Add(ProcessTableRefSimple(notation, jexpr1));
                        ProcessJoinCondition(notation, sym, dataJoin);
                        return dataJoin;
                    }

                case Token.FULL:
                    {
                        DataJoin dataJoin1 = new DataJoin(method, resolver, JoinSpec.LeftOuter);
                        dataJoin1.ChildNodes.Add(ProcessTableRefSimple(notation, jexpr1));
                        dataJoin1.ChildNodes.Add(ProcessTableRef(notation, jexpr2));
                        ProcessJoinCondition(notation, sym, dataJoin1);
                        DataJoin dataJoin2 = new DataJoin(method, resolver, JoinSpec.RightOuter);
                        dataJoin2.ChildNodes.Add(ProcessTableRef(notation, jexpr2));
                        dataJoin2.ChildNodes.Add(ProcessTableRefSimple(notation, jexpr1));
                        ProcessJoinCondition(notation, sym, dataJoin2);
                        DataConnector connector = new DataConnector(ConnectorType.ComplexUnion, false);
                        connector.ChildNodes.Add(dataJoin1);
                        connector.ChildNodes.Add(dataJoin2);
                        return connector;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private DataJoin.KeyResolver ProcessJoinSpec(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.HintKeyPair, 2);
            if (recs.Length > 0)
            {
                string[] k1 = (string [])recs[0].args[0];
                string[] k2 = (string [])recs[0].args[1];
                return new DataJoin.QualifiedJoin(k1, k2);
            }
            else
            {
                recs = notation.Select(sym, Descriptor.JoinSpec, 1);
                if (recs.Length > 0)
                {
                    Notation.Record[] recs1 = notation.Select(recs[0].Arg0, Descriptor.Using, 1);
                    if (recs1.Length > 0)
                    {
                        Symbol[] arr = Lisp.ToArray<Symbol>(recs1[0].args[0]);
                        String[] columns = new string[arr.Length];
                        for (int k = 0; k < arr.Length; k++)
                            columns[k] = arr[k].ToString();
                        return new DataJoin.NamedColumnsJoin(columns);
                    }
                }
            }
            return null;
        }

        private void ProcessJoinCondition(Notation notation, Symbol sym, DataJoin dataJoin)
        {
            Notation.Record[] recs1 = notation.Select(sym, Descriptor.JoinSpec, 1);
            if (recs1.Length > 0)
            {
                Notation.Record[] recs2 = notation.Select(recs1[0].Arg0,
                    new Descriptor[] { Descriptor.Using, Descriptor.Constraint }, 1);
                if (recs2.Length == 0)
                {
                    dataJoin.FilterPredicate = ProcessSearchCondition(notation, recs1[0].Arg0);
                    SubqueryAnalyzer analyzer = new SubqueryAnalyzer(notation);
                    analyzer.WalkSearchCondition(recs1[0].Arg0);
                    foreach (Symbol squery in analyzer.Queries)
                    {
                        QueryNode node = ProcessQueryExp(notation, squery);
                        node.NodeID = squery;
                        dataJoin.ChildNodes.Add(node);
                    }
                }
            }
        }

        private DataSelector ProcessSelectList(Notation notation, Symbol sym)
        {
            DataSelector.Column[] columns = null;
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Select });
            if (recs.Length > 0)
            {
                if (recs[0].args.Length > 0)
                {
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                    columns = new DataSelector.Column[arr.Length];
                    for (int k = 0; k < arr.Length; k++)
                    {
                        columns[k] = new DataSelector.Column();
                        Notation.Record[] recs1 = notation.Select(arr[k], Descriptor.TableFields, 1);
                        if (recs1.Length > 0)
                        {
                            Qname qname = (Qname)recs1[0].Arg0;
                            columns[k].Expr = Lisp.List(DataSelector.Table, qname.Name);
                        }
                        else
                        {
                            bool value_exp = false;
                            if (arr[k].Tag == Tag.SQuery && notation.Flag(arr[k], Descriptor.Dynatable))
                                columns[k].Expr = Lisp.List(ID.SQuery, arr[k], BindSQuery(notation, arr[k]));
                            else
                            {
                                columns[k].Expr = ProcessValueExpr(notation, arr[k]);
                                value_exp = true;
                            }
                            recs1 = notation.Select(arr[k], Descriptor.Alias, 1);
                            if (recs1.Length > 0)
                            {
                                Qname qname = (Qname)recs1[0].Arg0;
                                columns[k].Alias = qname.Name;
                            }
                            else
                                if (value_exp && !Lisp.IsAtom(columns[k].Expr))
                                {
                                    SqlWriter writer = new SqlWriter(notation);
                                    writer.WriteValueExp(arr[k]);
                                    columns[k].Alias = writer.ToString();
                                }
                        }
                    }
                }
            }
            else
                throw new InvalidOperationException();
            
            return new DataSelector(notation.Flag(sym, Descriptor.Distinct), columns);
        }        

        private object ProcessValueExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Concat, 2);
            if (recs.Length > 0)
                return Lisp.List(ID.Concat, ProcessValueExpr(notation, recs[0].Arg0),
                    ProcessCharFactor(notation, recs[0].Arg1));
            else
                return ProcessCharFactor(notation, sym);
        }

        private object ProcessCharFactor(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Substring, Descriptor.StringUpper, 
                Descriptor.StringLower, Descriptor.StringConvert, Descriptor.StringTrim });
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.Substring:
                        {
                            object arg0 = ProcessValueExpr(notation, recs[0].Arg0);
                            object arg1 = ProcessValueExpr(notation, recs[0].Arg1);
                            if (recs[0].args.Length == 3)
                                return Lisp.List(ID.Substr, arg0, arg1,
                                    ProcessValueExpr(notation, recs[0].Arg2));
                            else
                                return Lisp.List(ID.Substr, arg0, arg1, null);
                        }

                    case Descriptor.StringUpper:
                        return Lisp.List(ID.StrUpper, ProcessValueExpr(notation, recs[0].Arg0));

                    case Descriptor.StringLower:
                        return Lisp.List(ID.StrLower, ProcessValueExpr(notation, recs[0].Arg0));

                    case Descriptor.StringConvert:
                        {
                            Qname qname = (Qname)recs[0].Arg1;
                            return Lisp.List(ID.StrConvert,
                                ProcessValueExpr(notation, recs[0].Arg0), qname.Name);
                        }

                    case Descriptor.StringTrim:
                        {                                                        
                            object trim_char;
                            if (recs[0].args[1] is Literal)
                            {
                                Literal lit = (Literal)recs[0].args[1];
                                trim_char = lit.Data;
                            }
                            else
                                trim_char = ProcessValueExpr(notation, recs[0].Arg1);
                            object trim_src = ProcessValueExpr(notation, recs[0].Arg2);
                            TokenWrapper w = (TokenWrapper)recs[0].args[0];
                            switch (w.Data)
                            {
                                case Token.TRAILING:
                                    return Lisp.List(ID.TrimEnd, trim_src, trim_char);

                                case Token.LEADING:
                                    return Lisp.List(ID.TrimStart, trim_src, trim_char);

                                default: /* Token.BOTH */
                                    return Lisp.List(ID.Trim, trim_src, trim_char);
                            }
                        }

                    default:
                        throw new InvalidOperationException();
                }
            }
            else
                return ProcessNumValueExpr(notation, sym);
        }

        private object ProcessNumValueExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym,
                new Descriptor[] { Descriptor.Add, Descriptor.Sub }, 2);
            if (recs.Length > 0)
            {
                object arg0 = ProcessNumValueExpr(notation, recs[0].Arg0);
                switch (recs[0].descriptor)
                {
                    case Descriptor.Add:
                        return Lisp.List(Funcs.Add, arg0, ProcessTerm(notation, recs[0].Arg1));

                    case Descriptor.Sub:
                        return Lisp.List(Funcs.Sub, arg0, ProcessTerm(notation, recs[0].Arg1));

                    default:
                        throw new InvalidOperationException();
                }
            }
            else
                return ProcessTerm(notation, sym);
        }

        private object ProcessTerm(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym,
                new Descriptor[] { Descriptor.Mul, Descriptor.Div }, 2);
            if (recs.Length > 0)
            {
                object arg0 = ProcessTerm(notation, recs[0].Arg0);
                switch (recs[0].descriptor)
                {
                    case Descriptor.Mul:
                        return Lisp.List(Funcs.Mul, arg0, ProcessFactor(notation, recs[0].Arg1));

                    case Descriptor.Div:
                        return Lisp.List(Funcs.Div, arg0, ProcessFactor(notation, recs[0].Arg1));

                    default:
                        throw new InvalidOperationException();
                }
            }
            else
                return ProcessFactor(notation, sym);
        }

        private object ProcessFactor(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.UnaryMinus, 1);
            if (recs.Length > 0)
                return Lisp.List(Funcs.Sub, 0.0, ProcessNumPrimary(notation, recs[0].Arg0));
            else
                return ProcessNumPrimary(notation, sym);
        }

        private object ProcessNumPrimary(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.PosString, 2);
            if (recs.Length > 0)
                return Lisp.List(ID.StrPos,
                    ProcessValueExpr(notation, recs[0].Arg0), ProcessValueExpr(notation, recs[0].Arg1));
            else
                if (sym.Tag == Tag.SQLX)
                    return ProcessXmlValueFunc(notation, sym);
                else
                    return ProcessNumExpPrimary(notation, sym);
        }

        private object ProcessXmlValueFunc(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.XMLConcat, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                object[] func = new Object[arr.Length + 1];
                func[0] = ID.XmlValueConcat;
                for (int k = 0; k < arr.Length; k++)
                    func[k + 1] = Lisp.List(Funcs.LambdaQuote, ProcessValueExpr(notation, arr[k]));
                return Lisp.List(func);
            }
            recs = notation.Select(sym, Descriptor.XMLQuery, 3);
            if (recs.Length > 0)
            {
                //DataEngine.XQuery.XQueryCommand command = 
                //    new DataEngine.XQuery.XQueryCommand(new DataEngine.XQuery.XQueryDBContext(dictionary, context.NameTable));
                //command.SearchPath = dictionary.SearchPath;
                string commandText = (String)recs[0].args[0];
                XQueryAdapter adapter = XQueryAdapter.Create(dictionary, context.NameTable, commandText);
                context.AddResource(adapter);
                object context_item = null;
                object arg = null;                
                if (recs[0].args[1] != null)
                {                    
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                    List<object> form = new List<object>();
                    form.Add(Funcs.List);
                    for (int k = 0; k < arr.Length; k++)
                    {
                        object expr;
                        if (arr[k].Tag == Tag.SQuery && notation.Flag(arr[k], Descriptor.Dynatable))
                            expr = Lisp.List(ID.SQuery, arr[k], BindSQuery(notation, arr[k]));
                        else
                            expr = ProcessValueExpr(notation, arr[k]);
                        Notation.Record[] recs1 = notation.Select(arr[k], Descriptor.Alias, 1);
                        if (recs1.Length > 0)
                        {
                            Qname qname = (Qname)recs1[0].Arg0;
                            adapter.AddParameter(qname.Name);
                            form.Add(expr);
                        }
                        else
                        {
                            if (context_item != null)
                                throw new ESQLException(Properties.Resources.DuplicateContextItem);
                            context_item = expr;
                        }                            
                    }
                    if (form.Count > 1)
                        arg = Lisp.List(form.ToArray());
                }
                return Lisp.List(ID.XmlQuery, adapter, context_item, arg);
            }
            recs = notation.Select(sym, Descriptor.XMLForestAll, 0);
            if (recs.Length > 0)
                return Lisp.List(ID.XmlForest);
            recs = notation.Select(sym, Descriptor.XMLForest,  1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                object[] func = new Object[arr.Length + 1];
                func[0] = ID.XmlForest;
                for (int k = 0; k < arr.Length; k++)
                {
                    XmlValueOption option = XmlValueOption.EmptyOnNull;
                    Notation.Record[] recs1 = notation.Select(arr[k], Descriptor.ContentOption, 1);
                    if (recs1.Length > 0)
                    {
                        TokenWrapper w = (TokenWrapper)recs1[0].Arg0;
                        switch (w.Data)
                        {
                            case Token.OPTION_NULL:
                                option = XmlValueOption.NullOnNull;
                                break;
                            case Token.OPTION_EMPTY:
                                option = XmlValueOption.EmptyOnNull;
                                break;
                            case Token.OPTION_ABSENT:
                                option = XmlValueOption.AbsentOnNull;
                                break;
                            case Token.OPTION_NIL:
                                option = XmlValueOption.NilOnNull;
                                break;
                            case Token.NO_CONTENT:
                                option = XmlValueOption.NilOnNoContent;
                                break;
                        }
                    }
                    if (arr[k].Tag == Tag.TableFields)
                    {
                        recs1 = notation.Select(arr[k], Descriptor.TableFields, 1);
                        if (recs1.Length > 0)
                            func[k + 1] = Lisp.List(DataSelector.Table, ((Qname)recs1[0].Arg0).Name, option);
                    }
                    else
                    {
                        string name;
                        recs1 = notation.Select(arr[k], Descriptor.Alias, 1);
                        if (recs1.Length > 0)
                            name = ((Qname)recs1[0].Arg0).UnqoutedName;
                        else
                            if (arr[k].Tag == Tag.Qname)
                                name = ((Qname)arr[k]).UnqoutedName;
                            else
                                throw new ESQLException(Properties.Resources.ParameterMustBeAliased, k + 1, "XMLForest");
                        func[k + 1] = Lisp.List(ID.XmlValueElem, name, Lisp.List(ID.XmlValueConcat,
                            Lisp.List(Funcs.LambdaQuote, ProcessValueExpr(notation, arr[k]))), option);
                    }
                }
                Notation.Record[] recs_ns = notation.Select(sym, Descriptor.XMLNamespaces, 1);
                if (recs_ns.Length > 0)
                    return ProcessXmlNamespaces(notation, recs_ns, Lisp.List(func));
                else
                    return Lisp.List(func);
            }
            recs = notation.Select(sym, Descriptor.XMLParse, 3);
            if (recs.Length > 0)
            {
                XmlValueParseFlags flags = 0;
                if (((Literal)recs[0].Arg0).Data.Equals("CONTENT",
                    StringComparison.InvariantCultureIgnoreCase))
                    flags = XmlValueParseFlags.ParseContent;
                TokenWrapper w = (TokenWrapper)recs[0].Arg2;
                if (w != null && w.Data == Token.PRESERVE_WHITESPACE)
                    flags |= XmlValueParseFlags.PreserveWhitespace;
                return Lisp.List(ID.XmlValueParse, flags,
                    Lisp.List(Funcs.LambdaQuote, ProcessValueExpr(notation, recs[0].Arg1)));
            }
            recs = notation.Select(sym, Descriptor.XMLPI, 2);
            if (recs.Length > 0)
            {
                Qname qname = (Qname)recs[0].Arg0;
                return Lisp.List(ID.XmlValuePi, qname.Name, recs[0].Arg1 == null ? null :
                     Lisp.List(Funcs.LambdaQuote, ProcessValueExpr(notation, recs[0].Arg1)));
            }
            recs = notation.Select(sym, Descriptor.XMLComment, 1);
            if (recs.Length > 0)
                return Lisp.List(ID.XmlValueComment,
                    Lisp.List(Funcs.LambdaQuote, ProcessValueExpr(notation, recs[0].Arg0)));
            recs = notation.Select(sym, Descriptor.XMLCDATA, 1);
            if (recs.Length > 0)
                return Lisp.List(ID.XmlValueCdata,
                    Lisp.List(Funcs.LambdaQuote, ProcessValueExpr(notation, recs[0].Arg0)));
            recs = notation.Select(sym, Descriptor.XMLRoot, 3);
            if (recs.Length > 0)
            {
                string version = "1.0";
                if (recs[0].Arg1 != null)
                    version = ((Literal)recs[0].Arg1).Data;
                string standalone = null;
                if (recs[0].Arg2 != null)
                    standalone = ((Qname)recs[0].Arg2).Name;
                return Lisp.List(ID.XmlValueRoot, version, standalone,
                    Lisp.List(Funcs.LambdaQuote, ProcessValueExpr(notation, recs[0].Arg0)));
            }
            recs = notation.Select(sym, Descriptor.XMLElement, 2);
            if (recs.Length > 0)
            {
                object value = null;
                if (recs[0].args[1] != null)
                {
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                    if (arr.Length > 1)
                    {
                        object[] func = new Object[arr.Length + 1];
                        func[0] = ID.XmlValueConcat;
                        for (int k = 0; k < arr.Length; k++)
                            func[k + 1] = Lisp.List(Funcs.LambdaQuote, ProcessValueExpr(notation, arr[k]));
                        value = Lisp.List(func);
                    }
                    else
                        value = Lisp.List(Funcs.LambdaQuote, ProcessValueExpr(notation, arr[0]));
                }
                object elem = Lisp.List(ID.XmlValueElem, ((Qname)recs[0].Arg0).UnqoutedName, value, XmlValueOption.EmptyOnNull);
                recs = notation.Select(sym, Descriptor.XMLAttributes, 1);
                if (recs.Length > 0)
                {
                    if (recs[0].args[0] == null)
                        elem = Lisp.List(ID.XmlForestAtt, elem, null, XmlValueOption.AbsentOnNull);
                    else
                    {
                        Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                        for (int k = 0; k < arr.Length; k++)
                        {
                            XmlValueOption option = XmlValueOption.AbsentOnNull;
                            Notation.Record[] recs1 = notation.Select(arr[k], Descriptor.ContentOption, 1);
                            if (recs1.Length > 0)
                            {
                                TokenWrapper w = (TokenWrapper)recs1[0].Arg0;
                                switch (w.Data)
                                {
                                    case Token.OPTION_NULL:
                                        option = XmlValueOption.NullOnNull;
                                        break;
                                    case Token.OPTION_EMPTY:
                                        option = XmlValueOption.EmptyOnNull;
                                        break;
                                    case Token.OPTION_ABSENT:
                                        option = XmlValueOption.AbsentOnNull;
                                        break;
                                    case Token.OPTION_NIL:
                                        option = XmlValueOption.NilOnNull;
                                        break;
                                    case Token.NO_CONTENT:
                                        option = XmlValueOption.NilOnNoContent;
                                        break;
                                }
                            }
                            if (arr[k].Tag == Tag.TableFields)
                            {
                                recs1 = notation.Select(arr[k], Descriptor.TableFields, 1);
                                if (recs1.Length > 0)
                                    elem = Lisp.List(ID.XmlForestAtt, elem, ((Qname)recs1[0].Arg0).Name, option);
                            }
                            else
                            {
                                string name;
                                recs1 = notation.Select(arr[k], Descriptor.Alias, 1);
                                if (recs1.Length > 0)
                                    name = ((Qname)recs1[0].Arg0).UnqoutedName;
                                else
                                    if (arr[k].Tag == Tag.Qname)
                                        name = ((Qname)arr[k]).UnqoutedName;
                                    else
                                        throw new ESQLException(Properties.Resources.ParameterMustBeAliased, k + 1, "XMLAttributes");
                                elem = Lisp.List(ID.XmlValueAtt, elem,
                                    name, Lisp.List(Funcs.LambdaQuote, ProcessValueExpr(notation, arr[k])), option);
                            }
                        }
                    }
                }
                recs = notation.Select(sym, Descriptor.XMLNamespaces, 1);
                if (recs.Length > 0)
                    return ProcessXmlNamespaces(notation, recs, elem);
                else
                    return elem;
            }
            throw new InvalidOperationException();
        }

        private object ProcessXmlNamespaces(Notation notation, Notation.Record[] recs, object p)
        {
            Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
            for (int k = 0; k < arr.Length; k++)
                if (arr[k] != null)
                {
                    Notation.Record[] recs1 = notation.Select(arr[k], Descriptor.DeclNamespace, 2);
                    if (recs1.Length > 0)
                        if (recs1[0].Arg1 == null)
                            p = Lisp.List(ID.XmlValueNs, p, ((Literal)recs1[0].Arg0).Data, null);
                        else
                            p = Lisp.List(ID.XmlValueNs, p, ((Literal)recs1[0].Arg0).Data, ((Qname)recs1[0].Arg1).Name);
                    else
                        throw new InvalidOperationException();
                }
            return p;
        }

        private object ProcessNumExpPrimary(Notation notation, Symbol sym)
        {
            if (sym is Value)
            {
                if (sym is Parameter) // We must process all parameters by QueryBinder before translation
                    throw new InvalidOperationException(); 
                else if (sym is Placeholder)
                {
                    Placeholder pl = (Placeholder)sym;
                    return Lisp.List(ID.ParamRef, pl.Index);
                }
                else if (sym is Qname)
                {
                    string prefix = null;
                    Qname qname = (Qname)sym;                    
                    Notation.Record[] recs = notation.Select(qname, Descriptor.Prefix, 1);
                    if (recs.Length > 0)
                    {
                        Literal lit = (Literal)recs[0].Arg0;
                        prefix = lit.Data;
                    }                    
                    return ATOM.Create(prefix, qname.ToArray(), false);
                }
                else
                    return ((Value)sym).Data;
            }
            else
            {
                switch (sym.Tag)
                {
                    case Tag.SQuery:
                            return Lisp.List(ID.SingleColumn, 
                                 Lisp.List(ID.SingleRow, Lisp.List(ID.SQuery, sym, BindSQuery(notation, sym))));

                    case Tag.CaseExpr:
                        return ProcessCaseExpr(notation, sym);

                    case Tag.AggExpr:
                        {
                            foreach (DataAggregator.Column col in aggregators.Peek().Columns)
                                if (col.Expr == sym)
                                    return ATOM.Create(null, new string[] { "$result", col.Name }, false);
                            throw new InvalidOperationException();
                        }

                    case Tag.Dref:
                        return ProcessDrefExp(notation, sym);

                    default:
                        {
                            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { 
                                Descriptor.Branch, Descriptor.NullIf, Descriptor.Coalesce, Descriptor.Funcall, Descriptor.Cast });
                            switch (recs[0].descriptor)
                            {
                                case Descriptor.Branch:
                                        return ProcessValueExpr(notation, recs[0].Arg0);

                                case Descriptor.NullIf:
                                    return Lisp.List(ID.NullIf, ProcessValueExpr(notation, recs[0].Arg0),
                                        ProcessValueExpr(notation, recs[0].Arg1));

                                case Descriptor.Coalesce:
                                    {
                                        Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                                        object[] values = new object[arr.Length +1];
                                        for (int k = 0; k < arr.Length; k++)
                                            values[k + 1] = ProcessValueExpr(notation, arr[k]);
                                        values[0] = ID.Coalesce;
                                        return Lisp.List(values);
                                    }

                                case Descriptor.Funcall:
                                    {
                                        Qname name = (Qname)recs[0].Arg0;
                                        Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                                        object[] values = new object[arr.Length + 1];
                                        values[0] = ATOM.Create(null, new string[] {name.Name.ToLower()}, false);
                                        for (int k = 0; k < arr.Length; k++)
                                            values[k + 1] = ProcessValueExpr(notation, arr[k]);
                                        return Lisp.List(values);
                                    }

                                case Descriptor.Cast:
                                    if (recs[0].Arg1.Tag == Tag.Qname)
                                        throw new NotImplementedException();
                                    else
                                    {
                                        object expr;
                                        if (recs[0].Arg0 == null)
                                            expr = Lisp.List(Funcs.Trap, Lisp.UNKNOWN);
                                        else
                                            expr = ProcessValueExpr(notation, recs[0].Arg0);
                                        TokenWrapper w = (TokenWrapper)recs[0].Arg1;
                                        switch (w.Data)
                                        {
                                            case Token.CHAR:
                                            case Token.CHAR_VARYING:
                                            case Token.CHARACTER:
                                            case Token.CHARACTER_VARYING:
                                            case Token.VARCHAR:
                                                {
                                                    Notation.Record[] recs1 = notation.Select(w, Descriptor.Typelen, 1);
                                                    if (recs1.Length > 0)
                                                        return Lisp.List(ID.ToString, expr, recs1[0].args[0]);
                                                    else
                                                        return Lisp.List(ID.ToString, expr);
                                                }

                                            case Token.SMALLINT:
                                                return Lisp.List(ID.ToInt16, expr);

                                            case Token.INT:
                                            case Token.INTEGER:
                                                return Lisp.List(ID.ToInt32, expr);

                                            case Token.REAL:
                                                return Lisp.List(ID.ToSingle, expr);

                                            case Token.FLOAT:
                                            case Token.DOUBLE_PRECISION:
                                                return Lisp.List(ID.ToDouble, expr);

                                            case Token.DEC:
                                            case Token.DECIMAL:
                                            case Token.NUMERIC:
                                                return Lisp.List(ID.ToDecimal, expr);

                                            case Token.DATE:
                                                return Lisp.List(ID.ToDateTime, expr);

                                            default:
                                                throw new InvalidOperationException();
                                        }
                                    }

                                default:
                                    throw new InvalidOperationException();
                            }
                        }
                }
            }
        }

        private object ProcessDrefExp(Notation notation, Symbol sym)
        {
            object res;
            Notation.Record[] recs = notation.Select(sym, Descriptor.Dref, 2);
            if (recs.Length > 0)
                res = Lisp.List(ID.Dref, ProcessNumExpPrimary(notation, recs[0].Arg0),
                         ProcessNumExpPrimary(notation, recs[0].Arg1));
            else
            {
                recs = notation.Select(sym, Descriptor.At, 2);
                if (recs.Length > 0)
                    res = Lisp.List(ID.At, ProcessNumExpPrimary(notation, recs[0].Arg0),
                           ProcessNumExpPrimary(notation, recs[0].Arg1));
                else
                {
                    recs = notation.Select(sym, Descriptor.Wref, 2);
                    if (recs.Length > 0)
                        res = Lisp.List(ID.Wref, ProcessNumExpPrimary(notation, recs[0].Arg0),
                                 ProcessNumExpPrimary(notation, recs[0].Arg1));
                    else
                        throw new InvalidOperationException();
                }
            }
            if (notation.Flag(sym, Descriptor.ColumnRef))
                return Lisp.List(ID.Rval, res);
            else
                return res;
        }

        private object ProcessCaseExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Case, 2);
            if (recs.Length > 0)
            {  // Simple case
                object value = ProcessValueExpr(notation, recs[0].Arg0);
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                List<object> branch = new List<object>();
                object default_branch = null;
                for (int k = 0; k < arr.Length; k++)
                {
                    recs = notation.Select(arr[k], Descriptor.CaseBranch, 2);
                    if (recs.Length > 0)
                    {
                        branch.Add(Lisp.List(ProcessValueExpr(notation, recs[0].Arg0),
                            ProcessValueExpr(notation, recs[0].Arg1)));
                    }
                    else
                    {
                        recs = notation.Select(arr[k], Descriptor.ElseBranch, 1);
                        if (recs.Length > 0)
                            default_branch = ProcessValueExpr(notation, recs[0].Arg0);
                    }
                }
                return Lisp.List(ID.Case, value, Lisp.List(branch.ToArray()), default_branch);
            }
            else
            {  // Searched case 
                recs = notation.Select(sym, Descriptor.Case, 1);
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                object[] branch = new object[arr.Length];                
                for (int k = 0; k < arr.Length; k++)
                {
                    recs = notation.Select(arr[k], Descriptor.CaseBranch, 2);
                    if (recs.Length > 0)
                    {
                        branch[k] = Lisp.List(ProcessSearchCondition(notation, recs[0].Arg0),
                            ProcessValueExpr(notation, recs[0].Arg1));
                    }
                    else
                    {
                        recs = notation.Select(arr[k], Descriptor.ElseBranch, 1);
                        if (recs.Length > 0) // ((range T) expr)
                            branch[arr.Length -1] = Lisp.List(Lisp.T, 
                                ProcessValueExpr(notation, recs[0].Arg0));
                    }
                }
                return Lisp.Append(Lisp.Cons(Funcs.Cond), Lisp.List(branch));
            }
        }

        private object ProcessSearchCondition(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.LogicalOR, 2);
            if (recs.Length > 0)
                return Lisp.List(Funcs.Or, ProcessSearchCondition(notation, recs[0].Arg0), 
                    ProcessBooleanTerm(notation, recs[0].Arg1));
            else
                return ProcessBooleanTerm(notation, sym);
        }

        private object ProcessBooleanTerm(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.LogicalAND, 2);
            if (recs.Length > 0)
                return Lisp.List(Funcs.And, ProcessBooleanTerm(notation, recs[0].Arg0),
                    ProcessBooleanFactor(notation, recs[0].Arg1));
            else
                return ProcessBooleanFactor(notation, sym);
        }

        private object ProcessBooleanFactor(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Inverse, 1);
            if (recs.Length > 0)
                return Lisp.List(Funcs.Not, ProcessBooleanTest(notation, recs[0].Arg0));
            else
                return ProcessBooleanTest(notation, sym);
        }

        private object ProcessBooleanTest(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.BooleanTest, 2);
            if (recs.Length > 0)
            {
                object expr; 
                TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                switch (w.Data)
                {
                    case Token.TRUE:
                        expr = Lisp.List(ID.IsTrue, 
                            ProcessBooleanPrimary(notation, recs[0].Arg1)); 
                        break;
                    
                    case Token.FALSE:
                        expr = Lisp.List(ID.IsFalse,
                            ProcessBooleanPrimary(notation, recs[0].Arg1));
                        break;
                    
                    case Token.UNKNOWN:
                        expr = Lisp.List(ID.IsUnknown,
                            ProcessBooleanPrimary(notation, recs[0].Arg1));
                        break;

                    default:
                        throw new InvalidOperationException();
                }
                if (notation.Flag(sym, Descriptor.Inverse))
                    return Lisp.List(Funcs.Not, expr);
                else
                    return expr;
            }
            else
                return ProcessBooleanPrimary(notation, sym);
        }

        private object ProcessBooleanPrimary(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Branch, 1);
            if (recs.Length > 0)
                return ProcessSearchCondition(notation, recs[0].Arg0);
            else
                return ProcessPredicate(notation, sym);
        }

        private object ProcessPredicate(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] 
               {Descriptor.Pred, 
                Descriptor.Between, 
                Descriptor.InSet, 
                Descriptor.Like, 
                Descriptor.IsNull, 
                Descriptor.QuantifiedPred, 
                Descriptor.Exists, 
                Descriptor.Unique, 
                Descriptor.Match, 
                Descriptor.Overlaps});

            if (recs.Length > 0)
                switch (recs[0].descriptor)
                {
                    case Descriptor.Pred:
                        {
                            string op = (String)recs[0].args[1];
                            object arg0 = ProcessRowConstructor(notation, recs[0].Arg0);
                            object arg1 = ProcessRowConstructor(notation, recs[0].Arg2);
                            if (op.Equals("="))
                                return Lisp.List(ID.EQ, arg0, arg1);
                            else if (op.Equals("<>"))
                                return Lisp.List(ID.NE, arg0, arg1);
                            else if (op.Equals("<"))
                                return Lisp.List(ID.LT, arg0, arg1);
                            else if (op.Equals(">"))
                                return Lisp.List(ID.GT, arg0, arg1);
                            else if (op.Equals("<="))
                                return Lisp.List(ID.LE, arg0, arg1);
                            else if (op.Equals(">="))
                                return Lisp.List(ID.GE, arg0, arg1);
                            else
                                throw new InvalidOperationException();
                        }

                    case Descriptor.QuantifiedPred:
                        {
                            object x = ATOM.Create(null, new string[] {"x"}, false);
                            string op = (String)recs[0].args[1];                                                        
                            object expr = null;
                            if (op.Equals("="))
                                expr = Lisp.List(ID.EQ, ProcessRowConstructor(notation, recs[0].Arg0), x);
                            else if (op.Equals("<>"))
                                expr = Lisp.List(ID.NE, ProcessRowConstructor(notation, recs[0].Arg0), x);
                            else if (op.Equals("<"))
                                expr = Lisp.List(ID.LT, ProcessRowConstructor(notation, recs[0].Arg0), x);
                            else if (op.Equals(">"))
                                expr = Lisp.List(ID.GT, ProcessRowConstructor(notation, recs[0].Arg0), x);
                            else if (op.Equals("<="))
                                expr = Lisp.List(ID.LE, ProcessRowConstructor(notation, recs[0].Arg0), x);
                            else if (op.Equals(">="))
                                expr = Lisp.List(ID.GE, ProcessRowConstructor(notation, recs[0].Arg0), x);
                            TokenWrapper w = (TokenWrapper)recs[0].Arg2;
                            switch (w.Data)
                            {
                                case Token.ALL:
                                    return Lisp.List(ID.ForAll, Lisp.List(ID.SQuery, recs[0].Arg3, BindSQuery(notation, recs[0].Arg3)), 
                                        Lisp.List(Lisp.QUOTE, x), Lisp.List(Lisp.QUOTE, expr));

                                case Token.SOME:
                                case Token.ANY:
                                    return Lisp.List(ID.ForAny, Lisp.List(ID.SQuery, recs[0].Arg3, BindSQuery(notation, recs[0].Arg3)), 
                                        Lisp.List(Lisp.QUOTE, x), Lisp.List(Lisp.QUOTE, expr));

                                default:
                                    throw new InvalidOperationException();
                            }
                        }

                    case Descriptor.InSet:
                        {
                            object expr = Lisp.List(ID.Member, ProcessRowConstructor(notation, recs[0].Arg0),
                                ProcessInPredicateValue(notation, recs[0].Arg1));
                            if (notation.Flag(sym, Descriptor.Inverse))
                                return Lisp.List(Funcs.Not, expr);
                            else
                                return expr;
                        }

                    case Descriptor.Between:
                        {
                            object arg0 = ProcessRowConstructor(notation, recs[0].Arg0);
                            object arg1 = ProcessRowConstructor(notation, recs[0].Arg1);
                            object arg2 = ProcessRowConstructor(notation, recs[0].Arg2);
                            if (notation.Flag(sym, Descriptor.Inverse))
                                return Lisp.List(Funcs.Not, Lisp.List(ID.Between, arg0, arg1, arg2));
                            else
                                return Lisp.List(ID.Between, arg0, arg1, arg2);
                        }

                    case Descriptor.Like:
                        {
                            object escape = null;
                            Notation.Record[] recs1 =
                                notation.Select(sym, Descriptor.Escape, 1);
                            if (recs1.Length > 0)
                                escape = ProcessValueExpr(notation, recs1[0].Arg0);
                            object pattern = ProcessValueExpr(notation, recs[0].Arg1);
                            object expr;
                            if (pattern is String && (escape == null || escape is String))
                            {
                                char escape_char = '\0';
                                if (escape != null)
                                {
                                    string escape_str = (string)escape;
                                    if (escape_str.Length > 1)
                                        throw new ESQLException(Properties.Resources.EscapeCharToLong);
                                    escape_char = escape_str[0];
                                }

                                expr = Lisp.List(ID.Like1, ProcessValueExpr(notation, recs[0].Arg0),
                                    new LikeOperator((String)pattern, escape_char, false, System.Globalization.CultureInfo.CurrentCulture));
                            }
                            else    
                               expr = Lisp.List(ID.Like2,  ProcessValueExpr(notation, recs[0].Arg0), pattern, escape);
                            if (notation.Flag(sym, Descriptor.Inverse))
                                return Lisp.List(Funcs.Not, expr);
                            else
                                return expr;
                        }

                    case Descriptor.IsNull:
                        {
                            object arg = ProcessRowConstructor(notation, recs[0].Arg0);
                            if (notation.Flag(sym, Descriptor.Inverse))
                                return Lisp.List(Funcs.Not, Lisp.List(ID.IsNull, arg));
                            else
                                return Lisp.List(ID.IsNull, arg);
                        }

                    case Descriptor.Exists:
                        return Lisp.List(ID.Exist, Lisp.List(ID.SQuery, recs[0].Arg0,
                            BindSQuery(notation, recs[0].Arg0)));

                    default:
                        throw new NotImplementedException();
                }
            else
                throw new InvalidOperationException();
        }

        private object ProcessInPredicateValue(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.ValueList, 1);
            if (recs.Length > 0)
            {                
                Value[] arr = Lisp.ToArray<Value>(recs[0].args[0]);
                object[] values = new object[arr.Length + 1];                
                for (int k = 0; k < arr.Length; k++)
                    values[k + 1] = ProcessValueExpr(notation, arr[k]);
                values[0] = Funcs.List;                
                return Lisp.List(values);  
            }
            else
                return Lisp.List(ID.SQuery, sym, BindSQuery(notation, sym));  
        }

        private object ProcessRowConstructor(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.RowValue, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                object[] value = new object[arr.Length + 1];
                value[0] = Funcs.List;
                for (int i = 0; i < arr.Length; i++)
                    value[i + 1] = ProcessRowConstructorElem(notation, arr[i]);
                return Lisp.List(value);
            }
            else
            {
                object expr = ProcessRowConstructorElem(notation, sym);
                if (Lisp.IsFunctor(expr, ID.SingleColumn))
                    return Lisp.Second(expr);
                else
                    return expr;
            }
        }

        private object ProcessRowConstructorElem(Notation notation, Symbol sym)
        {
            if (sym is TokenWrapper)
            {
                TokenWrapper w = (TokenWrapper)sym;
                switch (w.Data)
                {
                    case Token.NULL:
                        return Lisp.List(Funcs.Trap, Lisp.UNKNOWN);

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                object expr = ProcessValueExpr(notation, sym);
                return expr;
            }
        }

        private bool IsProviderSubquery(Notation notation, Symbol sym, out DataSourceInfo dsi)
        {
            dsi = null;
            Notation.Record[] recs = notation.Select(sym, Descriptor.HintServerSubquery, 1);
            if (recs.Length > 0)
            {
                dsi = dictionary.GetDataSource((string)recs[0].args[0]);
                if (dsi.TableAccessor == AcessorType.DataProvider)
                {
                    DataProviderHelper helper = new DataProviderHelper(dsi.ProviderInvariantName, dsi.ConnectionString);
                    if (helper.Smart)
                        return true;
                }
            }            
            return false;
        }

        private object BindSQuery(Notation notation, Symbol qexpr)
        {
            Notation.Record[] recs = notation.Select(qexpr, Descriptor.Binding, 1);
            if (recs.Length > 0)
            {                
                Symbol[] bindings = Lisp.ToArray<Symbol>(recs[0].args[0]);
                Object[] expr = new object[bindings.Length + 1];
                expr[0] = Funcs.List;
                for (int k = 0; k < bindings.Length; k++)
                {
                    recs = notation.Select(bindings[k], Descriptor.Link, 1);
                    if (recs.Length > 0)
                        expr[k + 1] = ProcessValueExpr(notation, recs[0].Arg0);
                }
                return Lisp.List(expr);
            }
            else
                return null;
        }

        private bool ContainsDynatable(QueryNode node)
        {
            if (node is DynatableAccessor)
                return true;
            else
                if (node.ChildNodes != null)
                {
                    foreach (QueryNode child in node.ChildNodes)
                        if (ContainsDynatable(child))
                            return true;
                }
            return false;
        }

        
        #region Analyzers

        private class SubqueryAnalyzer : QueryWalker
        {
            private List<Symbol> queries = new List<Symbol>();

            public SubqueryAnalyzer(Notation notation)
                : base(notation)
            {
            }

            public override void WalkSubQuery(Symbol squery)
            {
                queries.Add(squery);
                return; // We not scan deeper
            }

            public Symbol[] Queries
            {
                get
                {
                    return queries.ToArray();
                }
            }
        }

        private class AggregationAnalyzer : QueryWalker
        {
            private List<Symbol> groupFuncList = new List<Symbol>();
            private List<Qname> groupByList = new List<Qname>();
            
            public AggregationAnalyzer(Notation notation)
                : base(notation)
            {
            }

            public override void WalkSubQuery(Symbol squery)
            {
                return;
            }

            public override void WalkSetFuncSpec(Symbol sym)
            {
                base.WalkSetFuncSpec(sym);
                groupFuncList.Add(sym);
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

            public bool hasAggregation
            {
                get
                {
                    return groupFuncList.Count > 0;
                }
            }
        }

        #endregion
    }
}
