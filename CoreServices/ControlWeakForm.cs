﻿//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
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
    internal class ControlWeakForm: ControlFormBase
    {
        public ControlWeakForm()
        {
            ID = Funcs.Weak;
        }

        public override object MacroExpand(Executive engine, object[] lval, ref bool proceed)
        {
            return Lisp.Functor(ID, lval);
        }

        public override void CreateVariables(Executive engine, object form, ILGen il, LocalAccess locals, object[] args)
        {
            return;
        }

        public override void Compile(Executive engine, object form, ILGen il, LocalAccess locals, Stack<Type> st, object[] args)
        {
            if (args.Length != 1 || !Lisp.IsAtom(args[0]))
                throw new ArgumentException("Unproperly formated expression");
            SymbolLink link = engine.Get(args[0]);
            il.Emit(OpCodes.Ldarg_1);
            il.EmitInt(locals.DefineConstant(link));
            il.Emit(OpCodes.Ldelem_Ref);
            il.EmitPropertyGet(typeof(SymbolLink), "Value");
            st.Push(typeof(System.Object));
        }
    }
}
