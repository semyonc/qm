/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2009  Semyon A. Chertkov (semyonc@gmail.com)

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;


namespace DataEngine.CoreServices.Data
{
    public class Resultset: IListSource, IEnumerable<Row>, ICloneable
    {        
        internal Row _begin;
        internal Row _end;
        internal int _count;

        internal RowType _rtype;        
        internal QueryNode.DemandProcessingContext _context;
        internal WeakReference _cache_wr;
        
        public Resultset(RowType rtype, QueryNode.DemandProcessingContext context)          
        {
            _context = context;
            _rtype = rtype;
        }

        public void CheckValid()
        {
        }

        public RowType RowType
        {
            get { return _rtype; }
        }

        public RowType.TypeInfo GetFieldType(RowType.Locator locator)
        {
            RowType.TypeInfo res = _rtype.Fields[locator.master];
            if (locator.detail != null)
                res = res.NestedType.Fields[locator.detail.Value];
            return res;
        }

        public object Clone()
        {
            Resultset rs = new Resultset(_rtype, null);
            for (Row r = Begin; r != null; r = NextRow(r))
                rs.Enqueue((Row)r.Clone());
            return rs;
        }

        public Row Begin
        {
            get 
            {
                CheckValid();
                if (_begin == null)
                    BeforeReturnEOF();
                return _begin;
            }
        }

        public Row End
        {
            get
            {
                CheckValid();
                return _end;
            }
        }

        public int Count
        {
            get
            {
                CheckValid();
                return _count;
            }
        }

        public Row NewRow()
        {
            CheckValid();
            return new Row(_rtype);
        }

        public void Enqueue(Row row)
        {
            CheckValid();
#if PARALLEL
            lock(this) 
#endif 
            {
                if (_begin == null)
                    _begin = _end = row;
                else
                {
                    _end._next = row;
                    _end = row;
                }
                if (_cache_wr != null)
                {
                    RowCache rc = (RowCache)_cache_wr.Target;
                    if (rc != null)
                        rc.Add(row);
                    else
                        _cache_wr = null;
                }
                _count++;
            }
        }

        public Row Dequeue()
        {
            CheckValid();
            if (_begin == null)
                throw new InvalidOperationException();
            else
#if PARALLEL
                lock (this)
#endif 
                {
                    Row curr = _begin;
                    _begin = curr._next;
                    if (_begin == null)
                        _end = null;
                    _count--;
                    curr._next = null;
                    return curr;
                }
        }

        public void Truncate()
        {
            Dequeue();                        
        }

        public Row NextRow(Row row)
        {
            if (row._next == null)
                BeforeReturnEOF();
            return row._next;
        }        

        public void Clear()
        {
            CheckValid();
            while (_begin != null)
                Truncate();
        }

        public void Fill()
        {
            Row current = Begin;
            while (current != null)
                current = NextRow(current);
        }

        public void Cancel()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
            _cache_wr = null;
        }

        internal void BeforeReturnEOF()
        {
            if (_context != null)
            {
                try
                {
                    _context.CheckCancelled();
                    if (!_context.ProcessNextPiece(this))
                    {
                        _context.Dispose();
                        _context = null;
                        InvalidateCache();
                    }
                }
                catch
                {
                    _context.Dispose();
                    _context = null;
                    _cache_wr = null;
                    throw;
                }
            }
        }

        private void InvalidateCache()
        {
            if (_cache_wr != null)
            {
                RowCache rc = (RowCache)_cache_wr.Target;
                if (rc != null)
                    rc.SetValid();
                else
                    _cache_wr = null;
            }
        }

        #region Sort

        public void Sort(RowComparer comparer)
        {
            if (_begin == null || _begin._next == null)
                return;

            Row source0, source1;
            Row[] target = new Row[2];
            Row[] targetlast = new Row[2];
            int dest = 0;
            Row n = _begin;

            while (n != null)
            {
                Row next = n._next;
                n._next = target[dest];
                target[dest] = n;
                n = next;
                dest ^= 1;
            }

            for (int blocksize = 1; target[1] != null; blocksize <<= 1)
            {
                source0 = target[0];
                source1 = target[1];
                target[0] = target[1] = targetlast[0] = targetlast[1] = null;
                for (dest = 0; source0 != null; dest ^= 1)
                {
                    int n0 = blocksize,
                        n1 = blocksize;

                    while (true)
                    {
                        if (n0 == 0 || source0 == null)
                        {
                            if (n1 == 0 || source1 == null)
                                break;
                            n = source1;
                            source1 = source1._next;
                            n1--;
                        }
                        else if (n1 == 0 || source1 == null)
                        {
                            n = source0;
                            source0 = source0._next;
                            n0--;
                        }
                        else if (comparer.Compare(source0, source1) > 0)
                        {
                            n = source1;
                            source1 = source1._next;
                            n1--;
                        }
                        else
                        {
                            n = source0;
                            source0 = source0._next;
                            n0--;
                        }

                        if (target[dest] == null)
                            target[dest] = n;
                        else
                            targetlast[dest]._next = n;
                        
                        targetlast[dest] = n;
                        n._next = null;
                    }
                }
            }
            _begin = target[0];
            _end = targetlast[0];
        }
        #endregion

        #region IListSource Members

        public bool ContainsListCollection
        {
            get { return false; }
        }

        public System.Collections.IList GetList()
        {
            return new ResultsetView(this);
        }

        #endregion

        #region IEnumerable<Row> Members

        public IEnumerator<Row> GetEnumerator()
        {
            Row current = Begin;
            while (current != null)
            {
                yield return current;
                current = NextRow(current);
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            Row current = Begin;
            while (current != null)
            {
                yield return current;
                current = NextRow(current);
            }
        }

        #endregion
    
    }
}
