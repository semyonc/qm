using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Xml;
using System.Threading;
using System.Diagnostics;

using DataEngine.CoreServices.Data;

namespace DataEngine.Export
{
    public enum ExportTarget
    {
        Xml,
        AdoNet,
        Csv,
        TabDelimited,
        FixedLength,
        Xls,
        AdoProvider
    }

    public partial class ExportDataDialog : Form
    {
        public ExportTarget Target { get; set; }
        public String FileName { get; set; }

        public BatchMove BatchMove { get; set; }
        public Resultset Source { get; set; }
        
        public static String CommaSeparator { get; set; }
        public static bool CreateSchemaIni { get; set; }
        public static bool ExportNlsSettings { get; set; }

        public ExportDataDialog()
        {
            InitializeComponent();
        }

        private long timestamp;
        public Thread workThread;

        private volatile int count;

        public void Run()
        {
            try
            {
                if (Source == null)
                    throw new ArgumentNullException();
                Cursor.Current = Cursors.WaitCursor;
                timestamp = Stopwatch.GetTimestamp();
                UseWaitCursor = true;
                timer1.Enabled = true;
                ShowDialog();
            }
            catch (Exception ex)
            {
                RuntimeException(ex);
            }
            Cursor.Current = Cursors.Default;
        }

        delegate void DialogResultDelegate(DialogResult dialogResult);
        delegate void StatusInfoDelegate(String text);
        delegate void RuntimeExceptionDelegate(Exception ex);

        private void RuntimeException(Exception ex)
        {
            if (!(ex is ThreadAbortException))
                MessageBox.Show(this, ex.Message, "Runtime error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            DialogResult = DialogResult.OK;
        }

        private void SetDialogResult(DialogResult dialogResult)
        {
            DialogResult = dialogResult;
        }

        private void SetStatusInfo(String text)
        {
            label1.Text = text;
        }        

        private void BackgroundRun()
        {
#if !LITE
            try
            {
                count = 0;
                AbstractWriter writer = null;
                switch (Target)
                {
                    case ExportTarget.Xml:
                        writer = new XmlFileWriter(FileName);
                        break;

                    case ExportTarget.Xls:
                        writer = new XlsWriter(FileName);
                        break;

                    case ExportTarget.AdoNet:
                        writer = new AdoNetWriter(FileName);
                        break;

                    case ExportTarget.AdoProvider:
                        writer = new AdoProviderWriter(BatchMove);
                        break;

                    case ExportTarget.TabDelimited:
                        writer = new CsvWriter(FileName, "\t", CreateSchemaIni);
                        break;

                    case ExportTarget.Csv:
                        writer = new CsvWriter(FileName, CommaSeparator, CreateSchemaIni);
                        break;

                    case ExportTarget.FixedLength:
                        writer = new FlvWriter(FileName, CreateSchemaIni);
                        break;

                    default:
                        return;
                }
                writer.OnRowProceded += new EventHandler(RowProceded);
                writer.Write(Source);                
                Invoke(new DialogResultDelegate(SetDialogResult), DialogResult.OK);
            }
            catch (Exception ex)
            {
                Invoke(new RuntimeExceptionDelegate(RuntimeException), ex);
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
            }
#endif
        }

        private void RowProceded(object sender, EventArgs a)
        {
            count++;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (workThread != null &&
                workThread.ThreadState == System.Threading.ThreadState.Running)
                workThread.Abort();
        }

        private void ExportDataDialog_Shown(object sender, EventArgs e)
        {
            workThread = new Thread(new ThreadStart(BackgroundRun));
            workThread.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = String.Format("Create {0}: {1} Record(s) proceeded.",
               Path.GetFileName(FileName), count);
        }
    }
}