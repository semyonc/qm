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
using System.Xml.Schema;


namespace DataEngine.XQuery
{
    class XQuerySchemaInfoTable
    {
        private int count;
        private Entry[] entries;
        private Entry[] handleArray;
        private int hashCodeRandomizer;
        private int mask = 0x1f;

        public XQuerySchemaInfoTable()
        {
            entries = new Entry[mask + 1];
            handleArray = new Entry[mask + 1];
            hashCodeRandomizer = Environment.TickCount;
        }

        public IXmlSchemaInfo Add(IXmlSchemaInfo xmlSchemaInfo)
        {
            int hashCode = GetHashCode(xmlSchemaInfo);
            for (Entry entry = entries[hashCode & mask]; entry != null; entry = entry.next)
                if ((entry.hashCode == hashCode) && InternalEquals(entry.xmlSchemaInfo, xmlSchemaInfo))
                    return entry.xmlSchemaInfo;
            return AddEntry(xmlSchemaInfo, hashCode);
        }

        public int GetHandle(IXmlSchemaInfo schemaInfo)
        {
            XmlSchemaInfoCust cust = schemaInfo as XmlSchemaInfoCust;
            if (cust != null)
                return cust.handle;
            else
                return -1;
        }

        public IXmlSchemaInfo Get(int handle)
        {
            if (handle == -1)
                return null;
            else
            {
                if (handle > -1 && handle < handleArray.Length &&
                    handleArray[handle] != null)
                    return handleArray[handle].xmlSchemaInfo;
                else
                    throw new ArgumentException("handle");
            }
        }

        private int GetHashCode(IXmlSchemaInfo xmlSchemaInfo)
        {
            int hashCode = hashCodeRandomizer;
            if (xmlSchemaInfo.SchemaType != null)
                hashCode = (hashCode << 7) ^ xmlSchemaInfo.SchemaType.GetHashCode();
            if (xmlSchemaInfo.SchemaAttribute != null)
                hashCode = (hashCode << 7) ^ xmlSchemaInfo.SchemaAttribute.GetHashCode();
            if (xmlSchemaInfo.SchemaElement != null)
                hashCode = (hashCode << 7) ^ xmlSchemaInfo.SchemaElement.GetHashCode();
            if (xmlSchemaInfo.MemberType != null)
                hashCode = (hashCode << 7) ^ xmlSchemaInfo.MemberType.GetHashCode();
            return hashCode;
        }

        private bool InternalEquals(XmlSchemaInfo a, IXmlSchemaInfo b)
        {
            return a.Validity == b.Validity && a.SchemaType == b.SchemaType &&
                a.SchemaAttribute == b.SchemaAttribute && a.SchemaElement == b.SchemaElement &&
                a.MemberType == b.MemberType && a.IsDefault == b.IsDefault && a.IsNil == b.IsNil;
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

        private IXmlSchemaInfo AddEntry(IXmlSchemaInfo xmlSchemaInfo, int hashCode)
        {
            XmlSchemaInfoCust schemaInfo = new XmlSchemaInfoCust();
            schemaInfo.IsDefault = xmlSchemaInfo.IsDefault;
            schemaInfo.IsNil = xmlSchemaInfo.IsNil;
            schemaInfo.MemberType = xmlSchemaInfo.MemberType;
            schemaInfo.SchemaAttribute = xmlSchemaInfo.SchemaAttribute;
            schemaInfo.SchemaElement = xmlSchemaInfo.SchemaElement;
            schemaInfo.SchemaType = xmlSchemaInfo.SchemaType;
            schemaInfo.Validity = xmlSchemaInfo.Validity;
            schemaInfo.handle = count;
            int index = hashCode & mask;
            Entry entry = new Entry(schemaInfo, hashCode, entries[index]);
            entries[index] = entry;
            handleArray[count] = entry;            
            if (count++ == mask)
                Grow();
            return entry.xmlSchemaInfo;
        }

        private class Entry
        {
            internal int hashCode;
            internal Entry next;
            internal XmlSchemaInfo xmlSchemaInfo;

            internal Entry(XmlSchemaInfo xmlSchemaInfo, int hashCode, Entry next)
            {
                this.xmlSchemaInfo = xmlSchemaInfo;
                this.hashCode = hashCode;
                this.next = next;
            }
        }

        private class XmlSchemaInfoCust : XmlSchemaInfo
        {
            internal int handle;
        }
    }
}
