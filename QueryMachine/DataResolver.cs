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

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{   
    /// <summary>
    /// Resolve column names 
    /// </summary>
    public class DataResolver: Resolver 
    {
        protected Binder _binder;
        protected Dictionary<Object, ColumnBinding> _bindings_t;

        public Resolver NewScope()
        {
            return this;
        }

        public void Init(MemoryPool pool)
        {
            return;
        }

        public ColumnBinding[] Bindings
        {
            get
            {
                return _binder.Bindings;
            }
        }

        public DataResolver(Binder binder)
        {
            _binder = binder;
            _bindings_t = new Dictionary<Object, ColumnBinding>();
        }

        public ColumnBinding GetBinding(object atom)
        {
            ColumnBinding b;
            if (!_bindings_t.TryGetValue(atom, out b))
            {
                ATOM a = (ATOM)atom;
                if (a.parts.Length == 1)
                    b = _binder.Get(null, a.parts[0]);
                else
                    b = _binder.Get(a.parts[0], a.parts[1]);
                if (b != null)                    
                    _bindings_t.Add(atom, b);
            }
            return b;
        }

        public virtual bool Get(object atom, out SymbolLink result)
        {
            ColumnBinding b = GetBinding(atom);
            if (b == null)
            {
                result = null;
                return false;
            }
            result = b.data;
            return true;                
        }
    }

}
