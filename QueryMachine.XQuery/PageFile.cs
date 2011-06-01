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
using System.Collections.Concurrent;
using System.Text;
using System.IO;

using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

using DataEngine.CoreServices;
using DataEngine.XQuery.DocumentModel;


namespace DataEngine.XQuery
{
    sealed class XdmWriter
    {
        private List<DmNode> heads;
        private BinaryWriter binWriter;

        public XdmWriter(Stream stream, List<DmNode> heads)
        {
            binWriter = new EmbeddedWriter(stream, Encoding.UTF8);
            this.heads = heads;
        }

        public void WriteBoolean(bool value)
        {
            binWriter.Write(value);
        }

        public void WriteString(string value)
        {
            binWriter.Write(value);
        }

        public void WriteInt32(int value)
        {
            binWriter.Write(value);
        }

        public void WriteAttributeInfo(DmAttribute dm)
        {
            if (dm._index == -1)
            {
                lock (heads)
                {
                    dm._index = heads.Count;
                    heads.Add(dm);
                }
            }
            binWriter.Write(dm._index);
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

    sealed class XdmReader
    {
        private List<DmNode> heads;
        private BinaryReader binReader;

        public XdmReader(Stream stream, List<DmNode> heads)
        {
            binReader = new EmbeddedReader(stream, Encoding.UTF8);
            this.heads = heads;
        }

        public bool ReadBoolean()
        {
            return binReader.ReadBoolean();
        }

        public String ReadString()
        {
            return binReader.ReadString();
        }

        public int ReadInt32()
        {
            return binReader.ReadInt32();
        }

        public DmAttribute ReadAttributeInfo()
        {
            lock (heads)
            {
                return (DmAttribute)heads[binReader.ReadInt32()];
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
    }

    sealed class PageFile: IDisposable
    {
        internal const int Leaf = -2;
        internal const int MixedLeaf = -3;

        private volatile bool closed;

        private int pagesize;
        private int min_workset;
        private int max_workset;
        private int workset_delta;
        private int min_increment;
        private int max_decrement;
        private int count;
        private int lastcount;
        private double workset;
        private long hit_count;
        private long miss_count;
        private double ratio;
        private int pagecount;

        private XdmNode lastnode;
        private Page lastpage;
        private List<Page> pagelist;
        private List<DmNode> heads;

        private int partSize;
        private volatile int cacheCount;
        private PageFilePart[] parts;
        private readonly HashSet<Page> cache;
        private SpinLock cacheLock;

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
                                                                   
        public PageFile(bool large)
        {
            if (large)
            {
                pagesize = XQueryLimits.LargeFilePageSize;
                min_workset = XQueryLimits.LargeFileMinWorkset;
                max_workset = XQueryLimits.LargeFileMaxWorkset;
                workset_delta = XQueryLimits.LargeFileWorksetDelta;
                min_increment = XQueryLimits.LargeFileMinIncrement;
                max_decrement = XQueryLimits.LargeFileMaxDecrement;
            }
            else
            {
                pagesize = XQueryLimits.SmallFilePageSize;
                min_workset = XQueryLimits.SmallFileMinWorkset;
                max_workset = XQueryLimits.SmallFileMaxWorkset;
                workset_delta = XQueryLimits.SmallFileWorksetDelta;
                min_increment = XQueryLimits.SmallFileMinIncrement;
                max_decrement = XQueryLimits.SmallFileMaxDecrement;
            }
            pagelist = new List<Page>();
            heads = new List<DmNode>();
            workset = min_workset;
            cache = new HashSet<Page>();
            cacheLock = new SpinLock();
            partSize = Environment.ProcessorCount;
            _optimizer.Add(this);
        }

        ~PageFile()
        {
            Dispose(false);
        }

        public bool HasSchemaInfo { get; set; }

        private void CreateFile()
        {            
            parts = new PageFilePart[partSize];
            for (int k = 0; k < partSize; k++)
            {
                PageFilePart part = new PageFilePart();
                part.fileName = Path.GetTempFileName();
                part.fileStream = new FileStream(part.fileName, 
                    FileMode.Open, FileAccess.ReadWrite);
                part.fileWriter = new XdmWriter(part.fileStream, heads);
                part.fileReader = new XdmReader(part.fileStream, heads);
                parts[k] = part;
            }
        }

        public void Close()
        {
            lock (_optimizer.SyncRoot)
            {                          
                if (!closed)
                {
                    closed = true;
                    if (parts != null)
                    {
                        foreach (PageFilePart part in parts)
                        {
                            lock (part.fileStream)
                            {
                                part.fileStream.Close();
                                File.Delete(part.fileName);
                            }
                        }
                    }
                }
            }
        }

        private void AddCache(Page page)
        {
            bool lockTaken = false;
            cacheLock.Enter(ref lockTaken);
            cache.Add(page);
            cacheCount++;
            if (lockTaken)
                cacheLock.Exit();
        }

        private void RemoveCache(Page page)
        {
            bool lockTaken = false;
            cacheLock.Enter(ref lockTaken);
            cache.Remove(page);
            cacheCount--;
            if (lockTaken)
                cacheLock.Exit();
        }
   
        public void Get(int index, bool load, out int parent, out DmNode head, out XdmNode node)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException("index");
            int pagenum = index / pagesize;
            Page page = pagelist[pagenum];
            int k = index % pagesize;
            if (load)
            {
                Interlocked.Increment(ref page.pin);
                Interlocked.Increment(ref hit_count);
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
            Page page = pagelist[pagenum];
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
                Page page = pagelist[index / pagesize];
                return page.next[index % pagesize];
            }
            set
            {
                if (index < 0 || index >= count)
                    throw new ArgumentException("index");
                Page page = pagelist[index / pagesize];
                page.next[index % pagesize] = value;
            }
        }

        public int Select(int[] targets, ref int index, ref int length, int[] buffer)
        {
            int count = 0;
            int size = buffer.Length;
            Page curr = pagelist[index / pagesize];
            int k = index % pagesize;
            while (length > 0 && count < size)
            {
                if (targets == null || Array.BinarySearch(targets, curr.hindex[k]) >= 0)
                    buffer[count++] = index;
                length--;
                index++;
                k++;                
                if (k == pagesize)
                {
                    int pagenum = index / pagesize;
                    if (pagenum == pagelist.Count)
                        break;
                    curr = pagelist[pagenum];
                    k = 0;
                }
            }
            return count;
        }

        private XdmNode[] ReadPage(Page page)
        {
#if DEBUG
            try
            {
                PerfMonitor.Global.Begin("PageFile.ReadPage()");
#endif
                XdmNode[] nodes = new XdmNode[pagesize];
                PageFilePart part = parts[page.partid];
                lock (part.fileStream)
                {
                    part.fileStream.Seek(page.offset, SeekOrigin.Begin);
                    for (int k = 0; k < pagesize; k++)
                    {
                        int hindex = page.hindex[k];
                        if (hindex != -1)
                        {
                            XdmNode node = heads[hindex].CreateNode();
                            node.Load(part.fileReader);
                            if (page.next[k] == MixedLeaf)
                                ((XdmElement)node).LoadTextValue(part.fileReader);
                            nodes[k] = node;
                        }
                    }
                    page.nodes = nodes;
                    AddCache(page);
                }
                Interlocked.Increment(ref miss_count);
                return nodes;
#if DEBUG
            }
            finally
            {
                PerfMonitor.Global.End("PageFile.ReadPage()");
            }
#endif
        }

        private void WritePage(Page page)
        {
            PageFilePart part = parts[page.partid];
            lock (part.fileStream)
            {
                if (!closed && !page.stored)
                {
                    part.fileStream.Seek(0, SeekOrigin.End);
                    page.offset = part.fileStream.Position;
                    for (int k = 0; k < page.nodes.Length; k++)
                    {
                        XdmNode node = page.nodes[k];
                        if (node != null)
                        {
                            node.Store(part.fileWriter);
                            if (page.next[k] == MixedLeaf)
                                ((XdmElement)node).StoreTextValue(part.fileWriter);
                        }
                    }
                    page.stored = true;
                }
            }
            page.nodes = null;
            RemoveCache(page);
        }

        private void OptimizeCache()
        {            
            if (cacheCount > workset + workset_delta)
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
                if (workset < cacheCount)
                {
                    int len;
                    Page[] pages;
                    Page[] cached_pages;
                    bool lockTaken = false;
                    cacheLock.Enter(ref lockTaken);
                    len = cache.Count - (int)workset;
                    if (len <= 0)
                    {
                        if (lockTaken)
                            cacheLock.Exit();
                        return;
                    }
                    pages = new Page[len];
                    cached_pages = new Page[cache.Count];
                    cache.CopyTo(cached_pages);
                    if (lockTaken)
                        cacheLock.Exit(); 
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
                    if (parts == null)
                        CreateFile();
                    List<Page>[] slices = new List<Page>[partSize];
                    for (int i = 0; i < partSize; i++)
                        slices[i] = new List<Page>();
                    for (int k = 0; k < len; k++)
                        if (pages[k] != null)
                        {
                            Page p = pages[k];
                            slices[p.partid].Add(p);
                        }
                    for (int i = 0; i < partSize; i++)
                        Task.Factory.StartNew((object o) =>
                            {
                                List<Page> page_list = (List<Page>)o;
                                foreach (Page p in page_list)
                                    WritePage(p);
                            }, slices[i]);
                }
            }            
        }

        public void AddNode(int parent, DmNode head, XdmNode node)
        {
            if (lastpage == null ||
                lastcount == pagesize)
            {
                if (lastpage != null)
                    AddCache(lastpage);
                lastpage = new Page(pagesize);
                lastpage.partid = (short)(pagecount % partSize); 
                lastpage.pin = 1;
                lastpage.num = pagecount++;
                lastcount = 0;
                pagelist.Add(lastpage);
            }
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
            lastpage.next[lastcount] = count + 1;
            lastpage.parent[lastcount] = parent;
            lastnode = node;
            lastcount++;
            count++;
        }

        internal XdmNode LastNode
        {
            get
            {
                return lastnode;
            }
        }

        private sealed class Page
        {
            internal short partid;
            internal int num;
            internal long offset;
            internal bool stored;
            internal XdmNode[] nodes;
            internal int pin;            
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

        private struct PageFilePart
        {
            public string fileName;
            public Stream fileStream;
            public XdmWriter fileWriter;
            public XdmReader fileReader;
        }

        private sealed class PageFileOptimizer
        {
            private object syncRoot;
            private Timer timer;
            private bool active;
            private LinkedList<WeakReference> pagefiles; 

            public PageFileOptimizer()
            {
                syncRoot = new Object();
                pagefiles = new LinkedList<WeakReference>();
                timer = new Timer(OnTimer);
                active = false;
            }

            public void Add(PageFile p)
            {
                lock (syncRoot)
                {
                    pagefiles.AddLast(new WeakReference(p));
                    if (!active)
                    {
                        timer.Change(50, 0);
                        active = true;
                    }
                }
            }

            public void OnTimer(object context)
            {
                lock (syncRoot)
                {
                    LinkedListNode<WeakReference> curr = pagefiles.First;
                    while (curr != null)
                    {
                        PageFile p = (PageFile)curr.Value.Target;
                        LinkedListNode<WeakReference> next = curr.Next;
                        if (p == null || p.closed)
                            pagefiles.Remove(curr);
                        else                               
                            p.OptimizeCache();                        
                        curr = next;
                    }
                    if (pagefiles.Count == 0)
                        active = false;
                    else
                        timer.Change(50, 0);
                }
            }

            public Object SyncRoot
            {
                get
                {
                    return syncRoot;
                }
            }
        }

        private static PageFileOptimizer _optimizer = new PageFileOptimizer();
    }
}
