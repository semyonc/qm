//        Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Threading;

namespace DataEngine.XQuery.Iterator
{
    abstract class SequentialAxisNodeIterator : AxisNodeIterator
    {
        protected SequentialAxisNodeIterator()
        {
        }

        public SequentialAxisNodeIterator(XQueryContext context, object nodeTest, bool matchSelf, XQueryNodeIterator iter)
            : base(context, nodeTest, matchSelf, iter)
        {
        }

        private bool first = false;

        protected abstract bool MoveToFirst(XPathNavigator nav);

        protected abstract bool MoveToNext(XPathNavigator nav);

        protected override XPathItem NextItem()
        {
            while (true)
            {
                if (!accept)
                {
                    if (!MoveNextIter())
                        return null;
                    first = true;
                    if (matchSelf && TestItem())
                    {
                        sequentialPosition++;
                        return curr;
                    }
                }
                if (first)
                {
                    accept = MoveToFirst(curr);
                    first = false;
                }
                else
                    accept = MoveToNext(curr);
                if (accept)
                {
                    if (TestItem())
                    {
                        sequentialPosition++;
                        return curr;
                    }
                }
            }
        }
    }
}
