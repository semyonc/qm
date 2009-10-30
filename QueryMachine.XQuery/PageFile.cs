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

namespace DataEngine.XQuery
{
    class PageFile: IDisposable
    {
        private string fileName;
        private Stream fileStream;
        private BinaryWriter binWriter;
        private BinaryReader binReader;
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
        private XQueryNodeInfoTable nodeInfoTable;
        private XQuerySchemaInfoTable schemaInfoTable;
        private Dictionary<int, XdmNode> crosslink;

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

        public const int XQueryBlockSize = 100;
        public const int XQueryPageSize = 500;
        public const int XQueryMinWorkset = 3;
        public const int XQueryMaxWorkset = 15;
        public const int XQueryWorksetDelta = 5;

        public PageFile(XQueryNodeInfoTable nodeInfoTable, XQuerySchemaInfoTable schemaInfoTable)
        {
            pagelist = new List<Page[]>();
            cached = new List<Page>();
            this.nodeInfoTable = nodeInfoTable;
            this.schemaInfoTable = schemaInfoTable;
            workset = XQueryMinWorkset;
            crosslink = new Dictionary<int, XdmNode>();
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

        public XQueryNodeInfo ReadNodeInfo()
        {
            return nodeInfoTable.Get(binReader.ReadInt32());
        }

        public void WriteNodeInfo(XQueryNodeInfo nodeInfo)
        {
            binWriter.Write(nodeInfo.handle);
        }

        public IXmlSchemaInfo ReadSchemaInfo()
        {
            return schemaInfoTable.Get(binReader.ReadInt32());
        }

        public void WriteSchemaInfo(IXmlSchemaInfo schemaInfo)
        {
            binWriter.Write(schemaInfoTable.GetHandle(schemaInfo));
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
   
        public XdmNode GetNode(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException("index");
            int pagenum = index / XQueryPageSize;
            Page[] block = pagelist[pagenum / 100];
            Page page = block[pagenum % 100];
            hit_count++;
            if (page.nodes == null)
                ReadPage(page);
            return page.nodes[index % XQueryPageSize];            
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public XdmNode this[int index]
        {
            get
            {
                return GetNode(index);
            }
        }

        public XQueryNodeInfoTable NodeInfoTable
        {
            get
            {
                return nodeInfoTable;
            }
        }

        public XQuerySchemaInfoTable SchemaInfoTable
        {
            get
            {
                return schemaInfoTable;
            }
        }

        private void ReadPage(Page page)
        {
            page.nodes = new XdmNode[XQueryPageSize];
            fileStream.Seek(page.offset, SeekOrigin.Begin);
            int num = page.num * XQueryPageSize;
            for (int k = 0; k < page.nodes.Length; k++)
            {
                XdmNode node;
                if (page.crosslink && crosslink.TryGetValue(num++, out node))
                    page.nodes[k] = node;
                else
                {
                    XdmNodeType nodeType = (XdmNodeType)binReader.ReadByte();
                    switch (nodeType)
                    {
                        case XdmNodeType.ElementStart:
                            node = new XdmElementStart();
                            break;

                        case XdmNodeType.ElementEnd:
                            node = new XdmElementEnd();
                            break;

                        case XdmNodeType.Text:
                            node = new XdmText();
                            break;

                        case XdmNodeType.Document:
                            node = new XdmDocument();
                            break;

                        case XdmNodeType.Pi:
                            node = new XdmProcessingInstruction();
                            break;

                        case XdmNodeType.Comment:
                            node = new XdmComment();
                            break;

                        case XdmNodeType.Whitespace:
                            node = new XdmWhitespace();
                            break;

                        case XdmNodeType.Cdata:
                            node = new XdmCdata();
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
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
                int num = page.num * XQueryPageSize;
                for (int k = 0; k < page.nodes.Length; k++)
                {
                    XdmNode node = page.nodes[k];
                    if (node != lastnode && node.Completed)
                    {
                        binWriter.Write((byte)node.NodeType);
                        node.Store(this);
                    }
                    else
                    {
                        crosslink.Add(num + k, node);
                        page.crosslink = true;
                    }
                }
                page.stored = true;
            }            
            page.nodes = null;
            cached.Remove(page);            
        }

        private void OptimizeCache()
        {
            if (cached.Count > workset + XQueryWorksetDelta)
            {
                if (miss_count > 0 && hit_count > 0)
                {
                    double ratio1 = (double)miss_count / hit_count;
                    if (ratio > 0)
                    {
                        workset = Math.Ceiling((workset * ratio1) / ratio);
                        if (workset < XQueryMinWorkset)
                            workset = XQueryMinWorkset;
                        else if (workset > XQueryMaxWorkset)
                            workset = XQueryMaxWorkset;
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

        public void AddNode(XdmNode node)
        {
            if (lastpage == null ||
                lastcount == XQueryPageSize)
            {
                if (lastpage != null)
                {
                    cached.Add(lastpage);
                    OptimizeCache();
                }
                lastpage = new Page();
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
            lastpage.nodes[lastcount++] = node;
            lastnode = node;
            count++;
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
            internal bool crosslink;
            internal XdmNode[] nodes;

            public Page()
            {
                nodes = new XdmNode[XQueryPageSize];
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
