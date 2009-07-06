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
using System.ComponentModel;

namespace DataEngine.CoreServices.Data
{
    public class ResultsetView: IBindingList, ITypedList, IDisposable
    {
        private Resultset _source;
        private List<Row> _bindingList;
        private object _syncRoot;
        private ListChangedEventHandler _onListChanged;
        private PropertyDescriptorCollection _props;
        private bool _disposed = false;

        public ResultsetView(Resultset source)
        {
            _source = source;
            _bindingList = new List<Row>();
            _syncRoot = new Object();
            
            while (source.Begin != null)
                _bindingList.Add(source.Dequeue());            
        }

        ~ResultsetView()
        {
            Dispose(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                Clear();
            _disposed = true;
        }

        protected virtual void OnListChanged(ListChangedEventArgs ev)
        {
            if (_onListChanged != null)
                _onListChanged(this, ev);
        }

        #region IBindingList Members

        public object AddNew()
        {
            Row row = _source.NewRow();
            Add(row);
            return row;
        }

        public bool AllowEdit
        {
            get { return true; }
        }

        public bool AllowNew
        {
            get { return true; }
        }

        public bool AllowRemove
        {
            get { return true; }
        }

        public event ListChangedEventHandler ListChanged
        {
            add
            {
                _onListChanged += value;
            }
            remove
            {
                _onListChanged -= value;
            }
        }

        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        public bool SupportsSearching
        {
            get { return false; }
        }

        public bool SupportsSorting
        {
            get { return false; }
        }

        #region Unsupported fetures
        
        public bool IsSorted
        {
            get { throw new NotImplementedException(); }
        }

        public ListSortDirection SortDirection
        {
            get { throw new NotImplementedException(); }
        }

        public PropertyDescriptor SortProperty
        {
            get { throw new NotImplementedException(); }
        }

        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotImplementedException();
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion

        #region IList Members

        public int Add(object value)
        {
            _bindingList.Add((Row)value);
            int index = _bindingList.Count - 1;
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
            return index;
        }

        public void Clear()
        {
            _props = null;
            _bindingList.Clear();            
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public bool Contains(object value)
        {
            return _bindingList.Contains((Row)value);
        }

        public int IndexOf(object value)
        {
            return _bindingList.IndexOf((Row)value);
        }

        public void Insert(int index, object value)
        {
            Row row = (Row)value;
            _bindingList.Insert(index, row);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            int index = _bindingList.IndexOf((Row)value);
            if (index == -1)
                throw new ArgumentException();
            RemoveAt(index);
        }

        public void RemoveAt(int index)
        {            
            Row row = _bindingList[index];
            _bindingList.RemoveAt(index);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }

        public object this[int index]
        {
            get
            {
                return _bindingList[index];
            }
            set
            {
                Row oldValue = _bindingList[index];
                _bindingList[index] = (Row)value;                
                if (oldValue != value)
                    OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            int length = array.Length - index;
            for (int i = 0; i < length && i < _bindingList.Count; i++)
                array.SetValue(_bindingList[i], index + i);
        }

        public int Count
        {
            get { return _bindingList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _bindingList.GetEnumerator();
        }

        #endregion

        #region ITypedList Members

        protected class RowPropertyDescriptor : PropertyDescriptor
        {
            private Resultset _rs;            
            private int _column;

            public RowPropertyDescriptor(Resultset rs, int column)
                : base(rs.RowType.Fields[column].Name, null)
            {
                _rs = rs;
                _column = column;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override Type ComponentType
            {
                get { return typeof(Row); }
            }

            public override object GetValue(object component)
            {
                Row row = (Row)component;
                if (row.IsDbNull(_column))
                    return null;
                else
                    return row.GetValue(_column);
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type PropertyType
            {
                get { return _rs.RowType.Fields[_column].DataType; }
            }

            public override void ResetValue(object component)
            {
                ((Row)component).SetDbNull(_column);
            }

            public override void SetValue(object component, object value)
            {
                Row row = (Row)component;
                if (value == null)
                    row.SetDbNull(_column);
                else
                    row.SetValue(_column, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (_props == null)
            {
                List<PropertyDescriptor> list = new List<PropertyDescriptor>();
                for (int k = 0; k < _source.RowType.Fields.Length; k++)
                    list.Add(new RowPropertyDescriptor(_source, k));
                _props = new PropertyDescriptorCollection(list.ToArray());
            }
            return _props;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return typeof(Row).Name;
        }

        #endregion
    }
}
