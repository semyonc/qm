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
using System.IO;

using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.Threading;

using DataEngine.XQuery.DocumentModel;

namespace DataEngine.XQuery
{
    sealed class PageFile: IDisposable
    {
        internal const int Leaf = -2;
        internal const int MixedLeaf = -3;

        private object filelock;
        private string fileName;
        private Stream fileStream;
        private bool closed;
        private BinaryWriter binWriter;
        private BinaryReader binReader;
        private int pagesize;
        private int min_workset;
        private int max_workset;
        private int workset_delta;
        private int min_increment;
        private int max_decrement;
        private int count;
        private int lastcount;
        private int blockcount;
        private double workset;
        private long hit_count;
        private long miss_count;
        private double ratio;
        private int pagecount;
        private XdmNode lastnode;
        private Page lastpage;
        private Page[] lastblock;
        private List<Page[]> pagelist;
        private List<Page> cached;
        private List<DmNode> heads;

        #region IDisposable Members

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (! _disposed)
            {
                Close();
                _disposed = true;
            }
        }

        #endregion
                                                   
        public const int XQueryBlockSize = 1000;
        public const int XQueryDirectAcessBufferSize = 500;    
        
        public PageFile(bool large)
        {
            if (large)
            {
                pagesize = 800;
                min_workset = 3;
                max_workset = 15;
                workset_delta = 5;
                min_increment = 2;
                max_decrement = 1;
            }
            else
            {
                pagesize = 16;
                min_workset = 100;
                max_workset = 3000;
                workset_delta = 500;
                min_increment = 150;
                max_decrement = 100;
            }
            pagelist = new List<Page[]>();
            cached = new List<Page>();
            heads = new List<DmNode>();
            workset = min_workset;
            filelock = new object();
        }

        ~PageFile()
        {
            Dispose(false);
        }

        public bool HasSchemaInfo { get; set; }

        public bool ReadBoolean()
        {
            return binReader.ReadBoolean();
        }

        public void WriteBoolean(bool value)
        {
            binWriter.Write(value);
        }

        public String ReadString()
        {
            return binReader.ReadString();
        }

        public void WriteString(string value)
        {
            binWriter.Write(value);
        }

        public int ReadInt32()
        {
            return binReader.ReadInt32();
        }

        public void WriteInt32(int value)
        {
            binWriter.Write(value);
        }

        public DmAttribute ReadAttributeInfo()
        {
            return (DmAttribute)heads[binReader.ReadInt32()];
        }

        public void WriteAttributeInfo(DmAttribute dm)
        {
            if (dm._index == -1)
            {
                dm._index = heads.Count;
                heads.Add(dm);
            }
            binWriter.Write(dm._index);
        }

        private void CreateFile()
        {
            fileName = Path.GetTempFileName();
            fileStream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);
            binWriter = new EmbeddedWriter(fileStream, Encoding.UTF8);
            binReader = new EmbeddedReader(fileStream, Encoding.UTF8);
        }

        public void Close()
        {
            lock (filelock)
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    File.Delete(fileName);
                    fileStream = null;
                    closed = true;
                }
            }
        }
   
        public void Get(int index, bool load, out int parent, out DmNode head, out XdmNode node)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException("index");
            int pagenum = index / pagesize;
            Page[] block = pagelist[pagenum / XQueryBlockSize];
            Page page = block[pagenum % XQueryBlockSize];
            int k = index % pagesize;
            if (load)
            {
                page.pin++;
                hit_count++;
            }
            int hindex = page.hindex[k];
            if (hindex == -1)
                head = null;
            else
                head = heads[hindex];
            parent = page.parent[k];
            XdmNode[] nodes = page.nodes;
            if (nodes == null)
            {
                if (load)
                {
                    OptimizeCache();
                    nodes = ReadPage(page);
                    node = nodes[k];
                }
                else
                    node = null;
            }
            else
                node = nodes [k];                
        }

        public int GetHIndex(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException("index");
            int pagenum = index / pagesize;
            Page[] block = pagelist[pagenum / XQueryBlockSize];
            Page page = block[pagenum % XQueryBlockSize];
            return page.hindex[index % pagesize];
        }

        public DmNode GetHead(int index)
        {
            int hindex = GetHIndex(index);
            if (hindex == -1)
                return null;
            return heads[hindex];
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public int this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                    throw new ArgumentException("index");
                int pagenum = index / pagesize;
                Page[] block = pagelist[pagenum / XQueryBlockSize];
                Page page = block[pagenum % XQueryBlockSize];
                return page.next[index % pagesize];
            }
            set
            {
                if (index < 0 || index >= count)
                    throw new ArgumentException("index");
                int pagenum = index / pagesize;
                Page[] block = pagelist[pagenum / XQueryBlockSize];
                Page page = block[pagenum % XQueryBlockSize];
                page.next[index % pagesize] = value;
            }
        }

        public int Select(int[] targets, ref int index, ref int length, int[] buffer)
        {
            int count = 0;
            int size = buffer.Length;
            int pagenum = index / pagesize;
            int blocknum = pagenum / XQueryBlockSize;
            Page[] block = pagelist[blocknum];
            Page curr = block[pagenum % XQueryBlockSize];
            int k = index % pagesize;
            while (length > 0 && count < size)
            {
                if (targets == null || Array.BinarySearch(targets, curr.hindex[k]) >= 0)
                    buffer[count++] = index;
                index++;
                k++;
                if (k == pagesize)
                {
                    pagenum = index / pagesize;
                    block = pagelist[pagenum / XQueryBlockSize];
                    curr = block[pagenum % XQueryBlockSize];
                    k = 0;
                }
                length--;
            }
            return count;
        }
        
        private XdmNode[] ReadPage(Page page)
        {
            XdmNode[] nodes = new XdmNode[pagesize];
            lock (filelock)
            {
                fileStream.Seek(page.offset, SeekOrigin.Begin);
                for (int k = 0; k < pagesize; k++)
                {
                    int hindex = page.hindex[k];
                    if (hindex != -1)
                    {
                        XdmNode node = heads[hindex].CreateNode();
                        node.Load(this);
                        if (page.next[k] == MixedLeaf)
                            ((XdmElement)node).LoadTextValue(this);
                        nodes[k] = node;
                    }
                }
                page.nodes = nodes;
                cached.Add(page);
            }
            miss_count++;
            return nodes;
        }

        private void WritePage(Page page)
        {
            lock (filelock)
            {
                if (!page.stored && !closed)
                {
                    if (fileStream == null)
                        CreateFile();
                    fileStream.Seek(0, SeekOrigin.End);
                    page.offset = fileStream.Position;
                    int num = page.num * pagesize;
                    for (int k = 0; k < page.nodes.Length; k++)
                    {
                        XdmNode node = page.nodes[k];
                        if (node != null)
                        {
                            node.Store(this);
                            if (page.next[k] == MixedLeaf)
                                ((XdmElement)node).StoreTextValue(this);
                        }
                    }
                    page.stored = true;
                }
                page.nodes = null;
            }
        }

        private void OptimizeCache()
        {            
            if (cached.Count > workset + workset_delta)
            {
                if (miss_count > 0 && hit_count > 0)
                {
                    double ratio1 = (double)miss_count / hit_count;
                    if (ratio > 0)
                    {
                        double delta = Math.Ceiling((workset * ratio1) / ratio) - workset;
                        if (delta > 0 && delta < min_increment)
                            delta = min_increment;
                        else if (delta < 0 && delta > max_decrement)
                            delta = -max_decrement;
                        workset += delta;
                        if (workset < min_workset)
                            workset = min_workset;
                        else if (workset > max_workset)
                            workset = max_workset;
                    }
                    ratio = ratio1;
                }
                if (workset < cached.Count)
                {
                    int len; 
                    Page[] pages;
                    Page[] cached_pages;
                    lock (filelock)
                    {
                        len = cached.Count - (int)workset;
                        pages = new Page[len];
                        cached_pages = cached.ToArray();
                        cached.Clear();
                    }
                    for (int i = 0; i < cached_pages.Length; i++)
                    {
                        Page curr = cached_pages[i];
                        if (curr == lastpage)
                            continue;
                        for (int k = 0; k < len; k++)
                            if (pages[k] == null || pages[k].pin < curr.pin)
                            {
                                Page p = pages[k];
                                pages[k] = curr;
                                if (p == null)
                                    break;
                                else
                                    curr = p;
                            }
                    }                    
                    ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
                        {
                            for (int k = 0; k < len; k++)
                                if (pages[k] != null)
                                    WritePage(pages[k]);
                            lock (filelock)
                            {
                                for (int k = 0; k < cached_pages.Length; k++)
                                    if (cached_pages[k].nodes != null)
                                        cached.Add(cached_pages[k]);
                            }
                        }));
                }
            }
        }

        public void AddNode(int parent, DmNode head, XdmNode node)
        {
            if (lastpage == null ||
                lastcount == pagesize)
            {
                if (lastpage != null)
                {
                    lock (filelock)
                        cached.Add(lastpage);
                    OptimizeCache();
                }
                lastpage = new Page(pagesize);                
                lastpage.pin = 1;
                lastpage.num = pagecount++;
                if (lastblock == null ||
                    blockcount == lastblock.Length)
                {
                    lastblock = new Page[XQueryBlockSize];
                    blockcount = 0;
                    pagelist.Add(lastblock);
                }
                lastblock[blockcount++] = lastpage;
                lastcount = 0;
            }
            count++;
            if (head == null)
                lastpage.hindex[lastcount] = -1;
            else
            {
                if (head._index == -1)
                {
                    head._index = heads.Count;
                    heads.Add(head);
                }
                lastpage.hindex[lastcount] = head._index;
            }
            lastpage.nodes[lastcount] = node;
            lastpage.next[lastcount] = count;
            lastpage.parent[lastcount] = parent;
            lastnode = node;
            lastcount++;
        }

        internal XdmNode LastNode
        {
            get
            {
                return lastnode;
            }
        }

        private class Page
        {
            internal int num;
            internal long offset;
            internal int pin;
            internal bool stored;
            internal XdmNode[] nodes;
            internal int[] next;
            internal int[] hindex;
            internal int[] parent;

            public Page(int pagesize)
            {
                hindex = new int[pagesize];
                for (int k = 0; k < pagesize; k++)
                    hindex[k] = -1;
                nodes = new XdmNode[pagesize];
                next = new int[pagesize];
                parent = new int[pagesize];
                stored = false;
            }
        }

        private class EmbeddedReader : BinaryReader
        {
            public EmbeddedReader(Stream fs, Encoding encoding)
                : base(fs, encoding)
            {
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(false);
            }
        }

        private class EmbeddedWriter : BinaryWriter
        {
            public EmbeddedWriter(Stream fs, Encoding encoding)
                : base(fs, encoding)
            {
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(false);
            }
        }
    }
}
