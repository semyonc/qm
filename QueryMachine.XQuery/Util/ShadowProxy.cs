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

using DataEngine.CoreServices;

namespace DataEngine.XQuery.Util
{
    internal sealed class ShadowProxy : ValueProxy
    {

        private object _value;
        private int _valueCode;
        private bool _isNumeric;

        public ShadowProxy(ValueProxy proxy)
        {
            _value = proxy.Value;
            _valueCode = proxy.GetValueCode();
            _isNumeric = proxy.IsNumeric();
        }


        public override int GetValueCode()
        {
            return _valueCode;
        }

        public override bool IsNumeric()
        {
            return _isNumeric;
        }

        public override object Value
        {
            get
            {
                return _value;
            }
        }

        protected override bool Eq(ValueProxy val)
        {
            throw new OperatorMismatchException(Funcs.Eq, _value, val.Value);
        }

        protected override bool Gt(ValueProxy val)
        {
            throw new OperatorMismatchException(Funcs.Gt, _value, val.Value);
        }

        protected override ValueProxy Promote(ValueProxy val)
        {
            throw new NotImplementedException();
        }

        protected override ValueProxy Neg()
        {
            throw new OperatorMismatchException(Funcs.Neg, _value, null);
        }

        protected override ValueProxy Add(ValueProxy val)
        {
            throw new OperatorMismatchException(Funcs.Add, _value, val.Value);
        }

        protected override ValueProxy Sub(ValueProxy val)
        {
            throw new OperatorMismatchException(Funcs.Sub, _value, val.Value);
        }

        protected override ValueProxy Mul(ValueProxy val)
        {
            switch (val.GetValueCode())
            {
                case YearMonthDurationValue.ProxyValueCode:
                    return new YearMonthDurationValue.Proxy(
                        YearMonthDurationValue.Multiply((YearMonthDurationValue)val.Value, Convert.ToDouble(_value)));
                case DayTimeDurationValue.ProxyValueCode:
                    return new DayTimeDurationValue.Proxy(
                        DayTimeDurationValue.Multiply((DayTimeDurationValue)val.Value, Convert.ToDouble(_value)));
                default:
                    throw new OperatorMismatchException(Funcs.Mul, _value, val.Value);
            }
        }

        protected override ValueProxy Div(ValueProxy val)
        {
            throw new OperatorMismatchException(Funcs.Div, _value, val.Value);
        }

        protected override Integer IDiv(ValueProxy val)
        {
            throw new OperatorMismatchException(Funcs.IDiv, _value, val.Value);
        }

        protected override ValueProxy Mod(ValueProxy val)
        {
            throw new OperatorMismatchException(Funcs.Mod, _value, val.Value);
        }
    }
}
