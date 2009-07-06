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
using System.Globalization;

namespace DataEngine.CoreServices
{
    class LispTokenizer
    {
        private char[] Buffer;
        private char current;
        private int pos;
        private int lineNo;
        private int colNo;

        private int m_token;
        private object val = null;

        private StringBuilder sb = new StringBuilder();

        public LispTokenizer(string strInput)
		{
            Buffer = strInput.ToCharArray();
            lineNo = colNo = 1;
            pos = 0;
            NextChar();
        }

		public char NextChar()
		{
			if(pos < Buffer.Length)
				current = Buffer[pos++];
			else
			{
				current = '\0';
				pos = Buffer.Length + 1;
			}
			if (current == '\n')
			{
				lineNo++;
				colNo = 1;
			}
			else
				colNo++;
			return current;
		}

		public char PeekChar(int index)
		{
			if (pos + index -1 < Buffer.Length)
				return Buffer[pos + index -1];
			else
				return '\0';
		}

		public char PeekChar()
		{
			return PeekChar(0);
		}

		public void Skip(int c)
		{
			while (c-- > 0)
				NextChar();
		}

		public bool MatchText(string text)
		{
			for (int i = 0; i < text.Length; i++)
				if (PeekChar(i) != text[i])
					return false;
			Skip(text.Length);
			return true;
		}

        public int is_sign(LispTokenizer tok, char c, out string val, out bool do_read)
        {
            do_read = false;
            val = Convert.ToString(c);

            switch (c)
            {
                case '(':
                    return LToken.LBRA;

                case ')':
                    return LToken.RBRA;

                case '.':
                    return LToken.PERIOD;

                case '\'':
                    return LToken.QUOTE;

                default:
                    return LToken.ERROR;
            }
        }

        public bool is_identifier_start_character(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || Char.IsLetter(c) ||
                (c == '+') || (c == '-') || (c == '*') || (c == '/') || (c == '%') || (c == '@') || (c == '$') || (c == '&') || (c == '?') || (c == '#');
        }

        public bool is_identifier_part_character(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || (c >= '0' && c <= '9') || Char.IsLetter(c) ||
                (c == '+') || (c == '-') || (c == '*') || (c == '/') || (c == '%') || (c == '@') || (c == '$') || (c == '&') || (c == '?') || (c == ':') || (c == '#');
        }

        int is_punct(char c, out bool do_read)
        {

            string sval;
            int result = is_sign(this, c, out sval, out do_read);
            val = sval;
            return result;
        }

        private int is_number(char c)
        {
            sb.Length = 0;

            while (Char.IsDigit(c))
            {
                sb.Append(c);
                c = NextChar();
            }

            if (c == '.')
            {
                if (Char.IsDigit(PeekChar(1)))
                {
                    sb.Append(c);
                    c = NextChar();
                    while (Char.IsDigit(c))
                    {
                        sb.Append(c);
                        c = NextChar();
                    }
                }
                else
                    goto return_result;
            }

            if (c == 'e' || c == 'E')
            {
                sb.Append(c);

                c = NextChar();
                if (c == '+' || c == '-')
                {
                    sb.Append(c);
                    c = NextChar();
                }

                while (Char.IsDigit(c))
                {
                    sb.Append(c);
                    c = NextChar();
                }
            }

        return_result:
            string s = sb.ToString();           
            val = Convert.ToDecimal(s, NumberFormatInfo.InvariantInfo);
            return LToken.NUMBER;
        }

        private int consume_string(char s)
        {
            char c;
            sb.Length = 0;
            while ((c = NextChar()) != s)
            {
                if (c == '\n')
                    throw new ParseException("Newline in constant");
                if (c == '\0')
                    throw new ParseException("Unterminated string literal");
                sb.Append(c);
            }
            NextChar();
            val = sb.ToString();
            return LToken.TEXT;
        }

        private int consume_identifier(char s)
        {
            char c;

            sb.Length = 0;
            sb.Append(s);

            while (is_identifier_part_character(c = NextChar()))
                sb.Append(c);

            val = sb.ToString();
            return LToken.ATOM;
        }

        private int xtoken()
        {
            char c;
            int t;
            bool doread;

            while ((c = PeekChar()) != '\0')
            {
                if (c == ' ' || c == '\t' || c == '\f' || c == '\v' || c == '\n' || c == '\r' || c == 0xa0)
                {

                    if (c == '\t')
                        colNo = (((colNo + 8) / 8) * 8) - 1;
                    NextChar();
                    continue;
                }

                if (c == '%')
                {
                    while ((c = NextChar()) != '\0' && (c != '\n') && c != '\r')
                        ;
                    continue;
                }

                if (is_identifier_start_character(c))
                    return consume_identifier(c);

                if ((t = is_punct(c, out doread)) != LToken.ERROR)
                {
                    if (!doread)
                        for (int k = 1; k <= val.ToString().Length; k++)
                            NextChar();
                    return t;
                }

                if (Char.IsDigit(c))
                    return is_number(c);

                if (c == '"')
                    return consume_string(c);

                return LToken.ERROR;
            }

            return 0;
        }

        public int token()
        {
            m_token = xtoken();
            return m_token;
        }

        public int peek()
        {
            return m_token;
        }

        public Object value()
        {
            return val;
        }

        public int getPosition()
        {
            return pos;
        }
    }
}
