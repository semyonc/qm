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
    internal class ControlCastForm: ControlFormBase
    {
        private MethodInfo _raiseInvalidCast;

        public ControlCastForm()
        {
            ID = Funcs.Cast;
            Type type = GetType();
            _raiseInvalidCast = type.GetMethod("CastRaiseError", BindingFlags.Static | BindingFlags.Public);
        }

        public static void CastRaiseError()
        {
            throw new InvalidCastException();
        }

        public override void Compile(Executive engine, object form, ILGen il, LocalAccess locals, Stack<Type> st, object[] args)
        {
            if (args.Length != 2)
                throw new ArgumentException("Unproperly formated cast expression");
            engine.CompileExpr(il, locals, st, args[0]);
            Type type = args[1] as Type;
            if (type == null)
                throw new ArgumentException("Expecting type value");
            Type curr_type = st.Pop();
            if (curr_type.IsValueType)
            {
                if (type == typeof(System.Object))
                    il.EmitBoxing(curr_type);
                else if (curr_type != type)
                    throw new InvalidCastException();
            }
            else
            {
                il.Emit(OpCodes.Isinst, type);
                il.Emit(OpCodes.Dup);
                Label ok = il.DefineLabel();
                il.Emit(OpCodes.Brtrue_S, ok);
                il.EmitCall(_raiseInvalidCast);
                il.MarkLabel(ok);
                if (type.IsValueType)
                    il.EmitUnbox(type);
            }
            st.Push(type);
        }
    }
}
