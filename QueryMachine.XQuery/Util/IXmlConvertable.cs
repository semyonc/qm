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
using System.Globalization;
using System.Xml;

namespace DataEngine.XQuery.Util
{
    public interface IXmlConvertable
    {
        object ValueAs(XQuerySequenceType type, XmlNamespaceManager nsmgr);
    }
}
