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

using DataEngine.XQuery.Parser;

namespace DataEngine.XQuery
{
    public class XQueryCommand: IDisposable
    {
        protected XQueryContext m_context;
        protected bool m_compiled;
        protected XQueryExprBase m_res;
        protected bool m_disposed = false;

        public XQueryCommand()
        {
            m_context = new XQueryContext();
            m_compiled = false;
            
            BaseUri = null;
            SearchPath = null;
        }

        ~XQueryCommand()
        {
            Dispose(false);
        }

        protected void CheckDisposed()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                m_context.Close();
                m_disposed = true;
            }
        }

        public void Compile(string queryText)
        {
            CheckDisposed();
            TokenizerBase tok = new Tokenizer(queryText);
            Notation notation = new Notation();
            YYParser parser = new YYParser(notation);
            parser.yyparseSafe(tok);
            if (BaseUri != null)
                m_context.BaseUri = BaseUri;
            if (SearchPath != null)
                m_context.SearchPath = SearchPath;
            Translator translator = new Translator(m_context);            
            m_res = translator.Process(notation);
            m_compiled = true;
        }

        public XQueryNodeIterator Execute()
        {
            CheckDisposed();
            if (!m_compiled)
                throw new XQueryException("XQuery expression not compiled");
            if (m_res == null)
                throw new XQueryException("Can't run XQuery function module");
            return m_res.Execute(null);
        }

        public String BaseUri { get; set; }
        public String SearchPath { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
