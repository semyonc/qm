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
