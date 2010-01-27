//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using DataEngine.CoreServices.Generation;

namespace DataEngine.CoreServices
{    
    public class LocalAccess: IEnumerable<LocalAccess.LocalBinding>
    {
        public struct LocalBinding
        {
            public object Id;
            public LocalBuilder Local;
            public SymbolLink Link;

            public LocalBinding(object id, LocalBuilder local, SymbolLink link)
            {
                Id = id;
                Local = local;
                Link = link;
            }
        }

        private struct ScopeLocal
        {
            public object ID;
            public LocalBuilder Local;

            public ScopeLocal(object id, LocalBuilder local)
            {
                ID = id;
                Local = local;
            }
        }

        ILGen il;
        Dictionary<object, LocalBuilder> locals = new Dictionary<object, LocalBuilder>();
        List<LocalBinding> values = new List<LocalBinding>();
        List<Object> consts = new List<object>();
        List<LambdaExpr> dependence = new List<LambdaExpr>();

        private class Scope
        {
            public object form;
            public bool active;
            public Dictionary<object, LocalBuilder> locals;

            public Scope(object form, bool active, Dictionary<object, LocalBuilder> locals)
            {
                this.form = form;
                this.active = active;
                this.locals = locals;
            }
        }

        private class ParameterBinding
        {
            public object id;
            public Type type;
            public bool binded;
        }

        Dictionary<object, ParameterBinding> bindings = new Dictionary<object, ParameterBinding>();
        Stack<Scope> scope = new Stack<Scope>();
        Dictionary<object, Dictionary<object, LocalBuilder>> scope_locals = new Dictionary<object, Dictionary<object, LocalBuilder>>();
        Dictionary<object, LocalBuilder> current_scope = null;
        bool scope_active = false;

        public Executive.Parameter[] Parameters { get; private set; }

        public LocalAccess(ILGen il, Executive.Parameter[] parameters)
        {
            this.il = il;
            Parameters = parameters;
        }

        public void DeclareScope(object lval)
        {
            Dictionary<object, LocalBuilder> scope = new Dictionary<object, LocalBuilder>();
            scope_locals.Add(lval, scope);
        }

        public void EnterScope(object lval, bool active)
        {
            current_scope = scope_locals[lval];
            scope_active = active;
            scope.Push(new Scope(lval, active, current_scope));
        }

        public void ActivateScope()
        {
            scope.Peek().active =
                scope_active = true;
        }

        public void LeaveScope()
        {
            if (current_scope == null)
                throw new InvalidOperationException();
            scope.Pop();
            if (scope.Count > 0)
            {
                Scope sc = scope.Peek();
                current_scope = sc.locals;
                scope_active = sc.active;
            }
            else
            {
                current_scope = null;
                scope_active = false;
            }
        }

        public void DestroyScope()
        {
            if (current_scope == null)
                throw new InvalidOperationException();
            foreach (LocalBuilder localvar in current_scope.Values)
                if (localvar != null)
                    il.FreeLocal(localvar);
            LeaveScope();
        }

        public void DeclareLocal(object atom)
        {
            LocalBuilder localVar;
            if (current_scope == null)
                throw new InvalidOperationException();
            if (current_scope.TryGetValue(atom, out localVar))
                throw new ArgumentException("Local is redeclarated in the current scope", "atom");
            current_scope.Add(atom, null);
        }

        public LocalBuilder BindLocal(object atom, Type type)
        {
            LocalBuilder localVar;
            if (current_scope == null)
                throw new InvalidOperationException();
            if (!current_scope.TryGetValue(atom, out localVar))
                throw new ArgumentException("Local is undeclared in the current scope", "atom");
            if (localVar != null)
                throw new ArgumentException("Local is already binded in the current scope", "atom");
            localVar = il.DeclareLocal(type);
            current_scope[atom] = localVar;
            return localVar;
        }

        public void Bind(Executive engine, object atom)
        {
            LocalBuilder localVar;
            if (current_scope != null)
            {
                Scope[] scs = scope.ToArray();
                foreach (Scope sc in scs)
                    if (sc.active)
                    {
                        if (sc.locals.TryGetValue(atom, out localVar))
                            return;
                    }
            }
            if (!locals.TryGetValue(atom, out localVar))
            {
                SymbolLink link = engine.TryGet(atom, false, true);
                if (link == null)
                {
                    ParameterBinding b;
                    if (bindings.TryGetValue(atom, out b))
                    {
                        localVar = il.DeclareLocal(b.type);
                        locals.Add(atom, localVar);
                        b.binded = true;
                        return;
                    }
                    else
                    {
                        link = engine.TryGet(atom, true, false);
                        if (link == null)
                            throw new ValueNotDefined(atom.ToString());
                    }
                }
                localVar = il.DeclareLocal(link.Type);
                locals.Add(atom, localVar);
                values.Add(new LocalBinding(atom, localVar, link));
            }
        }

        public void BindParameter(object atom, Type type)
        {
            ParameterBinding b = new ParameterBinding();
            b.id = atom;
            b.type = type;
            b.binded = false;
            bindings.Add(atom, b);
        }

        public bool IsParameterBinded(object atom)
        {
            return bindings[atom].binded;
        }

        public SymbolLink[] GetValues()
        {
            SymbolLink[] arr = new SymbolLink[values.Count];
            for (int k = 0; k < values.Count; k++)
                arr[k] = values[k].Link;
            return arr;
        }

        public Object[] GetValuesID()
        {
            object[] arr = new object[values.Count];
            for (int k = 0; k < values.Count; k++)
                arr[k] = values[k].Id;
            return arr;
        }

        public LocalBuilder GetLocal(object atom)
        {
            LocalBuilder localVar;
            if (current_scope != null)
            {
                Scope[] scs = scope.ToArray();
                foreach (Scope sc in scs)
                    if (sc.active)
                    {
                        if (sc.locals.TryGetValue(atom, out localVar))
                            return localVar;
                    }
            }
            if (locals.TryGetValue(atom, out localVar))
                return localVar;
            else
                return null;
        }

        public int DefineConstant(object tag)
        {
            for (int k = 0; k < consts.Count; k++)
                if (tag == consts[k])
                    return k;
            consts.Add(tag);
            return consts.Count - 1;
        }

        public void AddDependence(LambdaExpr expr)
        {
            if (dependence.IndexOf(expr) == -1)
                dependence.Add(expr);
        }

        public Object[] GetConsts()
        {
            if (consts.Count == 0)
                return null;
            else
                return consts.ToArray();
        }

        public LambdaExpr[] GetDependences()
        {
            return dependence.ToArray();
        }

        public bool HasLocals
        {
            get
            {
                return values.Count > 0;
            }
        }


        #region IEnumerable<LocalBinding> Members

        public IEnumerator<LocalAccess.LocalBinding> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }

        #endregion
    }
}
