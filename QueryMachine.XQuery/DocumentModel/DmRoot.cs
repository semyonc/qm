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
using System.IO;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace DataEngine.XQuery.DocumentModel
{
    internal class DmRoot : DmContainer
    {
        public DmRoot()
        {
        }

        public override XPathNodeType NodeType
        {
            get
            {
                return XPathNodeType.Root;
            }
        }

        public bool Standalone { get; set; }

        public DmElement DocumentElement
        {
            get
            {
                if (ChildNodes != null)
                    foreach (DmNode child in ChildNodes)
                        if (child.NodeType == XPathNodeType.Element)
                            return (DmElement)child;
                return null;
            }
        }

        public void Dump(TextWriter o)
        {
            o.Write("DmRoot");
            if (HasChildNodes)
            {
                foreach (DmNode node in ChildNodes)
                {
                    o.WriteLine();
                    Dump(o, node, "  ");
                }
            }
        }

        private void Dump(TextWriter o, DmNode node, string pad)
        {
            o.Write(pad);
            if (node is DmElement)
            {
                if (node.Prefix != "")
                {
                    o.Write(node.Prefix);
                    o.Write(":");
                }
                o.Write(node.LocalName);
                o.Write(" [{0}]", node.NamespaceURI);
                DmElement elem = (DmElement)node;
                if (elem.ChildText != null)
                    o.Write(" (mixed)");
                if (elem.HasAttributes)
                {
                    foreach (DmNode attr in elem.ChildAttributes)
                    {
                        o.WriteLine();
                        o.Write(pad + "  @");
                        if (attr.Prefix != "")
                        {
                            o.Write(attr.Prefix);
                            o.Write(":");
                        }
                        o.Write(attr.LocalName);
                        o.Write(" [{0}] ", attr.NamespaceURI);
                    }
                }
                if (elem.HasChildNodes)
                {
                    o.WriteLine();
                    o.Write(pad);
                    o.Write("{");
                    foreach (DmNode child in elem.ChildNodes)
                    {
                        o.WriteLine();
                        Dump(o, child, pad + "  ");
                    }
                    o.WriteLine();
                    o.Write(pad);
                    o.Write("}");
                }
            }
            else if (node is DmPI)
                o.Write("DmPI: {0}", node.Name);
            else
                o.Write(node.NodeType);
        }
    }
}