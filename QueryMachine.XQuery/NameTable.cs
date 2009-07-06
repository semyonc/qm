using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DataEngine.XQuery
{
    internal class NameTable: XmlNameTable
    {
        private int count;
        private Entry[] entries;
        private int hashCodeRandomizer;
        private int mask = 0x1f;

        public NameTable()
        {
            entries = new Entry[mask + 1];
            hashCodeRandomizer = Environment.TickCount;
        }

        public override string Add(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            int length = key.Length;
            if (length == 0)
            {
                return string.Empty;
            }
            int hashCode = length + hashCodeRandomizer;
            for (int i = 0; i < key.Length; i++)
            {
                hashCode += (hashCode << 7) ^ key[i];
            }
            hashCode -= hashCode >> 0x11;
            hashCode -= hashCode >> 11;
            hashCode -= hashCode >> 5;
            for (Entry entry = entries[hashCode & mask]; entry != null; entry = entry.next)
            {
                if ((entry.hashCode == hashCode) && entry.str.Equals(key))
                    return entry.str;
            }
            return AddEntry(key, hashCode);
        }

        public override string Add(char[] key, int start, int len)
        {
            if (len == 0)
                return string.Empty;
            int hashCode = len + hashCodeRandomizer;
            hashCode += (hashCode << 7) ^ key[start];
            int num2 = start + len;
            for (int i = start + 1; i < num2; i++)
            {
                hashCode += (hashCode << 7) ^ key[i];
            }
            hashCode -= hashCode >> 0x11;
            hashCode -= hashCode >> 11;
            hashCode -= hashCode >> 5;
            for (Entry entry = entries[hashCode & mask]; entry != null; entry = entry.next)
            {
                if ((entry.hashCode == hashCode) && TextEquals(entry.str, key, start))
                {
                    return entry.str;
                }
            }
            return AddEntry(new string(key, start, len), hashCode);
        }

        private string AddEntry(string str, int hashCode)
        {
            int index = hashCode & mask;
            Entry entry = new Entry(str, hashCode, entries[index]);
            entries[index] = entry;
            if (count++ == mask)
                Grow();
            return entry.str;
        }

        public override string Get(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.Length == 0)
                return string.Empty;
            int num = value.Length + hashCodeRandomizer;
            int num2 = num;
            for (int i = 0; i < value.Length; i++)
            {
                num2 += (num2 << 7) ^ value[i];
            }
            num2 -= num2 >> 0x11;
            num2 -= num2 >> 11;
            num2 -= num2 >> 5;
            for (Entry entry = entries[num2 & mask]; entry != null; entry = entry.next)
            {
                if ((entry.hashCode == num2) && entry.str.Equals(value))
                    return entry.str;
            }
            return null;
        }

        public override string Get(char[] key, int start, int len)
        {
            if (len == 0)
            {
                return string.Empty;
            }
            int num = len + hashCodeRandomizer;
            num += (num << 7) ^ key[start];
            int num2 = start + len;
            for (int i = start + 1; i < num2; i++)
            {
                num += (num << 7) ^ key[i];
            }
            num -= num >> 0x11;
            num -= num >> 11;
            num -= num >> 5;
            for (Entry entry = entries[num & mask]; entry != null; entry = entry.next)
            {
                if ((entry.hashCode == num) && TextEquals(entry.str, key, start))
                    return entry.str;
            }
            return null;
        }

        private void Grow()
        {
            int num = (mask * 2) + 1;
            Entry[] entries = this.entries;
            Entry[] entryArray2 = new Entry[num + 1];
            for (int i = 0; i < entries.Length; i++)
            {
                Entry next;
                for (Entry entry = entries[i]; entry != null; entry = next)
                {
                    int index = entry.hashCode & num;
                    next = entry.next;
                    entry.next = entryArray2[index];
                    entryArray2[index] = entry;
                }
            }
            entries = entryArray2;
            mask = num;
        }

        private static bool TextEquals(string array, char[] text, int start)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != text[start + i])
                    return false;
            }
            return true;
        }

        private class Entry
        {            
            internal int hashCode;
            internal NameTable.Entry next;
            internal string str;

            internal Entry(string str, int hashCode, NameTable.Entry next)
            {
                this.str = str;
                this.hashCode = hashCode;
                this.next = next;
            }
        }
    }
}
