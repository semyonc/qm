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

namespace DataEngine
{
    public class QueryNodeCollection: CollectionBase
    {
        private QueryNode _container;

        public QueryNodeCollection(QueryNode container)
        {
            _container = container;
        }

        public QueryNode this[int index]
        {
            get
            {
                return ((QueryNode)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(QueryNode value)
        {
            return (List.Add(value));
        }

        public int IndexOf(QueryNode value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, QueryNode value)
        {
            List.Insert(index, value);
        }

        public void Remove(QueryNode value)
        {
            List.Remove(value);
        }

        public bool Contains(QueryNode value)
        {
            return (List.Contains(value));
        }

        protected override void OnValidate(Object value)
        {
            if (!(value is QueryNode))
                throw new ArgumentException();
        }

        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);
            ((QueryNode)value)._parent = _container;
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            base.OnRemoveComplete(index, value);
            ((QueryNode)value)._parent = null;
        }
    }
}
