//      Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
//     
//      The use and distribution terms for this software are contained in the file
//      named license.txt, which can be found in the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by the
//      terms of this license.
//     
//      You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;
using System.IO;
using System.Reflection;

namespace DataEngine.XQuery.MS
{
    public struct XmlCharType
    {
        // Whitespace chars -- Section 2.3 [3]
        // Letters -- Appendix B [84]
        // Starting NCName characters -- Section 2.3 [5] (Starting Name characters without ':')
        // NCName characters -- Section 2.3 [4] (Name characters without ':')
        // Character data characters -- Section 2.2 [2]
        // PubidChar ::= #x20 | #xD | #xA | [a-zA-Z0-9] | [-'()+,./:=?;!*#@$_%] Section 2.3 of spec
        
        internal const int fWhitespace = 1;
        internal const int fLetter = 2;
        internal const int fNCStartName = 4;
        internal const int fNCName = 8;
        internal const int fCharData = 16;
        internal const int fPublicId = 32;
        internal const int fText = 64;
        internal const int fAttrValue = 128;

        private const uint CharPropertiesSize = (uint)char.MaxValue + 1;

        private static object s_Lock;

        private static object StaticLock
        {
            get
            {
                if (s_Lock == null)
                {
                    object o = new object();
                    Interlocked.CompareExchange(ref s_Lock, o, null);
                }
                return s_Lock;
            }
        }

        private static byte[] s_CharProperties;
        internal byte[] charProperties;

        static void InitInstance()
        {
            lock (StaticLock)
            {
                if (s_CharProperties != null)
                    return;

                Stream memStream = Assembly.GetAssembly(typeof(XmlNode)).GetManifestResourceStream("XmlCharType.bin");
                s_CharProperties = new byte[CharPropertiesSize];
                memStream.Read(s_CharProperties, 0, (int)CharPropertiesSize);
            }
        }

        private XmlCharType(byte[] charProperties)
        {
            this.charProperties = charProperties;
        }

        static public XmlCharType Instance
        {
            get
            {
                if (s_CharProperties == null)
                    InitInstance();
                return new XmlCharType(s_CharProperties);
            }
        }

        public bool IsWhiteSpace(char ch)
        {
            return (charProperties[ch] & fWhitespace) != 0;
        }

        public bool IsLetter(char ch)
        {
            return (charProperties[ch] & fLetter) != 0;
        }

        public bool IsExtender(char ch)
        {
            return (ch == 183);
        }

        public bool IsNCNameChar(char ch)
        {
            return (charProperties[ch] & fNCName) != 0;
        }

        public bool IsStartNCNameChar(char ch)
        {
            return (charProperties[ch] & fNCStartName) != 0;
        }

        public bool IsCharData(char ch)
        {
            return (charProperties[ch] & fCharData) != 0;
        }

        // [13] PubidChar ::= #x20 | #xD | #xA | [a-zA-Z0-9] | [-'()+,./:=?;!*#@$_%] Section 2.3 of spec
        public bool IsPubidChar(char ch)
        {
            return (charProperties[ch] & fPublicId) != 0;
        }

        // TextChar = CharData - { 0xA, 0xD, '<', '&', ']' }
        internal bool IsTextChar(char ch)
        {
            return (charProperties[ch] & fText) != 0;
        }

        // AttrValueChar = CharData - { 0xA, 0xD, 0x9, '<', '>', '&', '\'', '"' }
        internal bool IsAttributeValueChar(char ch)
        {
            return (charProperties[ch] & fAttrValue) != 0;
        }

        public bool IsNameChar(char ch)
        {
            return IsNCNameChar(ch) || ch == ':';
        }

        public bool IsStartNameChar(char ch)
        {
            return IsStartNCNameChar(ch) || ch == ':';
        }

        public bool IsDigit(char ch)
        {
            return (ch >= 48 && ch <= 57);
        }

        public bool IsHexDigit(char ch)
        {
            return (ch >= 48 && ch <= 57) || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
        }

        public bool IsOnlyWhitespace(string str)
        {
            return IsOnlyWhitespaceWithPos(str) == -1;
        }

        public int IsOnlyWhitespaceWithPos(string str)
        {
            if (str != null)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if ((charProperties[str[i]] & fWhitespace) == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public bool IsName(string str)
        {
            if (str.Length == 0 || !IsStartNameChar(str[0]))
            {
                return false;
            }
            for (int i = 1; i < str.Length; i++)
            {
                if (!IsNameChar(str[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsNmToken(string str)
        {
            if (str.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if ((charProperties[str[i]] & fNCName) == 0 && str[i] != ':')
                {
                    return false;
                }
            }
            return true;
        }

        public int IsOnlyCharData(string str)
        {
            if (str != null)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if ((charProperties[str[i]] & fCharData) == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int IsPublicId(string str)
        {
            if (str != null)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if ((charProperties[str[i]] & fPublicId) == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
