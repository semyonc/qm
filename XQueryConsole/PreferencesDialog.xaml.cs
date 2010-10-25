using System;
using System.Collections.Generic;
using System.Linq;
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

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for PreferencesDialog.xaml
    /// </summary>
    public partial class PreferencesDialog : Window, INotifyPropertyChanged
    {
        private DocumentController controller;

        public static void ShowDialog(DocumentController controller, bool focusQuerisFolder)
        {
            PreferencesDialog dlg = new PreferencesDialog(controller);
            dlg.Owner = System.Windows.Application.Current.MainWindow;
            if (focusQuerisFolder)
                dlg.FocusAndSelect();
            dlg.ShowDialog();
        }

        public PreferencesDialog(DocumentController controller)
        {
            InitializeComponent();
            
            this.controller = controller;
            MyQueriesPath = controller.MyQueriesPath;
            SearchPath = controller.SearchPath;
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

        public void FocusAndSelect()
        {
            Dispatcher.BeginInvoke(new Action(() => 
            {
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
