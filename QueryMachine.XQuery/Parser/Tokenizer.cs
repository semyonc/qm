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
