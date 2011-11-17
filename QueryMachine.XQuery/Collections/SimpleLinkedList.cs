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

namespace DataEngine.XQuery.Collections
{
    internal class SimpleLinkedList<T> : ICloneable, IEnumerable, IEnumerable<T>
    {
        private class SimpleListNode
        {
            public T value;
            public SimpleListNode next;

            public SimpleListNode(T value)
            {
                this.value = value;
            }                   
        }

        private int count;
        private SimpleListNode first;
        private SimpleListNode last;

        public void Add(T value)
        {
            SimpleListNode node = new SimpleListNode(value);
            if (first == null)
                first = last = node;
            else
            {
                last.next = node;
                last = node;
            }
            count++;
        }

        public void Add(SimpleLinkedList<T> src)
        {
            if (!src.IsEmpty)
            {
                if (first == null)
                {
                    first = src.first;
                    last = src.last;
                }
                else
                {
                    last.next = src.first;
                    last = src.last;
                }
                count += src.count;
                src.Clear();
            }
        }

        public T Remove()
        {
            if (first == null)
                throw new InvalidOperationException("The list is empty");
            T value = first.value;
            first = first.next;
            if (first == null)
                last = null;
            count--;
            return value;
        }

        public void Clear()
        {
            first = last = null;
        }

        public SimpleLinkedList<T> Clone()
        {
            SimpleLinkedList<T> res = new SimpleLinkedList<T>();
            foreach (T val in this)
                res.Add(val);
            return res;
        }

        public T[] ToArray()
        {
            T[] res = new T[count];
            int index = 0;
            SimpleListNode node = first;
            while (node != null)
            {
                res[index++] = node.value;
                node = node.next;
            }
            return res;
        }

        public bool IsEmpty
        {
            get
            {
                return first == null;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public override string  ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            int i = 0;
            foreach (T item in this)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(item.ToString());
                i++;
            }
            sb.Append(']');
            return sb.ToString();
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
            return new Enumerator(first);
        }

        #endregion

        #region IEnumerable<XPathItem> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(first);
        }

        #endregion

        private class Enumerator : IEnumerator, IEnumerator<T>
        {
            private SimpleListNode first;
            private SimpleListNode current;
            private bool iterationStarted;

            public Enumerator(SimpleListNode node)
            {
                first = node;
                iterationStarted = false;
            }

            #region IEnumerator<T> Members

            public T Current
            {
                get 
                {
                    if (!iterationStarted || current == null)
                        throw new InvalidOperationException();
                    return current.value;
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {                
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get 
                {
                    if (!iterationStarted || current == null)
                        throw new InvalidOperationException();
                    return current.value;
                }
            }

            public bool MoveNext()
            {
                if (!iterationStarted)
                {
                    current = first;
                    iterationStarted = true;
                    if (current != null)
                        return true;
                }
                else
                {
                    if (current.next != null)
                    {
                        current = current.next;
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                iterationStarted = false;
                current = null;
            }

            #endregion
        }
    }
}
