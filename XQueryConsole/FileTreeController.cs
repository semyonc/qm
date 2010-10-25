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

namespace XQueryConsole
{   
    public class FileTreeController : DispatcherObject
    {
        private TreeView dataTree;
        //private DocumentController documentController;
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
                    watcher.Filter = "*.xq";
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
            Extensions.SetWindowTheme(dataTree.Handle, "explorer", null);
            
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
                    TreeNode node = new TreeNode(
                        Path.GetFileNameWithoutExtension(fi.Name), 2, 2);
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
            foreach (FileInfo fi in di.GetFiles("*.xq"))
            {
                TreeNode node = new TreeNode(
                    Path.GetFileNameWithoutExtension(fi.Name), 2, 2);
                node.Tag = fi.FullName;
                parent.Nodes.Add(node);
            }
        }
    }
}
