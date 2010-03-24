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

using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public interface IContextProvider
    {
        XPathItem Context { get; }
        int CurrentPosition { get; }
        int LastPosition { get; }
    }

    [DebuggerDisplay("{curr}")]
    [DebuggerTypeProxy(typeof(XQueryNodeIteratorDebugView))]
    public abstract class XQueryNodeIterator: ICloneable, IEnumerable, IEnumerable<XPathItem>
    {
        internal int count = -1;
        private XPathItem curr;
        private int pos;
        private bool iteratorStarted;
        private bool iteratorFinished;

        public XQueryNodeIterator()
        {
        }

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

        public virtual bool IsSingleIterator
        {
            get
            {
                XQueryNodeIterator iter = Clone();
                if (iter.MoveNext() && !iter.MoveNext())
                    return true;
                return false;
            }
        }

        public XPathItem Current 
        {
            get
            {
                if (!iteratorStarted)
                    throw new InvalidOperationException();
                return curr;
            }
        }

        public int CurrentPosition 
        {
            get
            {
                if (!iteratorStarted)
                    throw new InvalidOperationException();
                return pos;
            }
        }

        public bool IsFinished
        {
            get
            {
                return iteratorFinished;
            }
        }

        [DebuggerStepThrough]
        public bool MoveNext()
        {
            if (!iteratorStarted)
            {
                Init();
                pos = -1;
                iteratorStarted = true;
            }
#if PARALLEL
            CheckThreadCanceled();
#endif
            XPathItem item = GetNextItem();
            if (item != null)
            {
                pos++;
                curr = item;
                return true;
            }
            iteratorFinished = true;
            return false;
        }

#if PARALLEL
        public static event EventHandler CheckThread;

        public static void CheckThreadCanceled()
        {
            if (CheckThread != null)
                CheckThread(null, EventArgs.Empty);
        }
#endif

        public abstract XQueryNodeIterator CreateBufferedIterator();

        public virtual void Init()
        {
        }

        protected virtual XPathItem GetNextItem()
        {
            return NextItem();
        }

        protected abstract XPathItem NextItem();

        public static XQueryNodeIterator Create(object value)
        {
            if (value == Undefined.Value)
                return EmptyIterator.Shared;
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
                return iter.Clone();
            XPathItem item = value as XPathItem;
            if (item == null)
                item = new XQueryItem(value);
            return new SingleIterator(item);
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

        private class SingleIterator : XQueryNodeIterator
        {
            private XPathItem _item;            

            public SingleIterator(XPathItem item)
            {
                _item = item;
            }

            public override XQueryNodeIterator Clone()
            {
                return new SingleIterator(_item);
            }

            public override bool IsSingleIterator
            {
                get
                {
                    return true;
                }
            }

            protected override XPathItem NextItem()
            {
                if (CurrentPosition == -1)
                    return _item;
                return null;
            }

            public override XQueryNodeIterator CreateBufferedIterator()
            {
                return Clone();
            }
        }
                
        internal class XQueryNodeIteratorDebugView
        {
            private XQueryNodeIterator iter;

            public XQueryNodeIteratorDebugView(XQueryNodeIterator iter)
            {
                this.iter = iter;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public XPathItem[] Items
            {
                get
                {
                    List<XPathItem> res = new List<XPathItem>();
                    foreach (XPathItem item in iter)
                    {
                        if (res.Count == 10)
                            break;
                        res.Add(item.Clone());
                    }
                    return res.ToArray();
                }
            }

            public XPathItem Current
            {
                get
                {
                    return iter.curr;
                }
            }

            public int CurrentPosition
            {
                get
                {
                    return iter.pos;
                }
            }
        }
    }
}
