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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using DataEngine.CoreServices;
using DataEngine.XQuery.Parser;
using DataEngine.XQuery.Util;

namespace DataEngine.XQuery
{
    public static class XPathFactory
    {
        public static XPathItem Clone(this XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
                return nav.Clone();
            else
                return ((XQueryItem)item).Clone();
        }

        public static XQueryNodeIterator Query(this XPathNavigator node, string xquery, IXmlNamespaceResolver nsmgr)
        {
            XPathContext context = new XPathContext(node.NameTable);
            XQueryCommand command = new XQueryCommand(context);
            if (nsmgr != null)
                context.CopyNamespaces(nsmgr);
            command.ContextItem = node;
            command.CommandText = xquery;
            return command.Execute();
        }        

        public static XQueryNodeIterator Query(this XPathNavigator node, string xquery)
        {
            return Query(node, xquery, null);
        }

        private class NodeList : XmlNodeList
        {
            private List<XmlNode> _list;
            private XQueryNodeIterator _iter;
            private bool _done;

            public NodeList(XQueryNodeIterator iter)
            {
                _list = new List<XmlNode>();
                _iter = iter;
            }

            public override int Count
            {
                get 
                {
                    return _iter.Count;
                }
            }

            private static XmlNode GetNode(XPathItem item)
            {
                XQueryNavigatorWrapper nav = item as XQueryNavigatorWrapper;
                if (nav != null)
                {
                    IHasXmlNode node = nav.m_inner as IHasXmlNode;
                    if (node != null)
                        return node.GetNode();
                }
                return null;
            }

            public override IEnumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public override XmlNode Item(int index)
            {
                if (_list.Count <= index && !_done)
                {
                    int count = _list.Count;
                    while (!_done && (count <= index))
                    {
                        if (_iter.MoveNext())
                        {
                            XmlNode node = GetNode(_iter.Current);
                            if (node != null)
                            {
                                _list.Add(node);
                                count++;
                            }
                        }
                        else
                            _done = true;                            
                    }
                }
                if (index >= 0 && _list.Count > index)
                    return _list[index];
                return null;
            }

            private class Enumerator : IEnumerator, IEnumerator<XmlNode>
            {
                private NodeList _owner;
                private XmlNode _curr;
                private int _pos = -1;

                public Enumerator(NodeList owner)
                {
                    _owner = owner;
                }

                #region IEnumerator Members

                object IEnumerator.Current
                {
                    get 
                    {
                        if (_pos == -1)
                            throw new InvalidOperationException();
                        return _curr;
                    }
                }

                bool IEnumerator.MoveNext()
                {
                    XmlNode node = _owner[_pos + 1];
                    if (node != null)
                    {
                        _curr = node;
                        _pos++;
                        return true;
                    }
                    return false;
                }

                void IEnumerator.Reset()
                {
                    _pos = -1;
                }

                #endregion

                #region IEnumerator<XmlNode> Members

                XmlNode IEnumerator<XmlNode>.Current
                {
                    get 
                    {
                        if (_pos == -1)
                            throw new InvalidOperationException();
                        return _curr;
                    }
                }

                #endregion

                #region IDisposable Members

                void IDisposable.Dispose()
                {
                    return;
                }

                #endregion
            }

        }

        public static XmlNodeList QueryNodes(this XmlNode node, string xquery)
        {
            return QueryNodes(node, xquery, null);
        }

        public static XmlNodeList QueryNodes(this XmlNode node, string xquery, XmlNamespaceManager nsmgr)
        {
            XPathNavigator nav = node.CreateNavigator();
            XPathContext context = new XPathContext(nav.NameTable);
            XQueryCommand command = new XQueryCommand(context);
            if (nsmgr != null)
                context.CopyNamespaces(nsmgr);
            command.ContextItem = nav;
            command.CommandText = xquery;
            return new NodeList(command.Execute());
        }

        public static XmlNode QuerySingleNode(this XmlNode node, string xquery)
        {
            return QuerySingleNode(node, xquery, null);
        }

        public static XmlNode QuerySingleNode(this XmlNode node, string xquery, XmlNamespaceManager nsmgr)
        {
            XmlNodeList nodes = QueryNodes(node, xquery, nsmgr);
            return nodes[0];
        }
      
        internal static Type GetNavigatorValueType(XPathNavigator nav, Type valueType)
        {
            IXmlSchemaInfo schemaInfo = nav.SchemaInfo;
            if (schemaInfo == null || schemaInfo.SchemaType == null)
                return typeof(UntypedAtomic);
            switch (schemaInfo.SchemaType.TypeCode)
            {
                case XmlTypeCode.UntypedAtomic:
                    return typeof(UntypedAtomic);
                case XmlTypeCode.Integer:
                case XmlTypeCode.PositiveInteger:
                case XmlTypeCode.NegativeInteger:
                case XmlTypeCode.NonPositiveInteger:
                    return typeof(Integer);
                case XmlTypeCode.Date:
                    return typeof(DateValue);
                case XmlTypeCode.DateTime:
                    return typeof(DateTimeValue);
                case XmlTypeCode.Time:
                    return typeof(TimeValue);
                case XmlTypeCode.Duration:
                    return typeof(DurationValue);
                case XmlTypeCode.DayTimeDuration:
                    return typeof(DayTimeDurationValue);
                case XmlTypeCode.YearMonthDuration:
                    return typeof(YearMonthDurationValue);
                case XmlTypeCode.GDay:
                    return typeof(GDayValue);
                case XmlTypeCode.GMonth:
                    return typeof(GMonthValue);
                case XmlTypeCode.GMonthDay:
                    return typeof(GMonthDayValue);
                case XmlTypeCode.GYear:
                    return typeof(GYearValue);
                case XmlTypeCode.GYearMonth:
                    return typeof(GYearMonthValue);
                case XmlTypeCode.QName:
                    return typeof(QNameValue);
                case XmlTypeCode.Notation:
                    return typeof(NotationValue);
                case XmlTypeCode.AnyUri:
                    return typeof(AnyUriValue);
                case XmlTypeCode.HexBinary:
                    return typeof(HexBinaryValue);
                case XmlTypeCode.Base64Binary:
                    return typeof(Base64BinaryValue);
                case XmlTypeCode.Idref:
                    if (schemaInfo.SchemaType == XQuerySequenceType.XmlSchema.IDREFS)
                        return typeof(IDREFSValue);
                    goto default;
                case XmlTypeCode.NmToken:
                    if (schemaInfo.SchemaType == XQuerySequenceType.XmlSchema.NMTOKENS)
                        return typeof(NMTOKENSValue);
                    goto default;
                case XmlTypeCode.Entity:
                    if (schemaInfo.SchemaType == XQuerySequenceType.XmlSchema.ENTITIES)
                        return typeof(ENTITIESValue);
                    goto default;                    
                default:
                    return valueType;
            }
        }

        internal static XPathItem ChangeType(this XPathItem item, XQuerySequenceType destType,
            XmlNameTable nameTable, XmlNamespaceManager nsmgr)
        {
            if (destType.IsNode)
            {
                if (!destType.Match(item))
                    throw new XQueryException(Properties.Resources.XPTY0004,
                        new XQuerySequenceType(item.XmlType.TypeCode), destType);
                return item.Clone();
            }
            else
            {
                if (destType.SchemaType == item.XmlType)
                    return item.Clone();
                else if (destType.TypeCode == XmlTypeCode.Item &&
                    (destType.Cardinality == XmlTypeCardinality.One || destType.Cardinality == XmlTypeCardinality.ZeroOrOne))
                    return item.Clone();
                else
                {
                    XmlSchemaSimpleType simpleType = destType.SchemaType as XmlSchemaSimpleType;
                    if (simpleType == null)
                        throw new InvalidOperationException();
                    if (simpleType == XQuerySequenceType.XmlSchema.AnySimpleType)
                        throw new XQueryException(Properties.Resources.XPST0051, "xs:anySimpleType");
                    return new XQueryItem(XQueryConvert.ChangeType(item.XmlType, item.TypedValue,
                        destType, nameTable, nsmgr), destType.SchemaType);
                }
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
            XmlNameTable nameTable, XmlNamespaceManager nsmgr)
        {
            int num = 0;
            XQuerySequenceType itemType = new XQuerySequenceType(destType);
            itemType.Cardinality = XmlTypeCardinality.One;            
            foreach (XPathItem item in iter)
            {
                if (num == 1)
                {
                    if (destType.Cardinality == XmlTypeCardinality.ZeroOrOne ||
                        destType.Cardinality == XmlTypeCardinality.One)
                        throw new XQueryException(Properties.Resources.XPTY0004, "item()+", destType);
                }
                yield return item.ChangeType(itemType, nameTable, nsmgr);
                num++;
            }
            if (num == 0)
            {
                if (destType.Cardinality == XmlTypeCardinality.One ||
                    destType.Cardinality == XmlTypeCardinality.OneOrMore)
                    throw new XQueryException(Properties.Resources.XPTY0004, "item()?", destType);
            }
        }

        internal static IEnumerable<XPathItem> ValueIterator(XQueryNodeIterator iter, XQuerySequenceType destType,
            XmlNameTable nameTable, XmlNamespaceManager nsmgr)
        {
            int num = 0;
            foreach (XPathItem item in iter)
            {
                if (num == 1)
                {
                    if (destType.Cardinality == XmlTypeCardinality.ZeroOrOne ||
                        destType.Cardinality == XmlTypeCardinality.One)
                        throw new XQueryException(Properties.Resources.XPTY0004, "item()+", destType);
                }
                if (destType.IsNode)
                {
                    if (!destType.Match(item))
                        throw new XQueryException(Properties.Resources.XPTY0004,
                            new XQuerySequenceType(item.XmlType, XmlTypeCardinality.OneOrMore, null), destType);
                    yield return item;
                }
                else
                    yield return new XQueryItem(XQueryConvert.ValueAs(item.TypedValue, destType, nameTable, nsmgr));
                num++;
            }
            if (num == 0)
            {
                if (destType.Cardinality == XmlTypeCardinality.One ||
                    destType.Cardinality == XmlTypeCardinality.OneOrMore)
                    throw new XQueryException(Properties.Resources.XPTY0004, "item()?", destType);
            }
        }

        internal static IEnumerable<XPathItem> TreatIterator(XQueryNodeIterator iter, XQuerySequenceType destType)
        {
            int num = 0;
            foreach (XPathItem item in iter)
            {
                if (num == 1)
                {
                    if (destType.Cardinality == XmlTypeCardinality.ZeroOrOne ||
                        destType.Cardinality == XmlTypeCardinality.One)
                        throw new XQueryException(Properties.Resources.XPTY0004, "item()+", destType);
                }
                if (destType.IsNode)
                {
                    if (!destType.Match(item))
                        throw new XQueryException(Properties.Resources.XPTY0004,
                            new XQuerySequenceType(item.XmlType, XmlTypeCardinality.OneOrMore, null), destType);
                    yield return item;
                }
                else
                    yield return new XQueryItem(XQueryConvert.TreatValueAs(item.TypedValue, destType));
                num++;
            }
            if (num == 0)
            {
                if (destType.Cardinality == XmlTypeCardinality.One ||
                    destType.Cardinality == XmlTypeCardinality.OneOrMore)
                    throw new XQueryException(Properties.Resources.XPTY0004, "item()?", destType);
            }
        }

        internal static IEnumerable<XPathItem> ValidateIterator(XQueryNodeIterator iter, XmlSchemaSet schemaSet, bool lax)
        {
            int n = 0;
            foreach (XPathItem item in iter)
            {
                if (!item.IsNode)
                    throw new XQueryException(Properties.Resources.XPTY0004,
                        new XQuerySequenceType(item.TypedValue.GetType(), XmlTypeCardinality.One), "node()*");
                XPathNavigator nav = (XPathNavigator)item.Clone();
                try
                {
                    nav.CheckValidity(schemaSet, null);
                }
                catch (XmlSchemaValidationException ex)
                {
                    throw new XQueryException(ex.Message, ex);
                }
                catch (InvalidOperationException ex)
                {
                    throw new XQueryException(ex.Message, ex);
                }
                yield return nav;
                n++;
            }
            if (n == 0)
              throw new XQueryException(Properties.Resources.XQTY0030);
        }

        internal static IEnumerable<XPathItem> CodepointIterator(string text)
        {
            for (int k = 0; k < text.Length; k++)
                yield return new XQueryItem(Convert.ToInt32(text[k]));
        }


        // Prevent (attribute attr {})/.., (text {})/.., etc expressions
        internal class XQueryDynNodeNavigator : XQueryNavigator
        {            
            internal XQueryDynNodeNavigator(XQueryDocument doc)
                : base(doc)
            {
            }

            public override XPathNavigator Clone()
            {
                XQueryDynNodeNavigator clone = new XQueryDynNodeNavigator(Document);
                clone.MoveTo(this);
                return clone;                
            }

            public override bool MoveToParent()
            {
                return false;
            }

            public override bool MoveToNextAttribute()
            {
                return false;
            }
        }

        internal static IEnumerable<XPathItem> DynAttributeIterator(XPathNavigator nav)
        {
            XQueryNavigator curr = (XQueryNavigator)nav.Clone();
            if (curr.MoveToFirstAttribute())
                do
                {
                    XQueryDynNodeNavigator node = new XQueryDynNodeNavigator(curr.Document);
                    node.MoveTo(curr);
                    yield return node;
                } while (curr.MoveToNextAttribute());            
        }
    }
}
