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
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    /// <summary>
    /// QueryBinder is special object designed for binding correlated subqueries in
    /// notation before it trasformation to query plan.
    /// Also we process external parameters
    /// </summary>
    public class QueryBinder    
    {
        private class Binding
        {
            public object target;               // Qname || Parameter
            public object src;                  // Binding expression
            public Placeholder placeholder;     // Corresponded placeholder
        }

        private class QueryContext
        {
            public List<String> tables;
            public List<Binding> bindings; 
            public QueryContext owner;
            public bool allowCorrelation;

            public QueryContext(QueryContext owner)
                : this(owner, true)
            {
            }

            public QueryContext(QueryContext owner, bool allowCorrelation)
            {
                this.owner = owner;
                this.allowCorrelation = allowCorrelation;
                tables = new List<string>();
                bindings = new List<Binding>();
            }

            public Placeholder CreatePlaceholder()
            {
                return new Placeholder(bindings.Count + 1);
            }
        }

        public bool IsServerQuery { get; set; }

        public void Process(Notation notation)
        {
            Notation.Record[] recs = notation.Select(Descriptor.Root, 1);
            ProcessQueryExp(notation, recs[0].Arg0, new QueryContext(null));            
        }

        private void ProcessQueryExp(Notation notation, Symbol qexpr, QueryContext context)        
        {
            Notation.Record[] recs = notation.Select(qexpr,
                new Descriptor[] { Descriptor.Union, Descriptor.Except }, 2);
            if (recs.Length > 0)
            {
                ProcessQueryExp(notation, recs[0].Arg0, context);
                ProcessQueryTerm(notation, recs[0].Arg1, context);
            }
            else
                ProcessQueryTerm(notation, qexpr, context);
            ConfirmBindings(notation, qexpr, context);
        }

        private void ProcessQueryTerm(Notation notation, Symbol qterm, QueryContext context)
        {
            Notation.Record[] recs = notation.Select(qterm, Descriptor.Intersect, 2);
            if (recs.Length > 0)
            {
                ProcessQueryTerm(notation, recs[0].Arg0, context);
                ProcessQueryTerm(notation, recs[0].Arg1, context);
            }
            else
                ProcessSimpleTable(notation, qterm, context);
        }

        private void ProcessSimpleTable(Notation notation, Symbol sym, QueryContext context)
        {
            if (sym.Tag == Tag.SQuery)
                ProcessQuerySpec(notation, sym, context);
        }

        private void ProcessQuerySpec(Notation notation, Symbol sym, QueryContext context)
        {
            context.tables.Clear();
            TableAnalyzer analyzer = new TableAnalyzer(notation);
            analyzer.WalkFromClause(sym);
            foreach (String name in analyzer.Tables)
                context.tables.Add(name);
            ProcessSelectList(notation, sym, context);
            ProcessFromClause(notation, sym, context);
            ProcessWhereClause(notation, sym, context);
            ProcessHavingClause(notation, sym, context);            
        }

        private void ProcessSelectList(Notation notation, Symbol sym, QueryContext context)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Select });
            if (recs.Length > 0 && recs[0].args.Length != 0)
                ScanRecord(notation, recs[0], context);
        }

        private void ProcessFromClause(Notation notation, Symbol sym, QueryContext context)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.From, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                    ProcessTableRef(notation, arr[k], context);
            }
        }

        private void ProcessTableRef(Notation notation, Symbol sym, QueryContext context)
        {
            if (sym.Tag == Tag.Join)
                ProcessJoinedTable(notation, sym, context);
            else
                ProcessTableRefSimple(notation, sym, context);
        }

        private void ProcessTableRefSimple(Notation notation, Symbol sym, QueryContext context)
        {
            if (sym.Tag != Tag.Qname)
            {
                Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Dynatable, Descriptor.Tuple }, 1);
                if (recs.Length > 0)
                    ScanExpr(notation, recs[0].Arg0, context);
                else
                    ProcessQueryExp(notation, sym, new QueryContext(context, true));
            }
        }

        private void ProcessJoinedTable(Notation notation, Symbol sym, QueryContext context)
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
                    ProcessTableRefSimple(notation, recs[0].Arg0, context);
                    ProcessTableRef(notation, recs[0].Arg1, context);
                    break;

                case Descriptor.UnionJoin:
                    ProcessTableRefSimple(notation, recs[0].Arg0, context);
                    ProcessTableRef(notation, recs[0].Arg1, context);
                    break;

                case Descriptor.NaturalJoin:
                    ProcessTableRefSimple(notation, recs[0].Arg0, context);
                    ProcessTableRef(notation, recs[0].Arg1, context);
                    break;

                case Descriptor.QualifiedJoin:
                    ProcessTableRefSimple(notation, recs[0].Arg0, context);
                    ProcessTableRef(notation, recs[0].Arg1, context);
                    recs = notation.Select(sym, Descriptor.JoinSpec, 1);
                    if (recs.Length > 0)
                        ScanExpr(notation, recs[0].Arg0, context);                    
                    break;

                case Descriptor.Branch:
                    ProcessTableRef(notation, recs[0].Arg0, context);
                    break;
            }
        }

        private void ProcessWhereClause(Notation notation, Symbol sym, QueryContext context)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Where, 1);
            if (recs.Length > 0)
                ScanExpr(notation, recs[0].Arg0, context);
        }

        private void ProcessHavingClause(Notation notation, Symbol sym, QueryContext context)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Having, 1);
            if (recs.Length > 0)
                ScanExpr(notation, recs[0].Arg0, context);
        }

        private void ScanExpr(Notation notation, Symbol sym, QueryContext context)
        {
            if (sym.Tag == Tag.SQuery)
                ProcessQueryExp(notation, sym, new QueryContext(context));
            else
                foreach (Notation.Record rec in notation.Select())
                    if (rec.sym == sym)
                        ScanRecord(notation, rec, context);
        }

        private void ScanRecord(Notation notation, Notation.Record rec, QueryContext context)
        {
            for (int k = 0; k < rec.args.Length; k++)
            {
                object arg = rec.args[k];
                if (Lisp.IsNode(arg))
                {
                    if (arg is Parameter)
                        ProcessParameter(notation, ref rec.args[k], context);
                    else if (arg is Qname)
                    {
                        if (!IsServerQuery)
                            ProcessQualifiedName(notation, ref rec.args[k], context);
                    }
                    else if (arg is Symbol)
                        ScanExpr(notation, (Symbol)arg, context);
                }
                else
                    for (object curr = arg; curr != null; curr = Lisp.Cdr(curr))
                    {
                        object item = Lisp.Car(curr);
                        if (item is Parameter)
                        {
                            ProcessParameter(notation, ref item, context);
                            Lisp.Rplaca(curr, item);
                        }
                        else if (item is Qname)
                        {
                            if (!IsServerQuery)
                            {
                                ProcessQualifiedName(notation, ref item, context);
                                Lisp.Rplaca(curr, item);
                            }
                        }
                        else if (item is Symbol)
                            ScanExpr(notation, (Symbol)item, context);
                    }
            }
        }

        private void ConfirmBindings(Notation notation, Symbol qexpr, QueryContext context)
        {
            if (context.bindings.Count > 0)
            {
                Symbol[] bindings = new Symbol[context.bindings.Count];
                for (int k = 0; k < bindings.Length; k++)
                {
                    bindings[k] = new Symbol(Tag.Binding);
                    if (context.bindings[k].src == null)
                        notation.Confirm(bindings[k], Descriptor.Link, context.bindings[k].target);
                    else
                        notation.Confirm(bindings[k], Descriptor.Link, context.bindings[k].src);
                }
                notation.Confirm(qexpr, Descriptor.Binding, Lisp.List(bindings));
            }
        }

        private void ProcessQualifiedName(Notation notation, ref object arg, QueryContext context)
        {
            Qname qname = (Qname)arg;
            if (qname.Qualifier != null)
            {
                Stack<Binding> bindings = new Stack<Binding>();
                for (QueryContext curr = context; curr != null; curr = curr.owner)
                {                    
                    object src = null;
                    if (curr.tables.Contains(qname.Qualifier))
                        src = qname;
                    else
                        for (int k = 0; k < curr.bindings.Count; k++)
                            if (qname.Equals(curr.bindings[k].target))
                            {
                                src = curr.bindings[k].placeholder;
                                break;
                            }
                    Binding b;
                    if (src == null)
                    {
                        b = new Binding();
                        b.target = qname;
                        b.placeholder = curr.CreatePlaceholder();
                        bindings.Push(b);
                        curr.bindings.Add(b);
                    }
                    else
                    {
                        while (bindings.Count > 0)
                        {
                            b = bindings.Pop();
                            b.src = src;
                            src = b.placeholder;
                        }
                        arg = src;
                        return;
                    }
                    if (!curr.allowCorrelation)
                        break;
                }
                throw new ESQLException(Properties.Resources.InvalidIdentifier, qname);
            }            
        }

        private void ProcessParameter(Notation notation, ref object arg, QueryContext context)
        {
            Parameter param = (Parameter)arg;
            Stack<Binding> bindings = new Stack<Binding>();
            object src = null;
            for (QueryContext curr = context; curr != null; curr = curr.owner)
            {
                src = null;
                for (int k = 0; k < curr.bindings.Count; k++)
                    if (param.Equals(curr.bindings[k].target))
                    {
                        src = curr.bindings[k].placeholder;
                        break;
                    }
                Binding b;
                if (src == null)
                {
                    b = new Binding();
                    b.target = param;
                    b.placeholder = curr.CreatePlaceholder();
                    bindings.Push(b);
                    curr.bindings.Add(b);
                }
                else
                    break;
            }
            while (bindings.Count > 0)
            {
                Binding b = bindings.Pop();
                b.src = src;
                src = b.placeholder;
            }
            arg = src;
        }

        private class TableAnalyzer : QueryWalker
        {
            private List<String> _tables = new List<String>();

            public TableAnalyzer(Notation notation)
                : base(notation)
            {
            }

            public override void WalkTableRefSimple(Symbol sym)
            {                
                string alias = null;
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
                    _tables.Add(alias);
                base.WalkTableRefSimple(sym);
            }

            public override void WalkSubQuery(Symbol squery)
            {
                return;
            }

            public List<String> Tables { get { return _tables; } }
        }
    }
}
