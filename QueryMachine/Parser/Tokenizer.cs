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
using System.Text;
using System.Collections;
using System.Globalization;

using DataEngine.Parser.yyParser;
using DataEngine.CoreServices;

namespace DataEngine.Parser
{    
	/// <summary>
	/// Summary description for Tokenizer.
	/// </summary>
	public class Tokenizer: yyInput
	{
		private char [] Buffer;
		private char current;
		private int pos;
		private int lineNo;
		private int colNo;
        private object val = null;
		
        private int m_token;
		
        private StringBuilder sb = new StringBuilder();
		private Hashtable s_mapTokens = new Hashtable();

		public Tokenizer(string strInput)
		{
            FillHashtab(s_mapTokens);
            Init(strInput);
		}

        private void Init(string strInput)
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

        private bool NextId(string id)
        {
            if (xtoken() == Token.id)
            {
                if (svalue().Equals(id, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        private class TokenizerBookmark
        {
            internal char current;
            internal int pos;
            internal int lineNo;
            internal int colNo;
            internal object val;
        }

        private TokenizerBookmark GetBookmark()
        {
            TokenizerBookmark b = new TokenizerBookmark();
            b.current = current;
            b.pos = pos;
            b.lineNo = lineNo;
            b.colNo = colNo;
            b.val = val;            
            return b;
        }

        private void GotoBookmark(TokenizerBookmark b)
        {
            current = b.current;
            pos = b.pos;
            lineNo = b.lineNo;
            colNo = b.colNo;
            val = b.val;
        }

        public void FillHashtab(Hashtable mapTokens)
        {
            for (int t = Token._RS_START + 1; t < Token._RS_END; t++)
                mapTokens.Add(YYParser.yyname(t).ToLower(), t);
        }

        public int is_sign(Tokenizer tok, char c, out string val, out bool do_read)
        {
            do_read = false;
            val = Convert.ToString(c);

            switch (c)
            {
                case '(':
                    return Convert.ToInt32(c);

                case ')':
                    return Convert.ToInt32(c);

                case '*':
                    return Convert.ToInt32(c);

                case '/':
                    if (tok.MatchText("//"))
                    {
                        val = "//";
                        do_read = true;
                        return Token.double_slash;
                    }
                    else
                        return Convert.ToInt32(c);

                case '=':
                    return Convert.ToInt32(c);

                case '+':
                    return Convert.ToInt32(c);

                case '-':
                    return Convert.ToInt32(c);

                case ',':
                    return Convert.ToInt32(c);

                case '?':
                    return Convert.ToInt32(c);

                case ':':
                    if (tok.MatchText("::"))
                    {
                        val = "::";
                        do_read = true;
                        return Token.double_colon;
                    }
                    else
                        return Convert.ToInt32(c);
                
                case '[':
                    return Convert.ToInt32(c);

                case ']':
                    return Convert.ToInt32(c);

                case '.':
                    if (tok.MatchText(".*"))
                    {
                        val = ".*";
                        do_read = true;
                        return Token.asterisk_tag;
                    }
                    else
                        return Convert.ToInt32(c);

                case '|':
                    if (tok.MatchText("||"))
                    {
                        val = "||";
                        do_read = true;
                        return Token.concatenation_operator;
                    }
                    else
                        throw new ESQLException("'|' is ommited");

                case '<':
                    if (tok.MatchText("<="))
                    {
                        val = "<=";
                        do_read = true;
                        return Token.less_than_or_equals_operator;
                    }
                    else if (tok.MatchText("<>"))
                    {
                        val = "<>";
                        do_read = true;
                        return Token.not_equals_operator;
                    }
                    else
                        return Convert.ToInt32(c);

                case '>':
                    if (tok.MatchText(">="))
                    {
                        val = ">=";
                        do_read = true;
                        return Token.greater_than_or_equals_operator;
                    }
                    else
                        return Convert.ToInt32(c);

                default:
                    return Token.yyErrorCode;
            }
        }

        public static bool is_identifier_start_character(char c)
        {
            return (c == '@') || (c == '$') || (c == '&') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || Char.IsLetter(c);
        }

        public static bool is_identifier_part_character(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || (c >= '0' && c <= '9') || Char.IsLetter(c);
        }

		int is_punct (char c, out bool do_read)
		{

            string sval;
            int result = is_sign(this, c, out sval, out do_read);
            val = sval;
            return result;
		}

		private int is_number (char c)
		{
			sb.Length = 0;
            int rs = Token.unsigned_integer;

			while (Char.IsDigit(c))
			{
				sb.Append(c);
				c = NextChar();
			}

			if (c == '.')
			{
                rs = Token.unsigned_float;
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
                rs = Token.unsigned_double;
	            
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
            if (rs == Token.unsigned_float)
                val = Convert.ToDecimal(s, NumberFormatInfo.InvariantInfo);
            else if (rs == Token.unsigned_double)
                val = Convert.ToDouble(s, NumberFormatInfo.InvariantInfo);
            else
                val = Convert.ToInt32(s);
			return rs;
		}

		private int consume_string (char s)
		{
			char c;
		    sb.Length = 0;
			while ((c = NextChar()) != s)
			{
				if (c == '\0')
					throw new ESQLException("Unterminated string literal");
				sb.Append(c);
			}
            NextChar();
            val = sb.ToString();
            if (s == '\'')
                return Token.string_literal;
            else
            {
                val = Util.QuoteName((string)val);
                return Token.id;
            }
		}

		private int consume_identifier (char s) 
		{
			char c;

			sb.Length = 0;
			sb.Append(s);

			while (is_identifier_part_character (c = NextChar()))
				sb.Append(c);

			string ident = sb.ToString();
            object tok = s_mapTokens[ident.ToLower()];
            val = ident;

            if (tok != null)
            {
                val = tok;
                return (int)tok;
            }
            else
            {
                if (s == '&')
                    return Token.parameter_name;
                else if (s == '$')
                    return Token.func;
                else
                    return Token.id;
            }
		}

        private int consume_optimizer_hint()
        {
            sb.Length = 0;

            char c;
            while ((c = NextChar()) != '\0' && (c != '\n') && c != '\r')
                sb.Append(c);

            val = sb.ToString();
            return Token.optimizer_hint;
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

				if (c == '-')
				{
                    char d = PeekChar(1);
				
					if (d == '-')
					{
						NextChar();
                        if (NextChar() == '!')
                            return consume_optimizer_hint();
                        else
                        {
                            while ((d = NextChar()) != '\0' && (d != '\n') && d != '\r')
                                ;
                            continue;
                        }
					} 
					else if (d == '*')
					{
						NextChar();

						while ((d = NextChar()) != '\0')
						{
							if (d == '*' && PeekChar() == '/')
							{
								NextChar();
								break;
							}
						}
						continue;
					}
					goto is_punct_label;
				}

				if (is_identifier_start_character (c))
					return consume_identifier (c);

			is_punct_label:
				if ((t = is_punct (c, out doread)) != Token.yyErrorCode)
				{
                    if (!doread)                      
                        for (int k = 1; k <= val.ToString().Length; k++)
                            NextChar();
					return t;
				}

				if (Char.IsDigit(c))
					return is_number(c);

				if (c == '\'' || c == '"') 
					return consume_string (c);

                return Token.yyErrorCode;
			}

			return 0;
		}

		public bool advance ()
		{
			return current != '\0';
		}

		public int token ()
		{
            m_token = xtoken();
            TokenizerBookmark bookmark;
            switch (m_token)
            {
                case Token.COUNT: // COUNT(*)
                    bookmark = GetBookmark();
                    if (xtoken() == Convert.ToInt32('(') &&
                        xtoken() == Convert.ToInt32('*') &&
                        xtoken() == Convert.ToInt32(')'))
                    {
                        m_token = Token.count_all_fct;
                        val = YYParser.yyname(m_token);
                    }
                    else
                        GotoBookmark(bookmark);
                    break;

                case Token.XMLFOREST: // XMLFOREST(*)
                    bookmark = GetBookmark();
                    if (xtoken() == Convert.ToInt32('(') &&
                        xtoken() == Convert.ToInt32('*') &&
                        xtoken() == Convert.ToInt32(')'))
                    {
                        m_token = Token.xml_forest_all;
                        val = YYParser.yyname(m_token);
                    }
                    else
                        GotoBookmark(bookmark);
                    break;

                case Token.XMLATTRIBUTES: // XMLATTRIBUTES(*)
                    bookmark = GetBookmark();
                    if (xtoken() == Convert.ToInt32('(') &&
                        xtoken() == Convert.ToInt32('*') &&
                        xtoken() == Convert.ToInt32(')'))
                    {
                        m_token = Token.xml_attributes_all;
                        val = YYParser.yyname(m_token);
                    }
                    else
                        GotoBookmark(bookmark);
                    break;
                
                case Token.NOT: // NOT LIKE
                    bookmark = GetBookmark();
                    if (xtoken() == Token.LIKE)
                    {
                        m_token = Token.NOTLIKE;
                        val = YYParser.yyname(m_token);
                    }
                    else
                        GotoBookmark(bookmark);
                    break;
                
                case Token.CROSS: // CROSS JOIN
                    bookmark = GetBookmark();
                    if (xtoken() == Token.JOIN)
                    {
                        m_token = Token.CROSSJOIN;
                        val = YYParser.yyname(m_token);
                    }
                    else
                        GotoBookmark(bookmark);
                    break;
                
                case Token.UNION: // UNION JOIN
                    bookmark = GetBookmark();
                    if (xtoken() == Token.JOIN)
                    {
                        m_token = Token.UNIONJOIN;
                        val = YYParser.yyname(m_token);
                    }
                    else
                        GotoBookmark(bookmark);
                    break;

                case Token.CHARACTER:
                    {
                        bookmark = GetBookmark();
                        if (xtoken() == Token.id && svalue().Equals("VARYING",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            m_token = Token.CHARACTER_VARYING;
                            val = m_token;
                        }
                        else
                            GotoBookmark(bookmark);
                    }
                    break;

                case Token.CHAR:
                    {
                        bookmark = GetBookmark();
                        if (xtoken() == Token.id && svalue().Equals("VARYING",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            m_token = Token.CHAR_VARYING;
                            val = m_token;
                        }
                        else
                            GotoBookmark(bookmark);
                    }
                    break;

                case Token.DOUBLE:
                    {
                        bookmark = GetBookmark();
                        if (xtoken() == Token.id && svalue().Equals("PRECISION",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            m_token = Token.DOUBLE_PRECISION;
                            val = m_token;
                        }
                        else
                            GotoBookmark(bookmark);
                    }
                    break;

                case Token.OPTION:
                    {
                        bookmark = GetBookmark();
                        int tok = xtoken();
                        if (tok == Token.NULL)
                        {
                            m_token = Token.OPTION_NULL;
                            val = YYParser.yyname(m_token);
                        }
                        else if (tok == Token.id && svalue().Equals("EMPTY",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            m_token = Token.OPTION_EMPTY;
                            val = YYParser.yyname(m_token);
                        }
                        else if (tok == Token.id && svalue().Equals("ABSENT",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            m_token = Token.OPTION_ABSENT;
                            val = YYParser.yyname(m_token);
                        }
                        else if (tok == Token.id && svalue().Equals("NIL",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            m_token = Token.OPTION_NIL;
                            val = YYParser.yyname(m_token);
                        }
                        else
                            GotoBookmark(bookmark);                        
                    }
                    break;

                case Token.id:                    
                    if (svalue().Equals("NO",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        bookmark = GetBookmark();
                        int tok = xtoken();
                        if (tok == Token.DEFAULT)
                        {
                            m_token = Token.NO_DEFAULT;
                            val = YYParser.yyname(m_token);
                        }
                        else if (tok == Token.VALUE)
                        {
                            m_token = Token.NO_VALUE;
                            val = YYParser.yyname(m_token);
                        }
                        else if (tok == Token.id && svalue().Equals("CONTENT",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            m_token = Token.NO_CONTENT;
                            val = YYParser.yyname(m_token);
                        }
                        else
                            GotoBookmark(bookmark);
                    }
                    else if (svalue().Equals("PRESERVE",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        bookmark = GetBookmark();
                        if (NextId("WHITESPACE"))
                        {
                            m_token = Token.PRESERVE_WHITESPACE;
                            val = YYParser.yyname(m_token);                            
                        }
                        else
                            GotoBookmark(bookmark);
                    }
                    else if (svalue().Equals("STRIP",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        bookmark = GetBookmark();
                        if (NextId("WHITESPACE"))
                        {
                            m_token = Token.STRIP_WHITESPACE;
                            val = YYParser.yyname(m_token);
                        }
                        else
                            GotoBookmark(bookmark);
                    }
                    else if (svalue().Equals("RETURNING",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        bookmark = GetBookmark();
                        if (NextId("CONTENT"))
                        {
                            m_token = Token.RETURNING_CONTENT;
                            val = YYParser.yyname(m_token);
                        }
                        else if (NextId("SEQUENCE"))
                        {
                            m_token = Token.RETURNING_SEQUENCE;
                            val = YYParser.yyname(m_token);
                        }
                        else
                            GotoBookmark(bookmark);
                    }
                    break;
            }
			return m_token;
		}

        public int peek()
        {
            return m_token;
        }

		public Object value ()
		{
		   return val;
		}

        public String svalue()
        {
            return val.ToString();
        }

        public int getPosition()
        {
            return pos;
        }
	}
}

