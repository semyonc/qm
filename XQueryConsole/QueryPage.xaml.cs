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
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Common;

using WmHelp.XmlGrid;
using DataEngine.XQuery;
using System.ComponentModel;
using ICSharpCode.AvalonEdit;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using ICSharpCode.AvalonEdit.Highlighting;
using DataEngine.Export;

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for QueryPage.xaml
    /// </summary>
    public partial class QueryPage : Page, INotifyPropertyChanged
    {
        private string fileName;
        private string filePath;
        private XmlGridView xmlGrid;
        private Task queryTask;
        private IQueryEngineFacade engine;
        private Stopwatch workTime;
        private String status;
        private bool hasTextContent;
        private bool hasModified;

        private GridLength height1;
        private GridLength height2;
        
        private BracketHighlightRenderer bracketRenderer;
        private BracketSearcher bracketSearcher;
        private ControlAdorner ctrlAdorner;

        public static readonly ICommand ExecuteCommand =
            new RoutedUICommand("Execute", "Execute", typeof(QueryPage));
        public static readonly ICommand SaveResultCommand =
            new RoutedUICommand("Save Result", "SaveResult", typeof(QueryPage));
        public static readonly ICommand MoveResultCommand =
            new RoutedUICommand("Move Result", "SaveResult", typeof(QueryPage));

        public delegate void QueryExecuteCallback(DbDataReader result);

        private class EmbeddedGrid : XmlGridView
        {
            public EmbeddedGrid()
                : base()
            {
                SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, false);
            }
        }

        public QueryPage(IQueryEngineFacade engine)
        {
            InitializeComponent();

            this.engine = engine;
            textEditor.SyntaxHighlighting = 
                HighlightingManager.Instance.GetDefinitionByExtension(engine.DefaultExt);

            height1 = layout.RowDefinitions[3].Height;
            height2 = layout.RowDefinitions[4].Height;
            
            filePath = String.Empty;
            fileName = String.Empty;
            HasContent = false;
            ShowResultPane = false;
            hasModified = false;
            
            xmlGrid = new EmbeddedGrid();
            xmlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            xmlGrid.Location = new System.Drawing.Point(0, 100);
            xmlGrid.Name = "xmlGridView1";
            xmlGrid.Size = new System.Drawing.Size(100, 100);
            xmlGrid.TabIndex = 0;
            xmlGrid.AutoHeightCells = true;            
            windowsFormsHost.Child = xmlGrid;

            // see http://community.sharpdevelop.net/forums/t/10312.aspx          
            bracketSearcher = new BracketSearcher();
            bracketRenderer = new BracketHighlightRenderer(textEditor.TextArea.TextView);
            textEditor.TextArea.Caret.PositionChanged += new EventHandler(Caret_PositionChanged);
            ctrlAdorner = new ControlAdorner(textEditor, new CircularProgressBar());
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            BracketSearchResult bracketSearchResult = bracketSearcher
                .SearchBracket(textEditor.Document, textEditor.TextArea.Caret.Offset);
            bracketRenderer.SetHighlight(bracketSearchResult);
        }

        public void Load()
        {
            HasContent = true;
            textEditor.Load(FileName);
            Update(false);
        }

        public void Save()
        {
            textEditor.Save(FileName);
            Update(false);
        }

        public bool HasContent
        {
            get; private set;
        }

        public bool Modified
        {
            get
            {
                return hasModified;
            }
        }

        public string FileName 
        {
            get
            {
                return System.IO.Path.Combine(filePath, fileName);
            }
            set
            {
                fileName = System.IO.Path.GetFileName(value);
                filePath = System.IO.Path.GetDirectoryName(value);
                Title = fileName;
            }
        }

        public string ShortFileName
        {
            get
            {
                return fileName;
            }
        }

        public string FilePath
        {
            get
            {
                return filePath;
            }
        }

        public bool ShowResultPane
        {
            get
            {
                return splitter.Visibility == Visibility.Visible;
            }
            set
            {
                if (value != (splitter.Visibility == Visibility.Visible))
                {
                    if (value)
                    {
                        layout.RowDefinitions[3].Height = height1;
                        layout.RowDefinitions[4].Height = height2;
                        splitter.Visibility = Visibility.Visible;                        
                    }
                    else
                    {
                        height1 = layout.RowDefinitions[3].Height;
                        height2 = layout.RowDefinitions[4].Height;
                        layout.RowDefinitions[3].Height = new GridLength(0);
                        layout.RowDefinitions[4].Height = new GridLength(0);
                        splitter.Visibility = Visibility.Hidden;
                    }
                    OnPropertyChanged("ShowResultPane");
                }
            }
        }

        public string StatusText
        {
            get
            {
                return status;
            }
        }

        public IQueryEngineFacade QueryFacade
        {
            get
            {
                return engine;
            }
        }

        public String QueryText
        {
            get
            {
                return textEditor.Text;
            }
        }
        
        private void Update(bool modified)
        {
            if (hasModified != modified)
            {
                if (modified)
                    Title = fileName + "*";
                else
                    Title = fileName;
                hasModified = modified;
            }
        }

        public void Close()
        {
            windowsFormsHost.Dispose();
            engine.CloseQuery();
        }

        public void BeginQueryExecute(QueryExecuteCallback callback)
        {
            engine.OpenQuery(textEditor.Text, filePath);
            textEditor.IsReadOnly = true;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(textEditor);
            ctrlAdorner.SetLayer(adornerLayer);
            queryTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    DbDataReader res = engine.ExecuteReader();
                    Dispatcher.Invoke(new HideAdornerDelegate(HideAdorner));
                    Dispatcher.BeginInvoke(callback, res);
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke(new RuntimeExceptionDelegate(HandleException), ex);
                }
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement overflowGrid = gridToolBar.Template.FindName("OverflowGrid", gridToolBar) as FrameworkElement;
            if (overflowGrid != null)
                overflowGrid.Visibility = Visibility.Collapsed;
            ScrollViewer sv = Extensions.FindVisualChild<ScrollViewer>(sourceViewer);
            if (sv != null)
            {
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            textEditor.Focus();
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            HasContent = true;
            Update(true);
        }

        private void textEditor_PreviewDrop(object sender, DragEventArgs e)
        {
            textEditor.Focus();
        }

        private void gridViewButton_Click(object sender, RoutedEventArgs e)
        {
            gridViewBorder.Visibility = System.Windows.Visibility.Visible;
            sourceViewBorder.Visibility = System.Windows.Visibility.Hidden;
        }

        private void xmlSourceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!hasTextContent)
                CreateTextContent();
            gridViewBorder.Visibility = System.Windows.Visibility.Hidden;
            sourceViewBorder.Visibility = System.Windows.Visibility.Visible;
        }

        delegate void UpdateTextEditDelegate(int rows);
        delegate void UpdateXmlGridDelegate(GridCellGroup rootCell);
        delegate void RuntimeExceptionDelegate(Exception ex);
        delegate void HideAdornerDelegate();

        private void CommandBinding_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            engine.OpenQuery(textEditor.Text, filePath);
            xmlGrid.Cell = null;
            gridViewButton.IsChecked = true;
            gridViewBorder.Visibility = System.Windows.Visibility.Visible;
            sourceViewBorder.Visibility = System.Windows.Visibility.Hidden;
            sourceViewer.Clear();
            hasTextContent = false;
            textEditor.IsReadOnly = true;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(textEditor);
            ctrlAdorner.SetLayer(adornerLayer);
            workTime = new Stopwatch();     
            workTime.Start();
            queryTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        GridCellGroup rootCell = engine.Execute();
                        Dispatcher.BeginInvoke(new UpdateXmlGridDelegate(UpdateXmlGrid), rootCell);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.BeginInvoke(new RuntimeExceptionDelegate(HandleException), ex);
                    }
                });
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = queryTask == null;
        }

        private void HandleException(Exception ex)
        {
            if (workTime != null)
                workTime.Stop();
            queryTask = null;
            textEditor.IsReadOnly = false;
            ctrlAdorner.RemoveLayer();
            if (engine.IsQueryException(ex))
                MessageBox.Show(ex.Message /*+ "\r\n" + ex.StackTrace*/, "Query error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            else
                if ((ex is OperationCanceledException))
                {
                    status = "Cancelled";
                    OnPropertyChanged("StatusText");
                }
                else                
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Runtime error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            textEditor.Focus();
            engine.CloseQuery();
        }

        private string FormatElapsedTime()
        {
            if (workTime.Elapsed.Hours > 0)
                return String.Format("{0} hr, {1} min, {2} sec",
                    workTime.Elapsed.Hours, workTime.Elapsed.Minutes, workTime.Elapsed.Seconds);
            else if (workTime.Elapsed.Minutes > 0)
                return String.Format("{0} min, {1} sec",
                    workTime.Elapsed.Minutes, workTime.Elapsed.Seconds);
            else
                return String.Format("{0} sec ({1} ms)",
                    workTime.Elapsed.Seconds, workTime.ElapsedMilliseconds);
        }

        private void UpdateTextEdit(int rowsExported)
        {
            workTime.Stop();
            queryTask = null;
            textEditor.IsReadOnly = false;
            ctrlAdorner.RemoveLayer();
            textEditor.Focus();
            if (rowsExported != -1)
            {
                String elapsed = FormatElapsedTime();
                status = String.Format("{0} node(s) exported. {1} elapsed.", rowsExported, elapsed);
                OnPropertyChanged("StatusText");
            }
        }

        private void UpdateXmlGrid(GridCellGroup rootCell)
        {
            workTime.Stop();
            queryTask = null;
            textEditor.IsReadOnly = false;
            ctrlAdorner.RemoveLayer();
            textEditor.Focus();
            String elapsed = FormatElapsedTime();
            if (rootCell.Table.Height > 0 && rootCell.Table.Width > 0 &&
                rootCell.Table[0, 0] is GridHeadLabel)
            {
                xmlGrid.ShowColumnHeader = true;
                status = String.Format("{0} node(s) read. {1} elapsed.",
                   rootCell.Table.Height - 1, elapsed);
            }
            else
            {
                xmlGrid.ShowColumnHeader = false;
                if (rootCell.Table.Height > 1)
                    status = String.Format("{0} node(s) read. {1} elapsed.",
                       rootCell.Table.Height, elapsed);
                else
                    status = String.Format("{0} elapsed.", elapsed);
            }
            MainWindow main = (MainWindow)Application.Current.MainWindow;
            if (main.Controller.LimitSQLQueryResults && engine.IsTruncated)
                status += " Possible not all results selected.";
            OnPropertyChanged("StatusText");
            xmlGrid.Cell = rootCell;
            ShowResultPane = true;
            if (engine.DefaultExt == ".xq")
            {
                xmlSourceButton.IsChecked = true;
                xmlSourceButton_Click(null, null);
            }
        }

        private void CreateTextContent()
        {
            if (xmlGrid.Cell != null)
            {
                Cursor = Cursors.Wait;
                try
                {
                    sourceViewer.Document.Text = engine.GetSourceXML(xmlGrid.Cell);
                    hasTextContent = true;
                }
                finally
                {
                    Cursor = null;
                }
            }
        }

        private void CommandBinding_SaveResExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".xml";
            if (engine.CanExportDS(xmlGrid.Cell))
                dlg.Filter = "XML Data File (*.xml)|*.xml|Tab delimited text (*.txt)|*.txt|"+
                    "Comma Separated Value (*.csv)|*.csv|Fixed Length Text (*.txt)|*.txt|ADO .NET Dataset (*.xml)|*.xml|MS Office Excel 2007-2010 Workbook (*.xlsx)|*.xlsx";
            else
                dlg.Filter = "XML Data File (*.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                engine.OpenQuery(textEditor.Text, filePath);                    
                textEditor.IsReadOnly = true;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(textEditor);
                ctrlAdorner.SetLayer(adornerLayer);
                workTime = new Stopwatch();
                workTime.Start();
                queryTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        int nRows = 0;
                        switch (dlg.FilterIndex)
                        {
                            case 1:
                                nRows = engine.ExportTo(dlg.FileName, ExportTarget.Xml);
                                break;

                            case 2:
                                nRows = engine.ExportTo(dlg.FileName, ExportTarget.TabDelimited);
                                break;

                            case 3:
                                nRows = engine.ExportTo(dlg.FileName, ExportTarget.Csv);
                                break;

                            case 4:
                                nRows = engine.ExportTo(dlg.FileName, ExportTarget.FixedLength);
                                break;

                            case 5:
                                nRows = engine.ExportTo(dlg.FileName, ExportTarget.AdoNet);
                                break;

                            case 6:
                                nRows = engine.ExportTo(dlg.FileName, ExportTarget.Xls);
                                break;
                        }
                        Dispatcher.BeginInvoke(new UpdateTextEditDelegate(UpdateTextEdit), nRows);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.BeginInvoke(new RuntimeExceptionDelegate(HandleException), ex);
                    }
                });
            }
        }

        private void CommandBinding_StopExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (queryTask != null)
                engine.Terminate();
        }

        private void CommandBinding_CanStopExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = queryTask != null;
        }

        private void CommandBinding_BatchMoveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = xmlGrid != null && xmlGrid.Cell != null && engine.CanExportDS(xmlGrid.Cell);
        }

        private void CommandBinding_BatchMoveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            BeginQueryExecute(new QueryExecuteCallback(BeginBatchMove)); 
        }

        private void HideAdorner()
        {
            queryTask = null;
            textEditor.IsReadOnly = false;
            ctrlAdorner.RemoveLayer();
        }

        private void BeginBatchMove(DbDataReader reader)
        {
            CreateTableDialog dlg = new CreateTableDialog();
            dlg.TableName = System.IO.Path.GetFileNameWithoutExtension(ShortFileName).ToUpperInvariant();
            dlg.BatchMove.Source = ((DataEngine.ADO.DataReader)reader).Source;
            if (dlg.ShowDialog() == true)
            {
                textEditor.IsReadOnly = true;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(textEditor);
                ctrlAdorner.SetLayer(adornerLayer);
                workTime = new Stopwatch();
                workTime.Start();
                queryTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        AdoProviderWriter writer = new AdoProviderWriter(dlg.BatchMove);
                        writer.Write(dlg.BatchMove.Source);
                        Dispatcher.BeginInvoke(new UpdateTextEditDelegate(UpdateTextEdit), writer.RowCount);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.BeginInvoke(new RuntimeExceptionDelegate(HandleException), ex);
                    }
                });
            }
        }
    }
}
