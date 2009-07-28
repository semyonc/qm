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

    class XQuerySorter : XQueryExprBase
    {
        private bool m_stable;
        private XQueryOrderSpec[] m_spec;

        public XQuerySorter(XQueryContext context, XQueryOrderSpec[] spec, bool stable)
            : base(context)
        {
            m_stable = stable;
            m_spec = spec;
        }

        private IEnumerable<XPathItem> CreateEnumerable(List<XPathItem> items)
        {
            foreach (XQueryWrappedValue item in items)
                yield return item.Inner;
        }

        public override XQueryNodeIterator Execute(object[] parameters)
        {
            XQueryNodeIterator iter = (XQueryNodeIterator)parameters[0];
            List<XPathItem> buffer = new List<XPathItem>();
            foreach (XPathItem item in iter)
                buffer.Add(item);
            XQueryComparer comparer = new XQueryComparer(m_spec);
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
            sb.Append("]");
            return sb.ToString();
        }
#endif

        public class XQueryComparer : IComparer<XPathItem>
        {
            private XQueryOrderSpec[] _spec;

            public XQueryComparer(XQueryOrderSpec[] spec)
            {
                _spec = spec;
            }

            #region IComparer<XPathItem> Members

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
                    if (a != null || b != null)
                    {
                        if (a == null)
                            return _spec[k].emptySpec == XQueryEmptyOrderSpec.Greatest
                                ? 1 : -1;
                        else if (b == null)
                            return _spec[k].emptySpec == XQueryEmptyOrderSpec.Greatest
                                ? -1 : 1;
                        else
                        {
                            TypeCode typecode = TypeConverter.GetTypeCode(a, b);
                            object val1 = TypeConverter.ChangeType(a, typecode);
                            object val2 = TypeConverter.ChangeType(b, typecode);
                            int res = ((IComparable)val1).CompareTo(val2);
                            if (res != 0)
                                if (_spec[k].direction == XQueryOrderDirection.Descending)
                                    return -1 * res;
                                else
                                    return res;
                        }
                    }
                }
                return 0;
            }

            #endregion
        }
    }
}
