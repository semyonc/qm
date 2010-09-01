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
    public class OperatorMismatchException : Exception
    {
        public OperatorMismatchException(object id, object arg1, object arg2)
        {
            ID = id;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public OperatorMismatchException(object id, object arg)
            : this(id, arg, null)
        {
        }

        public object ID { get; private set; }

        public object Arg1 { get; private set; }

        public object Arg2 { get; private set; }
    }

    public class Runtime
    {
        public static ValueProxy DynamicNeg(object arg)
        {
            return -ValueProxy.New(arg);
        }

        public static ValueProxy DynamicAdd(object arg1, object arg2)
        {
            return ValueProxy.New(arg1) + ValueProxy.New(arg2);
        }

        public static ValueProxy DynamicSub(object arg1, object arg2)
        {
            return ValueProxy.New(arg1) - ValueProxy.New(arg2);
        }

        public static ValueProxy DynamicMul(object arg1, object arg2)
        {
            return ValueProxy.New(arg1) * ValueProxy.New(arg2);
        }

        public static ValueProxy DynamicDiv(object arg1, object arg2)
        {
            return ValueProxy.New(arg1) / ValueProxy.New(arg2);
        }

        public static ValueProxy DynamicMod(object arg1, object arg2)
        {
            return ValueProxy.New(arg1) % ValueProxy.New(arg2);
        }

        public static Integer DynamicIDiv(object arg1, object arg2)
        {
            return ValueProxy.op_IntegerDivide(ValueProxy.New(arg1), ValueProxy.New(arg2));
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

        public static decimal op_Divide(short a, short b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(int a, int b)
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

        public static Integer op_IntegerDivide(short a, short b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(int a, int b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(long a, long b)
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
