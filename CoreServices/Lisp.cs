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
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace DataEngine.CoreServices
{
	public class Lisp
	{        
		internal class CONS
		{
			public object car;
			public object cdr;

			internal CONS (object car, object cdr)
			{ 
				this.car = car;
				this.cdr = cdr;
			}

            public override string ToString()
            {
                return Lisp.Format(this);
            }
        }

        public static readonly object T = ATOM.Create("t");
        public static readonly object NIL = ATOM.Create("nil");
        public static readonly object QUOTE = ATOM.Create("quote");
        public static readonly object LAMBDA = ATOM.Create("lambda");
        public static readonly object INST = ATOM.Create("__inst");
        public static readonly object UNKNOWN = ATOM.Create("unknown");
        public static readonly object ARGV = ATOM.Create("argv");
        public static readonly object MPOOL = ATOM.Create("memoryPool");
        
		public static bool IsNode(object lval) 
		{
			return lval == null || !(lval is CONS);
		}

        public static bool IsAtom(object lval)
        {
            return lval != null && lval is ATOM;
        }
        
        public static bool IsNull(object lval)
        {
            return lval == null;
        }

        public static bool IsValue(object lval)
        {
            return IsNode(lval) && !IsAtom(lval);
        }

        public static bool IsAtom(object lval, string s)
        {
            return IsAtom(lval) && lval.ToString() == s;
        }

        public static bool IsAtomText(object lval, string s)
        {
            return IsAtom(lval) && lval.ToString().ToLower() == s.ToLower();
        }

        public static bool IsT(object lval)
        {
            return lval != null && lval != DBNull.Value;
        }

        public static bool IsFunctor(object lval)
        {
            return !IsNode(lval) && IsAtom(Car(lval));
        }

		public static bool IsFunctor(object lval, string s)
		{
			return !IsNode(lval) && IsAtom(Car(lval), s);
		}

        public static bool IsFunctor(object lval, object a)
        {
            return !IsNode(lval) && IsAtom(Car(lval)) && IsEqual(Car(lval), a);
        }

        public static bool IsFunctor(object lval, object a, int arity)
        {
            return !IsNode(lval) && IsAtom(Car(lval)) && IsEqual(Car(lval), a) && Length(lval) == arity + 1;
        }

        public static string SName(object lval)
        {
            return Nth(lval, 0).ToString();
        }

		public static string SArg1(object lval)
		{
			return Nth(lval, 1).ToString();
		}

		public static string SArg2(object lval)
		{
			return Nth(lval, 2).ToString();
		}

		public static string SArg3(object lval)
		{
			return Nth(lval, 3).ToString();
		}

        public static object Arg1(object lval)
        {
            return Nth(lval, 1);
        }

        public static object Arg2(object lval)
        {
            return Nth(lval, 2);
        }

        public static object Arg3(object lval)
        {
            return Nth(lval, 3);
        }

        public static bool ExpFunc(object expr, out object func, out object [] vargs)
        {            
            func = Lisp.Car(expr);
            if (Lisp.IsAtom(func))
            {
                object l = Lisp.Cdr(expr);
                ArrayList args = new ArrayList();
                while (l != null)
                {
                    args.Add(Lisp.Car(l));
                    l = Lisp.Cdr(l);
                }
                vargs = args.ToArray();
                return true;
            }
            else
            {
                vargs = null;
                return false;
            }
        }

        public class LispIterator<T> : IEnumerable<T>
        {
            private CONS lval;

            public LispIterator(object lval)
            {
                this.lval = (CONS)lval;
            }

            private class LispEnumerator : IEnumerator<T>
            {
                private object _list;
                private object _tail;
                private object _head;

                public LispEnumerator(CONS list)
                {
                    _list = list;
                    Reset();
                }

                #region IEnumerator<T> Members

                public T Current
                {
                    get { return (T)_head; }
                }

                #endregion

                #region IDisposable Members

                public void Dispose()
                {

                }

                #endregion

                #region IEnumerator Members

                object IEnumerator.Current
                {
                    get { return _head; }
                }

                public bool MoveNext()
                {
                    if (_tail != null)
                    {
                        _head = Lisp.Car(_tail);
                        _tail = Lisp.Cdr(_tail);
                        return true;
                    }
                    else
                        return false;
                }

                public void Reset()
                {
                    _head = null;
                    _tail = _list;
                }

                #endregion
            }

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return new LispEnumerator(lval);
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new LispEnumerator(lval);
            }

            #endregion
        }

        public static IEnumerable getIterator(object expr)
        {
            return new LispIterator<object>(expr);
        }

        public static IEnumerable<T> getIterator<T>(object expr)
        {
            return new LispIterator<T>(expr);
        }

        public static int Length(object expr)
        {
            int res = 0;
            while (expr != null)
            {
                res++;
                expr = Lisp.Cdr(expr);
            }
            return res;
        }

        public static T[] ToArray<T>(object expr)
        {
            List<T> args = new List<T>();
            while (expr != null)
            {
                args.Add((T)Car(expr));
                expr = Lisp.Cdr(expr);
            }
            return args.ToArray();
        }

        public static object[] ToArray(object expr)
        {
            List<object> args = new List<object>();
            while (expr != null)
            {
                args.Add(Car(expr));
                expr = Lisp.Cdr(expr);
            }
            return args.ToArray();
        }

        public static Dictionary<object, object> ToDictionary(object expr)
        {
            Dictionary<object, object> res = new Dictionary<object, object>();
            foreach (object o in getIterator(expr))
                res.Add(Car(o), Cdr(o));
            return res;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(object expr)
        {
            Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();
            foreach (object o in getIterator(expr))
                res.Add((TKey)Car(o),(TValue)Cdr(o));
            return res;
        }

        public static object Functor(object id, object tail)
        {
            return Lisp.Append(Lisp.Cons(id), tail);
        }

        public static object Functor(object id, object[] args)
        {
            if (args == null)
                return Lisp.Cons(id);
            else
            {
                object[] f = new object[args.Length + 1];
                f[0] = id;
                Array.Copy(args, 0, f, 1, args.Length);
                return Lisp.List(f);
            }
        }
  
        public static int getFunctorRank(object expr)
        {
            int result = 0;
            expr = Lisp.Cdr(expr);
            while (expr != null)
            {
                result++;
                expr = Lisp.Cdr(expr);
            }
            return result;
        }

		public static bool IsCons(object lval)
		{
			return lval is CONS;
		}

        public static bool IsConsPair(object lval)
        {
            if (lval is CONS && Cdr(lval) != null)
                return IsNode(Car(lval)) && IsNode(Cdr(lval));
            return false;
        }

        public static bool IsNilPair(object lval)
        {
            return lval is CONS && Car(lval) == null && Cdr(lval) == null;
        }

		public static object Cons(object car)
		{
			return Cons(car, null);
		}

		public static object Cons(object car, object cdr)
		{
			return new CONS(car, cdr);
		}

        public static object List(params object[] lvals)
        {
            object result = null;
            for (int i = lvals.Length - 1; i >= 0; i--)
                result = Cons(lvals[i], result);
            return result;
        }

        public static object List(List<object> items)
        {
            object result = null;
            for (int i = items.Count - 1; i >= 0; i--)
                result = Cons(items[i], result);
            return result;
        }

        public delegate bool LispNodeDelegate(object a);

        public static bool IsTrueForAll(object lval, LispNodeDelegate del)
        {
            if (IsNode(lval))
                return del(lval);
            else
            {
                while (lval != null)
                {
                    if (! del(Car(lval)))
                        return false;
                    lval = Cdr(lval);
                }
                return true;
            }
        }

        public static bool IsTrueForAny(object lval, LispNodeDelegate del)
        {
            if (IsNode(lval))
                return del(lval);
            else
            {
                while (lval != null)
                {
                    if (del(Car(lval)))
                        return true;
                    lval = Cdr(lval);
                }
                return false;
            }
        }

        public static T Car<T>(object lval)
        {
            return (T)(lval as CONS).car;
        }
        
		public static object Car(object lval)
		{
			return (lval as CONS).car;
		}

        public static T Cdr<T>(object lval)
        {
            return (T)(lval as CONS).cdr;
        }

		public static object Cdr(object lval)
		{
			return (lval as CONS).cdr;
		}

		public static object Caar(object lval)
		{
			return Car(Car(lval));
		}

		public static object Cadr(object lval)
		{
			return Cdr(Car(lval));
		}

		public static object Cdar(object lval)
		{ 
			return Car(Cdr(lval));
		}

		public static object Cddr(object lval)
		{
			return Cdr(Cdr(lval));
		}

		public static object Caaar(object lval)
		{
			return Car(Car(Car(lval)));
		}

		public static object Caadr(object lval)
		{
			return Cdr(Car(Car(lval)));
		}

		public static object Caddr(object lval)
		{
			return Cdr(Cdr(Car(lval)));
		}

		public static object Cdddr(object lval)
		{
			return Cdr(Cdr(Cdr(lval)));
		}

		public static object Cddar(object lval)
		{
			return Cdr(Cdr(Car(lval)));
		}
		
		public static object Cdaar(object lval)
		{
			return Cdr(Car(Car(lval)));
		}

		public static object Cadar(object lval)
		{
			return Car(Cdr(Car(lval)));
		}

		public static object Cdadr(object lval)
		{
			return Cdr(Car(Cdr(lval)));
		}

        public static T First<T>(object lval)
        {
            return Nth<T>(lval, 0);
        }

        public static T Second<T>(object lval)
        {
            return Nth<T>(lval, 1);
        }

        public static T Third<T>(object lval)
        {
            return Nth<T>(lval, 2);
        }

        public static T Fourth<T>(object lval)
        {
            return Nth<T>(lval, 3);
        }

        public static T Fifth<T>(object lval)
        {
            return Nth<T>(lval, 4);
        }

        public static T Sixth<T>(object lval)
        {
            return Nth<T>(lval, 5);
        }

        public static T Seventh<T>(object lval)
        {
            return Nth<T>(lval, 6);
        }

        public static T Eighth<T>(object lval)
        {
            return Nth<T>(lval, 7);
        }

        public static T Ninth<T>(object lval)
        {
            return Nth<T>(lval, 8);
        }

        public static T Tenth<T>(object lval)
        {
            return Nth<T>(lval, 9);
        }

        public static object First(object lval)
		{
			return Nth(lval, 0);
		}

		public static object Second(object lval)
		{
			return Nth(lval, 1);
		}

		public static object Third(object lval)
		{
			return Nth(lval, 2);
		}

		public static object Fourth(object lval)
		{
			return Nth(lval, 3);
		}

		public static object Fifth(object lval)
		{
			return Nth(lval, 4);
		}

        public static object Sixth(object lval)
		{
			return Nth(lval, 5);
		}

		public static object Seventh(object lval)
		{
			return Nth(lval, 6);
		}

		public static object Eighth(object lval)
		{
			return Nth(lval, 7);
		}

		public static object Ninth(object lval)
		{
			return Nth(lval, 8);
		}

		public static object Tenth(object lval)
		{
			return Nth(lval, 9);
		}

		public static object Nthx(object lval, int index)
		{
			object result = lval;
			while (index > 0)
			{
				if (result == null)
					throw new Exception("Invalid index in Nthx");
				result = Cdr(result);
				index --;
			}
			return result;
		}

        public static T Nth<T>(object lval, int index)
        {
            return Car<T>(Nthx(lval, index));
        }

		public static object Nth(object lval, int index)
		{
			return Car(Nthx(lval, index));
		}

		public static object NthCdr(object lval, int index)
		{
			return Cdr(Nthx(lval, index));
		}

		public static void Rplaca(object lval, object car)
		{
			(lval as CONS).car = car;
		}

		public static void Rplacd(object lval, object cdr)
		{
			(lval as CONS).cdr = cdr;
		}

		public static object Copy(object lval, bool deep)
		{
			if (IsNode(lval))
				return lval;
			else if (IsConsPair(lval))
				return Cons(Car(lval), Cdr(lval));
			else
			{
				object result = null;
				for (object tail = null; lval != null; lval = Cdr(lval))
					if (tail == null)
					{
						result = Cons(deep ? Copy(Car(lval), deep) : Car(lval));
						tail = result;
					}
					else
					{
						Rplacd(tail, Cons(deep ? Copy(Car(lval), deep) : Car(lval)));
						tail = Cdr(tail);
					}
				return result;
			}
		}

        public static object Replace(object old_value, object new_value, object lval)
        {
            if (lval == null)
                return null;
            if (IsNode(lval))
                return Object.ReferenceEquals(lval, old_value) ? new_value : lval;
            else
            {
                object result = null;
                for (object tail = null; lval != null; lval = Cdr(lval))
                    if (tail == null)
                    {
                        result = Cons(Replace(old_value, new_value, Car(lval)));
                        tail = result;
                    }
                    else
                    {
                        Rplacd(tail, Cons(Replace(old_value, new_value, Car(lval))));
                        tail = Cdr(tail);
                    }
                return result;
            }
        }

        public static object Subst(object lval, Dictionary<object, object> s)
        {
            if (s.ContainsKey(lval))
                return s[lval];
            else if (IsNode(lval))
                return lval;
            else if (IsConsPair(lval))
                return Cons(Car(Subst(lval, s)), Cdr(Subst(lval, s)));
            else
            {
                object result = null;
                for (object tail = null; lval != null; lval = Cdr(lval))
                    if (s.ContainsKey(lval))
                    {
                        if (tail == null)
                            result = s[lval];
                        else
                            Rplacd(tail, s[lval]);
                        break;
                    }
                    else
                        if (tail == null)
                        {
                            result = Cons(Subst(Car(lval), s));
                            tail = result;
                        }
                        else
                        {
                            Rplacd(tail, Cons(Subst(Car(lval), s)));
                            tail = Cdr(tail);
                        }
                return result;
            }
        }

		public static object Copy(object lval)
		{
			return Copy(lval, false);
		}

		public static object Last(object lval)
		{
			while (Cdr(lval) != null)
				lval = Cdr(lval);
			return Lisp.Car(lval);
        }

        public static object LastCdr(object lval)
        {
            while (Cdr(lval) != null)
                lval = Cdr(lval);
            return lval;
        }

		public static object Append(object lval1, object lval2)
		{
			if (lval1 == null)
				return lval2;
			else
			{
				Rplacd(LastCdr(lval1), lval2);
				return lval1;
			}
		}

		public static object Reverse(object lval)
		{
			object curr = null;
			while (lval != null)
			{
				curr = Cons(Car(lval), curr);
				lval = Cdr(lval);
			}
			return curr;
		}

        public static object Remove(object lval, object o)
        {
            object previos = null;
            object head = lval;            
            while (lval != null)
            {
                if (Car(lval).Equals(o))
                {
                    if (previos != null)
                        Rplacd(previos, Cdr(lval));
                    else
                        head = Cdr(lval);
                    return head;
                }
                previos = lval;
                lval = Cdr(lval);
            }
            return head;
        }

		public static bool IsEqual(object o1, object o2)
		{
			if (o1 == null || o2 == null)	
				return (o1 == null && o2 == null);
			else
				if (IsNode(o1) || IsNode(o2))
			{
				if (IsNode(o1) && IsNode(o2))
					return o1.Equals(o2);
				else
					return false;
			}
			else
				if (IsCons(o1) && IsCons(o2))
					return IsEqual(Car(o1), Car(o2)) && IsEqual(Cdr(o1), Cdr(o2));
			else 
				return false;
		}

        public static void GetTerms(object lval, List<object> terms)
        {
            if (Lisp.IsNode(lval))
            {
                if (Lisp.IsAtom(lval))
                    terms.Add(lval);
            }
            else
                while (lval != null)
                {
                    GetTerms(Car(lval), terms);
                    lval = Cdr(lval);
                }
        }

        public static object[] GetTerms(object lval)
        {
            List<object> res = new List<object>();
            GetTerms(lval, res);
            return res.ToArray();
        }

        private static object True = true;

        public static String EscapeString(string value)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\"");
            int num = 0;
            int startIndex = 0;
            while (num < value.Length)
            {
                char ch = value[num];
                if (ch < ' ' || ch == '"')
                {
                    if (builder == null)
                        builder = new StringBuilder(value.Length + 4);
                    if (num - startIndex > 0)
                        builder.Append(value, startIndex, num - startIndex);
                    startIndex = num + 1;
                    switch (ch)
                    {
                        case '\t':
                            builder.Append("\\t");
                            break;

                        case '\n':
                            builder.Append("\\n");
                            break;

                        case '\r':
                            builder.Append("\\r");
                            break;

                        case '"':
                            builder.Append("\\\"");
                            break;

                        default:
                            builder.Append(ch);
                            break;
                    }
                    
                }
                num++;
            }
            if (builder == null)
                return value;
            if ((num - startIndex) > 0)
                builder.Append(value, startIndex, num - startIndex);
            builder.Append("\"");
            return builder.ToString();
        }


		public static String Format(object lval)
		{
			if (IsNode(lval))
			{
				if (lval == null)
					return "null";
				else
					if (lval == CoreServices.Generation.RuntimeOps.True)
					return "t";
				else
					if (lval is String)
					  return EscapeString((string)lval);
				else
					if (lval is System.Decimal)
					  return ((decimal)lval).ToString(NumberFormatInfo.InvariantInfo);
				else
					if (lval is System.Double)
					  return ((double)lval).ToString(NumberFormatInfo.InvariantInfo);
				else
					return lval.ToString();
			}
			else 
				if (IsConsPair(lval))
					return "(" + Format(Car(lval)) + " . " + Format(Cdr(lval)) + ")";
			else
			{
				StringBuilder text = new StringBuilder();
				do
				{
					if (text.Length == 0)
						text.Append("(");
					else
						text.Append(" ");
					if (IsConsPair(lval))
					{
						text.AppendFormat("{0} . {1}",  Format(Car(lval)), Format(Cdr(lval)));
						break;
					}
					else 
						text.Append(Format(Car(lval)));
					lval = Cdr(lval);
				} while (lval != null);
				text.Append(")");
				return text.ToString();
			}
		}
	}
}
