using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DataEngine.XQuery
{
    class NameIdentity
    {
        public string Prefix { get; private set; }
        public string LocalName { get; private set; }
        public string Namespace { get; private set; }

        private NameIdentity()
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Prefix != "")
            {
                sb.Append(Prefix);
                sb.Append(":");
            }
            sb.Append(LocalName);
            if (Namespace != "")
                sb.AppendFormat("[{0}]", Namespace);
            return sb.ToString();
        }

        public static NameIdentity Parse(string name, IXmlNamespaceResolver resolver)
        {
            string prefix;
            string localName;
            Split(name, out prefix, out localName);
            NameIdentity identity = new NameIdentity();
            identity.Prefix = prefix;
            identity.LocalName = localName;
            identity.Namespace = resolver.LookupNamespace(identity.Prefix);
            if (identity.Namespace == null)
            {
                if (!String.IsNullOrEmpty(prefix))
                    throw new XQueryException(Properties.Resources.XPST0081, prefix);
                else
                    identity.Namespace = String.Empty;
            }
            return identity;
        }

        private static int ParseNCName(string s, int offset)
        {
            int num = offset;
            XmlCharType instance = XmlCharType.Instance;
            if (offset < s.Length && (instance.charProperties[s[offset]] & 4) != 0)
            {
                offset++;
                while (offset < s.Length)
                {
                    if ((instance.charProperties[s[offset]] & 8) == 0)
                        break;
                    offset++;
                }
            }
            return offset - num;
        }

        private static int ParseQName(string s, int offset, out int colonOffset)
        {
            colonOffset = 0;
            int num = ParseNCName(s, offset);
            if (num != 0)
            {
                offset += num;
                if (offset < s.Length && s[offset] == ':')
                {
                    int num2 = ParseNCName(s, offset + 1);
                    if (num2 != 0)
                    {
                        colonOffset = offset;
                        num += num2 + 1;
                    }
                }
            }
            return num;
        }        

        private static void Split(string value, out string prefix, out string localName)
        {
            prefix = String.Empty;
            int num;
            int num2 = ParseQName(value, 0, out num);
            if (num2 == 0 || num2 != value.Length)
                localName = null;
            else
            {
                if (num != 0)
                {
                    prefix = value.Substring(0, num);
                    localName = value.Substring(num + 1);
                }
                else
                    localName = value;
            }
        }
    }
}
