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
using System.Xml.XPath;
using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public abstract class XQueryExprBase: IBindableObject
    {
        private XQueryContext _queryContext;
        private bool? _isParameterSensitive;

        public XQueryExprBase(XQueryContext queryContext)
        {
            _queryContext = queryContext;
        }

        public abstract void Bind(Executive.Parameter[] parameters, MemoryPool pool);

        public abstract IEnumerable<FunctionLink> EnumDynamicFuncs();

        public virtual bool IsContextSensitive(Executive.Parameter[] parameters)
        {
            return false;
        }

        public virtual bool IsParameterSensitive()
        {
            if (!_isParameterSensitive.HasValue)
            {
                _isParameterSensitive = false;
                foreach (FunctionLink dynFunc in EnumDynamicFuncs())
                    if (QueryContext.Engine.IsParameterSensitive(dynFunc))
                    {
                        _isParameterSensitive = true;
                        break;
                    }
            }
            return _isParameterSensitive.Value;
        }

        public virtual void GetValueDependences(HashSet<Object> hs, Executive.Parameter[] parameters, bool reviewLambdaExpr, Action<SymbolLink> callback)
        {
            foreach (FunctionLink dynFunc in EnumDynamicFuncs())
                foreach (SymbolLink value in QueryContext.Engine.GetValueDependences(hs, parameters, null, dynFunc, reviewLambdaExpr))
                    callback(value);
        }

        public abstract object Execute(IContextProvider provider, object[] args, MemoryPool pool);

        public XQueryContext QueryContext
        {
            get
            {
                return _queryContext;
            }
        }

        public virtual object ToLispFunction()
        {
            return Lisp.List(ID.DynExecuteExpr, this, ID.Context, Lisp.ARGV, Lisp.MPOOL);
        }
    }
}
