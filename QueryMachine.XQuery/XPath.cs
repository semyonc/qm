//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
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
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

namespace DataEngine.XQuery
{
    public static class XPath
    {
        public static XPathItem Clone(this XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
                return nav.Clone();
            else
                return item;
        }

        internal static XPathItem ChangeType(this XPathItem item, XQuerySequenceType destType, 
            XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
        {
            if (destType.IsNode)
            {
                if (!destType.Match(item))
                    throw new XQueryException(Properties.Resources.XPTY0004, item.XmlType, destType);
                return item;
            }
            else
            {
                if (destType.TypeCode == XmlTypeCode.Item && 
                    (destType.Cardinality == XmlTypeCardinality.One || destType.Cardinality == XmlTypeCardinality.ZeroOrOne))
                    return item;
                else
                {
                    XmlSchemaSimpleType simpleType = destType.SchemaType as XmlSchemaSimpleType;
                    if (simpleType == null)
                        throw new InvalidOperationException();
                    if (simpleType == XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean))
                        return new XQueryAtomicValue(Core.BooleanValue(item), destType);
                    else
                        if (item.ValueType == null && item.ValueType == typeof(System.String))
                            return new XQueryAtomicValue(simpleType.Datatype.ParseValue(item.Value, nameTable, nsmgr));
                        else
                            return new XQueryAtomicValue(simpleType.Datatype.ChangeType(item.TypedValue, destType.ValueType), destType);
                }
            }
        }        

        internal static IEnumerable<XPathItem> NameTestIterator(XmlQualifiedNameTest nameTest, XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null &&
                    (nav.NodeType == XPathNodeType.Element || nav.NodeType == XPathNodeType.Attribute) &&
                    (nameTest.IsNamespaceWildcard || nameTest.Namespace == nav.NamespaceURI) &&
                    (nameTest.IsNameWildcard || nameTest.Name == nav.LocalName))
                    yield return nav;
            }
        }

        internal static IEnumerable<XPathItem> TypeTestIterator(XQuerySequenceType typeTest, XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                if (typeTest.Match(item))
                    yield return item;                    
            }
        }

        internal static IEnumerable<XPathItem> RootIterator(XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    curr.MoveToRoot();
                    yield return curr;
                }
            }
        }

        internal static IEnumerable<XPathItem> RangeIterator(XQueryContext context, int lo, int high)
        {
            for (int index = lo; index <= high; index++)
                yield return context.CreateItem(index);
        }

        internal static IEnumerable<XPathItem> ChildIterator(XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    if (curr.MoveToFirstChild())
                        do
                        {
                            yield return curr;
                        } while (curr.MoveToNext());
                }
            }
        }

        internal static IEnumerable<XPathItem> AttributeIterator(XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    if (curr.MoveToFirstAttribute())
                        do
                        {
                            yield return curr;
                        } while (curr.MoveToNextAttribute());
                }
            }
        }

        internal static IEnumerable<XPathItem> DescendantIterator(XQueryNodeIterator iter, bool self)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    int depth = 0;
                    if (self)
                        yield return curr;
                    do
                    {
                        if (curr.MoveToFirstChild())
                        {
                            depth++;
                            yield return curr;
                            continue;
                        }
                        while (depth != 0)
                        {
                            if (curr.MoveToNext())
                            {
                                yield return curr;
                                break;
                            }
                            if (!curr.MoveToParent())
                                throw new InvalidOperationException();
                            depth--;
                        }
                    } while (depth > 0);                    
                }
            }
        }

        internal static IEnumerable<XPathItem> ParentIterator(XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    if (curr.MoveToParent())
                        yield return curr;
                }
            }
        }

        internal static IEnumerable<XPathItem> AncestorIterator(XQueryNodeIterator iter, bool self)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    if (self)
                        yield return curr;
                    while (curr.MoveToParent())
                        yield return curr;
                }
            }
        }

        internal static IEnumerable<XPathItem> FollowingSiblingIterator(XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    while (curr.MoveToNext())
                        yield return curr;
                }
            }
        }

        internal static IEnumerable<XPathItem> PrecedingSiblingIterator(XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    while (curr.MoveToPrevious())
                        yield return curr;
                }
            }
        }

        internal static IEnumerable<XPathItem> FollowingIterator(XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    if (curr.MoveToNext())
                        yield return curr;
                    else
                        while (curr.MoveToParent())
                            if (curr.MoveToNext())
                            {
                                yield return curr;
                                break;
                            }
                    bool flag = true;
                    do
                    {
                        if (curr.MoveToFirstChild())
                            yield return curr;
                        else
                            do
                            {
                                if (curr.MoveToNext())
                                {
                                    yield return curr;
                                    break;
                                }
                            } while (flag = curr.MoveToParent());
                    } while (flag);
                }
            }
        }

        internal static IEnumerable<XPathItem> PrecedingIterator(XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    curr.MoveToRoot();
                    while (curr.ComparePosition(nav) == XmlNodeOrder.Before)
                    {
                        if (curr.MoveToFirstChild())
                        {     
                            if (!curr.IsDescendant(nav))
                                yield return curr;
                            continue;
                        }
                        while (true)
                        {
                            if (curr.MoveToNext())
                            {
                                if (!curr.IsDescendant(nav) &&
                                    curr.ComparePosition(nav) == XmlNodeOrder.Before)
                                    yield return curr;
                                break;
                            }
                            if (!curr.MoveToParent())
                                throw new InvalidOperationException();
                        }
                    } 
                }
            }
        }

        internal static IEnumerable<XPathItem> SelfIterator(XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                    yield return nav;
            }
        }

        internal static IEnumerable<XPathItem> NamespaceIterator(XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null)
                {
                    XPathNavigator curr = nav.Clone();
                    if (curr.MoveToFirstNamespace())
                    {
                        yield return curr;
                        while (curr.MoveToNextNamespace())
                            yield return curr;
                    }
                }
            }
        }

        internal static IEnumerable<XPathItem> UnionIterator1(XQueryNodeIterator iter1, XQueryNodeIterator iter2)
        {
            SortedDictionary<XPathItem, XPathItem> set = 
                new SortedDictionary<XPathItem, XPathItem>(new XPathComparer());
            foreach (XPathItem item in iter1)
                if (!set.ContainsKey(item))
                    set.Add(item.Clone(), null);
            foreach (XPathItem item in iter2)
                if (!set.ContainsKey(item))
                    set.Add(item.Clone(), null);
            foreach (KeyValuePair<XPathItem, XPathItem> kvp in set)
                yield return kvp.Key;
        }

        internal static IEnumerable<XPathItem> UnionIterator2(XQueryNodeIterator iter1, XQueryNodeIterator iter2)
        {
            HashSet<XPathItem> hs = new HashSet<XPathItem>(new XPathNavigatorEqualityComparer());
            foreach (XPathItem item in iter1)
                if (!hs.Contains(item))
                {
                    hs.Add(item.Clone());
                    yield return item;
                }
            foreach (XPathItem item in iter2)
                if (!hs.Contains(item))
                {
                    hs.Add(item.Clone());
                    yield return item;
                }
        }

        internal static IEnumerable<XPathItem> IntersectExceptIterator1(bool except, XQueryNodeIterator iter1, XQueryNodeIterator iter2)
        {
            SortedDictionary<XPathItem, XPathItem> set =
                new SortedDictionary<XPathItem, XPathItem>(new XPathComparer());
            HashSet<XPathItem> hs = new HashSet<XPathItem>(new XPathNavigatorEqualityComparer());
            foreach (XPathItem item in iter1)
                if (!set.ContainsKey(item))
                    set.Add(item.Clone(), null);
            foreach (XPathItem item in iter2)
                if (!hs.Contains(item))
                    hs.Add(item.Clone());
            foreach (KeyValuePair<XPathItem, XPathItem> kvp in set)
                if (except)
                {
                    if (!hs.Contains(kvp.Key))
                        yield return kvp.Key;
                }
                else
                {
                    if (hs.Contains(kvp.Key))
                        yield return kvp.Key;
                }
        }

        internal static IEnumerable<XPathItem> IntersectExceptIterator2(bool except, XQueryNodeIterator iter1, XQueryNodeIterator iter2)
        {
            HashSet<XPathItem> hs = new HashSet<XPathItem>(new XPathNavigatorEqualityComparer());
            foreach (XPathItem item in iter1)
                if (!hs.Contains(item))
                    hs.Add(item.Clone());
            if (except)
                hs.ExceptWith(iter2);
            else
                hs.IntersectWith(iter2);
            foreach (XPathItem item in hs)
                yield return item;
        }

        internal static IEnumerable<XPathItem> ConvertIterator(XQueryNodeIterator iter, XQuerySequenceType destType, 
            XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
        {
            int num = 0;
            XQuerySequenceType itemType = new XQuerySequenceType(destType);
            itemType.Cardinality = XmlTypeCardinality.One;
            foreach (XPathItem item in iter)
            {
                if (num == 2)
                {
                    if (destType.Cardinality == XmlTypeCardinality.ZeroOrOne ||
                        destType.Cardinality == XmlTypeCardinality.One)
                        throw new XQueryException(Properties.Resources.XPTY0004, "item()+", destType);
                }
                yield return item.ChangeType(itemType, nameTable, nsmgr);
                num++;
                if (num == 0)
                {
                    if (destType.Cardinality == XmlTypeCardinality.One ||
                        destType.Cardinality == XmlTypeCardinality.OneOrMore)
                        throw new XQueryException(Properties.Resources.XPTY0004, "item()?", destType);
                }
            }
        }

        internal static IEnumerable<XPathItem> RemoveIterator(XQueryNodeIterator iter, int index)
        {
            int pos = 1;
            foreach (XPathItem item in iter)
            {
                if (index != pos)
                    yield return item;
                pos++;
            }
        }

        internal static IEnumerable<XPathItem> InsertIterator(XQueryNodeIterator iter, int index, XQueryNodeIterator iter2)
        {
            int pos = 1;
            foreach (XPathItem item in iter)
            {                
                if (index == pos)
                    foreach (XPathItem item2 in iter2)
                        yield return item2;
                yield return item;
                pos++;
            }
        }

        internal static IEnumerable<XPathItem> SubsequenceIterator(XQueryNodeIterator iter, int startingLoc)
        {
            int pos = 1;
            foreach (XPathItem item in iter)
            {
                if (startingLoc <= pos)
                    yield return item;
                pos++;
            }
        }

        internal static IEnumerable<XPathItem> SubsequenceIterator(XQueryNodeIterator iter, int startingLoc, int length)
        {
            int pos = 1;
            foreach (XPathItem item in iter)
            {
                if (startingLoc <= pos)
                {
                    if (length-- <= 0)
                        break;
                    yield return item;
                }
                pos++;
            }
        }
    }
}
