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
using System.Globalization;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;

namespace DataEngine.XQuery
{

    public enum XQueryEmptyOrderSpec
    {
        Default,
        Greatest,
        Least
    }

    public enum XQueryOrderDirection
    {
        Ascending,
        Descending
    }

    public struct XQueryOrderSpec
    {
        public XQueryOrderDirection direction;
        public XQueryEmptyOrderSpec emptySpec;
        public String collation;
    }

    sealed class XQuerySorter : XQueryExprBase
    {
        private bool m_stable;
        private XQueryOrderSpec[] m_spec;
        private XQueryExprBase m_bodyExpr;

        public XQuerySorter(XQueryContext context, XQueryOrderSpec[] spec, bool stable, XQueryExprBase bodyExpr)
            : base(context)
        {
            m_stable = stable;
            m_spec = spec;
            m_bodyExpr = bodyExpr;
        }

        private IEnumerable<XPathItem> CreateEnumerable(List<XPathItem> items)
        {
            foreach (XQueryWrappedValue item in items)
                yield return item.Inner;
        }

        public override void Bind(Executive.Parameter[] parameters, MemoryPool pool)
        {
            m_bodyExpr.Bind(parameters, pool);
        }

        public override IEnumerable<FunctionLink> EnumDynamicFuncs()
        {
            return m_bodyExpr.EnumDynamicFuncs();
        }

        public override object Execute(IContextProvider provider, object[] args, MemoryPool pool)
        {
            XQueryNodeIterator iter = XQueryNodeIterator.Create(m_bodyExpr.Execute(provider, args, pool));
            List<XPathItem> buffer = new List<XPathItem>();
            foreach (XPathItem item in iter)
                buffer.Add(item);
            XQueryComparer comparer = new XQueryComparer(QueryContext, m_spec);
            if (m_stable)
                BubbleSort(buffer, comparer); // Stable but slowly sort
            else
                buffer.Sort(comparer);
            return new NodeIterator(CreateEnumerable(buffer));
        }

        private void BubbleSort(List<XPathItem> buffer, XQueryComparer comparer)
        {
            for (int i = buffer.Count -1; i >= 0; i--)
                for (int j = 0; j < buffer.Count -1; j++)
                    if (comparer.Compare(buffer[j], buffer[j + 1]) > 0)
                    {
                        XPathItem item = buffer[j];
                        buffer[j] = buffer[j + 1];
                        buffer[j + 1] = item;
                    }
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(base.ToString());
            sb.Append(": ");
            sb.Append(m_bodyExpr.ToString());
            sb.Append("]");
            return sb.ToString();
        }
#endif

        public class XQueryComparer : IComparer<XPathItem>
        {
            private XQueryOrderSpec[] _spec;
            private CultureInfo[] _culture;

            public XQueryComparer(XQueryContext context, XQueryOrderSpec[] spec)
            {
                _spec = spec;
                _culture = new CultureInfo[spec.Length];
                for (int k = 0; k < spec.Length; k++)
                    _culture[k] = context.GetCulture(spec[k].collation);
            }

            #region IComparer<XPathItem> Members

            public int CompareString(string s1, string s2, CultureInfo culture)
            {
                if (culture == null)
                    return String.Compare(s1, s2, StringComparison.Ordinal);
                else
                    return String.Compare(s1, s2, culture, CompareOptions.None);
            }

            public int Compare(XPathItem x, XPathItem y)
            {
                object[] key1 = ((XQueryWrappedValue)x).Annotation;
                object[] key2 = ((XQueryWrappedValue)y).Annotation;
                if (key1 == null || key2 == null)
                    throw new ArgumentException();
                if (key1.Length != _spec.Length || key2.Length != _spec.Length)
                    throw new ArgumentException();                
                for (int k = 0; k < _spec.Length; k++)
                {
                    object a = key1[k];
                    object b = key2[k];
                    int scale = _spec[k].direction == XQueryOrderDirection.Descending 
                        ? -1 : 1;
                    if (a is Double && Double.IsNaN((double)a))
                        a = Undefined.Value;
                    if (b is Double && Double.IsNaN((double)b))
                        b = Undefined.Value;
                    if (a != Undefined.Value || b != Undefined.Value)
                    {
                        if (a == Undefined.Value)
                            return scale * (_spec[k].emptySpec == XQueryEmptyOrderSpec.Greatest ? 1 : -1);
                        else if (b == Undefined.Value)
                            return scale * (_spec[k].emptySpec == XQueryEmptyOrderSpec.Greatest ? -1 : 1);
                        else
                        {
                            if (a.GetType() == b.GetType())
                            {
                                int res;
                                if (a is String)
                                    res = CompareString((string)a, (string)b, _culture[k]);
                                else
                                    res = ((IComparable)a).CompareTo(b);
                                if (res != 0)
                                    return scale * res;
                            }   
                            else
                            {                                
                                object val1;
                                object val2;
                                int res;
                                Type type = ValueProxy.GetType(a.GetType(), b.GetType());
                                if (type == typeof(System.Object))
                                {
                                    val1 = XQueryConvert.ToString(a);
                                    val2 = XQueryConvert.ToString(b);
                                    res = CompareString((string)val1, (string)val2, _culture[k]);
                                }
                                else
                                {
                                    val1 = Convert.ChangeType(a, type);
                                    val2 = Convert.ChangeType(b, type);
                                    res = ((IComparable)val1).CompareTo(val2);
                                }                                
                                if (res != 0)
                                    return scale * res;
                            }
                        }
                    }
                }
                return 0;
            }

            #endregion
        }
    }
}
