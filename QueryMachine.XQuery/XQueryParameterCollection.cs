//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections;

namespace DataEngine.XQuery
{
    public class XQueryParameterCollection : CollectionBase
    {
        public XQueryParameter this[int index]
        {
            get
            {
                return (XQueryParameter)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(XQueryParameter value)
        {
            return List.Add(value);
        }

        public int IndexOf(XQueryParameter value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, XQueryParameter value)
        {
            List.Insert(index, value);
        }

        public void Remove(XQueryParameter value)
        {
            List.Remove(value);
        }

        public bool Contains(XQueryParameter value)
        {
            return List.Contains(value);
        }

        protected override void OnValidate(Object value)
        {
            if (value.GetType() != typeof(XQueryParameter))
                throw new ArgumentException("value must be of type XQueryParameter.", "value");
        }
    }
}
