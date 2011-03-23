//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Win32;

using DataEngine.CoreServices;
using DataEngine.XQuery;
using System.Reflection;



namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private CompositionContainer _compositionContainer = null;

        public App()
        {
            HighlightingManager.Instance.RegisterHighlighting("XQuery", new string[] { ".xq" }, 
                GetHighlightingDefinition("XQueryConsole.XQuery.xshd"));
            HighlightingManager.Instance.RegisterHighlighting("SQLX", new string[] { ".xsql" }, 
                GetHighlightingDefinition("XQueryConsole.SQLX.xshd"));

            // set keyboard shortcuts
            KeyGesture ExecuteCmdKeyGesture = new KeyGesture(Key.F5, ModifierKeys.None);
            KeyGesture CancelExecuteCmdKeyGesture = new KeyGesture(Key.Escape, ModifierKeys.None);
            KeyGesture ShowResultsCmdKeyGesture = new KeyGesture(Key.R, ModifierKeys.Control);
            ((RoutedUICommand)QueryPage.ExecuteCommand).InputGestures.Add(ExecuteCmdKeyGesture);
            ApplicationCommands.Stop.InputGestures.Add(CancelExecuteCmdKeyGesture);
            ((RoutedUICommand)XQueryConsole.MainWindow.ShowResultsCommand).InputGestures.Add(ShowResultsCmdKeyGesture);

            KeyGesture NewSQLXCmdKeyGesture = new KeyGesture(Key.D1, ModifierKeys.Control);
            KeyGesture NewXQueryCmdKeyGesture = new KeyGesture(Key.D2, ModifierKeys.Control);
            ((RoutedUICommand)XQueryConsole.MainWindow.NewSQLXCommand).InputGestures.Add(NewSQLXCmdKeyGesture);
            ((RoutedUICommand)XQueryConsole.MainWindow.NewXQueryCommand).InputGestures.Add(NewXQueryCmdKeyGesture);

            // register extension function in wmh namespace
            XQueryFunctionTable.Register(typeof(WmhFuncs));
            XQueryAdapterImpl.Init();

            // register MEF extensions
            string probePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(App)).Location);
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(probePath));
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\WMHelp Software\\QueryMachine"))
            {
                if (key != null)
                {
                    String addinCatalog = (String)key.GetValue("AddinCatalog");
                    if (addinCatalog != null)
                    {
                        string[] paths = addinCatalog.Split(';');
                        foreach (String p in paths)
                            catalog.Catalogs.Add(new DirectoryCatalog(p));
                    }
                    key.Close();
                }
            }
            _compositionContainer = new CompositionContainer(catalog);
            _compositionContainer.ComposeParts(this);
        }

        private IHighlightingDefinition GetHighlightingDefinition(string resourceName)
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = GetType().Assembly.GetManifestResourceStream(resourceName))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource " + resourceName);
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            return customHighlighting;
        }

        [ImportMany]
        public IEnumerable<IServiceExtension> Addins { get; set; }
    }
}
