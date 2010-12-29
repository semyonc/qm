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

namespace DataEngine
{
    // Internal translation expression that contains expression as po.item to po.node.item
    //   if intermediate container dataset are proceeded
    class ExpressionTransformer
    {
        public bool NeedTransform { get; set; }

        private List<ColumnBinding> _bindings;

        public ExpressionTransformer(List<ColumnBinding> bindings)
        {
            _bindings = bindings;
            NeedTransform = false;
        }

        private bool IsContainerQualifier(string qualifer)
        {
            foreach (ColumnBinding b in _bindings)
                if (b.TableName.Equals(qualifer) && b.container)
                    return true;
            return false;
        }

        private object Subst(object expr, object parent_f)
        {
            if (Lisp.IsAtom(expr))
            {
                ATOM name = (ATOM)expr;
                if (name.parts.Length == 2)
                {
                    if (IsContainerQualifier(name.parts[0]) &&
                        !(name.parts[1].Equals("node") || name.parts[1].Equals("name")))
                    {
                        object res = Lisp.List(ID.Dref, ATOM.Create(null, new string[] { 
                            name.parts[0], "node" }, false), name.parts[1]);
                        if (parent_f != ID.Dref)
                            return Lisp.List(ID.Rval, res);
                        else
                            return res;
                    }
                }
            }
            else if (Lisp.IsFunctor(expr))
            {
                Object[] f = Lisp.ToArray(expr);
                for (int k = 1; k < f.Length; k++)
                    f[k] = Subst(f[k], f[0]);
                return Lisp.List(f);
            }
            return expr;
        }

        public object Process(object expr)
        {
            return Subst(expr, null);
        }
    }
}
