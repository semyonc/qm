/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2011  Semyon A. Chertkov (semyonc@gmail.com)

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

namespace DataEngine.CoreServices.Data
{
    public class ValueTuple
    {
        public ValueTuple(String name)
        {
            Name = name;
            Values = new Dictionary<String, Object>();
        }

        public ValueTuple(String name, IDictionary values)
        {
            Name = name;
            Values = values;
        }

        public String Name { get; private set; }

        public IDictionary Values { get; private set; }

        public bool Empty
        {
            get
            {
                return Values.Count == 0;
            }
        }
    }
}
