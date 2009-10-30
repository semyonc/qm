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
using System.Xml.XPath;
using System.Xml.Schema;

namespace DataEngine.XQuery
{
    class VarTable
    {
        private struct Record
        {
            public object id;
            public XQuerySequenceType type;
            public bool external;
        }

        private Stack<Record> st = new Stack<Record>();

        public void PushVar(object id, XQuerySequenceType type)
        {
            PushVar(id, type, false);
        }

        public void PushVar(object id, XQuerySequenceType type, bool external)
        {
            Record node = new Record();
            node.id = id;
            node.type = type;
            node.external = external;
            st.Push(node);
        }

        public int BeginFrame()
        {
            return st.Count;
        }

        public void EndFrame(int count)
        {
            while (st.Count > count)
                st.Pop();
        }
        
        public XQuerySequenceType GetType(object id)
        {
            XQuerySequenceType type;
            bool external;
            if (GetType(id, out type, out external))
                return type;
            return null;
        }

        public bool GetType(object id, out XQuerySequenceType type, out bool external)
        {
            Record[] nodes = st.ToArray();
            for (int k = nodes.Length - 1; k >= 0; k--)
                if (nodes[k].id == id)
                {
                    type = nodes[k].type;
                    external = nodes[k].external;
                    return true;
                }
            type = null;
            external = false;
            return false;
        }

        public void ImportVariables(XQueryContext module)
        {
            foreach (XQueryContext.VariableRecord vr in module.variables)
                if (vr.module == module)
                    PushVar(vr.id, vr.varType);
            foreach (KeyValuePair<object, XQueryContext.ExternalVariableRecord> kvp in module.externalVars)
                PushVar(kvp.Key, kvp.Value.varType);
        }
    }
}
