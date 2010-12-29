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
using System.IO;

using DataEngine.CoreServices;

namespace DataEngine
{
    public class Notation
    {
        public class Record
        {
            public Symbol sym;
            public Descriptor descriptor;
            public object[] args;

            public Symbol Arg0
            {
                get
                {
                    return (Symbol)args[0];
                }
            }

            public Symbol Arg1
            {
                get
                {
                    return (Symbol)args[1];
                }

            }

            public Symbol Arg2
            {
                get
                {
                    return (Symbol)args[2];
                }
            }

            public Symbol Arg3
            {
                get
                {
                    return (Symbol)args[3];
                }
            }

#if DEBUG
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}({1}", descriptor.ToString(), sym);
                if (args != null)
                {
                    for (int k = 0; k < args.Length; k++)
                    {
                        sb.Append(", ");
                        sb.Append(Lisp.Format(args[k]));
                    }
                }
                sb.Append(")");
                return sb.ToString();
            }
#endif
        }

        public class NotationComparer : IComparer<Record>
        {
            public int Compare(Record x, Record y)
            {
                if (x.descriptor < y.descriptor)
                    return -1;
                else if (x.descriptor > y.descriptor)
                    return 1;
                else
                {
                    if (x.args.Length < y.args.Length)
                        return -1;
                    else if (x.args.Length > y.args.Length)
                        return 1;
                    else
                        return 0;
                }
            }
        }
        
        private Stack<Dictionary<Tag, Symbol>> symtab;
        private List<Record> dtab;

        public Notation()
        {
            symtab = new Stack<Dictionary<Tag, Symbol>>();
            symtab.Push(new Dictionary<Tag, Symbol>());
            dtab = new List<Record>();
        }

        public void Clear()
        {
            symtab.Clear();
            symtab.Push(new Dictionary<Tag, Symbol>());
            
            dtab.Clear();
#if DEBUG
            Symbol.itab.Clear();
#endif
        }

        public Symbol GetSymbol(Tag tag)
        {
            Symbol sym;            
            if (symtab.Peek().TryGetValue(tag, out sym))
                return sym;
            sym = new Symbol(tag);
            symtab.Peek()[tag] = sym;            
            return sym;
        }

        public void EnterContext()
        {
            symtab.Push(new Dictionary<Tag, Symbol>());
        }

        public Symbol ResolveTag(Tag tag)
        {
            Symbol sym = GetSymbol(tag);
            symtab.Peek().Remove(tag);
            return sym;
        }

        public void LeaveContext()
        {
            symtab.Pop();
        }

        public void ConfirmTag(Tag tag, Descriptor desc, params object[] args)
        {
            Confirm(GetSymbol(tag), desc, args);
        }

        public Symbol Confirm(Symbol sym, Descriptor desc, params object[] args)
        {
            Record item = new Record();
            item.sym = sym;
            item.descriptor = desc;
            if (args != null)
                item.args = args;
            else
                item.args = new object[0];
            dtab.Add(item);
            return sym;
        }

        public Record[] Select()
        {
            return dtab.ToArray();
        }

        public Record[] Select(Symbol sym)
        {
            List<Record> result = new List<Record>();
            for (int i = 0; i < dtab.Count; i++)
                if (dtab[i].sym == sym)
                    result.Add(dtab[i]);
            return result.ToArray();
        }

        public Record[] Select(Symbol sym, Descriptor desc, int arity)
        {
            List<Record> result = new List<Record>();
            for (int i = 0; i < dtab.Count; i++)
                if (((sym == null) || dtab[i].sym == sym) &&
                    dtab[i].descriptor == desc && dtab[i].args.Length == arity)
                    result.Add(dtab[i]);            
            return result.ToArray();
        }

        public Record[] Select(Descriptor desc, int arity)
        {
            return Select(null, desc, arity);
        }

        public Record[] Select(Symbol sym, int arity)
        {
            List<Record> result = new List<Record>();
            for (int i = 0; i < dtab.Count; i++)
                if (dtab[i].sym == sym && dtab[i].args.Length == arity)
                    result.Add(dtab[i]);
            return result.ToArray();
        }

        public Record[] Select(Symbol sym, Descriptor[] descs)
        {
            List<Record> result = new List<Record>();
            for (int i = 0; i < dtab.Count; i++)
                if (dtab[i].sym == sym)
                    for (int k = 0; k < descs.Length; k++)
                        if (dtab[i].descriptor == descs[k])
                        {
                            result.Add(dtab[i]);
                            break;
                        }
            return result.ToArray();
        }

        public Record[] Select(Symbol sym, Descriptor[] descs, int arity)
        {
            List<Record> result = new List<Record>();
            for (int i = 0; i < dtab.Count; i++)
                if (dtab[i].sym == sym && dtab[i].args.Length == arity)
                    for (int k = 0; k < descs.Length; k++)
                        if (dtab[i].descriptor == descs[k])
                        {
                            result.Add(dtab[i]);
                            break;
                        }
            return result.ToArray();
        }

        public Record[] Select(Symbol sym, Descriptor desc, int arity, object[] args)
        {
            List<Record> result = new List<Record>();
            for (int i = 0; i < dtab.Count; i++)
                if (((sym == null) || dtab[i].sym == sym) &&
                    dtab[i].descriptor == desc && dtab[i].args.Length == arity)
                {
                    bool found = true;
                    for(int k = 0; k < args.Length && k < arity; k++)
                        if (dtab[i].args[k] != args[k])
                        {
                            found = false;
                            break;
                        }
                    if (found)
                        result.Add(dtab[i]);
                }
            return result.ToArray();
        }

        public bool Flag(Symbol sym, Descriptor desc)
        {
            Record[] result = Select(sym, desc, 0);
            return result.Length > 0;
        }

        public bool Flag(Symbol sym, Descriptor desc, object[] args)
        {
            Record[] result = Select(sym, desc, args.Length);
            foreach (Record r in result)
            {
                bool flag = true;
                for (int k = 0; k < args.Length; k++)
                    if (r.args[k] != args[k])
                    {
                        flag = false;
                        break;
                    }
                if (flag)
                    return true;
            }
            return false;
        }

        public delegate bool Scanner(Record rec);

        public void Scan(Symbol sym, Scanner scanner)
        {
            for (int i = 0; i < dtab.Count; i++)
                if (dtab[i].sym == sym)
                    Scan(dtab[i], scanner);
        }

        public void Scan(Record rec, Scanner scanner)
        {
            if (scanner(rec))
                foreach (object arg in rec.args)
                {
                    if (Lisp.IsNode(arg))
                    {
                        if (arg is Symbol)
                            Scan((Symbol)arg, scanner);
                    }
                    else
                        foreach (object item in Lisp.getIterator(arg))
                        {
                            if (item is Symbol)
                                Scan((Symbol)item, scanner);
                        }
                }
        }

        public Record[] ConnectivitySelect(Record rec)
        {
            List<Record> result = new List<Record>();
            Scan(rec, delegate(Record rc)
            {
                result.Add(rc);
                return true;
            });
            return result.ToArray() ;
        }

        public object Clone(Dictionary<Symbol, Symbol> map, object arg)
        {
            if (Lisp.IsNode(arg))
            {
                if (arg is Symbol)
                {
                    Symbol dest;
                    Symbol src = (Symbol)arg;
                    if (!map.TryGetValue(src, out dest))
                    {
                        dest = (Symbol)src.Clone();
                        map.Add(src, dest);
                    }
                    return dest;
                }
                else
                    return arg;
            }
            else
            {
                object result = null;
                for (object tail = null; arg != null; arg = Lisp.Cdr(arg))
                    if (tail == null)
                    {
                        result = Lisp.Cons(Clone(map, Lisp.Car(arg)));
                        tail = result;
                    }
                    else
                    {
                        Lisp.Rplacd(tail, Lisp.Cons(Clone(map, Lisp.Car(arg))));
                        tail = Lisp.Cdr(tail);
                    }
                return result;
            }
        }

        public Record[] Clone(Dictionary<Symbol, Symbol> map, Record[] recs)
        {            
            List<Record> result = new List<Record>();
            foreach (Record rec in recs)
            {
                Record rec1 = new Record();
                rec1.sym = (Symbol)Clone(map, rec.sym);
                rec1.descriptor = rec.descriptor;
                rec1.args = new object[rec.args.Length];
                for (int k = 0; k < rec1.args.Length; k++)
                    rec1.args[k] = Clone(map, rec.args[k]);
                result.Add(rec1);
            }
            return result.ToArray();
        }

        public void Pack()
        {
            HashSet<Symbol> hs = new HashSet<Symbol>();
            Record[] recs = Select(Descriptor.Root, 1);
            hs.Add(recs[0].sym);
            Scan(recs[0].sym,
                delegate(Record rec)
            {
                if (!hs.Contains(rec.sym))
                    hs.Add(rec.sym);
                return true;
            });
            for (int k = dtab.Count - 1; k >= 0; k--)
                if (!hs.Contains(dtab[k].sym))
                    dtab.RemoveAt(k);
        }

        public void Replace(Symbol sym1, Symbol sym2)
        {
            for (int i = 0; i < dtab.Count; i++)
                for (int k = 0; k < dtab[i].args.Length; k++)
                    dtab[i].args[k] = Lisp.Replace(sym1, sym2, dtab[i].args[k]);
        }

        public void Replace(Record rec1, Record rec2)
        {
            dtab[dtab.IndexOf(rec1)] = rec2;
        }

        public void Confirm(Record[] recs)
        {
            dtab.AddRange(recs);
        }

        public void Remove(Record[] recs)
        {
            foreach (Record rec in recs)
                dtab.Remove(rec);
        }

        public bool ConnectivityEquals(Symbol sym1, Symbol sym2)
        {
            if (sym1 == sym2)
                return true;

            if (sym1 == null || sym2 == null)
                return false;

            if (sym1 is Value || sym2 is Value)
            {
                if (sym1 is Value && sym2 is Value)
                    return sym1.Equals(sym2);
                else
                    return false;
            }

            Record[] recs1 = Select(sym1);
            Record[] recs2 = Select(sym2);

            if (recs1.Length == recs2.Length)
            {
                Array.Sort<Record>(recs1, new NotationComparer());
                Array.Sort<Record>(recs2, new NotationComparer());
                
                for (int k = 0; k < recs1.Length; k++)
                    if (recs1[k].descriptor == recs2[k].descriptor &&
                        recs1[k].args.Length == recs2[k].args.Length)
                        for (int s = 0; s < recs1[k].args.Length; s++)
                        {
                            object o1 = recs1[k].args[s];
                            object o2 = recs2[k].args[s];
                            if (o1 is Symbol || o2 is Symbol)
                            {
                                if (o1 is Symbol && o2 is Symbol)
                                {
                                    if (!ConnectivityEquals((Symbol)o1, (Symbol)o2))
                                        return false;
                                }
                                else
                                    return false;
                            }
                            else
                                if (!o1.Equals(o2))
                                    return false;
                        }
                    else
                        return false;                
            }
            else
                return false;

            return true;
        }

        public bool IsMemberOf(List<Symbol> list, Symbol sym)
        {
            foreach (Symbol s in list)
                if (ConnectivityEquals(s, sym))
                    return true;
            return false;
        }

#if DEBUG
        public void Dump(TextWriter tw)
        {
            for (int i = 0; i < dtab.Count; i++)
            {
                if (i > 0)
                    tw.WriteLine();
                tw.Write(dtab[i].ToString());
            }
        }
#endif
    }
}
