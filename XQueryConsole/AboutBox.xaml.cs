using System;
using System.Collections.Generic;
using System.Linq;
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

using DataEngine.XQuery;

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

        public static string Version
        {
            get
            {
                Assembly asm = Assembly.GetAssembly(typeof(Translator));
                return asm.GetName().Version.ToString();
            }
        }
    }
}
