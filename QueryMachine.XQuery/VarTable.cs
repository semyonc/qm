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
using System.Xml.XPath;
using System.Xml.Schema;

namespace DataEngine.XQuery
{
    public class VarTable
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
