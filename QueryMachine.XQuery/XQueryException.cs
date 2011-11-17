//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataEngine.XQuery
{
    public class XQueryException: Exception
    {
		protected XQueryException (SerializationInfo info, StreamingContext context) : base (info, context) {}

		public XQueryException (string message, Exception innerException) : base (message, innerException) {}

		internal XQueryException (string message) : base (message, null) {}

        internal XQueryException(string message, params object[] args) : base(String.Format(message, args), null) { }
    }
}
