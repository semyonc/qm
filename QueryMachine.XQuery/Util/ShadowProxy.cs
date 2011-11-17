//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

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
