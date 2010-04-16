using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Reflection;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

using DataEngine.XQuery;
using System.Threading;

namespace XQTSRun
{
    public partial class Form1 : Form
    {
        public const String XQTSNamespace = "http://www.w3.org/2005/02/query-test-XQTSCatalog";
        
        public enum TestResult
        {
            Pass,
            Fail,
            Exception
        }


        internal string _basePath;
        internal string _queryOffsetPath;
        internal string _sourceOffsetPath;
        internal string _resultOffsetPath;
        internal string _queryFileExtension;

        internal NameTable _nameTable;
        internal XmlNamespaceManager _nsmgr;
        internal XmlDocument _catalog;
        internal DataTable _testTab;
        internal Dictionary<string, string> _sources;
        internal Dictionary<string, string> _module;
        internal Dictionary<string, string[]> _collection;
        internal Dictionary<string, string[]> _schema;
        internal OutputWriter _out;
        internal string _lastFindString = "";
        
        internal ToolStripProgressBar _progressBar;
        internal ToolStripStatusLabel _statusLabel;
        internal int _total;
        internal int _passed;


        public Form1()
        {
            InitializeComponent();
            _out = new OutputWriter(richTextBox1);
            testToolStripMenuItem.Visible = false;
            _nameTable = new NameTable();
            _nsmgr = new XmlNamespaceManager(_nameTable);
            _nsmgr.AddNamespace("ts", XQTSNamespace);
            _testTab = new DataTable();
            _testTab.Columns.Add("Select", typeof(Boolean));
            _testTab.Columns.Add("Name", typeof(String));
            _testTab.Columns.Add("FilePath", typeof(String));
            _testTab.Columns.Add("scenario", typeof(String));
            _testTab.Columns.Add("Creator", typeof(String));
            _testTab.Columns.Add("Node", typeof(System.Object));
            _testTab.Columns.Add("Description", typeof(String));
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "XQTSCatalog file (*.xml)|*.xml|All files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        OpenCatalog(dialog.FileName);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_out != null)
                _out.Flush();
            if (_progressBar != null)
            {
                _progressBar.Value = _total;
                _statusLabel.Text = String.Format("{0}/{1}({2} Failed)", _total, 
                    _progressBar.Maximum, _total - _passed);
            }
        }

        private void OpenCatalog(string fileName)
        {
            _catalog = new XmlDocument(_nameTable);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas = schemaSet;
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = CredentialCache.DefaultCredentials;
            settings.XmlResolver = resolver;
            settings.NameTable = _nameTable;
            settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation |
                 XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationType = ValidationType.Schema;
            using (XmlReader reader = XmlReader.Create(fileName, settings))
            {
                _catalog.Load(reader);
                reader.Close();
            }
            if (!(_catalog.DocumentElement.NamespaceURI == XQTSNamespace &&
                  (_catalog.DocumentElement.LocalName == "test-suite")))
                throw new ArgumentException("Input file is not XQTS catalog.");
            if (_catalog.DocumentElement.GetAttribute("version") != "1.0.2")
                throw new NotSupportedException("Only version 1.0.2 is XQTS supported.");
            _basePath = Path.GetDirectoryName(fileName);
            _sourceOffsetPath = _catalog.DocumentElement.GetAttribute("SourceOffsetPath");
            _queryOffsetPath = _catalog.DocumentElement.GetAttribute("XQueryQueryOffsetPath");
            _resultOffsetPath = _catalog.DocumentElement.GetAttribute("ResultOffsetPath");
            _queryFileExtension = _catalog.DocumentElement.GetAttribute("XQueryFileExtension");

            _sources = new Dictionary<string, string>();
            _module = new Dictionary<string, string>();
            _collection = new Dictionary<string, string[]>();
            _schema = new Dictionary<string, string[]>();

            foreach (XmlElement node in _catalog.SelectNodes("/ts:test-suite/ts:sources/ts:schema", _nsmgr))
            {
                string id = node.GetAttribute("ID");
                string targetNs = node.GetAttribute("uri");
                string schemaFileName = Path.Combine(_basePath, node.GetAttribute("FileName").Replace('/', '\\'));
                if (!File.Exists(schemaFileName))
                    _out.WriteLine("Schema file {0} is not exists", schemaFileName);
                _schema.Add(id, new string[] { targetNs, schemaFileName });
            }
            foreach (XmlElement node in _catalog.SelectNodes("/ts:test-suite/ts:sources/ts:source", _nsmgr))
            {
                string id = node.GetAttribute("ID");
                string sourceFileName = Path.Combine(_basePath, node.GetAttribute("FileName").Replace('/', '\\'));
                if (!File.Exists(sourceFileName))
                    _out.WriteLine("Source file {0} is not exists", sourceFileName);
                _sources.Add(id, sourceFileName);
            }
            foreach (XmlElement node in _catalog.SelectNodes("/ts:test-suite/ts:sources/ts:collection", _nsmgr))
            {
                string id = node.GetAttribute("ID");
                XmlNodeList nodes = node.SelectNodes("ts:input-document", _nsmgr);
                String[] items = new String[nodes.Count];
                int k = 0;
                foreach (XmlElement curr in nodes)
                {
                    if (!_sources.ContainsKey(curr.InnerText))
                        _out.WriteLine("Referenced source ID {0} in collection {1} not exists", curr.InnerText, id);
                    items[k++] = curr.InnerText;
                }
                _collection.Add(id, items);
            }
            foreach (XmlElement node in _catalog.SelectNodes("/ts:test-suite/ts:sources/ts:module", _nsmgr))
            {
                string id = node.GetAttribute("ID");
                string moduleFileName = Path.Combine(_basePath, node.GetAttribute("FileName").Replace('/', '\\') + _queryFileExtension);
                if (!File.Exists(moduleFileName))
                    _out.WriteLine("Module file {0} is not exists", moduleFileName);
                _module.Add(id, moduleFileName);
            }
            treeView1.Nodes.Clear();
            treeView1.BeginUpdate();
            TreeNode rootNode = new TreeNode("Test-suite", 0, 0);
            treeView1.Nodes.Add(rootNode);
            ReadTestTree(_catalog.DocumentElement, rootNode);
            treeView1.EndUpdate();
            rootNode.Expand();
            toolStripMenuItem6.Visible = true;            
        }

        private void ReadTestTree(XmlNode node, TreeNode parentNode)
        {
            foreach (XmlNode child in node.ChildNodes)
                if (child.LocalName == "test-group" && child.NamespaceURI == XQTSNamespace)
                {
                    XmlElement elem = (XmlElement)child;
                    TreeNode childNode = new TreeNode(elem.GetAttribute("name"));
                    childNode.Tag = child;
                    ReadTestTree(child, childNode);
                    parentNode.Nodes.Add(childNode);
                }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            dataGridView1.DataSource = null;
            _testTab.Clear();
            testToolStripMenuItem.Visible = false;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                XmlNode node = e.Node.Tag as XmlNode;
                XmlNodeList nodes;
                if (node != null)
                {
                    XmlElement elem = node.SelectSingleNode("ts:GroupInfo/ts:title", _nsmgr) as XmlElement;
                    if (elem != null)
                        label3.Text = elem.InnerText;
                    else
                        label3.Text = "";
                    nodes = node.SelectNodes(".//ts:test-case", _nsmgr);
                }
                else
                    nodes = _catalog.SelectNodes(".//ts:test-case", _nsmgr);

                foreach (XmlElement child in nodes)
                {
                    DataRow row = _testTab.NewRow();
                    row[0] = false;
                    row[1] = child.GetAttribute("name");
                    row[2] = child.GetAttribute("FilePath");
                    row[3] = child.GetAttribute("scenario");
                    row[4] = child.GetAttribute("Creator");
                    row[5] = child;
                    XmlElement desc = (XmlElement)child.SelectSingleNode("ts:description", _nsmgr);
                    if (desc != null)
                        row[6] = desc.InnerText;
                    _testTab.Rows.Add(row);
                }
                dataGridView1.DataSource = _testTab;
                dataGridView1.Columns[0].Width = 50;
                dataGridView1.Columns[1].ReadOnly = true;
                dataGridView1.Columns[1].Width = 100;
                dataGridView1.Columns[2].ReadOnly = true;
                dataGridView1.Columns[2].Width = 190;
                dataGridView1.Columns[3].ReadOnly = true;
                dataGridView1.Columns[3].Width = 80;
                dataGridView1.Columns[4].ReadOnly = true;
                dataGridView1.Columns[4].Width = 150;
                dataGridView1.Columns[5].Visible = false;
                dataGridView1.Columns[6].Width = 300;
                dataGridView1.Columns[6].ReadOnly = true;
                testToolStripMenuItem.Visible = 
                    _testTab.Rows.Count > 0;
                if (node == null)
                {
                    int sel = 0;
                    HashSet<XmlNode> hs = new HashSet<XmlNode>();
                    foreach (XmlNode child in
                        _catalog.SelectNodes(".//ts:test-group[@name='MinimalConformance']//ts:test-case", _nsmgr))
                        hs.Add(child);
                    foreach (XmlNode child in
                        _catalog.SelectNodes(".//ts:test-group[@name='FullAxis']//ts:test-case", _nsmgr))
                        hs.Add(child);
                    foreach (XmlNode child in
                        _catalog.SelectNodes(".//ts:test-group[@name='Appendices']//ts:test-case", _nsmgr))
                        hs.Add(child);
                    foreach (XmlNode child in
                        _catalog.SelectNodes(".//ts:test-group[@name='UseCase']//ts:test-case", _nsmgr))
                        hs.Add(child);
                    foreach (XmlNode child in
                        _catalog.SelectNodes(".//ts:test-group[@name='Catalog']//ts:test-case", _nsmgr))
                        hs.Add(child);
                    foreach (XmlNode child in
                        _catalog.SelectNodes(".//ts:test-group[@name='SchemaImport']//ts:test-case", _nsmgr))
                        hs.Add(child);
                    sel = 0;
                    foreach (DataRow row in _testTab.Rows)
                    {
                        if (hs.Contains((XmlNode)row[5]))
                        {
                            row[0] = true;
                            sel++;
                        }
                    }
                    toolStripStatusLabel1.Text =
                        String.Format("{0} test case(s) loaded, {1} supported selected.", _testTab.Rows.Count, sel);
                }
                else
                    toolStripStatusLabel1.Text =
                        String.Format("{0} test case(s) loaded.", _testTab.Rows.Count);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    foreach (DataRow row in _testTab.Rows)
                        row[0] = true;
                    toolStripStatusLabel1.Text = String.Format("{0} tests selected.", _testTab.Rows.Count);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void viewCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringWriter tw = new StringWriter();
            if (dataGridView1.CurrentRow != null)
            {
                string fileName = GetFilePath((XmlElement)dataGridView1.CurrentRow.Cells[5].Value);
                tw.WriteLine(fileName);
                TextReader textReader = new StreamReader(fileName, true);
                String line;
                while ((line = textReader.ReadLine()) != null)
                    tw.WriteLine(line);
                textReader.Close();
            }
            richTextBox1.Text = tw.ToString();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            StringWriter tw = new StringWriter();
            if (dataGridView1.CurrentRow != null)
            {
                XmlElement testCase = (XmlElement)dataGridView1.CurrentRow.Cells[5].Value;
                foreach (XmlElement outputFile in testCase.SelectNodes("ts:output-file", _nsmgr))
                {
                    string fileName = GetResultPath(testCase, outputFile.InnerText);
                    tw.WriteLine("{0}:", fileName);
                    TextReader textReader = new StreamReader(fileName, true);
                    String line;
                    while ((line = textReader.ReadLine()) != null)
                        tw.WriteLine(line);
                    textReader.Close();
                }
            }
            richTextBox1.Text = tw.ToString();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            StringWriter tw = new StringWriter();
            if (dataGridView1.CurrentRow != null)
            {
                XmlElement testCase = (XmlElement)dataGridView1.CurrentRow.Cells[5].Value;
                foreach (XmlElement inputFile in testCase.SelectNodes("ts:input-file", _nsmgr))
                {
                    string fileName = _sources[inputFile.InnerText];
                    tw.WriteLine("{0}:", fileName);
                    TextReader textReader = new StreamReader(fileName, true);
                    String line;
                    while ((line = textReader.ReadLine()) != null)
                        tw.WriteLine(line);
                    textReader.Close();
                }
            }
            richTextBox1.Text = tw.ToString();
        }

        private void runCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            richTextBox1.Clear();
            XmlElement testCase = (XmlElement)dataGridView1.CurrentRow.Cells[5].Value;
            try
            {
                using (XQueryCommand command = CreateCommand(_out, testCase))
                {
                    XQueryNodeIterator iter = command.Execute();
                    while (iter.MoveNext())
                    {
                        _out.WriteLine();
                        if (iter.Current.IsNode)
                            _out.Write(((XPathNavigator)iter.Current).OuterXml);
                        else
                            _out.Write(iter.Current.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                _out.WriteLine();
                _out.WriteLine(ex);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            richTextBox1.Clear();
            XmlElement testCase = (XmlElement)dataGridView1.CurrentRow.Cells[5].Value;
            if (PerformTest(_out, testCase))
                _out.WriteLine("Passed.");
            else
                _out.WriteLine("Failed.");
        }

        private void batchRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            dataGridView1.EndEdit();
            int count = 0;
            foreach (DataRow r in _testTab.Select(""))
                if ((bool)r[0])
                    count++;
            if (count == 0)
            {
                MessageBox.Show("No test case selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (count > 100)
            {
                if (MessageBox.Show(String.Format("{0} test case(s) selected. Continue ?", count),
                    "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    return;
            }
            _total = 0;
            _passed = 0;
            _progressBar = new ToolStripProgressBar();
            _progressBar.Minimum = 0;
            _progressBar.Value = 0;
            _progressBar.Maximum = count;
            _statusLabel = new ToolStripStatusLabel();
            toolStripStatusLabel1.Text = "Running";
            statusStrip1.Items.Add(_progressBar);
            statusStrip1.Items.Add(_statusLabel);
            Thread worker = new Thread(new ThreadStart(BatchTestThread));
            worker.Start();
        }

        delegate void CompleteDelegate(ResultFile resFile);

        private void CompleteBatchTest(ResultFile resFile)
        {
            foreach (DataRow row in _testTab.Rows)
                row[0] = false;
            string fileName = _basePath + "\\ReportingResults\\XQTS_QM_Result.xml";
            resFile.Create(fileName);
            statusStrip1.Items.Remove(_statusLabel);
            statusStrip1.Items.Remove(_progressBar);
            toolStripStatusLabel1.Text = "Done";
            _progressBar = null;
            _statusLabel = null;
        }

        private void CreateHtmlReport(bool detail)
        {
            if (!File.Exists(_basePath + "\\ReportingResults\\QM_Results.xml"))
            {
                MessageBox.Show("No test results found. Run batch test first", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                XsltArgumentList xslArg = new XsltArgumentList();
                xslArg.AddParam("details", "", detail);
                xslArg.AddParam("resultFiles", "", "QM_Results.xml");
                xslArg.AddParam("impdef", "", detail);
                xslArg.AddParam("test-run-details", "", detail);                
                string stylesheetName = _basePath + "\\ReportingResults\\XQTSResults.xsl";
                string reportName = _basePath + "\\ReportingResults\\XQTS_QM_Result.html";
                XsltSettings settings = new XsltSettings();
                settings.EnableDocumentFunction = true;
                settings.EnableScript = false;
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(stylesheetName, settings, new XmlUrlResolver());
                using (TextWriter textWriter = new StreamWriter(reportName, false, Encoding.UTF8))
                {
                    transform.Transform(_catalog.BaseURI, xslArg, textWriter);
                    textWriter.Close();
                }
                Process.Start(reportName);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }            
        }

        private void BatchTestThread()
        {
            ResultFile resFile = new ResultFile();
            resFile.AddFeature("Minimal Conformance", true);
            resFile.AddFeature("Schema Import", true);
            resFile.AddFeature("Schema Validation", false);
            resFile.AddFeature("Full Axis", true);
            resFile.AddFeature("Serialization", false);
            resFile.AddFeature("Trivial XML Embedding", false);
            resFile.Worktime = new Stopwatch();
            resFile.Worktime.Start();
            foreach (DataRow dr in _testTab.Select(""))
            {
                if ((bool)dr[0])
                {
                    XmlElement curr = (XmlElement)dr[5];
                    string id = curr.GetAttribute("name");
                    TextWriter tw = new StringWriter();
                    if (PerformTest(tw, curr))
                    {
                        tw.WriteLine("Passed.");
                        //Console.Write(tw.ToString());
                        resFile.AddResult(id, "pass");
                        Interlocked.Increment(ref _passed);
                    }
                    else
                    {
                        tw.WriteLine("Failed.");
                        resFile.AddResult(id, "fail");
                        _out.Write(tw.ToString());
                    }
                    Interlocked.Increment(ref _total);
                }
            }
            resFile.Worktime.Stop();
            if (_total > 0)
            {
                decimal total = _total;
                decimal passed = _passed;
                _out.WriteLine("{0} executed, {1} ({2}%) successed.", total, passed,
                    Math.Round(passed / total * 100, 2));
            }
            Invoke(new CompleteDelegate(CompleteBatchTest), resFile);
        }

        private string GetFilePath(XmlElement node)
        {
            XmlNode queryName = node.SelectSingleNode("ts:query/@name", _nsmgr);
            return _basePath + "\\" + (_queryOffsetPath + node.GetAttribute("FilePath") +
                queryName.Value + _queryFileExtension).Replace('/', '\\');
        }

        private string GetResultPath(XmlElement node, string fileName)
        {
            return _basePath + "\\" + (_resultOffsetPath + node.GetAttribute("FilePath") + fileName).Replace('/', '\\');
        }

        private XQueryCommand CreateCommand(TextWriter tw, XmlElement node)
        {
            string fileName = GetFilePath(node);
            tw.Write("{0}: ", node.GetAttribute("name"));
            if (!File.Exists(fileName))
            {
                _out.WriteLine("File {0} not exists.", fileName);
                throw new ArgumentException();
            }
            XQueryCommand command = new XQueryCommand();
            try
            {
                TextReader textReader = new StreamReader(fileName, true);
                command.CommandText = textReader.ReadToEnd();
                textReader.Close();
                command.OnResolveCollection += new ResolveCollectionEvent(command_OnResolveCollection);
                foreach (XmlNode child in node.ChildNodes)
                {
                    XmlElement curr = child as XmlElement;
                    if (curr == null || curr.NamespaceURI != XQTSNamespace)
                        continue;
                    if (curr.LocalName == "module")
                    {
                        string ns = curr.GetAttribute("namespace");
                        string id = curr.InnerText;
                        command.DefineModuleNamespace(ns, _module[id]);
                    }
                }
                foreach (string[] schema in _schema.Values)
                    command.DefineSchemaNamespace(schema[0], schema[1]);
                command.Compile();
                foreach (XmlNode child in node.ChildNodes)
                {
                    XmlElement curr = child as XmlElement;
                    if (curr == null || curr.NamespaceURI != XQTSNamespace)
                        continue;
                    if (curr.LocalName == "input-file")
                    {
                        string var = curr.GetAttribute("variable");
                        string id = curr.InnerText;
                        IXPathNavigable doc = command.Context.OpenDocument(_sources[id]);
                        command.Parameters.Add(new XQueryParameter(var, doc.CreateNavigator()));
                    }
                    else if (curr.LocalName == "contextItem")
                    {
                        string id = curr.InnerText;
                        XmlReader reader = XmlReader.Create(_sources[id], command.Context.GetSettings());
                        XQueryDocument doc = new XQueryDocument(reader);
                        command.ContextItem = doc.CreateNavigator();
                    }
                    else if (curr.LocalName == "defaultCollection")
                    {
                        string id = curr.InnerText;
                        command.OnResolveCollection += new ResolveCollectionEvent(delegate(object sender, ResolveCollectionArgs args)
                            {
                                if (args.CollectionName == "")
                                    args.Collection = CreateCollection(command, id);
                            });
                    }
                    else if (curr.LocalName == "input-URI")
                    {
                        string var = curr.GetAttribute("variable");
                        string value = curr.InnerText;
                        string expandedUri;
                        if (!_sources.TryGetValue(value, out expandedUri))
                            expandedUri = value;
                        command.Parameters.Add(new XQueryParameter(var, expandedUri));
                    }
                    else if (curr.LocalName == "input-query")
                    {
                        string var = curr.GetAttribute("variable");
                        using (XQueryCommand command2 = new XQueryCommand())
                        {
                            TextReader textReader2 = new StreamReader(Path.Combine(
                                Path.GetDirectoryName(fileName), curr.GetAttribute("name") + ".xq"), true);
                            command2.CommandText = textReader2.ReadToEnd();
                            textReader2.Close();
                            command.Parameters.Add(new XQueryParameter(var, command2.Execute()));
                        }
                    }
                }
                return command;
            }
            catch (Exception)
            {
                command.Dispose();
                throw;
            }
        }

        private bool PerformTest(TextWriter tw, XmlElement testCase)
        {
            try
            {
                XQueryCommand command;
                try
                {
                    command = CreateCommand(tw, testCase);
                }
                catch (XQueryException)
                {
                    if (testCase.GetAttribute("scenario") == "parse-error" ||
                        testCase.GetAttribute("scenario") == "runtime-error" ||
                        testCase.SelectSingleNode("ts:expected-error", _nsmgr) != null)
                        return true;
                    throw;
                }
                using (command)
                {
                    XQueryNodeIterator iter;
                    try
                    {
                        iter = command.Execute();
                    }
                    catch (XQueryException)
                    {
                        if (testCase.GetAttribute("scenario") == "parse-error" ||
                            testCase.GetAttribute("scenario") == "runtime-error" ||
                            testCase.SelectSingleNode("ts:expected-error", _nsmgr) != null)
                            return true;
                        throw;
                    }
                    try
                    {
                        if (testCase.GetAttribute("scenario") == "standard")
                        {
                            foreach (XmlElement outputFile in testCase.SelectNodes("ts:output-file", _nsmgr))
                            {
                                string compare = outputFile.GetAttribute("compare");
                                if (compare == "Text" || compare == "Fragment")
                                {
                                    if (CompareFragment(command.Context, GetResultPath(testCase, outputFile.InnerText), iter, XmlSpace.Default))
                                        return true;
                                }
                                else if (compare == "XML")
                                {
                                    if (CompareFragment(command.Context, GetResultPath(testCase, outputFile.InnerText), iter, XmlSpace.Preserve))
                                        return true;
                                }
                                else if (compare == "Inspect")
                                {
                                    _out.WriteLine("{0}: Inspection needed.", testCase.GetAttribute("name"));
                                    return true;
                                }
                                else if (compare == "Ignore")
                                    continue;
                                else
                                    throw new InvalidOperationException();
                            }
                            return false;
                        }
                        else if (testCase.GetAttribute("scenario") == "runtime-error")
                        {
                            while (iter.MoveNext())
                                ;
                            return false;
                        }
                        return true;
                    }
                    catch (XQueryException)
                    {
                        if (testCase.GetAttribute("scenario") == "runtime-error" ||
                            testCase.SelectSingleNode("ts:expected-error", _nsmgr) != null)
                            return true;
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _out.WriteLine();
                _out.WriteLine(ex);
                return false;
            }            
        }

        private IEnumerable<XPathItem> DocumentIterator(XQueryContext context, XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
            {
                XPathNavigator nav = item as XPathNavigator;
                if (nav != null && nav.NodeType != XPathNodeType.Root)
                {
                    XQueryDocument doc = context.CreateDocument();
                    XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
                    builder.WriteStartDocument();
                    Core.WriteNode(context.Engine, builder, nav);
                    yield return doc.CreateNavigator();
                }
                else
                    yield return item;
            }
        }

        private bool CompareXML(XQueryContext context, string sourceFile, XQueryNodeIterator iter)
        {
            IXPathNavigable doc = context.OpenDocument(sourceFile);
            XQueryNodeIterator src = new NodeIterator(new XPathItem[] { doc.CreateNavigator() });
            TreeComparer comparer = new TreeComparer(context.Engine);
            comparer.IgnoreWhitespace = true;
            return comparer.DeepEqual(src, new NodeIterator(DocumentIterator(context, iter)));
        }

        private bool CompareFragment(XQueryContext context, string sourceFile, XQueryNodeIterator iter, XmlSpace space)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version='1.0'?>");
            sb.Append("<root>");
            TextReader textReader = new StreamReader(sourceFile, true);
            sb.Append(textReader.ReadToEnd());
            textReader.Close();
            sb.Append("</root>");
            XmlReaderSettings settings = context.GetSettings();
            XmlReader reader = XmlReader.Create(new StringReader(sb.ToString()), settings);
            XQueryDocument doc1 = new XQueryDocument(reader, space);
            doc1.Fill();
            context.AddDocument(doc1);
            XQueryDocument doc2 = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc2);
            builder.WriteStartDocument();
            builder.WriteStartElement("root", "");
            Core.WriteNode(context.Engine, builder, iter.Clone());
            builder.WriteEndElement();
            XQueryNodeIterator iter1 = new NodeIterator(new XPathItem[] { doc1.CreateNavigator() });
            XQueryNodeIterator iter2 = new NodeIterator(new XPathItem[] { doc2.CreateNavigator() });
            TreeComparer comparer = new TreeComparer(context.Engine);
            comparer.IgnoreWhitespace = true;
            bool res = comparer.DeepEqual(iter1, iter2);
            return res;
        }

        void command_OnResolveCollection(object sender, ResolveCollectionArgs args)
        {
            if (args.CollectionName != String.Empty)
                args.Collection = CreateCollection((XQueryCommand)sender, args.CollectionName);
        }

        private IEnumerable<XPathItem> EnumerateItems(XQueryContext context, string[] items)
        {
            foreach (String id in items)
            {
                IXPathNavigable doc = context.OpenDocument(_sources[id]);
                yield return doc.CreateNavigator();
            }
        }

        private XQueryNodeIterator CreateCollection(XQueryCommand command, string id)
        {
            string[] items;
            if (_collection.TryGetValue(id, out items))
                return new NodeIterator(EnumerateItems(command.Context, items));
            return null;
        }

        private void xMLQueryTestSuiteHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.w3.org/XML/Query/test-suite/");
        }

        private void standaloneXQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://qm.codeplex.com");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly asm = Assembly.GetAssembly(typeof(Translator));
            MessageBox.Show(
                String.Format("{0} {1}\n", Text, asm.GetName().Version) +
                "Copyright © Semyon A. Chertkov, 2009\n" +
                "http://www.wmhelp.com\n" +
                "e-mail: semyonc@gmail.com",
                "About " + Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.wmhelp.com");
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            CreateHtmlReport(false);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            CreateHtmlReport(true);
        }

        private void toolStripFind_Click(object sender, EventArgs e)
        {
            using (FindForm form = new FindForm())
            {
                form.RowIndex = 0;
                form.Value = _lastFindString;
                form.FindNext += new EventHandler(form_FindNext);
                form.ShowDialog();
            }
        }

        void form_FindNext(object sender, EventArgs e)
        {
            FindForm form = (FindForm)((Button)sender).Parent;
            for (int index = form.RowIndex; index < dataGridView1.Rows.Count; index++)
            {
                string text = dataGridView1.Rows[index].Cells[1].Value.ToString();
                if (text.StartsWith(form.Value))
                {
                    _lastFindString = form.Value;
                    form.RowIndex = index + 1;
                    dataGridView1.CurrentCell = dataGridView1.Rows[index].Cells[1];
                    return;
                }
            }
            form.RowIndex = 0;
            MessageBox.Show("Passed the end of the table.", "Information", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
