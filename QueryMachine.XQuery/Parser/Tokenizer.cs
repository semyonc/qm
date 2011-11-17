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

namespace DataEngine.XQuery.Parser
{
    public class Tokenizer: TokenizerBase
    {
        private TextReader m_reader;
        private int m_position;
        private StringBuilder m_buffer = new StringBuilder();

        public Tokenizer(string strInput)
        {
            m_reader = new StringReader(strInput);
        }

        public Tokenizer(TextReader reader)
        {
            m_reader = reader;
        }

        public override int Position
        {
            get { return m_position; }
        }        

        protected override char Peek(int lookahead)
        {
            while (lookahead >= m_buffer.Length && m_reader.Peek() != -1)
                m_buffer.Append((char)m_reader.Read());
            if (lookahead < m_buffer.Length)
                return m_buffer[lookahead];
            else
                return '\0';
        }

        protected override char Read()
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
    }
}
