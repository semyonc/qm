//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Xml.Schema;

namespace DataEngine.XQuery.DocumentModel
{
    internal class DmSchemaInfo : XmlSchemaInfo
    {
        public DmSchemaInfo(IXmlSchemaInfo xmlSchemaInfo)
        {
            IsDefault = xmlSchemaInfo.IsDefault;
            IsNil = xmlSchemaInfo.IsNil;
            MemberType = xmlSchemaInfo.MemberType;
            SchemaAttribute = xmlSchemaInfo.SchemaAttribute;
            SchemaElement = xmlSchemaInfo.SchemaElement;
            SchemaType = xmlSchemaInfo.SchemaType;
            Validity = xmlSchemaInfo.Validity;
        }
    }

}
