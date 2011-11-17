//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

using DataEngine.XQuery.DTD.yyParser;
using DataEngine.CoreServices;
using System.Net;


namespace DataEngine.XQuery.DTD
{
    internal class Tokenizer : yyInput
    {
        private string m_baseUri;
        private TextReader m_reader;
        private object m_value;

        private int m_position;
        private StringBuilder m_buffer = new StringBuilder();
        private Dictionary<string, int> s_mapTokens = new Dictionary<string, int>();
        private XmlCharType charType = XmlCharType.Instance;

        private class PE
        {
            public string name;
            public string publicID;
            public string systemID;
            public string body;

            public PE(string name, string body)
            {
                this.name = name;
                this.body = body;
            }

            public PE(string name, string publicID, string systemID)
            {
                this.name = name;
                this.publicID = publicID;
                this.systemID = systemID;
            }

            public override string ToString()
            {                
                return String.Format("%{0};", name);
            }
        }

        private Dictionary<string, PE> pe = new Dictionary<string, PE>();

        public Tokenizer(string baseUri, string input)
        {
            m_baseUri = baseUri;
            m_reader = new StringReader(input);
            FillHashtab();
        }

        public Tokenizer(string publicId, string systemId, string baseUri)
        {
            m_baseUri = baseUri;
            m_reader = new StringReader(GetPE(publicId, systemId, baseUri));
            FillHashtab();
        }

        private void FillHashtab()
        {
            s_mapTokens.Add("EMPTY", Token.EMPTY);
            s_mapTokens.Add("ANY", Token.ANY);
            s_mapTokens.Add("CDATA", Token.CDATA);
            s_mapTokens.Add("ID", Token.ID);
            s_mapTokens.Add("IDREF", Token.IDREF);
            s_mapTokens.Add("IDREFS", Token.IDREFS);
            s_mapTokens.Add("ENTITY", Token.ENTITY);
            s_mapTokens.Add("ENTITIES", Token.ENTITIES);
            s_mapTokens.Add("NMTOKEN", Token.NMTOKEN);
            s_mapTokens.Add("NMTOKENS", Token.NMTOKENS);
            s_mapTokens.Add("NOTATION", Token.NOTATION);
            s_mapTokens.Add("NDATA", Token.NDATA);
            s_mapTokens.Add("#REQUIRED", Token.REQUIRED);
            s_mapTokens.Add("#IMPLIED", Token.IMPLIED);
            s_mapTokens.Add("#FIXED", Token.FIXED);
            s_mapTokens.Add("SYSTEM", Token.SYSTEM);
            s_mapTokens.Add("PUBLIC", Token.PUBLIC);
            s_mapTokens.Add("#PCDATA", Token.PCDATA);
        }

        private char Peek(int lookahead)
        {
            while (lookahead >= m_buffer.Length && m_reader.Peek() != -1)
                m_buffer.Append((char)m_reader.Read());
            if (lookahead < m_buffer.Length)
                return m_buffer[lookahead];
            else
                return '\0';
        }

        private char Read()
        {
            char ch;
            if (m_buffer.Length > 0)
            {
                ch = m_buffer[0];
                m_buffer.Remove(0, 1);
            }
            else
            {
                int c = m_reader.Read();
                if (c == -1)
                    return '\0';
                else
                    ch = (char)c;
            }
            if (ch != '\r')
            {
                if (ch == '\n')
                {
                    LineNo++;
                    ColNo = 1;
                }
                else
                    ++ColNo;
            }
            m_position++;
            return ch;
        }

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

        private void SkipWhitespace()
        {
            if (charType.IsWhiteSpace(Peek(0)))
            {
                char c;
                while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsWhiteSpace(c))
                    Read();
            }
        }

        private String ExpandReferences(string text)
        {
            StringBuilder sb = new StringBuilder(text);
            return sb.ToString();
        }

        public void AddPE(string name, string body)
        {
            pe.Add(String.Format("%{0};", name), new PE(name, body));
        }

        public void AddPE(string name, string publicID, string systemID)
        {
            pe.Add(String.Format("%{0};", name), new PE(name, publicID, systemID));
        }

        private String GetPE(string publicId, string systemId, string baseUri)
        {
            if (Uri.IsWellFormedUriString(systemId, UriKind.Absolute) ||
                (baseUri != null && Uri.IsWellFormedUriString(baseUri, UriKind.Absolute)))
            {
                WebClient client = new WebClient();
                client.Proxy.Credentials = CredentialCache.DefaultCredentials;
                string absolutePath;
                if (m_baseUri != null)
                    absolutePath = new Uri(new Uri(baseUri), systemId).AbsoluteUri;
                else
                    absolutePath = systemId;
                return client.DownloadString(absolutePath);
            }
            else
            {
                string absolutePath;
                if (baseUri != null)
                    absolutePath = Path.Combine(Path.GetDirectoryName(new Uri(baseUri).AbsolutePath), systemId);
                else
                    absolutePath = systemId;
                TextReader reader = new StreamReader(new FileStream(absolutePath,
                    FileMode.Open, FileAccess.Read));
                string res = reader.ReadToEnd();
                reader.Close();
                return res;
            }
        }

        private String ExpandPEReference(string name)
        {
            PE entity;
            if (!pe.TryGetValue(name, out entity))
                throw new XQueryException("Can't expand PEReference {0}", name);
            if (entity.body == null)
                entity.body = GetPE(entity.publicID, entity.systemID, m_baseUri);
            return entity.body;
        }

        #region yyInput Members

        public bool advance()
        {
            return Peek(0) != 0;
        }

        public int token()
        {
            m_value = null;
            while (true)
            {
                SkipWhitespace();
                char c = Peek(0);
                if (c == '<')
                {
                    Read();
                    c = Peek(0);
                    if (c == '!' && Peek(1) != '[')
                    {
                        Read();
                        if (MatchText("ELEMENT"))
                            return Token.ELEMENT_DECL;
                        else if (MatchText("ATTLIST"))
                            return Token.ATTLIST_DECL;
                        else if (MatchText("ENTITY"))
                            return Token.ENTITY_DECL;
                        else if (MatchText("NOTATION"))
                            return Token.NOTATION_DECL;
                        else if (MatchText("--"))
                        {
                            StringBuilder sb = new StringBuilder();
                            while (!((c = Peek(0)) == '-' && Peek(1) == '-' && Peek(2) == '>'))
                            {
                                if (Peek(0) == 0)
                                    throw new XQueryException("Unexpected end-of-file in comment");
                                sb.Append(Read());
                            }
                            Read();
                            Read();
                            Read();
                            m_value = Lisp.List(DTDID.Comment, sb.ToString());
                            return Token.Comment;
                        }
                    }
                    else if (c == '?')
                    {
                        StringBuilder sb = new StringBuilder();
                        while ((c = Peek(0)) != 0 && XmlCharType.Instance.IsNameChar(c))
                            sb.Append(Read());
                        if (sb.ToString() == "xml")
                            throw new XQueryException(Properties.Resources.InvalidPITarget);
                        StringBuilder sb2 = new StringBuilder();
                        while (!((c = Peek(0)) == '?' && Peek(1) == '>'))
                        {
                            if (Peek(0) == 0)
                                throw new XQueryException("Unexpected end-of-file in processing-instruction");
                            sb2.Append(Read());
                        }
                        Read();
                        Read();
                        m_value = Lisp.List(DTDID.PI, sb.ToString(), sb2.ToString());
                        return Token.PI;
                    }
                    else if (MatchText("[[IGNORE"))
                    {
                        while (!((c = Peek(0)) == ']' && Peek(1) == ']' && Peek(2) == '>'))
                        {
                            if (Peek(0) == 0)
                                throw new XQueryException("Unexpected end-of-file in <[[IGNORE operator");
                            Read();
                        }
                        Read();
                        Read();
                        Read();
                        return Token.IGNORE;
                    }
                    else if (MatchText("[[INCLUDE"))
                        return Token.INCLUDE;
                    return '<';
                }
                else if (c == '#')
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Read());
                    while ((c = Peek(0)) != 0 && charType.IsNameChar(c))
                        sb.Append(Read());
                    string name = sb.ToString();
                    int tok;
                    if (s_mapTokens.TryGetValue(name, out tok))
                        return tok;
                    throw new XQueryException("Invalid DTD token {0} at line {1} col {2}", name, LineNo, ColNo);
                }
                else if (charType.IsNameChar(c))
                {
                    StringBuilder sb = new StringBuilder();
                    while ((c = Peek(0)) != 0 && charType.IsNameChar(c))
                        sb.Append(Read());
                    string name = sb.ToString();
                    int tok;
                    if (s_mapTokens.TryGetValue(name, out tok))
                        return tok;
                    m_value = name;
                    return Token.Name;
                }
                else if (c == '"' || c == '\'')
                {
                    StringBuilder sb = new StringBuilder();
                    char qoute = Read();
                    while ((c = Peek(0)) != 0 && c != qoute)
                        sb.Append(Read());
                    Read();
                    m_value = ExpandReferences(sb.ToString());
                    return Token.Literal;
                }
                else if (c == '%' && charType.IsNameChar(Peek(1)))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Read());
                    while ((c = Peek(0)) != 0 && charType.IsNameChar(c))
                        sb.Append(Read());
                    sb.Append(Read());
                    if (c != ';')
                        throw new XQueryException("Unproperly formatted PEReference	{0}", sb.ToString());
                    m_buffer.Insert(0, ExpandPEReference(sb.ToString()));
                    continue;
                }
                return Read();
            }
        }

        public object value()
        {
            return m_value;
        }

        #endregion

        public int ColNo { get; private set; }

        public int LineNo { get; private set; }

        public int Position
        {
            get { return m_position; }
        }        
    }
}
