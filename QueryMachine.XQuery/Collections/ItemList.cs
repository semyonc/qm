//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

using DataEngine.XQuery;
using DataEngine.XQuery.Util;
using System.Threading;

namespace DataEngine.XQuery.Collections
{
    public class ItemList
    {
        private List<ItemSegment> segments;
        private ItemSegment current;

        private volatile int count;
        internal volatile bool _finished;

        public ItemList()
        {
            segments = new List<ItemSegment>();
            current = null;
        }

        public void Add(XPathItem item)
        {
            XQueryNavigator nav = item as XQueryNavigator;
            if (nav != null)
            {
                if (current == null || current.document != nav.Document)
                {
                    current = new ElementsSegment(nav.Document);
                    segments.Add(current);
                }
                current.Add(nav.Position);
            }
            else
            {
                if (current == null || current.document != null)
                {
                    current = new DataSegment();
                    segments.Add(current);
                }
                current.Add(item.Clone());
            }
            count++;
        }

        public Iterator CreateIterator()
        {
            return new Iterator(this);
        }

        public XQueryNodeIterator CreateNodeIterator()
        {
            return new NodeIterator(this);
        }

        public ItemList Clone()
        {
            ItemList res = new ItemList();
            foreach (XPathItem item in CreateNodeIterator())
                res.Add(item);
            return res;
        }

        public int Tag { get; set; }

        public int Count
        {
            get
            {
                return count;
            }
        }

        private sealed class NodeIterator : XQueryNodeIterator
        {
            private ItemList src;
            private ItemList.Iterator iter;

            public NodeIterator(ItemList src)
            {
                this.src = src;
            }

            public override int Count
            {
                get
                {
                    return src.Count;
                }
            }

            public override XQueryNodeIterator Clone()
            {
                return new NodeIterator(src);
            }

            public override XQueryNodeIterator CreateBufferedIterator()
            {
                return new NodeIterator(src);
            }

            protected override void Init()
            {
                iter = src.CreateIterator();
            }

            protected override XPathItem NextItem()
            {
                int index = CurrentPosition + 1;
                if (index < src.Count)
                    return iter[index];
                return null;
            }
        }


        private abstract class ItemSegment
        {
            public readonly XQueryDocument document;            

            public ItemSegment(XQueryDocument document)
            {
                this.document = document;                
            }

            public abstract void Add(object item);

            public abstract int Count { get; }

            public abstract object this[int index] { get; }
        }

        private sealed class ElementsSegment : ItemSegment
        {
            private List<int> _indexes = new List<int>();

            public ElementsSegment(XQueryDocument document)
                : base(document)
            {
            }

            public override void Add(object item)
            {
                _indexes.Add((int)item);
            }

            public override int Count
            {
                get 
                {
                    return _indexes.Count;
                }
            }
            
            public override object this[int index]
            {
                get 
                {
                    return _indexes[index];
                }
            }
        }

        private sealed class DataSegment : ItemSegment
        {
            private List<XPathItem> _items = new List<XPathItem>();

            public DataSegment()
                : base(null)
            {
            }

            public override void Add(object item)
            {
                _items.Add((XPathItem)item);
            }

            public override int Count
            {
                get 
                {
                    return _items.Count;
                }
            }
            
            public override object this[int index]
            {
                get 
                {
                    return _items[index];
                }
            }
        }


        public class Iterator
        {
            private ItemList owner;
            private int pos;
            private int index;
            private int offset;
            private ItemSegment current;
            private XQueryNavigator nav;

            internal Iterator(ItemList list)
            {
                owner = list;
                pos = 0;
                current = null;
            }

            public XPathItem this[int i]
            {
                get
                {
                    if (i < pos)
                    {
                        current = null;
                        pos = 0;
                    }
                    if (current == null)
                    {
                        index = 0;
                        offset = 0;
                        if (owner.segments.Count == 0)
                            return null;
                        current = owner.segments[0];
                        nav = null;
                    }
                    while (pos < i)
                    {
                        if (offset == current.Count - 1)
                        {
                            index++;
                            if (index == owner.segments.Count)
                                return null;
                            current = owner.segments[index];
                            offset = 0;
                            nav = null;
                        }
                        else
                            offset++;
                        pos++;
                    }
                    object packedItem = current[offset];
                    XPathItem item = packedItem as XPathItem;
                    if (item != null)
                        return item;
                    if (nav == null)
                        nav = current.document.CreateNavigator();
                    nav.Position = (int)packedItem;
                    return nav;
                }
            }
        }
    }
}
