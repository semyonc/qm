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

        public override bool IsOrderedSet
        {
            get
            {
                return true;
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
