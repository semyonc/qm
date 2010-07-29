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
using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    class RangeIterator: XQueryNodeIterator
    {
        private Integer _min;
        private Integer _max;
        private Integer _index;
        private XQueryItem _item;

        public RangeIterator(Integer min, Integer max)
        {
            _min = min;
            _max = max;
        }

        public override XQueryNodeIterator Clone()
        {
            return new RangeIterator(_min, _max);
        }

        public override int Count
        {
            get
            {
                Integer c = _max - _min + 1;
                return (int)Math.Max(0, (decimal)c);
            }
        }

        protected override void Init()
        {
            _index = _min;
        }

        protected override XPathItem NextItem()
        {
            if (_index <= _max)
            {
                if (_item == null)
                    _item = new XQueryItem();
                _item.RawValue = _index;
                _index++;
                return _item;
            }
            return null;
        }

        public override XQueryNodeIterator CreateBufferedIterator()
        {
            return Clone();
        }

        public override bool IsRange
        {
            get
            {
                return true;
            }
        }
    }
}
