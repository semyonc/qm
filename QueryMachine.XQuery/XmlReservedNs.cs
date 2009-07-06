//     
//      Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
//     
//      The use and distribution terms for this software are contained in the file
//      named license.txt, which can be found in the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by the
//      terms of this license.
//     
//      You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataEngine.XQuery
{
    /// <summary>
    /// This class defines a set of common XML namespaces for sharing across multiple source files.
    /// </summary>
    internal abstract class XmlReservedNs
    {
        internal const string NsWdXsl = "http://www.w3.org/TR/WD-xsl";
        internal const string NsXml = "http://www.w3.org/XML/1998/namespace";
        internal const string NsXmlNs = "http://www.w3.org/2000/xmlns/";
        internal const string NsXs = "http://www.w3.org/2001/XMLSchema";
        internal const string NsXsd = "http://www.w3.org/2001/XMLSchema-datatypes";
        internal const string NsXsi = "http://www.w3.org/2001/XMLSchema-instance";
        internal const string NsXQueryFunc = "http://www.w3.org/2003/11/xpath-functions";
        internal const string NsXQueryDataType = "http://www.w3.org/2003/11/xpath-datatypes";
        internal const string NsXQueryLocalFunc = "http://www.w3.org/2005/xquery-local-functions";
    };
}
