//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

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
    sealed class PageFile: IDisposable
    {
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

        private Page lastpage;
        private List<Page> pagelist;
        private List<DmNode> heads;

        private int partSize;
        private volatile int cacheCount;
        private PageFilePart[] parts;
        private readonly HashSet<Page> cache;
        private SpinLock cacheLock;
        private StringBuilder ltb;
        private DmNode lastnode;

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
            partSize = 1;
            ltb = new StringBuilder();
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
                part.reader = new BinaryReader(part.fileStream);
                part.writer = new BinaryWriter(part.fileStream);
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
   
        public void Get(int index, out DmNode head, out int parent)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException("index");
            int pagenum = index / pagesize;
            Page page = pagelist[pagenum];
            int k = index % pagesize;
            XNode node = page.nodes[k];
            if (node.hindex == -1)
                head = null;
            else
                head = heads[node.hindex];
            parent = node.parent;
        }

        public int Get(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException("index");
            int pagenum = index / pagesize;
            Page page = pagelist[pagenum];
            return page.nodes[index % pagesize].child;
        }

        public String GetValue(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException("index");
            int pagenum = index / pagesize;
            Page page = pagelist[pagenum];
            Interlocked.Increment(ref page.pin);
            Interlocked.Increment(ref hit_count);
            int k = index % pagesize;
            TextData td = page.textData;
            if (td == null)
            {
                td = ReadPage(page);
                return new String(td.tbuf, td.t[k].pos, td.t[k].len);
            }
            else
            {
                if (td.tbuf == null)
                    return ltb.ToString(td.t[k].pos, td.t[k].len);
                return new String(td.tbuf, td.t[k].pos, td.t[k].len);
            }
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
                return page.nodes[index % pagesize].next;
            }
            set
            {
                if (index < 0 || index >= count)
                    throw new ArgumentException("index");
                Page page = pagelist[index / pagesize];
                page.nodes[index % pagesize].next = value;
            }
        }

        public DmNode GetHead(int index)
        {
            DmNode head;
            int parent;
            Get(index, out head, out parent);
            return head;
        }

        public void Update(int index, int pos)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException("index");
            Page page = pagelist[index / pagesize];
            int k = index % pagesize;
            if (page.nodes[k].child == -1)
                page.nodes[k].child = pos;
        }

        public int Select(HashSet<int> targets, ref int index, ref int length, int[] buffer)
        {
            int count = 0;
            int size = buffer.Length;
            Page curr = pagelist[index / pagesize];
            int k = index % pagesize;
            while (length > 0 && count < size)
            {
                if (targets == null || targets.Contains(curr.nodes[k].hindex))
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

        private TextData ReadPage(Page page)
        {
#if DEBUG
            try
            {
                PerfMonitor.Global.Begin("PageFile.ReadPage()");
#endif
                TextData textData = new TextData(pagesize);
                PageFilePart part = parts[page.partid];
                lock (part.fileStream)
                {
                    part.fileStream.Seek(page.offset, SeekOrigin.Begin);
                    textData = new TextData(pagesize);
                    textData.Read(part.reader);
                    page.textData = textData;                                        
                    AddCache(page);
                }
                Interlocked.Increment(ref miss_count);
                return textData;
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
                    page.textData.Write(part.writer);
                    page.textData = null; 
                    page.stored = true;
                }
            }
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

        public void AddNode(int parent, DmNode head, String text)
        {
            if (lastpage == null ||
                lastcount == pagesize)
            {
                if (lastpage != null)
                {
                    lastpage.textData.tbuf = new char[ltb.Length];
                    ltb.CopyTo(0, lastpage.textData.tbuf, 0, ltb.Length);                    
                    AddCache(lastpage);
                }
                ltb.Clear();
                lastpage = new Page(pagesize);
                lastpage.partid = (short)(pagecount % partSize); 
                lastpage.pin = 1;
                lastpage.num = pagecount++;
                lastcount = 0;
                pagelist.Add(lastpage);
            }
            XNode node;
            node.next = 0;
            node.child = -1;
            node.parent = parent;
            if (head == null)
                node.hindex = -1;
            else
            {
                if (head._index == -1)
                {
                    head._index = heads.Count;
                    heads.Add(head);
                }
                node.hindex = head._index;
            }
            lastpage.nodes[lastcount] = node;
            TextNode tn;            
            tn.pos = ltb.Length;
            if (text != null)
            {
                tn.len = text.Length;
                ltb.Append(text);
            }
            else
                tn.len = 0;
            lastpage.textData.t[lastcount] = tn;
            lastnode = head;
            lastcount++;
            count++;
        }

        public void LastNodeAppendValue(String text)
        {
            if (lastcount == 0)
                throw new InvalidOperationException("LastNodeAppendValue");
            if (text == null)
                throw new ArgumentNullException("text");
            int index = lastcount - 1;
            lastpage.textData.t[index].len += text.Length;
            ltb.Append(text);            
        }

#if DEBUG
        public void Dump(TextWriter writer)
        {
            DmNode node;
            int parent;
            writer.WriteLine("{0,5}   next child parent", "@"); 
            for (int k = 0; k < Count; k++)
            {
                if (k > 0)
                    writer.WriteLine();
                Get(k, out node, out parent);
                writer.Write("{0:00000}: {1,5} {2,5} {3,6} | {4} {5}",
                    k, this[k], Get(k), parent, node, Lisp.EscapeString(GetValue(k))); 
            }
        }
#endif

        private struct TextNode
        {
            public int pos;
            public int len;
        }

        private sealed class TextData
        {
            internal char[] tbuf;
            internal TextNode[] t;

            public TextData(int pagesize)
            {
                t = new TextNode[pagesize];
            }

            public void Read(BinaryReader reader)
            {
                int len = reader.ReadInt32();
                tbuf = new char[len];
                reader.Read(tbuf, 0, len);
                for (int k = 0; k < t.Length; k++)
                {
                    t[k].pos = reader.ReadInt32();
                    t[k].len = reader.ReadInt32();
                }
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(tbuf.Length);
                writer.Write(tbuf, 0, tbuf.Length);
                for (int k = 0; k < t.Length; k++)
                {
                    writer.Write(t[k].pos);
                    writer.Write(t[k].len);
                }
            }
        }

        private struct XNode
        {
            public int next;
            public int hindex;
            public int parent;
            public int child;
        }

        private sealed class Page
        {
            internal short partid;
            internal int num;
            internal long offset;
            internal bool stored;
            internal int pin;            
            
            internal XNode[] nodes;                        
            internal TextData textData;
            
            public Page(int pagesize)
            {
                nodes = new XNode[pagesize];
                textData = new TextData(pagesize);
                stored = false;
            }
        }

        private struct PageFilePart
        {
            public string fileName;
            public Stream fileStream;
            public BinaryWriter writer;
            public BinaryReader reader;
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
