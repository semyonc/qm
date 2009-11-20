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
using System.Globalization;

namespace DataEngine.CoreServices
{
    public class Runtime
    {        
        public static object DynamicNeg([Implict] Executive engine, object arg)
        {
            return engine.DynamicOperators.Neg(arg);
        }

        public static object DynamicAdd([Implict] Executive engine, object arg1, object arg2)
        {
            return engine.DynamicOperators.Add(arg1, arg2);
        }

        public static object DynamicSub([Implict] Executive engine, object arg1, object arg2)
        {
            return engine.DynamicOperators.Sub(arg1, arg2);
        }

        public static object DynamicMul([Implict] Executive engine, object arg1, object arg2)
        {
            return engine.DynamicOperators.Mul(arg1, arg2);
        }

        public static object DynamicDiv([Implict] Executive engine, object arg1, object arg2)
        {
            return engine.DynamicOperators.Div(arg1, arg2);
        }

        public static object DynamicMod([Implict] Executive engine, object arg1, object arg2)
        {
            return engine.DynamicOperators.Mod(arg1, arg2);
        }

        public static Integer DynamicIDiv([Implict] Executive engine, object arg1, object arg2)
        {
            return engine.DynamicOperators.IDiv(arg1, arg2);
        }

        public static object DynamicEq([Implict] Executive engine, object arg1, object arg2)
        {
            return engine.OperatorEq(arg1, arg2);
        }

        public static object DynamicGt([Implict] Executive engine, object arg1, object arg2)
        {
            return engine.OperatorGt(arg1, arg2);
        }

        public static bool IsAtom(object o)
        {
            return Lisp.IsNode(o);
        }

        public static bool IsDBNull(object o)
        {
            return o == DBNull.Value;
        }

        public static double ConvertToDouble(object value)
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        // preventing xs:float(-0.0) = -0
        public static decimal op_UnaryNegation(decimal a)
        {
            return 0 - a;
        }

        public static decimal op_Divide(sbyte a, sbyte b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(byte a, byte b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(short a, short b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(ushort a, ushort b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(int a, int b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(uint a, uint b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(long a, long b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(ulong a, ulong b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(Integer a, Integer b)
        {
            return (Decimal)a / (Decimal)b;
        }

        public static Integer op_IntegerDivide(sbyte a, sbyte b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(byte a, byte b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(short a, short b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(ushort a, ushort b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(int a, int b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(uint a, uint b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(long a, long b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(ulong a, ulong b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(float a, float b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(double a, double b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(decimal a, decimal b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }
    }
}
