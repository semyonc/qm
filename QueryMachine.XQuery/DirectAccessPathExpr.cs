//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
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
    internal sealed class DirectAccessPathExpr: XQueryExprBase
    {
        private XQueryPathExpr _shadowXPath;
        private XQueryExprBase[] _path;        

        public DirectAccessPathExpr(XQueryContext context, XQueryExprBase[] path, bool isOrdered)
            : base(context)
        {
            XQueryExprBase[] tmp = new XQueryExprBase[path.Length + 1];
            tmp[0] = new XQueryStepExpr(XQueryPathExprType.Self, context);
            for (int k = 0; k < path.Length; k++)
                tmp[k + 1] = path[k];
            _shadowXPath = new XQueryPathExpr(context, tmp, isOrdered);
            _path = path;
        }

        public override void Bind(Executive.Parameter[] parameters)
        {
            _shadowXPath.Bind(parameters);
        }

        public override IEnumerable<SymbolLink> EnumDynamicFuncs()
        {
            return _shadowXPath.EnumDynamicFuncs();
        }

        public override object Execute(IContextProvider provider, object[] args)
        {
            XPathItem item = provider.Context;
            if (!item.IsNode)
                throw new XQueryException(Properties.Resources.XPTY0019, provider.Context.Value);
            XQueryNavigator nav = item as XQueryNavigator;
            if (nav == null || (nav.NodeType != XPathNodeType.Root && nav.NodeType != XPathNodeType.Element))
                return _shadowXPath.Execute(provider, args);
            return new NodeIterator(Iterator((XQueryNavigator)nav.Clone()));
        }

        private IEnumerable<XPathItem> Iterator(XQueryNavigator src)
        {
            int inf;
            int sup;
            int length = src.GetLength();
            DmNode node = src.DmNode;
            NodeSet nset = node.GetNodeSet(this);
            if (nset == null)
                nset = node.CreateNodeSet(this, _path);
            nset.GetBounds(out inf, out sup);
            int index;
            if (src.Position < inf)
            {
                index = inf;
                length -= inf - src.Position;
            }
            else
                index = src.Position;
            length = Math.Min(length, sup - inf + 1);
            XQueryNavigator res = (XQueryNavigator)src.Clone();
            PageFile pf = src.Document.pagefile;
            int[] buffer = new int[PageFile.XQueryDirectAcessBufferSize];
            while (length > 0)
            {
                int size = pf.Select(nset.hindex, ref index, ref length, buffer);
                for (int k = 0; k < size; k++)
                {
                    res.Position = buffer[k];
                    if (nset.Accept(res))
                        yield return res;
                    if (nset.mixed && res.IsLeafTextElement)
                    {
                        res.MoveToFirstChild();
                        if (nset.Accept(res))
                            yield return res;
                    }                        
                }
            }
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            for (int k = 0; k < _path.Length; k++)
            {
                if (k > 0)
                    sb.Append(", ");
                sb.Append(_path[k].ToString());
            }
            sb.Append("]");
            return sb.ToString();

        }
#endif
    }
}
