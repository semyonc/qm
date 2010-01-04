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
using DataEngine.XQuery.DocumentModel;

namespace DataEngine.XQuery
{
    sealed class PageFile: IDisposable
    {
        private string fileName;
        private Stream fileStream;
        private BinaryWriter binWriter;
        private BinaryReader binReader;
        private int pagesize;
        private int min_workset;
        private int max_workset;
        private int workset_delta; 
        private int count;
        private int lastcount;
        private int blockcount;
        private double workset;
        private long hit_count;
        private long miss_count;
        private int pincount;
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
        
        public PageFile(bool large)
        {
            if (large)
            {
                pagesize = 800;
                min_workset = 3;
                max_workset = 15;
                workset_delta = 5;
            }
            else
            {
                pagesize = 16;
                min_workset = 100;
                max_workset = 300;
                workset_delta = 150;
            }
            pagelist = new List<Page[]>();
            cached = new List<Page>();
            heads = new List<DmNode>();
            workset = min_workset;
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
            if (fileStream != null)
            {
                fileStream.Close();
                File.Delete(fileName);
                fileStream = null;
            }
        }
   
        public void Get(int index, bool load, out DmNode head, out XdmNode node)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException("index");            
            int pagenum = index / pagesize;
            Page[] block = pagelist[pagenum / XQueryBlockSize];
            Page page = block[pagenum % XQueryBlockSize];
            int k = index % pagesize;
            if (load)
                hit_count++;
            int hindex = page.hindex[k];
            if (hindex == -1)
                head = null;
            else
                head = heads[hindex];
            if (page.nodes == null)
            {
                if (load)
                {
                    ReadPage(page);
                    node = page.nodes[k];
                }
                else
                    node = null;
            }
            else
                node = page.nodes[k];            
        }

        public DmNode Head(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException("index");
            int pagenum = index / pagesize;
            Page[] block = pagelist[pagenum / XQueryBlockSize];
            Page page = block[pagenum % XQueryBlockSize];
            int hindex = page.hindex[index % pagesize];
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
        
        private void ReadPage(Page page)
        {
            page.nodes = new XdmNode[pagesize];
            fileStream.Seek(page.offset, SeekOrigin.Begin);
            for (int k = 0; k < page.nodes.Length; k++)
            {
                int hindex = page.hindex[k];
                if (hindex != -1)
                {
                    XdmNode node = heads[hindex].CreateNode();
                    node.Load(this);
                    page.nodes[k] = node;
                }
            }
            page.pin = ++pincount;
            cached.Add(page);
            miss_count++;
            OptimizeCache();
        }

        private void WritePage(Page page)
        {
            if (!page.stored)
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
                        node.Store(this);
                }
                page.stored = true;
            }            
            page.nodes = null;
            cached.Remove(page);            
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
                        workset = Math.Ceiling((workset * ratio1) / ratio);
                        if (workset < min_workset)
                            workset = min_workset;
                        else if (workset > max_workset)
                            workset = max_workset;
                    }
                    ratio = ratio1;
                }
                Page[] pages = cached.ToArray();
                Array.Sort<Page>(pages, (Page x, Page y) =>
                    {
                        if (x.pin < y.pin)
                            return -1;
                        else if (x.pin > y.pin)
                            return 1;
                        else
                        {
                            if (x.stored && !y.stored)
                                return 1;
                            else if (!x.stored && y.stored)
                                return -1;
                            else
                                return 0;
                        }
                    });
                for (int k = 0; k < pages.Length && cached.Count > workset; k++)
                    WritePage(pages[k]);
            }
        }

        public void AddNode(DmNode head, XdmNode node)
        {
            if (lastpage == null ||
                lastcount == pagesize)
            {
                if (lastpage != null)
                {
                    cached.Add(lastpage);
                    OptimizeCache();
                }
                lastpage = new Page(pagesize);
                lastpage.pin = ++pincount;
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

            public Page(int pagesize)
            {
                hindex = new int[pagesize];
                for (int k = 0; k < pagesize; k++)
                    hindex[k] = -1;
                nodes = new XdmNode[pagesize];
                next = new int[pagesize];
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
