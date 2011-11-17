//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

using DataEngine.CoreServices;
using DataEngine.XQuery.Collections;
using System.Globalization;

namespace DataEngine.XQuery
{
    internal abstract class XQueryFLWORBase : XQueryExprBase
    {
        internal object m_var;
        internal XQuerySequenceType m_varType;
        internal XQueryExprBase m_expr;
        internal XQueryExprBase m_bodyExpr;
        
        protected FunctionLink m_conditionExpr;
        public object ConditionExpr { get; set; }

        public XQueryFLWORBase(XQueryContext context, object var, 
            XQuerySequenceType varType, XQueryExprBase expr, XQueryExprBase bodyExpr)
            : base(context)
        {
            m_var = var;
            m_varType = varType;
            m_expr = expr;
            m_bodyExpr = bodyExpr;
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            List<FunctionLink> res = new List<FunctionLink>();
            res.AddRange(m_expr.EnumDynamicFuncs());
            if (m_conditionExpr != null)
                res.Add(m_conditionExpr);
            res.AddRange(m_bodyExpr.EnumDynamicFuncs());
            return res;
        }
    }

    internal sealed class XQueryFLWOR : XQueryFLWORBase
    {
        private object m_pos;                
        private SymbolLink m_value;
        private SymbolLink m_posValue;
        private Type m_itemType;
        private bool m_convert;        
        
        private bool m_join;
        private volatile bool m_dirty;        
        private List<KeyValuePair<XQueryExprBase, XQueryExprBase>> m_keyPair;
        private Dictionary<Key, ItemList> m_items;
        private Dictionary<Key, List<int>> m_posIndex;

        public bool Parallel { get; set; }
        public bool EnableHashJoin { get; set; }

        public XQueryFLWOR(XQueryContext context, object var, XQuerySequenceType varType, object pos, XQueryExprBase expr, XQueryExprBase bodyExpr, bool convert)
            : base(context, var, varType, expr, bodyExpr)
        {
            m_var = var;
            m_varType = varType;            
            m_pos = pos;
            m_value = new SymbolLink(varType.ValueType);
            m_itemType = varType.ItemType;
            if (m_pos != null)
                m_posValue = new SymbolLink(typeof(Integer));
            m_convert = convert;
            EnableHashJoin = true;
        }

        private bool IsJoinCondition(object expr, List<Object[]> pairs)
        {
            if (Lisp.IsFunctor(expr, ID.Par))
                return IsJoinCondition(Lisp.Second(expr), pairs);
            if (Lisp.IsFunctor(expr, ID.GeneralEQ))
            {
                if (pairs != null)
                    pairs.Add(new Object[] { 
                        Lisp.Second(expr), Lisp.Third(expr) });
                return true;
            }
            if (Lisp.IsFunctor(expr, Funcs.And))
                return IsJoinCondition(Lisp.Second(expr), pairs) &&
                   IsJoinCondition(Lisp.Third(expr), pairs);
            return false;
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            bool hasJoinCondition = false;
            List<Object[]> pairs = null;
            HashSet<SymbolLink> outerValues = null;
            if (EnableHashJoin && !Parallel && 
                ConditionExpr != null && QueryContext.EnableHashJoin)
            {
                pairs = new List<object[]>();
                if (IsJoinCondition(ConditionExpr, pairs))
                {
                    hasJoinCondition = true;
                    outerValues = new HashSet<SymbolLink>(QueryContext.Resolver.List());
                }
            }
            m_expr.Bind(parameters, pool);
            if (hasJoinCondition)
            { // Non-static dependences in iterator is not allowed in hash-join 
                if (m_expr.IsParameterSensitive())
                    hasJoinCondition = false;
                else
                    m_expr.GetValueDependences(null, parameters, false, (SymbolLink value) =>
                        { 
                            if (!value.IsStatic)
                                hasJoinCondition = false;
                        });
            }
            object data = QueryContext.Resolver.GetCurrentStack();
            pool.Bind(m_value);
            QueryContext.Resolver.SetValue(m_var, m_value);
            if (m_pos != null)
            {
                pool.Bind(m_posValue);
                QueryContext.Resolver.SetValue(m_pos, m_posValue);
            }
            if (ConditionExpr != null)
            {
                if (hasJoinCondition)
                {
                    m_join = true;
                    m_keyPair = new List<KeyValuePair<XQueryExprBase, XQueryExprBase>>();
                    foreach (Object[] expr in pairs)
                    {                        
                        XQueryExprBase expr1 = XQueryExpr.Create(QueryContext, expr[0]);
                        XQueryExprBase expr2 = XQueryExpr.Create(QueryContext, expr[1]);                        
                        expr1.Bind(parameters, pool);
                        bool key1 = false;
                        bool value1 = expr1.IsParameterSensitive();
                        expr1.GetValueDependences(null, parameters, false, (SymbolLink value) =>
                            {
                                if (value == m_value)
                                    key1 = true;
                                else if (m_posValue != null && value == m_posValue)
                                    value1 = true;
                                else
                                    if (!value.IsStatic && outerValues.Contains(value))
                                        value1 = true;

                            });                        
                        expr2.Bind(parameters, pool);
                        bool key2 = false;
                        bool value2 = expr2.IsParameterSensitive();
                        expr2.GetValueDependences(null, parameters, false, (SymbolLink value) =>
                            {
                                if (value == m_value)
                                    key2 = true;
                                else if (m_posValue != null && value == m_posValue)
                                    value2 = true;
                                else
                                    if (!value.IsStatic && outerValues.Contains(value))
                                        value2 = true;
                            });
                        KeyValuePair<XQueryExprBase, XQueryExprBase> keyPair;
                        if ((key1 && !value1) && !key2)
                            keyPair = new KeyValuePair<XQueryExprBase, XQueryExprBase>(expr1, expr2);
                        else
                            if (!key1 && (key2 && !value2))
                                keyPair = new KeyValuePair<XQueryExprBase, XQueryExprBase>(expr2, expr1);
                            else
                            {
                                keyPair = new KeyValuePair<XQueryExprBase, XQueryExprBase>(expr1, expr2);
                                m_join = false;
                            }
                        m_keyPair.Add(keyPair);
                    }
                    if (m_join)
                    {
                        m_items = new Dictionary<Key, ItemList>();
                        if (m_pos != null)
                            m_posIndex = new Dictionary<Key, List<int>>();
                        m_dirty = true;
                    }
                }
                else
                {
                    m_conditionExpr = new FunctionLink();
                    QueryContext.Engine.Compile(parameters, ConditionExpr, m_conditionExpr);
                }
            }
            m_bodyExpr.Bind(parameters, pool);
            QueryContext.Resolver.RevertToStack(data);
        }

        private void CreateDictionary(IContextProvider provider, Object[] args, MemoryPool pool)
        {
            XQueryNodeIterator[] iter = new XQueryNodeIterator[m_keyPair.Count];
            ItemList items;
            List<int> posIndex = null;
            XQueryNodeIterator.Combinator combinator;
            XQueryNodeIterator baseIter = XQueryNodeIterator.Create(m_expr.Execute(provider, args, pool));
            int index = 1;
            while (baseIter.MoveNext())
            {
                XPathItem curr = baseIter.Current;
                object value;
                if (baseIter.Current.IsNode)
                    value = curr;
                else
                    value = curr.TypedValue;
                if (m_varType != XQuerySequenceType.Item && m_convert)
                {
                    if (m_varType.IsNode && !Core.InstanceOf(QueryContext.Engine, value, m_varType))
                        throw new XQueryException(Properties.Resources.XPTY0004,
                           new XQuerySequenceType(curr.XmlType.TypeCode), m_varType);
                    value = XQueryConvert.TreatValueAs(value, m_varType);
                    if (m_varType.Cardinality == XmlTypeCardinality.ZeroOrMore ||
                        m_varType.Cardinality == XmlTypeCardinality.OneOrMore)
                        value = XQueryNodeIterator.Create(value);
                }
                pool.SetData(m_value, value);
                if (m_pos != null)
                    pool.SetData(m_posValue, new Integer(index));
                for (int k = 0; k < iter.Length; k++)
                    iter[k] = XQueryNodeIterator.Create(m_keyPair[k].Key.Execute(provider, args, pool));
                combinator = new XQueryNodeIterator.Combinator(iter);
                while (combinator.Next())
                {
                    Key keys = new Key(QueryContext.Engine, combinator.Current);
                    if (!m_items.TryGetValue(keys, out items))
                    {
                        keys.CloneItems();
                        items = new ItemList();
                        m_items[keys] = items;
                        if (m_posIndex != null)
                        {
                            posIndex = new List<int>();
                            m_posIndex[keys] = posIndex;
                        }
                    }
                    if (items.Tag < index) // Prevent duplicate items if keys is not unique
                    {
                        items.Add(curr);
                        items.Tag = index;
                        if (posIndex != null)
                            posIndex.Add(index);
                    }
                }
                index++;
            }
        }

        private object ExecuteHashJoin(IContextProvider provider, Object[] args, MemoryPool pool)
        {            
            if (m_dirty)
            {
                lock (m_items)
                {
                    if (m_dirty)
                    {                        
                        CreateDictionary(provider, args, pool);
                        m_dirty = false;
                    }
                }
            }
            XQueryNodeIterator[] iter = new XQueryNodeIterator[m_keyPair.Count];
            for (int k = 0; k < iter.Length; k++)
                iter[k] = XQueryNodeIterator.Create(m_keyPair[k].Value.Execute(provider, args, pool));
            XQueryNodeIterator.Combinator combinator = new XQueryNodeIterator.Combinator(iter);
            return new XQueryFLWORJoinIterator(this, provider, args, pool, combinator);
        }

        public override object Execute(IContextProvider provider, Object[] args, MemoryPool pool)
        {
            if (Parallel && QueryContext.EnableHPC)
                return new XQueryFLWORIteratorHPC(this, provider, args, pool, 
                    XQueryNodeIterator.Create(m_expr.Execute(provider, args, pool)));
            else
                if (m_join)
                    return ExecuteHashJoin(provider, args, pool);
                else
                    return new XQueryFLWORIterator(this, provider, args, pool, 
                        XQueryNodeIterator.Create(XQueryNodeIterator.Create(m_expr.Execute(provider, args, pool))));
        }

        private bool TestCondition(IContextProvider provider, object[] args, MemoryPool pool)
        {
            if (m_conditionExpr != null)
                return Core.BooleanValue(QueryContext.Engine.Apply(null, null, ConditionExpr, args, m_conditionExpr, pool));
            if (m_keyPair != null)
                foreach (KeyValuePair<XQueryExprBase, XQueryExprBase> keyPair in m_keyPair)
                    if (!Core.GeneralEQ(QueryContext.Engine, keyPair.Key.Execute(provider, args, pool),
                        keyPair.Value.Execute(provider, args, pool)))
                        return false;
            return true;
        }

        private bool MoveNext(IContextProvider provider, object[] args, MemoryPool pool, XPathItem curr, Integer index, out object res)
        {
            object value;
            if (curr.IsNode)
                value = curr;
            else
                value = curr.TypedValue;
            if (m_varType != XQuerySequenceType.Item && m_convert)
            {
                if (m_varType.IsNode && !Core.InstanceOf(QueryContext.Engine, value, m_varType))
                    throw new XQueryException(Properties.Resources.XPTY0004,
                       new XQuerySequenceType(curr.XmlType.TypeCode), m_varType);
                value = XQueryConvert.TreatValueAs(value, m_varType);
                if (m_varType.Cardinality == XmlTypeCardinality.ZeroOrMore ||
                    m_varType.Cardinality == XmlTypeCardinality.OneOrMore)
                    value = XQueryNodeIterator.Create(value);
            }
            pool.SetData(m_value, value);
            if (m_pos != null)
                pool.SetData(m_posValue, index);
            res = null;
            if (TestCondition(provider, args, pool))
            {
                res = m_bodyExpr.Execute(provider, args, pool);
                if (res != Undefined.Value)
                    return true;
            }
            return false;
        }

        private sealed class Key
        {
            private Executive engine;
            private XPathItem[] items;
            private int hashcode;

            public Key(Executive engine, XPathItem[] value)
            {
                this.engine = engine;
                items = new XPathItem[value.Length];
                Array.Copy(value, items, value.Length);
                hashcode = 0;
                for (int k = 0; k < items.Length; k++)
                    hashcode = hashcode << 6 ^ CalcHashCode(items[k]);
            }

            private int CalcHashCode(XPathItem item)
            {
                // Elimination of ambiguity of numbers: 
                //   values .1, 0.1 and 1e-01 is equals but have different string hashcodes.
                // So we give all the numerical values to double and attempt to convert all untyped atomic to doubles.
                object value = item.TypedValue;
                if (ValueProxy.IsNumeric(value.GetType()))
                    return Convert.ToDouble(value).GetHashCode();
                UntypedAtomic untypedAtomic = value as UntypedAtomic;
                if (untypedAtomic != null)
                {
                    double num;
                    if (untypedAtomic.TryParseDouble(out num))
                        return num.GetHashCode();

                }
                return value.GetHashCode();
            }

            public void CloneItems()
            {
                for (int k = 0; k < items.Length; k++)
                    items[k] = items[k].Clone();
            }

            public override int GetHashCode()
            {
                return hashcode;
            }

            public override bool Equals(object obj)
            {
                Key other = obj as Key;
                if (other == null || other.items.Length != items.Length)
                    return false;
                for (int k = 0; k < items.Length; k++)
                    if (!Core.GeneralEQ(engine, items[k], other.items[k]))
                        return false;
                return true;
            }
        }

        #region FLWOR Iterators
        private abstract class XQueryFLWORIteratorBase : XQueryNodeIterator
        {
            protected XQueryFLWOR owner;
            protected IContextProvider provider;
            protected object[] args;
            protected MemoryPool pool;
            protected XQueryNodeIterator baseIter;
            protected XQueryNodeIterator iter;
            protected XQueryNodeIterator childIter;
            protected CancellationToken token;

            public XQueryFLWORIteratorBase(XQueryFLWOR owner, IContextProvider provider, object[] args, MemoryPool pool, XQueryNodeIterator baseIter)
            {
                this.owner = owner;
                this.provider = provider;
                this.args = args;
                this.pool = pool;
                this.baseIter = baseIter;
                token = owner.QueryContext.Token;
            }

            public override XQueryNodeIterator CreateBufferedIterator()
            {
                return new BufferedNodeIterator(this);
            }

            protected override void Init()
            {
                iter = baseIter.Clone();
            }
        }

        private sealed class XQueryFLWORIterator : XQueryFLWORIteratorBase
        {
            private Integer index;
            private XQueryItem currItem = new XQueryItem();

            public XQueryFLWORIterator(XQueryFLWOR owner, IContextProvider provider, object[] args, MemoryPool pool, XQueryNodeIterator baseIter)
                : base(owner, provider, args, pool, baseIter)
            {
                index = 1;
            }

            public override XQueryNodeIterator Clone()
            {
                return new XQueryFLWORIterator(owner, provider, args, pool, baseIter);
            }

            protected override XPathItem NextItem()
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    if (childIter != null)
                    {
                        if (childIter.MoveNext())
                            return childIter.Current;
                        else
                            childIter = null;
                    }
                    if (!iter.MoveNext())
                        return null;
                    object res;
                    if (owner.MoveNext(provider, args, pool, iter.Current, index++, out res))
                    {
                        childIter = res as XQueryNodeIterator;
                        if (childIter == null)
                        {
                            XPathItem item = res as XPathItem;
                            if (item != null)
                                return item;
                            currItem.RawValue = res;
                            return currItem;
                        }
                    }
                }
            }
        }

        private sealed class XQueryFLWORIteratorHPC : XQueryFLWORIteratorBase
        {
            private ConcurrentDictionary<int, XQueryNodeIterator> orderedBag = new ConcurrentDictionary<int, XQueryNodeIterator>();
            private int index;
            private int length;

            public XQueryFLWORIteratorHPC(XQueryFLWOR owner, IContextProvider provider, object[] args, MemoryPool pool, XQueryNodeIterator baseIter)
                : base(owner, provider, args, pool, baseIter)
            {                
            }

            public override XQueryNodeIterator Clone()
            {
                return new XQueryFLWORIteratorHPC(owner, provider, args, pool, baseIter);
            }

            private XQueryNodeIterator CreateIterator(object value, long index)
            {
                XPathItem item = value as XPathItem;
                if (item != null)
                    return new SingleIterator(item.Clone());
                XQueryNodeIterator iter = value as XQueryNodeIterator;
                if (iter != null)
                    return BufferedNodeIterator.Preload(iter);
                return new SingleIterator(new XQueryItem(value));
            }

            protected override void Init()
            {
#if DEBUG
                Trace.WriteLine(String.Format("FLWOR HPC Init: {0}", owner.GetHashCode()));
#endif
                base.Init();
                index = 0;
                List<XPathItem> itemList = iter.ToList();
                length = itemList.Count;
                ParallelOptions options = new ParallelOptions();
                options.CancellationToken = token;
                ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();
                ParallelLoopResult result = System.Threading.Tasks.Parallel.ForEach(itemList, options, () => pool.Fork(), 
                    (XPathItem item, ParallelLoopState state, long i, MemoryPool localPool) =>
                    {
                        try
                        {
                            object val;
                            if (owner.MoveNext(provider, args, localPool, item, new Integer(i + 1), out val))
                                orderedBag[(int)i] = CreateIterator(val, i);                           
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(ex); 
                            state.Break();
                        }
                        return localPool;
                    }, 
                    (MemoryPool local) => { });
                if (exceptions.Count > 0)
                    owner.QueryContext.AggregateMultiplyException(exceptions.ToArray());
            }

            protected override XPathItem NextItem()
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    if (childIter == null)
                    {
                        if (index == length)
                            break;
                        orderedBag.TryGetValue(index++, out childIter);
                    }
                    else
                    {
                        if (childIter.MoveNext())
                            return childIter.Current;
                        else
                            childIter = null;
                    }
                }
                return null;
            }
        }

        private sealed class XQueryFLWORJoinIterator : XQueryNodeIterator
        {
            private XQueryFLWOR owner;
            private IContextProvider provider;
            private object[] args;
            private MemoryPool pool;
            private XQueryNodeIterator iter;
            private List<int> posIndex;
            private XQueryNodeIterator childIter;
            private CancellationToken token;
            private XQueryNodeIterator.Combinator combinator;
            private int index;

            private XQueryItem currItem = new XQueryItem();

            public XQueryFLWORJoinIterator(XQueryFLWOR owner, IContextProvider provider, object[] args, MemoryPool pool, 
                XQueryNodeIterator.Combinator combinator)
            {
                this.owner = owner;
                this.provider = provider;
                this.args = args;
                this.pool = pool;
                this.combinator = combinator;
                this.iter = EmptyIterator.Shared;
                token = owner.QueryContext.Token;
            }

            public override XQueryNodeIterator Clone()
            {
                return new XQueryFLWORJoinIterator(owner, provider, args, pool, combinator.Clone());
            }

            public override XQueryNodeIterator CreateBufferedIterator()
            {
                return new BufferedNodeIterator(this);
            }

            protected override XPathItem NextItem()
            {
                SymbolLink value = owner.m_value;
                SymbolLink pos = owner.m_posValue;
                XQueryExprBase bodyExpr = owner.m_bodyExpr;
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    if (childIter != null)
                    {
                        if (childIter.MoveNext())
                            return childIter.Current;
                        else
                            childIter = null;
                    }
                    while (!iter.MoveNext())
                    {
                        token.ThrowIfCancellationRequested();
                        if (!combinator.Next())
                            return null;
                        Key keys = new Key(owner.QueryContext.Engine, combinator.Current);
                        ItemList items;
                        if (owner.m_items.TryGetValue(keys, out items))
                        {
                            iter = items.CreateNodeIterator();
                            if (owner.m_posIndex != null)
                            { 
                                posIndex = owner.m_posIndex[keys];
                                index = 0;
                            }
                        }
                    }
                    XPathItem curr = iter.Current;
                    pool.SetData(value, curr.IsNode ? 
                        curr : curr.TypedValue);
                    if (posIndex != null)
                        pool.SetData(pos, new Integer(posIndex[index++]));
                    object res = bodyExpr.Execute(provider, args, pool);
                    childIter = res as XQueryNodeIterator;
                    if (childIter == null)
                    {
                        XPathItem item = res as XPathItem;
                        if (item != null)
                            return item;
                        currItem.RawValue = res;
                        return currItem;
                    }
                }
            }
        }
        #endregion

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Parallel)
                sb.Append("HPC: ");
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            sb.Append(Lisp.Format(m_var));
            sb.Append(" as ");
            sb.Append(m_varType.ToString());
            if (m_pos != null)
            {
                sb.Append(" at $");
                sb.Append(Lisp.Format(m_pos));
            }
            sb.Append(" := ");
            sb.Append(Lisp.Format(m_expr));
            if (ConditionExpr != null)
            {
                sb.Append(" where ");
                sb.Append(ConditionExpr.ToString());
            }
            sb.Append(" return ");
            sb.Append(m_bodyExpr.ToString());
            sb.Append("]");
            return sb.ToString();
        }
#endif
    }       
}
