//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
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

namespace DataEngine.CoreServices
{
    public class ATOM
    {
        public readonly string prefix;
        public readonly string[] parts;

        private ATOM(string prefix, string[] parts)
        {
            this.prefix = prefix;
            this.parts = parts;
        }

        public override string ToString()
        {
            return GetFullName(prefix, parts);
        }

        public static object Create(string name)
        {
            return Create(null, new string[] { name }, true);
        }

        public static object Create(string prefix, string[] parts, bool is_global)
        {
            ATOM result;
            string name = GetFullName(prefix, parts);
            lock (_global_t)
            {
                if (_global_t.TryGetValue(name, out result))
                    return result;

                if (_local_t == null)
                    _local_t = new Dictionary<String, ATOM>();

                if (!_local_t.TryGetValue(name, out result))
                {
                    result = new ATOM(prefix, parts);
                    if (is_global)
                        _global_t.Add(name, result);
                    else
                        _local_t.Add(name, result);
                }
            }
            return result;
        }

        internal static readonly Dictionary<String, ATOM> _global_t = new Dictionary<String, ATOM>();

        internal static Dictionary<String, ATOM> _local_t;

        public static void Prune()
        {
            if (_local_t != null)
                _local_t.Clear();
        }

        private static string GetFullName(string prefix, string[] parts)
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(prefix))
            {
                sb.Append(prefix);
                sb.Append(':');
            }
            for (int k = 0; k < parts.Length; k++)
            {
                if (k > 0)
                    sb.Append(".");
                sb.Append(parts[k]);
            }
            return sb.ToString();
        }
    }
}
