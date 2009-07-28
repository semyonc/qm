using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using DataEngine.XQuery;
using System.Diagnostics;
using System.Xml.XPath;
using Microsoft.Win32;
using System.Net;

namespace SimpleTestConsole
{
    public partial class Form1 : Form
    {
        string baseUri;
        string searchPath;

        public Form1()
        {
            InitializeComponent();
            ActiveControl = textBox2;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\WMHelp Software\\DataEngine", true))
            {
                if (key != null)
                {
                    searchPath = (string)key.GetValue("SearchPath");
                    key.Close();
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XQuery File (*.xq,*.xquery)|*.xq;*.xquery|All files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TextReader textReader = new StreamReader(dialog.FileName);
                textBox2.Text = textReader.ReadToEnd();
                textReader.Close();
                Text = String.Format("XQuery {0}", Path.GetFileName(dialog.FileName));
                tabControl1.SelectedTab = Source;
                baseUri = Path.GetDirectoryName(dialog.FileName);
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PathForm form = new PathForm())
            {
                if (searchPath != null)
                    form.textBox1.Text = searchPath;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\WMHelp Software\\DataEngine", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("SearchPath", form.textBox1.Text);
                            key.Close();
                        }
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly asm = Assembly.GetAssembly(typeof(XQueryCommand));
            string title = "QueryMachine.XQuery";
            MessageBox.Show(
                String.Format("{0} {1}\n", title, asm.GetName().Version) +
                "Copyright © Semyon A. Chertkov 2009\n" +
                "e-mail: semyonc@gmail.com",
                "About " + Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            GC.Collect();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
                return;
            StringWriter sw = new StringWriter();
            bool switchTab = false;
            try
            {                
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (XQueryCommand command = new XQueryCommand())
                {
                    command.OnResolveCollection += new ResolveCollectionEvent(command_OnResolveCollection);
                    command.BaseUri = baseUri;
                    command.SearchPath = searchPath;
                    command.Compile(textBox2.Text);
                    XQueryNodeIterator iter = command.Execute();
                    while (iter.MoveNext())
                    {
                        if (iter.Current.IsNode)
                            sw.WriteLine(((XPathNavigator)iter.Current).OuterXml);
                        else
                            sw.WriteLine(iter.Current.Value);
                    }
                }
                stopwatch.Stop();
                sw.WriteLine("Proceeded: {0} ms", stopwatch.ElapsedMilliseconds);
                switchTab = true;
            }
            catch (XQueryException ex)
            {
                MessageBox.Show(ex.Message, "Execution Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (switchTab)
            {
                textBox1.Text = sw.ToString();
                tabControl1.SelectedTab = tabPage2;
            }
        }
        
        // You can write something like this for process html data from XQuery
        // See http://htmlagilitypack.codeplex.com/ for details
        void command_OnResolveCollection(object sender, ResolveCollectionArgs args)
        {
            //WebClient client = new WebClient();
            //client.Proxy.Credentials = CredentialCache.DefaultCredentials;
            //String s = client.DownloadString(args.CollectionName);
            //StringReader reader = new StringReader(s);
            //HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            //doc.Load(reader);
            //args.Navigator = doc.CreateNavigator();
        }
    }
}
