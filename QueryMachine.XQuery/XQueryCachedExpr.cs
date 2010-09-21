//        Copyright (c) 2009-2010, Semyon A. Chertkov (semyonc@gmail.com)
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
using DataEngine.XQuery.Util;


namespace DataEngine.XQuery
{
    sealed class XQueryCachedExpr: XQueryExprBase
    {
        private XQueryExpr m_body;
        private List<SymbolLink> m_params;
        private Dictionary<Key, object> m_cache;

        public XQueryCachedExpr(XQueryContext context, XQueryExpr body)
            : base(context)
        {
            m_body = body;
            m_cache = new Dictionary<Key, object>();
        }
        
        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            HashSet<SymbolLink> outerValues = new HashSet<SymbolLink>(QueryContext.Resolver.List());
            m_body.Bind(parameters, pool);            
            m_params = new List<SymbolLink>();
            m_body.GetValueDependences(null, parameters, false, (SymbolLink value) =>
            {
                if (!value.IsStatic && outerValues.Contains(value))
                    m_params.Add(value);
            });
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            return m_body.EnumDynamicFuncs();
        }

        public override bool IsContextSensitive(Executive.Parameter[] parameters)
        {
            return m_body.IsContextSensitive(parameters);
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            object[] keys = new object[m_params.Count + 1];
            keys[0] = provider.Context;
            for (int k = 0; k < m_params.Count; k++)
                keys[k + 1] = pool.GetData(m_params[k]);
            Key key = new Key(QueryContext.Engine, keys);
            object res;
            lock (m_cache)
            {
                if (m_cache.TryGetValue(key, out res))
                    return res.CloneObject();
            }
            res = m_body.Execute(provider, args, pool);
            lock (m_cache)
            {
                if (!m_cache.ContainsKey(key))
                {
                    key.CloneKeys();
                    m_cache.Add(key, res);
                }
            }
            return res.CloneObject();
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            sb.Append(m_body.ToString());
            sb.Append("]");
            return sb.ToString();
        }
#endif
        
        private sealed class Key
        {
            private Executive engine;
            private object[] keys;
            private int hashcode;

            public Key(Executive engine, object[] keys)
            {
                this.engine = engine;
                this.keys = keys;
                hashcode = CalcHashCode();
            }

            public void CloneKeys()
            {
                for (int k = 0; k < keys.Length; k++)
                    keys[k] = keys[k].CloneObject();
            }
           
            private int CalcHashCode()
            {
                int hashCode = 0;
                for (int k = 0; k < keys.Length; k++)
                {
                    object a = keys[k];
                    if (a == null)
                        continue;
                    else if (a is XQueryNavigator)
                    {
                        XQueryNavigator nav = (XQueryNavigator)a;
                        hashCode = hashCode << 6 ^ nav.Position;
                    }
                    else if (a is XPathItem)
                    {
                        XPathItem item = (XPathItem)a;
                        hashCode = hashCode << 6 ^ item.Value.GetHashCode();
                    }
                    else if (a is XQueryNodeIterator)
                    {
                        XQueryNodeIterator iter = (XQueryNodeIterator)a;
                        iter = iter.Clone();
                        foreach (XPathItem item in iter)
                        {
                            if (item.IsNode)
                            {
                                XQueryNavigator nav = item as XQueryNavigator;
                                if (nav != null)
                                    hashCode = hashCode << 6 ^ nav.Position;
                                else
                                    hashCode = hashCode << 6 ^ 
                                        XPathNavigator.NavigatorComparer.GetHashCode(item);
                            }
                            else
                                hashCode = hashCode << 6 ^ item.Value.GetHashCode();
                        }
                    }
                    else
                        hashCode = hashCode << 6 ^ a.GetHashCode();
                }
                return hashCode;
            }

            public override int GetHashCode()
            {
                return hashcode;
            }

            public override bool Equals(object obj)
            {
                Key other = obj as Key;
                if (other == null || other.keys.Length != keys.Length)
                    return false;
                for (int k = 0; k < keys.Length; k++)
                {
                    object a = keys[k];
                    object b = other.keys[k];
                    if (a == null || b == null)
                    {
                        if (a != b)
                            return false;
                    }
                    else if (a.GetType() != b.GetType())
                        return false;
                    else if (a is XQueryNavigator && b is XQueryNavigator)
                    {
                        XQueryNavigator nav1 = (XQueryNavigator)a;
                        XQueryNavigator nav2 = (XQueryNavigator)b;
                        if (nav1.Position != nav2.Position)
                            return false;
                    }
                    else if (a is XQueryNodeIterator && b is XQueryNodeIterator)
                    {
                        if (!XQueryFuncs.DeepEqual((XQueryNodeIterator)a, (XQueryNodeIterator)b))
                            return false;
                    }
                    else
                    {
                        if (a is XPathItem)
                            a = ((XPathItem)a).TypedValue;
                        if (b is XPathItem)
                            b = ((XPathItem)b).TypedValue;
                        if (engine.OperatorEq(a, b) == null)
                            return false;
                    }
                }
                return true;
            }
        }
    }
}
