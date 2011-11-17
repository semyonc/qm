//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Threading;
using System.Diagnostics;

using DataEngine.CoreServices;
using DataEngine.XQuery.Collections;


namespace DataEngine.XQuery
{
    public sealed class BufferedNodeIterator: XQueryNodeIterator
    {
        private ItemList buffer;
        private ItemList.Iterator iter;
        private XQueryNodeIterator src;

        [DebuggerStepThrough]
        private BufferedNodeIterator()
        {
        }

        public BufferedNodeIterator(XQueryNodeIterator src)
            : this(src, true)
        {
        }

        public BufferedNodeIterator(XQueryNodeIterator src, bool clone)
        {
            this.src = clone ? src.Clone() : src;
            buffer = new ItemList();
        }

        public override int Count
        {
            get
            {
                if (buffer._finished)
                    return buffer.Count;
                return base.Count;
            }
        }

        public override bool IsSingleIterator
        {
            get
            {
                if (buffer.Count > 1)
                    return false;
                else
                {
                    if (buffer._finished && buffer.Count == 1)
                        return true;
                    return base.IsSingleIterator;
                }
            }
        }

        public void Fill()
        {
            if (!buffer._finished)
            {
                lock (src)
                {
                    if (!buffer._finished)
                    {
                        while (src.MoveNext())
                            buffer.Add(src.Current.Clone());
                        buffer._finished = true;
                    }
                }
            }
        }

        public static BufferedNodeIterator Preload(XQueryNodeIterator baseIter)
        {
            BufferedNodeIterator res = new BufferedNodeIterator(baseIter);
            res.Fill();
            return res;
        }

        [DebuggerStepThrough]
        public override XQueryNodeIterator Clone()
        {
            BufferedNodeIterator clone = new BufferedNodeIterator();
            clone.src = src;
            clone.buffer = buffer;
            return clone;
        }

        protected override void Init()
        {
            iter = buffer.CreateIterator();
        }

        protected override XPathItem NextItem()
        {
            bool lockTaken = false;
            if (!buffer._finished)
                Monitor.Enter(src, ref lockTaken);
            try
            {
                int index = CurrentPosition + 1;
                if (index < buffer.Count)
                    return iter[index];
                else
                {
                    if (!buffer._finished)
                    {
                        if (src.MoveNext())
                        {
                            buffer.Add(src.Current);
                            return src.Current.Clone();
                        }
                        buffer._finished = true;
                    }
                    return null;
                }
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(src);
            }
        }

        public override object ThreadClone()
        {
            return Clone();
        }
        
        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return Clone();
        }
    }
}
