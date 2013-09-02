//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Threading;

using DataEngine;
using Data.Remote;

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for PreferencesDialog.xaml
    /// </summary>
    public partial class PreferencesDialog : Window, INotifyPropertyChanged
    {
        private DocumentController controller;

        public static PreferencesDialog ShowDialog(DocumentController controller, bool focusQuerisFolder)
        {
            PreferencesDialog dlg = new PreferencesDialog(controller);
            dlg.Owner = System.Windows.Application.Current.MainWindow;
            if (focusQuerisFolder)
                dlg.FocusAndSelect();
            dlg.ShowDialog();
            if (dlg.ReloadDatasources)
            {
                MainWindow main = (MainWindow)System.Windows.Application.Current.MainWindow;
                main.DatasourceController.Reload();
            }
            return dlg;
        }

        public PreferencesDialog(DocumentController controller)
        {
            InitializeComponent();
            
            this.controller = controller;
            MyQueriesPath = controller.MyQueriesPath;
            SearchPath = controller.SearchPath;
            DefaultPanel = controller.DefaultPanel;
            EnableServerQuery = controller.EnableServerQuery;
            LimitSQLQueryResults = controller.LimitSQLQueryResults;
            searchPathTextBox.Focus();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public string MyQueriesPath { get; set; }
        public string SearchPath { get; set; }
        public StartupPanel DefaultPanel { get; set; }
        public bool EnableServerQuery { get; set; }
        public bool LimitSQLQueryResults { get; set; }
        
        public bool ReloadDatasources { get; set; }        

        public void FocusAndSelect()
        {
            Dispatcher.BeginInvoke(new Action(() => 
            {
                OptionTab.SelectedIndex = 1;
                queriesFolderTextBox.SelectAll();
                queriesFolderTextBox.Focus();
            }));
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = MyQueriesPath;
                if (dlg.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK)
                {
                    MyQueriesPath = dlg.SelectedPath;
                    OnPropertyChanged("MyQueriesPath");
                }
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            controller.MyQueriesPath = MyQueriesPath;
            controller.SearchPath = SearchPath;
            controller.DefaultPanel = DefaultPanel;
            controller.EnableServerQuery = EnableServerQuery;
            controller.LimitSQLQueryResults = LimitSQLQueryResults;
            controller.SaveSettings();
            DialogResult = true;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            SearchPath = SearchPathDialog.Edit(SearchPath);
            OnPropertyChanged("SearchPath");
        }
    }

    
}
