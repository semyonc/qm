//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public class XQueryParameter
    {
        public XQueryParameter()
        {
        }

        public XQueryParameter(String localName, Object value)
            : this(localName, String.Empty, value)
        {
        }

        public XQueryParameter(String localName, String namespaceUri, Object value)
        {
            LocalName = localName;
            NamespaceUri = namespaceUri;
            Value = value;
        }

        public String LocalName { get; set; }

        public String NamespaceUri { get; set; }

        public object Value { get; set; }

        internal object ID { get; set; }
    }
}
