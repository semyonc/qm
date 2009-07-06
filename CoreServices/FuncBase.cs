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
using DataEngine.CoreServices.Generation;

namespace DataEngine.CoreServices
{
    /// <summary>
    /// Abstract base class for lisp functions
    /// </summary>
    abstract public class FuncBase
    {
        /// <summary>
        /// Atom of this function
        /// </summary>
        /// <returns>Atom</returns>
        public FuncName Name { get; protected set; }

        /// <summary>
        /// Execute function
        /// </summary>
        /// <param name="engine">Calling executive</param>
        /// <param name="args">Argument values</param>
        /// <param name="context">Current context</param>
        /// <returns>Result value</returns>
        abstract public Type Compile(Executive engine, ILGen il, LocalAccess locals, Type[] parameterTypes);

        public override string ToString()
        {
            return Name.ToString();
        }
    }

    /// <summary>
    /// Abstract base class for lisp control function
    /// such as cond, prog and so on
    /// </summary>
    abstract internal class ControlFormBase
    {
        public object ID { get; protected set; }

        public virtual void CreateVariables(Executive engine, object form, ILGen il, LocalAccess locals, object[] args)
        {
            foreach (object arg in args)
                engine.CreateVariables(il, locals, arg);
        }

        public virtual object MacroExpand(Executive engine, object[] lval, ref bool proceed)
        {
            return engine.MacroExpand(ID, lval, ref proceed);
        }

        abstract public void Compile(Executive engine, object form, ILGen il, LocalAccess locals, Stack<Type> st, object[] args);
    }

    /// <summary>
    /// Abstract base class for lisp macros
    /// </summary>
    abstract public class MacroFuncBase
    {
        public MacroFuncName Name { get; protected set; }

        abstract public object Execute(Executive engine, object[] lval, out bool proceed);
    }

}
