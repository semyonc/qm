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
using System.Collections;

using System.Xml;
using System.Xml.Schema;


namespace DataEngine.XQuery.DocumentModel
{
    internal sealed class DmNodeList : IEnumerable
    {
        private ICollection<DmNode> nodes;

        public DmNodeList(ICollection<DmNode> nodes)
        {
            this.nodes = nodes;
        }

        public int Count
        {
            get
            {
                return nodes.Count;
            }
        }

        public DmNode Item(int index)
        {
            if (index >= 0)
            {
                IEnumerator<DmNode> e = nodes.GetEnumerator();
                while (e.MoveNext())
                {
                    if (index == 0)
                        return e.Current;
                    index--;
                }
            }
            return null;
        }


        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        #endregion
    }
}