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
using System.Runtime.Serialization;

namespace DataEngine
{
	[Serializable]
	public class ESQLException: SystemException
	{
		protected ESQLException (SerializationInfo info, StreamingContext context) : base (info, context) {}

		public ESQLException (string message, Exception innerException) : base (message, innerException) {}

        public ESQLException(Exception innerException, string message, params object[] args) : base(String.Format(message, args), innerException) { }

		internal ESQLException (string message) : base (message, null) {}

        internal ESQLException(string message, params object[] args) : base(String.Format(message, args), null) { }

	}
}
