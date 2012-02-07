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
using DataEngine.XQuery.Collections;

namespace DataEngine.XQuery.Iterator
{
    sealed class DocumentOrderNodeIterator : XQueryNodeIterator
    {
        private ItemSet itemSet;
        private XPathNavigator lastNode;
        private IEnumerator<XPathItem> items;

        private DocumentOrderNodeIterator(ItemSet itemSet)
        {
            this.itemSet = itemSet;
        }

        public DocumentOrderNodeIterator(XQueryNodeIterator baseIter)
        {
            bool? isNode = null;
            itemSet = new ItemSet();
            while (baseIter.MoveNext())
            {
                if (!isNode.HasValue)
                    isNode = baseIter.Current.IsNode;
                else
                    if (baseIter.Current.IsNode != isNode)
                        throw new XQueryException(Properties.Resources.XPTY0018, baseIter.Current.Value);
                itemSet.Add(baseIter.Current.Clone());
            }
            if (isNode.HasValue && isNode.Value)
                itemSet.Sort();
        }

        public override XQueryNodeIterator Clone()
        {
            return new DocumentOrderNodeIterator(itemSet);
        }

        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return Clone();
        }

        public override bool IsOrderedSet
        {
            get
            {
                return true;
            }
        }

        protected override void Init()
        {
            items = ((IEnumerable<XPathItem>)itemSet).GetEnumerator();
        }

        protected override XPathItem NextItem()
        {
            while (items.MoveNext())
            {
                XPathItem item = items.Current;
                XPathNavigator node = item as XPathNavigator;
                if (node != null)
                {
                    if (lastNode != null)
                    {
                        if (lastNode.IsSamePosition(node))
                            continue;
                    }
                    lastNode = node.Clone();
                }
                return item;
            }
            return null;
        }
    }
}
