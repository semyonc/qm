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
using System.IO;
using System.Globalization;

using DataEngine.CoreServices;
using DataEngine.Parser;

namespace DataEngine
{
    public class SqlWriter
    {        
        private Notation notation;
        
        public DataProviderHelper ProviderHelper { get; set; }
        public bool ShowHints { get; set; }
        public bool ExtendedSyntax { get; set; }

        public SqlWriter(Notation notation)
        {
            ProviderHelper = new DataProviderHelper();
            this.notation = notation;
            sb = new StringBuilder();
            newLineFlag = false;
        }

        public override string ToString()
        {            
            return sb.ToString();            
        }

        public void Write()
        {
            Notation.Record[] recs = notation.Select(Descriptor.Root, 1);
            WriteStmt(recs[0].sym);
        }

        public virtual void WriteStmt(Symbol stmt)
        {
            Notation.Record[] recs = notation.Select(stmt, Descriptor.Root, 1);
            if (ShowHints)
                WriteOptimizerHint(stmt);
            WriteQueryExp(recs[0].Arg0);
            WriteOrderByClause(stmt);
        }

        protected virtual void WriteOrderByClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Order, 1);
            if (recs.Length > 0)
            {
                SmartNewLine();
                WriteText("ORDER BY ");
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteSortKey((Value)arr[k]);
                    if (notation.Flag(arr[k], Descriptor.Desc))
                        WriteText(" DESC");
                }
            }
        }

        protected virtual void WriteOptimizerHint(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.OptimizerHint, 1);
            if (recs.Length > 0)
            {
                String[] arr = Lisp.ToArray<String>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                {
                    WriteText("//!");
                    WriteText(arr[k]);
                    SmartNewLine();
                }
            }
            recs = notation.Select(sym, Descriptor.HintNamespace, 2);
            if (recs.Length > 0)
            {
                WriteText("/*+ ");
                for (int k = 0; k < recs.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteTextFormat("namespace({0}:{1})", 
                        recs[k].args[0], recs[k].args[1]);
                }
                WriteText("*/");
                SmartNewLine();
            }
            recs = notation.Select(sym, Descriptor.HintColumn, 3);
            if (recs.Length > 0)
            {
                WriteText("/*+ ");
                for (int k = 0; k < recs.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteTextFormat("column({0},{1},{2})",
                        recs[k].args[0], recs[k].args[1], recs[k].args[2]);
                }
                WriteText("*/");
                SmartNewLine();
            }
#if DEBUG
            recs = notation.Select(sym, Descriptor.HintServerSubquery, 1);
            if (recs.Length > 0)
            {
                WriteText("/*+ server(");
                if (recs[0].args[0] == null)
                    WriteText("null");
                else
                    WriteText((string)recs[0].args[0]);
                WriteText(")*/");
            }
#endif
        }

        public virtual void WriteQueryExp(Symbol qexpr)
        {
#if DEBUG
            if (ShowHints)
            {
                Notation.Record[] recsd = notation.Select(qexpr, Descriptor.Binding, 1);
                if (recsd.Length > 0)
                {
                    WriteText("/*+ bind(");
                    Symbol[] bindings = Lisp.ToArray<Symbol>(recsd[0].args[0]);
                    for (int k = 0; k < bindings.Length; k++)
                    {
                        if (k > 0)
                            WriteText(", ");
                        recsd = notation.Select(bindings[k], Descriptor.Link, 1);
                        if (recsd.Length > 0)
                            WriteNumExpPrimary(recsd[0].Arg0);
                    }
                    WriteText(")*/");
                    SmartNewLine();
                }
            }
#endif
            Notation.Record[] recs = notation.Select(qexpr, 
                new Descriptor[] { Descriptor.Union, Descriptor.Except }, 2);
            if (recs.Length > 0)
            {
                WriteQueryExp(recs[0].Arg0);
                switch (recs[0].descriptor)
                {
                    case Descriptor.Union:
                        WriteText("UNION");
                        break;
                    case Descriptor.Except:
                        WriteText("EXCEPT");
                        break;
                }
                if (notation.Flag(qexpr, Descriptor.Distinct))
                    WriteText(" ALL");
                SmartNewLine();
                WriteQueryTerm(recs[0].Arg1);
            }
            else
                WriteQueryTerm(qexpr);
        }

        protected virtual void WriteQueryTerm(Symbol qterm)
        {
            Notation.Record[] recs = notation.Select(qterm, Descriptor.Intersect, 2);
            if (recs.Length > 0)
            {
                WriteQueryTerm(recs[0].Arg0);
                WriteText("INTERSECT");
                if (notation.Flag(qterm, Descriptor.Distinct))
                    WriteText(" ALL");
                SmartNewLine();
                WriteQueryTerm(recs[0].Arg1);
            }
            else
                WriteSimpleTable(qterm);
        }

        protected virtual void WriteSimpleTable(Symbol sym)
        {
            switch (sym.Tag)
            {
                case Tag.SQuery:
                    WriteQuerySpec(sym);
                    break;

                case Tag.TableConstructor:
                    WriteTableConstructor(sym);
                    break;

                case Tag.ExplictTable:
                    WriteExplictTable(sym);
                    break;
            }                
        }

        protected virtual void WriteExplictTable(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Explicit, 1);
            if (recs.Length > 0)
            {
                WriteText("TABLE ");
                WriteQualifiedName((Qname)recs[0].Arg0);
                SmartNewLine();
            }
        }

        protected virtual void WriteTableConstructor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.TableValue, 1);
            if (recs.Length > 0)
            {
                WriteText("VALUES ");
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i > 0)
                    {
                        WriteText(",");
                        SmartNewLine();
                    }
                    WriteRowConstructor((Symbol)arr[i]);
                }
                SmartNewLine();
            }
        }

        protected virtual void WriteQuerySpec(Symbol sym)
        {            
            WriteText("SELECT ");
            if (ExtendedSyntax)
            {
                Notation.Record[] recs = notation.Select(sym, Descriptor.Top, 1);
                if (recs.Length > 0)
                {
                    WriteText("TOP(");
                    WriteText(recs[0].args[0].ToString());
                    WriteText(") ");
                }
            }
            if (notation.Flag(sym, Descriptor.Distinct))
                WriteText("DISTINCT ");
            WriteSelectList(sym);
            WriteFromClause(sym);
            WriteWhereClause(sym);
            WriteGroupByClause(sym);
            WriteHavingClause(sym);            
        }

        protected virtual void WriteFromClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.From, 1);
            if (recs.Length > 0)
            {
                WriteText(" FROM ");
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteTableRef(arr[k]);
#if DEBUG
                    if (ShowHints)
                    {
                        Notation.Record[] recsd = notation.Select(arr[k], Descriptor.HintKeyPair, 2);
                        if (recsd.Length > 0)
                        {
                            WriteText(" /*+ ");
                            string[] k1 = (string[])recsd[0].args[0];
                            string[] k2 = (string[])recsd[0].args[1];
                            WriteText("nested_loops");
                            WriteText("(");
                            for (int p = 0; p < k1.Length; p++)
                            {
                                if (p > 0)
                                    WriteText(", ");
                                WriteTextFormat("{0}={1}", k1[p], k2[p]);
                            }
                            WriteText(")*/");
                        }
                    }
#endif
                }                
            }
            SmartNewLine();
        }

        protected virtual void WriteTableRef(Symbol sym)
        {
            if (sym.Tag == Tag.Join)
                WriteJoinedTable(sym);
            else 
                WriteTableRefSimple(sym);
        }

        protected virtual void WriteTableRefSimple(Symbol sym)
        {
#if DEBUG
            if (ShowHints)
            {
                Notation.Record[] recsd = notation.Select(sym, Descriptor.HintSort, 1);
                if (recsd.Length > 0)
                {
                    WriteText("/*+ sort(");
                    string[] columns = Lisp.ToArray<String>(recsd[0].args[0]);
                    for (int k = 0; k < recsd.Length; k++)
                    {
                        if (k > 0)
                            WriteText(", ");
                        WriteText(columns[k]);
                    }
                    WriteText(")*/");
                }
            }
#endif
            if (sym is Qname)
            {
                WriteQualifiedName((Qname)sym);
#if DEBUG
                if (ShowHints)
                {
                    Notation.Record[] recsd = notation.Select(sym, Descriptor.HintFilter, 1);
                    if (recsd.Length > 0)
                    {
                        WriteText("/*+ filter(");
                        WriteSearchCondition(recsd[0].Arg0);
                        WriteText(")*/");
                    }
                }
#endif
            }
            else
            {
                Notation.Record[] recs1 = notation.Select(sym, new Descriptor[] { Descriptor.Dynatable, Descriptor.Tuple },  1);
                if (recs1.Length > 0)
                {
                    switch (recs1[0].descriptor)
                    {
                        case Descriptor.Dynatable:
                            WriteText("TABLE ");
                            break;

                        case Descriptor.Tuple:
                            WriteText("TUPLE ");
                            break;
                    }                    
                    WriteValueExp(recs1[0].Arg0);
                }
                else
                    WriteSubQuery(sym);
            }
            Notation.Record[] recs = notation.Select(sym, Descriptor.Alias, 1);
            if (recs.Length > 0)
            {
                WriteText(" AS ");
                WriteQualifiedName((Qname)recs[0].Arg0);
            }
        }

        protected virtual void WriteJoinedTable(Symbol sym)
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
                    WriteTableRefSimple(recs[0].Arg0);
                    WriteText(" CROSS JOIN ");
                    WriteTableRef(recs[0].Arg1);
                    break;

                case Descriptor.UnionJoin:
                    WriteTableRefSimple(recs[0].Arg0);
                    WriteText(" UNION JOIN ");
                    WriteTableRef(recs[0].Arg1);
                    break;

                case Descriptor.NaturalJoin:
                    WriteTableRefSimple(recs[0].Arg0);
                    WriteText(" NATURAL");
                    WriteJoinType(sym);
                    WriteText(" JOIN ");
                    WriteTableRef(recs[0].Arg1);
                    break;

                case Descriptor.QualifiedJoin:
                    WriteTableRefSimple(recs[0].Arg0);
                    WriteJoinType(sym);
                    WriteText(" JOIN ");
                    WriteTableRef(recs[0].Arg1);
                    WriteJoinSpec(sym);
                    break;

                case Descriptor.Branch:
                    WriteText("(");
                    WriteTableRef(recs[0].Arg0);
                    WriteText(")");
                    break;
            }        
        }

        protected virtual void WriteJoinSpec(Symbol sym)
        {
#if DEBUG
            if (ShowHints)
            {
                Notation.Record[] recsd = notation.Select(sym, Descriptor.HintKeyPair, 2);
                if (recsd.Length > 0)
                {
                    WriteText(" /*+ ");
                    string[] k1 = (string[])recsd[0].args[0];
                    string[] k2 = (string[])recsd[0].args[1];
                    recsd = notation.Select(sym, Descriptor.HintJoin, 1);
                    if (recsd.Length > 0)
                    {
                        JoinMethod method = (JoinMethod)recsd[0].args[0];
                        switch (method)
                        {
                            case JoinMethod.NestedLoops:
                                WriteText("nested_loops");
                                break;
                            case JoinMethod.MergeJoin:
                                WriteText("merge");
                                break;
                            case JoinMethod.HashJoin:
                                WriteText("hash_join");
                                break;
                            case JoinMethod.SemiJoin:
                                WriteText("semi_join");
                                break;
                        }
                    }
                    else
                        WriteText("nested_loops");
                    WriteText("(");
                    for (int p = 0; p < k1.Length; p++)
                    {
                        if (p > 0)
                            WriteText(", ");
                        WriteTextFormat("{0}={1}", k1[p], k2[p]);
                    }
                    WriteText(")*/ ");
                }
            }
#endif
            Notation.Record[] recs = notation.Select(sym, Descriptor.JoinSpec, 1);
            if (recs.Length > 0)
            {
                Notation.Record[] recs1 = notation.Select(recs[0].Arg0, Descriptor.Using, 1);
                if (recs1.Length > 0)
                {
                    WriteText(" USING (");
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs1[0].args[0]);
                    for (int k = 0; k < arr.Length; k++)
                    {
                        if (k > 0)
                            WriteText(", ");
                        WriteQualifiedName((Qname)arr[k]);
                    }
                    WriteText(")");
                    SmartNewLine();
                }
                else
                {
                    recs1 = notation.Select(recs[0].Arg0, Descriptor.Constraint, 1);
                    if (recs1.Length > 0)
                    {
                        WriteText(" USING ");
                        if (recs1[0].Arg0.Tag == Tag.Qname)
                        {
                            WriteText("CONSTRAINT ");
                            WriteQualifiedName((Qname)recs1[0].Arg0);
                        }
                        else
                        {
                            TokenWrapper w = (TokenWrapper)recs1[0].Arg0;
                            switch (w.Data)
                            {
                                case Token.PRIMARY:
                                    WriteText("PRIMARY KEY");
                                    break;

                                case Token.FOREIGN:
                                    WriteText("FOREIGN KEY");
                                    break;
                            }
                        }
                        SmartNewLine();
                    }
                    else
                    {
                        WriteText(" ON ");
                        WriteSearchCondition(recs[0].Arg0);
                    }
                }
            }
        }

        protected virtual void WriteJoinType(Symbol sym)
        {
            Notation.Record[] recs0 = notation.Select(sym, Descriptor.JoinType, 1);
            if (recs0.Length > 0)
            {
                TokenWrapper w = (TokenWrapper)recs0[0].args[0];
                switch (w.Data)
                {
                    case Token.INNER:
                        WriteText(" INNER");
                        break;
                    case Token.LEFT:
                        WriteText(" LEFT");
                        break;
                    case Token.RIGHT:
                        WriteText(" RIGHT");
                        break;
                    case Token.FULL:
                        WriteText(" FULL");
                        break;
                }
            }
            if (notation.Flag(sym, Descriptor.Outer))
                WriteText(" OUTER");
        }

        protected virtual void WriteWhereClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Where, 1);
            if (recs.Length > 0)
            {
                SmartNewLine();
                WriteText("WHERE ");
                WriteSearchCondition(recs[0].Arg0);
                SmartNewLine();
            }
        }

        protected virtual void WriteGroupByClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.GroupBy, 1);
            if (recs.Length > 0)
            {
                WriteText("GROUP BY ");
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteQualifiedName((Qname)arr[k]);
                }
                SmartNewLine();
            }
        }

        protected virtual void WriteHavingClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Having, 1);
            if (recs.Length > 0)
            {
                WriteText("HAVING ");
                WriteSearchCondition(recs[0].Arg0);
                SmartNewLine();
            }
        }

        protected virtual void WriteSelectList(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Select });
            if (recs.Length > 0)
                if (recs[0].args.Length == 0)
                    WriteText("*");
                else
                {
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                    for (int k = 0; k < arr.Length; k++)
                    {
                        if (k > 0)
                            WriteText(", ");
                        WriteSelectSubList(arr[k]);
                    }
                }
            SmartNewLine();
        }

        protected virtual void WriteSelectSubList(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.TableFields, 1);
            if (recs.Length > 0)
            {
                WriteQualifiedName((Qname)recs[0].Arg0);
                WriteText(".*");
            }
            else
            {
                if (sym.Tag == Tag.SQuery && notation.Flag(sym, Descriptor.Dynatable))
                {
                    WriteText("TABLE ");
                    WriteSubQuery(sym);
                }
                else
                    WriteValueExp(sym);
                recs = notation.Select(sym, Descriptor.Alias, 1);
                if (recs.Length > 0)
                {
                    WriteText(" AS ");
                    WriteQualifiedName((Qname)recs[0].Arg0);
                }
            }
        }

        protected virtual void WriteRowConstructor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.RowValue, 1);
            if (recs.Length > 0)
            {
                WriteText("(");
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i > 0)
                        WriteText(", ");
                    WriteRowConstructorElem(arr[i]);
                }
                WriteText(")");
            }
            else
                WriteRowConstructorElem(sym);
        }

        protected virtual void WriteRowConstructorElem(Symbol sym)
        {
            if (sym is TokenWrapper)
            {
                TokenWrapper w = (TokenWrapper)sym;
                switch (w.Data)
                {
                    case Token.NULL:
                        WriteText("NULL");
                        break;
                    case Token.DEFAULT:
                        WriteText("DEFAULT");
                        break;
                }
            }
            else
                WriteValueExp(sym);
        }

        public virtual void WriteValueExp(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Concat, 2);
            if (recs.Length > 0)
            {
                WriteValueExp(recs[0].Arg0);
                WriteText("||");
                WriteCharFactor(recs[0].Arg1);
            }
            else
                WriteCharFactor(sym);
        }

        protected virtual void WriteCharFactor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Substring, Descriptor.StringUpper, 
                Descriptor.StringLower, Descriptor.StringConvert, Descriptor.StringTrim });
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.Substring:
                        WriteText("SUBSTRING (");
                        WriteValueExp(recs[0].Arg0);
                        WriteText(" FROM ");
                        WriteNumValueExp(recs[0].Arg1);  
                        if (recs[0].args.Length == 3)
                        {
                            WriteText(" FOR ");
                            WriteNumValueExp(recs[0].Arg2);  
                        }
                        WriteText(")");
                        break;
                    
                    case Descriptor.StringUpper:
                        WriteText("UPPER (");
                        WriteValueExp(recs[0].Arg0);
                        WriteText(")");
                        break;
                    
                    case Descriptor.StringLower:
                        WriteText("LOWER (");
                        WriteValueExp(recs[0].Arg0);
                        WriteText(")");
                        break;
                    
                    case Descriptor.StringConvert:
                        WriteText("CONVERT (");
                        WriteValueExp(recs[0].Arg0);
                        WriteText(" USING ");
                        WriteQualifiedName((Qname)recs[0].Arg1);
                        WriteText(")");
                        break;

                    case Descriptor.StringTrim:
                        TokenWrapper w = (TokenWrapper)recs[0].args[0];
                        Literal lit = null;
                        if (recs[0].args[1] is Literal)
                            lit = (Literal)recs[0].args[1];
                        WriteText("TRIM (");
                        if (w.Data == Token.BOTH && (lit != null && lit.Data.Equals(" ")))
                            WriteValueExp(recs[0].Arg2);
                        else
                        {
                            switch (w.Data)
                            {
                                case Token.TRAILING:
                                    WriteText("TRAILING ");
                                    break;
                                case Token.LEADING:
                                    WriteText("LEADING ");
                                    break;
                            }
                            if (lit == null || !lit.Data.Equals(" "))
                                WriteValueExp(recs[0].Arg1);
                            WriteText(" FROM ");
                            WriteValueExp(recs[0].Arg2);                            
                        }
                        WriteText(")");
                        break;
                }
            }
            else
                WriteNumValueExp(sym); 
        }

        protected virtual void WriteNumValueExp(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym,
                new Descriptor[] { Descriptor.Add, Descriptor.Sub }, 2);
            if (recs.Length > 0)
            {
                WriteNumValueExp(recs[0].Arg0);
                switch (recs[0].descriptor)
                {
                    case Descriptor.Add:
                        WriteText("+");
                        break;
                    case Descriptor.Sub:
                        WriteText("-");
                        break;
                }
                WriteTerm(recs[0].Arg1);
            }
            else
                WriteTerm(sym);
        }

        protected virtual void WriteTerm(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym,
                new Descriptor[] { Descriptor.Mul, Descriptor.Div }, 2);
            if (recs.Length > 0)
            {
                WriteTerm(recs[0].Arg0);
                switch (recs[0].descriptor)
                {
                    case Descriptor.Mul:
                        WriteText("*");
                        break;
                    case Descriptor.Div:
                        WriteText("/");
                        break;
                }
                WriteFactor(recs[0].Arg1);
            }
            else
                WriteFactor(sym);
        }

        protected virtual void WriteFactor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.UnaryMinus, 1);
            if (recs.Length > 0)
            {
                WriteText("-");
                WriteNumPrimary(recs[0].Arg0);
            }
            else
                WriteNumPrimary(sym);
        }

        protected virtual void WriteNumPrimary(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.PosString, 2);
            if (recs.Length > 0)
            {
                WriteText("POSITION (");
                WriteValueExp(recs[0].Arg1);
                WriteText(" IN ");
                WriteValueExp(recs[0].Arg0);
                WriteText(")");
            }
            else
                if (sym.Tag == Tag.SQLX)
                    WriteXmlValueFunc(sym);
                else
                    WriteNumExpPrimary(sym);
        }

        protected virtual void WriteXmlValueFunc(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.XMLConcat, 1);
            if (recs.Length > 0)
            {
                WriteText("XMLCONCAT (");
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteValueExp(arr[k]);
                }
                WriteText(")");
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLQuery, 3);
            if (recs.Length > 0)
            {
                WriteText("XMLQUERY (");
                WriteText("'");
                WriteText((String)recs[0].args[0]);
                WriteText("'");
                if (recs[0].args[1] != null)
                {
                    WriteText(" PASSING BY VALUE ");
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                    for (int k = 0; k < arr.Length; k++)
                    {
                        if (k > 0)
                            WriteText(", ");
                        if (arr[k].Tag == Tag.SQuery && 
                            notation.Flag(arr[k], Descriptor.Dynatable))
                            WriteSubQuery(arr[k]);
                        else
                            WriteValueExp(arr[k]);
                        Notation.Record[] recs1 = notation.Select(arr[k], Descriptor.Alias, 1);
                        if (recs1.Length > 0)
                        {
                            WriteText(" AS ");
                            WriteQualifiedName((Qname)recs1[0].Arg0);
                        }
                    }
                }
                WriteText(")");
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLForestAll, 0);
            if (recs.Length > 0)
            {
                WriteText("XMLFOREST (*)");
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLForest, 1);
            if (recs.Length > 0)
            {
                WriteText("XMLFOREST (");
                Notation.Record[] recs1 = notation.Select(sym, Descriptor.XMLNamespaces, 1);
                if (recs1.Length > 0)
                {
                    WriteXmlNamespaces(recs1);
                    WriteText(", ");
                }
                WriteContentValueList(recs[0].args[0]);
                WriteText(")");
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLParse, 3);
            if (recs.Length > 0)
            {
                WriteText("XMLPARSE (");
                WriteText(((Literal)recs[0].Arg0).Data);
                WriteText(" ");
                WriteValueExp(recs[0].Arg1);
                TokenWrapper w = (TokenWrapper)recs[0].Arg2;
                if (w != null)
                    switch (w.Data)
                    {
                        case Token.PRESERVE_WHITESPACE:
                            WriteText(" PRESERVE WHITESPACE");
                            break;

                        case Token.STRIP_WHITESPACE:
                            WriteText(" STRIP WHITESPACE");
                            break;
                    }
                WriteText(")");
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLPI, 2);
            if (recs.Length > 0)
            {
                WriteText("XMLPI (NAME ");
                WriteQualifiedName((Qname)recs[0].Arg0);
                if (recs[0].Arg1 != null)
                {
                    WriteText(", ");
                    WriteValueExp(recs[0].Arg1);
                }
                WriteText(")");
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLComment, 1);
            if (recs.Length > 0)
            {
                WriteText("XMLCOMMENT (");
                WriteValueExp(recs[0].Arg0);
                WriteText(")");
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLCDATA, 1);
            if (recs.Length > 0)
            {
                WriteText("XMLCDATA (");
                WriteValueExp(recs[0].Arg0);
                WriteText(")");
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLRoot, 3);
            if (recs.Length > 0)
            {
                WriteText("XMLROOT (");
                WriteValueExp(recs[0].Arg0);
                if (recs[0].Arg1 != null)
                {
                    WriteText(" VERSION ");
                    WriteUnsignedLit((Value)recs[0].Arg1);
                }
                if (recs[0].Arg2 != null)
                {
                    WriteText(" STANDALONE ");
                    WriteQualifiedName((Qname)recs[0].Arg2);
                }
                WriteText(")");
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLElement, 2);
            if (recs.Length > 0)
            {
                WriteText("XMLELEMENT (");
                WriteQualifiedName((Qname)recs[0].Arg0);
                Notation.Record[] recs1 = notation.Select(sym, Descriptor.XMLNamespaces, 1);
                if (recs1.Length > 0)
                {
                    WriteText(", ");
                    WriteXmlNamespaces(recs1);                    
                }
                recs1 = notation.Select(sym, Descriptor.XMLAttributes, 1);
                if (recs1.Length > 0)
                {
                    WriteText(", XMLATTRIBUTES (");
                    if (recs1[0].args[0] == null)
                        WriteText("*");
                    else
                        WriteContentValueList(recs1[0].args[0]);
                    WriteText(")");
                }
                if (recs[0].args[1] != null)
                {
                    WriteText(", ");
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                    for (int k = 0; k < arr.Length; k++)
                    {
                        if (k > 0)
                            WriteText(", ");
                        WriteValueExp(arr[k]);
                    }
                }
                WriteText(")");
                return;
            }
        }

        protected virtual void WriteContentValueList(object arg)
        {
            Notation.Record[] recs;
            Symbol[] arr = Lisp.ToArray<Symbol>(arg);
            for (int k = 0; k < arr.Length; k++)
            {
                if (k > 0)
                    WriteText(", ");
                if (arr[k].Tag == Tag.TableFields)
                {
                    recs = notation.Select(arr[k], Descriptor.TableFields, 1);
                    if (recs.Length > 0)
                    {
                        WriteQualifiedName((Qname)recs[0].Arg0);
                        WriteText(".*");
                    }
                }
                else
                    WriteValueExp(arr[k]);
                recs = notation.Select(arr[k], Descriptor.Alias, 1);
                if (recs.Length > 0)
                {
                    WriteText(" AS ");
                    WriteQualifiedName((Qname)recs[0].Arg0);
                }
                recs = notation.Select(arr[k], Descriptor.ContentOption, 1);
                if (recs.Length > 0)
                {
                    WriteText(" ");
                    TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                    switch (w.Data)
                    {
                        case Token.OPTION_NULL:
                            WriteText("OPTION NULL ON NULL");
                            break;
                        case Token.OPTION_EMPTY:
                            WriteText("OPTION EMPTY ON NULL");
                            break;
                        case Token.OPTION_ABSENT:
                            WriteText("OPTION ABSENT ON NULL");
                            break;
                        case Token.OPTION_NIL:
                            WriteText("OPTION NIL ON NULL");
                            break;
                        case Token.NO_CONTENT:
                            WriteText("OPTION NIL ON NO CONTENT");
                            break;
                    }
                }
            }
        }

        protected virtual void WriteXmlNamespaces(Notation.Record[] recs)
        {
            WriteText("XMLNAMESPACES (");
            Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
            for (int k = 0; k < arr.Length; k++)
            {
                if (k > 0)
                    WriteText(", ");
                if (arr[k] == null)
                    WriteText("NO DEFAULT");
                else
                {
                    Notation.Record[] recs1 = notation.Select(arr[k], Descriptor.DeclNamespace, 2);
                    if (recs1.Length > 0)
                        if (recs1[0].Arg1 == null)
                        {
                            WriteText("DEFAULT ");
                            WriteUnsignedLit((Value)recs1[0].Arg0);
                        }
                        else
                        {
                            WriteUnsignedLit((Value)recs1[0].Arg0);
                            WriteText(" AS ");
                            WriteQualifiedName((Qname)recs1[0].Arg1);
                        }
                    else
                        throw new InvalidOperationException();
                }
            }
            WriteText(")");
        }

        protected virtual void WriteNumExpPrimary(Symbol sym)
        {
            if (sym is Value)
            {
                if (sym is Parameter)
                    WriteParameter((Parameter)sym);
                else if (sym is Placeholder)
                    WritePlaceholder((Placeholder)sym);
                else if (sym is Qname)
                    WriteQualifiedName((Qname)sym);
                else
                    WriteUnsignedLit((Value)sym);
            }
            else
            {
                switch (sym.Tag)
                {
                    case Tag.SQuery:
                        WriteSubQuery(sym);
                        break;

                    case Tag.CaseExpr:
                        WriteCaseExp(sym);
                        break;

                    case Tag.AggExpr:
                        WriteSetFuncSpec(sym);
                        break;

                    case Tag.Dref:
                        WriteDrefExp(sym);
                        break;

                    default:
                        Notation.Record[] recs = notation.Select(sym, new Descriptor[] { 
                            Descriptor.Branch, Descriptor.NullIf, Descriptor.Coalesce, Descriptor.Funcall, Descriptor.Cast });
                        switch (recs[0].descriptor)
                        {
                            case Descriptor.Branch:
                                WriteText("(");
                                WriteValueExp(recs[0].Arg0);
                                WriteText(")");
                                break;

                            case Descriptor.NullIf:
                                WriteText("NULLIF (");
                                WriteValueExp(recs[0].Arg0);
                                WriteText(", ");
                                WriteValueExp(recs[0].Arg1);
                                WriteText(")");
                                break;

                            case Descriptor.Coalesce:
                                {
                                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                                    WriteText("COALESCE (");
                                    for (int k = 0; k < arr.Length; k++)
                                    {
                                        if (k > 0)
                                            WriteText(", ");
                                        WriteValueExp(arr[k]);
                                    }
                                    WriteText(")");
                                }
                                break;

                            case Descriptor.Funcall:
                                {
                                    Qname name = (Qname)recs[0].Arg0;
                                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                                    WriteTextFormat("{0}(", name.ToString().ToLowerInvariant());
                                    for (int k = 0; k < arr.Length; k++)
                                    {
                                        if (k > 0)
                                            WriteText(", ");
                                        WriteRowConstructorElem(arr[k]);
                                    }
                                    WriteText(")");
                                }
                                break;

                            case Descriptor.Cast:
                                {
                                    WriteText("CAST (");
                                    if (recs[0].Arg0 == null)
                                        WriteText("NULL");
                                    else
                                        WriteValueExp(recs[0].Arg0);
                                    WriteText(" AS ");
                                    if (recs[0].Arg1.Tag == Tag.Qname)
                                        WriteQualifiedName((Qname)recs[0].Arg1);
                                    else
                                    {
                                        TokenWrapper w = (TokenWrapper)recs[0].Arg1;
                                        switch (w.Data)
                                        {
                                            case Token.DOUBLE_PRECISION:
                                                WriteText("DOUBLE PRECISION");
                                                break;

                                            case Token.CHAR_VARYING:
                                                WriteText("CHAR VARYING");
                                                break;

                                            case Token.CHARACTER_VARYING:
                                                WriteText("CHARACTER VARYING");
                                                break;

                                            default:
                                                WriteText(YYParser.yyname(w.Data));
                                                break;
                                        }                                        
                                        if (w.Data == Token.CHAR || w.Data == Token.CHAR_VARYING ||
                                            w.Data == Token.CHARACTER || w.Data == Token.CHARACTER_VARYING ||
                                            w.Data == Token.VARCHAR)
                                        {
                                            Notation.Record[] recs1 = notation.Select(w, Descriptor.Typelen, 1);
                                            if (recs1.Length > 0)
                                                WriteTextFormat("({0})", recs1[0].args[0]);
                                        }
                                        else if (w.Data == Token.DEC || w.Data == Token.DECIMAL ||
                                            w.Data == Token.NUMERIC || w.Data == Token.FLOAT)
                                        {
                                            Notation.Record[] recs1 = notation.Select(w, Descriptor.Typeprec, 1);
                                            if (recs1.Length > 0)
                                            {
                                                WriteText("(");
                                                WriteText(recs1[0].args[0].ToString());
                                                recs1 = notation.Select(w, Descriptor.Typescale, 1);
                                                if (recs1.Length > 0)
                                                {
                                                    WriteText(",");
                                                    WriteText(recs1[0].args[0].ToString());
                                                }
                                                WriteText(")");
                                            }
                                        }
                                    }
                                    WriteText(")");
                                }
                                break;
                        }
                        break;
                }                
            }
        }

        //protected virtual void WriteXPathExp(Symbol sym)
        //{
        //    Notation.Record[] recs = notation.Select(sym, Descriptor.At, 2);
        //    if (recs.Length > 0)
        //    {
        //        WriteQualifiedName((Qname)recs[0].Arg0);
        //        WriteText("[");
        //        WriteValueExp(recs[0].Arg1);
        //        WriteText("]");
        //    }
        //    else
        //    {
        //        recs = notation.Select(sym, Descriptor.XPath, 2);
        //        if (recs.Length > 0)
        //        {
        //            Notation.Record[] recs1 = notation.Select(recs[0].Arg0, Descriptor.At, 2);
        //            if (recs1.Length > 0)
        //            {
        //                WriteQualifiedName((Qname)recs1[0].Arg0);
        //                WriteText("[");
        //                WriteValueExp(recs1[0].Arg1);
        //                WriteText("]");
        //            }
        //            else
        //                WriteQualifiedName((Qname)recs[0].Arg0);                    
        //            Symbol[] path = Lisp.ToArray<Symbol>(recs[0].args[1]);
        //            foreach (Symbol p in path)
        //            {
        //                WriteText("\\");                        
        //                recs1 = notation.Select(p, Descriptor.Aixes, 1);
        //                if (recs1.Length > 0)
        //                {                            
        //                    WriteText(recs1[0].args[0].ToString());
        //                    WriteText("::");
        //                }
        //                WriteQualifiedName((Qname)p);
        //                recs1 = notation.Select(p, Descriptor.At, 1);
        //                if (recs1.Length > 0)
        //                {
        //                    WriteText("[");
        //                    WriteValueExp(recs1[0].Arg0);
        //                    WriteText("]");
        //                }
        //            }
        //        }
        //    }
        //}

        protected virtual void WriteDrefExp(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Dref, 2);
            if (recs.Length > 0)
            {
                WriteValueExp(recs[0].Arg0);                
                WriteText("->");
                Literal lit = (Literal)recs[0].Arg1;
                WriteText(lit.Data);
                return;
            }
            recs = notation.Select(sym, Descriptor.At, 2);
            if (recs.Length > 0)
            {
                WriteValueExp(recs[0].Arg0);
                WriteText("[");
                WriteValueExp(recs[0].Arg1);
                WriteText("]");
                return;
            }
            recs = notation.Select(sym, Descriptor.Wref, 2);
            if (recs.Length > 0)
            {
                WriteValueExp(recs[0].Arg0);
                WriteText("//");
                Literal lit = (Literal)recs[0].Arg1;
                WriteText(lit.Data);
                return;
            }
        }

        protected virtual void WriteSubQuery(Symbol squery)
        {
            WriteOptimizerHint(squery);
            WriteText("(");
            WriteQueryExp(squery);
            WriteOrderByClause(squery);
            WriteText(")");
        }

        protected virtual void WriteSetFuncSpec(Symbol sym)
        {
            if (notation.Flag(sym, Descriptor.AggCount))
                WriteText("COUNT(*)");
            else
            {
                Notation.Record[] recs = notation.Select(sym, Descriptor.Aggregate, 2);
                if (recs.Length == 0)
                    return;
                TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                switch (w.Data)
                {
                    case Token.COUNT:
                        WriteText("COUNT");
                        break;
                    case Token.SUM:
                        WriteText("SUM");
                        break;
                    case Token.AVG:
                        WriteText("AVG");
                        break;
                    case Token.MIN:
                        WriteText("MIN");
                        break;
                    case Token.MAX:
                        WriteText("MAX");
                        break;
                    case Token.XMLAGG:
                        WriteText("XMLAGG");
                        break;
                }
                WriteText("(");
                if (notation.Flag(sym, Descriptor.Distinct))
                    WriteText("DISTINCT ");
                WriteValueExp(recs[0].Arg1);
                recs = notation.Select(sym, Descriptor.Order, 1);
                if (recs.Length > 0)
                {
                    WriteText(" ORDER BY ");
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                    for (int k = 0; k < arr.Length; k++)
                    {
                        if (k > 0)
                            WriteText(", ");
                        WriteSortKey((Value)arr[k]);
                        if (notation.Flag(arr[k], Descriptor.Desc))
                            WriteText(" DESC");
                    }
                }
                WriteText(")");
            }
        }

        protected virtual void WriteCaseExp(Symbol sym)
        {
            WriteText("CASE ");
            Notation.Record[] recs = notation.Select(sym, Descriptor.Case, 2);
            if (recs.Length > 0)
            {
                WriteValueExp(recs[0].Arg0);
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                SmartNewLine();
                for (int k = 0; k < arr.Length; k++)
                {
                    recs = notation.Select(arr[k], Descriptor.CaseBranch, 2);
                    if (recs.Length > 0)
                    {
                        WriteText("  WHEN ");
                        WriteValueExp(recs[0].Arg0);
                        WriteText(" THEN ");
                        WriteValueExp(recs[0].Arg1);
                    }
                    else
                    {
                        recs = notation.Select(arr[k], Descriptor.ElseBranch, 1);
                        if (recs.Length > 0)
                        {
                            WriteText("  ELSE ");
                            WriteValueExp(recs[0].Arg0);
                        }
                    }
                    SmartNewLine();
                }
            }
            else
            {
                recs = notation.Select(sym, Descriptor.Case, 1);
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                SmartNewLine();
                for (int k = 0; k < arr.Length; k++)
                {
                    recs = notation.Select(arr[k], Descriptor.CaseBranch, 2);
                    if (recs.Length > 0)
                    {
                        WriteText("  WHEN ");
                        WriteSearchCondition(recs[0].Arg0);
                        WriteText(" THEN ");
                        WriteValueExp(recs[0].Arg1);
                    }
                    else
                    {
                        recs = notation.Select(arr[k], Descriptor.ElseBranch, 1);
                        if (recs.Length > 0)
                        {
                            WriteText("  ELSE ");
                            WriteValueExp(recs[0].Arg0);
                        }
                    }
                    SmartNewLine();
                }
            }
            WriteText("END");
            SmartNewLine();
        }

        protected virtual void WriteSearchCondition(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.LogicalOR, 2);
            if (recs.Length > 0)
            {
                WriteSearchCondition(recs[0].Arg0);
                WriteText(" OR ");
                WriteBooleanTerm(recs[0].Arg1);
            }
            else
                WriteBooleanTerm(sym);
        }

        protected virtual void WriteBooleanTerm(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.LogicalAND, 2);
            if (recs.Length > 0)
            {
                WriteBooleanTerm(recs[0].Arg0);
                WriteText(" AND ");
                WriteBooleanFactor(recs[0].Arg1);
            }
            else
                WriteBooleanFactor(sym);
        }

        protected virtual void WriteBooleanFactor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Inverse, 1);
            if (recs.Length > 0)
            {
                WriteText(" NOT ");
                WriteBooleanTest(recs[0].Arg0);
            }
            else
                WriteBooleanTest(sym);
        }

        protected virtual void WriteBooleanTest(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.BooleanTest, 2);
            if (recs.Length > 0)
            {
                WriteBooleanPrimary(recs[0].Arg1);
                WriteText(" IS ");
                if (notation.Flag(sym, Descriptor.Inverse))
                    WriteText(" NOT ");
                TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                switch (w.Data)
                {
                    case Token.TRUE:
                        WriteText("TRUE");
                        break;
                    case Token.FALSE:
                        WriteText("FALSE");
                        break;
                    case Token.UNKNOWN:
                        WriteText("UNKNOWN");
                        break;
                }
            }
            else
                WriteBooleanPrimary(sym);
        }

        protected virtual void WriteBooleanPrimary(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Branch, 1);
            if (recs.Length > 0)
            {
                WriteText(" (");
                WriteSearchCondition(recs[0].Arg0);
                WriteText(")");
            }
            else
                WritePredicate(sym);
        }

        protected virtual void WritePredicate(Symbol sym)
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
                        WriteRowConstructor(recs[0].Arg0);
                        WriteTextFormat(" {0} ", recs[0].args[1]);
                        WriteRowConstructor(recs[0].Arg2);
                        break;

                    case Descriptor.QuantifiedPred:
                        WriteRowConstructor(recs[0].Arg0);
                        WriteTextFormat(" {0} ", recs[0].args[1]);
                        TokenWrapper w = (TokenWrapper)recs[0].Arg2;
                        switch (w.Data)
                        {
                            case Token.ALL:
                                WriteText("ALL ");
                                break;

                            case Token.SOME:
                                WriteText("SOME ");
                                break;

                            case Token.ANY:
                                WriteText("ANY ");
                                break;
                        }
                        WriteSubQuery(recs[0].Arg3);
                        break;

                    case Descriptor.Between:
                        WriteRowConstructor(recs[0].Arg0);
                        if (notation.Flag(sym, Descriptor.Inverse))
                            WriteText(" NOT");
                        WriteText(" BETWEEN ");
                        WriteRowConstructor(recs[0].Arg1);
                        WriteText(" AND ");
                        WriteRowConstructor(recs[0].Arg2);
                        break;

                    case Descriptor.InSet:
                        WriteRowConstructor(recs[0].Arg0);
                        if (notation.Flag(sym, Descriptor.Inverse))
                            WriteText(" NOT");
                        WriteText(" IN ");
                        WriteInPredicateValue(recs[0].Arg1);
                        break;

                    case Descriptor.Like:
                        WriteValueExp(recs[0].Arg0);
                        if (notation.Flag(sym, Descriptor.Inverse))
                            WriteText(" NOT");
                        WriteText(" LIKE ");
                        WriteValueExp(recs[0].Arg1);
                        Notation.Record[] recs1 = 
                            notation.Select(sym, Descriptor.Escape, 1);
                        if (recs1.Length > 0)
                        {
                            WriteText(" ESCAPE ");
                            WriteValueExp(recs1[0].Arg0);
                            //WriteUnsignedLit((Value)recs1[0].args[0]);
                        }
                        break;

                    case Descriptor.IsNull:
                        WriteRowConstructor(recs[0].Arg0);
                        if (notation.Flag(sym, Descriptor.Inverse))
                            WriteText(" IS NOT NULL");
                        else
                            WriteText(" IS NULL");
                        break;

                    case Descriptor.Exists:
                        WriteText(" EXISTS ");
                        WriteSubQuery(recs[0].Arg0);
                        break;

                    case Descriptor.Unique:
                        WriteText(" UNIQUE ");
                        WriteSubQuery(recs[0].Arg0);
                        break;

                    case Descriptor.Match:
                        WriteRowConstructor(recs[0].Arg0);
                        WriteText(" MATCH ");
                        if (notation.Flag(sym, Descriptor.Unique))
                            WriteText("UNIQUE ");
                        Notation.Record[] recs2 =
                            notation.Select(sym, Descriptor.MatchType, 1);
                        if (recs2.Length > 0)
                        {
                            TokenWrapper w1 = (TokenWrapper)recs2[0].Arg0;
                            switch (w1.Data)
                            {
                                case Token.FULL:
                                    WriteText("FULL ");
                                    break;
                                case Token.PARTIAL:
                                    WriteText("PARTIAL ");
                                    break;
                            }
                        }
                        WriteSubQuery(recs[0].Arg1);
                        break;

                    case Descriptor.Overlaps:
                        WriteRowConstructor(recs[0].Arg0);
                        WriteText(" OVERLAPS ");
                        WriteRowConstructor(recs[0].Arg1);
                        break;
                }
        }

        protected virtual void WriteInPredicateValue(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.ValueList, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                WriteText("(");
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteValueExp(arr[k]);
                }
                WriteText(")");
            }
            else
                WriteSubQuery(sym);
        }

        protected virtual void WriteSortKey(Value value)
        {
            if (value is Qname)
                WriteQualifiedName((Qname)value);
            else
                WriteUnsignedLit(value);
        }

        protected virtual void WriteParameter(Parameter param)
        {
            WriteText(ProviderHelper.FormatParameter(param.ParameterName));
        }

        protected virtual void WritePlaceholder(Placeholder placeholder)
        {
            WriteText(placeholder.ToString());
        }

        protected virtual void WriteQualifiedNamePrefix(Qname qname)
        {
            Notation.Record[] recs = notation.Select(qname, Descriptor.Prefix, 1);
            if (recs.Length > 0)
            {
                Literal lit = (Literal)recs[0].Arg0;
                WriteText(lit.Data);
                WriteText(':');
            }
        }

        protected virtual void WriteQualifiedName(Qname qname)
        {
            WriteQualifiedNamePrefix(qname);
            string[] identifierParts = qname.ToArray();            
            for (int i = 0; i < identifierParts.Length; i++)
            {                
                if (i > 0)
                    WriteText(ProviderHelper.Qualifer);
                WriteText(ProviderHelper.FormatIdentifier(identifierParts[i]));
            }
        }

        protected virtual void WriteUnsignedLit(Value value)
        {
            if (value is Literal)
            {
                Literal literal = (Literal)value;                
                WriteText(ProviderHelper.FormatLiteral(literal.Data));
            }
            else if (value is DecimalValue)
            {
                DecimalValue udecimal = (DecimalValue)value;
                WriteText(udecimal.Data.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is DoubleValue)
            {
                DoubleValue udouble = (DoubleValue)value;
                WriteText(udouble.Data.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is IntegerValue)
            {
                IntegerValue uinteger = (IntegerValue)value;
                WriteText(uinteger.Data.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is DateTimeValue)
            {
                DateTimeValue udateTime = (DateTimeValue)value;
                WriteText(ProviderHelper.FormatDateTime(udateTime.Data));
            }
        }

        #region Text formatting billet

        private StringBuilder sb;
        private bool newLineFlag;

        protected void SmartNewLine()
        {
            if (!newLineFlag)
            {
                sb.AppendLine();
                newLineFlag = true;
            }
        }

        protected void WriteText(char c)
        {
            sb.Append(c);
            newLineFlag = false;
        }

        protected void WriteText(string s)
        {
            sb.Append(s);
            newLineFlag = false;
        }

        protected void WriteTextFormat(string s, params object[] args)
        {
            WriteText(String.Format(s, args));
        }
        
        #endregion
    }
}
