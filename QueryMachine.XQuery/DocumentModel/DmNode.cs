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

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;


namespace DataEngine.XQuery.DocumentModel
{
    internal class NodeSet
    {
        public readonly DmNode[] nodes;
        public readonly int inf;
        public readonly int sup;

        public NodeSet(DmNode[] nodes)
        {
            this.nodes = nodes;
            inf = -1;
            sup = -1;
            foreach (DmNode node in nodes)
            {
                if (inf == -1 || inf > node._begin_pos)
                    inf = node._begin_pos;
                if (sup == -1 || sup < node._end_pos)
                    sup = node._end_pos;
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
        
        public abstract XdmNode CreateNode();

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
        public override string  ToString()
        {
            string name = Name;
            if (name == "")
                return GetType().Name;
            return String.Format("{0}:{1}", GetType().Name, name);
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

        protected class DmSchemaInfo : XmlSchemaInfo
        {
            public DmSchemaInfo(IXmlSchemaInfo xmlSchemaInfo)
            {
                IsDefault = xmlSchemaInfo.IsDefault;
                IsNil = xmlSchemaInfo.IsNil;
                MemberType = xmlSchemaInfo.MemberType;
                SchemaAttribute = xmlSchemaInfo.SchemaAttribute;
                SchemaElement = xmlSchemaInfo.SchemaElement;
                SchemaType = xmlSchemaInfo.SchemaType;
                Validity = xmlSchemaInfo.Validity;
            }
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

        public NodeSet GetNodeSet(object key)
        {
            NodeSet res;
            if (_cached_set != null && _cached_set.TryGetValue(key, out res))
                return res;
            return null;
        }

        public NodeSet CreateNodeSet(object key, DmNode[] nodes)
        {
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
