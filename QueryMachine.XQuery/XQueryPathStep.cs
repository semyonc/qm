//        Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

using DataEngine.CoreServices;
using DataEngine.XQuery.Iterator;

namespace DataEngine.XQuery
{
    public enum XPath2ExprType
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
        Namespace,
        PositionFilter,
        ChildOverDescendants,   
        Expr,
        DirectAccess
    };

    class XQueryPathStep
    {
        public readonly object nodeTest;
        public readonly XPath2ExprType type;
        public readonly XQueryExprBase expr;

        public XQueryPathStep(XQueryExprBase expr)
        {
            this.nodeTest = null;
            this.type = XPath2ExprType.Expr;
            this.expr = expr;
        }

        public XQueryPathStep(object nodeTest, XPath2ExprType type)
        {
            this.nodeTest = nodeTest;
            this.type = type;
            this.expr = null;
        }

        public XQueryPathStep(XPath2ExprType type)
            : this(null, type)
        {
        }

        public XQueryPathStep(ChildOverDescendantsNodeIterator.NodeTest nodeTest)
        {
            if (nodeTest.nameTest != null)
                this.nodeTest = nodeTest.nameTest;
            else
                this.nodeTest = nodeTest.typeTest;
            type = XPath2ExprType.Child;
        }

        public XQueryNodeIterator Create(XQueryContext context, object[] args, MemoryPool pool, XQueryNodeIterator baseIter, bool special)
        {
            switch (type)
            {
                case XPath2ExprType.Attribute:
                    return new AttributeNodeIterator(context, nodeTest, baseIter);
                case XPath2ExprType.Child:
                    {
                        if (special && nodeTest != XQuerySequenceType.Node)
                            return new SpecialChildNodeIterator(context, nodeTest, baseIter);
                        return new ChildNodeIterator(context, nodeTest, baseIter);
                    }
                case XPath2ExprType.Descendant:
                    {
                        if (special && nodeTest != XQuerySequenceType.Node)
                            return new SpecialDescendantNodeIterator(context, nodeTest, false, baseIter);
                        return new DescendantNodeIterator(context, nodeTest, false, baseIter);
                    }
                case XPath2ExprType.DescendantOrSelf:
                    {
                        if (special && nodeTest != XQuerySequenceType.Node)
                            return new SpecialDescendantNodeIterator(context, nodeTest, true, baseIter);
                        return new DescendantNodeIterator(context, nodeTest, true, baseIter);
                    }
                case XPath2ExprType.Ancestor:
                    return new AncestorNodeIterator(context, nodeTest, false, baseIter);
                case XPath2ExprType.AncestorOrSelf:
                    return new AncestorNodeIterator(context, nodeTest, true, baseIter);
                case XPath2ExprType.Following:
                    return new FollowingNodeIterator(context, nodeTest, baseIter);
                case XPath2ExprType.FollowingSibling:
                    return new FollowingSiblingNodeIterator(context, nodeTest, baseIter);
                case XPath2ExprType.Parent:
                    return new ParentNodeIterator(context, nodeTest, baseIter);
                case XPath2ExprType.Preceding:
                    return new PrecedingNodeIterator(context, nodeTest, baseIter);
                case XPath2ExprType.PrecedingSibling:
                    return new PrecedingSiblingNodeIterator(context, nodeTest, baseIter);
                case XPath2ExprType.Namespace:
                    return new NamespaceNodeIterator(context, nodeTest, baseIter);
                case XPath2ExprType.Self:
                    return new SelfNodeIterator(context, nodeTest, baseIter);
                case XPath2ExprType.Expr:
                    return new ExprNodeIterator(expr, args, pool, baseIter);
                case XPath2ExprType.PositionFilter:
                    return new PositionFilterNodeIterator(Convert.ToInt32(nodeTest), baseIter);
                case XPath2ExprType.ChildOverDescendants:
                    return new ChildOverDescendantsNodeIterator(context,
                        (ChildOverDescendantsNodeIterator.NodeTest[])nodeTest, baseIter);
                case XPath2ExprType.DirectAccess:
                    return new DirectAccessNodeIterator((XQueryPathExpr)nodeTest, args, pool, baseIter);
                default:
                    return null;
            }
        }

        public XQueryPathStep Next { get; private set; }

        public void AddLast(object node)
        {
            XQueryPathStep pathStep = node as XQueryPathStep;
            if (pathStep == null)
                pathStep = new XQueryPathStep((XQueryExprBase)node);
            XQueryPathStep last = this;
            while (last.Next != null)
                last = last.Next;
            last.Next = pathStep;
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            if (nodeTest != null)
            {
                sb.Append(nodeTest);
                sb.Append(", ");
            }
            sb.Append(type);
            if (expr != null)
            {
                sb.Append(", ");
                sb.Append(expr.ToString());
            }
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }
}
