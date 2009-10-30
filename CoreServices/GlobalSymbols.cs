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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using DataEngine.CoreServices.Generation;
using System.Threading;
using System.IO;

namespace DataEngine.CoreServices
{
    public class GlobalSymbols
    {
        public static readonly GlobalSymbols Shared;

        internal Dictionary<object, FuncDef> m_func;
        internal Dictionary<object, MacroFuncDef> m_macro;
        
        internal Dictionary<object, ControlFormBase> m_control;
        internal List<ValueConverter> m_converter;

        static GlobalSymbols()
        {
            Shared = new GlobalSymbols();
            DefineControlOperator(new ControlQuoteForm());
            DefineControlOperator(new ControlPrognForm());
            DefineControlOperator(new ControlProg1Func());
            DefineControlOperator(new ControlAndForm());
            DefineControlOperator(new ControlOrForm());
            DefineControlOperator(new ControlInvokeForm());
            DefineControlOperator(new ControlCondForm());
            DefineControlOperator(new ControlWeakForm());
            DefineControlOperator(new ControlLetForm(Funcs.Let1, false));
            DefineControlOperator(new ControlLetForm(Funcs.Let2, true));
            DefineControlOperator(new ControlTrapForm());
            DefineControlOperator(new ControlCastForm());
            DefineControlOperator(new ControlLambdaQuoteForm());

            Type[] buildinTypes = { 
                typeof(System.SByte), typeof(System.Byte), 
                typeof(System.Int16), typeof(System.UInt16),
                typeof(System.Int32), typeof(System.UInt32),
                typeof(System.Int64), typeof(System.UInt64),
                typeof(System.Single), typeof(System.Double) };

            DefineConverter(buildinTypes, typeof(System.SByte), OpCodes.Conv_I1);
            DefineConverter(buildinTypes, typeof(System.Int16), OpCodes.Conv_I2);
            DefineConverter(buildinTypes, typeof(System.Int32), OpCodes.Conv_I4);
            DefineConverter(buildinTypes, typeof(System.Int64), OpCodes.Conv_I8);
            DefineConverter(buildinTypes, typeof(System.Byte), OpCodes.Conv_U1);
            DefineConverter(buildinTypes, typeof(System.UInt16), OpCodes.Conv_U2);
            DefineConverter(buildinTypes, typeof(System.UInt32), OpCodes.Conv_U4);
            DefineConverter(buildinTypes, typeof(System.UInt64), OpCodes.Conv_U8);
            DefineConverter(buildinTypes, typeof(System.Single), OpCodes.Conv_R4);
            DefineConverter(buildinTypes, typeof(System.Double), OpCodes.Conv_R8);

            DefineConverter(new Type[] { 
                typeof(System.SByte), typeof(System.Byte), 
                typeof(System.Int16), typeof(System.UInt16), 
                typeof(System.Int32), typeof(System.UInt32),
                typeof(System.Int64), typeof(System.UInt64) 
            }, typeof(System.Decimal), typeof(System.Decimal), "op_Implicit");
            DefineConverter(new Type[] { 
                typeof(System.Single), typeof(System.Double) 
            }, typeof(System.Decimal), typeof(System.Decimal), "op_Explicit");

            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.Byte),
                typeof(System.Decimal), "op_Explicit");
            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.Char),
                typeof(System.Decimal), "op_Explicit");
            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.Single),
                typeof(System.Decimal), "op_Explicit");
            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.Double),
                typeof(System.Decimal), "op_Explicit");
            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.Int16),
                typeof(System.Decimal), "op_Explicit");
            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.Int32),
                typeof(System.Decimal), "op_Explicit");
            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.Int64),
                typeof(System.Decimal), "op_Explicit");
            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.SByte),
                typeof(System.Decimal), "op_Explicit");
            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.UInt16),
                typeof(System.Decimal), "op_Explicit");
            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.UInt32),
                typeof(System.Decimal), "op_Explicit");
            DefineConverter(new Type[] { typeof(System.Decimal) }, typeof(System.UInt64),
                typeof(System.Decimal), "op_Explicit");


            DefineConverter(new Type[] { 
                typeof(System.SByte), typeof(System.Byte), 
                typeof(System.Int16), typeof(System.UInt16), 
                typeof(System.Int32), typeof(System.UInt32),
                typeof(System.Int64), typeof(System.UInt64) 
            }, typeof(Integer), typeof(Integer), "op_Implicit");
            DefineConverter(new Type[] { 
                typeof(System.Single), typeof(System.Double) 
            }, typeof(Integer), typeof(Integer), "op_Explicit");

            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.Byte),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.Char),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.Single),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.Double),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.Int16),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.Int32),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.Int64),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.SByte),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.UInt16),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.UInt32),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.UInt64),
                typeof(Integer), "op_Explicit");
            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.Decimal),
                typeof(Integer), "op_Explicit");
                        
            DefineConverter(new Type[] { 
                typeof(System.SByte), typeof(System.Byte), 
                typeof(System.Int16), typeof(System.UInt16), 
                typeof(System.Int32), typeof(System.UInt32),
                typeof(System.Int64), typeof(System.UInt64),
                typeof(System.Single), typeof(System.Double),
                typeof(System.Decimal), typeof(DateTime) 
            }, typeof(System.String), typeof(Convert), "ToString");

            DefineConverter(new Type[] { typeof(Integer) }, typeof(System.String),
                typeof(Integer), "ToString");

            //DefineConverter(new Type[] { typeof(System.Object) }, typeof(System.Double),
            //    typeof(Runtime), "ConvertToDouble");
            //DefineConverter(new Type[] { typeof(System.String) },
            //    typeof(DateTime), typeof(Convert), "ToDateTime");

            //DefineUnaryOperator(Funcs.Neg, typeof(System.SByte), OpCodes.Neg);
            //DefineUnaryOperator(Funcs.Neg, typeof(System.Int16), OpCodes.Neg);
            DefineUnaryOperator(Funcs.Neg, typeof(System.Int32), OpCodes.Neg);
            DefineUnaryOperator(Funcs.Neg, typeof(System.Int64), OpCodes.Neg);
            DefineUnaryOperator(Funcs.Neg, typeof(System.Single), OpCodes.Neg);
            DefineUnaryOperator(Funcs.Neg, typeof(System.Double), OpCodes.Neg);
            DefineUnaryOperator(Funcs.Neg, typeof(Integer), "op_UnaryNegation");
            //DefineUnaryOperator(Funcs.Neg, typeof(System.Decimal), "op_UnaryNegation");
            DefineUnaryOperator(Funcs.Neg, typeof(System.Decimal), typeof(Runtime), "op_UnaryNegation");

            DefineBinaryOperator(Funcs.Add, typeof(System.SByte), OpCodes.Add);
            DefineBinaryOperator(Funcs.Add, typeof(System.Int16), OpCodes.Add);
            DefineBinaryOperator(Funcs.Add, typeof(System.Int32), OpCodes.Add);
            DefineBinaryOperator(Funcs.Add, typeof(System.Int64), OpCodes.Add);
            DefineBinaryOperator(Funcs.Add, typeof(System.Byte), OpCodes.Add);
            DefineBinaryOperator(Funcs.Add, typeof(System.UInt16), OpCodes.Add);
            DefineBinaryOperator(Funcs.Add, typeof(System.UInt32), OpCodes.Add);
            DefineBinaryOperator(Funcs.Add, typeof(System.UInt64), OpCodes.Add);
            DefineBinaryOperator(Funcs.Add, typeof(System.Single), OpCodes.Add);
            DefineBinaryOperator(Funcs.Add, typeof(System.Double), OpCodes.Add);
            DefineBinaryOperator(Funcs.Add, typeof(System.Decimal), "op_Addition");
            DefineBinaryOperator(Funcs.Add, typeof(Integer), "op_Addition");

            DefineBinaryOperator(Funcs.Sub, typeof(System.SByte), OpCodes.Sub);
            DefineBinaryOperator(Funcs.Sub, typeof(System.Int16), OpCodes.Sub);
            DefineBinaryOperator(Funcs.Sub, typeof(System.Int32), OpCodes.Sub);
            DefineBinaryOperator(Funcs.Sub, typeof(System.Int64), OpCodes.Sub);
            DefineBinaryOperator(Funcs.Sub, typeof(System.Byte), OpCodes.Sub);
            DefineBinaryOperator(Funcs.Sub, typeof(System.UInt16), OpCodes.Sub);
            DefineBinaryOperator(Funcs.Sub, typeof(System.UInt32), OpCodes.Sub);
            DefineBinaryOperator(Funcs.Sub, typeof(System.UInt64), OpCodes.Sub);
            DefineBinaryOperator(Funcs.Sub, typeof(System.Single), OpCodes.Sub);
            DefineBinaryOperator(Funcs.Sub, typeof(System.Double), OpCodes.Sub);
            DefineBinaryOperator(Funcs.Sub, typeof(System.Decimal), "op_Subtraction");
            DefineBinaryOperator(Funcs.Sub, typeof(Integer), "op_Subtraction");            

            DefineBinaryOperator(Funcs.Mul, typeof(System.SByte), OpCodes.Mul);
            DefineBinaryOperator(Funcs.Mul, typeof(System.Int16), OpCodes.Mul);
            DefineBinaryOperator(Funcs.Mul, typeof(System.Int32), OpCodes.Mul);
            DefineBinaryOperator(Funcs.Mul, typeof(System.Int64), OpCodes.Mul);
            DefineBinaryOperator(Funcs.Mul, typeof(System.Byte), OpCodes.Mul);
            DefineBinaryOperator(Funcs.Mul, typeof(System.UInt16), OpCodes.Mul);
            DefineBinaryOperator(Funcs.Mul, typeof(System.UInt32), OpCodes.Mul);
            DefineBinaryOperator(Funcs.Mul, typeof(System.UInt64), OpCodes.Mul);
            DefineBinaryOperator(Funcs.Mul, typeof(System.Single), OpCodes.Mul);
            DefineBinaryOperator(Funcs.Mul, typeof(System.Double), OpCodes.Mul);
            DefineBinaryOperator(Funcs.Mul, typeof(System.Decimal), "op_Multiply");
            DefineBinaryOperator(Funcs.Mul, typeof(Integer), "op_Multiply");

            DefineBinaryOperator(Funcs.Div, typeof(System.SByte), typeof(Runtime), "op_Divide", typeof(System.Decimal));
            DefineBinaryOperator(Funcs.Div, typeof(System.Int16), typeof(Runtime), "op_Divide", typeof(System.Decimal));
            DefineBinaryOperator(Funcs.Div, typeof(System.Int32), typeof(Runtime), "op_Divide", typeof(System.Decimal));
            DefineBinaryOperator(Funcs.Div, typeof(System.Int64), typeof(Runtime), "op_Divide", typeof(System.Decimal));
            DefineBinaryOperator(Funcs.Div, typeof(System.Byte), typeof(Runtime), "op_Divide", typeof(System.Decimal));
            DefineBinaryOperator(Funcs.Div, typeof(System.UInt16), typeof(Runtime), "op_Divide", typeof(System.Decimal));
            DefineBinaryOperator(Funcs.Div, typeof(System.UInt32), typeof(Runtime), "op_Divide", typeof(System.Decimal));
            DefineBinaryOperator(Funcs.Div, typeof(System.UInt64), typeof(Runtime), "op_Divide", typeof(System.Decimal));
            DefineBinaryOperator(Funcs.Div, typeof(Integer), typeof(Runtime), "op_Divide", typeof(System.Decimal));
            DefineBinaryOperator(Funcs.Div, typeof(System.Single), OpCodes.Div);
            DefineBinaryOperator(Funcs.Div, typeof(System.Double), OpCodes.Div);
            DefineBinaryOperator(Funcs.Div, typeof(System.Decimal), "op_Division");

            DefineBinaryOperator(Funcs.Mod, typeof(System.SByte), OpCodes.Rem);
            DefineBinaryOperator(Funcs.Mod, typeof(System.Int16), OpCodes.Rem);
            DefineBinaryOperator(Funcs.Mod, typeof(System.Int32), OpCodes.Rem);
            DefineBinaryOperator(Funcs.Mod, typeof(System.Int64), OpCodes.Rem);
            DefineBinaryOperator(Funcs.Mod, typeof(System.Byte), OpCodes.Rem);
            DefineBinaryOperator(Funcs.Mod, typeof(System.UInt16), OpCodes.Rem);
            DefineBinaryOperator(Funcs.Mod, typeof(System.UInt32), OpCodes.Rem);
            DefineBinaryOperator(Funcs.Mod, typeof(System.UInt64), OpCodes.Rem);
            DefineBinaryOperator(Funcs.Mod, typeof(System.Single), OpCodes.Rem);
            DefineBinaryOperator(Funcs.Mod, typeof(System.Double), OpCodes.Rem);
            DefineBinaryOperator(Funcs.Mod, typeof(System.Decimal), "op_Modulus");
            DefineBinaryOperator(Funcs.Mod, typeof(Integer), "op_Modulus");

            DefineBinaryOperator(Funcs.IDiv, typeof(System.SByte), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(System.Int16), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(System.Int32), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(System.Int64), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(System.Byte), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(System.UInt16), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(System.UInt32), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(System.UInt64), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(System.Single), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(System.Double), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(System.Decimal), typeof(Runtime), "op_IntegerDivide", typeof(Integer));
            DefineBinaryOperator(Funcs.IDiv, typeof(Integer), "op_Division");

            DefineStaticOperator(Funcs.Neg, typeof(Runtime), "DynamicNeg");
            DefineStaticOperator(Funcs.Add, typeof(Runtime), "DynamicAdd");
            DefineStaticOperator(Funcs.Sub, typeof(Runtime), "DynamicSub");
            DefineStaticOperator(Funcs.Mul, typeof(Runtime), "DynamicMul");
            DefineStaticOperator(Funcs.Div, typeof(Runtime), "DynamicDiv");
            DefineStaticOperator(Funcs.Mod, typeof(Runtime), "DynamicMod");
            DefineStaticOperator(Funcs.IDiv, typeof(Runtime), "DynamicIDiv");
            DefineStaticOperator(Funcs.Eq, typeof(Runtime), "DynamicEq");
            DefineStaticOperator(Funcs.Gt, typeof(Runtime), "DynamicGt");

            DefineBinaryBoolOperator(Funcs.Eq, typeof(System.SByte), OpCodes.Ceq);
            DefineBinaryBoolOperator(Funcs.Eq, typeof(System.Int16), OpCodes.Ceq);
            DefineBinaryBoolOperator(Funcs.Eq, typeof(System.Int32), OpCodes.Ceq);
            DefineBinaryBoolOperator(Funcs.Eq, typeof(System.Int64), OpCodes.Ceq);
            DefineBinaryBoolOperator(Funcs.Eq, typeof(System.Byte), OpCodes.Ceq);
            DefineBinaryBoolOperator(Funcs.Eq, typeof(System.UInt16), OpCodes.Ceq);
            DefineBinaryBoolOperator(Funcs.Eq, typeof(System.UInt32), OpCodes.Ceq);
            DefineBinaryBoolOperator(Funcs.Eq, typeof(System.UInt64), OpCodes.Ceq);
            DefineBinaryBoolOperator(Funcs.Eq, typeof(System.Single), OpCodes.Ceq);
            DefineBinaryBoolOperator(Funcs.Eq, typeof(System.Double), OpCodes.Ceq);
            DefineBinaryOperator(Funcs.Eq, typeof(System.Decimal), "op_Equality", typeof(System.Boolean));
            DefineBinaryOperator(Funcs.Eq, typeof(Integer), "op_Equality", typeof(System.Boolean));

            DefineBinaryBoolOperator(Funcs.Gt, typeof(System.SByte), OpCodes.Cgt);
            DefineBinaryBoolOperator(Funcs.Gt, typeof(System.Int16), OpCodes.Cgt);
            DefineBinaryBoolOperator(Funcs.Gt, typeof(System.Int32), OpCodes.Cgt);
            DefineBinaryBoolOperator(Funcs.Gt, typeof(System.Int64), OpCodes.Cgt);
            DefineBinaryBoolOperator(Funcs.Gt, typeof(System.Byte), OpCodes.Cgt);
            DefineBinaryBoolOperator(Funcs.Gt, typeof(System.UInt16), OpCodes.Cgt);
            DefineBinaryBoolOperator(Funcs.Gt, typeof(System.UInt32), OpCodes.Cgt);
            DefineBinaryBoolOperator(Funcs.Gt, typeof(System.UInt64), OpCodes.Cgt);
            DefineBinaryBoolOperator(Funcs.Gt, typeof(System.Single), OpCodes.Cgt);
            DefineBinaryBoolOperator(Funcs.Gt, typeof(System.Double), OpCodes.Cgt);
            DefineBinaryOperator(Funcs.Gt, typeof(System.Decimal), "op_GreaterThan", typeof(System.Boolean));
            DefineBinaryOperator(Funcs.Gt, typeof(Integer), "op_GreaterThan", typeof(System.Boolean));

            Shared.Defun(new NotOperator());

            DefineStaticOperator(Funcs.Car, typeof(Lisp), "Car");
            DefineStaticOperator(Funcs.Cdr, typeof(Lisp), "Cdr");
            DefineStaticOperator(Funcs.Cons, typeof(Lisp), "Cons", 1);
            DefineStaticOperator(Funcs.Cons, typeof(Lisp), "Cons", 2);
            DefineStaticOperator(Funcs.ListP, typeof(Lisp), "IsCons");
            DefineStaticOperator(Funcs.List, typeof(Lisp), "List");
            DefineStaticOperator(Funcs.Null, typeof(Lisp), "IsNull");
            DefineStaticOperator(Funcs.Atom, typeof(Runtime), "IsAtom");
            DefineStaticOperator(Funcs.Nth, typeof(Lisp), "Nth");
            DefineStaticOperator(Funcs.NthCdr, typeof(Lisp), "NthCdr");
            DefineStaticOperator(Funcs.Append, typeof(Lisp), "Append");
            DefineStaticOperator(Funcs.Reverse, typeof(Lisp), "Reverse");
            DefineStaticOperator(Funcs.Last, typeof(Lisp), "Last");
            DefineStaticOperator(Funcs.LastCdr, typeof(Lisp), "LastCdr");
            DefineStaticOperator(Funcs.EqualP, typeof(Lisp), "IsEqual");
            DefineStaticOperator(Funcs.Length, typeof(Lisp), "Length");
            DefineStaticOperator(Funcs.First, typeof(Lisp), "First");
            DefineStaticOperator(Funcs.Third, typeof(Lisp), "Third");
            DefineStaticOperator(Funcs.Fourth, typeof(Lisp), "Fourth");
            DefineStaticOperator(Funcs.Fifth, typeof(Lisp), "Fifth");
            DefineStaticOperator(Funcs.Sixth, typeof(Lisp), "Sixth");
            DefineStaticOperator(Funcs.Seventh, typeof(Lisp), "Seventh");
            DefineStaticOperator(Funcs.Eighth, typeof(Lisp), "Eighth");
            DefineStaticOperator(Funcs.Ninth, typeof(Lisp), "Ninth");
            DefineStaticOperator(Funcs.Tenth, typeof(Lisp), "Tenth");

            Defmacro(Funcs.Ne, "(a b)", "(list 'not (list 'eq a b))");
            Defmacro(Funcs.Lt, "(a b)", "(list 'gt b a)");
            Defmacro(Funcs.Ge, "(a b)", "(list 'or (list 'gt a b) (list 'eq a b))");
            Defmacro(Funcs.Le, "(a b)", "(list 'or (list 'lt a b) (list 'eq a b))");
            Defmacro(Funcs.Eval, "(a)", "(list 'invoke '__inst \"Eval\" a)");
            Defmacro("set", "(a b)", "(list 'invoke '__inst \"Set\" a b)");
            Defmacro("setq", "(a b)", "(list 'set (list 'quote a) b)");
            Defmacro("caar", "(a)", "(list 'car (list 'car a))");
            Defmacro("cadr", "(a)", "(list 'cdr (list 'car a))");
            Defmacro("cdar", "(a)", "(list 'car (list 'cdr a))");
            Defmacro("cddr", "(a)", "(list 'cdr (list 'cdr a))");
            Defmacro(Funcs.If, "(a b c)", "(list 'cond (list a b) (list 't c))");
        }

        private GlobalSymbols()
        {
            m_func = new Dictionary<object, FuncDef>();
            m_macro = new Dictionary<object, MacroFuncDef>();
            m_control = new Dictionary<object, ControlFormBase>();
            m_converter = new List<ValueConverter>();
        }

        internal static void DefineControlOperator(ControlFormBase func)
        {
            Shared.m_control.Add(func.ID, func);
        }

        public static void DefineConverter(ValueConverter converter)
        {
            Shared.m_converter.Add(converter);
        }

        public static void DefineConverter(Type[] source, Type destination, OpCode opcode)
        {
            DefineConverter(new ILConverter(source, destination, opcode));
        }

        public static void DefineConverter(Type[] source, Type destination, Type type, string methodName)
        {
            DefineConverter(new MethodConverter(source, destination, type, methodName));
        }

        public static void DefineUnaryOperator(object id, Type type, OpCode opcode)
        {
            Shared.Defun(new BinaryFunc(new FuncName(id, new Type[] { type }), opcode, type));
        }

        public static void DefineUnaryOperator(object id, Type type, string name)
        {
            Shared.Defun(new BinaryFunc(new FuncName(id, new Type[] { type }), 
                type, name, type));
        }

        public static void DefineUnaryOperator(object id, Type type, Type classType, string name)
        {
            Shared.Defun(new BinaryFunc(new FuncName(id, new Type[] { type }),
                classType, name, type));
        }

        public static void DefineBinaryOperator(object id, Type type, OpCode opcode)
        {
            Shared.Defun(new BinaryFunc(new FuncName(id, new Type[] { type, type }), opcode, type));
        }

        public static void DefineBinaryBoolOperator(object id, Type type, OpCode opcode)
        {
            Shared.Defun(new BinaryBoolFunc(id, opcode, type));
        }

        public static void DefineBinaryOperator(object id, Type type, string name)
        {
            Shared.Defun(new BinaryFunc(new FuncName(id, new Type[] { type, type }),
                type, name, type));
        }

        public static void DefineBinaryOperator(object id, Type type, Type classType, string name, Type returnType)
        {
            Shared.Defun(new BinaryFunc(new FuncName(id, new Type[] { type, type }),
                classType, name, new Type[] { type, type }, returnType));
        }

        public static void DefineBinaryOperator(object id, Type type, string name, Type returnType)
        {
            Shared.Defun(new BinaryFunc(new FuncName(id, new Type[] { type, type }),
                type, name, returnType));
        }
        

        public static void DefineStaticOperator(string fname, Type type, string name)
        {
            DefineStaticOperator(Lisp.Defatom(fname), type, name);
        }

        public static void DefineStaticOperator(object id, MethodInfo method)
        {
            Shared.Defun(new InvokeStatic(id, method));
        }

        public static void DefineStaticOperator(object id, Type type, string name)
        {
            bool bfound = false;
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in methods)
                if (method.Name == name && !method.IsGenericMethod)
                {
                    Shared.Defun(new InvokeStatic(id, method));
                    bfound = true;
                }                 
            if (!bfound)
                throw new ArgumentException("Static method not found in class", name);
        }

        public static void DefineStaticOperator(object id, Type type, string name, int arity)
        {
            bool bfound = false;
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in methods)
                if (method.Name == name && !method.IsGenericMethod && 
                    method.GetParameters().Length == arity)
                {
                    Shared.Defun(new InvokeStatic(id, method));
                    bfound = true;
                }
            if (!bfound)
                throw new ArgumentException("Static method not found in class", name);
        }

        public static void DefineStaticOperator(string fname, Type type, string name, int arity)
        {
            DefineStaticOperator(Lisp.Defatom(fname), type, name, arity);
        }

        public static void Defun(string name, string parameters, string body)
        {
            Shared.Defun(new LambdaExpr(Lisp.Defatom(name), parameters, typeof(System.Object), body));
        }

        public void Defun(FuncBase func)
        {
            FuncDef def;
            if (!m_func.TryGetValue(func.Name.ID, out def))
            {
                def = new FuncDef(func);
                m_func.Add(def.ID, def);
            }
            else
                def.Overload(func);
        }

        public static void Defmacro(string name, string parameters, string body)
        {
            Defmacro(Lisp.Defatom(name), parameters, body);
        }

        public static void Defmacro(object id, string parameters, string body)
        {
            Shared.Defmacro(new MacroFunc(id, parameters, body));
        }

        public static void Defmacro(object id, Executive.Parameter[] parameters, object body)
        {
            Shared.Defmacro(new MacroFunc(id, parameters, body));
        }
        
        public void Defmacro(MacroFuncBase func)
        {
            MacroFuncDef def;
            if (!m_macro.TryGetValue(func.Name.ID, out def))
            {
                def = new MacroFuncDef(func);
                m_macro.Add(def.ID, def);
            }
            else
                def.Overload(func);
        }

        internal Dictionary<object, FuncDef> CreateFuncs()
        {
            Dictionary<object, FuncDef> res = new Dictionary<object, FuncDef>();
            foreach (KeyValuePair<object, FuncDef> kvp in m_func)
                res.Add(kvp.Key, kvp.Value);
            return res;
        }

        internal Dictionary<object, MacroFuncDef> CreateMacros()
        {
            Dictionary<object, MacroFuncDef> res = new Dictionary<object, MacroFuncDef>();
            foreach (KeyValuePair<object, MacroFuncDef> kvp in m_macro)
                res.Add(kvp.Key, kvp.Value);
            return res;
        }

        internal Dictionary<object, ControlFormBase> CreateControls()
        {
            return m_control;
        }

        internal List<ValueConverter> CreateConverters()
        {
            return m_converter;
        }
    }
}
