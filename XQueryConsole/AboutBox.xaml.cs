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
using System.Reflection;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        public static void Run()
        {
            AboutBox dlg = new AboutBox();
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hlink = (Hyperlink)sender;
            Extensions.ShellExecute(hlink.NavigateUri.AbsoluteUri);
        }

        public static string VersionXQuery
        {
            get
            {
                Assembly asm = Assembly.GetAssembly(typeof(DataEngine.XQuery.Translator));
                return asm.GetName().Version.ToString();
            }
        }

        public static string VersionSQLX
        {
            get
            {
                Assembly asm = Assembly.GetAssembly(typeof(DataEngine.Translator));
                return asm.GetName().Version.ToString();
            }
        }

        public static string VersionCoreServices
        {
            get
            {
                Assembly asm = Assembly.GetAssembly(typeof(DataEngine.CoreServices.Executive));
                return asm.GetName().Version.ToString();
            }
        }
    }
}
