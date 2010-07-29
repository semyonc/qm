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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DataEngine.CoreServices
{
    class LToken
    {
        public const int LBRA = 1;
        public const int RBRA = 2;
        public const int PERIOD = 3;
        public const int QUOTE = 4;
        public const int NUMBER = 5;
        public const int TEXT = 6;
        public const int ATOM = 7;
        public const int ERROR = -1;
    }

    public class LispParser
    {
        public static object Parse(string expr)
        {
            LispTokenizer tok = new LispTokenizer(expr);
            object result = DoParse(tok, tok.token());
            if (tok.token() != 0)
                throw new ImproperlyFormat();
            return result;
        }

        private static object DoParse(LispTokenizer tok, int tk)
        {
            switch (tk)
            {
                case LToken.LBRA:
                    return ParseList(tok);

                case LToken.RBRA:
                    throw new ImproperlyFormat();

                case LToken.QUOTE:
                    return Lisp.List(Lisp.QUOTE, DoParse(tok, tok.token()));

                case LToken.ERROR:
                    throw new ImproperlyFormat();

                default:
                    return MakeValue(tk, tok.value());
            }
        }

        private static object ParseList(LispTokenizer tok)
        {
            int tk;
            object result = null;
            if (tok.peek() != LToken.LBRA)
                throw new ImproperlyFormat();
            while ((tk = tok.token()) != LToken.RBRA)
            {
                if (tk == 0)
                    throw new ImproperlyFormat();
                result = Lisp.Append(result, Lisp.Cons(DoParse(tok, tk)));
            }
            return result;
        }

        private static object MakeValue(int token, object value)
        {
            if (token == LToken.ATOM)
                return ATOM.Create(null, new string[]{(string)value}, false);            
            return value;
        }
    }
}
