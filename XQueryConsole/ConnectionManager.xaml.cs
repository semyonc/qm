//        Copyright (c) 2010-2012, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Data.ConnectionUI;

using Forms = System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Timers;
using Data.Remote;
using DataEngine;

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for ConnectionBuilder.xaml
    /// </summary>
    public partial class ConnectionManager : Window, INotifyPropertyChanged 
    {
        private System.Windows.Forms.PropertyGrid propertyEditor;
        private ObservableCollection<Connection> connections;
        private Connection current;
        private Timer timer;
        private DataConnectionConfiguration dcs;

        public static readonly ICommand NewConnCommand =
            new RoutedUICommand("New Connection", "NewConnection", typeof(ConnectionManager));

        public static bool ShowDialog(DataSourceTreeController dataController)
        {
            ConnectionManager dlg = new ConnectionManager();
            dlg.Owner = Application.Current.MainWindow;
            dlg.connections = new ObservableCollection<Connection>(dataController.Container.connections);
            dlg.connectionList.ItemsSource = dlg.connections;
            if (dlg.ShowDialog() == true)
            {
                dataController.Container.connections = dlg.connections.ToArray();
                return true;
            }
            return false;
        }

        public ConnectionManager()
        {
            InitializeComponent();
            propertyEditor = new Forms.PropertyGrid();
            propertyEditor.Dock = Forms.DockStyle.Fill;
            propertyEditorHost.Child = propertyEditor;
            propertyEditor.PropertyValueChanged += new Forms.PropertyValueChangedEventHandler(propertyEditor_PropertyValueChanged);

            SourceInitialized += (x, y) => Extensions.HideMinimizButton(this);
            timer = new Timer(150);
            timer.Enabled = false;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            newItemMenu.ItemsSource = DbProviderFactories.GetFactoryClasses().Rows;
            DataContext = this;

            dcs = new DataConnectionConfiguration(Extensions.GetAppDataPath());
        }

        public string ConnectionString
        {
            get
            {
                if (current == null)
                    return "";
                return current.ConnectionString;
            }
            set
            {
                current.ConnectionString = value;
                OnPropertyChanged("ConnectionString");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void connectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            timer.Enabled = false;
            if (e.AddedItems != null && e.AddedItems.Count > 0)
                current = (Connection)e.AddedItems[0];
            else
            {
                current = null;
                propertyEditor.SelectedObject = null;
            }
            OnPropertyChanged("ConnectionString");
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (current != null)
                    {
                        ConnectionProperties props = new ConnectionProperties(current, 
                            connectionStringTextBox.Text);
                        if (props.IsValid)
                        {
                            connectionStringTextBox.Foreground = SystemColors.ControlTextBrush;
                            props.Update();
                        }
                        else
                            connectionStringTextBox.Foreground = Brushes.Red;
                        propertyEditor.SelectedObject = props;
                    }
                }));
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = connectionList.SelectedIndex;
            if (selectedIndex != -1)
            {                
                connections.RemoveAt(selectedIndex);
                if (connections.Count > 0)
                {
                    if (selectedIndex > connections.Count - 1)
                        selectedIndex = connections.Count - 1;
                    connectionList.SelectedIndex = selectedIndex;
                }
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = connectionList.SelectedIndex;
            if (selectedIndex != -1)
            {                
                Connection conn = connections[selectedIndex];                
                try
                {
                    Cursor = Cursors.Wait;
                    conn.TestConnection();
                    MessageBox.Show("Test connection succeeded.", "Test results", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Test results",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Cursor = Cursors.Arrow;
            }
        }

        private void propertyEditor_PropertyValueChanged(object s, Forms.PropertyValueChangedEventArgs e)
        {
            ConnectionProperties props = (ConnectionProperties)propertyEditor.SelectedObject;
            if (props.IsConnectionBuilderProperty(e.ChangedItem.PropertyDescriptor))
            {
                props.Update();
                OnPropertyChanged("ConnectionString");
            }
        }

        private void connectionStringTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            timer.Enabled = false;
            timer.Enabled = true;
        }

        private string CreateUniqueConnPrefix()
        {
            for (int index = 1; ; index++)
            {
                string prefix = String.Format("CONN{0}", index);
                if (connections.FirstOrDefault((conn) => conn.Prefix == prefix) == null)
                    return prefix;
            }
        }

        private void NewConnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            timer.Enabled = false;
            DataRow row = (DataRow)e.Parameter;
            Connection conn = new Connection();
            conn.Accessor = AccessorType.DataProvider;
            conn.Prefix = CreateUniqueConnPrefix();
            conn.InvariantName = (string)row["InvariantName"];
            connections.Add(conn);
            connectionList.SelectedItem = conn;
            timer.Enabled = true;
        }

        private void NewMongoConnection_Click(object sender, RoutedEventArgs e)
        {
            timer.Enabled = false;
            Connection conn = new Connection();
            conn.InvariantName = "MongoDB.Driver";
            conn.Accessor = AccessorType.MongoDb;
            conn.Prefix = CreateUniqueConnPrefix();
            connections.Add(conn);
            connectionList.SelectedItem = conn;
            timer.Enabled = true;
        }   

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            timer.Enabled = false;
            using (DataConnectionDialog dlg = new DataConnectionDialog())
            {
                dcs.LoadConfiguration(dlg);
                if (DataConnectionDialog.Show(dlg, this.GetIWin32Window()) ==
                        System.Windows.Forms.DialogResult.OK)
                {
                    Connection conn = new Connection();
                    conn.Prefix = CreateUniqueConnPrefix();
                    conn.InvariantName = dlg.SelectedDataProvider.Name;
                    conn.ConnectionString = dlg.ConnectionString;
                    connections.Add(conn);
                    connectionList.SelectedItem = conn;
                    timer.Enabled = true;
                    dcs.SaveConfiguration(dlg);
                }
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }             
    }
}
