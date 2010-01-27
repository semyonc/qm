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

namespace DataEngine.CoreServices
{
	public class HaltException: Exception 
	{
		const string haltMessage = "Internal error in executive";

		public HaltException() : 
			base (haltMessage)
		{}
	}

	public class ParseException: Exception 
	{
		public ParseException(string message, params object [] args) :
			base(String.Format(message, args))
		{}
	}

	public class ConnNotPerform: Exception
	{
		const string message = "Can not perform this operation on open connection";

		public  ConnNotPerform() :
			base (message)
		{}
	}

	public class NoDataSource: Exception
	{
		const string mesg = "No datasource set for current connection";

		public NoDataSource() :
			base (mesg)
		{}
	}

    public class UnknownFuncCall : Exception
    {
        const string mesg1 = "The name {0} is not binded";
        const string mesg2 = "No overloads found with {1} args for name {0}";

        public UnknownFuncCall(string name)  :
            base(String.Format(mesg1, name))
        { }

        public UnknownFuncCall(string name, int signature) :
            base(String.Format(mesg2, name, signature))
        { }
    }

    public class UnproperlyFormatedExpr : Exception
    {
        const string mesg = "Unproperly formated expression {0}";

        public UnproperlyFormatedExpr(string expr) : 
            base(String.Format(mesg, expr))
        {}
    }

    public class CannotOverrideThisFunc : Exception
    {
        const string mesg = "Override allowed only for dynamically (lambda) defined functions";

        public CannotOverrideThisFunc() :
            base(mesg)
        {}
    }

    public class ValueNotDefined : Exception
    {
        const string mesg = "Value not defined for atom '{0}'";

        public String Name { get; private set; }

        public ValueNotDefined(string name) :
            base(String.Format(mesg, name))
        {
            Name = name;
        }
    }

    public class ImproperlyFormat : Exception
    {
        const string mesg = "Improperly formated expression";

        public ImproperlyFormat() :
            base(mesg)
        { }
    }

    public class BadParameter : Exception
    {
        const string mesg = "Bad parameter {1} value {2} in function {0}";

        public BadParameter(string func, int param, string val) :
            base(String.Format(mesg, func, param, val)) 
        {}
    }

    public class LispStackOverflow : Exception
    {
        const string mesg = "Recursion limit exceed";

        public LispStackOverflow() :
            base(mesg)
        {}
    }
}
