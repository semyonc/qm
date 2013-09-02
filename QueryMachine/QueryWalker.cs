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

using DataEngine.CoreServices;
using DataEngine.Parser;

namespace DataEngine
{
    public class QueryWalker
    {
        protected readonly Notation notation;

        public QueryWalker(Notation notation)
        {
            this.notation = notation;
        }

        public virtual void WalkStmt()
        {
            Notation.Record[] recs = notation.Select(Descriptor.Root, 1);
            WalkQueryExp(recs[0].Arg0);
            recs = notation.Select(recs[0].Arg0, Descriptor.Order, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                    WalkSortKey((Value)arr[k]);
            }
        }

        public virtual void WalkQueryExp(Symbol qexpr)
        {
            Notation.Record[] recs = notation.Select(qexpr,
                new Descriptor[] { Descriptor.Union, Descriptor.Except }, 2);
            if (recs.Length > 0)
            {
                WalkDescriptor(recs[0].descriptor);
                WalkQueryExp(recs[0].Arg0);
                WalkQueryTerm(recs[0].Arg1);
            }
            else
                WalkQueryTerm(qexpr);
        }

        public virtual void WalkQueryTerm(Symbol qterm)
        {
            Notation.Record[] recs = notation.Select(qterm, Descriptor.Intersect, 2);
            if (recs.Length > 0)
            {
                WalkDescriptor(recs[0].descriptor);
                WalkQueryTerm(recs[0].Arg0);
                WalkQueryTerm(recs[0].Arg1);
            }
            else
                WalkSimpleTable(qterm);
        }

        public virtual void WalkSimpleTable(Symbol sym)
        {
            switch (sym.Tag)
            {
                case Tag.SQuery:
                    WalkQuerySpec(sym);
                    break;

                case Tag.TableConstructor:
                    WalkTableConstructor(sym);
                    break;

                case Tag.ExplictTable:
                    WalkExplictTable(sym);
                    break;
            }
        }

        public virtual void WalkExplictTable(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Explicit, 1);
            if (recs.Length > 0)
            {
                WalkDescriptor(recs[0].descriptor);
                WalkQualifiedName((Qname)recs[0].Arg0);
            }
        }

        public virtual void WalkTableConstructor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.TableValue, 1);
            if (recs.Length > 0)
            {
                WalkDescriptor(recs[0].descriptor);
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int i = 0; i < arr.Length; i++)
                    WalkRowConstructor((Symbol)arr[i]);
            }
        }

        public virtual void WalkQuerySpec(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Top, 1);
            if (recs.Length > 0)
                WalkDescriptor(Descriptor.Top);
            WalkFromClause(sym);
            WalkSelectList(sym);            
            WalkWhereClause(sym);
            WalkGroupByClause(sym);
            WalkHavingClause(sym);
        }

        public virtual void WalkFromClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.From, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                    WalkTableRef(arr[k]);
            }
        }

        public virtual void WalkTableRef(Symbol sym)
        {
            if (sym.Tag == Tag.Join)
                WalkJoinedTable(sym);
            else
                WalkTableRefSimple(sym);
        }

        public virtual void WalkTableRefSimple(Symbol sym)
        {
            if (sym is Qname)
                WalkTableQualifiedName((Qname)sym);
            else
            {
                Notation.Record[] recs1 = notation.Select(sym, 
                    new Descriptor[] { Descriptor.Dynatable, Descriptor.Tuple }, 1);                
                if (recs1.Length > 0)
                {
                    switch (recs1[0].descriptor)
                    {
                        case Descriptor.Dynatable:
                            WalkDescriptor(Descriptor.Dynatable);
                            WalkDynatable(notation, recs1[0].Arg0);
                            break;

                        case Descriptor.Tuple:
                            WalkDescriptor(Descriptor.Tuple);
                            WalkTuple(notation, recs1[0].Arg0);
                            break;
                    }
                }
                else
                    WalkSubQuery(sym);
            }
            Notation.Record[] recs = notation.Select(sym, Descriptor.Alias, 1);
            if (recs.Length > 0)
                WalkQualifiedName((Qname)recs[0].Arg0);
        }

        public virtual void WalkTableQualifiedName(Qname qname)
        {
            WalkQualifiedName(qname);
        }

        public virtual void WalkDynatable(Notation notation, Symbol sym)
        {
            WalkValueExp(sym);
        }

        public virtual void WalkTuple(Notation notation, Symbol sym)
        {
            WalkValueExp(sym);
        }

        public virtual void WalkJoinedTable(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] {
                Descriptor.CrossJoin,
                Descriptor.UnionJoin,
                Descriptor.NaturalJoin,
                Descriptor.QualifiedJoin,
                Descriptor.Branch });
            WalkDescriptor(recs[0].descriptor);
            switch (recs[0].descriptor)
            {
                case Descriptor.CrossJoin:
                    WalkTableRefSimple(recs[0].Arg0);
                    WalkTableRef(recs[0].Arg1);
                    break;

                case Descriptor.UnionJoin:
                    WalkTableRefSimple(recs[0].Arg0);
                    WalkTableRef(recs[0].Arg1);
                    break;

                case Descriptor.NaturalJoin:
                    WalkTableRefSimple(recs[0].Arg0);
                    WalkJoinType(sym);
                    WalkTableRef(recs[0].Arg1);
                    break;

                case Descriptor.QualifiedJoin:
                    WalkTableRefSimple(recs[0].Arg0);
                    WalkJoinType(sym);
                    WalkTableRef(recs[0].Arg1);
                    WalkJoinSpec(sym);
                    break;

                case Descriptor.Branch:
                    WalkTableRef(recs[0].Arg0);
                    break;
            }
        }

        public virtual void WalkJoinSpec(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.JoinSpec, 1);
            if (recs.Length > 0)
            {
                Notation.Record[] recs0 = notation.Select(sym, Descriptor.Using, 1);
                if (recs0.Length > 0)
                {
                    WalkDescriptor(Descriptor.Using);
                    WalkNamedColumnJoin(recs[0].Arg0);
                }
                else
                {
                    recs0 = notation.Select(sym, Descriptor.Constraint, 1);
                    if (recs0.Length > 0)
                    {
                        WalkDescriptor(Descriptor.Constraint);
                        WalkConstraintJoin(recs[0].Arg0);
                    }
                    else
                        WalkJoinSearchCondition(recs[0].Arg0);
                }
            }
        }

        public virtual void WalkJoinSearchCondition(Symbol sym)
        {
            WalkSearchCondition(sym);
        }

        public virtual void WalkConstraintJoin(Symbol sym)
        {            
        }

        public virtual void WalkNamedColumnJoin(Symbol sym)
        {            
        }

        public virtual void WalkJoinType(Symbol sym)
        {
        }

        public virtual void WalkWhereClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Where, 1);
            if (recs.Length > 0)
                WalkSearchCondition(recs[0].Arg0);
        }

        public virtual void WalkGroupByClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.GroupBy, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                    WalkQualifiedName((Qname)arr[k]);
            }
        }

        public virtual void WalkHavingClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Having, 1);
            if (recs.Length > 0)
                WalkSearchCondition(recs[0].Arg0);
        }

        public virtual void WalkSelectList(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Select });
            if (recs.Length > 0)
                if (recs[0].args.Length > 0)
                {
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                    for (int k = 0; k < arr.Length; k++)
                        WalkSelectSubList(arr[k]);
                }
        }

        public virtual void WalkSelectSubList(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.TableFields, 1);
            if (recs.Length > 0)
            {
                WalkQualifiedName((Qname)recs[0].Arg0);
            }
            else
            {
                if (sym.Tag == Tag.SQuery && notation.Flag(sym, Descriptor.Dynatable))
                    WalkSubQuery(sym);
                else
                    WalkValueExp(sym);
                recs = notation.Select(sym, Descriptor.Alias, 1);
                if (recs.Length > 0)
                    WalkAlias((Qname)recs[0].Arg0);
                    
            }
        }

        public virtual void WalkAlias(Qname qname)
        {
            WalkQualifiedName(qname);
        }

        public virtual void WalkRowConstructor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.RowValue, 1);
            if (recs.Length > 0)
            {
                WalkDescriptor(Descriptor.RowValue);
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int i = 0; i < arr.Length; i++)
                    WalkRowConstructorElem(arr[i]);
            }
            else
                WalkRowConstructorElem(sym);
        }

        public virtual void WalkRowConstructorElem(Symbol sym)
        {
            if (sym is TokenWrapper)
            {

            }
            else
                WalkValueExp(sym);
        }

        public virtual void WalkValueExp(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Concat, 2);
            if (recs.Length > 0)
            {
                WalkDescriptor(recs[0].descriptor);
                WalkValueExp(recs[0].Arg0);
                WalkCharFactor(recs[0].Arg1);
            }
            else
                WalkCharFactor(sym);
        }

        public virtual void WalkCharFactor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Substring, Descriptor.StringUpper, 
                Descriptor.StringLower, Descriptor.StringConvert, Descriptor.StringTrim });
            if (recs.Length > 0)
            {
                WalkDescriptor(recs[0].descriptor);
                switch (recs[0].descriptor)
                {
                    case Descriptor.Substring:
                        WalkValueExp(recs[0].Arg0);
                        WalkNumValueExp(recs[0].Arg1);
                        if (recs[0].args.Length == 3)
                            WalkNumValueExp(recs[0].Arg2);
                        break;

                    case Descriptor.StringUpper:
                        WalkValueExp(recs[0].Arg0);
                        break;

                    case Descriptor.StringLower:
                        WalkValueExp(recs[0].Arg0);
                        break;

                    case Descriptor.StringConvert:
                        WalkValueExp(recs[0].Arg0);
                        WalkQualifiedName((Qname)recs[0].Arg1);
                        break;

                    case Descriptor.StringTrim:
                        WalkValueExp(recs[0].Arg1);
                        WalkValueExp(recs[0].Arg2);
                        break;
                }
            }
            else
                WalkNumValueExp(sym);
        }

        public virtual void WalkNumValueExp(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym,
                new Descriptor[] { Descriptor.Add, Descriptor.Sub }, 2);
            if (recs.Length > 0)
            {
                WalkNumValueExp(recs[0].Arg0);
                WalkTerm(recs[0].Arg1);
            }
            else
                WalkTerm(sym);
        }

        public virtual void WalkTerm(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym,
                new Descriptor[] { Descriptor.Mul, Descriptor.Div }, 2);
            if (recs.Length > 0)
            {
                WalkTerm(recs[0].Arg0);
                WalkFactor(recs[0].Arg1);
            }
            else
                WalkFactor(sym);
        }

        public virtual void WalkFactor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.UnaryMinus, 1);
            if (recs.Length > 0)
                WalkNumPrimary(recs[0].Arg0);
            else
                WalkNumPrimary(sym);
        }

        public virtual void WalkNumPrimary(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.PosString, 2);
            if (recs.Length > 0)
            {
                WalkDescriptor(recs[0].descriptor);
                WalkValueExp(recs[0].Arg0);
                WalkValueExp(recs[0].Arg1);
            }
            else
                if (sym.Tag == Tag.SQLX)
                    WalkXmlValueFunc(sym);
                else
                    WalkNumExpPrimary(sym);
        }

        public virtual void WalkXmlValueFunc(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.XMLConcat, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                    WalkValueExp(arr[k]);
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLQuery, 3);
            if (recs.Length > 0)
            {
                if (recs[0].args[1] != null)
                {
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                    for (int k = 0; k < arr.Length; k++)
                    {
                        if (arr[k].Tag == Tag.SQuery &&
                            notation.Flag(arr[k], Descriptor.Dynatable))
                            WalkSubQuery(arr[k]);
                        else
                            WalkValueExp(arr[k]);
                        Notation.Record[] recs1 = notation.Select(arr[k], Descriptor.Alias, 1);
                        if (recs1.Length > 0)
                            WalkAlias((Qname)recs1[0].Arg0);
                    }
                }
                return;
            }            
            recs = notation.Select(sym, Descriptor.XMLForest, 1);
            if (recs.Length > 0)
            {                
                Notation.Record[] recs1 = notation.Select(sym, Descriptor.XMLNamespaces, 1);
                if (recs1.Length > 0)
                    WalkXmlNamespaces(recs1);
                WalkContentValueList(recs[0].args[0]);
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLParse, 3);
            if (recs.Length > 0)
            {
                WalkValueExp(recs[0].Arg1);
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLPI, 2);
            if (recs.Length > 0)
            {
                if (recs[0].Arg1 != null)
                    WalkValueExp(recs[0].Arg1);
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLComment, 1);
            if (recs.Length > 0)
            {
                WalkValueExp(recs[0].Arg0);
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLCDATA, 1);
            if (recs.Length > 0)
            {
                WalkValueExp(recs[0].Arg0);
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLRoot, 3);
            if (recs.Length > 0)
            {
                WalkValueExp(recs[0].Arg0);
                return;
            }
            recs = notation.Select(sym, Descriptor.XMLElement, 2);
            if (recs.Length > 0)
            {
                Notation.Record[] recs1 = notation.Select(sym, Descriptor.XMLNamespaces, 1);
                if (recs1.Length > 0)
                    WalkXmlNamespaces(recs1);
                recs1 = notation.Select(sym, Descriptor.XMLAttributes, 1);
                if (recs1.Length > 0)
                    WalkContentValueList(recs1[0].args[0]);
                if (recs[0].args[1] != null)
                {
                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                    for (int k = 0; k < arr.Length; k++)
                        WalkValueExp(arr[k]);
                }
                return;
            }
        }

        public virtual void WalkContentValueList(object arg)
        {
            Symbol[] arr = Lisp.ToArray<Symbol>(arg);
            for (int k = 0; k < arr.Length; k++)
                if (arr[k].Tag == Tag.TableFields)
                {
                    Notation.Record[] recs = notation.Select(arr[k], Descriptor.TableFields, 1);
                    if (recs.Length > 0)
                        WalkQualifiedName((Qname)recs[0].Arg0);
                }
                else
                    WalkValueExp(arr[k]);
        }

        public virtual void WalkXmlNamespaces(Notation.Record[] recs)
        {
            return;
        }

        public virtual void WalkNumExpPrimary(Symbol sym)
        {
            if (sym is Value)
            {
                if (sym is Parameter)
                    WalkParameter((Parameter)sym);
                else if (sym is Placeholder)
                    WalkPlaceholder((Placeholder)sym);
                else if (sym is Qname)
                {
                    Notation.Record[] recs = notation.Select(sym, Descriptor.Prefix, 1);
                    if (recs.Length > 0)
                        WalkTableQualifiedName((Qname)sym);
                    else
                        WalkQualifiedName((Qname)sym);
                }
                else
                    WalkUnsignedLit((Value)sym);
            }
            else
            {
                switch (sym.Tag)
                {
                    case Tag.SQuery:
                        WalkSubQuery(sym);
                        break;

                    case Tag.CaseExpr:
                        WalkCaseExp(sym);
                        break;

                    case Tag.AggExpr:
                        WalkSetFuncSpec(sym);
                        break;

                    case Tag.Dref:
                        WalkDrefExpr(sym);
                        break;

                    default:
                        Notation.Record[] recs = notation.Select(sym, new Descriptor[] { 
                            Descriptor.Branch, Descriptor.NullIf, Descriptor.Coalesce, Descriptor.Funcall, Descriptor.Cast });
                        WalkDescriptor(recs[0].descriptor);
                        switch (recs[0].descriptor)
                        {
                            case Descriptor.Branch:
                                WalkValueExp(recs[0].Arg0);
                                break;

                            case Descriptor.NullIf:
                                WalkValueExp(recs[0].Arg0);
                                WalkValueExp(recs[0].Arg1);
                                break;

                            case Descriptor.Coalesce:
                                {
                                    Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                                    for (int k = 0; k < arr.Length; k++)
                                        WalkValueExp(arr[k]);
                                }
                                break;

                            case Descriptor.Funcall:
                                WalkFuncall(recs[0]);
                                break;

                            case Descriptor.Cast:
                                if (recs[0].Arg0 != null)
                                    WalkValueExp(recs[0].Arg0);
                                break;
                        }
                        break;
                }
            }
        }

        //public virtual void WalkXPathExpr(Symbol sym)
        //{
        //    Notation.Record[] recs = notation.Select(sym, Descriptor.At, 2);
        //    if (recs.Length > 0)
        //    {
        //        WalkQualifiedName((Qname)recs[0].Arg0);
        //        WalkValueExp(recs[0].Arg1);
        //    }
        //    else
        //    {
        //        recs = notation.Select(sym, Descriptor.XPath, 2);
        //        if (recs.Length > 0)
        //        {
        //            Notation.Record[] recs1 = notation.Select(recs[0].Arg0, Descriptor.At, 2);
        //            if (recs1.Length > 0)
        //            {
        //                WalkQualifiedName((Qname)recs1[0].Arg0);
        //                WalkValueExp(recs1[0].Arg1);
        //            }
        //            else
        //                WalkQualifiedName((Qname)recs[0].Arg0);
        //            Symbol[] path = Lisp.ToArray<Symbol>(recs[0].args[1]);
        //            foreach (Symbol p in path)
        //            {
        //                WalkQualifiedName((Qname)p);
        //                recs1 = notation.Select(p, Descriptor.At, 1);
        //                if (recs1.Length > 0)
        //                    WalkValueExp(recs1[0].Arg0);
        //            }
        //        }
        //    }
        //}

        public virtual void WalkDrefExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Dref, 2);
            if (recs.Length > 0)                
            {
                WalkDescriptor(Descriptor.Dref);
                WalkDref(recs);
                return;
            }
            recs = notation.Select(sym, Descriptor.At, 2);
            if (recs.Length > 0)
            {
                WalkDescriptor(Descriptor.At);
                WalkValueExp(recs[0].Arg0);
                WalkValueExp(recs[0].Arg1);
                return;
            }
            recs = notation.Select(sym, Descriptor.Wref, 2);
            if (recs.Length > 0)
            {
                WalkDescriptor(Descriptor.Wref);
                WalkValueExp(recs[0].Arg0);
                return;
            }
        }

        public virtual void WalkDref(Notation.Record[] recs)
        {
            WalkValueExp(recs[0].Arg0);
        }

        public virtual void WalkFuncall(Notation.Record rec)
        {
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[1]);
            for (int k = 0; k < arr.Length; k++)
                WalkRowConstructorElem(arr[k]);            
        }

        public virtual void WalkSubQuery(Symbol squery)
        {
            WalkQueryExp(squery);
        }

        public virtual void WalkSetFuncSpec(Symbol sym)
        {
            if (! notation.Flag(sym, Descriptor.AggCount))
            {
                Notation.Record[] recs = notation.Select(sym, Descriptor.Aggregate, 2);
                if (recs.Length == 0)
                    return;
                WalkDescriptor(recs[0].descriptor);
                WalkValueExp(recs[0].Arg1);
            }
        }

        public virtual void WalkCaseExp(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Case, 2);
            if (recs.Length > 0)
            {
                WalkValueExp(recs[0].Arg0);
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[1]);
                for (int k = 0; k < arr.Length; k++)
                {
                    recs = notation.Select(arr[k], Descriptor.CaseBranch, 2);
                    if (recs.Length > 0)
                    {
                        WalkValueExp(recs[0].Arg0);
                        WalkValueExp(recs[0].Arg1);
                    }
                    else
                    {
                        recs = notation.Select(arr[k], Descriptor.ElseBranch, 1);
                        if (recs.Length > 0)
                            WalkValueExp(recs[0].Arg0);
                    }
                }
            }
            else
            {
                recs = notation.Select(sym, Descriptor.Case, 1);
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                {
                    recs = notation.Select(arr[k], Descriptor.CaseBranch, 2);
                    if (recs.Length > 0)
                    {
                        WalkSearchCondition(recs[0].Arg0);
                        WalkValueExp(recs[0].Arg1);
                    }
                    else
                    {
                        recs = notation.Select(arr[k], Descriptor.ElseBranch, 1);
                        if (recs.Length > 0)
                            WalkValueExp(recs[0].Arg0);
                    }
                }
            }
        }

        public virtual void WalkSearchCondition(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.LogicalOR, 2);
            if (recs.Length > 0)
            {
                WalkSearchCondition(recs[0].Arg0);
                WalkBooleanTerm(recs[0].Arg1);
            }
            else
                WalkBooleanTerm(sym);
        }

        public virtual void WalkBooleanTerm(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.LogicalAND, 2);
            if (recs.Length > 0)
            {
                WalkBooleanTerm(recs[0].Arg0);
                WalkBooleanFactor(recs[0].Arg1);
            }
            else
                WalkBooleanFactor(sym);
        }

        public virtual void WalkBooleanFactor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Inverse, 1);
            if (recs.Length > 0)
                WalkBooleanTest(recs[0].Arg0);
            else
                WalkBooleanTest(sym);
        }

        public virtual void WalkBooleanTest(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.BooleanTest, 2);
            if (recs.Length > 0)
                WalkBooleanPrimary(recs[0].Arg1);
            else
                WalkBooleanPrimary(sym);
        }

        public virtual void WalkBooleanPrimary(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Branch, 1);
            if (recs.Length > 0)
                WalkSearchCondition(recs[0].Arg0);
            else
                WalkPredicate(sym);
        }
     
        public virtual void WalkPredicate(Symbol sym)
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
            {
                WalkDescriptor(recs[0].descriptor);
                switch (recs[0].descriptor)
                {
                    case Descriptor.Pred:
                        WalkRowConstructor(recs[0].Arg0);
                        WalkRowConstructor(recs[0].Arg2);
                        break;

                    case Descriptor.QuantifiedPred:
                        WalkRowConstructor(recs[0].Arg0);
                        WalkSubQuery(recs[0].Arg3);
                        break;

                    case Descriptor.Between:
                        WalkRowConstructor(recs[0].Arg0);
                        WalkRowConstructor(recs[0].Arg1);
                        WalkRowConstructor(recs[0].Arg2);
                        break;

                    case Descriptor.InSet:
                        WalkRowConstructor(recs[0].Arg0);
                        WalkInPredicateValue(recs[0].Arg1);
                        break;

                    case Descriptor.Like:
                        WalkValueExp(recs[0].Arg0);
                        WalkValueExp(recs[0].Arg1);
                        Notation.Record[] recs1 =
                            notation.Select(sym, Descriptor.Escape, 1);
                        if (recs1.Length > 0)
                            WalkValueExp(recs1[0].Arg0);
                        break;

                    case Descriptor.IsNull:
                        WalkRowConstructor(recs[0].Arg0);
                        break;

                    case Descriptor.Exists:
                        WalkSubQuery(recs[0].Arg0);
                        break;

                    case Descriptor.Unique:
                        WalkSubQuery(recs[0].Arg0);
                        break;

                    case Descriptor.Match:
                        WalkRowConstructor(recs[0].Arg0);
                        WalkSubQuery(recs[0].Arg1);
                        break;

                    case Descriptor.Overlaps:
                        WalkRowConstructor(recs[0].Arg0);
                        WalkRowConstructor(recs[0].Arg1);
                        break;
                }
            }
        }

        public virtual void WalkInPredicateValue(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.ValueList, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                    WalkValueExp(arr[k]);
            }
            else
                WalkSubQuery(sym);
        }

        public virtual void WalkSortKey(Value value)
        {
            if (value is Qname)
                WalkQualifiedName((Qname)value);
            else
                WalkUnsignedLit(value);
        }

        public virtual void WalkParameter(Parameter param)
        {
        }

        public virtual void WalkPlaceholder(Placeholder placeholder)
        {
        }

        public virtual void WalkQualifiedName(Qname qname)
        {
        }

        public virtual void WalkUnsignedLit(Value value)
        {
        }

        public virtual void WalkDescriptor(Descriptor descriptor)
        {
        }
    }
}
