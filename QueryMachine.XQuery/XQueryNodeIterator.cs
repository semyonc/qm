//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public delegate void ChangeContextDelegate(XQueryNodeIterator iter);

    public interface IContextProvider
    {
        XPathItem Context { get; }
       
        int CurrentPosition { get; }
        
        int LastPosition { get; }        
                
    }

    internal sealed class ContextProvider : IContextProvider
    {
        private XQueryNodeIterator m_iter;

        public ContextProvider(XQueryNodeIterator iter)
        {
            m_iter = iter;
        }

        #region IContextProvider Members

        public XPathItem Context
        {
            get
            {
                return m_iter.Current;
            }
        }

        public int CurrentPosition
        {
            get
            {
                return m_iter.CurrentPosition + 1;
            }
        }

        public int LastPosition
        {
            get
            {
                return m_iter.Count;
            }
        }

        #endregion
    }


    [DebuggerDisplay("{curr}")]
    [DebuggerTypeProxy(typeof(XQueryNodeIteratorDebugView))]
    public abstract class XQueryNodeIterator: ICloneable, IThreadCloneable, IEnumerable, IEnumerable<XPathItem>
    {
        internal int count = -1;
        private XPathItem curr;
        private int pos;
        private bool iteratorStarted;
        private bool iteratorFinished;

        public event ChangeContextDelegate OnChange;

        public XQueryNodeIterator()
        {
        }

        public abstract XQueryNodeIterator Clone();

        public virtual object ThreadClone()
        {
            return new BufferedNodeIterator(this);
        }

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

        public virtual bool IsRange
        {
            get
            {
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

        public virtual bool IsFinished
        {
            get
            {
                return iteratorFinished;
            }
        }

        public virtual int SequentialPosition
        {
            get
            {
                return CurrentPosition + 1;
            }
        }

        public virtual void ResetSequentialPosition()
        {
            return;
        }

        public virtual bool IsOrderedSet
        {
            get
            {
                return false;
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
            XPathItem item = GetNextItem();
            if (item != null)
            {
                pos++;
                curr = item;
                if (OnChange != null)
                    OnChange(this);
                return true;
            }
            iteratorFinished = true;
            return false;
        }
       
        public virtual List<XPathItem> ToList()
        {
            XQueryNodeIterator iter = Clone();
            List<XPathItem> res = new List<XPathItem>();
            while (iter.MoveNext())
                res.Add(iter.Current.Clone());
            return res;
        }

        public abstract XQueryNodeIterator CreateBufferedIterator();

        protected virtual void Init()
        {
        }

        protected virtual XPathItem GetNextItem()
        {
            //Trace.WriteLine(String.Format("[{0}] {1} {2}", 
            //    Thread.CurrentThread.ManagedThreadId, GetHashCode(), GetType().Name));
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

        internal class SingleIterator : XQueryNodeIterator
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

        public class Combinator
        {
            private XQueryNodeIterator[] baseIter;
            private XQueryNodeIterator[] curr;
            private XPathItem[] items;

            public Combinator(XQueryNodeIterator[] iter)
            {
                baseIter = iter;
                curr = new XQueryNodeIterator[iter.Length];
                curr[0] = iter[0].Clone();
                items = new XPathItem[iter.Length];
            }

            private bool Next(int index)
            {
                if (curr[index] != null && curr[index].MoveNext())
                    return true;
                else
                    if (index > 0)
                    {
                        if (Next(index - 1))
                        {
                            curr[index] = baseIter[index].Clone();
                            if (curr[index].MoveNext())
                                return true;
                        }
                    }
                return false;
            }

            public bool Next()
            {
                if (Next(baseIter.Length - 1))
                {
                    for (int k = 0; k < curr.Length; k++)
                        items[k] = curr[k].Current;
                    return true;
                }
                return false;
            }

            public XPathItem[] Current
            {
                get
                {
                    return items; 
                }
            }

            public Combinator Clone()
            {
                XQueryNodeIterator[] iter = new XQueryNodeIterator[baseIter.Length];
                for (int k = 0; k < iter.Length; k++)
                    iter[k] = baseIter[k].Clone();
                return new Combinator(iter);
            }
        }
    }
}
