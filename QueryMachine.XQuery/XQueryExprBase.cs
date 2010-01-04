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
using System.Xml.XPath;
using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    internal class EmptySequnceException: Exception
    {
    }

    public abstract class XQueryExprBase: IBindableObject
    {
        private XQueryContext _queryContext;

        public XQueryExprBase(XQueryContext queryContext)
        {
            _queryContext = queryContext;
        }

        public abstract void Bind(Executive.Parameter[] parameters);

        public abstract IEnumerable<SymbolLink> EnumDynamicFuncs();

        public virtual bool IsContextSensitive(Executive.Parameter[] parameters)
        {
            return false;
        }

        public abstract object Execute(IContextProvider provider, object[] args);

        public XQueryContext QueryContext
        {
            get
            {
                return _queryContext;
            }
        }

        public object ToLispFunction()
        {
            XQueryExpr expr = this as XQueryExpr;
            if (expr != null && expr.m_expr.Length == 1)
                return expr.m_expr[0];
            else
                return Lisp.List(ID.DynExecuteExpr, this, ID.Context, Lisp.ARGV);
        }
    }
}
