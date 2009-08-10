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
using System.Reflection;
using System.Reflection.Emit;

using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public class XQuerySignatureAttribute : Attribute
    {
        public XQuerySignatureAttribute(string name)
        {
            NamespaceUri = XmlReservedNs.NsXQueryFunc;
            Name = name;
            Return = XmlTypeCode.None;
            Cardinality = XmlTypeCardinality.One;
        }

        public String NamespaceUri { get; set; }

        public String Name { get; set; }

        public XmlTypeCode Return { get; set; }

        public XmlTypeCardinality Cardinality { get; set; }

        public bool VariableParams { get; set; }
    }

    public class XQueryParameterAttribute : Attribute
    {
        public XQueryParameterAttribute(XmlTypeCode typeCode)
        {
            TypeCode = typeCode;
            Cardinality = XmlTypeCardinality.One;
        }

        public XmlTypeCode TypeCode { get; set; }

        public XmlTypeCardinality Cardinality { get; set; }
    }

    public class XQueryFunctionRecord
    {
        public object id;
        public XQuerySequenceType[] parameters;
        public XQuerySequenceType returnType;
        public bool variableParams;
    }

    public class XQueryFunctionTable
    {
        protected class FunctionSocket
        {
            public XQueryFunctionRecord rec;
            public FunctionSocket next;

            public FunctionSocket(XQueryFunctionRecord rec)
            {
                this.rec = rec;
            }
        }

        private Dictionary<object, FunctionSocket> m_table = new Dictionary<object, FunctionSocket>();

        public XQueryFunctionTable()
        {
        }

        private XQueryFunctionTable(Type baseClass)
        {
            MethodInfo[] methods = baseClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                object[] attrs = method.GetCustomAttributes(typeof(XQuerySignatureAttribute), false);
                if (attrs.Length > 0)
                {
                    XQuerySignatureAttribute sig = (XQuerySignatureAttribute)attrs[0];
                    object id = Lisp.Defatom(sig.NamespaceUri, new string[] { sig.Name }, true);
                    Register(id, method);
                }
            }
        }

        public void Register(object id, MethodInfo method)
        {
            XQueryFunctionRecord rec = new XQueryFunctionRecord();
            rec.id = id;
            object[] attrs = method.GetCustomAttributes(typeof(XQuerySignatureAttribute), false);
            XQuerySignatureAttribute sig = null;
            if (attrs.Length > 0)
                sig = (XQuerySignatureAttribute)attrs[0];
            ParameterInfo[] parameter_info = method.GetParameters();
            List<XQuerySequenceType> type_list = new List<XQuerySequenceType>();
            foreach (ParameterInfo pi in parameter_info)
            {
                object[] pi_attrs = pi.GetCustomAttributes(false);
                if (pi_attrs.Length > 0)
                {
                    if (pi_attrs[0] is ImplictAttribute && pi.Position == 0)
                        continue;
                    else if (pi_attrs[0] is XQueryParameterAttribute)
                    {
                        XQueryParameterAttribute xattr = (XQueryParameterAttribute)pi_attrs[0];
                        type_list.Add(new XQuerySequenceType(xattr.TypeCode, xattr.Cardinality, pi.ParameterType));
                        continue;
                    }
                }                
                if (pi.ParameterType == typeof(XQueryNodeIterator))
                    type_list.Add(new XQuerySequenceType(XmlTypeCode.Item, XmlTypeCardinality.ZeroOrMore, pi.ParameterType));
                else
                    type_list.Add(new XQuerySequenceType(pi.ParameterType, XmlTypeCardinality.One));
            }
            if (sig != null && sig.Return != XmlTypeCode.None)
                rec.returnType = new XQuerySequenceType(sig.Return, sig.Cardinality, method.ReturnType);
            else
            {
                if (method.ReturnType == typeof(XQueryNodeIterator))
                    rec.returnType = new XQuerySequenceType(XmlTypeCode.Item, XmlTypeCardinality.ZeroOrMore, method.ReturnType);
                else
                    rec.returnType = new XQuerySequenceType(method.ReturnType, XmlTypeCardinality.One);
            }
            if (sig != null)
                rec.variableParams = sig.VariableParams;
            rec.parameters = type_list.ToArray();
            FunctionSocket sock = new FunctionSocket(rec);
            FunctionSocket next;
            if (m_table.TryGetValue(rec.id, out next))
                sock.next = next;
            m_table[rec.id] = sock;
            GlobalSymbols.DefineStaticOperator(rec.id, method);
        }

        public bool IsRegistered(object id, int arity)
        {
            FunctionSocket curr;
            if (m_table.TryGetValue(id, out curr))
                while (curr != null)
                {
                    if (curr.rec.parameters.Length == arity)
                        return true;
                    curr = curr.next;
                }
            return false;
        }

        public void Register(object id, XQuerySequenceType[] parameters, XQuerySequenceType resType)
        {
            XQueryFunctionRecord rec = new XQueryFunctionRecord();
            rec.id = id;
            rec.parameters = parameters;
            rec.returnType = resType;
            FunctionSocket sock = new FunctionSocket(rec);
            FunctionSocket next;
            if (m_table.TryGetValue(rec.id, out next))
                sock.next = next;
            m_table[rec.id] = sock;
        }

        public XQueryFunctionRecord GetRecord(object id, int arity)
        {
            FunctionSocket s;
            if (m_table.TryGetValue(id, out s))
                while (s != null)
                {
                    if (s.rec.parameters.Length == arity ||
                        s.rec.variableParams)
                        return s.rec;
                    s = s.next;
                }
            return null;
        }

        public XQueryFunctionRecord GetRecord(object expr)
        {
            return GetRecord(Lisp.First(expr), Lisp.Length(expr) - 1);
        }

        public void CopyFrom(XQueryFunctionTable src)
        {
            foreach (KeyValuePair<object, FunctionSocket> kvp in src.m_table)
                if (!m_table.ContainsKey(kvp.Key))
                    m_table.Add(kvp.Key, kvp.Value);
        }

        #region Static methods
        private static XQueryFunctionTable shared;

        static XQueryFunctionTable()
        {
            if (shared == null)
                shared = new XQueryFunctionTable(typeof(CliFuncs));
        }

        public static void Register(object id, Type type, string methodName)
        {
            bool bfound = false;
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in methods)
                if (method.Name == methodName && !method.IsGenericMethod)
                {
                    shared.Register(id, method);
                    bfound = true;
                }
            if (!bfound)
                throw new ArgumentException("Static method not found in class", methodName);
        }

        public static XQueryFunctionTable CreateInstance()
        {
            XQueryFunctionTable res = new XQueryFunctionTable();
            foreach (KeyValuePair<object, FunctionSocket> kvp in shared.m_table)
                res.m_table.Add(kvp.Key, kvp.Value);
            return res;
        }
        #endregion
    }
}
