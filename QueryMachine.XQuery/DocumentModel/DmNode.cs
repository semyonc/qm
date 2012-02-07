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
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.XQuery.MS;

namespace DataEngine.XQuery.DocumentModel
{
    internal class NodeSet
    {
        public readonly HashSet<int> hindex;
        public readonly DmNode[] anchors;        

        public NodeSet(List<DmNode> nodes)
        {
            List<DmNode> anchors = new List<DmNode>();
            foreach (DmNode node in nodes)
                anchors.Add(node);
            this.anchors = anchors.ToArray();
            hindex = new HashSet<int>();
            for (int k = 0; k < anchors.Count; k++)
                hindex.Add(anchors[k]._index);
        }

        public void GetBounds(out int inf, out int sup)
        {
            inf = -1;
            sup = -1;
            foreach (DmNode n in anchors)
            {
                if (inf == -1 || inf > n._begin_pos)
                    inf = n._begin_pos;
                if (sup == -1 || sup < n._end_pos)
                    sup = n._end_pos;
            }
        }
    }

    internal abstract class DmNode
    {        
        internal DmNode _parent = null;
        internal int _index = -1;

        internal Dictionary<object, NodeSet> _cached_set;
        internal int _begin_pos = -1;
        internal int _end_pos = -1;

        internal int _builder_pos = -1;
        internal int _builder_prior_pos = -1;
        
        public DmNode()
        {
        }

        public void IndexNode(int pos)
        {
            if (_begin_pos == -1 || pos < _begin_pos)
                _begin_pos = pos;
            if (_end_pos == -1 || pos > _end_pos)
                _end_pos = pos;
        }

        public abstract XPathNodeType NodeType
        {
            get;
        }

        public DmNode ParentNode
        {
            get
            {
                return _parent;
            }
        }

        public virtual bool HasChildNodes
        {
            get
            {
                return false;
            }
        }

        public virtual bool HasAttributes
        {
            get
            {
                return false;
            }
        }

        public virtual bool HasNamespaces
        {
            get
            {
                return false;
            }
        }

        public virtual DmNodeList ChildNodes
        {
            get
            {
                return null;
            }
        }

        public virtual DmNodeList ChildAttributes
        {
            get
            {
                return null;
            }
        }

        public bool IsText
        {
            get
            {
                return NodeType == XPathNodeType.Text ||
                       NodeType == XPathNodeType.Whitespace ||
                       NodeType == XPathNodeType.SignificantWhitespace;
            }
        }

        public virtual String Name
        {
            get
            {
                string prefix = Prefix;
                if (String.IsNullOrEmpty(prefix))
                    return LocalName;
                else
                    return prefix + ':' + LocalName;
            }
        }

        public virtual String Prefix
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual String LocalName
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual String NamespaceURI
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual IXmlSchemaInfo SchemaInfo
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public DmRoot OwnerDocument
        {
            get
            {
                if (_parent != null)
                {
                    if (_parent.NodeType == XPathNodeType.Root)
                        return (DmRoot)_parent;
                    return _parent.OwnerDocument;
                }
                return null;
            }
        }                

#if DEBUG
        public override string ToString()
        {
            switch (NodeType)
            {
                case XPathNodeType.Element:
                    return String.Format("<{0}>[{1}]", Name, NamespaceURI);
                case XPathNodeType.Attribute:
                    return String.Format("@{0}[{1}]", Name, NamespaceURI);
                case XPathNodeType.Namespace:
                    return String.Format("NS {0}", Name);
                case XPathNodeType.Text:
                    return "T";
                default:
                    return String.Format("{0}:{1}", GetType().Name, Name);
            }
        }
#endif

        public bool IsAncestor(DmNode node)
        {
            for (node = node.ParentNode; node != null; node = node.ParentNode)
            {
                if (node == this)
                    return true;
            }
            return false;
        }

        public bool TestNode(XmlQualifiedNameTest nameTest, XQuerySequenceType typeTest)
        {
            if (nameTest != null)
            {
                return (NodeType == XPathNodeType.Element || NodeType == XPathNodeType.Attribute) &&
                    (nameTest.IsNamespaceWildcard || nameTest.Namespace == NamespaceURI) &&
                    (nameTest.IsNameWildcard || nameTest.Name == LocalName);
            }
            else if (typeTest != null)
                return typeTest.Match(this);
            return true;
        }

        private void DescendantsVisitor(List<DmNode> nodes, bool recursive, DmNode exclude)
        {
            if (ChildNodes != null)
                foreach (DmNode node in ChildNodes)
                {
                    if (node != exclude)
                        nodes.Add(node);
                    if (recursive)
                        node.DescendantsVisitor(nodes, true, exclude);
                }
            DmContainer container = this as DmContainer;
            if (container != null)
            {
                if (container.ChildText != null)
                    nodes.Add(container.ChildText);
                if (container.ChildComment != null)
                    nodes.Add(container.ChildComment);
                if (container.ChildWhitespace != null)
                    nodes.Add(container.ChildWhitespace);
            }
        }

        private void GetNodesVisitor(XQueryPathStep[] path, DmNode curr, int index, int length, List<DmNode> res)
        {
            DmNode[] nodes;
            XQueryPathStep step = path[index];
            switch (step.type)
            {
                case XPath2ExprType.Self:
                    nodes = curr.GetSelf();
                    break;

                case XPath2ExprType.Child:
                    nodes = curr.GetChilds();
                    break;

                case XPath2ExprType.Attribute:
                    nodes = curr.GetAttributes();
                    break;

                case XPath2ExprType.Descendant:
                    nodes = curr.GetDescendants();
                    break;

                case XPath2ExprType.DescendantOrSelf:
                    nodes = curr.GetDescendantOrSelf();
                    break;

                default:
                    throw new ArgumentException();
            }
            foreach (DmNode node in nodes)
                if (node.TestNode(step.nodeTest as XmlQualifiedNameTest, step.nodeTest as XQuerySequenceType))
                {
                    if (index < length - 1)
                        GetNodesVisitor(path, node, index + 1, length, res);
                    else
                        res.Add(node);
                }
        }

        protected void UpdateNodeSet()
        {
            DmNode node = this;
            while (node != null)
            {
                node._cached_set = null;
                node = node._parent;
            }
        }

        public NodeSet GetNodeSet(object key)
        {
            NodeSet res;
            if (_cached_set != null && _cached_set.TryGetValue(key, out res))
                return res;
            return null;
        }

        public NodeSet CreateNodeSet(object key, XQueryPathExpr pathExpr)
        {
            List<DmNode> nodes = new List<DmNode>();
            GetNodesVisitor(pathExpr.Path, this, 0, pathExpr.Path.Length, nodes);
            NodeSet res = new NodeSet(nodes);
            if (_cached_set == null)
                _cached_set = new Dictionary<object, NodeSet>();
            _cached_set.Add(key, res);
            return res;
        }

        public DmNode[] GetSelf()
        {
            List<DmNode> res = new List<DmNode>();
            res.Add(this);
            return res.ToArray();
        }

        public DmNode[] GetChilds()
        {
            List<DmNode> res = new List<DmNode>();
            DescendantsVisitor(res, false, null);
            return res.ToArray();
        }

        public DmNode[] GetAttributes()
        {
            List<DmNode> res = new List<DmNode>();
            if (ChildAttributes != null)
                foreach (DmNode node in ChildAttributes)
                    res.Add(node);
            return res.ToArray();
        }

        public DmNode[] GetDescendants()
        {
            List<DmNode> res = new List<DmNode>();
            DescendantsVisitor(res, true, null);
            return res.ToArray();
        }

        public DmNode[] GetDescendantOrSelf()
        {
            List<DmNode> res = new List<DmNode>();
            res.Add(this);
            DescendantsVisitor(res, true, null);
            return res.ToArray();
        }

        public DmNode[] GetParent()
        {
            List<DmNode> res = new List<DmNode>();
            if (ParentNode != null)
                res.Add(ParentNode);
            return res.ToArray();
        }

        public DmNode[] GetAncestor()
        {
            List<DmNode> res = new List<DmNode>();
            DmNode curr = ParentNode;
            while (curr != null)
            {
                res.Add(curr);
                curr = curr.ParentNode;
            }
            return res.ToArray();
        }

        public DmNode[] GetSiblings()
        {
            List<DmNode> res = new List<DmNode>();
            if (NodeType != XPathNodeType.Attribute && ParentNode != null)
                foreach (DmNode node in ParentNode.GetChilds())
                    if (node != this)
                        res.Add(node);
            return res.ToArray();
        }

        public DmNode[] GetClosure()
        {
            List<DmNode> res = new List<DmNode>();
            OwnerDocument.DescendantsVisitor(res, true, this);
            return res.ToArray();
        }
    }
}
