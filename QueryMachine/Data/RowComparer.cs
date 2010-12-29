/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2009  Semyon A. Chertkov (semyonc@gmail.com)

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataEngine.CoreServices.Data
{
    public abstract class RowComparer : IComparer<Row>
    {
        #region IComparer<Row> Members

        public abstract int Compare(Row x, Row y);

        #endregion

        public abstract int Compare(Object[] x, Row y);
    }

    public sealed class IndexComparer : RowComparer
    {
        private int[] _ordinals;
        private int[] _way;

        public IndexComparer(int[] ordinals)
            : this(ordinals, null)
        {
        }

        public IndexComparer(int[] ordinals, int[] way)
        {
            _ordinals = ordinals;

            if (_way == null)
            {
                _way = new int[ordinals.Length];
                for (int k = 0; k < _way.Length; k++)
                    _way[k] = 1;
            }
            else
            {
                _way = way;
                if (_way.Length != _ordinals.Length)
                    throw new InvalidOperationException();
            }
        }

        public override int Compare(Row x, Row y)
        {
            for (int k = 0; k < _ordinals.Length; k++)
            {
                int result = Row.CompareValue(x[_ordinals[k]], y[_ordinals[k]]);
                if (result != 0)
                    return result * _way[k];
            }
            return 0;
        }

        public override int Compare(Object[] x, Row y)
        {
            for (int k = 0; k < _ordinals.Length; k++)
            {
                int result = Row.CompareValue(x[k], y[_ordinals[k]]);
                if (result != 0)
                    return result * _way[k];
            }
            return 0;
        }
    }

    public sealed class ComplexComparer : RowComparer
    {
        public RowType.Locator[] _locator;
        public int[] _way;

        public ComplexComparer(RowType.Locator[] locator)
            : this(locator, null)
        {
        }

        public ComplexComparer(RowType.Locator[] locator, int[] way)
        {
            _locator = locator;

            if (way == null)
            {
                _way = new int[_locator.Length];
                for (int k = 0; k < _way.Length; k++)
                    _way[k] = 1;
            }
            else
            {
                _way = way;
                if (_way.Length != _locator.Length)
                    throw new InvalidOperationException();
            }
        }

        public override int Compare(Row x, Row y)
        {
            for (int k = 0; k < _locator.Length; k++)
            {
                if (_locator[k].detail == null)
                {
                    int result = Row.CompareValue(x[_locator[k].master], y[_locator[k].master]);
                    if (result != 0)
                        return result * _way[k];
                }
                else
                {
                    Row r1 = (Row)x.GetObject(_locator[k].master);
                    Row r2 = (Row)y.GetObject(_locator[k].master);

                    object val1 = r1 != null ?
                        r1[_locator[k].detail.Value] : null;
                    object val2 = r2 != null ?
                        r2[_locator[k].detail.Value] : null;

                    int result = Row.CompareValue(val1, val2);
                    if (result != 0)
                        return result * _way[k];
                }
            }
            return 0;
        }

        public override int Compare(Object[] x, Row y)
        {
            for (int k = 0; k < _locator.Length; k++)
            {
                if (_locator[k].detail == null)
                {
                    int result = Row.CompareValue(x[k], y[_locator[k].master]);
                    if (result != 0)
                        return result * _way[k];
                }
                else
                {
                    Row r = (Row)y.GetObject(_locator[k].master);
                    object val = r != null ?
                        r[_locator[k].detail.Value] : null;

                    int result = Row.CompareValue(x[k], val);
                    if (result != 0)
                        return result * _way[k];
                }
            }
            return 0;
        }
    }

    public class ReferencingComparer<T> : IEqualityComparer<T>
    {
        #region IEqualityComparer<T> Members

        public bool Equals(T x, T y)
        {
            return Object.ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }

    public sealed class RowComplexEqualityComparer : IEqualityComparer<Row>
    {
        private RowType.Locator[] _locator;

        public RowComplexEqualityComparer(RowType.Locator[] locator)
        {
            _locator = locator;
        }

        #region IEqualityComparer<Row> Members

        public bool Equals(Row x, Row y)
        {
            if (x._type == y._type)
            {
                for (int k = 0; k < _locator.Length; k++)
                {
                    if (_locator[k].detail == null)
                    {
                        if (Row.CompareValue(x[_locator[k].master], y[_locator[k].master]) != 0)
                            return false;
                    }
                    else
                    {
                        Row r1 = (Row)x.GetObject(_locator[k].master);
                        Row r2 = (Row)y.GetObject(_locator[k].master);

                        object val1 = r1 != null ?
                            r1[_locator[k].detail.Value] : null;
                        object val2 = r2 != null ?
                            r2[_locator[k].detail.Value] : null;

                        if (Row.CompareValue(val1, val2) != 0)
                            return false;
                    }
                }
                return true;
            }
            else
                return false;
        }

        public int GetHashCode(Row obj)
        {
            int hash = 23; // Josh Bloch's "Effective Java"
            for (int k = 0; k < _locator.Length; k++)
            {
                if (_locator[k].detail == null)
                    hash = hash * 37 + obj[_locator[k].master].GetHashCode();
                else
                {
                    Row r = (Row)obj.GetObject(_locator[k].master);
                    if (r != null)
                        hash = hash * 37 + r[_locator[k].detail.Value].GetHashCode();
                }
            }
            return hash;
        }
        #endregion
    }

}