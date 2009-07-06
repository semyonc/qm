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
    /// <summary>
    /// Implemention lisp pattern recognition.
    /// We using generalized NFA-based algoritm.
    /// </summary>
    public class LispRecognizer
    {
        const int EndState = Int32.MaxValue;
        
        static readonly object EPS = new Lisp.ATOM("EPS");
        static readonly object LBR = new Lisp.ATOM("(");
        static readonly object RBR = new Lisp.ATOM(")");

        public static readonly object ANY_P = new Lisp.ATOM("?");
        public static readonly object CONS_P = new Lisp.ATOM("?cons");
        public static readonly object NCONS_P = new Lisp.ATOM("?_");
        public static readonly object ATOM_P = new Lisp.ATOM("?a");
        public static readonly object NUMBER_P = new Lisp.ATOM("?n");
        public static readonly object STRING_P = new Lisp.ATOM("?s");
        public static readonly object DATE_P = new Lisp.ATOM("?d");

        public static readonly object OPTNOTREPEAT = new Lisp.ATOM("p0");
        public static readonly object REQREPEAT = new Lisp.ATOM("p+");
        public static readonly object OPTREPEAT = new Lisp.ATOM("p*");
        public static readonly object BRANCH_P = new Lisp.ATOM("p:");        

        
        enum Relation { AND, OR }; 
              
        class NFAState
        {
            public int      stateno;
            public object   symbol;
            public int      stateno1;
            public NFAState nextstate;
        }

        class NFATable
        {
            private List<NFAState> m_states;
            private int m_maxState;
            private NFAState[] m_orderstates;

            public NFATable()
            {
                m_states = new List<NFAState>();
                Clear();
            }

            /// <summary>
            /// Clear table contents
            /// </summary>
            public void Clear()
            {
                m_states.Clear();
                m_orderstates = null;
                m_maxState = -1;
            }

            /// <summary>
            /// Index NFA table by state number.
            /// Also update references to nextstate.
            /// </summary>
            public void MakeIndex()
            {
                m_orderstates = new NFAState[m_maxState + 1];
                for (int stateno = 0; stateno <= m_maxState; stateno++)
                {
                    NFAState r0 = null;
                    foreach (NFAState r in m_states)
                    {
                        if (r.stateno == stateno)
                        {
                            if (r0 == null)
                                m_orderstates[stateno] = r;
                            else
                                r0.nextstate = r;
                            r0 = r;
                        }
                    }
                }
            }

            /// <summary>
            /// Count of states in table
            /// </summary>
            public int Count
            {
                get
                {
                    return m_states.Count;
                }
            }

            /// <summary>
            /// Indexer for sequantial access to states
            /// </summary>
            /// <param name="index">Index of state</param>
            /// <returns>NFAState object</returns>
            public NFAState this[int index]
            {
                get
                {
                    return m_states[index];
                }
            }

            /// <summary>
            /// Gets the state by stateno. 
            /// This method works only if m_orderstates was build
            ///    in previos MakeIndex() call.
            /// </summary>
            /// <param name="stateno">State number</param>
            /// <returns>NFAState object</returns>
            public NFAState GetState(int stateno)
            {
                if (m_orderstates != null && stateno >= 0 && stateno <= m_maxState)
                    return m_orderstates[stateno];
                return null;
            }

            /// <summary>
            /// Gets the next state number
            /// </summary>
            /// <returns>Next state number</returns>
            public int GetNextState()
            {
                return m_maxState + 1;
            }

            /// <summary>
            /// Create new state in NFA table, 
            /// that references to some next states
            /// </summary>
            /// <param name="stateno">State number</param>
            /// <param name="Symbol">Symbol</param>
            /// <param name="NextStates">Array of the next states</param>
            public void NewState(int stateno, object Symbol, params int [] NextStates)
            {
                for (int i = 0; i < NextStates.Length; i++)
                {
                    NFAState r = new NFAState();
                    r.stateno = stateno;
                    r.symbol = Symbol;
                    r.stateno1 = NextStates[i];
                    r.nextstate = null;
                    m_states.Add(r);
                }
                if (stateno > m_maxState)
                    m_maxState = stateno;
            }

            /// <summary>
            /// Move state
            /// </summary>
            /// <param name="stateno">State number</param>
            /// <param name="Symbol">Symbol</param>
            /// <param name="S">int stack</param>
            public void Move(int stateno, object Symbol, Stack<int> S, LispRecognizer recognizer)
            {
                NFAState r = GetState(stateno);
                while (r != null)
                {
                    if (recognizer.IsEqual(r.symbol, Symbol) && !S.Contains(r.stateno1))
                        S.Push(r.stateno1);
                    r = r.nextstate;
                }
            }

            /// <summary>
            /// Insert new state number 0 into NFA table 
            /// </summary>
            public void InsertState()
            {
                foreach (NFAState r in m_states)
                {
                    r.stateno++;
                    if (r.stateno > m_maxState)
                        m_maxState = r.stateno;
                    if (r.stateno1 != EndState)
                        r.stateno1++;
                }
            }

            /// <summary>
            /// Append new state to NFA table and connect it
            /// </summary>
            public void AppendState()
            {
                int stateno = GetNextState();
                foreach (NFAState r in m_states)
                    if (r.stateno1 == EndState)
                        r.stateno1 = stateno;
            }

            /// <summary>
            /// Copy contents of NFA table
            /// </summary>
            /// <param name="src">Source table</param>
            /// <returns>Begining state number of first copied state</returns>
            public int Copy(NFATable src)
            {
                int stateno = GetNextState();
                foreach (NFAState r in src.m_states)
                {
                    if (r.stateno1 != EndState)
                        NewState(r.stateno + stateno, r.symbol, r.stateno1 + stateno);
                    else
                        NewState(r.stateno + stateno, r.symbol, EndState);
                }
                return stateno;
            }

            public void CompileAND(NFATable src)
            {
                AppendState();
                Copy(src);
            }

            public void CompileOR(NFATable src)
            {
                InsertState();
                NewState(0, EPS, 1);
                AppendState();
                NewState(GetNextState(), EPS, EndState);
                src.AppendState();
                src.NewState(src.GetNextState(), EPS, EndState);
                NewState(0, EPS, Copy(src));
            }

            public void CompileOptNotRepeat()
            {
                InsertState();
                NewState(0, EPS, 1, EndState);
            }

            public void CompileOptRepeat()
            {
                AppendState();
                NewState(GetNextState(), EPS, 0, EndState);
                InsertState();
                NewState(0, EPS, 1, EndState);
            }

            public void CompileReqRepeat()
            {
                AppendState();
                NewState(GetNextState(), EPS, 0, EndState);
            }
        }
                
        virtual public bool IsEqual(object metasymbol, object symbol)
        {
            if (metasymbol == ATOM_P && Lisp.IsAtom(symbol) ||
                metasymbol == NUMBER_P &&
                  (symbol is System.Int16 || symbol is System.Int32 ||
                   symbol is System.Int64 || symbol is System.Single ||
                   symbol is System.Double || symbol is System.Decimal ||
                   symbol is System.SByte || symbol is System.UInt16 ||
                   symbol is System.UInt32 || symbol is System.UInt64 ||
                   symbol is System.Byte) ||
                metasymbol == STRING_P && symbol is System.String ||
                metasymbol == DATE_P && symbol is System.DateTime ||
                metasymbol == NCONS_P && symbol != LBR && symbol != RBR)
                return true;
            else
                return Lisp.IsEqual(metasymbol, symbol);
        }
        
        private void Closure(Stack<int> S1, out Stack<int> S2, NFATable t)
        {
            S2 = new Stack<int>(S1);
            while (S1.Count > 0)
            {
                NFAState r = t.GetState(S1.Pop());
                while (r != null)
                {
                    if (Lisp.IsEqual(r.symbol, EPS) && !S2.Contains(r.stateno1))
                    {
                        S2.Push(r.stateno1);
                        S1.Push(r.stateno1);
                    }
                    r = r.nextstate;
                }
            }
        }

        private void OneStep(Stack<int> S1, ref Stack<int> S2, NFATable t, object o)
        {
            while (S2.Count > 0)
                t.Move(S2.Pop(), o, S1, this);
            Closure(S1, out S2, t);
        }

        private void ProduceList(Stack<int> S1, ref Stack<int> S2, NFATable t, object lval)
        {
            OneStep(S1, ref S2, t, LBR);
            foreach (object o in Lisp.getIterator(lval))
                if (Lisp.IsCons(o))
                    ProduceList(S1, ref S2, t, o);
                else
                    OneStep(S1, ref S2, t, o);
            OneStep(S1, ref S2, t, RBR);
        }

        private bool Produce(object expr, NFATable grammar)
        {
            Stack<int> S1 = new Stack<int>();
            Stack<int> S2;
            S1.Push(0);
            Closure(S1, out S2, grammar);
            if (Lisp.IsCons(expr))
                ProduceList(S1, ref S2, grammar, expr);
            else
                OneStep(S1, ref S2, grammar, expr);
            return S2.Contains(EndState);
        }

        private void CompileList(object lval, NFATable t, Relation rel)
        {
            foreach (object o in Lisp.getIterator(lval))
            {
                NFATable t1 = CompileExpr(o);
                if (rel == Relation.AND)
                    t.CompileAND(t1);
                else
                    t.CompileOR(t1);
            }
        }
       
        private NFATable CompileExpr(object expr)
        {
            NFATable t = new NFATable();
            if (Lisp.IsEqual(expr, ANY_P))
            {
                t.NewState(0, EPS, 1);
                t.NewState(1, NCONS_P, EndState);
                t.NewState(0, EPS, 2);
                t.NewState(2, LBR, 3);
                t.NewState(3, EPS, 4);
                t.NewState(3, EPS, 5);
                t.NewState(5, NCONS_P, 3);
                t.NewState(5, LBR, 3);
                t.NewState(4, RBR, 6);
                t.NewState(6, EPS, EndState);
                t.NewState(6, EPS, 3);
            }
            else if (Lisp.IsEqual(expr, CONS_P))
            {
                t.NewState(0, LBR, 1);
                t.NewState(1, EPS, 2);
                t.NewState(1, EPS, 3);
                t.NewState(3, NCONS_P, 1);
                t.NewState(3, LBR, 1);
                t.NewState(2, RBR, 4);
                t.NewState(4, EPS, EndState);
                t.NewState(4, EPS, 1);
            }
            else if (Lisp.IsFunctor(expr, OPTNOTREPEAT))
            {
                CompileList(Lisp.Cdr(expr), t, Relation.AND);
                t.CompileOptNotRepeat();
            }
            else if (Lisp.IsFunctor(expr, REQREPEAT))
            {
                CompileList(Lisp.Cdr(expr), t, Relation.AND);
                t.CompileReqRepeat();
            }
            else if (Lisp.IsFunctor(expr, OPTREPEAT))
            {
                CompileList(Lisp.Cdr(expr), t, Relation.AND);
                t.CompileOptRepeat();
            }
            else if (Lisp.IsFunctor(expr, BRANCH_P))
                CompileList(Lisp.Cdr(expr), t, Relation.OR);
            else if (Lisp.IsCons(expr))
            {
                t.NewState(0, LBR, EndState);
                CompileList(expr, t, Relation.AND);
                NFATable t1 = new NFATable();
                t1.NewState(0, RBR, EndState);
                t.CompileAND(t1);
            }
            else
                t.NewState(0, expr, EndState);
            return t;
        }

        private NFATable m_grammar;
       
        public LispRecognizer(object pattern)
        {
            m_grammar = CompileExpr(pattern);
            m_grammar.MakeIndex();
        }

        public bool Match(object expr)
        {            
            return Produce(expr, m_grammar);
        }

        private void _InternalFind(object expr, bool fDeep, List<object> mathes)
        {
            if (Match(expr))
            {
                mathes.Add(expr);
                if (!fDeep)
                    return;
            }
            foreach (object o in Lisp.getIterator(expr))
                _InternalFind(o, fDeep, mathes);
        }

        public object[] FindAll(object expr, bool fDeep)
        {
            List<object> res = new List<object> ();
            _InternalFind(expr, fDeep, res);

            return res.ToArray();
        }

        public object[] FindAll(object expr)
        {
            return FindAll(expr, false);
        }

        public static bool Match(object pattern, object expr)
        {
            LispRecognizer r = new LispRecognizer(pattern);
            return r.Match(expr);
        }

    }
}
