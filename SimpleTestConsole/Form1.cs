using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Net;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;
using DataEngine.XQuery;
using DataEngine.XQuery.OpenXML;
using WmHelp.XmlGrid;

namespace SimpleTestConsole
{
    public partial class Form1 : Form
    {
        string baseUri;
        string searchPath;
        
        XmlGridView xmlGrid;
        bool modified1;
        bool modified2;

        public Form1()
        {
            InitializeComponent();
            ActiveControl = textBox2;

            xmlGrid = new XmlGridView();
            xmlGrid.Dock = DockStyle.Fill;
            xmlGrid.Location = new Point(0, 100);
            xmlGrid.Name = "xmlGridView1";
            xmlGrid.Size = new Size(100, 100);
            xmlGrid.TabIndex = 0;
            xmlGrid.AutoHeightCells = true;
            tabPage1.Controls.Add(xmlGrid);

            XQueryFunctionTable.Register(typeof(PerfTest));

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
                TextReader textReader = new StreamReader(dialog.FileName, true);
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
                            searchPath = form.textBox1.Text;
                            key.SetValue("SearchPath", searchPath);
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
                "Copyright © Semyon A. Chertkov 2009-2010\n" +
                "e-mail: semyonc@gmail.com",
                "About " + Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            GC.Collect();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != tabPage1)
            {
                if (ExecuteTextQuery())
                    tabControl1.SelectedTab = tabPage2;
            }
            else
                ExecuteGridQuery();
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

        void command_OnInputValidation(object sender, ValidationEventArgs e)
        {
            Console.Out.WriteLine(e.Message);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            modified1 = modified2 = true;
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabPage2 && modified2)
                e.Cancel = !ExecuteTextQuery();
            else if (e.TabPage == tabPage1 && modified1)
                e.Cancel = !ExecuteGridQuery();
        }

        private bool ExecuteTextQuery()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                modified2 = false;
                if (textBox2.Text == "")
                    return true;
                StringWriter sw = new StringWriter();
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    XQueryCommand command;
                    if (textBox2.Text.Contains("oxml:") || textBox2.Text.Contains("http://schemas.openxmlformats.org"))
                        command = new OpenXmlQueryCommand(); // OpenXmlQueryCommand adds support of MS Office OpenXML and depends from it
                    else
                        command = new XQueryCommand(); // XQueryCommand not depends from DocumentFormat.OpenXML assembly                
                    using (command)
                    {
                        command.OnResolveCollection += new ResolveCollectionEvent(command_OnResolveCollection);
                        command.OnInputValidation += new System.Xml.Schema.ValidationEventHandler(command_OnInputValidation);
                        command.BaseUri = baseUri;
                        command.SearchPath = searchPath;
                        command.CommandText = textBox2.Text;
                        XQueryNodeIterator iter = command.Execute();
                        while (iter.MoveNext())
                        {
                            if (iter.Current.IsNode)
                            {
                                XPathNavigator nav = (XPathNavigator)iter.Current;
                                sw.WriteLine(nav.OuterXml);
                            }
                            else
                                sw.WriteLine(iter.Current.Value);
                        }
                    }
                    stopwatch.Stop();
                    sw.WriteLine("Proceeded: {0} ms", stopwatch.ElapsedMilliseconds);
                }
                catch (XQueryException ex)
                {
                    MessageBox.Show(ex.Message, "Execution Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Application Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                textBox1.Text = sw.ToString();
                return true;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private bool ExecuteGridQuery()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                modified1 = false;
                xmlGrid.Cell = null;
                xmlGrid.Footer = null;
                if (textBox2.Text == "")
                    return true;
                List<object> nodes = new List<object>();
                Stopwatch stopwatch = new Stopwatch();
                try
                {
                    stopwatch.Start();
                    XQueryCommand command;
                    if (textBox2.Text.Contains("oxml:") || textBox2.Text.Contains("http://schemas.openxmlformats.org"))
                        command = new OpenXmlQueryCommand(); // OpenXmlQueryCommand adds support of MS Office OpenXML and depends from it
                    else
                        command = new XQueryCommand(); // XQueryCommand not depends from DocumentFormat.OpenXML assembly                
                    DOMConverter converter = new DOMConverter();
                    using (command)
                    {
                        command.OnResolveCollection += new ResolveCollectionEvent(command_OnResolveCollection);
                        command.OnInputValidation += new System.Xml.Schema.ValidationEventHandler(command_OnInputValidation);
                        command.BaseUri = baseUri;
                        command.SearchPath = searchPath;
                        command.CommandText = textBox2.Text;
                        XQueryNodeIterator iter = command.Execute();
                        while (iter.MoveNext())
                        {
                            if (iter.Current.IsNode)
                            {
                                XPathNavigator nav = (XPathNavigator)iter.Current;
                                nodes.Add(converter.ToXmlNode(nav));
                            }
                            else
                                nodes.Add(iter.Current.TypedValue);
                        }
                        stopwatch.Stop();
                    }
                }
                catch (XQueryException ex)
                {
                    MessageBox.Show(ex.Message, "Execution Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Application Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                ResultBuilder builder = new ResultBuilder();
                GridCellGroup rootCell = builder.Parse(nodes);
                xmlGrid.Cell = rootCell;
                xmlGrid.Footer = new string[] { String.Format("Proceeded: {0} ms", stopwatch.ElapsedMilliseconds) };
                return true;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
