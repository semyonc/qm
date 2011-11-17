//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace DataEngine.XQuery
{
    public class EmptyIterator : XQueryNodeIterator
    {
        public static EmptyIterator Shared = new EmptyIterator();

        private EmptyIterator()
        {
        }

        public override XQueryNodeIterator Clone()
        {
            return this;
        }

        public override bool IsFinished
        {
            get
            {
                return true;
            }
        }

        protected override XPathItem NextItem()
        {
            return null;
        }

        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return this;
        }

        public override object ThreadClone()
        {
            return this;
        }
    }
}
