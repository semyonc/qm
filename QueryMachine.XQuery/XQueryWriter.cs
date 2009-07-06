//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Text;

using DataEngine.CoreServices;
using DataEngine.XQuery.Parser;
using System.Globalization;

namespace DataEngine.XQuery
{
    public class XQueryWriter
    {
        private Notation notation;

        public XQueryWriter(Notation notation)
        {
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
            WriteModule(recs[0].sym);
        }

        public virtual void WriteModule(Symbol module)
        {
            Notation.Record[] recs = notation.Select(module, Descriptor.Version, 2);
            if (recs.Length > 0)
            {
                WriteText("xquery version ");
                WriteLiteral((Literal)recs[0].Arg0);
                if (recs[0].Arg1 != null)
                {
                    WriteText(" encoding ");
                    WriteLiteral((Literal)recs[0].Arg1);
                }
                WriteSeparator();
            }
            recs = notation.Select(module, Descriptor.Root, 1);
            if (recs.Length == 0)
                throw new InvalidOperationException();
            Notation.Record[] recs_c = notation.Select(recs[0].Arg0, 
                new Descriptor[] { Descriptor.Query, Descriptor.Library });
            if (recs_c.Length > 0)
                if (recs_c[0].descriptor == Descriptor.Query)
                {
                    WriteProlog(recs_c[0].args[0]);
                    WriteExpr(recs_c[0].args[1]);
                }
                else
                {
                    WriteModuleDecl(recs_c[0].Arg0);
                    WriteProlog(recs_c[0].args[1]);
                }
        }

        public virtual void WriteModuleDecl(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.ModuleNamespace, 2);
            if (recs.Length > 0)
            {
                WriteText("module namespace ");
                WriteQName((Qname)recs[0].Arg0);
                WriteText(" = ");
                WriteLiteral((Literal)recs[0].Arg1);
                WriteSeparator();
            }
        }

        public virtual void WriteProlog(object prolog)
        {
            if (prolog != null)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(prolog);
                foreach (Symbol sym in arr)
                {
                    Notation.Record[] recs = notation.Select(sym);
                    if (recs.Length > 0)
                    {
                        switch (recs[0].descriptor)
                        {
                            case Descriptor.BoundarySpace:
                                WriteBoundarySpace(recs[0]);
                                break;

                            case Descriptor.DefaultCollation:
                                WriteDefaultCollaction(recs[0]);
                                break;

                            case Descriptor.BaseUri:
                                WriteBaseUri(recs[0]);
                                break;

                            case Descriptor.ConstructionDecl:
                                WriteConstructionDecl(recs[0]);
                                break;

                            case Descriptor.Ordering:
                                WriteOrdering(recs[0]);
                                break;

                            case Descriptor.DefaultOrder:
                                WriteDefaultOrder(recs[0]);
                                break;

                            case Descriptor.CopyNamespace:
                                WriteCopyNamespace(recs[0]);
                                break;

                            case Descriptor.ImportSchema:
                                WriteImportSchema(recs[0]);
                                break;

                            case Descriptor.ImportModule:
                                WriteImportModule(recs[0]);
                                break;

                            case Descriptor.Namespace:
                                WriteNamespaceDecl(recs[0]);
                                break;

                            case Descriptor.DefaultElement:
                            case Descriptor.DefaultFunction:
                                WriteDefaultNamespaceDecl(recs[0]);
                                break;

                            case Descriptor.VarDecl:
                                WriteVarDecl(recs[0]);
                                break;

                            case Descriptor.DeclareFunction:
                                WriteFuncDecl(recs[0]);
                                break;

                            case Descriptor.OptionDecl:
                                WriteOptionDecl(recs[0]);
                                break;

                            default:
                                throw new InvalidOperationException();
                        }
                        WriteSeparator();
                    }
                }
            }
        }

        public virtual void WriteBoundarySpace(Notation.Record rec)
        {
            WriteText("declare boundary-space ");
            TokenWrapper wrapper = (TokenWrapper)rec.Arg0;
            switch (wrapper.Data)
            {
                case Token.PRESERVE:
                    WriteText("preserve");
                    break;

                case Token.STRIP:
                    WriteText("strip");
                    break;
            }
        }

        public virtual void WriteDefaultCollaction(Notation.Record rec)
        {
            WriteText("declare default collation ");
            WriteLiteral((Literal)rec.Arg0);
        }

        public virtual void WriteBaseUri(Notation.Record rec)
        {
            WriteText("declare base-uri ");
            WriteLiteral((Literal)rec.Arg0);
        }

        public virtual void WriteConstructionDecl(Notation.Record rec)
        {
            WriteText("declare construction ");
            TokenWrapper wrapper = (TokenWrapper)rec.Arg0;
            switch (wrapper.Data)
            {
                case Token.PRESERVE:
                    WriteText("preserve");
                    break;

                case Token.STRIP:
                    WriteText("strip");
                    break;
            }
        }

        public virtual void WriteOrdering(Notation.Record rec)
        {
            WriteText("declare ordering ");
            TokenWrapper wrapper = (TokenWrapper)rec.Arg0;
            switch (wrapper.Data)
            {
                case Token.ORDERED:
                    WriteText("ordered");
                    break;

                case Token.UNORDERED:
                    WriteText("unordered");
                    break;
            }
        }

        private void WriteDefaultOrder(Notation.Record rec)
        {
            WriteText("declare default order ");
            TokenWrapper wrapper = (TokenWrapper)rec.Arg0;
            switch (wrapper.Data)
            {
                case Token.EMPTY_GREATEST:
                    WriteText("empty greatest");
                    break;

                case Token.EMPTY_LEAST:
                    WriteText("empty least");
                    break;
            }
        }

        public virtual void WriteCopyNamespace(Notation.Record rec)
        {
            WriteText("declare copy-namespace ");
            TokenWrapper wrapper = (TokenWrapper)rec.Arg0;
            switch (wrapper.Data)
            {
                case Token.PRESERVE:
                    WriteText("preserve");
                    break;

                case Token.NO_PRESERVE:
                    WriteText("no_preserve");
                    break;
            }
            WriteText(", ");
            wrapper = (TokenWrapper)rec.Arg1;
            switch (wrapper.Data)
            {
                case Token.INHERIT:
                    WriteText("inherit");
                    break;

                case Token.NO_INHERIT:
                    WriteText("no_inherit");
                    break;
            }
        }

        public virtual void WriteImportSchema(Notation.Record rec)
        {
            WriteText("import schema ");
            if (rec.Arg0 != null)
            {
                Notation.Record[] recs = notation.Select(rec.Arg0, Descriptor.Namespace, 1);
                if (recs.Length > 0)
                {
                    WriteQName((Qname)recs[0].Arg0);
                    WriteText("=");
                }
                else
                {
                    recs = notation.Select(rec.Arg0, Descriptor.DefaultElement, 0);
                    if (recs.Length > 0)
                        WriteText("default element namespace ");
                    else
                        throw new InvalidOperationException();
                }
            }
            WriteLiteral((Literal)rec.Arg1);
            if (rec.args[2] != null)
            {
                WriteText(" at ");
                Literal[] arr = Lisp.ToArray<Literal>(rec.args[2]);
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteLiteral(arr[k]);
                }
            }
        }

        public virtual void WriteImportModule(Notation.Record rec)
        {
            Literal[] arr;
            WriteText("import module ");
            if (rec.args.Length == 2)
            {
                WriteLiteral((Literal)rec.Arg0);
                arr = Lisp.ToArray<Literal>(rec.args[1]);
            }
            else
            {
                WriteText("namespace ");
                WriteQName((Qname)rec.Arg0);
                WriteText('=');
                WriteLiteral((Literal)rec.Arg1);
                arr = Lisp.ToArray<Literal>(rec.args[2]);
            }
            WriteText(" at ");
            for (int k = 0; k < arr.Length; k++)
            {
                if (k > 0)
                    WriteText(", ");
                WriteLiteral(arr[k]);
            }
        }

        public virtual void WriteNamespaceDecl(Notation.Record rec)
        {
            WriteText("declare namespace ");
            WriteQName((Qname)rec.Arg0);
            WriteText('=');
            WriteLiteral((Literal)rec.Arg1);
        }

        public virtual void WriteDefaultNamespaceDecl(Notation.Record rec)
        {
            switch (rec.descriptor)
            {
                case Descriptor.DefaultElement:
                    WriteText("declare default element namespace ");
                    break;

                case Descriptor.DefaultFunction:
                    WriteText("declare default function namespace ");
                    break;
            }
            WriteLiteral((Literal)rec.Arg0);
        }

        public virtual void WriteVarDecl(Notation.Record rec)
        {
            WriteText("declare variable $");
            WriteVarName((VarName)rec.Arg0);
            if (rec.Arg1 != null)
            {
                WriteText(' ');
                WriteTypeDecl(rec.Arg1);
            }
            WriteText(" := ");
            if (rec.args.Length > 2)
                WriteExprSingle(rec.Arg2);
            else
                WriteText("external");
        }

        public virtual void WriteFuncDecl(Notation.Record rec)
        {
            WriteText("declare function ");
            WriteQName((Qname)rec.Arg0);
            WriteText('(');
            WriteParamList(rec.args[1]);
            WriteText(')');
            if (rec.args.Length > 3)
            {
                WriteText(" as ");
                WriteSequenceType(rec.Arg2);
                if (rec.args[3] == null)
                    WriteText(" external");
                else
                {
                    SmartNewLine();
                    WriteText('{');
                    SmartNewLine();
                    WriteExpr(rec.args[3]);
                    SmartNewLine();
                    WriteText('}');
                }
            }
            else
            {
                if (rec.args[2] == null)
                    WriteText(" external");
                else
                {
                    SmartNewLine();
                    WriteText('{');
                    SmartNewLine();
                    WriteExpr(rec.args[2]);
                    SmartNewLine();
                    WriteText('}');
                }
            }
        }

        public virtual void WriteParamList(object param)
        {
            if (param != null)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(param);
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteText('$');
                    WriteVarName((VarName)arr[k]);
                    Notation.Record[] recs = notation.Select(arr[k], Descriptor.TypeDecl, 1);
                    if (recs.Length > 0)
                    {
                        WriteText(' ');
                        WriteTypeDecl(recs[0].Arg0);
                    }
                }
            }
        }

        public virtual void WriteOptionDecl(Notation.Record rec)
        {
            WriteText("declare option ");
            WriteQName((Qname)rec.Arg0);
            WriteText(' ');
            WriteLiteral((Literal)rec.Arg1);
        }

        public virtual void WriteSeparator()
        {
            WriteText(';');
            SmartNewLine();
        }

        public virtual void WriteExpr(object expr)
        {
            Symbol[] arr = Lisp.ToArray<Symbol>(expr);
            for (int k = 0; k < arr.Length; k++)
            {
                if (k > 0)
                    WriteText(", ");
                WriteExprSingle(arr[k]);
            }
        }

        public virtual void WriteExprSingle(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym);
            switch (recs[0].descriptor)
            {
                case Descriptor.FLWORExpr:
                    WriteFLORExpr(recs[0]);
                    break;

                case Descriptor.Some:
                case Descriptor.Every:
                    WriteQuantifiedExpr(recs[0]);
                    break;

                case Descriptor.Typeswitch:
                    WriteTypeswitch(recs[0]);
                    break;

                case Descriptor.If:
                    WriteIfExpr(recs[0]);
                    break;

                default:
                    WriteOrExpr(sym);
                    break;
            }            
        }

        public virtual void WriteFLORExpr(Notation.Record rec)
        {
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[0]);
            for (int k = 0; k < arr.Length; k++)
            {
                if (k > 0)
                    WriteText(' ');
                Notation.Record[] recs = notation.Select(arr[k]);
                switch (recs[0].descriptor)
                {
                    case Descriptor.For:
                        WriteForOperator(recs[0]);
                        break;

                    case Descriptor.Let:
                        WriteLetOperator(recs[0]);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
            if (rec.Arg1 != null)
                WriteWhereClause(rec.Arg1);
            if (rec.Arg2 != null)
                WriteOrderByClause(rec.Arg2);
            WriteText(" return ");
            WriteExprSingle(rec.Arg3);
        }

        public virtual void WriteForOperator(Notation.Record rec)
        {
            WriteText("for ");
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[0]);
            for (int k = 0; k < arr.Length; k++)
            {
                Notation.Record[] recs = notation.Select(arr[k], Descriptor.ForClauseOperator, 4);
                if (recs.Length > 0)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteText('$');
                    WriteVarName((VarName)recs[0].Arg0);
                    if (recs[0].Arg1 != null)
                    {
                        WriteText(' ');
                        WriteTypeDecl(recs[0].Arg1);
                    }
                    if (recs[0].Arg2 != null)
                    {
                        WriteText(" at $");
                        WriteVarName((VarName)recs[0].Arg2);
                    }
                    WriteText(" in ");
                    WriteExprSingle(recs[0].Arg3);
                }
            }
        }

        public virtual void WriteLetOperator(Notation.Record rec)
        {
            WriteText("let ");
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[0]);
            for (int k = 0; k < arr.Length; k++)
            {
                Notation.Record[] recs = notation.Select(arr[k], Descriptor.LetClauseOperator, 3);
                if (recs.Length > 0)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteText('$');
                    WriteVarName((VarName)recs[0].Arg0);
                    if (recs[0].Arg1 != null)
                    {
                        WriteText(' ');
                        WriteTypeDecl(recs[0].Arg1);
                    }
                    WriteText(" := ");
                    WriteExprSingle(recs[0].Arg2);
                }
            }
        }

        public virtual void WriteWhereClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Where, 1);
            if (recs.Length > 0)
            {
                WriteText(" where ");
                WriteExprSingle(recs[0].Arg0);
            }
        }

        public virtual void WriteOrderByClause(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { 
                Descriptor.OrderBy, Descriptor.StableOrderBy });
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.OrderBy:
                        WriteText(" order by ");
                        break;

                    case Descriptor.StableOrderBy:
                        WriteText(" stable order by ");
                        break;
                }
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteExprSingle(arr[k]);
                    Notation.Record[] recs1 = notation.Select(arr[k], Descriptor.Modifier, 1);
                    if (recs1.Length > 0)
                    {                        
                        Symbol[] modifier = Lisp.ToArray<Symbol>(recs1[0].args[0]);
                        switch (((TokenWrapper)modifier[0]).Data)
                        {
                            case Token.ASCENDING:
                                WriteText(" ascending");
                                break;
                            case Token.DESCENDING:
                                WriteText(" descending");
                                break;
                        }
                        if (modifier[1] != null)
                            switch (((TokenWrapper)modifier[1]).Data)
                            {
                                case Token.EMPTY_GREATEST:
                                    WriteText(" empty greatest");
                                    break;
                                case Token.EMPTY_LEAST:
                                    WriteText(" empty least");
                                    break;
                            }
                        if (modifier[2] != null)
                        {
                            WriteText(" collation ");
                            WriteLiteral((Literal)modifier[2]);
                        }
                    }
                }
            }
        }

        public virtual void WriteQuantifiedExpr(Notation.Record rec)
        {
            switch (rec.descriptor)
            {
                case Descriptor.Some:
                    WriteText("some ");
                    break;
                case Descriptor.Every:
                    WriteText("every ");
                    break;
            }
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[0]);
            for (int k = 0; k < arr.Length; k++)
            {
                Notation.Record[] recs = notation.Select(arr[k], Descriptor.QuantifiedExprOper, 3);
                if (recs.Length > 0)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteText('$');
                    WriteVarName((VarName)recs[0].Arg0);
                    if (recs[0].Arg1 != null)
                    {
                        WriteText(' ');
                        WriteTypeDecl(recs[0].Arg1);
                    }
                    WriteText(" in ");
                    WriteExprSingle(recs[0].Arg2);
                }
            }
            WriteText(" satisfies ");
            WriteExprSingle(rec.Arg1);
        }

        public virtual void WriteTypeswitch(Notation.Record rec)
        {
            WriteText("typeswitch (");
            WriteExpr(rec.args[0]);
            WriteText(")");
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[1]);
            for (int k = 0; k < arr.Length; k++)
            {
                Notation.Record[] recs = notation.Select(arr[k], new Descriptor[] { Descriptor.Case });
                if (recs.Length > 0)
                {
                    WriteText(" case ");
                    if (recs[0].args.Length > 2)
                    {
                        WriteText('$');
                        WriteVarName((VarName)recs[0].Arg0);
                        WriteText(" as ");
                        WriteSequenceType(recs[0].Arg1);
                        WriteText(" return ");
                        WriteExprSingle(recs[0].Arg2);
                    }
                    else
                    {
                        WriteSequenceType(recs[0].Arg0);
                        WriteText(" return ");
                        WriteExprSingle(recs[0].Arg1);
                    }
                }
            }
            if (rec.args.Length > 3)
            {
                WriteText(" default $");
                WriteVarName((VarName)rec.Arg2);
                WriteText(" return ");
                WriteExprSingle(rec.Arg3);
            }
            else
            {
                WriteText(" default return ");
                WriteExprSingle(rec.Arg2);
            }
        }

        public virtual void WriteIfExpr(Notation.Record rec)
        {
            WriteText(" if (");
            WriteExpr(rec.args[0]);
            WriteText(") then ");
            WriteExprSingle(rec.Arg1);
            WriteText(" else ");
            WriteExprSingle(rec.Arg2);
        }

        public virtual void WriteOrExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Or, 2);
            if (recs.Length > 0)
            {
                WriteOrExpr(recs[0].Arg0);
                WriteText(" or ");
                WriteAndExpr(recs[0].Arg1);
            }
            else
                WriteAndExpr(sym);
        }

        public virtual void WriteAndExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.And, 2);
            if (recs.Length > 0)
            {
                WriteAndExpr(recs[0].Arg0);
                WriteText(" and ");
                WriteComparisonExpr(recs[0].Arg1);
            }
            else
                WriteComparisonExpr(sym);
        }

        public virtual void WriteComparisonExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.ValueComp, 
                Descriptor.GeneralComp, Descriptor.NodeComp }, 3);
            if (recs.Length > 0)
            {
                WriteRangeExpr(recs[0].Arg0);
                switch (recs[0].descriptor)
                {
                    case Descriptor.ValueComp:
                        {
                            TokenWrapper w = (TokenWrapper)recs[0].Arg1;
                            switch (w.Data)
                            {
                                case Token.EQ:
                                    WriteText(" eq ");
                                    break;

                                case Token.NE:
                                    WriteText(" ne ");
                                    break;

                                case Token.LT:
                                    WriteText(" lt ");
                                    break;

                                case Token.GT:
                                    WriteText(" gt ");
                                    break;

                                case Token.GE:
                                    WriteText(" ge ");
                                    break;
                            }
                        }
                        break;

                    case Descriptor.GeneralComp:
                        {
                            Literal lit = (Literal)recs[0].Arg1;
                            WriteText(lit.Data);
                        }
                        break;

                    case Descriptor.NodeComp:
                        {
                            if (recs[0].Arg1.Tag == Tag.TokenWrapper)
                            {
                                if (((TokenWrapper)recs[0].Arg1).Data == Token.IS)
                                    WriteText(" is ");
                            }
                            else
                            {
                                Literal lit = (Literal)recs[0].Arg1;
                                WriteText(lit.Data);
                            }
                        }
                        break;
                }
                WriteRangeExpr(recs[0].Arg2);
            }
            else
                WriteRangeExpr(sym);
        }

        public virtual void WriteRangeExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Range, 2);
            if (recs.Length > 0)
            {
                WriteAdditiveExpr(recs[0].Arg0);
                WriteText(" to ");
                WriteAdditiveExpr(recs[0].Arg1);
            }
            else
                WriteAdditiveExpr(sym);
        }

        public virtual void WriteAdditiveExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Add, 3);
            if (recs.Length > 0)
            {
                WriteAdditiveExpr(recs[0].Arg0);
                TokenWrapper w = (TokenWrapper)recs[0].Arg1;                
                WriteText((char)w.Data);
                WriteMultiplicativeExpr(recs[0].Arg2);
            }
            else
                WriteMultiplicativeExpr(sym);
        }

        public virtual void WriteMultiplicativeExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Mul, 3);
            if (recs.Length > 0)
            {
                WriteMultiplicativeExpr(recs[0].Arg0);
                TokenWrapper w = (TokenWrapper)recs[0].Arg1;
                switch (w.Data)
                {
                    case Token.ML:
                        WriteText('*');
                        break;

                    case Token.DIV:
                        WriteText(" div ");
                        break;

                    case Token.IDIV:
                        WriteText(" idiv ");
                        break;

                    case Token.MOD:
                        WriteText(" mod ");
                        break;
                }
                WriteUnionExpr(recs[0].Arg2);
            }
            else
                WriteUnionExpr(sym);
        }

        public virtual void WriteUnionExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, 
                new Descriptor[] { Descriptor.Union, Descriptor.Concatenate }, 2);
            if (recs.Length > 0)
            {
                WriteUnionExpr(recs[0].Arg0);
                switch (recs[0].descriptor)
                {
                    case Descriptor.Union:
                        WriteText(" union ");
                        break;

                    case Descriptor.Concatenate:
                        WriteText('|');
                        break;
                }
                WriteIntersectExceptExpr(recs[0].Arg1);
            }
            else
                WriteIntersectExceptExpr(sym);
        }

        public virtual void WriteIntersectExceptExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.IntersectExcept, 3);
            if (recs.Length > 0)
            {
                WriteIntersectExceptExpr(recs[0].Arg0);
                TokenWrapper w = (TokenWrapper)recs[0].Arg1;
                switch (w.Data)
                {
                    case Token.INTERSECT:
                        WriteText(" intersect ");
                        break;

                    case Token.EXCEPT:
                        WriteText(" except ");
                        break;
                }
                WriteInstanceofExpr(recs[0].Arg2);
            }
            else
                WriteInstanceofExpr(sym);
        }

        public virtual void WriteInstanceofExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.InstanceOf, 2);
            if (recs.Length > 0)
            {
                WriteTreatExpr(recs[0].Arg0);
                WriteText(" instance of ");
                WriteSequenceType(recs[0].Arg1);
            }
            else
                WriteTreatExpr(sym);
        }

        public virtual void WriteTreatExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.TreatAs, 2);
            if (recs.Length > 0)
            {
                WriteCastableExpr(recs[0].Arg0);
                WriteText(" treat as ");
                WriteSequenceType(recs[0].Arg1);
            }
            else
                WriteCastableExpr(sym);
        }

        public virtual void WriteCastableExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.CastableAs, 2);
            if (recs.Length > 0)
            {
                WriteCastExpr(recs[0].Arg0);
                WriteText(" castable as ");
                WriteSequenceType(recs[0].Arg1);
            }
            else
                WriteCastExpr(sym);
        }

        public virtual void WriteCastExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.CastAs, 2);
            if (recs.Length > 0)
            {
                WriteUnaryExpr(recs[0].Arg0);
                WriteText(" cast as ");
                WriteSequenceType(recs[0].Arg1);
            }
            else
                WriteUnaryExpr(sym);
        }

        public virtual void WriteUnaryExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Unary, 2);
            if (recs.Length > 0)
            {
                TokenWrapper[] arr = Lisp.ToArray<TokenWrapper>(recs[0].args[0]);
                foreach (TokenWrapper w in arr)
                    WriteText((char)w.Data);
                WriteValueExpr(recs[0].Arg1);
            }
            else
                WriteValueExpr(sym);
        }

        public virtual void WriteValueExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym);
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.ExtensionExpr:
                        WriteExtensionExpr(recs[0]);
                        break;

                    case Descriptor.Validate:
                        WriteValidateExpr(recs[0]);
                        break;

                    default:
                        WritePathExpr(sym);
                        break;
                }
            }
        }

        public virtual void WriteExtensionExpr(Notation.Record rec)
        {
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[0]);
            foreach (Symbol sym in arr)
            {
                Notation.Record[] recs = notation.Select(sym, Descriptor.Pragma, 2);
                if (recs.Length > 0)
                {
                    WriteText("(#");
                    WriteQName((Qname)recs[0].Arg0);
                    WriteText(' ');
                    Literal lit = (Literal)recs[0].Arg1;
                    WriteText(lit.Data);
                    WriteText("#)");
                }
            }            
            WriteText('{');
            SmartNewLine();
            WriteExpr(rec.args[1]);
            SmartNewLine();
            WriteText('}');
            SmartNewLine();
        }

        public virtual void WriteValidateExpr(Notation.Record rec)
        {
            WriteText("validate ");
            if (rec.Arg0 != null)
            {
                TokenWrapper w = (TokenWrapper)rec.Arg0;
                switch (w.Data)
                {
                    case Token.STRICT:
                        WriteText("strict ");
                        break;

                    case Token.LAX:
                        WriteText("lax ");
                        break;        
                }
            }
            WriteText('{');
            WriteExpr(rec.args[1]);
            SmartNewLine();
            WriteText('}');
            SmartNewLine();
        }

        public virtual void WritePathExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Child, 
                Descriptor.Descendant }, 1);
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.Child:
                        WriteText('/');
                        if (recs[0].Arg0 != null)
                            WriteRelativePathExpr(recs[0].Arg0);
                        break;

                    case Descriptor.Descendant:
                        WriteText("//");
                        WriteRelativePathExpr(recs[0].Arg0);
                        break;
                }
            }
            else
                WriteRelativePathExpr(sym);
        }

        public virtual void WriteRelativePathExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Child, 
                Descriptor.Descendant }, 2);
            if (recs.Length > 0)
            {
                WriteRelativePathExpr(recs[0].Arg0);
                switch (recs[0].descriptor)
                {
                    case Descriptor.Child:
                        WriteText('/');
                        break;

                    case Descriptor.Descendant:
                        WriteText("//");
                        break;
                }
                WriteStepExpr(recs[0].Arg1);
            }
            else
                WriteStepExpr(sym);
        }

        public virtual void WriteStepExpr(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.AxisStep, 
                Descriptor.FilterExpr }, 1);
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.AxisStep:
                        WriteAxisStep(recs[0]);
                        break;

                    case Descriptor.FilterExpr:
                        WriteFilterExpr(recs[0]);
                        break;
                }
            }
            else
                throw new InvalidOperationException();
        }

        public virtual void WriteAxisStep(Notation.Record rec)
        {
            if (rec.Arg0.Tag == Tag.TokenWrapper && ((TokenWrapper)rec.Arg0).Data == Token.DOUBLE_PERIOD)
                WriteText("..");
            else
            {
                Notation.Record[] recs = notation.Select(rec.Arg0, new Descriptor[] { Descriptor.ForwardStep,
                    Descriptor.AbbrevForward, Descriptor.ReverseStep });
                if (recs.Length > 0)
                {
                    switch (recs[0].descriptor)
                    {
                        case Descriptor.ForwardStep:
                            WriteForwardStep(recs[0]);
                            break;

                        case Descriptor.AbbrevForward:
                            WriteAbbrevForward(recs[0]);
                            break;

                        case Descriptor.ReverseStep:
                            WriteReverseStep(recs[0]);
                            break;
                    }                    
                }
                else
                    WriteNodeTest(rec.Arg0);
                WritePredicateList(rec.Arg0);
            }
        }

        public virtual void WriteForwardStep(Notation.Record rec)
        {
            TokenWrapper w = (TokenWrapper)rec.Arg0;
            switch (w.Data)
            {
                case Token.AXIS_CHILD:
                    WriteText("child::");
                    break;

                case Token.AXIS_DESCENDANT:
                    WriteText("descendant::");
                    break;

                case Token.AXIS_ATTRIBUTE:
                    WriteText("attribute::");
                    break;

                case Token.AXIS_SELF:
                    WriteText("self::");
                    break;

                case Token.AXIS_DESCENDANT_OR_SELF:
                    WriteText("descendant-or-self::");
                    break;

                case Token.AXIS_FOLLOWING_SIBLING:
                    WriteText("following-sibling::");
                    break;

                case Token.AXIS_FOLLOWING:
                    WriteText("following::");
                    break;

                case Token.AXIS_NAMESPACE:
                    WriteText("namespace::");
                    break;
            }
            WriteNodeTest(rec.Arg1);
        }

        public virtual void WriteAbbrevForward(Notation.Record rec)
        {
            WriteText('@');
            WriteNodeTest(rec.Arg0);
        }

        public virtual void WriteReverseStep(Notation.Record rec)
        {
            TokenWrapper w = (TokenWrapper)rec.Arg0;
            switch (w.Data)
            {
                case Token.AXIS_PARENT:
                    WriteText("parent::");
                    break;

                case Token.AXIS_ANCESTOR:
                    WriteText("ancestor::");
                    break;

                case Token.AXIS_PRECEDING_SIBLING:
                    WriteText("preceding-sibling::");
                    break;

                case Token.AXIS_PRECEDING:
                    WriteText("preceding::");
                    break;

                case Token.AXIS_ANCESTOR_OR_SELF:
                    WriteText("ancestor-or-self::");
                    break;
            }
            WriteNodeTest(rec.Arg1);
        }

        public virtual void WriteNodeTest(Symbol sym)
        {
            if (sym.Tag == Tag.TokenWrapper)
                WriteText('*');
            else if (sym.Tag == Tag.Qname)
                WriteQName((Qname)sym);
            else
            {
                Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Wildcard1, Descriptor.Wildcard2 }, 1);
                if (recs.Length > 0)
                {
                    switch (recs[0].descriptor)
                    {
                        case Descriptor.Wildcard1:
                            WriteQName((Qname)recs[0].Arg0);
                            WriteText(":*");
                            break;

                        case Descriptor.Wildcard2:
                            WriteText("*:");
                            WriteQName((Qname)recs[0].Arg0);
                            break;
                    }
                }
                else
                    WriteKindTest(sym);
            }
        }

        public virtual void WritePredicateList(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.PredicateList, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                foreach (Symbol expr in arr)
                {
                    Notation.Record[] recs1 = notation.Select(expr, Descriptor.Predicate, 1);
                    if (recs1.Length > 0)
                    {
                        WriteText('[');
                        WriteExpr(recs1[0].args[0]);
                        WriteText(']');
                    }
                }
            }
        }        

        public virtual void WriteFilterExpr(Notation.Record rec)
        {
            WritePrimaryExpr(rec.Arg0);
            WritePredicateList(rec.Arg0);
        }

        public virtual void WritePrimaryExpr(Symbol sym)
        {
            if (sym.Tag == Tag.Literal)
                WriteLiteral((Literal)sym);
            else if (sym.Tag == Tag.Integer)
                WriteInteger((IntegerValue)sym);
            else if (sym.Tag == Tag.Double)
                WriteDecimal((DoublelValue)sym);
            else if (sym.Tag == Tag.VarName)
            {
                WriteText('$');
                WriteVarName((VarName)sym);
            }
            else if (sym.Tag == Tag.TokenWrapper)
            {
                if (((TokenWrapper)sym).Data == '.')
                    WriteText('.');
            }
            else
            {
                Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.ParenthesizedExpr,
                    Descriptor.Ordered, Descriptor.Unordered, Descriptor.Funcall });
                if (recs.Length > 0)
                {
                    switch (recs[0].descriptor)
                    {
                        case Descriptor.ParenthesizedExpr:
                            WriteParenthesizedExpr(recs[0]);
                            break;

                        case Descriptor.Ordered:
                        case Descriptor.Unordered:
                            WriteOrderedExpr(recs[0]);
                            break;

                        case Descriptor.Funcall:
                            WriteFuncallExpr(recs[0]);
                            break;
                    }
                }
                else
                    WriteDirectConstructor(sym);
            }
        }

        public virtual void WriteParenthesizedExpr(Notation.Record rec)
        {
            WriteText('(');
            if (rec.args[0] != null)
                WriteExpr(rec.args[0]);
            WriteText(')');
        }

        public virtual void WriteOrderedExpr(Notation.Record rec)
        {
            switch (rec.descriptor)
            {
                case Descriptor.Ordered:
                    WriteText("ordered ");
                    break;

                case Descriptor.Unordered:
                    WriteText("unordered ");
                    break;
            }
            SmartNewLine();
            WriteText('{');
            SmartNewLine();
            WriteExpr(rec.args[0]);
            SmartNewLine();
            WriteText('}');
            SmartNewLine();
        }

        public virtual void WriteFuncallExpr(Notation.Record rec)
        {
            WriteQName((Qname)rec.Arg0);
            WriteText('(');
            if (rec.args[1] != null)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[1]);
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0)
                        WriteText(", ");
                    WriteExprSingle(arr[k]);
                }
            }
            WriteText(')');
        }

        public virtual void WriteDirectConstructor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.DirElemConstructor,
                Descriptor.DirCommentConstructor, Descriptor.DirPIConstructor });
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.DirElemConstructor:
                        WriteDirElemConstructor(recs[0]);
                        break;

                    case Descriptor.DirCommentConstructor:
                        WriteText("<!--");
                        WriteText(((Literal)recs[0].Arg0).Data);
                        WriteText("-->");
                        break;

                    case Descriptor.DirPIConstructor:
                        WriteText("<?");
                        WriteText(((Literal)recs[0].Arg0).Data);
                        if (recs[0].Arg1 != null)
                        {
                            WriteText(' ');
                            WriteText(((Literal)recs[0].Arg1).Data);
                        }
                        WriteText("?>");
                        break;
                }
            }
            else
                WriteComputedConstructor(sym);
        }

        public virtual void WriteDirElemConstructor(Notation.Record rec)
        {
            WriteText('<');
            WriteQName((Qname)rec.Arg0);
            if (rec.args[1] != null)
                foreach (Symbol sym in Lisp.getIterator<Symbol>(rec.args[1]))
                {
                    if (sym.Tag == Tag.Literal)
                        WriteText(((Literal)sym).Data);
                    else
                    {
                        Notation.Record[] recs = notation.Select(sym, Descriptor.DirAttribute, 5);
                        if (recs.Length > 0)
                        {
                            WriteQName((Qname)recs[0].Arg0);
                            if (recs[0].Arg1 != null)
                                WriteText(((Literal)recs[0].Arg1).Data);
                            WriteText('=');
                            if (recs[0].Arg2 != null)
                                WriteText(((Literal)recs[0].Arg2).Data);
                            string quote = ((Literal)recs[0].Arg3).Data;
                            WriteText(quote);
                            WriteDirAttributeValue(recs[0].args[4]);
                            WriteText(quote);
                        }
                    }
                }
            if (rec.args.Length > 2)
            {
                WriteText('>');
                if (rec.args[2] != null)
                    foreach (Symbol sym in Lisp.getIterator<Symbol>(rec.args[2]))
                        WriteDirElemContent(sym);
                WriteText("</");
                WriteQName((Qname)rec.Arg3);
                if (rec.args[4] != null)
                    WriteText(((Literal)rec.args[4]).Data);
                WriteText('>');
            }
            else
                WriteText("/>");
        }

        public virtual void WriteDirAttributeValue(object o)
        {
            foreach (Symbol sym in Lisp.getIterator<Symbol>(o))
                if (sym.Tag == Tag.TokenWrapper)
                {
                    TokenWrapper w = (TokenWrapper)sym;
                    switch (w.Data)
                    {
                        case Token.EscapeApos:
                            WriteText("\'\'");
                            break;

                        case Token.EscapeQuot:
                            WriteText("\"\"");
                            break;
                    }
                }
                else
                    WriteCommonContent(sym);
        }

        public virtual void WriteDirElemContent(Symbol sym)
        {
            if (sym.Tag == Tag.Literal)
                WriteText(((Literal)sym).Data);
            else if (sym.Tag == Tag.Constructor)
                WriteDirectConstructor(sym);
            else 
            {
                Notation.Record[] recs = notation.Select(sym, Descriptor.CDataSection, 1);
                if (recs.Length > 0)
                {
                    WriteText("<![CDATA[");
                    WriteText(((Literal)recs[0].Arg0).Data);
                    WriteText("]]>");
                }
                else
                    WriteCommonContent(sym);
            }                
        }

        public virtual void WriteCommonContent(Symbol sym)
        {
            if (sym.Tag == Tag.Literal)
                WriteText(((Literal)sym).Data);
            else if (sym.Tag == Tag.PredefinedEntityRef)
                WriteText(((PredefinedEntityRef)sym).Data);
            else if (sym.Tag == Tag.CharRef)
            {
                if (sym is CharRefHex)
                {
                    CharRefHex charRef = (CharRefHex)sym;
                    WriteText(String.Format("&x{0};", charRef.Data));
                }
                else
                {
                    CharRef charRef = (CharRef)sym;
                    WriteText(String.Format("&{0};", charRef.Data));
                }
            }
            else
            {
                Notation.Record[] recs = notation.Select(sym, Descriptor.EnclosedExpr, 1);
                if (recs.Length > 0)
                {
                    WriteText('{');
                    WriteExpr(recs[0].args[0]);
                    WriteText('}');
                }
                else
                    throw new InvalidOperationException();
            }
        }

        public virtual void WriteComputedConstructor(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.CompDocConstructor,
                Descriptor.CompElemConstructor, Descriptor.CompAttrConstructor, Descriptor.CompTextConstructor,
                Descriptor.CompCommentConstructor, Descriptor.CompPIConstructor });
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.CompDocConstructor:
                        WriteText("document {");
                        SmartNewLine();
                        WriteExpr(recs[0].args[0]);
                        SmartNewLine();
                        WriteText('}');
                        SmartNewLine();
                        break;

                    case Descriptor.CompElemConstructor:
                        WriteText("element ");
                        if (recs[0].args[0] is Qname)
                            WriteQName((Qname)recs[0].args[0]);
                        else
                        {
                            WriteText("{");
                            WriteExpr(recs[0].args[0]);
                            WriteText("}");
                        }
                        SmartNewLine();
                        WriteText("{");
                        if (recs[0].args[1] != null)
                        {
                            SmartNewLine();
                            WriteExpr(recs[0].args[1]);
                            SmartNewLine();
                        }
                        WriteText('}');
                        SmartNewLine();
                        break;

                    case Descriptor.CompAttrConstructor:
                        WriteText("attribute ");
                        if (recs[0].args[0] is Qname)
                            WriteQName((Qname)recs[0].args[0]);
                        else
                        {
                            SmartNewLine();
                            WriteText("{");
                            WriteExpr(recs[0].args[0]);
                            WriteText("}");
                        }
                        SmartNewLine();
                        WriteText("{");
                        if (recs[0].args[1] != null)
                        {
                            SmartNewLine();
                            WriteExpr(recs[0].args[1]);
                            SmartNewLine();
                        }
                        WriteText('}');
                        SmartNewLine();
                        break;

                    case Descriptor.CompTextConstructor:
                        WriteText("text {");
                        WriteExpr(recs[0].args[0]);                        
                        WriteText('}');
                        SmartNewLine();
                        break;

                    case Descriptor.CompCommentConstructor:
                        WriteText("comment {");
                        WriteExpr(recs[0].args[0]);
                        WriteText('}');
                        SmartNewLine();
                        break;

                    case Descriptor.CompPIConstructor:
                        WriteText("processing-instruction ");
                        if (recs[0].args[0] is Qname)
                            WriteQName((Qname)recs[0].args[0]);
                        else
                        {
                            WriteText("{");
                            WriteExpr(recs[0].args[0]);
                            WriteText("}");
                        }
                        SmartNewLine();
                        WriteText("{");
                        if (recs[0].args[1] != null)
                        {
                            SmartNewLine();
                            WriteExpr(recs[0].args[1]);
                            SmartNewLine();
                        }
                        WriteText('}');
                        SmartNewLine();
                        break;
                }
            }
            else
                throw new InvalidOperationException();
        }

        public virtual void WriteTypeDecl(Symbol sym)
        {
            WriteText(" as ");
            WriteSequenceType(sym);
        }

        public virtual void WriteSequenceType(Symbol sym)
        {
            if (sym.Tag == Tag.TokenWrapper && 
                ((TokenWrapper)sym).Data == Token.VOID)
                WriteText("void()");
            else
            {
                WriteItemType(sym);
                Notation.Record[] recs = notation.Select(sym, Descriptor.Occurrence, 1);
                if (recs.Length > 0)
                {
                    TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                    switch (w.Data)
                    {
                        case Token.Indicator1:
                            WriteText("* ");
                            break;

                        case Token.Indicator2:
                            WriteText("+ ");
                            break;

                        case Token.Indicator3:
                            WriteText("? ");
                            break;
                    }
                }
            }
        }

        public virtual void WriteItemType(Symbol sym)
        {
            if (sym.Tag == Tag.TokenWrapper)
            {
                TokenWrapper w = (TokenWrapper)sym;
                if (w.Data == Token.ITEM)
                    WriteText("item()");
            }
            else if (sym.Tag == Tag.Qname)
                WriteTypeName(sym);
            else
                WriteKindTest(sym);
        }

        public virtual void WriteKindTest(Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.KindTest, 1);
            if (recs.Length > 0)
            {
                if (recs[0].Arg0.Tag == Tag.TokenWrapper)
                {
                    TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                    switch (w.Data)
                    {
                        case Token.NODE:
                            WriteText("node()");
                            break;

                        case Token.TEXT:
                            WriteText("text()");
                            break;

                        case Token.COMMENT:
                            WriteText("comment()");
                            break;

                        case Token.ELEMENT:
                            WriteText("element()");
                            break;

                        case Token.ATTRIBUTE:
                            WriteText("attribute()");
                            break;

                        case Token.DOCUMENT_NODE:
                            WriteText("document-node()");
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                {
                    Notation.Record[] recs1 = notation.Select(recs[0].Arg0);
                    switch (recs1[0].descriptor)
                    {
                        case Descriptor.DocumentNode:
                            WriteDocumentTest(recs1[0]);
                            break;

                        case Descriptor.ProcessingInstruction:
                            WritePITest(recs1[0]);
                            break;

                        case Descriptor.Element:
                            WriteElementTest(recs1[0]);
                            break;

                        case Descriptor.Attribute:
                            WriteAttributeTest(recs1[0]);
                            break;

                        case Descriptor.SchemaElement:
                            WriteSchemaElementTest(recs1[0]);
                            break;

                        case Descriptor.SchemaAttribute:
                            WriteSchemaAttributeTest(recs1[0]);
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
        }

        public virtual void WriteDocumentTest(Notation.Record rec)
        {            
            Notation.Record[] recs = notation.Select(rec.Arg0);
            if (recs.Length > 0)
            {
                WriteText("document-node(");
                switch (recs[0].descriptor)
                {
                    case Descriptor.Element:
                        WriteElementTest(recs[0]);
                        break;

                    case Descriptor.SchemaElement:
                        WriteSchemaElementTest(recs[0]);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
                WriteText(")");
            }
        }

        public virtual void WritePITest(Notation.Record rec)
        {
            WriteText("processing-instruction(");
            if (rec.Arg0.Tag == Tag.Qname)
                WriteQName((Qname)rec.Arg0);
            else if (rec.Arg0.Tag == Tag.Literal)
                WriteLiteral((Literal)rec.Arg0);
            else
                throw new InvalidOperationException();
            WriteText(")");
        }

        public virtual void WriteElementTest(Notation.Record rec)
        {
            WriteText("element(");
            if (rec.Arg0.Tag == Tag.TokenWrapper)
            {
                if (((TokenWrapper)rec.Arg0).Data == '*')
                    WriteText('*');
            }
            else
                WriteQName((Qname)rec.Arg0);
            if (rec.args.Length > 1)
            {
                WriteText(", ");
                WriteTypeName(rec.Arg1);
                if (rec.args.Length > 2)
                {
                    if (rec.Arg2.Tag == Tag.TokenWrapper)
                    {
                        if (((TokenWrapper)rec.Arg2).Data == '?')
                            WriteText('?');
                    }
                }
            }
            WriteText(")");
        }

        public virtual void WriteAttributeTest(Notation.Record rec)
        {
            WriteText("attribute(");
            if (rec.Arg0.Tag == Tag.TokenWrapper)
            {
                if (((TokenWrapper)rec.Arg0).Data == '*')
                    WriteText('*');
            }
            else
                WriteQName((Qname)rec.Arg0);
            if (rec.args.Length > 1)
            {
                WriteText(", ");
                WriteTypeName(rec.Arg1);
            }
            WriteText(")");
        }

        public virtual void WriteSchemaElementTest(Notation.Record rec)
        {
            WriteText("schema-element(");
            WriteQName((Qname)rec.Arg0);
            WriteText(")");
        }

        public virtual void WriteSchemaAttributeTest(Notation.Record rec)
        {
            WriteText("schema-attribute(");
            WriteQName((Qname)rec.Arg0);
            WriteText(")");
        }

        public virtual void WriteTypeName(Symbol sym)
        {
            WriteQName((Qname)sym);
        }

        public virtual void WriteDecimal(DoublelValue decimalValue)
        {
            WriteText(decimalValue.Data.ToString(CultureInfo.InvariantCulture));
        }

        public virtual void WriteInteger(IntegerValue integerValue)
        {
            WriteText(integerValue.Data.ToString());
        }

        public virtual void WriteLiteral(Literal lit)
        {
            if (lit.Quote != 0)
                WriteText(lit.Quote);
            WriteText(lit.Data);
            if (lit.Quote != 0)
                WriteText(lit.Quote);
        }

        public virtual void WriteQName(Qname n)
        {
            WriteText(n.Name);
        }

        public virtual void WriteVarName(VarName n)
        {
            WriteText(n.Name);
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
