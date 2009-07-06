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
    internal class ControlInvokeForm: ControlFormBase
    {
        public ControlInvokeForm()
        {
            ID = Funcs.Invoke;
        }

        public override void Compile(Executive engine, object form, ILGen il, LocalAccess locals, Stack<Type> st, object[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException();
            engine.CompileExpr(il, locals, st, args[0]);
            Type inst = st.Pop();
            il.Emit(OpCodes.Castclass, inst);
            String name = args[1].ToString();
            MethodInfo methodInfo;
            if (args.Length > 2)
            {
                Type[] parameters = new Type[args.Length - 2];
                ParameterModifier modifier = new ParameterModifier(parameters.Length);
                LocalBuilder[] localVar = new LocalBuilder[parameters.Length];
                for (int k = 0; k < args.Length - 2; k++)
                {
                    engine.CompileExpr(il, locals, st, args[k + 2]);
                    parameters[k] = st.Pop();
                    localVar[k] = il.DeclareLocal(parameters[k]);
                    il.Emit(OpCodes.Stloc, localVar[k]);
                }
                methodInfo = inst.GetMethod(name, parameters, new ParameterModifier[] { modifier });
                if (methodInfo == null)
                    throw new ArgumentException("Method not found in class", name);
                ParameterInfo[] pi = methodInfo.GetParameters();
                for (int k = 0; k < pi.Length; k++)
                {
                    il.Emit(OpCodes.Ldloc, localVar[k]);
                    if (pi[k].ParameterType != parameters[k] && parameters[k].IsValueType)
                        il.Emit(OpCodes.Box, parameters[k]);
                    il.FreeLocal(localVar[k]);
                }
            }
            else
            {
                methodInfo = inst.GetMethod(name, null);
                if (methodInfo == null)
                    throw new ArgumentException("Method not found in class", name);
            }
            il.Emit(OpCodes.Callvirt, methodInfo);
            Type resType = methodInfo.GetReturnType();
            if (resType == typeof(System.Boolean))
            { // Implict boolean convertion
                Label l_false = il.DefineLabel();
                Label end = il.DefineLabel();
                il.Emit(OpCodes.Brfalse_S, l_false);
                il.Emit(OpCodes.Ldsfld, typeof(RuntimeOps).GetField("True"));
                il.Emit(OpCodes.Br_S, end);
                il.MarkLabel(l_false);
                il.EmitNull();
                il.MarkLabel(end);
                st.Push(typeof(System.Object));
            }
            else if (resType == typeof(void))
            { // assume that all procedures returns t
                il.Emit(OpCodes.Ldsfld, typeof(RuntimeOps).GetField("True"));
                st.Push(typeof(System.Object));
            }
            else
                st.Push(resType);
        }
    }
}
