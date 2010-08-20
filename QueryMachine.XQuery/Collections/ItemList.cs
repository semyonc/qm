//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

using DataEngine.XQuery;
using DataEngine.XQuery.Util;

namespace DataEngine.XQuery.Collections
{
    public class ItemList
    {
        private List<ItemSegment> segments;
        private ItemSegment current;
        private int count;

        public ItemList()
        {
            segments = new List<ItemSegment>();
            current = null;
        }

        public void Add(XPathItem item)
        {
            XQueryNavigator nav = item as XQueryNavigator;
            if (nav != null && nav.NodeType == XPathNodeType.Element)
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

        public int Count
        {
            get
            {
                return count;
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
