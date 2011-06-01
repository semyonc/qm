//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

using DataEngine;

namespace XQueryConsole
{
    public enum StartupPanel
    {
        SQL,
        XQuery
    }

    public class DocumentController: INotifyPropertyChanged
    {
        private string myQueriesPath;
        private string searchPath;
        private bool confirmFileSave;
        private bool enableServerQuery;
        private StartupPanel startupPanel;
        private bool limitSQLQueryResults;
        private bool showDragDropPromo;

        public DocumentController()
        {
            LoadSettings();
        }

        public string MyQueriesPath 
        {
            get
            {
                return myQueriesPath;
            }
            set
            {
                myQueriesPath = value;
                OnPropertyChanged("MyQueriesPath");
            }
        }

        public string SearchPath 
        {
            get
            {
                return searchPath;
            }
            set
            {
                searchPath = value;
                OnPropertyChanged("SearchPath");
            }
        }

        public bool ConfirmFileSave 
        {
            get
            {
                return confirmFileSave;
            }
            set
            {
                confirmFileSave = value;
                OnPropertyChanged("ConfirmFileSave");
            }
        }

        public bool EnableServerQuery
        {
            get
            {
                return enableServerQuery;
            }
            set
            {
                enableServerQuery = value;
                OnPropertyChanged("EnableServerQuery");
            }
        }

        public StartupPanel DefaultPanel
        {
            get
            {
                return startupPanel;
            }
            set
            {
                startupPanel = value;
                OnPropertyChanged("DefaultPanel");
            }
        }

        public bool LimitSQLQueryResults
        {
            get
            {
                return limitSQLQueryResults;
            }
            set
            {
                limitSQLQueryResults = value;
                OnPropertyChanged("LimitSQLQueryResults");
            }
        }

        public bool ShowDragDropPromo
        {
            get
            {
                return showDragDropPromo;
            }
            set
            {
                showDragDropPromo = value;
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\WMHelp Software\\QueryMachine", true);
                if (key == null)
                    key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\WMHelp Software\\QueryMachine");
                key.SetValue("ShowDragDropPromo", Convert.ToInt32(showDragDropPromo));
                key.Close();
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

        public String GetApplicationTitle()
        {
            return Path.GetFileNameWithoutExtension(
                Assembly.GetExecutingAssembly().Location);
        }

        private void LoadSettings()
        {
            showDragDropPromo = true;
            startupPanel = StartupPanel.XQuery;
            enableServerQuery = true;
            DataProviderHelper.HostADOProviders = true;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\WMHelp Software\\QueryMachine", true))
            {
                if (key != null)
                {
                    object confirmSave = key.GetValue("ConfirmSave");
                    if (confirmSave != null)
                        ConfirmFileSave = (int)confirmSave == 1;
                    else
                        ConfirmFileSave = true;
                    SearchPath = (string)key.GetValue("SearchPath");
                    MyQueriesPath = (string)key.GetValue("QueryPath");
                    
                    object startupPanelValue = key.GetValue("StartupPanel");
                    if (startupPanelValue != null)
                    {
                        switch ((int)startupPanelValue)
                        {
                            case 0:
                                startupPanel = StartupPanel.SQL;
                                break;
                            case 1:
                                startupPanel = StartupPanel.XQuery;
                                break;
                        }
                    }

                    object enableServerQueryValue = key.GetValue("EnableServerQuery");
                    if (enableServerQueryValue != null)
                        enableServerQuery = (int)enableServerQueryValue == 1;

                    object hostADOProvidersValue = key.GetValue("HostADOProviders");
                    if (hostADOProvidersValue != null)
                        DataProviderHelper.HostADOProviders = (int)hostADOProvidersValue == 1;

                    object limitSQLQueryResultsValue = key.GetValue("LimitQueryResults");
                    if (limitSQLQueryResultsValue != null)
                        limitSQLQueryResults = (int)limitSQLQueryResultsValue == 1;

                    object showDragDropPromoValue = key.GetValue("ShowDragDropPromo");
                    if (showDragDropPromoValue != null)
                        showDragDropPromo = (int)showDragDropPromoValue == 1;

                    key.Close();
                }

                if (SearchPath == null)
                    SearchPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (MyQueriesPath == null)
                {
                    MyQueriesPath = Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments), "XQuery");
                    if (key == null && !Directory.Exists(MyQueriesPath))
                        Directory.CreateDirectory(MyQueriesPath);
                }
            }
        }

        public void SaveSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\WMHelp Software\\QueryMachine", true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\WMHelp Software\\QueryMachine");
            key.SetValue("SearchPath", SearchPath);
            key.SetValue("QueryPath", MyQueriesPath);
            key.SetValue("StartupPanel", (int)DefaultPanel);
            key.SetValue("EnableServerQuery", Convert.ToInt32(EnableServerQuery));
            key.SetValue("HostADOProviders", Convert.ToInt32(DataProviderHelper.HostADOProviders));
            key.SetValue("LimitQueryResults", Convert.ToInt32(LimitSQLQueryResults));
            key.Close();
        }

        private string GetNewQueryTitle(TabControl queryTabs, IQueryEngineFacade facade)
        {
            int k = 1;
            String name;
            bool found;
            Frame frame;
            QueryPage page;
            do
            {
                found = false;
                name = String.Format("Query{0}{1}", k++, facade.DefaultExt);
                foreach (TabItem item in queryTabs.Items)
                {
                    frame = (Frame)item.Content;
                    page = (QueryPage)frame.Content;
                    if (page.ShortFileName == name)
                    {
                        found = true;
                        break;
                    }
                }
                if (MyQueriesPath != null)
                {
                    if (!found && File.Exists(Path.Combine(MyQueriesPath, name)))
                        found = true;
                }
            } while (found);
            return name;
        }

        private void AddPage(TabControl queryTabs, QueryPage page)
        {
            Binding binding = new Binding();
            binding.Source = page;
            binding.Path = new PropertyPath("Title");
            binding.Mode = BindingMode.OneWay;
            TabItem tab = new CloseableTabItem();
            tab.SetBinding(TabItem.HeaderProperty, binding);
            tab.IsSelected = true;
            tab.AddHandler(CloseableTabItem.CloseTabEvent, new RoutedEventHandler(CloseTab));
            Frame frame = new Frame();
            frame.Content = page;
            tab.Content = frame;
            queryTabs.Items.Add(tab);            
        }

        private void CloseTab(object source, RoutedEventArgs args)
        {
            TabItem tabItem = args.Source as TabItem;
            if (tabItem != null)
            {
                Frame frame = (Frame)tabItem.Content;
                QueryPage page = (QueryPage)frame.Content;
                if (CloseQuery(page))
                {
                    TabControl tabControl = tabItem.Parent as TabControl;
                    if (tabControl != null)
                        tabControl.Items.Remove(tabItem);
                }
            }
        }

        public IQueryEngineFacade CreateQueryFacade(string fileExt)
        {
            if (String.Compare(fileExt, ".xq", true) == 0)
                return new XQueryFacade();
            if (String.Compare(fileExt, ".xsql", true) == 0)
                return new SQLXFacade();
            return null;
        }


        public void NewQuery(TabControl queryTabs, IQueryEngineFacade facade)
        {
            QueryPage page = new QueryPage(facade);
            page.FileName = GetNewQueryTitle(queryTabs, facade);
            AddPage(queryTabs, page);
        }

        public void OpenQuery(TabControl queryTabs, string fileName)
        {
            QueryPage page = null;
            String ext = Path.GetExtension(fileName);
            Frame frame = (Frame)queryTabs.SelectedContent;
            if (frame != null)
            {
                page = (QueryPage)frame.Content;
                if (page.HasContent)
                    page = null;
            }
            if (page == null || 
                String.Compare(page.QueryFacade.DefaultExt,  ext, true) != 0)
            {
                page = new QueryPage(CreateQueryFacade(ext));
                AddPage(queryTabs, page);
            }
            page.Title = Path.GetFileName(fileName);
            page.FileName = fileName;
            page.Load();
        }

        public void CloneQuery(TabControl queryTabs, QueryPage page)
        {
            QueryPage cloned_page = new QueryPage(page.QueryFacade);
            cloned_page.FileName = GetNewQueryTitle(queryTabs, page.QueryFacade);
            cloned_page.textEditor.Text = page.textEditor.Text;
            AddPage(queryTabs, cloned_page);
        }

        public bool SaveQuery(QueryPage page)
        {
            if (page.FilePath != String.Empty)
                page.Save();
            else
            {
                IQueryEngineFacade facade = page.QueryFacade;
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = facade.DefaultExt;
                dlg.Filter = String.Format("{0} files|*{1}|Text documents|*.txt|All files|*.*", 
                    facade.EngineName, facade.DefaultExt);
                dlg.FileName = page.ShortFileName;
                if (dlg.ShowDialog() != true)
                    return false;
                SaveAsQuery(page, dlg.FileName);
            }
            return true;
        }

        public void SaveAsQuery(QueryPage page, string fileName)
        {
            page.FileName = fileName;            
            page.Save();
        }

        public bool CloseQuery(QueryPage page)
        {
            if (page.Modified && ConfirmFileSave)
            {
                switch (MessageBox.Show(String.Format("Save {0}?", page.FileName),
                    GetApplicationTitle(), MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        if (!SaveQuery(page))
                            return false;
                        break;

                    case MessageBoxResult.Cancel:
                        return false;
                }
            }
            page.Close();
            return true;
        }
    }
}
