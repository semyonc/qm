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
using System.Collections;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Runtime.CompilerServices;

namespace DataEngine.XQuery.DocumentModel
{
    internal struct DmQName
    {
        public string prefix;
        public string localName;
        public string ns;

        public DmQName(string prefix, string localName, string ns, XmlNameTable nameTable)
        {
            this.prefix = nameTable.Add(prefix);
            this.localName = nameTable.Add(localName);
            this.ns = nameTable.Add(ns);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(localName);
        }

        public override bool Equals(object obj)
        {            
            if (obj is DmQName)
            {
                DmQName other = (DmQName)obj;
                return Object.ReferenceEquals(localName, other.localName) &&
                    Object.ReferenceEquals(prefix, other.prefix) &&
                    Object.ReferenceEquals(ns, other.ns);
            }
            return false;
        }
    }

    abstract class DmContainer: DmNode
    {
        private Dictionary<DmQName, DmNode> _childs;

        private DmComment _comment;
        private DmText _text;
        private DmWhitespace _whitespace;

        public DmNode CreateChild(DmQName name)
        {
            if (_childs == null)
                _childs = new Dictionary<DmQName, DmNode>();
            DmNode node;
            if (_childs.TryGetValue(name, out node))
                return node;
            node = new DmElement(name);
            node._parent = this;
            _childs.Add(name, node);
            UpdateNodeSet();
            return node;
        }

        public DmNode CreateChildPI(string name, XmlNameTable nameTable)
        {
            DmQName id = new DmQName(String.Empty, name, DmPI.InternalNs, nameTable);
            if (_childs == null)
                _childs = new Dictionary<DmQName, DmNode>();
            DmNode node;
            if (_childs.TryGetValue(id, out node))
                return node;
            node = new DmPI(id.localName);
            node._parent = this;
            _childs.Add(id, node);
            UpdateNodeSet();
            return node;
        }

        public DmNode CreateChildText()
        {
            if (_text == null)
            {
                _text = new DmText(this);
                UpdateNodeSet();
            }
            return _text;
        }

        public DmNode CreateChildComment()
        {
            if (_comment == null)
            {
                _comment = new DmComment(this);
                UpdateNodeSet();
            }
            return _comment;
        }

        public DmNode CreateChildWhitespace()
        {
            if (_whitespace == null)
            {
                _whitespace = new DmWhitespace(this);
                UpdateNodeSet();
            }
            return _whitespace;
        }

        public override bool HasChildNodes
        {
            get
            {
                return _childs != null;
            }
        }

        public override DmNodeList ChildNodes
        {
            get
            {
                if (_childs == null)
                    return null;
                return new DmNodeList(_childs.Values);
            }
        }

        public DmText ChildText
        {
            get
            {
                return _text;
            }
        }

        public DmComment ChildComment
        {
            get
            {
                return _comment;
            }
        }

        public DmWhitespace ChildWhitespace
        {
            get
            {
                return _whitespace;
            }
        }
    }
}
