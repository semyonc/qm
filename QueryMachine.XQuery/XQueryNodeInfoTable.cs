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
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DataEngine.XQuery
{
    public class XQueryNodeInfo
    {
        public int handle;        
        public string name;
        public string prefix;
        public string localName;
        public string namespaceUri;

        public XQueryNodeInfo(string prefix, string localName, string namespaceUri)
        {
            handle = -1;
            if (String.IsNullOrEmpty(prefix))
                name = localName;
            else
                name = prefix + ':' + localName;
            this.prefix = prefix;
            this.localName = localName;
            this.namespaceUri = namespaceUri;
        }        
    }

    internal class XQueryNodeInfoTable
    {
        private XmlNameTable nameTable;
        private int count;
        private Entry[] entries;
        private Entry[] handleArray;
        private int hashCodeRandomizer;
        private int mask = 0x1f;

        public XQueryNodeInfoTable(XmlNameTable nameTable)
        {
            entries = new Entry[mask + 1];
            handleArray = new Entry[mask + 1];
            hashCodeRandomizer = Environment.TickCount;
            this.nameTable = nameTable;
        }

        public XQueryNodeInfo Add(string prefix, string localName, string namespaceUri)
        {
            if (prefix == null)
                throw new ArgumentException();
            prefix = nameTable.Add(prefix);
            if (String.IsNullOrEmpty(localName))
                throw new ArgumentException();
            localName = nameTable.Add(localName);
            if (namespaceUri == null)
                throw new ArgumentException();
            namespaceUri = nameTable.Add(namespaceUri);
            int hashCode = localName.GetHashCode() + hashCodeRandomizer;
            hashCode = (hashCode << 7) ^ prefix.GetHashCode();
            hashCode = (hashCode << 7) ^ namespaceUri.GetHashCode();
            hashCode -= hashCode >> 0x11;
            hashCode -= hashCode >> 11;
            hashCode -= hashCode >> 5;
            for (Entry entry = entries[hashCode & mask]; entry != null; entry = entry.next)
                if ((entry.hashCode == hashCode) && InternalEquals(entry.body, prefix, localName, namespaceUri))
                    return entry.body;
            XQueryNodeInfo nodeInfo = new XQueryNodeInfo(prefix, localName, namespaceUri);
            return AddEntry(nodeInfo, hashCode);            
        }

        public XQueryNodeInfo Get(int handle)
        {
            if (handle == -1)
                return null;
            else
            {
                if (handle > -1 && handle < handleArray.Length &&
                    handleArray[handle] != null)
                       return handleArray[handle].body;
                else
                    throw new ArgumentException("handle");
            }
        }

        private void Grow()
        {
            int num = (mask * 2) + 1;
            Entry[] entryArray2 = new Entry[num + 1];
            Entry[] handleArray2 = new Entry[num + 1];
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
            for (int i = 0; i < handleArray.Length; i++)
                handleArray2[i] = handleArray[i];
            entries = entryArray2;
            handleArray = handleArray2;
            mask = num;
        }

        private bool InternalEquals(XQueryNodeInfo nodeInfo, string prefix, 
            string localName, string namespaceUri)
        {
            return Object.ReferenceEquals(nodeInfo.prefix, prefix) &&
                Object.ReferenceEquals(nodeInfo.localName, localName) &&
                Object.ReferenceEquals(nodeInfo.namespaceUri, namespaceUri);
        }

        private XQueryNodeInfo AddEntry(XQueryNodeInfo nodeInfo, int hashCode)
        {
            int index = hashCode & mask;
            Entry entry = new Entry(nodeInfo, hashCode, entries[index]);
            entries[index] = entry;
            handleArray[count] = entry;
            nodeInfo.handle = count;
            if (count++ == mask)
                Grow();
            return entry.body;
        }

        private class Entry
        {
            internal int hashCode;
            internal Entry next;
            internal XQueryNodeInfo body;

            internal Entry(XQueryNodeInfo body, int hashCode, Entry next)
            {
                this.body = body;
                this.hashCode = hashCode;
                this.next = next;
            }
        }
    }
}
