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

namespace DataEngine.XQuery.Collections
{
    public class ItemSet : IEnumerable, IEnumerable<XPathItem>
    {
        private List<XPathItem> items;
        private Dictionary<XQueryDocument, List<int>> segments;
        private XQueryDocument current;
        private List<int> segment;
        private int count;

        public ItemSet()
        {
            segments = new Dictionary<XQueryDocument, List<int>>();
            items = new List<XPathItem>();
            current = null;
        }

        public void Add(XPathItem item)
        {
            XQueryNavigator nav = item as XQueryNavigator;
            if (nav != null && nav.NodeType == XPathNodeType.Element)
            {
                if (current != nav.Document)
                {
                    current = nav.Document;
                    if (!segments.TryGetValue(current, out segment))
                    {
                        segment = new List<int>();
                        segments.Add(nav.Document, segment);
                    }
                }
                segment.Add(nav.Position);
            }
            else
                items.Add(item.Clone());
            count++;
        }

        public bool Completed { get; set; }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public void Sort()
        {
            IComparer<int> comparer = new IntegerComparer();
            foreach (List<int> s in segments.Values)
                s.Sort(comparer);
            items.Sort(new XPathComparer());
        }

        private class IntegerComparer : IComparer<int>
        {
            #region IComparer<int> Members

            public int Compare(int x, int y)
            {
                if (x < y)
                    return -1;
                else if (x > y)
                    return 1;
                return 0;
            }

            #endregion
        }


        #region IEnumerable<XPathItem> Members

        IEnumerator<XPathItem> IEnumerable<XPathItem>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        private class Enumerator : IEnumerator<XPathItem>, IDisposable, IEnumerator
        {
            private ItemSet nodeSet;
            private int count;
            private int index;
                       
            private IEnumerator<KeyValuePair<XQueryDocument, List<int>>> enum1;
            private IEnumerator<int> enum2;
            private IEnumerator<XPathItem> enum3;
            
            private XQueryDocument document;
            private XPathItem current;

            public Enumerator(ItemSet nodeSet)
            {
                this.nodeSet = nodeSet;
                index = 0;
                count = nodeSet.count;
                enum1 = nodeSet.segments.GetEnumerator();
            }

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get 
                {
                    if (index == 0 || index == count + 1)
                        throw new InvalidOperationException();
                    return current;
                }
            }

            bool IEnumerator.MoveNext()
            {
                if (count == nodeSet.count && index < count)
                {
                    while (true)
                    {
                        if (enum1 != null)
                        {
                            if (enum2 == null)
                            {
                                if (!enum1.MoveNext())
                                    enum1 = null;
                                else
                                {
                                    KeyValuePair<XQueryDocument, List<int>> kvp = enum1.Current;
                                    document = kvp.Key;
                                    enum2 = kvp.Value.GetEnumerator();
                                }
                            }
                            else
                            {
                                if (enum2.MoveNext())
                                {
                                    XQueryNavigator nav = document.CreateNavigator();
                                    nav.Position = enum2.Current;
                                    current = nav;
                                    break;
                                }
                                else
                                    enum2 = null;
                            }
                        }
                        else
                        {
                            if (enum3 == null)
                                enum3 = nodeSet.items.GetEnumerator();
                            enum3.MoveNext();
                            current = enum3.Current;
                            break;
                        }
                    }
                    index++;
                    return true;
                }
                if (count != nodeSet.Count)
                    throw new InvalidOperationException();
                index = count + 1;
                current = null;
                return false;
            }

            void IEnumerator.Reset()
            {
                index = 0;
                enum1 = nodeSet.segments.GetEnumerator();
                enum2 = null;
                enum3 = null;
            }

            #endregion

            #region IEnumerator<XPathItem> Members

            XPathItem IEnumerator<XPathItem>.Current
            {
                get 
                {
                    return current;
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                
            }

            #endregion
        }

    }
}
