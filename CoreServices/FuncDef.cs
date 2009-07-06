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

namespace DataEngine.CoreServices
{
    internal class FuncDef
    {        
        private LinkedList<FuncBase> m_def;
        
        public FuncDef(FuncBase func)
        {
            m_def = new LinkedList<FuncBase>();
            m_def.AddFirst(func);
        }

        public void Overload(FuncBase func)
        {
            LinkedListNode<FuncBase> node = m_def.First;
            while (node != null)
            {
                if (node.Value.Name.Equals(func))
                {
                    node.Value = func;
                    return;
                }
                node = node.Next;
            }                    
            m_def.AddLast(func);
        }

        public object ID
        {
            get
            {
                return m_def.First.Value.Name.ID;
            }
        }

        public FuncBase FindMatched(FuncName name, bool untyped)
        {
            foreach (FuncBase func in m_def)
                if (func.Name.Match(name, untyped))
                    return func;
            return null;
        }
    }

    internal class MacroFuncDef: IEnumerable<MacroFuncBase>
    {
        private LinkedList<MacroFuncBase> m_def;

        public MacroFuncDef(MacroFuncBase func)
        {
            m_def = new LinkedList<MacroFuncBase>();
            m_def.AddFirst(func);
        }

        public void Overload(MacroFuncBase func)
        {
            LinkedListNode<MacroFuncBase> node = m_def.First;
            while (node != null)
            {
                if (node.Value.Name.Equals(func))
                {
                    node.Value = func;
                    return;
                }
                node = node.Next;
            }
            m_def.AddLast(func);
        }

        public object ID
        {
            get
            {
                return m_def.First.Value.Name.ID;
            }
        }

        #region IEnumerable<MacroFuncBase> Members

        public IEnumerator<MacroFuncBase> GetEnumerator()
        {
            return m_def.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_def.GetEnumerator();
        }

        #endregion
    }
}
