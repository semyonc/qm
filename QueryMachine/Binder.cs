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

using DataEngine.CoreServices.Data;

namespace DataEngine.CoreServices
{
    public class ColumnBinding
    {
        public int rnum;
        public int snum;
        public TypeCode typecode;
        public String TableName;
        public String Name;
        public RowType.TypeInfo fieldType;
        public SymbolLink data;
        public bool ambiguous;
        public bool natural;
        public bool container;
        public bool caseSensitive;

        public void Set(Row row, Executive exec)
        {
            if (!data.IsBinded)
                exec.DefaultPool.Bind(data);
            // We are not expecting DBNull and address to itemArray directly 
            exec.DefaultPool.SetData(data, DBNull.Value);
            Row nested_row = (Row)row.ItemArray[rnum];
            if (nested_row != null)
            {
                object value = nested_row.ItemArray[fieldType.Ordinal];
                if (value != null)
                    exec.DefaultPool.SetData(data, value);
            }
        }
    }

    public class Binder
    {
        private ColumnBinding[] _columnBinding;

        public Binder(List<ColumnBinding> columnBinding)
            : this(columnBinding.ToArray())
        {
        }

        public Binder(ColumnBinding[] columnBinding)
        {
            _columnBinding = columnBinding;
            FlagAmbiguos();
        }

        public Binder(Resultset rs)
            : this(rs.RowType.Fields)
        {
        }

        public Binder(RowType.TypeInfo[] fields)
        {
            List<ColumnBinding> list = new List<ColumnBinding>();
            foreach (RowType.TypeInfo ti in fields)
                if (ti.NestedType == null)
                {
                    if (!ti.IsHidden)
                    {
                        ColumnBinding b = new ColumnBinding();
                        b.Name = ti.Name;
                        b.rnum = ti.Ordinal;
                        b.snum = -1;
                        b.fieldType = ti;
                        b.caseSensitive = ti.IsCaseSensitive;
                        list.Add(b);
                    }
                }
                else
                    foreach (RowType.TypeInfo ti_det in ti.NestedType.Fields)
                    {
                        if (!ti.IsHidden)
                        {
                            ColumnBinding b = new ColumnBinding();
                            b.TableName = ti.Name;
                            b.Name = ti_det.Name;
                            b.rnum = ti.Ordinal;
                            b.snum = ti_det.Ordinal;
                            b.caseSensitive = ti_det.IsCaseSensitive;
                            b.fieldType = ti_det;
                            list.Add(b);
                        }
                    }
            _columnBinding = list.ToArray();
            FlagAmbiguos();
        }
       
        public ColumnBinding Get(string tableName, string name, bool caseSensitive = true)
        {
            if (String.IsNullOrEmpty(tableName))
                foreach (ColumnBinding b in _columnBinding)
                {
                    if (Util.EqualsName(name, b.Name, b.caseSensitive && caseSensitive))
                        if (b.ambiguous)
                            throw new ESQLException(Properties.Resources.ColumnAmbiguoslyDefined, b.Name);
                        else
                            return b;
                }
            else
                foreach (ColumnBinding b in _columnBinding)
                    if (Util.EqualsName(tableName, b.TableName, false) &&
                        Util.EqualsName(name, b.Name, b.caseSensitive && caseSensitive))
                        return b;
            return null;
        }

        public ColumnBinding Get(string qualifiedName, bool caseSensitive = true)
        {
            String[] parts = Util.SplitName(qualifiedName);
            if (parts.Length == 1)
                return Get(null, parts[0], caseSensitive);
            else
                return Get(parts[0], parts[1], caseSensitive);
        }

        public bool GetLocator(string qualifiedName, ref RowType.Locator loc)
        {
            ColumnBinding b = Get(qualifiedName);
            if (b != null)
            {
                loc.master = b.rnum;
                if (b.snum != -1)
                    loc.detail = b.snum;
                return true;
            }
            else
                return false;           
        }

        public RowType.Locator GetLocator(string name)
        {
            RowType.Locator locator = new RowType.Locator();
            if (GetLocator(name, ref locator))
                return locator;
            else
                throw new ESQLException(Properties.Resources.InvalidIdentifier, name);
        }

        public RowType.Locator GetLocator(int ordinal)
        {
            foreach (ColumnBinding b in _columnBinding)
            {
                if (ordinal-- == 0)
                {
                    RowType.Locator loc = new RowType.Locator();
                    loc.master = b.rnum;
                    if (b.snum != -1)
                        loc.detail = b.snum;
                    return loc;
                }
            }
            throw new ESQLException(Properties.Resources.BadOrderByItem, ordinal);
        }

        public String GetName(RowType.Locator loc)
        {
            foreach (ColumnBinding b in _columnBinding)
                if (loc.master == b.rnum && (loc.detail ?? -1) == b.snum)
                {
                    if (b.TableName != null)
                        return String.Format("{0}.{1}", b.TableName, b.Name);
                    else
                        return b.Name;
                }
            return null;
        }
                
        private void FlagAmbiguos()
        {
            foreach (ColumnBinding b1 in _columnBinding)
                foreach (ColumnBinding b2 in _columnBinding)
                    if (b1 != b2 && String.Compare(b1.Name, b2.Name,
                        !(b1.caseSensitive || b2.caseSensitive)) == 0)
                    {
                        b1.ambiguous = b2.ambiguous = true;
                        b1.caseSensitive = b2.caseSensitive =
                            b1.caseSensitive || b2.caseSensitive;
                    }
        }

        public ColumnBinding[] Bindings
        {
            get
            {
                return _columnBinding;
            }
        }

        public static void GetParameterBindings(object lval, List<int> bindings)
        {
            if (!Lisp.IsNode(lval))
            {
                object head = Lisp.Car(lval);
                if (Lisp.IsCons(head))
                    foreach (object o in Lisp.getIterator(lval))
                        GetParameterBindings(o, bindings);
                else
                {
                    object[] args = Lisp.ToArray(Lisp.Cdr(lval));
                    if (head.Equals(ID.ParamRef))
                        bindings.Add((int)args[0] - 1);
                    else if (head.Equals(ID.Like1))
                    {
                        GetParameterBindings(args[0], bindings);
                        GetParameterBindings(args[1], bindings);
                        if (args.Length > 2)
                            GetParameterBindings(args[2], bindings);
                    }
                    else if (head.Equals(ID.Member) || head.Equals(Funcs.Div) ||
                        head.Equals(Funcs.And) || head.Equals(Funcs.Or) || head.Equals(ID.LT) ||
                        head.Equals(ID.GT) || head.Equals(ID.EQ) || head.Equals(ID.NE) ||
                        head.Equals(ID.LE) || head.Equals(ID.GE) || head.Equals(ID.Concat) ||
                        head.Equals(Funcs.Add) || head.Equals(Funcs.Sub) || head.Equals(Funcs.Mul))
                    {
                        GetParameterBindings(args[0], bindings);
                        GetParameterBindings(args[1], bindings);
                    }
                    else if (head.Equals(ID.Between))
                    {
                        GetParameterBindings(args[0], bindings);
                        GetParameterBindings(args[1], bindings);
                        GetParameterBindings(args[3], bindings);
                    }
                    else if (head.Equals(Funcs.List))
                        foreach (object arg in args)
                            GetParameterBindings(args[0], bindings);
                    else if (head.Equals(Funcs.Not) || head.Equals(ID.IsNull))
                        GetParameterBindings(args[0], bindings);
                    else
                        throw new UnproperlyFormatedExpr(Lisp.Format(lval));
                }
            }
        }
    }
}
