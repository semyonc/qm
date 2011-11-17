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

namespace DataEngine.XQuery
{
    internal class XQueryWrappedValue : XQueryItemBase
    {
        public XQueryWrappedValue(XPathItem inner, object[] annotation)
        {
            Inner = inner.Clone();
            Annotation = annotation;
        }

        public override XPathItem Clone()
        {
            return new XQueryWrappedValue(Inner, Annotation);
        }

        public XPathItem Inner { get; private set;  }

        public object[] Annotation { get; private set; }

        public override bool IsNode
        {
            get { throw new NotImplementedException(); }
        }

        public override object TypedValue
        {
            get { throw new NotImplementedException(); }
        }

        public override string Value
        {
            get { throw new NotImplementedException(); }
        }

        public override object ValueAs(Type returnType, IXmlNamespaceResolver nsResolver)
        {
            throw new NotImplementedException();
        }

        public override bool ValueAsBoolean
        {
            get { throw new NotImplementedException(); }
        }

        public override DateTime ValueAsDateTime
        {
            get { throw new NotImplementedException(); }
        }

        public override double ValueAsDouble
        {
            get { throw new NotImplementedException(); }
        }

        public override int ValueAsInt
        {
            get { throw new NotImplementedException(); }
        }

        public override long ValueAsLong
        {
            get { throw new NotImplementedException(); }
        }

        public override Type ValueType
        {
            get { throw new NotImplementedException(); }
        }

        public override XmlSchemaType XmlType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
