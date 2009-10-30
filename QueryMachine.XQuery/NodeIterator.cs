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

namespace DataEngine.XQuery
{
    public class NodeIterator: XQueryNodeIterator
    {
        private IEnumerable<XPathItem> master;
        private IEnumerator<XPathItem> iterator;        
        private bool iterationStarted;
        private int pos = -1;
        
        public NodeIterator(IEnumerable<XPathItem> enumerable)
        {
            master = enumerable;
            iterator = master.GetEnumerator();
            iterationStarted = false;
        }        

        [DebuggerStepThrough]
        public override XQueryNodeIterator Clone()
        {
            NodeIterator iter = new NodeIterator(master);
            iter.ItemType = ItemType;
            return iter;
        }

        public override XPathItem Current
        {
            get 
            {
                if (!iterationStarted)
                    throw new InvalidOperationException();
                return iterator.Current;
            }
        }
        
        public override int CurrentPosition
        {
            get 
            {
                if (!iterationStarted)
                    throw new InvalidOperationException();
                return pos; 
            }
        }

        [DebuggerStepThrough]
        public override bool MoveNext()
        {
            if (!iterationStarted)
            {
                pos = -1;
                iterationStarted = true;
            }
            if (iterator.MoveNext())
            {
                pos++;
                return true;
            }
            else
                return false;
        }
    }
}
