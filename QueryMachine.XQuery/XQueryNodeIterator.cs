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
using System.Diagnostics;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace DataEngine.XQuery
{
    public interface IContextProvider
    {
        XPathItem Context { get; }
        int CurrentPosition { get; }
        int LastPosition { get; }
    }

    [DebuggerDisplay("Position={CurrentPosition}, Current={Current}")]
    public abstract class XQueryNodeIterator: ICloneable, IEnumerable, IEnumerable<XPathItem>
    {
        internal int count = -1;

        public abstract XQueryNodeIterator Clone();

        public virtual int Count
        {
            get
            {
                if (this.count == -1)
                {
                    count = 0;
                    XQueryNodeIterator iter = Clone();
                    while (iter.MoveNext())
                        count++;
                }
                return count;
            }
        }

        public abstract XPathItem Current { get; }

        public abstract int CurrentPosition { get; }

        public abstract bool MoveNext();

        public XQuerySequenceType ItemType { get; set; }

        public XQueryNodeIterator()
        {
            ItemType = XQuerySequenceType.Item;
        }
        

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion        

        #region IEnumerable<XPathItem> Members

        IEnumerator<XPathItem> IEnumerable<XPathItem>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        private class Enumerator : IEnumerator, IEnumerator<XPathItem>
        {
            private XQueryNodeIterator current;
            private bool iterationStarted;
            private XQueryNodeIterator original;

            public Enumerator(XQueryNodeIterator iter)
            {
                original = iter.Clone();
            }

            public object Current
            {
                get 
                {
                    if (!iterationStarted || current == null)
                        throw new InvalidOperationException();
                    return current.Current;
                }
            }

            [DebuggerStepThrough]
            public bool MoveNext()
            {
                if (!iterationStarted)
                {
                    current = original.Clone();
                    iterationStarted = true;
                }
                if (current != null && current.MoveNext())
                    return true;
                current = null;
                return false;
            }

            public void Reset()
            {
                iterationStarted = false;
            }

            #region IEnumerator<XPathItem> Members

            XPathItem IEnumerator<XPathItem>.Current
            {
                get 
                {
                    if (!iterationStarted || current == null)
                        throw new InvalidOperationException();
                    return current.Current;
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                return;
            }

            #endregion
        }
    }
}
