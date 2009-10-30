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

namespace DataEngine.XQuery.Util
{
    class StringTokenizer
    {
        private string _text;
        private int _offset;

        public StringTokenizer(string text)
        {
            _text = text;
        }

        public int Token { get; private set; }

        public string Value { get; private set; }

        public int LineCount { get; private set; }

        public int Offset
        {
            get
            {
                return _offset;
            }
        }

        public static readonly int TokenInt = 1;        

        public override string ToString()
        {
            return _text;
        }

        public int NextToken()
        {
            int anchor = _offset;
            char ch = '\0';
            Value = String.Empty;
            while (_offset < _text.Length)
            {
                ch = _text[_offset];
                if (Char.IsWhiteSpace(ch) && ch != '\n' && ch != '\r')
                    _offset++;
                else
                    break;
            }
            switch (ch)
            {
                case '\0':
                    Token = ch;
                    break;
                
                case '\n':
                    _offset++;
                    if (_offset < _text.Length - 1 && _text[_offset] == '\r')
                        _offset++;
                    LineCount++;
                    Token = ch;
                    break;

                case '\r':
                    _offset++;
                    if (_offset < _text.Length - 1 && _text[_offset] == '\n')
                        _offset++;
                    LineCount++;
                    Token = '\n';
                    break;

                default:
                    if (Char.IsDigit(ch))
                    {
                        StringBuilder sb = new StringBuilder();
                        while (_offset < _text.Length)
                        {
                            ch = _text[_offset];
                            if (!Char.IsDigit(ch))
                                break;
                            sb.Append(ch);
                            _offset++;
                        }
                        Token = TokenInt;
                        Value = sb.ToString();
                    }
                    else
                    {
                        Token = ch;
                        _offset++;
                    }
                    break;

            }
            return Token;
        }

        public void SkipTo(char ch)
        {
            SkipTo(new char[] { ch });
        }

        public void SkipTo(char[] charset)
        {
            SkipTo(new String(charset));
        }

        public void SkipTo(string charset)
        {
            int anchor = _offset;
            while (Token != 0 && charset.IndexOf((char)Token) == -1)
                NextToken();
            Value = _text.Substring(anchor, _offset - anchor);
        }

        public string SkipToEOL()
        {
            if (_offset == _text.Length)
                return String.Empty;
            int anchor = _offset;
            while (_offset < _text.Length)
            {
                char ch = _text[_offset];
                if (ch == '\n')
                {
                    _offset++;
                    if (_offset < _text.Length && _text[_offset] == '\r')
                        _offset++;
                    LineCount++;
                    Token = '\n';
                    return _text.Substring(anchor, _offset - anchor);
                }
                else if (ch == '\r')
                {
                    _offset++;
                    if (_offset < _text.Length && _text[_offset] == '\n')
                        _offset++;
                    LineCount++;
                    Token = '\n';
                    return _text.Substring(anchor, _offset - anchor);
                }
                else
                    _offset++;
            }
            return _text.Substring(anchor);
        }
    }
}
