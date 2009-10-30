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
    public class LambdaExpr: FuncBase
    {
        internal Executive.Parameter[] _parameters;
        internal object _body;
        internal Type _retType;
        internal SymbolLink _compiledBody;

        public bool CompileBody { get; set; }
        
        public LambdaExpr(object id, string paramstr, Type returnType, string body)
            : this(id, Executive.CreateParameters(paramstr), returnType, LispParser.Parse(body))
        {
        }

        public LambdaExpr(object id, Executive.Parameter[] parameters, Type returnType, object body)
        {
            Name = new FuncName(id, parameters);
            _parameters = parameters;
            _retType = returnType;
            _body = body;
            _compiledBody = new SymbolLink();
        }        

        public override Type Compile(Executive engine, ILGen il, LocalAccess locals, Type[] parameterTypes)
        {
            if (CompileBody && _compiledBody.Value == null)
                engine.Compile(_parameters, _body, _compiledBody);
            locals.AddDependence(this);

            LocalBuilder[] localVar = new LocalBuilder[parameterTypes.Length];
            for (int k = parameterTypes.Length - 1; k >= 0; k--)
            {
                localVar[k] = il.DeclareLocal(parameterTypes[k]);
                il.Emit(OpCodes.Stloc, localVar[k]);
            }
            
            il.Emit(OpCodes.Ldarg_0);
            il.EmitPropertyGet(typeof(CompiledLambda), "Engine");

            il.Emit(OpCodes.Ldarg_1);
            il.EmitInt(locals.DefineConstant(Name.ID));
            il.Emit(OpCodes.Ldelem_Ref);

            il.Emit(OpCodes.Ldarg_1);
            il.EmitInt(locals.DefineConstant(_parameters));
            il.Emit(OpCodes.Ldelem_Ref);
            il.Emit(OpCodes.Isinst, typeof(Executive.Parameter[]));

            il.Emit(OpCodes.Ldarg_1);
            il.EmitInt(locals.DefineConstant(_body));
            il.Emit(OpCodes.Ldelem_Ref);

            il.EmitInt(Name.Arity);
            il.Emit(OpCodes.Newarr, typeof(System.Object));
            for (int k = 0; k < Name.Arity; k++)
            {
                il.Emit(OpCodes.Dup);
                il.EmitInt(k);
                if (!Name.VariableLength || k < Name.Arity - 1)
                {
                    il.Emit(OpCodes.Ldloc, localVar[k]);
                    if (localVar[k].LocalType.IsValueType)
                        il.Emit(OpCodes.Box, localVar[k].LocalType);
                    il.FreeLocal(localVar[k]);
                }
                else
                {
                    il.EmitInt(localVar.Length - k);
                    il.Emit(OpCodes.Newarr, Name.VariableParamType);
                    for (int s = k; s < localVar.Length; s++)
                    {
                        il.Emit(OpCodes.Dup);
                        il.EmitInt(s - k);
                        il.Emit(OpCodes.Ldloc, localVar[s]);
                        if (localVar[s].LocalType != Name.VariableParamType &&
                            localVar[s].LocalType.IsValueType)
                            il.Emit(OpCodes.Box, localVar[s].LocalType);
                        il.EmitStoreElement(Name.VariableParamType);
                        il.FreeLocal(localVar[s]);
                    }
                    il.EmitCall(typeof(Lisp), "List");
                }
                il.EmitStoreElement(typeof(System.Object));
            }

            il.Emit(OpCodes.Ldarg_1);
            il.EmitInt(locals.DefineConstant(_compiledBody));
            il.Emit(OpCodes.Ldelem_Ref);
            il.Emit(OpCodes.Isinst, typeof(SymbolLink));
            
            MethodInfo mi = engine.GetType().GetMethod("Apply");
            il.Emit(OpCodes.Callvirt, mi);
            return _retType;            
        }
    }
}
