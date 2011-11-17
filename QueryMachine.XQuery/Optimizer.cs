//        Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    class Optimizer
    {
        private XQueryContext _context;

        private const double MagicRate = 3000.0;

        public QueryPlanTarget Target { get; set; }
        public bool ForceParallelPlan { get; private set; }

        public Optimizer(XQueryContext context)
        {
            _context = context;
        }

        public void Process(Notation notation)
        {
            FLWORVisitor visitor = new FLWORVisitor(notation);
            if (Target == QueryPlanTarget.AllItems)
                visitor.Action = new VisitorAction((Notation.Record rec, Symbol s) =>
                    {
                        if (IsComplexExpr(notation, rec))
                        {
                            notation.Confirm(s, Descriptor.Parallel);
                            ForceParallelPlan = true;
                        }
                    });
            visitor.Write();
#if DEBUG
            if (_context.uri != null)
                Trace.WriteLine(_context.uri);
            else
                Trace.WriteLine("<MainModule>");
            XQueryTextWriter qtw = new XQueryTextWriter(notation);
            qtw.Write();
            Trace.Indent();
            Trace.WriteLine(qtw.ToString());
            Trace.Unindent();
            if (ForceParallelPlan)
                Trace.WriteLine("Optimizer: FORCE_PARALLEL_PLAN");
#endif
        }

        private bool IsComplexExpr(Notation notation, Notation.Record rec)
        {
            ComplexityEstimationVisitor estimateVisitor = new ComplexityEstimationVisitor(notation);
            estimateVisitor.WriteFLWORExpr(rec);
            double rate = estimateVisitor.Rate;
            double bodyRate = estimateVisitor.TopRate(0);
            if (rate > MagicRate && bodyRate > rate - bodyRate)
                return true;
            return false;
        }

        private delegate void VisitorAction(Notation.Record rec, Symbol s);

        private class FLWORVisitor : XQueryNullWriter
        {
            private bool funcScope;
            private int flworCount;

            public VisitorAction Action { get; set; }

            public FLWORVisitor(Notation notation)
                : base(notation)
            {
                flworCount = 0; 
                funcScope = false;
            }

            public override void WriteFuncDecl(Notation.Record rec)
            {
                funcScope = true;
                base.WriteFuncDecl(rec);
                funcScope = false;
            }

            public override void WriteFLWORExpr(Notation.Record rec)
            {
                int count = 0;
                Symbol flworS = null;
                if (!funcScope)
                {
                    Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[0]);
                    for (int k = 0; k < arr.Length; k++)
                    {
                        Notation.Record[] recs = notation.Select(arr[k]);
                        if (recs[0].descriptor == Descriptor.For)
                        {
                            if (flworS == null)
                                flworS = recs[0].sym;
                            count++;
                        }
                    }
                }
                if (count > 0)
                    flworCount++;
                base.WriteFLWORExpr(rec);
                if (count > 0 && --flworCount == 0)
                {
                    if (Action != null)
                        Action(rec, flworS);
                    if (count == 1 && rec.Arg1 != null)
                        notation.Confirm(rec.Arg1, Descriptor.HashJoin, false);
                }
            }
        }

        private class ComplexityEstimationVisitor : XQueryNullWriter
        {
            private Stack<double> values = new Stack<double>();
            private Stack<int> anchor = new Stack<int>();

            private const double ArithmeticOperationCost = 0.1;
            private const double LogicalOperationCost = 0.05;
            private const double FuncCost = 0.01;
            private const double ComparisonOperationCost = 0.5;
            private const double UnionExprCost = 1.4;

            public double Rate
            {
                get
                {
                    double res = 0.0;
                    foreach (double v in values)
                        res += v;
                    return res;
                }
            }

            public double TopRate(int index)
            {
                return values.ToArray()[index];
            }

            public void Enter()
            {
                anchor.Push(values.Count);
            }

            private void DoubleFactor()
            {
                if (values.Count > 0)
                    values.Push(2 * values.Pop());
            }

            private void Power(double n)
            {
                if (values.Count > 0)
                    values.Push(Math.Pow(values.Pop(), n));
            }

            private void LeaveWithSum()
            {
                double res = 0;
                for (int n = anchor.Pop(); values.Count > n; )
                    res += values.Pop();
                values.Push(res == 0 ? 1 : res);
            }

            private void LeaveWithProduct()
            {
                double res = 1.0;
                for (int n = anchor.Pop(); values.Count > n; )
                {
                    double a = values.Pop();
                    if (a > 1)
                        res *= a;
                }
                values.Push(res);
            }

            public ComplexityEstimationVisitor(Notation notation)
                : base(notation)
            {
            }

            public override void WriteExpr(object expr)
            {
                Enter();
                base.WriteExpr(expr);
                LeaveWithSum();
            }

            public override void WriteExprSingle(Symbol sym)
            {
                Enter();
                base.WriteExprSingle(sym);
                LeaveWithSum();
            }

            public override void WriteFLWORExpr(Notation.Record rec)
            {
                int forExprCount = 0;
                Enter();
                Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[0]);
                for (int k = 0; k < arr.Length; k++)
                {
                    Notation.Record[] recs = notation.Select(arr[k]);
                    switch (recs[0].descriptor)
                    {
                        case Descriptor.For:
                            WriteForOperator(recs[0]);
                            forExprCount++;
                            break;

                        case Descriptor.Let:
                            WriteLetOperator(recs[0]);
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
                LeaveWithSum();
                Enter();
                if (rec.Arg1 != null)
                    WriteWhereClause(rec.Arg1);
                if (rec.Arg2 != null)
                    WriteOrderByClause(rec.Arg2);
                WriteExprSingle(rec.Arg3);
                LeaveWithSum();
                if (forExprCount > 0)
                    Power(forExprCount);
            }

            public override void WritePrimaryExpr(Symbol sym)
            {
                if (sym.Tag == Tag.Literal)
                    values.Push(0);
                else if (sym.Tag == Tag.Integer)
                    values.Push(0);
                else if (sym.Tag == Tag.Double)
                    values.Push(0);
                else if (sym.Tag == Tag.Decimal)
                    values.Push(0);
                else if (sym.Tag == Tag.VarName)
                    values.Push(0);
                else if (sym.Tag == Tag.TokenWrapper)
                {
                    if (((TokenWrapper)sym).Data == '.')
                        values.Push(0);
                }
                else
                    base.WritePrimaryExpr(sym);
            }

            public override void WriteDirectConstructor(Symbol sym)
            {
                Enter();
                base.WriteDirectConstructor(sym);
                LeaveWithSum();
            }

            public override void WriteFilterExpr(Notation.Record rec)
            {
                Enter();                
                WritePrimaryExpr(rec.Arg0);
                WritePredicateList(rec.Arg0);
                LeaveWithSum();
            }

            public override void WriteFuncallExpr(Notation.Record rec)
            {
                Enter();
                if (rec.args[1] != null)
                {
                    Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[1]);
                    for (int k = 0; k < arr.Length; k++)
                        WriteExprSingle(arr[k]);
                }
                values.Push(FuncCost);
                LeaveWithSum();
            }

            public override void WriteUnaryExpr(Symbol sym)
            {
                Enter();
                values.Push(ArithmeticOperationCost);
                base.WriteUnaryExpr(sym);
                DoubleFactor();
                LeaveWithSum();
            }

            public override void WriteCastExpr(Symbol sym)
            {
                Enter();
                values.Push(LogicalOperationCost);
                base.WriteCastExpr(sym);
                DoubleFactor();
                LeaveWithSum();
            }

            public override void WriteCastableExpr(Symbol sym)
            {
                Enter();
                values.Push(LogicalOperationCost);
                base.WriteCastableExpr(sym);
                DoubleFactor();
                LeaveWithSum();
            }

            public override void WriteTreatExpr(Symbol sym)
            {
                Enter();
                values.Push(LogicalOperationCost);
                base.WriteTreatExpr(sym);
                DoubleFactor();
                LeaveWithSum();
            }

            public override void WriteInstanceofExpr(Symbol sym)
            {
                Enter();
                values.Push(LogicalOperationCost);
                base.WriteInstanceofExpr(sym);
                DoubleFactor();
                LeaveWithSum();
            }

            public override void WriteIntersectExceptExpr(Symbol sym)
            {
                Enter();
                values.Push(UnionExprCost);
                base.WriteIntersectExceptExpr(sym);
                LeaveWithSum();
            }

            public override void WriteUnionExpr(Symbol sym)
            {
                Enter();
                values.Push(UnionExprCost);
                base.WriteUnionExpr(sym);
                LeaveWithSum();
            }

            public override void WriteMultiplicativeExpr(Symbol sym)
            {
                Enter();
                values.Push(ArithmeticOperationCost);
                base.WriteMultiplicativeExpr(sym);
                LeaveWithSum();
            }

            public override void WriteAdditiveExpr(Symbol sym)
            {
                Enter();
                values.Push(ArithmeticOperationCost);
                base.WriteAdditiveExpr(sym);
                LeaveWithSum();
            }

            public override void WriteRangeExpr(Symbol sym)
            {
                Enter();
                base.WriteRangeExpr(sym);
                LeaveWithSum();
            }

            public override void WriteComparisonExpr(Symbol sym)
            {
                Enter();
                values.Push(ComparisonOperationCost);
                base.WriteComparisonExpr(sym);
                LeaveWithSum();
            }

            public override void WriteAndExpr(Symbol sym)
            {
                Enter();
                values.Push(LogicalOperationCost);
                base.WriteAndExpr(sym);
                LeaveWithSum();
            }

            public override void WriteOrExpr(Symbol sym)
            {
                Enter();
                values.Push(LogicalOperationCost);
                base.WriteOrExpr(sym);
                LeaveWithSum();
            }

            public override void WriteRelativePathExpr(Symbol sym)
            {
                Enter();
                base.WriteRelativePathExpr(sym);
                LeaveWithProduct();
            }

            public override void WriteNodeTest(Symbol sym)
            {
                base.WriteNodeTest(sym);
                values.Push(1);
            }
        }
    }
}
