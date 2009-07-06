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
    public class InvokeDelegate: FuncBase
    {
        private Delegate m_delegate;

        public InvokeDelegate(Object ID, Delegate del)
        {
            ParameterInfo[] parameter_info = del.Method.GetParameters();
            Type[] parameter_types = new Type[parameter_info.Length];
            foreach(ParameterInfo pi in parameter_info)
            {
                if (pi.IsByRefParameter())
                    throw new ArgumentException("ByRef parameter is not allowed");
                parameter_types[pi.Position] = pi.ParameterType;
            }
            Name = new FuncName(ID, parameter_types);
            m_delegate = del;
        }

        public override Type Compile(Executive engine, ILGen il, LocalAccess locals, Type[] parameterTypes)
        {
            LocalBuilder[] localVar = new LocalBuilder[Name.Arity];
            for (int k = Name.Arity -1; k >= 0; k--)
            {
                localVar[k] = il.DeclareLocal(Name.Signature[k]);
                il.Emit(OpCodes.Stloc, localVar[k]);
            }
            il.Emit(OpCodes.Ldarg_1);
            il.EmitInt(locals.DefineConstant(m_delegate));
            il.Emit(OpCodes.Ldelem_Ref);
            il.Emit(OpCodes.Isinst, m_delegate.GetType());
            for (int k = 0; k < Name.Arity; k++)
            {
                il.Emit(OpCodes.Ldloc, localVar[k]);
                il.FreeLocal(localVar[k]);
            }
            il.EmitCall(m_delegate.GetType(), "Invoke");
            return m_delegate.Method.GetReturnType();
        }
    }
}
