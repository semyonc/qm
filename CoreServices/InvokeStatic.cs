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
    public class ImplictAttribute : Attribute
    {
        public ImplictAttribute()
        {
        }
    }

    public class InvokeStatic: FuncBase
    {
        private MethodInfo _methodInfo;
        private bool _implictExecutive;

        public InvokeStatic(Object ID, MethodInfo mi)
        {
            _methodInfo = mi;
            _implictExecutive = false;
            ParameterInfo[] parameter_info = _methodInfo.GetParameters();
            List<Executive.Parameter> parameters = new List<Executive.Parameter>();
            foreach (ParameterInfo pi in parameter_info)
            {
                if (pi.IsByRefParameter())
                    throw new ArgumentException("ByRef parameter is not allowed");
                Executive.Parameter p = new Executive.Parameter();
                p.Type = pi.ParameterType;
                if (pi.Position == 0 && pi.ParameterType == typeof(Executive))
                {
                    object[] attrs = pi.GetCustomAttributes(true);
                    foreach (object atr in attrs)
                        if (atr is ImplictAttribute)
                        {
                            _implictExecutive = true;
                            break;
                        }
                    if (_implictExecutive)
                        continue;
                }
                else if (pi.ParameterType.IsArray && 
                    pi.Position == parameter_info.Length - 1)
                {
                    object[] attrs = pi.GetCustomAttributes(true);
                    foreach (object atr in attrs)
                        if (atr is System.ParamArrayAttribute)
                        {
                            p.VariableParam = true;
                            p.Type = pi.ParameterType.GetElementType();
                            break;
                        }
                }
                parameters.Add(p);
            }
            Name = new FuncName(ID, parameters.ToArray());
        }

        public override Type Compile(Executive engine, ILGen il, LocalAccess locals, Type[] parameterTypes)
        {
            if (_implictExecutive)
            {
                LocalBuilder[] localVar = new LocalBuilder[parameterTypes.Length];
                for (int k = localVar.Length - 1; k >= 0; k--)
                {
                    localVar[k] = il.DeclareLocal(parameterTypes[k]);
                    il.Emit(OpCodes.Stloc, localVar[k]);
                }
                il.Emit(OpCodes.Ldarg_0);
                il.EmitPropertyGet(typeof(CompiledLambda), "Engine");
                for (int k = 0; k < localVar.Length; k++)
                {
                    il.Emit(OpCodes.Ldloc, localVar[k]);
                    il.FreeLocal(localVar[k]);
                }                
            }
            if (Name.VariableLength)
            {
                LocalBuilder[] localVar = new LocalBuilder[parameterTypes.Length - Name.Arity + 1];
                for (int k = 0; k < localVar.Length; k++)
                {
                    if (parameterTypes[k + Name.Arity - 1] != Name.VariableParamType &&
                        Name.VariableParamType != typeof(System.Object))
                        throw new InvalidOperationException();
                    localVar[localVar.Length - k -1] = il.DeclareLocal(parameterTypes[k + Name.Arity - 1]);
                    il.Emit(OpCodes.Stloc, localVar[localVar.Length - k - 1]);
                }
                il.EmitInt(localVar.Length);
                il.Emit(OpCodes.Newarr, Name.VariableParamType);
                for (int k = 0; k < localVar.Length; k++)
                {
                    il.Emit(OpCodes.Dup);
                    il.EmitInt(k);
                    il.Emit(OpCodes.Ldloc, localVar[k]);
                    if (localVar[k].LocalType != Name.VariableParamType &&
                        localVar[k].LocalType.IsValueType)
                        il.Emit(OpCodes.Box, localVar[k].LocalType);
                    il.EmitStoreElement(Name.VariableParamType);
                    il.FreeLocal(localVar[k]);
                }
            }
            il.EmitCall(_methodInfo);
            return _methodInfo.GetReturnType();
        }
    }
}
