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
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using Microsoft.Data.ConnectionUI;

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for ConnectionDialog.xaml
    /// </summary>
    public partial class ConnectionDialog : Window, INotifyPropertyChanged 
    {
        private Connection _connection = new Connection();
        private DataConnectionConfiguration _dcs;

        public Connection Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = new Connection(value);
                SetProviderInvariantName(value.InvariantName);
                OnPropertyChanged("Connection");
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

        public ConnectionDialog()
        {
            InitializeComponent();
            _dcs = new DataConnectionConfiguration(Extensions.GetAppDataPath());
            DataContext = DbProviderFactories.GetFactoryClasses();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            nameTextBox.Focus();
            okButton.IsEnabled = _connection.InvariantName != String.Empty;
        }

        private void providersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataTable dt = (DataTable)DataContext;            
            string invariantName = (string)dt.Rows[providersComboBox.SelectedIndex]["InvariantName"];
            if (invariantName != _connection.InvariantName)
            {
                _connection.InvariantName = invariantName;
                _connection.ConnectionString = String.Empty;
                connectionStringTextBox.Focus();
                OnPropertyChanged("Connection");
            }
        }

        private void addConnectionButton_Click(object sender, RoutedEventArgs e)
        {            
            using (DataConnectionDialog dlg = new DataConnectionDialog())
            {
                _dcs.LoadConfiguration(dlg);
                if (DataConnectionDialog.Show(dlg, this.GetIWin32Window()) == 
                    System.Windows.Forms.DialogResult.OK)
                {
                    SetProviderInvariantName(dlg.SelectedDataProvider.Name);
                    _connection.ConnectionString = dlg.ConnectionString;
                    nameTextBox.Focus();
                    nameTextBox.SelectAll();
                    OnPropertyChanged("Connection");
                    _dcs.SaveConfiguration(dlg);
                }
            }
        }

        private void SetProviderInvariantName(string name)
        {
            DataTable dt = (DataTable)DataContext;
            for (int k = 0; k < dt.Rows.Count; k++)
                if ((string)dt.Rows[k]["InvariantName"] == name)
                {
                    _connection.InvariantName = name;
                    providersComboBox.SelectedIndex = k;
                    break;
                }            
        }

        private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            okButton.IsEnabled = nameTextBox.Text.Length > 0;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
