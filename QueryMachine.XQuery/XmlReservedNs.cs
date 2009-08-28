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
    public abstract class XmlReservedNs
    {
        public const string NsWdXsl = "http://www.w3.org/TR/WD-xsl";
        public const string NsXml = "http://www.w3.org/XML/1998/namespace";
        public const string NsXmlNs = "http://www.w3.org/2000/xmlns/";
        public const string NsXs = "http://www.w3.org/2001/XMLSchema";
        public const string NsXsd = "http://www.w3.org/2001/XMLSchema-datatypes";
        public const string NsXsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string NsXQueryFunc = "http://www.w3.org/2003/11/xpath-functions";
        public const string NsXQueryDataType = "http://www.w3.org/2003/11/xpath-datatypes";
        public const string NsXQueryLocalFunc = "http://www.w3.org/2005/xquery-local-functions";
    };
}
