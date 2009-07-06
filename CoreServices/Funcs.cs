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

namespace DataEngine.CoreServices
{
    public partial class Funcs
    {
        public static readonly object Invoke = Lisp.Defatom("invoke");
        public static readonly object Eval = Lisp.Defatom("eval");
        public static readonly object Cond = Lisp.Defatom("cond");
        public static readonly object Weak = Lisp.Defatom("weak");
        public static readonly object Progn = Lisp.Defatom("progn");
        public static readonly object Prog1 = Lisp.Defatom("prog1");
        public static readonly object Cast = Lisp.Defatom("cast");
        public static readonly object If = Lisp.Defatom("if");

        public static readonly object Let1 = Lisp.Defatom("let");
        public static readonly object Let2 = Lisp.Defatom("let*");
        public static readonly object Rest = Lisp.Defatom("&rest");
        public static readonly object Trap = Lisp.Defatom("trap");

        public static readonly object Add = Lisp.Defatom("+");
        public static readonly object Sub = Lisp.Defatom("-");
        public static readonly object Mul = Lisp.Defatom("*");
        public static readonly object Div = Lisp.Defatom("/");

        public static readonly object Eq = Lisp.Defatom("eq");
        public static readonly object Ne = Lisp.Defatom("ne");
        public static readonly object Lt = Lisp.Defatom("lt");
        public static readonly object Gt = Lisp.Defatom("gt");
        public static readonly object Ge = Lisp.Defatom("ge");
        public static readonly object Le = Lisp.Defatom("le");

        public static readonly object And = Lisp.Defatom("and");
        public static readonly object Or = Lisp.Defatom("or");
        public static readonly object Not = Lisp.Defatom("not");

        public static readonly object Car = Lisp.Defatom("car");
        public static readonly object Cdr = Lisp.Defatom("cdr");
        public static readonly object Cons = Lisp.Defatom("cons");
        public static readonly object ListP = Lisp.Defatom("listp");
        public static readonly object List = Lisp.Defatom("list");
        public static readonly object Null = Lisp.Defatom("null");
        public static readonly object Atom = Lisp.Defatom("atom");
        public static readonly object Nth = Lisp.Defatom("nth");
        public static readonly object NthCdr = Lisp.Defatom("nthcdr");
        public static readonly object Append = Lisp.Defatom("append");
        public static readonly object Reverse = Lisp.Defatom("reverse");
        public static readonly object Last = Lisp.Defatom("last");
        public static readonly object LastCdr = Lisp.Defatom("lastcdr");
        public static readonly object EqualP = Lisp.Defatom("equalp");
        public static readonly object Length = Lisp.Defatom("length");

        public static readonly object First = Lisp.Defatom("first");
        public static readonly object Second = Lisp.Defatom("second");
        public static readonly object Third = Lisp.Defatom("third");
        public static readonly object Fourth = Lisp.Defatom("fourth");
        public static readonly object Fifth = Lisp.Defatom("fifth");
        public static readonly object Sixth = Lisp.Defatom("sixth");
        public static readonly object Seventh = Lisp.Defatom("seventh");
        public static readonly object Eighth = Lisp.Defatom("eighth");
        public static readonly object Ninth = Lisp.Defatom("ninth");
        public static readonly object Tenth = Lisp.Defatom("tenth");

        new public static readonly object ToString = Lisp.Defatom("$string");

        public static readonly object ToDateTime = Lisp.Defatom("$dateTime");
        public static readonly object ToDecimal = Lisp.Defatom("$decimal");
        public static readonly object ToDouble = Lisp.Defatom("$double");
        public static readonly object ToSingle = Lisp.Defatom("$float");
        public static readonly object ToInt16 = Lisp.Defatom("$short");
        public static readonly object ToInt32 = Lisp.Defatom("$int");
        public static readonly object ToInt64 = Lisp.Defatom("$long");

        public static readonly object FirstDate = Lisp.Defatom("$fdate");
        public static readonly object GetDay = Lisp.Defatom("$get_day");
        public static readonly object GetMonth = Lisp.Defatom("$get_month");
        public static readonly object GetYear = Lisp.Defatom("$get_year");
    }
}
