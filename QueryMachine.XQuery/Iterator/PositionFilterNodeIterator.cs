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

namespace DataEngine.XQuery.Iterator
{
    sealed class PositionFilterNodeIterator: XQueryNodeIterator
    {
        private XQueryNodeIterator iter;
        private int position;

        public PositionFilterNodeIterator(int pos, XQueryNodeIterator baseIter)
        {
            iter = baseIter;
            position = pos;
        }

        public override XQueryNodeIterator Clone()
        {
            return new PositionFilterNodeIterator(position, iter);
        }

        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return new BufferedNodeIterator(Clone());
        }

        protected override XPathItem NextItem()
        {
            while (iter.MoveNext())
            {
                if (iter.SequentialPosition == position)
                {
                    iter.ResetSequentialPosition();
                    return iter.Current;
                }
            }
            return null;
        }
    }
}
