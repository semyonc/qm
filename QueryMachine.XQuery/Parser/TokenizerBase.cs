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
using System.IO;
using System.Globalization;

using DataEngine.XQuery.Parser.yyParser;
using DataEngine.CoreServices;

namespace DataEngine.XQuery.Parser
{
    public abstract class TokenizerBase : yyInput
    {
        private LexerState m_state = LexerState.Default;       
        private Stack<LexerState> m_states = new Stack<LexerState>();
        private Queue<CurrentToken> m_token = new Queue<CurrentToken>();

        private int m_anchor;
        private int[] m_bookmark;
        private int m_length;
        
        private object m_value;

        public int LineNo { get; protected set; }
        public int ColNo { get; protected set; }

        public abstract int Position { get; }
        public bool NotIgnoreCommentsAndSpace { get; set; }

        private struct CurrentToken
        {
            public int token;
            public object value;
            public int anchor;
            public int length;
            public LexerState state;
        }

        public enum LexerState
        {
            Default,
            DeclareOrdering,
            Operator,
            ExprComment,
            XQueryVersion,
            NamespaceDecl,
            UriToOperator,
            NamespaceKeyword,
            XmlSpace_Decl,
            SingleType,
            ItemType,
            KindTest,
            KindTestForPi,
            CloseKindTest,
            OccurrenceIndicator,
            Option,
            Pragma,
            VarName,
            StartTag,
            ElementContent,
            EndTag,
            XmlComment,
            ProcessingInstruction,
            ProcessingInstructionContent,
            CDataSection,
            QuotAttributeContent,
            AposAttributeContent,
            AttributeState
        };

        public LexerState CurrentState { get; private set; }

        public TokenizerBase()
        {
            LineNo = ColNo = 1;
            m_bookmark = new int[5];
        }

        private void BeginToken()
        {
            m_anchor = Position;
            m_length = 0;
        }

        private void EndToken()
        {
            m_length = Position - m_anchor;
        }

        private void BeginToken(int anchor)
        {
            m_anchor = anchor;
            m_length = 0;
        }

        private void EndToken(string s)
        {
            m_length = s.Length;
        }

        private void ConsumeNumber()
        {
            int tok = Token.IntegerLiteral;
            StringBuilder sb = new StringBuilder();
            BeginToken();
            while (XmlCharType.Instance.IsDigit(Peek(0)))
                sb.Append(Read());
            if (Peek(0) == '.')
            {
                tok = Token.DecimalLiteral;
                sb.Append(Read());
                while (XmlCharType.Instance.IsDigit(Peek(0)))
                    sb.Append(Read());
            }
            char c = Peek(0);
            if (c == 'E' || c == 'e')
            {
                tok = Token.DoubleLiteral;
                sb.Append(Read());
                c = Peek(0);
                if (c == '+' || c == '-')
                    sb.Append(Read());
                while (XmlCharType.Instance.IsDigit(Peek(0)))
                    sb.Append(Read());
            }
            EndToken();
            string s = sb.ToString();
            switch (tok)
            {
                case Token.IntegerLiteral:
                    ConsumeToken(tok, new IntegerValue((Integer)Decimal.Parse(s, NumberFormatInfo.InvariantInfo)));
                    break;

                case Token.DecimalLiteral:
                    ConsumeToken(tok, new DecimalValue(Decimal.Parse(s, NumberFormatInfo.InvariantInfo)));
                    break;

                case Token.DoubleLiteral:
                    ConsumeToken(tok, new DoublelValue(Double.Parse(s, NumberFormatInfo.InvariantInfo)));
                    break;
            }
        }

        private void ConsumeLiteral()
        {
            BeginToken();
            char qoute = Read();
            StringBuilder sb = new StringBuilder();            
            char c;
            while ((c = Peek(0)) != qoute || Peek(1) == qoute)
            {
                if (Peek(0) == 0)
                    return;
                if (c == qoute && Peek(1) == qoute)
                    Read();
                sb.Append(Read());
            }
            Read();
            EndToken();
            ConsumeToken(Token.StringLiteral, 
                new Literal(Core.NormalizeStringValue(sb.ToString(), false, true), qoute));
        }

        private void ConsumeNCName()
        {            
            char c;
            StringBuilder sb = new StringBuilder();
            BeginToken();
            while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsNCNameChar(c))
                sb.Append(Read());
            EndToken();
            ConsumeToken(Token.NCName, new Qname(sb.ToString()));
        }

        private void ConsumeQName()
        {
            StringBuilder sb = new StringBuilder();
            char c;
            BeginToken();
            while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsNameChar(c))
                sb.Append(Read());
            EndToken();
            ConsumeToken(Token.QName, new Qname(sb.ToString()));
        }

        private void ConsumeCharRefHex()
        {
            StringBuilder sb = new StringBuilder();
            bool bad = false;
            char c;
            BeginToken();
            while ((c = Peek(0)) != ';')
            {
                if (Peek(0) == 0)
                    return;
                if (!XmlCharType.Instance.IsHexDigit(c))
                    bad = true;
                sb.Append(Read());
            }
            EndToken();
            Read();
            if (bad)
                throw new XQueryException(Properties.Resources.BadCharRef, "x", sb.ToString());
            ConsumeToken(Token.CharRef, new CharRefHex(sb.ToString()));
        }

        private void ConsumeCharRef()
        {
            StringBuilder sb = new StringBuilder();
            bool bad = false;
            char c;
            BeginToken();
            while ((c = Peek(0)) != ';')
            {
                if (Peek(0) == 0)
                    return;
                if (!XmlCharType.Instance.IsDigit(c))
                    bad = true;
                sb.Append(Read());
            }            
            EndToken();
            Read();
            if (bad)
                throw new XQueryException(Properties.Resources.BadCharRef, "", sb.ToString());
            ConsumeToken(Token.CharRef, new CharRef(sb.ToString()));
        }

        private void ConsumeS()
        {
            char c;
            StringBuilder sb = new StringBuilder();
            BeginToken();
            while (XmlCharType.Instance.IsWhiteSpace(c = Peek(0)) && c != 0)
                sb.Append(Read());
            EndToken();
            ConsumeToken(Token.S, new Literal(sb.ToString()));
        }

        private void ConsumeChar(char token)
        {
            CurrentToken curr;
            curr.token = token;
            curr.value = null;
            curr.anchor = m_anchor;
            curr.length = 1;
            curr.state = m_state;
            m_token.Enqueue(curr);            
        }

        private void ConsumeToken(int token)
        {
            ConsumeToken(token, null);
        }

        private void ConsumeToken(int token, int anchor, int length)
        {
            ConsumeToken(token, null, anchor, length);
        }

        private void ConsumeToken(int token, object value, int anchor, int length)
        {
            CurrentToken curr;
            curr.token = token;
            curr.value = value;
            curr.anchor = anchor;
            curr.length = length;
            curr.state = m_state;
            m_token.Enqueue(curr);            
        }

        private void ConsumeToken(int token, object value)
        {
            CurrentToken curr;
            curr.token = token;
            curr.value = value;
            curr.anchor = m_anchor;
            curr.length = m_length;
            curr.state = m_state;
            m_token.Enqueue(curr);            
        }

        protected abstract char Peek(int lookahead);
        protected abstract char Read();

        private bool MatchText(string text)
        {
            for (int k = 0; k < text.Length; k++)
            {
                char ch = Peek(k);
                if (ch == 0 || ch != text[k])
                    return false;
            }
            for (int k = 0; k < text.Length; k++)
                Read();
            return true;
        }

        private bool MatchIdentifer(params string[] identifer)
        {
            int i = 0;
            for (int sp = 0; sp < identifer.Length; sp++)
            {
                char c;
                while (true)
                {
                    if ((c = Peek(i)) != 0 && XmlCharType.Instance.IsWhiteSpace(c))
                    {
                        while ((c = Peek(i)) != 0 && XmlCharType.Instance.IsWhiteSpace(c))
                            i++;
                        continue;
                    }
                    if (Peek(i) == '(' && Peek(i + 1) == ':')
                    {
                        int n = 1;
                        i += 2;
                        while (true)
                        {
                            c = Peek(i);
                            if (c == 0)
                                break;
                            else if (c == '(' && Peek(i + 1) == ':')
                            {
                                i += 2;
                                n++;
                            }
                            else if (c == ':' && Peek(i + 1) == ')')
                            {
                                i += 2;
                                if (--n == 0)
                                    break;
                            }
                            else
                                i++;
                        }
                        continue;
                    }
                    break;
                }
                string s = identifer[sp];                
                m_bookmark[sp] = Position + i;
                if (s.Length > 0)
                {
                    for (int k = 0; k < s.Length; k++, i++)
                        if ((c = Peek(i)) == 0 || c != s[k])
                            return false;
                    if (XmlCharType.Instance.IsStartNCNameChar(s[0]) &&
                        XmlCharType.Instance.IsNCNameChar(Peek(i)))
                        return false;
                }
            }
            while (i-- > 0)
                Read();
            return true;
        }      
       
        private void SkipWhitespace()
        {
            do
            {
                if (XmlCharType.Instance.IsWhiteSpace(Peek(0)))
                {
                    char c;
                    if (NotIgnoreCommentsAndSpace)
                        BeginToken();
                    while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsWhiteSpace(c))
                        Read();
                    if (NotIgnoreCommentsAndSpace)
                    {
                        EndToken();
                        ConsumeToken(Token.XQWhitespace);
                    }
                    continue;
                }
                if (Peek(0) == '(' && Peek(1) == ':')
                {
                    if (NotIgnoreCommentsAndSpace)
                        BeginToken();
                    Read();
                    Read();
                    int n = 1;
                    while (true)
                    {
                        char c = Peek(0);
                        if (c == 0)
                            break;
                        else if (c == '(' && Peek(1) == ':')
                        {
                            Read();
                            Read();
                            n++;
                        }
                        else if (c == ':' && Peek(1) == ')')
                        {
                            Read();
                            Read();
                            if (--n == 0)
                            {
                                if (NotIgnoreCommentsAndSpace)
                                {
                                    EndToken();
                                    ConsumeToken(Token.XQComment);
                                }
                                break;
                            }                            
                        }
                        else
                            Read();
                    }
                    continue;
                }
                break;
            }
            while (true);
        }

        private void DefaultState()
        {
            SkipWhitespace();
            BeginToken();
            char c = Peek(0);
            if (c == '\0')
                ConsumeToken(0); // EOF
            //else if (MatchText("(:"))
            //{                
            //    m_states.Push(LexerState.Default);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //}
            else if (MatchText("(#"))
            {
                EndToken();
                ConsumeToken(Token.PRAGMA_BEGIN);
                m_state = LexerState.Pragma;
            }
            else if (c == '.')
            {
                if (Peek(1) == '.')
                {
                    Read();
                    Read();
                    EndToken();
                    ConsumeToken(Token.DOUBLE_PERIOD);
                }
                else if (XmlCharType.Instance.IsDigit(Peek(1)))
                    ConsumeNumber();
                else
                    ConsumeChar(Read());
                m_state = LexerState.Operator;
            }
            else if (c == ')')
            {
                ConsumeChar(Read());
                SkipWhitespace();
                BeginToken();
                if (MatchIdentifer("as"))
                {
                    EndToken();
                    ConsumeToken(Token.AS);
                    m_state = LexerState.ItemType;
                }
                else
                    m_state = LexerState.Operator;
            }
            else if (c == '*')
            {
                ConsumeChar(Read());
                if (Peek(0) == ':')
                {
                    BeginToken();
                    ConsumeChar(Read());
                    c = Peek(0);
                    if (c != 0 && XmlCharType.Instance.IsStartNCNameChar(c))
                        ConsumeNCName();
                    else
                        throw new XQueryException(Properties.Resources.ExpectedNCName);
                }
                m_state = LexerState.Operator;
            }
            else if (c == ';' || c == ',' || c == '(' || c == '-' || c == '+' || c == '@')
                ConsumeChar(Read());
            else if (c == '/')
            {
                if (Peek(1) == '/')
                {
                    Read();
                    Read();
                    EndToken();
                    ConsumeToken(Token.DOUBLE_SLASH);
                }
                else
                    ConsumeChar(Read());
            }
            else if (MatchIdentifer("if", "("))
            {
                EndToken("if");
                ConsumeToken(Token.IF);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
            }
            else if (MatchIdentifer("declare", "construction"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_CONSTRUCTION);
                m_state = LexerState.Operator;
            }
            else if (MatchIdentifer("declare", "default", "order"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_DEFAULT_ORDER);
                m_state = LexerState.Operator;
            }
            else if (MatchIdentifer("declare", "default", "collation"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_DEFAULT_COLLATION);
                m_state = LexerState.NamespaceDecl;
            }
            else if (MatchIdentifer("declare", "namespace"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_NAMESPACE);
                m_state = LexerState.NamespaceDecl;
            }
            else if (MatchIdentifer("module", "namespace"))
            {
                EndToken();
                ConsumeToken(Token.MODULE_NAMESPACE);
                m_state = LexerState.NamespaceDecl;
            }
            else if (MatchIdentifer("declare", "base-uri"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_BASE_URI);
                m_state = LexerState.NamespaceDecl;
            }
            else if (MatchIdentifer("declare", "default", "element"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_DEFAULT_ELEMENT);
                m_state = LexerState.NamespaceKeyword;
            }
            else if (MatchIdentifer("declare", "default", "function"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_DEFAULT_FUNCTION);
                m_state = LexerState.NamespaceKeyword;
            }
            else if (MatchIdentifer("import", "schema"))
            {
                EndToken();
                ConsumeToken(Token.IMPORT_SCHEMA);
                m_state = LexerState.NamespaceKeyword;
            }
            else if (MatchIdentifer("import", "module"))
            {
                EndToken();
                ConsumeToken(Token.IMPORT_MODULE);
                m_state = LexerState.NamespaceKeyword;
            }
            else if (MatchIdentifer("declare", "copy-namespaces"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_COPY_NAMESPACES);
                m_state = LexerState.NamespaceKeyword;
            }
            else if (MatchIdentifer("for"))
            {
                EndToken();
                ConsumeToken(Token.FOR);
                SkipWhitespace();
                BeginToken();
                if (Peek(0) == '$')
                    ConsumeChar(Read());
                else
                    throw new XQueryException(Properties.Resources.ExpectedVariablePrefix, "for");
                m_state = LexerState.VarName;
            }
            else if (MatchIdentifer("let"))
            {
                EndToken();
                ConsumeToken(Token.LET);
                SkipWhitespace();
                BeginToken();
                if (Peek(0) == '$')
                    ConsumeChar(Read());
                else
                    throw new XQueryException(Properties.Resources.ExpectedVariablePrefix, "let");
                m_state = LexerState.VarName;
            }
            else if (MatchIdentifer("some"))
            {
                EndToken();
                ConsumeToken(Token.SOME);
                SkipWhitespace();
                BeginToken();
                if (Peek(0) == '$')
                    ConsumeChar(Read());
                else
                    throw new XQueryException(Properties.Resources.ExpectedVariablePrefix, "some");
                m_state = LexerState.VarName;
            }
            else if (MatchIdentifer("every"))
            {
                EndToken();
                ConsumeToken(Token.EVERY);
                SkipWhitespace();
                BeginToken();
                if (Peek(0) == '$')
                    ConsumeChar(Read());
                else
                    throw new XQueryException(Properties.Resources.ExpectedVariablePrefix, "every");
                m_state = LexerState.VarName;
            }
            else if (MatchIdentifer("declare", "variable"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_VARIABLE);
                SkipWhitespace();
                BeginToken();
                if (Peek(0) == '$')
                    ConsumeChar(Read());
                else
                    throw new XQueryException(Properties.Resources.ExpectedVariablePrefix, "declare variable");
                m_state = LexerState.VarName;
            }
            else if (c == '$')
            {
                ConsumeChar(Read());
                m_state = LexerState.VarName;
            }
            else if (MatchIdentifer("element", "("))
            {
                EndToken("element");
                ConsumeToken(Token.ELEMENT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.Operator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("attribute", "("))
            {
                EndToken("attribute");
                ConsumeToken(Token.ATTRIBUTE);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.Operator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("schema-element", "("))
            {
                EndToken("schema-element");
                ConsumeToken(Token.SCHEMA_ELEMENT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.Operator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("schema-attribute", "("))
            {
                EndToken("schema-attribute");
                ConsumeToken(Token.SCHEMA_ATTRIBUTE);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.Operator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("comment", "("))
            {
                EndToken("comment");
                ConsumeToken(Token.COMMENT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.Operator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("text", "("))
            {
                EndToken("text");
                ConsumeToken(Token.TEXT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.Operator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("node", "("))
            {
                EndToken("node");
                ConsumeToken(Token.NODE);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.Operator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("document-node", "("))
            {
                EndToken("document-node");
                ConsumeToken(Token.DOCUMENT_NODE);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.Operator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("processing-instruction", "("))
            {
                EndToken("processing-instruction");
                ConsumeToken(Token.PROCESSING_INSTRUCTION);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.Operator);
                m_state = LexerState.KindTestForPi;
            }
            else if (MatchText("<!--"))
            {
                EndToken();
                ConsumeToken(Token.COMMENT_BEGIN);
                m_states.Push(LexerState.Operator);
                m_state = LexerState.XmlComment;
            }
            else if (MatchText("<?"))
            {
                EndToken();
                ConsumeToken(Token.PI_BEGIN);
                m_states.Push(LexerState.Operator);
                m_state = LexerState.ProcessingInstruction;
            }
            else if (MatchText("<![CDATA["))
            {
                EndToken();
                ConsumeToken(Token.CDATA_BEGIN);
                m_states.Push(LexerState.Operator);
                m_state = LexerState.CDataSection;
            }
            else if (c == '<')
            {
                Read();
                EndToken();
                ConsumeToken(Token.BeginTag);
                m_states.Push(LexerState.Operator);
                m_state = LexerState.StartTag;
            }
            else if (MatchIdentifer("declare", "boundary-space"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_BOUNDARY_SPACE);
                m_state = LexerState.XmlSpace_Decl;
            }
            else if (c == '}')
            {
                ConsumeChar(Read());
                m_state = m_states.Pop();
            }
            else if (c == '{')
            {
                ConsumeChar(Read());
                m_states.Push(LexerState.Operator);
            }
            else if (MatchIdentifer("validate", "{"))
            {
                EndToken("validate");
                ConsumeToken(Token.VALIDATE);
                BeginToken(m_bookmark[1]);
                ConsumeChar('{');
                m_states.Push(LexerState.Operator);
            }
            else if (MatchIdentifer("validate", "lax"))
            {
                EndToken("validate");
                ConsumeToken(Token.VALIDATE);
                BeginToken(m_bookmark[1]);
                ConsumeToken(Token.LAX);
                m_states.Push(LexerState.Operator);
            }
            else if (MatchIdentifer("validate", "strict"))
            {
                EndToken("validate");
                ConsumeToken(Token.VALIDATE);
                BeginToken(m_bookmark[1]);
                ConsumeToken(Token.STRICT);
                m_states.Push(LexerState.Operator);
            }
            else if (MatchIdentifer("typeswitch", "("))
            {
                EndToken("typeswitch");
                ConsumeToken(Token.TYPESWITCH);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
            }
            else if (MatchIdentifer("document", "{"))
            {
                EndToken("document");
                ConsumeToken(Token.DOCUMENT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('{');
                m_states.Push(LexerState.Operator);
            }
            else if (MatchIdentifer("text", "{"))
            {
                EndToken("text");
                ConsumeToken(Token.TEXT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('{');
                m_states.Push(LexerState.Operator);
            }
            else if (MatchIdentifer("comment", "{"))
            {
                EndToken("comment");
                ConsumeToken(Token.COMMENT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('{');
                m_states.Push(LexerState.Operator);
            }
            else if (MatchIdentifer("declare", "function"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_FUNCTION);
            }
            else if (MatchIdentifer("ordered", "{"))
            {
                EndToken("ordered");
                ConsumeToken(Token.ORDERED);
                BeginToken(m_bookmark[1]);
                ConsumeChar('{');
                m_states.Push(LexerState.Default);
            }
            else if (MatchIdentifer("unordered", "{"))
            {
                EndToken("unordered");
                ConsumeToken(Token.UNORDERED);
                BeginToken(m_bookmark[1]);
                ConsumeChar('{');
                m_states.Push(LexerState.Default);
            }
            else if (MatchIdentifer("declare", "ordering"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_ORDERING);
                m_state = LexerState.DeclareOrdering;
            }
            else if (MatchIdentifer("xquery", "version"))
            {
                EndToken();
                ConsumeToken(Token.XQUERY_VERSION);
                m_state = LexerState.XQueryVersion;
            }
            else if (MatchText("(#"))
            {
                EndToken();
                ConsumeToken(Token.PRAGMA_BEGIN);
                m_state = LexerState.Pragma;
            }
            else if (MatchIdentifer("declare", "option"))
            {
                EndToken();
                ConsumeToken(Token.DECLARE_OPTION);
                m_state = LexerState.Option;
            }
            else if (MatchIdentifer("ancestor-or-self", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_ANCESTOR_OR_SELF);
            }
            else if (MatchIdentifer("ancestor", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_ANCESTOR);
            }
            else if (MatchIdentifer("attribute", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_ATTRIBUTE);
            }
            else if (MatchIdentifer("child", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_CHILD);
            }
            else if (MatchIdentifer("descendant-or-self", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_DESCENDANT_OR_SELF);
            }
            else if (MatchIdentifer("descendant", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_DESCENDANT);
            }
            else if (MatchIdentifer("following-sibling", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_FOLLOWING_SIBLING);
            }
            else if (MatchIdentifer("following", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_FOLLOWING);
            }
            else if (MatchIdentifer("parent", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_PARENT);
            }
            else if (MatchIdentifer("preceding-sibling", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_PRECEDING_SIBLING);
            }
            else if (MatchIdentifer("preceding", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_PRECEDING);
            }
            else if (MatchIdentifer("self", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_SELF);
            }
            else if (MatchIdentifer("namespace", "::"))
            {
                EndToken();
                ConsumeToken(Token.AXIS_NAMESPACE);
            }
            else if (MatchIdentifer("at"))
            {
                EndToken();
                SkipWhitespace();
                if (Peek(0) == '"' || Peek(0) == '\'')
                {
                    ConsumeToken(Token.AT);
                    ConsumeLiteral();
                    m_state = LexerState.NamespaceDecl;
                }
                else
                {
                    ConsumeToken(Token.QName, new Qname("at"));
                    if (Peek(0) != ')')
                        m_state = LexerState.Operator;
                }
            }
            else if (c == '"' || c == '\'')
            {
                ConsumeLiteral();
                m_state = LexerState.Operator;
            }
            else if (XmlCharType.Instance.IsDigit(c))
            {
                ConsumeNumber();
                m_state = LexerState.Operator;
            }
            else if (XmlCharType.Instance.IsStartNameChar(c))
            {
                StringBuilder sb = new StringBuilder();
                while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsNCNameChar(c))
                    sb.Append(Read());
                if (Peek(0) == ':')
                {
                    if (Peek(1) == '*')
                    {
                        EndToken();
                        ConsumeToken(Token.NCName, new Qname(sb.ToString()));
                        BeginToken();
                        ConsumeChar(Read());
                        BeginToken();
                        ConsumeChar(Read());
                        m_state = LexerState.Operator;
                    }
                    else
                    {
                        while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsNameChar(c))
                            sb.Append(Read());
                        EndToken();
                        ConsumeToken(Token.QName, new Qname(sb.ToString()));
                        SkipWhitespace();
                        if (Peek(0) != '(')
                            m_state = LexerState.Operator;
                    }
                }
                else
                {
                    EndToken();
                    int anchor = m_anchor;
                    int length = m_length;
                    string ncname = sb.ToString();
                    if (ncname == "element" || ncname == "attribute")
                    {
                        SkipWhitespace();
                        if (Peek(0) == '{')
                        {
                            if (ncname == "element")
                                ConsumeToken(Token.ELEMENT, anchor, length);
                            else
                                ConsumeToken(Token.ATTRIBUTE, anchor, length);
                            BeginToken();
                            ConsumeChar(Read());
                            m_states.Push(LexerState.Operator);
                            return;
                        }
                        else if (XmlCharType.Instance.IsStartNameChar(Peek(0)))
                        {
                            BeginToken();
                            sb = new StringBuilder();
                            while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsNameChar(c))
                                sb.Append(Read());
                            EndToken();
                            int anchor2 = m_anchor;
                            int length2 = m_length;
                            SkipWhitespace();
                            if (Peek(0) == '{')
                            {
                                if (ncname == "element")
                                    ConsumeToken(Token.ELEMENT, anchor, length);
                                else
                                    ConsumeToken(Token.ATTRIBUTE, anchor, length);
                                ConsumeToken(Token.QName, new Qname(sb.ToString()), anchor2, length2);
                                BeginToken();
                                ConsumeChar(Read());
                                m_states.Push(LexerState.Operator);
                                return;
                            }
                            else
                                throw new XQueryException(Properties.Resources.ExpectedBlockStart, ncname, sb.ToString());
                        }
                    }
                    else if (ncname == "processing-instruction")
                    {
                        SkipWhitespace();
                        if (Peek(0) == '{')
                        {
                            ConsumeToken(Token.PROCESSING_INSTRUCTION, anchor, length);
                            BeginToken();
                            ConsumeChar(Read());
                            m_states.Push(LexerState.Operator);
                            return;
                        }
                        else if (XmlCharType.Instance.IsStartNameChar(Peek(0)))
                        {
                            sb = new StringBuilder();
                            BeginToken();
                            while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsNameChar(c))
                                sb.Append(Read());
                            EndToken();
                            int anchor2 = m_anchor;
                            int length2 = m_length;
                            SkipWhitespace();
                            if (Peek(0) == '{')
                            {
                                ConsumeToken(Token.PROCESSING_INSTRUCTION, anchor, length);
                                ConsumeToken(Token.NCName, new Qname(sb.ToString()), anchor2, length2);
                                BeginToken();
                                ConsumeChar(Read());
                                m_states.Push(LexerState.Operator);
                                return;
                            }
                            else
                                throw new XQueryException(Properties.Resources.ExpectedBlockStart, ncname, sb.ToString());
                        }
                    }
                    ConsumeToken(Token.QName, new Qname(ncname));
                    SkipWhitespace();
                    if (Peek(0) != '(')
                        m_state = LexerState.Operator;
                }
            }
        }

        private void DeclareOrderingState()
        {
            if (MatchIdentifer("ordered"))
            {
                EndToken();
                ConsumeToken(Token.ORDERED);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("unordered"))
            {
                EndToken();
                ConsumeToken(Token.UNORDERED);
                m_state = LexerState.Default;
            }
            else
            {
                char c = Peek(0);
                if (c == 0)
                    throw new XQueryException(Properties.Resources.UnexpectedEOF);
                else
                    throw new XQueryException(Properties.Resources.UnexpectedChar, c);            
            }
        }

        private void VarNameState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            BeginToken();
            char c = Peek(0);
            //if (MatchText("(:"))
            //{
            //    m_states.Push(LexerState.VarName);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //} else
            if (XmlCharType.Instance.IsNCNameChar(c))
            {
                string prefix = String.Empty;
                StringBuilder sb = new StringBuilder();
                while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsNCNameChar(c))
                    sb.Append(Read());
                if (Peek(0) == ':' && XmlCharType.Instance.IsNCNameChar(Peek(1)))
                {
                    prefix = sb.ToString();                    
                    Read();
                    sb = new StringBuilder();
                    while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsNCNameChar(c))
                        sb.Append(Read());
                }
                EndToken();
                ConsumeToken(Token.VarName, new VarName(prefix, sb.ToString()));
                m_state = LexerState.Operator;
            }
        }

        private void OperatorState()
        {
            SkipWhitespace();
            BeginToken();
            char c = Peek(0);
            if (c == 0)
                ConsumeToken(0);
            //else if (MatchText("(:"))
            //{
            //    m_states.Push(m_state);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //}  
            else if (c == '{')
            {
                ConsumeChar(Read());
                m_states.Push(LexerState.Operator);
                m_state = LexerState.Default;
            }
            else if (c == '}')
            {
                ConsumeChar(Read());
                m_state = m_states.Pop();
            }
            else if (c == ';' || c == ',' || c == '=' || c == '+' || c == '-' || c == '[' || c == '|')
            {
                ConsumeChar(Read());
                m_state = LexerState.Default;
            }
            else if (c == '*')
            {
                Read();
                EndToken();
                ConsumeToken(Token.ML);
                m_state = LexerState.Default;
            }
            else if (c == ':' && Peek(1) == '=')
            {
                ConsumeChar(Read());
                BeginToken();
                ConsumeChar(Read());
                m_state = LexerState.Default;
            }
            else if (c == '!' && Peek(1) == '=')
            {
                ConsumeChar(Read());
                BeginToken();
                ConsumeChar(Read());
                m_state = LexerState.Default;
            }
            else if (c == '>')
            {
                ConsumeChar(Read());
                if (Peek(0) == '=' || Peek(0) == '>')
                {
                    BeginToken();
                    ConsumeChar(Read());
                }
                m_state = LexerState.Default;

            }
            else if (c == '<')
            {
                ConsumeChar(Read());
                if (Peek(0) == '=' || Peek(0) == '<')
                {
                    BeginToken();
                    ConsumeChar(Read());
                }
                m_state = LexerState.Default;

            }
            else if (c == '/')
            {
                if (Peek(1) == '/')
                {
                    Read();
                    Read();
                    EndToken();
                    ConsumeToken(Token.DOUBLE_SLASH);
                }
                else
                    ConsumeChar(Read());
                m_state = LexerState.Default;
            }
            else if (c == ')')
            {
                ConsumeChar(Read());
                SkipWhitespace();
                BeginToken();
                if (MatchIdentifer("as"))
                {
                    EndToken();
                    ConsumeToken(Token.AS);
                    m_state = LexerState.ItemType;
                }
            }
            else if (c == '?' || c == ']')
                ConsumeChar(Read());
            else if (MatchIdentifer("then"))
            {
                EndToken();
                ConsumeToken(Token.THEN);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("else"))
            {
                EndToken();
                ConsumeToken(Token.ELSE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("external"))
            {
                EndToken();
                ConsumeToken(Token.EXTERNAL);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("and"))
            {
                EndToken();
                ConsumeToken(Token.AND);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("at"))
            {
                EndToken();
                ConsumeToken(Token.AT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("div"))
            {
                EndToken();
                ConsumeToken(Token.DIV);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("except"))
            {
                EndToken();
                ConsumeToken(Token.EXCEPT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("idiv"))
            {
                EndToken();
                ConsumeToken(Token.IDIV);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("intersect"))
            {
                EndToken();
                ConsumeToken(Token.INTERSECT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("mod"))
            {
                EndToken();
                ConsumeToken(Token.MOD);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("order", "by"))
            {
                EndToken();
                ConsumeToken(Token.ORDER_BY);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("stable", "order", "by"))
            {
                EndToken();
                ConsumeToken(Token.STABLE_ORDER_BY);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("or"))
            {
                EndToken();
                ConsumeToken(Token.OR);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("return"))
            {
                EndToken();
                ConsumeToken(Token.RETURN);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("satisfies"))
            {
                EndToken();
                ConsumeToken(Token.SATISFIES);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("to"))
            {
                EndToken();
                ConsumeToken(Token.TO);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("union"))
            {
                EndToken();
                ConsumeToken(Token.UNION);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("where"))
            {
                EndToken();
                ConsumeToken(Token.WHERE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("preserve"))
            {
                EndToken();
                ConsumeToken(Token.PRESERVE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("strip"))
            {
                EndToken();
                ConsumeToken(Token.STRIP);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("castable", "as"))
            {
                EndToken();
                ConsumeToken(Token.CASTABLE_AS);
                m_state = LexerState.SingleType;
            }
            else if (MatchIdentifer("cast", "as"))
            {
                EndToken();
                ConsumeToken(Token.CAST_AS);
                m_state = LexerState.SingleType;
            }
            else if (MatchIdentifer("instance", "of"))
            {
                EndToken();
                ConsumeToken(Token.INSTANCE_OF);
                m_state = LexerState.ItemType;
            }
            else if (MatchIdentifer("treat", "as"))
            {
                EndToken();
                ConsumeToken(Token.TREAT_AS);
                m_state = LexerState.ItemType;
            }
            else if (MatchIdentifer("case"))
            {
                EndToken();
                ConsumeToken(Token.CASE);
                m_state = LexerState.ItemType;
            }
            else if (MatchIdentifer("in"))
            {
                EndToken();
                ConsumeToken(Token.IN);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("is"))
            {
                EndToken();
                ConsumeToken(Token.IS);
                m_state = LexerState.Default;
            }
            else if (c == '$')
            {
                ConsumeChar(Read());
                m_state = LexerState.VarName;
            }
            else if (MatchIdentifer("for"))
            {
                EndToken();
                ConsumeToken(Token.FOR);
                SkipWhitespace();
                BeginToken();
                if (Peek(0) == '$')
                    ConsumeChar(Read());
                else
                    throw new XQueryException(Properties.Resources.ExpectedVariablePrefix, "for");
                m_state = LexerState.VarName;
            }
            else if (MatchIdentifer("let"))
            {
                EndToken();
                ConsumeToken(Token.LET);
                SkipWhitespace();
                BeginToken();
                if (Peek(0) == '$')
                    ConsumeChar(Read());
                else
                    throw new XQueryException(Properties.Resources.ExpectedVariablePrefix, "let");
                m_state = LexerState.VarName;
            }
            else if (MatchIdentifer("empty", "greatest"))
            {
                EndToken();
                ConsumeToken(Token.EMPTY_GREATEST);
            }
            else if (MatchIdentifer("empty", "least"))
            {
                EndToken();
                ConsumeToken(Token.EMPTY_LEAST);
            }
            else if (MatchIdentifer("ascending"))
            {
                EndToken();
                ConsumeToken(Token.ASCENDING);
            }
            else if (MatchIdentifer("descending"))
            {
                EndToken();
                ConsumeToken(Token.DESCENDING);
            }
            else if (MatchIdentifer("default"))
            {
                EndToken();
                ConsumeToken(Token.DEFAULT);
            }
            else if (MatchIdentifer("collation"))
            {
                EndToken();
                ConsumeToken(Token.COLLATION);
                m_state = LexerState.UriToOperator;
            }
            else if (MatchIdentifer("as"))
            {
                EndToken();
                ConsumeToken(Token.AS);
                m_state = LexerState.ItemType;
            }
            else if (MatchIdentifer("eq"))
            {
                EndToken();
                ConsumeToken(Token.EQ);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("ge"))
            {
                EndToken();
                ConsumeToken(Token.GE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("gt"))
            {
                EndToken();
                ConsumeToken(Token.GT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("le"))
            {
                EndToken();
                ConsumeToken(Token.LE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("lt"))
            {
                EndToken();
                ConsumeToken(Token.LT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("ne"))
            {
                EndToken();
                ConsumeToken(Token.NE);
                m_state = LexerState.Default;
            }
            else if (c == '"' || c == '\'')
                ConsumeLiteral();
        }

       // private void ExprCommentState()
       // {            
       //     int n = 1;            
       //     while (true)
       //     {
       //         if (Peek(0) == 0)
       //             return;
       //         if (MatchText(":)"))
       //         {
       //             n--;
       //             if (n == 0)
       //                 break;
       //         }
       //         else if (MatchText("(:"))
       //             n++;
       //         else
       //             Read();
       //     }
       //     EndToken();
       //     if (NotIgnoreCommentsAndSpace)
       //     {
       //         m_state = m_states.Pop();
       //         ConsumeToken(Token.XQComment);
       //     }
       //     else
       //     {
       //         m_state = m_states.Pop();
       //         EnterState();
       //     }            
       //}

        private void PragmaState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            ConsumeQName();
            BeginToken();
            StringBuilder sb = new StringBuilder();
            char c;
            while ((c = Peek(0)) != 0)
            {
                if (c == '#' && Peek(1) == ')')
                    break;
                sb.Append(Read());
            }
            EndToken();
            ConsumeToken(Token.PragmaContents, sb.ToString().Trim());
            BeginToken();
            Read(); // #
            Read(); // )
            EndToken();
            ConsumeToken(Token.PRAGMA_END);
            m_state = LexerState.Default;
        }

        private void XQueryVersionState()
        {            
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            BeginToken();
            char c = Peek(0);
            if (c == ';')
            {
                ConsumeChar(Read());
                m_state = LexerState.Default;
            }
            else if (c == '"' || c == '\'')
                ConsumeLiteral();
            else if (MatchIdentifer("encoding"))
            {
                EndToken();
                ConsumeToken(Token.ENCODING);
            }
        }

        private void NamespaceDeclState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            BeginToken();
            char c = Peek(0);
            if (c == ',' || c == '=')
                ConsumeChar(Read());
            else if (c == '\'' || c == '"')
                ConsumeLiteral();
            else if (MatchIdentifer("at"))
            {
                EndToken();
                ConsumeToken(Token.AT);
            }
            //else if (MatchText("(:"))
            //{
            //    m_states.Push(m_state);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //} 
            else if (XmlCharType.Instance.IsNCNameChar(c))
                ConsumeNCName();
            else if (c == ';')
            {
                ConsumeChar(Read());
                m_state = LexerState.Default;
            }
        }

        private void UriToOperatorState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            char c = Peek(0);
            if (c == '\'' || c == '"')
            {
                ConsumeLiteral();
                m_state = LexerState.Operator;
            }
        }

        private void NamespaceKeywordState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            BeginToken();
            char c = Peek(0);
            if (c == '\'' || c == '"')
            {
                ConsumeLiteral();
                m_state = LexerState.NamespaceDecl;
            }
            else if (MatchIdentifer("inherit"))
            {
                EndToken();
                ConsumeToken(Token.INHERIT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("no-inherit"))
            {
                EndToken();
                ConsumeToken(Token.NO_INHERIT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("namespace"))
            {
                EndToken();
                ConsumeToken(Token.NAMESPACE);
                m_state = LexerState.NamespaceDecl;
            }
            //else if (MatchText("(:"))
            //{
            //    m_states.Push(m_state);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //}
            else if (MatchIdentifer("default", "element"))
            {
                EndToken();
                ConsumeToken(Token.DEFAULT_ELEMENT);
                m_state = LexerState.NamespaceKeyword;
            }
            else if (MatchIdentifer("preserve"))
            {
                EndToken();
                ConsumeToken(Token.PRESERVE);
                m_state = LexerState.NamespaceKeyword;
            }
            else if (MatchIdentifer("no-preserve"))
            {
                EndToken();
                ConsumeToken(Token.NO_PRESERVE);
                m_state = LexerState.NamespaceKeyword;
            }
            else if (c == ',')
            {
                ConsumeChar(Read());
                m_state = LexerState.NamespaceKeyword;
            }
        }

        private void XmlSpace_DeclState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            BeginToken();
            if (MatchIdentifer("preserve"))
            {
                EndToken();
                ConsumeToken(Token.PRESERVE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("strip"))
            {
                EndToken();
                ConsumeToken(Token.STRIP);
                m_state = LexerState.Default;
            }
            //else if (MatchText("(:"))
            //{
            //    m_states.Push(m_state);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //}
        }

        private void SingleTypeState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            if (XmlCharType.Instance.IsNameChar(Peek(0)))
            {
                ConsumeQName();
                m_state = LexerState.Operator;
            }
            //else if (MatchText("(:"))
            //{
            //    m_states.Push(m_state);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //}
        }

        private void ItemTypeState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            BeginToken();
            char c = Peek(0);
            if (c == '$')
            {
                ConsumeChar(Read());
                m_state = LexerState.VarName;
            }
            else if (MatchIdentifer("empty-sequence", "(", ")"))
            {
                EndToken();
                ConsumeToken(Token.EMPTY_SEQUENCE);
                m_state = LexerState.Operator;
            }
            //else if (MatchText("(:"))
            //{
            //    m_states.Push(m_state);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //}
            else if (MatchIdentifer("element", "("))
            {
                EndToken("element");
                ConsumeToken(Token.ELEMENT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.OccurrenceIndicator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("attribute", "("))
            {
                EndToken("attribute");
                ConsumeToken(Token.ATTRIBUTE);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.OccurrenceIndicator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("schema-element", "("))
            {
                EndToken("schema-element");
                ConsumeToken(Token.SCHEMA_ELEMENT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.OccurrenceIndicator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("schema-attribute", "("))
            {
                EndToken("schema-attribute");
                ConsumeToken(Token.SCHEMA_ATTRIBUTE);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.OccurrenceIndicator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("comment", "("))
            {
                EndToken("comment");
                ConsumeToken(Token.COMMENT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.OccurrenceIndicator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("text", "("))
            {
                EndToken("text");
                ConsumeToken(Token.TEXT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');                
                m_states.Push(LexerState.OccurrenceIndicator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("node", "("))
            {
                EndToken("node");
                ConsumeToken(Token.NODE);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');                
                m_states.Push(LexerState.OccurrenceIndicator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("document-node", "("))
            {
                EndToken("document-node");
                ConsumeToken(Token.DOCUMENT_NODE);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.OccurrenceIndicator);
                m_state = LexerState.KindTest;
            }
            else if (MatchIdentifer("processing-instruction", "("))
            {
                EndToken("processing-instruction");
                ConsumeToken(Token.PROCESSING_INSTRUCTION);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.OccurrenceIndicator);
                m_state = LexerState.KindTestForPi;
            }
            else if (MatchIdentifer("item", "(", ")"))
            {
                EndToken();
                ConsumeToken(Token.ITEM);
                m_state = LexerState.OccurrenceIndicator;
            }
            else if (c == ';')
            {
                ConsumeChar(Read());
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("then"))
            {
                EndToken();
                ConsumeToken(Token.THEN);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("else"))
            {
                EndToken();
                ConsumeToken(Token.ELSE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("at"))
            {
                EndToken();
                ConsumeToken(Token.AT);
                SkipWhitespace();
                c = Peek(0);
                if (c == '\'' || c == '"')
                {
                    ConsumeLiteral();
                    m_state = LexerState.NamespaceDecl;
                }
                else
                    m_state = LexerState.Default;
            }
            else if (c == '=' || c == '(' || c == '[' || c == '|')
            {
                ConsumeChar(Read());
                m_state = LexerState.Default;
            }
            else if (c == ':' && Peek(1) == '=')
            {
                ConsumeChar(Read());
                BeginToken();
                ConsumeChar(Read());
                m_state = LexerState.Default;
            }
            else if (c == '!' && Peek(1) == '=')
            {
                ConsumeChar(Read());
                BeginToken();
                ConsumeChar(Read());
                m_state = LexerState.Default;
            }
            else if (c == '>')
            {
                ConsumeChar(Read());
                if (Peek(0) == '=' || Peek(0) == '>')
                {
                    BeginToken();
                    ConsumeChar(Read());
                }
                m_state = LexerState.Default;

            }
            else if (c == '<')
            {
                ConsumeChar(Read());
                if (Peek(0) == '=' || Peek(0) == '<')
                {
                    BeginToken();
                    ConsumeChar(Read());
                }
                m_state = LexerState.Default;

            }
            else if (c == ')')
            {
                ConsumeChar(Read());
                SkipWhitespace();
                BeginToken();
                if (MatchIdentifer("as"))
                {
                    EndToken();
                    ConsumeToken(Token.AS);
                    m_state = LexerState.ItemType;
                }
            }
            else if (MatchIdentifer("external"))
            {
                EndToken();
                ConsumeToken(Token.EXCEPT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("and"))
            {
                EndToken();
                ConsumeToken(Token.AND);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("at"))
            {
                EndToken();
                ConsumeToken(Token.AT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("div"))
            {
                EndToken();
                ConsumeToken(Token.DIV);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("except"))
            {
                EndToken();
                ConsumeToken(Token.EXCEPT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("eq"))
            {
                EndToken();
                ConsumeToken(Token.EQ);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("ge"))
            {
                EndToken();
                ConsumeToken(Token.GE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("gt"))
            {
                EndToken();
                ConsumeToken(Token.GT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("le"))
            {
                EndToken();
                ConsumeToken(Token.LE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("lt"))
            {
                EndToken();
                ConsumeToken(Token.LT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("ne"))
            {
                EndToken();
                ConsumeToken(Token.NE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("idiv"))
            {
                EndToken();
                ConsumeToken(Token.IDIV);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("intersect"))
            {
                EndToken();
                ConsumeToken(Token.INTERSECT);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("mod"))
            {
                EndToken();
                ConsumeToken(Token.MOD);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("order", "by"))
            {
                EndToken();
                ConsumeToken(Token.ORDER_BY);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("stable", "order", "by"))
            {
                EndToken();
                ConsumeToken(Token.STABLE_ORDER_BY);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("or"))
            {
                EndToken();
                ConsumeToken(Token.OR);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("return"))
            {
                EndToken();
                ConsumeToken(Token.RETURN);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("satisfies"))
            {
                EndToken();
                ConsumeToken(Token.SATISFIES);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("to"))
            {
                EndToken();
                ConsumeToken(Token.TO);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("union"))
            {
                EndToken();
                ConsumeToken(Token.UNION);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("where"))
            {
                EndToken();
                ConsumeToken(Token.WHERE);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("castable", "as"))
            {
                EndToken();
                ConsumeToken(Token.CASTABLE_AS);
                m_state = LexerState.SingleType;
            }
            else if (MatchIdentifer("cast", "as"))
            {
                EndToken();
                ConsumeToken(Token.CAST_AS);
                m_state = LexerState.SingleType;
            }
            else if (MatchIdentifer("instance", "of"))
            {
                EndToken();
                ConsumeToken(Token.INSTANCE_OF);
            }
            else if (MatchIdentifer("treat", "as"))
            {
                EndToken();
                ConsumeToken(Token.TREAT_AS);
            }
            else if (MatchIdentifer("case"))
            {
                EndToken();
                ConsumeToken(Token.CASE);
            }
            else if (MatchIdentifer("as"))
            {
                EndToken();
                ConsumeToken(Token.AS);
            }
            else if (MatchIdentifer("in"))
            {
                EndToken();
                ConsumeToken(Token.IN);
                m_state = LexerState.Default;
            }
            else if (MatchIdentifer("is"))
            {
                EndToken();
                ConsumeToken(Token.IS);
                m_state = LexerState.Default;
            }
            else if (XmlCharType.Instance.IsNameChar(c))
            {
                ConsumeQName();
                m_state = LexerState.OccurrenceIndicator;
            }
        }

        private void KindTestState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            BeginToken();
            char c = Peek(0);
            if (c == '{')
            {
                ConsumeChar(Read());
                m_states.Push(LexerState.Operator);
                m_state = LexerState.Default;
            }
            else if (c == ')')
            {
                ConsumeChar(Read());
                m_state = m_states.Pop();
            }
            else if (c == '*')
            {
                ConsumeChar(Read());
                m_state = LexerState.CloseKindTest;
            }
            else if (MatchIdentifer("element", "("))
            {
                EndToken("element");
                ConsumeToken(Token.ELEMENT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.KindTest);
            }
            else if (MatchIdentifer("schema-element", "("))
            {
                EndToken("schema-element");
                ConsumeToken(Token.SCHEMA_ELEMENT);
                BeginToken(m_bookmark[1]);
                ConsumeChar('(');
                m_states.Push(LexerState.KindTest);
            }
            else if (XmlCharType.Instance.IsNameChar(c))
            {
                ConsumeQName();
                m_state = LexerState.CloseKindTest;
            }
            //else if (MatchText("(:"))
            //{
            //    m_states.Push(m_state);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //}
        }

        private void KindTestForPiState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            char c = Peek(0);
            BeginToken();
            if (c == ')')
            {
                ConsumeChar(Read());
                m_state = m_states.Pop();
            }
            //else if (MatchText("(:"))
            //{
            //    m_states.Push(m_state);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //}
            else if (XmlCharType.Instance.IsNCNameChar(c))
                ConsumeNCName();
            else if (c == '\'' || c == '"')
                ConsumeLiteral();
        }

        private void CloseKindTestState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            char c = Peek(0);
            BeginToken();
            if (c == ')')
            {
                ConsumeChar(Read());
                m_state = m_states.Pop();
            }
            else if (c == ',')
            {
                ConsumeChar(Read());
                m_state = LexerState.KindTest;
            }
            else if (c == '{')
            {
                ConsumeChar(Read());
                m_states.Push(LexerState.Operator);
                m_state = LexerState.Default;
            }
            else if (c == '?')
                ConsumeChar(Read());
            //else if (MatchText("(:"))
            //{
            //    m_states.Push(m_state);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //}
        }

        private void OccurrenceIndicatorState()
        {
            SkipWhitespace();
            BeginToken();
            //if (MatchText("(:"))
            //{
            //    m_states.Push(m_state);
            //    m_state = LexerState.ExprComment;
            //    ExprCommentState();
            //}
            //else
            {
                char c = Peek(0);
                if (c == '*')
                {
                    //if (!(XmlCharType.Instance.IsNameChar(Peek(1)) || XmlCharType.Instance.IsDigit)
                    //{
                    Read();
                    EndToken();
                    ConsumeToken(Token.Indicator1);
                }
                else if (c == '+')
                {
                    Read();
                    EndToken();
                    ConsumeToken(Token.Indicator2);
                }
                else if (c == '?')
                {
                    Read();
                    EndToken();
                    ConsumeToken(Token.Indicator3);
                }
                m_state = LexerState.Operator;
                OperatorState();
            }
        }

        private void OptionState()
        {
            SkipWhitespace();
            if (Peek(0) == 0)
                return;
            if (XmlCharType.Instance.IsStartNameChar(Peek(0)))
            {
                ConsumeQName();
                m_state = LexerState.Default;
            }
        }

        private void StartTagState()
        {
            if (Peek(0) == 0)
                return;
            char c = Peek(0);
            BeginToken();
            if (MatchText("/>"))
            {                
                ConsumeChar('/');
                m_anchor++;
                ConsumeChar('>');
                m_state = m_states.Pop();
            }
            else if (c == '>')
            {
                ConsumeChar(Read());
                m_state = LexerState.ElementContent;
            }
            else if (XmlCharType.Instance.IsWhiteSpace(c))
                ConsumeS();
            else if (XmlCharType.Instance.IsStartNameChar(c))
            {
                ConsumeQName();
                m_state = LexerState.AttributeState;
            }
        }

        private void TagAttributeState()
        {
            if (Peek(0) == 0)
                return;
            char c = Peek(0);
            BeginToken();
            if (MatchText("/>"))
            {
                ConsumeChar('/');
                m_anchor++;
                ConsumeChar('>');
                m_state = m_states.Pop();
            }
            else if (c == '>')
            {
                ConsumeChar(Read());
                m_state = LexerState.ElementContent;
            }
            else if (c == '"')
            {
                ConsumeChar(Read());
                m_state = LexerState.QuotAttributeContent;
            }
            else if (c == '\'')
            {
                Read();
                EndToken();
                ConsumeToken(Token.Apos);
                m_state = LexerState.AposAttributeContent;
            }
            else if (c == '=')
                ConsumeChar(Read());
            else if (XmlCharType.Instance.IsWhiteSpace(c))
                ConsumeS();
            else if (XmlCharType.Instance.IsStartNameChar(c))
                ConsumeQName();
        }

        private void ElementContentState()
        {
            BeginToken();
            char c = Peek(0);
            if (c == 0)
                ConsumeToken(0); // EOF
            else if (MatchText("</"))
            {
                ConsumeChar('<');
                m_anchor++;
                ConsumeChar('/');
                m_state = LexerState.EndTag;
            }
            else if (MatchText("{{"))
            {
                ConsumeChar('{');
                m_anchor++;
                ConsumeChar('{');
            }
            else if (MatchText("}}"))
            {
                ConsumeChar('}');
                m_anchor++;
                ConsumeChar('}');
            }
            else if (c == '{')
            {
                ConsumeChar(Read());
                m_states.Push(m_state);
                m_state = LexerState.Default;
            }
            else if (MatchText("<!--"))
            {
                EndToken();
                ConsumeToken(Token.COMMENT_BEGIN);
                m_states.Push(m_state);
                m_state = LexerState.XmlComment;
            }
            else if (MatchText("<?"))
            {
                EndToken();
                ConsumeToken(Token.PI_BEGIN);
                m_states.Push(m_state);
                m_state = LexerState.ProcessingInstruction;
            }
            else if (MatchText("<![CDATA["))
            {
                EndToken();
                ConsumeToken(Token.CDATA_BEGIN);
                m_states.Push(m_state);
                m_state = LexerState.CDataSection;
            }
            else if (c == '<')
            {
                Read();
                EndToken();
                ConsumeToken(Token.BeginTag);
                m_states.Push(m_state);
                m_state = LexerState.StartTag;
            }
            else if (MatchText("&gt;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&gt;"));
            }
            else if (MatchText("&lt;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&lt;"));
            }
            else if (MatchText("&amp;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&amp;"));
            }
            else if (MatchText("&quot;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&quot;"));
            }
            else if (MatchText("&apos;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&apos;"));
            }
            else if (MatchText("&#x"))
                ConsumeCharRefHex();
            else if (MatchText("&#"))
                ConsumeCharRef();
            else
            {
                StringBuilder sb = new StringBuilder();
                while ((c = Peek(0)) != 0 && c != '<' && c != '&' && c != '{' && c != '}')
                    sb.Append(Read());
                EndToken();
                if (sb.Length == 0)
                    return;
                ConsumeToken(Token.Char, new Literal(sb.ToString()));
            }
        }

        private void EndTagState()
        {
            if (Peek(0) == 0)
                return;
            BeginToken();
            char c = Peek(0);
            if (c == '>')
            {
                ConsumeChar(Read());
                m_state = m_states.Pop();
            }
            else if (XmlCharType.Instance.IsWhiteSpace(c))
                ConsumeS();
            else if (XmlCharType.Instance.IsStartNameChar(c))
                ConsumeQName();
        }

        private void XmlCommentState()
        {
            BeginToken();
            StringBuilder sb = new StringBuilder();
            char c;
            while (!((c = Peek(0)) == '-' && Peek(1) == '-' && Peek(2) == '>'))
            {
                if (Peek(0) == 0)
                    return;
                sb.Append(Read());
            }
            EndToken();
            ConsumeToken(Token.StringLiteral, new Literal(sb.ToString()));
            BeginToken();
            Read(); // -
            Read(); // -
            Read(); // >
            EndToken();
            ConsumeToken(Token.COMMENT_END);
            m_state = m_states.Pop();
        }

        private void ProcessingInstructionState()
        {
            if (Peek(0) == 0)
                return;
            BeginToken();
            char c = Peek(0);
            if (XmlCharType.Instance.IsWhiteSpace(c))
            {
                ConsumeS();
                m_state = LexerState.ProcessingInstructionContent;
            }
            else if (MatchText("?>"))
            {
                EndToken();
                ConsumeToken(Token.PI_END);
                m_state = m_states.Pop();
            }
            else if (XmlCharType.Instance.IsStartNameChar(c))
            {
                StringBuilder sb = new StringBuilder();
                while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsNameChar(c))
                    sb.Append(Read());
                EndToken();
                if (sb.ToString() == "xml")
                    throw new XQueryException(Properties.Resources.InvalidPITarget);
                ConsumeToken(Token.StringLiteral, new Literal(sb.ToString()));
            }
        }

        private void ProcessingInstructionContentState()
        {
            if (Peek(0) == 0)
                return;
            StringBuilder sb = new StringBuilder();
            char c;
            BeginToken();
            while (!((c = Peek(0)) == '?' && Peek(1) == '>'))
            {
                if (Peek(0) == 0)
                    return;
                sb.Append(Read());
            }
            EndToken();
            ConsumeToken(Token.StringLiteral, new Literal(sb.ToString()));
            BeginToken();
            Read(); // ?
            Read(); // >
            EndToken();
            ConsumeToken(Token.PI_END);
            m_state = m_states.Pop();
        }

        private void CDataSectionState()
        {
            StringBuilder sb = new StringBuilder();
            char c;
            BeginToken();
            while (!((c = Peek(0)) == ']' && Peek(1) == ']' && Peek(2) == '>'))
            {
                if (Peek(0) == 0)
                    return;
                sb.Append(Read());
            }
            EndToken();
            ConsumeToken(Token.StringLiteral, new Literal(sb.ToString()));
            BeginToken();
            Read(); // ]
            Read(); // ]
            Read(); // >
            EndToken();
            ConsumeToken(Token.CDATA_END);
            m_state = m_states.Pop();
        }

        private void QuotAttributeContentState()
        {
            if (Peek(0) == 0)
                return;
            BeginToken();
            char c = Peek(0);
            if (c == '"' && Peek(1) != '"')
            {
                ConsumeChar(Read());
                m_state = LexerState.AttributeState;
            }
            else if (MatchText("{{"))
            {
                ConsumeChar('{');
                m_anchor++;
                ConsumeChar('{');
            }
            else if (MatchText("}}"))
            {
                ConsumeChar('}');
                m_anchor++;
                ConsumeChar('}');
            }
            else if (c == '{')
            {
                ConsumeChar(Read());
                m_states.Push(m_state);
                m_state = LexerState.Default;
            }
            else if (MatchText("&gt;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&gt;"));
            }
            else if (MatchText("&lt;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&lt;"));
            }
            else if (MatchText("&amp;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&amp;"));
            }
            else if (MatchText("&quot;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&quot;"));
            }
            else if (MatchText("&apos;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&apos;"));
            }
            else if (MatchText("&#x"))
                ConsumeCharRefHex();
            else if (MatchText("&#"))
                ConsumeCharRef();
            else if (c == '"' && Peek(1) == '"')
            {
                Read();
                Read();
                EndToken();
                ConsumeToken(Token.EscapeQuot);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                while ((c = Peek(0)) != 0 && c != '{' && c != '&' && c != '"')
                    sb.Append(Read());
                EndToken();
                if (sb.Length == 0)
                    return;
                ConsumeToken(Token.Char, new Literal(sb.ToString()));
            }
        }

        private void AposAttributeContentState()
        {
            if (Peek(0) == 0)
                return;
            char c = Peek(0);
            BeginToken();
            if (c == '\'' && Peek(1) != '\'')
            {
                Read();
                EndToken();
                ConsumeToken(Token.Apos);
                m_state = LexerState.AttributeState;
            }
            else if (MatchText("{{"))
            {
                ConsumeChar('{');
                m_anchor++;
                ConsumeChar('{');
            }
            else if (MatchText("}}"))
            {
                ConsumeChar('}');
                m_anchor++;
                ConsumeChar('}');
            }
            else if (c == '{')
            {
                ConsumeChar(Read());
                m_states.Push(m_state);
                m_state = LexerState.Default;
            }
            else if (MatchText("&gt;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&gt;"));
            }
            else if (MatchText("&lt;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&lt;"));
            }
            else if (MatchText("&amp;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&amp;"));
            }
            else if (MatchText("&quot;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&quot;"));
            }
            else if (MatchText("&apos;"))
            {
                EndToken();
                ConsumeToken(Token.PredefinedEntityRef, new PredefinedEntityRef("&apos;"));
            }
            else if (MatchText("&#x"))
                ConsumeCharRefHex();
            else if (MatchText("&#"))
                ConsumeCharRef();
            else if (c == '\'' && Peek(1) == '\'')
            {
                Read();
                Read();
                EndToken();
                ConsumeToken(Token.EscapeApos);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                while ((c = Peek(0)) != 0 && c != '{' && c != '&' && c != '\'')
                    sb.Append(Read());
                EndToken();
                ConsumeToken(Token.Char, new Literal(sb.ToString()));
            }
        }

        private struct TokenizerState
        {
            public LexerState current;
            public LexerState[] states;
            public CurrentToken[] tokens;
        }

        public object GetState()
        {
            TokenizerState stateData;
            stateData.current = m_state;
            stateData.states = m_states.ToArray();
            stateData.tokens = m_token.ToArray();
            return stateData;
        }

        public void RevertToState(object data)
        {
            TokenizerState stateData = (TokenizerState)data;
            m_state = stateData.current;
            m_states = new Stack<LexerState>(stateData.states);
            m_token = new Queue<CurrentToken>(stateData.tokens);
        }

        private void EnterState()
        {
            switch (m_state)
            {
                case LexerState.Default:
                    DefaultState();
                    break;

                case LexerState.Operator:
                    OperatorState();
                    break;

                case LexerState.DeclareOrdering:
                    DeclareOrderingState();
                    break;

                case LexerState.VarName:
                    VarNameState();
                    break;

                //case LexerState.ExprComment:
                //    ExprCommentState();
                //    break;

                case LexerState.Pragma:
                    PragmaState();
                    break;

                case LexerState.XQueryVersion:
                    XQueryVersionState();
                    break;

                case LexerState.NamespaceDecl:
                    NamespaceDeclState();
                    break;

                case LexerState.UriToOperator:
                    UriToOperatorState();
                    break;

                case LexerState.NamespaceKeyword:
                    NamespaceKeywordState();
                    break;

                case LexerState.XmlSpace_Decl:
                    XmlSpace_DeclState();
                    break;

                case LexerState.SingleType:
                    SingleTypeState();
                    break;

                case LexerState.ItemType:
                    ItemTypeState();
                    break;

                case LexerState.KindTest:
                    KindTestState();
                    break;

                case LexerState.KindTestForPi:
                    KindTestForPiState();
                    break;

                case LexerState.CloseKindTest:
                    CloseKindTestState();
                    break;

                case LexerState.OccurrenceIndicator:
                    OccurrenceIndicatorState();
                    break;

                case LexerState.Option:
                    OptionState();
                    break;

                case LexerState.StartTag:
                    StartTagState();
                    break;

                case LexerState.AttributeState:
                    TagAttributeState();
                    break;

                case LexerState.ElementContent:
                    ElementContentState();
                    break;

                case LexerState.EndTag:
                    EndTagState();
                    break;

                case LexerState.XmlComment:
                    XmlCommentState();
                    break;

                case LexerState.ProcessingInstruction:
                    ProcessingInstructionState();
                    break;

                case LexerState.ProcessingInstructionContent:
                    ProcessingInstructionContentState();
                    break;

                case LexerState.CDataSection:
                    CDataSectionState();
                    break;

                case LexerState.QuotAttributeContent:
                    QuotAttributeContentState();
                    break;

                case LexerState.AposAttributeContent:
                    AposAttributeContentState();
                    break;
            }
        }

        #region yyInput Members

        public bool advance()
        {
            return m_token.Count > 0 || Peek(0) != 0;
        }

        public int token()
        {
            if (m_token.Count == 0)
            {
                EnterState();
                if (m_token.Count == 0)
                {
                    m_value = null;                    
                    return Token.yyErrorCode;
                }
            }
            CurrentToken curr = m_token.Dequeue();
            m_value = curr.value;
            CurrentPos = curr.anchor;
            CurrentLength = curr.length;
            CurrentState = curr.state;
            return curr.token;
        }

        public object value()
        {
            return m_value;
        }

        #endregion

        public int CurrentPos { get; private set; }

        public int CurrentLength { get; private set; }
    }
}
