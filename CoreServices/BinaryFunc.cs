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
    class BinaryFunc: FuncBase
    {
        private OpCode _opcode;
        private int _case;
        private Type _type;
        private String _methodName;
        private Type[] _paramTypes;
        private Type _returnType;

        public BinaryFunc(FuncName name, OpCode opcode, Type returnType)
        {
            Name = name;
            _case = 0;
            _opcode = opcode;        
            _returnType = returnType;
        }

        public BinaryFunc(FuncName name, OpCode opcode, Type type, Type returnType)
        {
            Name = name;
            _case = 1;
            _opcode = opcode;
            _type = type;
            _returnType = returnType;
        }

        public BinaryFunc(FuncName name, Type type, String method_name, Type returnType)
        {
            Name = name;
            _case = 2;
            _type = type;
            _methodName = method_name;
            _returnType = returnType;
        }

        public BinaryFunc(FuncName name, Type type, String method_name, Type[] paramTypes, Type returnType)
        {
            Name = name;
            _case = 3;
            _type = type;
            _methodName = method_name;
            _paramTypes = paramTypes;
            _returnType = returnType;
        }

        public override Type Compile(Executive engine, ILGen il, LocalAccess locals, Type[] parameterTypes)
        {
            switch (_case)
            {
                case 0:
                    il.Emit(_opcode);
                    break;

                case 1:
                    il.Emit(_opcode, _type);
                    break;

                case 2:
                    il.EmitCall(_type, _methodName);
                    break;

                case 3:
                    il.EmitCall(_type, _methodName, _paramTypes);
                    break;
            }
            
            return _returnType;
        }
    }
}
