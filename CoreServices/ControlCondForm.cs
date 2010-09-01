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
using System.Reflection;
using System.Reflection.Emit;

using DataEngine.CoreServices.Generation;

namespace DataEngine.CoreServices
{
    internal class ControlCondForm: ControlFormBase
    {
        private MethodInfo _raiseError;

        public ControlCondForm()
        {
            ID = Funcs.Cond;
            Type type = GetType();
            _raiseError = type.GetMethod("CondRaiseError", BindingFlags.Static | BindingFlags.Public);
        }

        public static void CondRaiseError()
        {
            throw new Exception("Error in COND: No expression returned");
        }

        public override object MacroExpand(Executive engine, object[] lval, ref bool proceed)
        {
            object[] args = new object[lval.Length];
            for (int k = 0; k < args.Length; k++)
            {
                args[k] = Lisp.List(engine.MacroExpand(Lisp.First(lval[k]), ref proceed),
                    engine.MacroExpand(Lisp.Second(lval[k]), ref proceed));
            }
            return Lisp.Functor(ID, args);
        }

        private class Branch
        {
            public Type type;
            public LocalBuilder localVar;
            public Label handler;
        }

        private Branch GetBranch(List<Branch> branches, ILGen il, Type type)
        {
            foreach (Branch b in branches)
                if (b.type == type)
                    return b;
            Branch branch = new Branch();
            branch.type = type;
            branch.localVar = il.DeclareLocal(type);
            branch.handler = il.DefineLabel();
            branches.Add(branch);
            return branch;
        }

        public override void Compile(Executive engine, object form, ILGen il, LocalAccess locals, Stack<Type> st, object[] args)
        {
            Type resType = null;
            List<Branch> branches = new List<Branch>();
            for (int k = 0; k < args.Length; k++)
            {
                Label next = il.DefineLabel();
                object cur = args[k];
                if (!Lisp.IsCons(cur))
                    throw new ArgumentException("Unproperly formated expression");
                object[] pair = Lisp.ToArray(cur);
                if (pair.Length != 2)
                    throw new ArgumentException("Unproperly formated expression");
                engine.CompileExpr(il, locals, st, pair[0]);
                if (st.Pop() != typeof(System.Object))
                    throw new ArgumentException("Expecting boolean value");
                il.Emit(OpCodes.Brfalse, next);
                engine.CompileExpr(il, locals, st, pair[1]);
                Type type = st.Pop();
                Branch branch = GetBranch(branches, il, type);
                il.Emit(OpCodes.Stloc, branch.localVar);
                il.Emit(OpCodes.Br, branch.handler);
                il.MarkLabel(next);
                if (resType == null)
                    resType = type;
                else
                    if (resType != type)
                        resType = typeof(System.Object);
            }
            il.EmitCall(_raiseError);
            Label end = il.DefineLabel();
            for (int k = 0; k < branches.Count; k++)
            {
                Branch b = branches[k];
                il.MarkLabel(b.handler);
                il.Emit(OpCodes.Ldloc, b.localVar);
                il.FreeLocal(b.localVar);
                if (resType != b.type)
                {
                    if (ValueProxy.IsProxyType(b.type))
                        il.EmitPropertyGet(typeof(ValueProxy), "Value");
                    else if (b.type.IsValueType)
                        il.Emit(OpCodes.Box, b.type);
                }
                if (k < branches.Count -1)
                    il.Emit(OpCodes.Br, end);                
            }
            il.MarkLabel(end);
            st.Push(resType);
        }
    }
}
