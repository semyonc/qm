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
using System.IO;

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for SearchPathDialog.xaml
    /// </summary>
    public partial class SearchPathDialog : Window
    {
        public static String Edit(String path)
        {
            SearchPathDialog dlg = new SearchPathDialog();
            dlg.Owner = System.Windows.Application.Current.MainWindow;
            dlg.Path = path;
            if (dlg.ShowDialog() == true)
                return dlg.Path;
            return path;
        }

        public SearchPathDialog()
        {
            InitializeComponent();
            currentPath.Focus();
        }

        public String Path
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int k = 0; k < pathListBox.Items.Count; k++)
                {
                    if (k > 0)
                        sb.Append(';');
                    TextBlock block = (TextBlock)pathListBox.Items[k];
                    Run textRun = (Run)block.Inlines.FirstInline;
                    sb.Append(textRun.Text);
                }
                return sb.ToString();
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    string[] s = value.Split(new char[] { ';' });
                    foreach (string item in s)
                    {
                        TextBlock block = new TextBlock(new Run(item));
                        if (!Directory.Exists(item))
                            block.Opacity = 0.3;
                        pathListBox.Items.Add(block);
                    }
                }
            }
        }

        private void pathListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pathListBox.SelectedIndex == -1)
            {
                currentPath.Clear();
                btnReplace.IsEnabled =
                    btnDelete.IsEnabled = false;
            }
            else
            {
                TextBlock block = (TextBlock)pathListBox.Items[pathListBox.SelectedIndex];
                Run textRun = (Run)block.Inlines.FirstInline;
                currentPath.Text = textRun.Text;
                btnReplace.IsEnabled =
                    btnDelete.IsEnabled = true;
            }
        }

        private void currentPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (currentPath.Text.Length > 0)
            {
                btnAdd.IsEnabled = true;
                btnReplace.IsEnabled = true;
            }
            else
            {
                btnAdd.IsEnabled = false;
                btnReplace.IsEnabled = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pathListBox_SelectionChanged(null, null);
            currentPath_TextChanged(null, null);
        }

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            if (pathListBox.SelectedIndex != -1 && currentPath.Text != "")
            {
                TextBlock block = (TextBlock)pathListBox.Items[pathListBox.SelectedIndex];
                if (Directory.Exists(currentPath.Text))
                    block.Opacity = 1.0;
                else
                    block.Opacity = 0.3;
                Run textRun = (Run)block.Inlines.FirstInline;
                textRun.Text = currentPath.Text;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (currentPath.Text != null)
            {
                TextBlock block = new TextBlock(new Run(currentPath.Text));
                if (!Directory.Exists(currentPath.Text))
                    block.Opacity = 0.3;
                pathListBox.Items.Add(block);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (pathListBox.SelectedIndex != -1)
                pathListBox.Items.RemoveAt(pathListBox.SelectedIndex);
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            for (int k = pathListBox.Items.Count - 1; k >= 0; k--)
            {
                TextBlock block = (TextBlock)pathListBox.Items[k];
                Run textRun = (Run)block.Inlines.FirstInline;
                DirectoryInfo di = new DirectoryInfo(textRun.Text);
                if (!di.Exists)
                    pathListBox.Items.RemoveAt(k);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (pathListBox.SelectedIndex > 0)
            {
                int selectedIndex = pathListBox.SelectedIndex;
                TextBlock block1 = (TextBlock)pathListBox.Items[selectedIndex - 1];
                TextBlock block2 = (TextBlock)pathListBox.Items[selectedIndex];
                Run textRun1 = (Run)block1.Inlines.FirstInline;
                Run textRun2 = (Run)block2.Inlines.FirstInline;
                using (pathListBox.Items.DeferRefresh())
                {
                    String text = textRun1.Text;
                    textRun1.Text = textRun2.Text;
                    textRun2.Text = text;
                    double opacity = block1.Opacity;
                    block1.Opacity = block2.Opacity;
                    block2.Opacity = opacity;
                }
                pathListBox.SelectedIndex = selectedIndex - 1;
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (pathListBox.SelectedIndex < pathListBox.Items.Count - 1)
            {
                int selectedIndex = pathListBox.SelectedIndex;
                TextBlock block1 = (TextBlock)pathListBox.Items[selectedIndex];
                TextBlock block2 = (TextBlock)pathListBox.Items[selectedIndex + 1];
                Run textRun1 = (Run)block1.Inlines.FirstInline;
                Run textRun2 = (Run)block2.Inlines.FirstInline;
                using (pathListBox.Items.DeferRefresh())
                {
                    String text = textRun1.Text;
                    textRun1.Text = textRun2.Text;
                    textRun2.Text = text;
                    double opacity = block1.Opacity;
                    block1.Opacity = block2.Opacity;
                    block2.Opacity = opacity;
                }
                pathListBox.SelectedIndex = selectedIndex + 1;
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                dlg.SelectedPath = currentPath.Text;
                dlg.Description = Title;
                if (dlg.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK)
                    currentPath.Text = dlg.SelectedPath;
            }
        }    
    }
}
