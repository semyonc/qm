//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using System.Drawing;
using System.Xml;

namespace XQueryConsole
{   
    public class FileTreeController : DispatcherObject
    {
        private TreeView dataTree;
        private string basePath;
        private FileSystemWatcher watcher;

        public TreeView TreeControl
        {
            get
            {
                return dataTree;
            }
        }

        public String SelectedFileName
        {
            get
            {
                if (dataTree.SelectedNode != null)
                    return dataTree.SelectedNode.Tag as String;
                return null;
            }
        }

        public event EventHandler FileOpen
        {
            add
            {
                dataTree.DoubleClick += value;
            }
            remove
            {
                dataTree.DoubleClick -= value;
            }
        }

        public bool MonitorFolders
        {
            get
            {
                if (watcher == null)
                    return false;
                return watcher.EnableRaisingEvents;
            }
            set
            {
                if (watcher == null)
                {
                    watcher = new FileSystemWatcher();
                    watcher.Path = basePath;
                    watcher.Filter = "*.x*";
                    watcher.IncludeSubdirectories = true;
                    watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
                    watcher.Created += new FileSystemEventHandler(watcher_Created);
                    watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
                    watcher.Deleted += new FileSystemEventHandler(watcher_Deleted);
                }
                watcher.EnableRaisingEvents = value;
            }
        }

        public FileTreeController(string basePath)
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(16, 16);
            imageList.Images.Add(Extensions.GetImageFromResourceUri("/XQueryConsole;component/Images/Folder1.png"));
            imageList.Images.Add(Extensions.GetImageFromResourceUri("/XQueryConsole;component/Images/Folder2.png"));
            imageList.Images.Add(Extensions.GetImageFromResourceUri("/XQueryConsole;component/Images/Item.png"));
            dataTree = new TreeView();
            dataTree.Dock = DockStyle.Fill;
            dataTree.BorderStyle = BorderStyle.FixedSingle;
            dataTree.ImageList = imageList;
            dataTree.ShowLines = false;
            dataTree.AfterExpand += new TreeViewEventHandler(dataTree_AfterExpand);
            dataTree.AfterCollapse += new TreeViewEventHandler(dataTree_AfterCollapse);
            dataTree.Font = new Font("Tahoma", 8.25f);

            Extensions.SetTreeViewTheme(dataTree.Handle);
            
            this.basePath = basePath;
            Reload();
        }

        private void dataTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
        }

        private void dataTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
        }

        private void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Reload();
            }));
            
        }

        private void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Reload();
            }));
        }

        private void watcher_Created(object sender, FileSystemEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Reload();
            }));
        }

        public void ChangeBasePath(string basePath)
        {
            this.basePath = basePath;
            if (watcher != null)
                watcher.Path = basePath;
            Reload();
        }

        private void Reload()
        {
            if (basePath != String.Empty && Directory.Exists(basePath))
            {
                DirectoryInfo rootDir = new DirectoryInfo(basePath);
                dataTree.BeginUpdate();
                dataTree.Nodes.Clear();
                foreach (DirectoryInfo child_di in rootDir.GetDirectories())
                {
                    TreeNode node = new TreeNode(child_di.Name, 0, 0);
                    dataTree.Nodes.Add(node);
                    LoadFileTree(node, child_di);
                }
                foreach (FileInfo fi in rootDir.GetFiles("*.xq"))
                {
                    TreeNode node = new TreeNode(fi.Name, 2, 2);
                    node.Tag = fi.FullName;
                    dataTree.Nodes.Add(node);
                }
                foreach (FileInfo fi in rootDir.GetFiles("*.xsql"))
                {
                    TreeNode node = new TreeNode(fi.Name, 2, 2);
                    node.Tag = fi.FullName;
                    dataTree.Nodes.Add(node);
                }
                dataTree.EndUpdate();
            }
        }

        private void LoadFileTree(TreeNode parent, DirectoryInfo di)
        {
            foreach (DirectoryInfo child_di in di.GetDirectories())
            {
                TreeNode node = new TreeNode(child_di.Name, 0, 0);
                parent.Nodes.Add(node);
                LoadFileTree(node, child_di);
            }
            string showcaseFile = Path.Combine(di.FullName, "showcase.xml");
            if (File.Exists(showcaseFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(showcaseFile);
                LoadSampleTree(di, doc.DocumentElement, parent);
            }
            else
            {
                foreach (FileInfo fi in di.GetFiles("*.xq"))
                {
                    TreeNode node = new TreeNode(fi.Name, 2, 2);
                    node.Tag = fi.FullName;
                    parent.Nodes.Add(node);
                }
                foreach (FileInfo fi in di.GetFiles("*.xsql"))
                {
                    TreeNode node = new TreeNode(fi.Name, 2, 2);
                    node.Tag = fi.FullName;
                    parent.Nodes.Add(node);
                }
            }
        }

        private void LoadSampleTree(DirectoryInfo di, XmlElement xmlElement, TreeNode parent)
        {
            foreach (XmlNode node in xmlElement.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                    continue;
                XmlElement elem = (XmlElement)node;
                if (elem.Name == "folder")
                {
                    TreeNode treeNode = new TreeNode(elem.GetAttribute("name"), 0, 0);
                    parent.Nodes.Add(treeNode);
                    LoadSampleTree(di, elem, treeNode);
                }
                else if (elem.Name == "item")
                {
                    TreeNode treeNode = new TreeNode(elem.GetAttribute("name"), 2, 2);                    
                    treeNode.Tag = Path.Combine(di.FullName, elem.InnerText);
                    parent.Nodes.Add(treeNode);
                }
            }
        }
    }
}
