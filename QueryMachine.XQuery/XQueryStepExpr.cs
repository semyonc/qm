//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public enum XQueryPathExprType
    {
        Child,
        Descendant,
        Attribute,
        Self,
        DescendantOrSelf,
        FollowingSibling,
        Following,
        Parent,
        Ancestor,
        PrecedingSibling,
        Preceding,
        AncestorOrSelf,
        Namespace
    };

    public sealed class XQueryStepExpr : XQueryExprBase
    {
        private delegate IEnumerable<XPathItem> PathExprIterator(XPathItem item);        

        private XQueryPathExprType m_type;
        private PathExprIterator m_iter;
        private XmlQualifiedNameTest m_nameTest;
        private XQuerySequenceType m_typeTest;        

        public XQueryStepExpr(object nodeTest, XQueryPathExprType type, XQueryContext queryContext)
            : this(type, queryContext)
        {
            if (nodeTest is XmlQualifiedNameTest)
                m_nameTest = (XmlQualifiedNameTest)nodeTest;
            else if (nodeTest is XQuerySequenceType)
                m_typeTest = (XQuerySequenceType)nodeTest;
            else
                throw new ArgumentException("nodeTest");
        }

        public XQueryStepExpr(XQueryPathExprType type, XQueryContext queryContext)
            : base(queryContext)
        {
            m_type = type;
            switch (m_type)
            {
                case XQueryPathExprType.Self:
                    m_iter = new PathExprIterator(SelfIterator);
                    break;

                case XQueryPathExprType.Child:
                    m_iter = new PathExprIterator(ChildIterator);
                    break;

                case XQueryPathExprType.Descendant:
                case XQueryPathExprType.DescendantOrSelf:
                    m_iter = new PathExprIterator(DescendantIterator);
                    break;

                case XQueryPathExprType.Attribute:
                    m_iter = new PathExprIterator(AttributeIterator);
                    break;

                case XQueryPathExprType.Following:
                    m_iter = new PathExprIterator(FollowingIterator);
                    break;

                case XQueryPathExprType.FollowingSibling:
                    m_iter = new PathExprIterator(FollowingSiblingIterator);
                    break;

                case XQueryPathExprType.Parent:
                    m_iter = new PathExprIterator(ParentIterator);
                    break;

                case XQueryPathExprType.Ancestor:
                case XQueryPathExprType.AncestorOrSelf:
                    m_iter = new PathExprIterator(AncestorIterator);
                    break;

                case XQueryPathExprType.Preceding:
                    m_iter = new PathExprIterator(PrecedingIterator);
                    break;

                case XQueryPathExprType.PrecedingSibling:
                    m_iter = new PathExprIterator(PrecedingSiblingIterator);
                    break;

                case XQueryPathExprType.Namespace:
                    m_iter = new PathExprIterator(NamespaceIterator);
                    break;

                default:
                    throw new ArgumentException("type");
            }
        }

        public XQueryPathExprType ExprType
        {
            get
            {
                return m_type;
            }
        }

        public XQuerySequenceType TypeTest
        {
            get
            {
                return m_typeTest;
            }
        }

        public XmlQualifiedNameTest NameTest
        {
            get
            {
                return m_nameTest;
            }
        }

        public bool IsText
        {
            get
            {
                return m_typeTest != null && m_typeTest.TypeCode == XmlTypeCode.Text;
            }
        }

        public bool IsAttribute
        {
            get
            {
                return m_type == XQueryPathExprType.Attribute;
            }
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
        }

        public override bool IsContextSensitive(Executive.Parameter[] parameters)
        {
            return true;
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            return new FunctionLink[0];
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            if (provider.Context == null)
                throw new XQueryException(Properties.Resources.XPDY0002);
            if (!provider.Context.IsNode)
                throw new XQueryException(Properties.Resources.XPTY0019, provider.Context.Value);
            return new NodeIterator(m_iter(provider.Context));                
        }

        public bool TestItem(XPathItem item)
        {
            if (m_nameTest != null)
            {
                XPathNavigator nav = item as XPathNavigator;
                return (nav != null && 
                    (nav.NodeType == XPathNodeType.Element || nav.NodeType == XPathNodeType.Attribute) &&
                    (m_nameTest.IsNamespaceWildcard || QueryContext.StringEquals(m_nameTest.Namespace, nav.NamespaceURI)) &&
                    (m_nameTest.IsNameWildcard || QueryContext.StringEquals(m_nameTest.Name, nav.LocalName)));
            }
            else if (m_typeTest != null)
                return m_typeTest.Match(item, QueryContext);
            return true;    
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            if (m_typeTest != null || m_nameTest != null)
            {
                if (m_nameTest != null)
                    sb.Append(m_nameTest.ToString());
                else if (m_typeTest != null)
                    sb.Append(m_typeTest.ToString());
                sb.Append(", ");
            }
            sb.Append(m_type.ToString());
            sb.Append("]");
            return sb.ToString();
        }
#endif

        private IEnumerable<XPathItem> SelfIterator(XPathItem item)
        {
            if (TestItem(item))
                yield return item; 
        }

        private IEnumerable<XPathItem> ChildIterator(XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
            {
                XPathNavigator curr = nav.Clone();
                if (curr.MoveToFirstChild())
                    do
                    {
                        if (TestItem(curr))
                            yield return curr;
                        if (nav.NodeType == XPathNodeType.Root &&
                            curr.NodeType == XPathNodeType.Element &&
                            m_nameTest != null)
                            break;
                    }
                    while (curr.MoveToNext());
            }
        }

        private IEnumerable<XPathItem> AttributeIterator(XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
            {
                XPathNavigator curr = nav.Clone();
                if (curr.MoveToFirstAttribute())
                    do
                    {
                        if (TestItem(curr))
                            yield return curr;
                    } 
                    while (curr.MoveToNextAttribute());
            }
        }

        private IEnumerable<XPathItem> DescendantIterator(XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
            {
                XPathNavigator curr = nav.Clone();
                int depth = 0;
                if (m_type == XQueryPathExprType.DescendantOrSelf)
                {
                    if (TestItem(curr))
                        yield return curr;
                }
                do
                {
                    if (curr.MoveToFirstChild())
                    {
                        depth++;
                        if (TestItem(curr))
                            yield return curr;
                        continue;
                    }
                    while (depth != 0)
                    {
                        if (curr.MoveToNext())
                        {
                            if (TestItem(curr))
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

        private IEnumerable<XPathItem> ParentIterator(XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
            {
                XPathNavigator curr = nav.Clone();
                if (curr.MoveToParent())
                {
                    if (TestItem(curr))
                        yield return curr;
                }
            }
        }

        private IEnumerable<XPathItem> AncestorIterator(XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
            {
                XPathNavigator curr = nav.Clone();
                if (m_type == XQueryPathExprType.AncestorOrSelf)
                {
                    if (TestItem(curr))
                        yield return curr;
                }
                while (curr.MoveToParent())
                {
                    if (TestItem(curr))
                        yield return curr;
                }
            }
        }

        private IEnumerable<XPathItem> FollowingSiblingIterator(XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
            {
                XPathNavigator curr = nav.Clone();
                while (curr.MoveToNext())
                {
                    if (TestItem(curr))
                        yield return curr;
                }
            }
        }

        private IEnumerable<XPathItem> PrecedingSiblingIterator(XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
            {
                XPathNavigator curr = nav.Clone();
                while (curr.MoveToPrevious())
                {
                    if (TestItem(curr))
                        yield return curr;
                }
            }
        }

        private IEnumerable<XPathItem> FollowingIterator(XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
            {
                XPathNavigator curr = nav.Clone();
                if (curr.MoveToNext())
                {
                    if (TestItem(curr))
                        yield return curr;
                }
                else
                    while (curr.MoveToParent())
                        if (curr.MoveToNext())
                        {
                            if (TestItem(curr))
                                yield return curr;
                            break;
                        }
                bool flag = true;
                do
                {
                    if (curr.MoveToFirstChild())
                    {
                        if (TestItem(curr))
                            yield return curr;
                    }
                    else
                        do
                        {
                            if (curr.MoveToNext())
                            {
                                if (TestItem(curr))
                                    yield return curr;
                                break;
                            }
                        } while (flag = curr.MoveToParent());
                } while (flag);
            }
        }

        private IEnumerable<XPathItem> PrecedingIterator(XPathItem item)
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
                        {
                            if (TestItem(curr))
                                yield return curr;
                        }
                        continue;
                    }
                    while (true)
                    {
                        if (curr.MoveToNext())
                        {
                            if (!curr.IsDescendant(nav) &&
                                curr.ComparePosition(nav) == XmlNodeOrder.Before)
                            {
                                if (TestItem(curr))
                                    yield return curr;
                            }
                            break;
                        }
                        if (!curr.MoveToParent())
                            throw new InvalidOperationException();
                    }
                }
            }
        }

        private IEnumerable<XPathItem> NamespaceIterator(XPathItem item)
        {
            XPathNavigator nav = item as XPathNavigator;
            if (nav != null)
            {
                XPathNavigator curr = nav.Clone();
                if (curr.MoveToFirstNamespace())
                {
                    if (TestItem(curr))
                        yield return curr;
                    while (curr.MoveToNextNamespace())
                    {
                        if (TestItem(curr))
                            yield return curr;
                    }
                }
            }
        }
    }
}
