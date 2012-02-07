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
    sealed class PrecedingSiblingNodeIterator : SequentialAxisNodeIterator
    {
        public PrecedingSiblingNodeIterator(XQueryContext context, object nodeTest, XQueryNodeIterator iter)
            : base(context, nodeTest, false, iter)
        {
        }

        private PrecedingSiblingNodeIterator(AxisNodeIterator src)
        {
            AssignFrom(src);
        }

        public override XQueryNodeIterator Clone()
        {
            return new PrecedingSiblingNodeIterator(this);
        }

        protected override bool MoveToFirst(XPathNavigator nav)
        {
            return nav.MoveToPrevious();
        }

        protected override bool MoveToNext(XPathNavigator nav)
        {
            return nav.MoveToPrevious();
        }
    }
}
