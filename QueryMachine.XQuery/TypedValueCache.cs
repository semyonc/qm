//        Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Text;
using System.Threading;

namespace DataEngine.XQuery
{
    public sealed class TypedValueCache
    {
        private class ValueSlot
        {
            public int index;
            public object value;

            public ValueSlot(int index, object value)
            {
                this.index = index;
                this.value = value;
            }
        }

        private int _size;
        private ValueSlot[] _slots;


#if DEBUG
        public static int hitCount;
        public static int missCount;
#endif

        public TypedValueCache(int size)
        {
            _size = PrimeHelper.GetPrime(size);
            _slots = new ValueSlot[_size];
        }

        public object this[int pos]
        {
            get
            {
                ValueSlot sl = _slots[pos.GetHashCode() % _size];
                if (sl != null && sl.index == pos)
                {
#if DEBUG
                    Interlocked.Increment(ref hitCount);
#endif
                    return sl.value;
                }
                else
                {
#if DEBUG
                    Interlocked.Increment(ref missCount);
#endif
                    return null;
                }
            }
            set
            {
                _slots[pos.GetHashCode() % _size] = new ValueSlot(pos, value);                
            }
        }

        // Borrow from System.Collection.Generic
        public static class PrimeHelper
        {
            private static readonly int[] primes = new int[] { 
                3, 7, 11, 0x11, 0x17, 0x1d, 0x25, 0x2f, 0x3b, 0x47, 0x59, 0x6b, 0x83, 0xa3, 0xc5, 0xef, 
                0x125, 0x161, 0x1af, 0x209, 0x277, 0x2f9, 0x397, 0x44f, 0x52f, 0x63d, 0x78b, 0x91d, 0xaf1, 
                0xd2b, 0xfd1, 0x12fd, 0x16cf, 0x1b65, 0x20e3, 0x2777, 0x2f6f, 0x38ff, 0x446f, 0x521f, 0x628d, 
                0x7655, 0x8e01, 0xaa6b, 0xcc89, 0xf583, 0x126a7, 0x1619b, 0x1a857, 0x1fd3b, 0x26315, 0x2dd67, 0x3701b, 
                0x42023, 0x4f361, 0x5f0ed, 0x72125, 0x88e31, 0xa443b, 0xc51eb, 0xec8c1, 0x11bdbf, 0x154a3f, 0x198c4f, 
                0x1ea867, 0x24ca19, 0x2c25c1, 0x34fa1b, 0x3f928f, 0x4c4987, 0x5b8b6f, 0x6dda89
            };

            private static bool IsPrime(int candidate)
            {
                if ((candidate & 1) == 0)
                    return candidate == 2;
                int num = (int)Math.Sqrt((double)candidate);
                for (int i = 3; i <= num; i += 2)
                {
                    if ((candidate % i) == 0)
                        return false;
                }
                return true;
            }

            public static int GetPrime(int min)
            {
                for (int i = 0; i < primes.Length; i++)
                {
                    int curr = primes[i];
                    if (curr >= min)
                        return curr;
                }
                for (int j = min | 1; j < 0x7fffffff; j += 2)
                {
                    if (IsPrime(j))
                        return j;
                }
                return min;
            }
        }
    }
}
