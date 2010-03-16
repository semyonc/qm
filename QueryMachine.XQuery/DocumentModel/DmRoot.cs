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

        public override XdmNode CreateNode()
        {
            return new XdmDocument();
        }

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