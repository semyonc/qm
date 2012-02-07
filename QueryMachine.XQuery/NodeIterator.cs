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
using System.Diagnostics;

namespace DataEngine.XQuery
{
    public sealed class NodeIterator: XQueryNodeIterator
    {
        private IEnumerable<XPathItem> master;
        private IEnumerator<XPathItem> iterator;
        private bool isOrderedSet;
        
        public NodeIterator(IEnumerable<XPathItem> enumerable, bool orderedSet)
        {
            master = enumerable;
            isOrderedSet = orderedSet;
        }

        public NodeIterator(IEnumerable<XPathItem> enumerable)
            : this(enumerable, false)
        {
        }

        [DebuggerStepThrough]
        public override XQueryNodeIterator Clone()
        {
            return new NodeIterator(master, isOrderedSet);
        }

        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return new BufferedNodeIterator(this);
        }

        protected override void Init()
        {
            iterator = master.GetEnumerator();
        }

        protected override XPathItem NextItem()
        {
            if (iterator.MoveNext())
                return iterator.Current;
            return null;
        }
    }
}
