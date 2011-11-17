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

        public override bool IsContextSensitive(Executive.Parameter[] parameters)
        {
            return _shadowXPath.IsContextSensitive(parameters);
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            _shadowXPath.Bind(parameters, pool);
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            return _shadowXPath.EnumDynamicFuncs();
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            XPathItem item = provider.Context;
            if (item == null)
                throw new XQueryException(Properties.Resources.XPDY0002);
            if (!item.IsNode)
                throw new XQueryException(Properties.Resources.XPTY0019, provider.Context.Value);
            XQueryNavigator nav = item as XQueryNavigator;
            if (nav == null || (nav.NodeType != XPathNodeType.Root && nav.NodeType != XPathNodeType.Element))
                return _shadowXPath.Execute(provider, args, pool);
            return new NodeIterator(Iterator((XQueryNavigator)nav.Clone()));
        }

        public XQueryExprBase LastStep
        {
            get
            {
                return _shadowXPath.LastStep;
            }
        }

        private IEnumerable<XPathItem> Iterator(XQueryNavigator src)
        {
            int inf;
            int sup;
            int bound;
            bool buffered;
            XQueryDocument doc = src.Document;
            PageFile pf = doc.pagefile;
            int index = src.GetBegin(); 
            DmNode node = src.DmNode;
            int[] buffer = new int[XQueryLimits.DirectAcessBufferSize];
            XQueryNavigator res = (XQueryNavigator)src.Clone();
            do
            {                
                buffered = src.IsCompleted;
                if (!buffered)
                {
                    if (index == pf.Count)
                    {
                        src.Document.ExpandUtilElementEnd(src.Position, XQueryLimits.DirectAccessDelta);
                        buffered = src.IsCompleted;
                    }
                }
                if (buffered)
                    bound = src.Position + src.GetEnd() -1;
                else
                    bound = pf.Count -1;
                NodeSet nset;
                lock (doc.SyncRoot)
                {
                    nset = node.GetNodeSet(this);
                    if (nset == null)
                        nset = node.CreateNodeSet(this, _path);
                    nset.GetBounds(out inf, out sup);
                }
                if (index < inf)
                    index = inf;
                int length = Math.Min(bound - index, sup - inf) + 1;
                while (length > 0)
                {
                    int count = pf.Select(nset.hindex, ref index, ref length, buffer);
                    for (int k = 0; k < count; k++)
                    {
                        res.Position = buffer[k];
                        yield return res;
                    }
                }
            }
            while (!buffered);
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
