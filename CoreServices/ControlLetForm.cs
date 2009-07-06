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
    internal class ControlLetForm : ControlFormBase
    {
        private bool _activeScope;

        public ControlLetForm(object id, bool activeScope)
        {
            ID = id;
            _activeScope = activeScope;
        }

        public override object MacroExpand(Executive engine, object[] lval, ref bool proceed)
        {
            object[] arg = Lisp.ToArray(lval[0]);
            for (int k = 0; k < arg.Length; k++)
            {
                object[] pair = Lisp.ToArray(arg[k]);
                pair[1] = engine.MacroExpand(pair[1], ref proceed);
                arg[k] = Lisp.List(pair);
            }
            return Lisp.List(ID, Lisp.List(arg), 
                engine.MacroExpand(lval[1], ref proceed));
        }

        public override void CreateVariables(Executive engine, object form, ILGen il, LocalAccess locals, object[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("Unproperly formated expression");
            locals.DeclareScope(form);
            locals.EnterScope(form, _activeScope);
            foreach (object arg in Lisp.getIterator(args[0]))
            {
                object[] pair = Lisp.ToArray(arg);
                if (pair.Length != 2 || !Lisp.IsAtom(pair[0]))
                    throw new ArgumentException("Unproperly formated expression");
                locals.DeclareLocal(pair[0]);
                engine.CreateVariables(il, locals, pair[1]);
            }
            if (!_activeScope)
                locals.ActivateScope();
            for (int k = 1; k < args.Length; k++)
                engine.CreateVariables(il, locals, args[k]);
            locals.LeaveScope();
        }

        public override void Compile(Executive engine, object form, ILGen il, LocalAccess locals, Stack<Type> st, object[] args)
        {
            locals.EnterScope(form, _activeScope);
            foreach (object arg in Lisp.getIterator(args[0]))
            {
                object[] pair = Lisp.ToArray(arg);
                engine.CompileExpr(il, locals, st, pair[1]);
                LocalBuilder localvar = locals.BindLocal(pair[0], st.Pop());
                il.Emit(OpCodes.Stloc, localvar);
            }
            if (!_activeScope)
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
            locals.DestroyScope();
        }
    }
}
