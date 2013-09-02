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
        public static readonly object Invoke = ATOM.Create("invoke");
        public static readonly object Eval = ATOM.Create("eval");
        public static readonly object Cond = ATOM.Create("cond");
        public static readonly object Weak = ATOM.Create("weak");
        public static readonly object Progn = ATOM.Create("progn");
        public static readonly object Prog1 = ATOM.Create("prog1");
        public static readonly object Cast = ATOM.Create("cast");
        public static readonly object If = ATOM.Create("if");
        public static readonly object LambdaQuote = ATOM.Create("lambda-qoute");
        public static readonly object ForEach = ATOM.Create("for-each");

        public static readonly object Let1 = ATOM.Create("let");
        public static readonly object Let2 = ATOM.Create("let*");
        public static readonly object Rest = ATOM.Create("&rest");
        public static readonly object Trap = ATOM.Create("trap");

        public static readonly object Add = ATOM.Create("+");
        public static readonly object Sub = ATOM.Create("-");
        public static readonly object Mul = ATOM.Create("*");
        public static readonly object Div = ATOM.Create("/");
        public static readonly object Neg = ATOM.Create("neg");
        public static readonly object Mod = ATOM.Create("mod");
        public static readonly object IDiv = ATOM.Create("idiv");

        public static readonly object Eq = ATOM.Create("eq");
        public static readonly object Ne = ATOM.Create("ne");
        public static readonly object Lt = ATOM.Create("lt");
        public static readonly object Gt = ATOM.Create("gt");
        public static readonly object Ge = ATOM.Create("ge");
        public static readonly object Le = ATOM.Create("le");

        public static readonly object And = ATOM.Create("and");
        public static readonly object Or = ATOM.Create("or");
        public static readonly object Not = ATOM.Create("not");

        public static readonly object Car = ATOM.Create("car");
        public static readonly object Cdr = ATOM.Create("cdr");
        public static readonly object Cons = ATOM.Create("cons");
        public static readonly object ListP = ATOM.Create("listp");
        public static readonly object List = ATOM.Create("list");
        public static readonly object Null = ATOM.Create("null");
        public static readonly object Atom = ATOM.Create("atom");
        public static readonly object Nth = ATOM.Create("nth");
        public static readonly object NthCdr = ATOM.Create("nthcdr");
        public static readonly object Append = ATOM.Create("append");
        public static readonly object Reverse = ATOM.Create("reverse");
        public static readonly object Last = ATOM.Create("last");
        public static readonly object LastCdr = ATOM.Create("lastcdr");
        public static readonly object EqualP = ATOM.Create("equalp");
        public static readonly object Length = ATOM.Create("length");

        public static readonly object First = ATOM.Create("first");
        public static readonly object Second = ATOM.Create("second");
        public static readonly object Third = ATOM.Create("third");
        public static readonly object Fourth = ATOM.Create("fourth");
        public static readonly object Fifth = ATOM.Create("fifth");
        public static readonly object Sixth = ATOM.Create("sixth");
        public static readonly object Seventh = ATOM.Create("seventh");
        public static readonly object Eighth = ATOM.Create("eighth");
        public static readonly object Ninth = ATOM.Create("ninth");
        public static readonly object Tenth = ATOM.Create("tenth");

        new public static readonly object ToString = ATOM.Create("$string");

        public static readonly object ToDateTime = ATOM.Create("$dateTime");
        public static readonly object ToDecimal = ATOM.Create("$decimal");
        public static readonly object ToDouble = ATOM.Create("$double");
        public static readonly object ToSingle = ATOM.Create("$float");
        public static readonly object ToInt16 = ATOM.Create("$short");
        public static readonly object ToInt32 = ATOM.Create("$int");
        public static readonly object ToInt64 = ATOM.Create("$long");

        public static readonly object FirstDate = ATOM.Create("$fdate");
        public static readonly object GetDay = ATOM.Create("$get_day");
        public static readonly object GetMonth = ATOM.Create("$get_month");
        public static readonly object GetYear = ATOM.Create("$get_year");
    }
}
