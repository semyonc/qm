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
using System.Xml.Schema;
using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public class XPathNavigableWrapper : IXPathNavigable
    {
        IXPathNavigable m_doc;

        public XPathNavigableWrapper(IXPathNavigable doc)
        {
            m_doc = doc;
        }

        #region IXPathNavigable Members

        public XPathNavigator CreateNavigator()
        {
            return new XQueryNavigatorWrapper(m_doc.CreateNavigator());
        }

        #endregion
    }
}
