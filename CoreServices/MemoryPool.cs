//        Copyright (c) 2009-2010, Semyon A. Chertkov (semyonc@gmail.com)
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
using System.Threading;

namespace DataEngine.CoreServices
{
    public class MemoryPool
    {
        public static readonly int InitPoolSize = 16;
        public static readonly int UnkID = -1;

        private static int s_maxID = 0;

        private int _id;
        private int _count;
        private bool _protected;
        private object[] _values;

        public MemoryPool()
        {
            _id = Interlocked.Increment(ref s_maxID);
            _values = new object[InitPoolSize];
        }

        public MemoryPool(MemoryPool src)
        {
            _id = src._id;
            _values = new object[src._values.Length];
            Array.Copy(src._values, 0, _values, 0, src._values.Length);
            _protected = true;
            src._protected = true;
        }

        private void EnsureCapacity()
        {
            if (_values.Length < _count)
            {
                int num = (_values.Length == 0) ? 4 : (_values.Length * 2);
                if (num < _count)
                    num = _count;
                object[] dest = new object[num];                
                Array.Copy(_values, 0, dest, 0, _values.Length);
                _values = dest;
            }
        }

        public void Bind(SymbolLink link)
        {
            if (link == null)
                throw new ArgumentNullException("link");
            if (_protected)
                throw new InvalidOperationException("the memory pool is protected for binding");
            if (link.pool_id != UnkID)
                throw new ArgumentException("the symbol link is already binded to the memory pool");
            link.pool_id = _id;
            link.index = _count++;
            EnsureCapacity();
        }

        private void CheckSymbolLink(SymbolLink link)
        {
            if (link.pool_id == UnkID)
                throw new ArgumentException("the symbol link is not binded");
            if (link.pool_id != _id)
                throw new ArgumentException("the symbol link is binded to other memory pool");
        }

        public object GetData(SymbolLink link)
        {
            CheckSymbolLink(link);
            return _values[link.index];
        }

        public void SetData(SymbolLink link, object value)
        {
            CheckSymbolLink(link);
            _values[link.index] = value;
            link.ChangeValue(this);
        }

        public MemoryPool Clone()
        {
            return new MemoryPool(this);
        }       
    }
}
