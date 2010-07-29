﻿//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
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

using System.Xml;
using System.Xml.XPath;

using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public class XQueryResolver: Resolver, IContextProvider
    {
        private Dictionary<object, SymbolLink> m_values = new Dictionary<object, SymbolLink>();

        public XQueryResolver()
        {
        }

        public void SetValue(object atom, SymbolLink link)
        {
            m_values[atom] = link;
        }

        public object GetCurrentStack()
        {
            return new Dictionary<object, SymbolLink>(m_values);
        }

        public void RevertToStack(object state)
        {
            m_values = new Dictionary<object, SymbolLink>((IDictionary<object, SymbolLink>)state);
        }

        public SymbolLink[] List()
        {
            SymbolLink[] res = new SymbolLink[m_values.Values.Count];
            m_values.Values.CopyTo(res, 0);
            return res;
        }

        #region Resolver Members

        public Resolver NewScope()
        {
            return new XQueryResolver();
        }

        public void Init(MemoryPool pool)
        {
            SymbolLink self = new SymbolLink(typeof(IContextProvider));
            pool.Bind(self);
            pool.SetData(self, this);
            SetValue(ID.Context, self);
        }

        public bool Get(object atom, out SymbolLink result)
        {
            SymbolLink link;
            if (m_values.TryGetValue(atom, out link) && link != null)
            {
                result = link;
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region IContextProvider Members

        XPathItem IContextProvider.Context
        {
            get
            {
                return null;
            }
        }

        int IContextProvider.CurrentPosition
        {
            get
            {
                throw new XQueryException(Properties.Resources.XPDY0002);
            }
        }

        int IContextProvider.LastPosition
        {
            get
            {
                throw new XQueryException(Properties.Resources.XPDY0002);
            }
        }

        #endregion
    }
}
