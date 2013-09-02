//        Copyright (c) 2009-2012, Semyon A. Chertkov (semyonc@gmail.com)
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

using DataEngine.CoreServices.Generation;

namespace DataEngine.CoreServices
{
    class ControlIteratorForm: ControlFormBase
    {
        private Type _generic_enumerable_type;
        private Type _generic_enumerator_type;
        private Type _enumerable_type;
        private Type _enumerator_type;
        private Type _disposable_type;
        private MethodInfo _raiseError;


        public ControlIteratorForm()
        {
            ID = Funcs.ForEach;
            Type type = GetType();
            _raiseError = type.GetMethod("IteratorRaiseError", BindingFlags.Static | BindingFlags.Public);
            _generic_enumerable_type = typeof(IEnumerable<>);
            _generic_enumerator_type = typeof(IEnumerator<>);
            _enumerable_type = typeof(IEnumerable);
            _enumerator_type = typeof(IEnumerator);
            _disposable_type = typeof(IDisposable);
        }

        public static void IteratorRaiseError()
        {
            throw new Exception("Error in ForEach: IEnumerable is not implemented by expression");
        }

        public override object MacroExpand(Executive engine, object[] lval, ref bool proceed)
        {
            if (lval.Length < 2 || !Lisp.IsCons(lval[0]))
                throw new ArgumentException("Unproperly formated expression");
            object[] param = Lisp.ToArray(lval[0]);
            for (int k = 1; k < param.Length; k++)
                param[k] = engine.MacroExpand(param[k], ref proceed);
            object[] res = new object[lval.Length + 1];
            res[0] = ID;
            res[1] = Lisp.List(param);
            for (int k = 1; k < lval.Length; k++)
                res[k + 1] = engine.MacroExpand(lval[k], ref proceed);
            return Lisp.List(res);
        }

        public override void CreateVariables(Executive engine, object form, ILGen il, LocalAccess locals, object[] args)
        {
            if (args.Length < 2 || !Lisp.IsCons(args[0]))
                throw new ArgumentException("Unproperly formated expression");
            object[] param = Lisp.ToArray(args[0]);
            if (param.Length != 2 && param.Length != 3)
                throw new ArgumentException("Unproperly formated expression");
            locals.DeclareScope(form);
            locals.EnterScope(form, false);
            locals.DeclareLocal(param[0]);
            if (param[1] != null)
                engine.CreateVariables(il, locals, param[1]);
            if (param.Length == 3)
                engine.CreateVariables(il, locals, param[2]);
            locals.ActivateScope();
            for (int k = 1; k < args.Length; k++)
                engine.CreateVariables(il, locals, args[k]);
            locals.LeaveScope();
        }

        public override void Compile(Executive engine, object form, ILGen il, LocalAccess locals, Stack<Type> st, object[] args)
        {
            if (args.Length < 2 || !Lisp.IsCons(args[0]))
                throw new ArgumentException("Unproperly formated expression");
            object[] param = Lisp.ToArray(args[0]);
            if (param.Length != 2 && param.Length != 3)
                throw new ArgumentException("Unproperly formated expression");
            object var = param[0];
            Type varType = null;
            object expr;
            if (param.Length == 3)
            {
                if (param[1] != null)
                {
                    if (!(param[1] is System.Type))
                        throw new ArgumentException("Unproperly formated expression");
                    varType = (System.Type)param[1];
                }
                expr = param[2];
            }
            else
                expr = param[1];
            Type enumerableType;
            Type enumeratorType;
            if (varType != null)
            {
                enumerableType = _generic_enumerable_type.MakeGenericType(
                    new Type[] { varType });
                enumeratorType = _generic_enumerator_type.MakeGenericType(
                    new Type[] { varType });
            }
            else
            {
                varType = typeof(System.Object);
                enumerableType = _enumerable_type;
                enumeratorType = _enumerator_type;
            }
            locals.EnterScope(form, false);
            engine.CompileExpr(il, locals, st, expr);
            st.Pop();
            Label typeCheck = il.DefineLabel();
            il.Emit(OpCodes.Isinst, enumerableType);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Brtrue, typeCheck);
            il.EmitCall(_raiseError);
            il.MarkLabel(typeCheck);
            il.EmitCall(enumerableType, "GetEnumerator");
            LocalBuilder iter = il.DeclareLocal(enumeratorType);
            il.Emit(OpCodes.Stloc, iter);
            il.EmitNull();
            Label begin = il.DefineLabel();
            LocalBuilder localvar = locals.BindLocal(var, varType);
            Label end = il.DefineLabel();
            il.MarkLabel(begin);
            il.Emit(OpCodes.Ldloc, iter);
            il.Emit(OpCodes.Isinst, _enumerator_type);
            il.EmitCall(_enumerator_type, "MoveNext");
            il.Emit(OpCodes.Brfalse_S, end);
            il.Emit(OpCodes.Pop);
            il.Emit(OpCodes.Ldloc, iter);
            il.EmitPropertyGet(enumeratorType, "Current");
            il.Emit(OpCodes.Stloc, localvar);
            locals.ActivateScope();
            for (int k = 1; k < args.Length; k++)
            {
                engine.CompileExpr(il, locals, st, args[k]);
                if (k < args.Length - 1)
                {
                    st.Pop();
                    il.Emit(OpCodes.Pop);
                }                
            }
            il.Emit(OpCodes.Br, begin);
            il.MarkLabel(end);
            if (enumeratorType.IsGenericType)
            {
                il.Emit(OpCodes.Ldloc, iter);
                il.Emit(OpCodes.Isinst, _disposable_type);
                il.EmitCall(_disposable_type, "Dispose");
            }
            locals.DestroyScope();
        }
    }
}
