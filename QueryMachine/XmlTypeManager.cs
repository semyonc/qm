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
using System.Xml;
using System.Reflection;

namespace DataEngine
{
    public class XmlTypeManager
    {
        private Dictionary<String, Type> _map = new Dictionary<String, Type>();

        public void Add(String Name, String NamespaceUri, String typeName)
        {
            Add(Name, NamespaceUri, Type.GetType(typeName));
        }

        public void Add(String Name, String NamespaceUri, Type type)
        {
            if (_map.ContainsKey(Name))
                _map.Remove(Name);
            _map.Add(Name, type);
        }

        public Type GetType(XmlNode node)
        {
            Type res;
            if (_map.TryGetValue(node.Name, out res))
                return res;
            else
                return null;
        }
    }
}
