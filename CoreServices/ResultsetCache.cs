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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataEngine.CoreServices.Data
{
    public class RowCache
    {
        private LinkedList<Row> _rows = new LinkedList<Row>();
        private RowType _rowType;

        internal RowCache(Resultset rs)
        {
            _rowType = rs._rtype;
        }

        public bool IsValid { get; private set; }

        public RowType RowType { get { return _rowType; } }

        public void Add(Row row)
        {
            if (_rowType != row._type)
                throw new InvalidOperationException();
            _rows.AddLast(row);
        }

        public void Fill(Resultset rs)
        {
            if (rs._rtype != _rowType)
                throw new InvalidOperationException();
            Row row = rs.Begin;
            while (row != null)
            {
                _rows.AddLast(row);
                row = rs.NextRow(row);
            }
            IsValid = true;
        }

        public Resultset GetResultset()
        {
            Resultset rs = new Resultset(_rowType, null);
            foreach (Row row in _rows)
                rs.Enqueue(row);
            return rs;
        }

        public void SetValid()
        {
            IsValid = true;
        }

        public void Clear()
        {
            _rows.Clear();
        }
    }

    public class ResultsetCache
    {
        private class Key
        {
            public QueryNode Node { get; private set; }
            public Object[] Parameters { get; private set; }

            public Key(QueryNode node, object[] parameters)
            {
                Node = node;
                Parameters = parameters;
            }

            public override bool Equals(object obj)
            {
                if (obj is Key)
                    return Equals((Key)obj);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                int hashCode = Node.GetHashCode();
                if (Parameters != null)
                    foreach (object p in Parameters)
                        if (p != null)
                            hashCode ^= p.GetHashCode();
                return hashCode;
            }

            private bool Equals(Key k)
            {
                return Node == k.Node &&
                    ParametersEqual(k.Parameters);
            }

            private bool ParametersEqual(object[] p)
            {
                if (Parameters == p)
                    return true;
                if (Parameters != null && p != null)
                    if (Parameters.Length == p.Length)
                    {
                        for (int k = 0; k < p.Length; k++)
                            if (! ParameterEqual(Parameters[k], p[k]))
                                return false;
                        return true;
                    }                    
                return false;
            }

            private bool ParameterEqual(object p1, object p2)
            {
                if (p1 != null && p2 != null)
                    return p1.Equals(p2);
                else
                    return p1 == null && p2 == null;
            }
        }

        private Dictionary<Key, WeakReference> inner;

        public ResultsetCache()
        {
            inner = new Dictionary<Key, WeakReference>();
        }

        public int CacheRequest = 0;
        public int CacheHit = 0;

        public int Count
        {
            get
            {
                CleanAbandonedItems();
                return inner.Count;
            }
        }

        public void Clear()
        {
            foreach (WeakReference wr in inner.Values)
            {
                RowCache curr = (RowCache)wr.Target;
                if (curr != null)
                    curr.Clear();
            }
            inner.Clear();
        }

        private void CleanAbandonedItems()
        {
            List<Key> deadKeys = new List<Key>();

            foreach (KeyValuePair<Key, WeakReference> kvp in inner)
                if (kvp.Value.Target == null)
                    deadKeys.Add(kvp.Key);

            foreach (Key key in deadKeys)
                inner.Remove(key);
        }

        public RowCache Get(QueryNode node, Object[] parameters)
        {
            WeakReference wr;
            Key key = new Key(node, parameters);
            CacheRequest++;

            if (inner.TryGetValue(key, out wr))
            {
                object result = wr.Target;

                if (result == null)
                    inner.Remove(key);
                else
                {
                    RowCache rc = (RowCache)result;
                    if (rc.IsValid)
                    {
                        CacheHit++;
                        return rc;
                    }
                    else
                        inner.Remove(key);
                }
            }
            
            return null;
        }

        public void Add(QueryNode node, Object[] parameters, Resultset rs)
        {
            if (rs._cache_wr != null)
                throw new InvalidOperationException();
            Key key = new Key(node, parameters);
            RowCache rowCache = new RowCache(rs);
            inner[key] = 
                rs._cache_wr = new WeakReference(rowCache);
            if (rs._context == null)
                rowCache.Fill(rs);
        }
    }
}
