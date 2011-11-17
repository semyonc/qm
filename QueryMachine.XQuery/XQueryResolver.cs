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
