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

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    sealed class XQueryValueExpr: XQueryExprBase
    {
        private object name;
        private int arg_index;
        private SymbolLink value;

        public XQueryValueExpr(XQueryContext queryContext, object name)
            : base(queryContext)
        {
            this.name = name;
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            value = QueryContext.Engine.TryGet(name, false, true);
            if (value == null)
            {
                if (parameters != null)
                {
                    arg_index = 0;
                    foreach (Executive.Parameter p in parameters)
                    {
                        if (p.ID == name)
                            return;
                        arg_index++;
                    }
                }
                value = QueryContext.Engine.TryGet(name, true, false);
                if (value == null)
                    throw new ValueNotDefined(name.ToString());
            }
        }

        public override bool IsParameterSensitive()
        {
            return value == null;
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            return new FunctionLink[0];
        }

        public override void GetValueDependences(HashSet<Object> hs, Executive.Parameter[] parameters, 
            bool reviewLambdaExpr, Action<SymbolLink> callback)
        {
            if (value != null)
                callback(value);
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            if (value == null)
                return args[arg_index];
            return pool.GetData(value);
        }

        public override object ToLispFunction()
        {
            return name;
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            sb.Append(name.ToString());
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }
}
