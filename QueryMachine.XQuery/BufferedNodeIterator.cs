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
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
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
                if (IsFinished)
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
                    if (IsFinished && buffer.Count == 1)
                        return true;
                    return base.IsSingleIterator;
                }
            }
        }

        public void Fill()
        {
            XQueryNodeIterator iter = Clone();
            while (iter.MoveNext())
                ;
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
            int index = CurrentPosition + 1;
            if (index < buffer.Count)
                return iter[index];
            else
                if (!src.IsFinished)
                {
                    lock (src)
                    {
                        int n = 0;
                        XPathItem res = null;
                        while (src.MoveNext())
                        {
                            if (res == null)
                                res = src.Current.Clone();
                            buffer.Add(src.Current);
                            if (n == XQueryLimits.IteratorPrefetchSize)
                                break;
                            n++;
                        }
                        return res;
                    }
                }
            return null;
        }
        
        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return Clone();
        }
    }
}
