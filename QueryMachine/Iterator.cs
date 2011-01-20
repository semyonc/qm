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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataEngine.CoreServices
{
// Provide some dummy class for compatibility with .NET Parallel Extensions
#if NO_TPL
    public class Iterator
    {
        public static void For(int fromInclusive, int toExclusive, Action<int> body)
        {
            for (int s = fromInclusive; s < toExclusive; s++)
                body(s);
        }

        public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            foreach (TSource s in source)
                body(s);
        }

        public static void Invoke(Action[] actions)
        {
            foreach (Action body in actions)
                body();
        }
    }    
#else
    public class Iterator
    {
        public static void For(int fromInclusive, int toExclusive, Action<int> body)
        {
            Parallel.For(fromInclusive, toExclusive, body);
        }

        public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            Parallel.ForEach<TSource>(source, body);
        }

        public static void Invoke(Action[] actions)
        {
            Parallel.Invoke(actions);
        }
    }
#endif
}
