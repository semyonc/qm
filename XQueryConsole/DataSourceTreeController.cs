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
using System.Data;
using System.Data.Common;
using System.Xml;
using System.Xml.Serialization;

using DataEngine;

namespace XQueryConsole
{
    public class DataSourceTreeController : DispatcherObject
    {
        private TreeView dataTree;
        private DocumentController documentController;
        private string configurationPath;
        private bool toolTipShown;
        private DatabaseDictionary _dictionary;
        
        class Node : TreeNode
        {
            public Connection Connection { get; set;  }
            public bool HasExpand { get; internal set; }

            public Node(String text, int imageIndex)
            {
                Text = text;
                ImageIndex =
                    SelectedImageIndex = imageIndex;
                HasExpand = false;
            }

            public Node(Connection conn)
            {
                Connection = conn;
                HasExpand = false;
            }

            public virtual void LoadChilds()
            {
            }
        }

        class DatabaseNode : Node
        {
            public XmlNode Metadata { get; private set; }
            public DataRow Data { get; private set; }

            public DatabaseNode(Connection conn, XmlNode metadata, DataRow data)
                : this(conn)
            {
                this.Metadata = metadata;
                this.Data = data;
            }

            public DatabaseNode(Connection conn)
                : base(conn)
            {
                Text = conn.ToString();
                ImageIndex =
                    SelectedImageIndex = 1;
                Nodes.Add(new TreeNode());
            }

            private string GetValue(DataRow data, string formula)
            {
                if (formula == null || formula.Equals("null"))
                    return null;
                else
                    if (formula.StartsWith("{") && formula.EndsWith("}"))
                    {
                        string column = formula.Substring(1, formula.Length - 2);
                        object value = data[column];
                        if (value == DBNull.Value)
                            return null;
                        else
                            return (String)value;
                    }
                    else
                        return formula;
            }

            private string[] GetRestrictionValues(string restrictions)
            {
                if (String.IsNullOrEmpty(restrictions))
                    return null;
                string[] r = restrictions.Split(new char[] { ',' });
                for (int k = 0; k < r.Length; k++)
                    r[k] = GetValue(Data, r[k]);
                return r;
            }

            public override void LoadChilds()
            {
                if (Metadata == null)
                {
                    XmlDocument doc = new XmlDocument();
                    using (Stream stream = DataProviderHelper.GetConfigurationStream())
                    {
                        doc.Load(stream);
                        stream.Close();
                    }
                    Metadata = doc.SelectSingleNode(String.Format("//add[@invariant='{0}']/SchemaBrowser",
                       Connection.InvariantName));
                }
                if (Metadata != null)
                {
                    DbConnection dbConnection = DataProviderHelper.CreateDbConnection(Connection.InvariantName);
                    dbConnection.ConnectionString = Connection.ConnectionString;
                    dbConnection.Open();
                    try
                    {
                        foreach (XmlNode child in Metadata)
                        {
                            if (child.NodeType != XmlNodeType.Element || child.Name != "node")
                                continue;
                            XmlElement node = (XmlElement)child;
                            if (node.HasAttribute("collection"))
                            {
                                DataTable collection = dbConnection.GetSchema(node.GetAttribute("collection"),
                                    GetRestrictionValues(node.GetAttribute("restrictions")));
                                foreach (DataRow row in collection.Rows)
                                {
                                    Node childNode;
                                    XmlElement target = (XmlElement)node.SelectSingleNode("target");
                                    int imageIndex = -1;
                                    if (target != null)
                                    {
                                        childNode = new TableNode(Connection, GetValue(row, target.GetAttribute("schema")),
                                            GetValue(row, target.GetAttribute("name")));
                                        imageIndex = 3;
                                    }
                                    else
                                    {
                                        childNode = new DatabaseNode(Connection, node, row);
                                        imageIndex = 1;
                                    }
                                    childNode.Text = GetValue(row, node.GetAttribute("caption"));
                                    XmlElement image = (XmlElement)node.SelectSingleNode("image");
                                    if (image != null)
                                        imageIndex = Convert.ToInt32(image.GetAttribute("index"));
                                    childNode.SelectedImageIndex =
                                        childNode.ImageIndex = imageIndex;
                                    Nodes.Add(childNode);
                                }
                            }
                            else
                            {
                                DatabaseNode childNode = new DatabaseNode(Connection, node, null);
                                childNode.Text = GetValue(Data, node.GetAttribute("caption"));
                                XmlElement image = (XmlElement)node.SelectSingleNode("image");
                                if (image != null)
                                    childNode.SelectedImageIndex =
                                        childNode.ImageIndex = Convert.ToInt32(image.GetAttribute("index"));
                                Nodes.Add(childNode);
                            }
                        }
                    }
                    finally
                    {
                        dbConnection.Close();
                    }
                }
            }
        }

        class TableNode : Node
        {
            public string SchemaName { get; private set; }
            public string TableName { get; private set; }

            public TableNode(Connection conn, string schemaName, string tableName)
                : base(conn)
            {
                SchemaName = schemaName;
                TableName = tableName;
                Text = tableName;
            }

            public string QualifiedName
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Connection.Prefix);
                    sb.Append(":");
                    if (!String.IsNullOrEmpty(SchemaName))
                    {
                        sb.Append(SchemaName);
                        sb.Append(".");
                    }
                    sb.Append(TableName);
                    return sb.ToString();
                }
            }
        }

        public TreeView TreeControl
        {
            get
            {
                return dataTree;
            }
        }

        public ConnectionContainer Container { get; private set; }
        public bool Modified { get; private set; }
        public event EventHandler RightClick;
        public event EventHandler ShowTooltip;

        public DataSourceTreeController(DocumentController controller)
        {
            documentController = controller;
            configurationPath = Path.Combine(Extensions.GetAppDataPath(), 
                "ConnectionSettings.xml");
            
            Container = new ConnectionContainer();
           
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(16, 16);
            imageList.Images.Add(Extensions.GetImageFromResourceUri("/XQueryConsole;component/Images/NewConnection.png"));
            imageList.Images.Add(Extensions.GetImageFromResourceUri("/XQueryConsole;component/Images/DataSource.png"));
            imageList.Images.Add(Extensions.GetImageFromResourceUri("/XQueryConsole;component/Images/DefaultDataSource.png"));
            imageList.Images.Add(Extensions.GetImageFromResourceUri("/XQueryConsole;component/Images/Table.png"));
            imageList.Images.Add(Extensions.GetImageFromResourceUri("/XQueryConsole;component/Images/Field.png"));
            dataTree = new TreeView();            
            dataTree.Dock = DockStyle.Fill;
            dataTree.BorderStyle = BorderStyle.FixedSingle;
            dataTree.ImageList = imageList;
            dataTree.ShowLines = false;
            dataTree.Font = new Font("Tahoma", 8.25f);
            
            try
            {
                Extensions.SetWindowTheme(dataTree.Handle, "explorer", null);
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            
            dataTree.BeforeExpand += new TreeViewCancelEventHandler(dataTree_BeforeExpand);
            dataTree.AfterExpand += new TreeViewEventHandler(dataTree_AfterExpand);
            dataTree.MouseMove += new MouseEventHandler(dataTree_MouseMove);
            dataTree.MouseDown += new MouseEventHandler(dataTree_MouseDown);
            dataTree.MouseUp += new MouseEventHandler(dataTree_MouseUp);
            dataTree.ItemDrag += new ItemDragEventHandler(dataTree_ItemDrag);            
            
            LoadConnections();
            Fill();
        }

        public void Reload()
        {
            dataTree.Nodes.Clear();
            LoadConnections();
            Fill();
        }

        private void dataTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (ShowTooltip != null && !toolTipShown)
            {
                ShowTooltip(this, EventArgs.Empty);
                toolTipShown = true;
            }
        }

        private void dataTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            Node node = (Node)e.Node;
            if (!node.HasExpand)
            {
                Cursor.Current = Cursors.WaitCursor;
                dataTree.BeginUpdate();
                node.Nodes.Clear();
                try
                {
                    node.LoadChilds();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error accessing database",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                dataTree.EndUpdate();
                node.HasExpand = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void dataTree_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode nodeAt = dataTree.GetNodeAt(e.Location);
            if (nodeAt == dataTree.Nodes[0])
                AddConnection();
            else if (e.Button == MouseButtons.Right)
                dataTree.SelectedNode = nodeAt;
        }

        private void dataTree_MouseUp(object sender, MouseEventArgs e)
        {
            if (RightClick != null && e.Button == MouseButtons.Right)
            {
                TreeNode nodeAt = dataTree.GetNodeAt(e.Location);
                if (nodeAt != null && nodeAt != dataTree.Nodes[0])
                    RightClick(this, EventArgs.Empty);
            }
        }

        private void dataTree_MouseMove(object sender, MouseEventArgs e)
        {
            TreeNode nodeAt = dataTree.GetNodeAt(e.Location);
            if (nodeAt == dataTree.Nodes[0])
                dataTree.Cursor = Cursors.Hand;
            else
                dataTree.Cursor = Cursors.Default;
        }

        private void dataTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Item is TableNode)
            {
                TableNode node = (TableNode)e.Item;
                MainWindow main = (MainWindow)System.Windows.Application.Current.MainWindow;
                QueryPage current = main.SelectedPage;
                if (current != null)
                {
                    if (current.QueryFacade is XQueryFacade)
                        dataTree.DoDragDrop(String.Format("wmh:ds('{0}')", node.QualifiedName),
                            DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Scroll);
                    else
                    {
                        if (current.HasContent)
                            dataTree.DoDragDrop(GetTableName(e.Item), 
                                DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Scroll);
                        else
                            dataTree.DoDragDrop(String.Format("select * from {0}", GetTableName(e.Item)),
                                DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Scroll);
                    }
                }
            }
        }

        private void LoadConnections()
        {
            FileInfo fi = new FileInfo(configurationPath);
            if (fi.Exists)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConnectionContainer));
                FileStream fs = new FileStream(configurationPath, FileMode.Open);
                Container = (ConnectionContainer)serializer.Deserialize(fs);
                Container.Check();
                fs.Close();
            }
        }

        private void SaveConnections()
        {
            XmlSerializer serializer =
                new XmlSerializer(typeof(ConnectionContainer));
            TextWriter writer = new StreamWriter(configurationPath);
            serializer.Serialize(writer, Container);
            writer.Close();
        }

        private void Fill()
        {
            dataTree.BeginUpdate();
            dataTree.Nodes.Clear();
            Node node = new Node("Add connection", 0);
            node.NodeFont = new Font(dataTree.Font, FontStyle.Underline);
            node.ForeColor = Color.DarkBlue;
            dataTree.Nodes.Add(node);
            if (Container.connections != null)
                foreach (Connection conn in Container.connections)
                {
                    node = new DatabaseNode(conn);
                    if (conn.Default)
                        node.ImageIndex =
                            node.SelectedImageIndex = 2;
                    dataTree.Nodes.Add(node);
                }
            dataTree.EndUpdate();
        }

        public void SetDefaultConnection(Connection conn)
        {
            foreach (Node node in dataTree.Nodes)
                if (node.Connection != null)
                {
                    if (node.Connection == conn)
                    {
                        node.ImageIndex =
                            node.SelectedImageIndex = 2;
                    }
                    else
                    {
                        node.Connection.Default = false;
                        node.ImageIndex =
                            node.SelectedImageIndex = 1;
                    }
                }
            Modified = true;
        }

        public void AddConnection()
        {
            ConnectionDialog dlg = new ConnectionDialog();
            dlg.Owner = System.Windows.Application.Current.MainWindow;
            if (dlg.ShowDialog() == true)
            {
                Connection conn = dlg.Connection;
                Node node = new DatabaseNode(conn);
                if (conn.Default)
                    SetDefaultConnection(conn);
                Container.Add(conn);
                SaveConnections();
                dataTree.Nodes.Add(node);
                Modified = true;
            }
        }

        public void EditConnection()
        {
            Node node = (Node)dataTree.SelectedNode;
            ConnectionDialog dlg = new ConnectionDialog();
            dlg.Owner = System.Windows.Application.Current.MainWindow;
            dlg.Connection = node.Connection;
            if (dlg.ShowDialog() == true)
            {
                node.Connection = dlg.Connection;
                node.Text = node.Connection.ToString();
                node.Nodes.Clear();
                node.Nodes.Add(new TreeNode());
                node.HasExpand = false;
                if (dlg.Connection.Default)
                    SetDefaultConnection(node.Connection);
                else
                    node.ImageIndex =
                        node.SelectedImageIndex = 1;
                SaveConnections();
                Modified = true;
            }
        }

        public void RemoveConnection()
        {
            Node node = (Node)dataTree.SelectedNode;
            Container.Remove(node.Connection);
            dataTree.Nodes.Remove(node);
            SaveConnections();
            Modified = true;
        }

        public void RefreshConnection()
        {
            Node node = (Node)dataTree.SelectedNode;
            node.Collapse();
            node.Nodes.Clear();
            node.Nodes.Add(new TreeNode());
            node.HasExpand = false;
        }

        public DatabaseDictionary Dictionary
        {
            get
            {
                if (_dictionary == null || Modified)
                {
                    _dictionary = new DatabaseDictionary();
                    Modified = false;
                    if (Container.connections != null)
                        foreach (Connection setting in Container.connections)
                            _dictionary.RegisterDataProvider(setting.Prefix, setting.Default,
                                setting.InvariantName, setting.ConnectionString);
                }
                return _dictionary;
            }
        }

        private string GetTableName(object treeNode)
        {
            TableNode node = (TableNode)treeNode;
            StringBuilder sb = new StringBuilder();
            DataProviderHelper helper = new DataProviderHelper();
            if (!node.Connection.Default)
            {
                sb.Append(node.Connection.Prefix);
                sb.Append(":");
            }
            if (!String.IsNullOrEmpty(node.SchemaName))
            {
                sb.Append(helper.NativeFormatIdentifier(node.SchemaName));
                sb.Append(".");
            }
            sb.Append(helper.NativeFormatIdentifier(node.TableName));
            return sb.ToString();
        }
    }
}
