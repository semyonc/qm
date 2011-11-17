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
using System.Collections;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;


namespace DataEngine.XQuery.DocumentModel
{
    internal class DmPI : DmNode
    {
        private string _name;

        public static readonly string InternalNs = "[PI]";

        public DmPI(string name)
        {
            _name = name;
        }

        public override XPathNodeType NodeType
        {
            get
            {
                return XPathNodeType.ProcessingInstruction;
            }
        }

        public override string LocalName
        {
            get
            {
                return _name;
            }
        }
    }
}