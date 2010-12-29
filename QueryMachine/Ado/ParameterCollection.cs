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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace DataEngine.ADO
{
    public delegate void ChangeHandler(Object sender, EventArgs args);

    public class ParameterCollection: DbParameterCollection
    {
        private Object _syncRoot = new Object();
        private List<Parameter> _params = new List<Parameter>();

        public override int Add(object value)
        {
            Parameter parameter = (Parameter)value;

            if (parameter.ParameterName == null)
			{
				String name;
				int k = 1;
				do
				{
					name = String.Format("{0}{1}", parameter.ParameterName, k++);
				} while (IndexOf(name) != -1);
				parameter.ParameterName = name;	
			}

			int index = _params.Count;
			_params.Add(parameter);
            Changed(this, EventArgs.Empty);			
			
            return index;
        }

        public override void AddRange(Array values)
        {
            _params.AddRange(System.Linq.Enumerable.Cast<Parameter>(values));
        }

        public void AddValue(string parameterName, Object value)
        {
            Parameter parameter = new Parameter(parameterName, value);
            Add(parameter);
        }

        public override void Clear()
        {
            _params.Clear();
            Changed(this, EventArgs.Empty);
        }

        public override bool Contains(string parameterName)
        {
            return IndexOf(parameterName) != -1;
        }

        public override bool Contains(object value)
        {
            return _params.Contains((Parameter)value);
        }

        public override void CopyTo(Array array, int index)
        {
            Parameter[] arr = _params.ToArray();
            arr.CopyTo(array, index);
        }

        public override int Count
        {
            get { return _params.Count; }
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return _params.GetEnumerator();
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index == -1)
                throw new ArgumentException(parameterName);
            return _params[index];
        }

        protected override DbParameter GetParameter(int index)
        {
            return _params[index];
        }

        public override int IndexOf(string parameterName)
        {
            for (int i = 0; i < _params.Count; i++)
            {
                if (_cultureAwareCompare(_params[i].ParameterName, parameterName) == 0)
                    return i;
            }
            return -1;
        }

        public override int IndexOf(object value)
        {
            return IndexOf(((Parameter)value).ParameterName);
        }

        public override void Insert(int index, object value)
        {
            _params.Insert(index, (Parameter)value);
            Changed(this, EventArgs.Empty);
        }

        public override bool IsFixedSize
        {
            get { return false; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSynchronized
        {
            get { return false; }
        }

        public override void Remove(object value)
        {
            _params.Remove((Parameter)value);
            Changed(this, EventArgs.Empty);
        }

        public override void RemoveAt(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index == -1)
                throw new ArithmeticException(parameterName);
            _params.RemoveAt(index);
            Changed(this, EventArgs.Empty);
        }

        public override void RemoveAt(int index)
        {
            _params.RemoveAt(index);
            Changed(this, EventArgs.Empty);
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            int index = IndexOf(parameterName);
            if (index == -1)
                throw new ArgumentException(parameterName);
            SetParameter(index, value);
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            _params[index] = (Parameter)value;
        }

        public override object SyncRoot
        {
            get { return _syncRoot; }
        }

        public event ChangeHandler OnChanged; 

        private void Changed(ParameterCollection sender, EventArgs args)
        {
            if (OnChanged != null)
                OnChanged(this, args);
        }

        private int _cultureAwareCompare(string name, string parameterName)
        {
            return CultureInfo.CurrentCulture.CompareInfo.Compare(name, parameterName,
                CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);
        }        
    }
}
