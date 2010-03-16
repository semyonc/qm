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
using System.Diagnostics;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using DataEngine.CoreServices;
using DataEngine.XQuery.DocumentModel;

namespace DataEngine.XQuery
{
    internal sealed class DescendantIterator : XQueryNodeIterator
    {
        private NodeSet nodeSet;
        private XQueryNodeIterator iter;
        private PageFile pf;
        private XQueryNavigator curr;
        private int[] buffer;
        private int index;
        private int length;
        private int k;
        private int size;

        public DescendantIterator(XQueryNodeIterator baseIter, NodeSet nodes)
        {
            iter = baseIter.Clone();
            nodeSet = nodes;
        }

        [DebuggerStepThrough]
        public override XQueryNodeIterator Clone()
        {
            return new DescendantIterator(iter, nodeSet);
        }

        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return new BufferedNodeIterator(this);
        }

        public override void Init()
        {
            buffer = new int[PageFile.XQueryDirectAcessBufferSize];
        }

        public override XPathItem NextItem()
        {
            while (true)
            {
                if (index < length)
                {
                    if (k == size)
                    {
                        size = pf.Select(nodeSet.nodes, ref index, ref length, buffer);
                        k = 0;
                    }
                    if (size > 0)
                    {
                        curr.Position = buffer[k++];
                        return curr;
                    }
                }
                if (!iter.MoveNext())
                    return null;
                if (!iter.Current.IsNode)
                    throw new XQueryException(Properties.Resources.XPTY0019, iter.Current.Value);
                curr = (XQueryNavigator)iter.Current.Clone();
                pf = curr.Document.pagefile;
                index = Math.Max(curr.Position, nodeSet.inf);
                length = Math.Min(curr.GetLength(), nodeSet.sup - nodeSet.inf + 1);
            }
        }
    }
}
