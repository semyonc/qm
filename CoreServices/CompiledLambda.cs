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
using System.Reflection.Emit;
using DataEngine.CoreServices.Generation;
using System.IO;
using System.Security;
using System.Security.Permissions;


namespace DataEngine.CoreServices
{    
    public class CompiledLambda
    {
        private delegate object InvokeLambda(CompiledLambda sender, object[] consts, object[] args);

        private DynamicMethod dynamicMethod;
        private InvokeLambda invoke;

        public CompiledLambda(Executive executive, Executive.Parameter[] parameters)
        {
            Engine = executive;
            Arity = parameters.Length;
            Parameters = parameters;
            dynamicMethod = new DynamicMethod("", typeof(System.Object),
                new Type[] { typeof(CompiledLambda), typeof(object[]), typeof(object[]) }, GetType().Module);
        }

        public Executive Engine { get; set; }
        public int Arity { get; set; }
        public Executive.Parameter[] Parameters { get; set; }
        public SymbolLink[] Values { get; set; }
        public Object[] Consts { get; set; }
        public LambdaExpr[] Dependences { get; set; }
        public Type ReturnType { get; set; }

        [SecurityCritical]
        public object Invoke(object[] args)
        {
            if (invoke == null)
                invoke = (InvokeLambda)dynamicMethod.CreateDelegate(typeof(InvokeLambda));
            if ((args == null && Arity == 0) || args.Length == Arity)
            {
                try
                {                    
                    return invoke(this, Consts, args);
                }
                catch (Exception ex)
                {
                    Engine.HandleRuntimeException(ex);
                }

            }
            throw new InvalidOperationException("Invoke");
        }

#if DEBUG
        public ILGen CreateILGen(TextWriter tw)
        {
            return new DebugILGen(dynamicMethod.GetILGenerator(), tw);
        }
#else
        public ILGen CreateILGen()
        {
            return new ILGen(dynamicMethod.GetILGenerator());
        }
#endif

    }
}
