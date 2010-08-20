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
