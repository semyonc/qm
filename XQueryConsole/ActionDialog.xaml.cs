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
using System.Threading.Tasks;

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for ActionDialog.xaml
    /// </summary>
    public partial class ActionDialog : Window
    {
        public static void Perform(Action action)
        {
            ActionDialog dlg = new ActionDialog(action);
            dlg.Owner = System.Windows.Application.Current.MainWindow;
            dlg.ShowDialog();
        }

        private Action _action;

        public ActionDialog(Action action)
        {
            InitializeComponent();
            _action = action;
            SourceInitialized += (x, y) => Extensions.HideCloseButton(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                _action();
                Dispatcher.BeginInvoke(new Action(() => Close()));
            });
        }
    }
}
