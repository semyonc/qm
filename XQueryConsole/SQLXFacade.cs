//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.ComponentModel;
using System.Xml;
using System.Windows;
using System.IO;
using System.Text;
using DataEngine.CoreServices.Data;
using System.Reflection;
using System.Globalization;
using System.Windows.Threading;
using System.Diagnostics;
using System.Data.Common;

using DataEngine;
using DataEngine.ADO;
using DataEngine.Parser;
using WmHelp.XmlGrid;
using DataEngine.Export;

namespace XQueryConsole
{
    class SQLXFacade : DispatcherObject, IQueryEngineFacade
    {
        private Command command;
        private DataReader reader;
        private QueryContext context;
        private bool limitResults;
        
        private volatile bool canExportDS;

        public SQLXFacade()
        {            
        }

        #region IQueryEngineFacade Members

        public void OpenQuery(string queryText, string baseUri)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            DataSourceTreeController dsController = mainWindow.DatasourceController;
            DocumentController docController = mainWindow.Controller;
            context = null;
            if (reader != null)
                reader.Close();
            limitResults = docController.LimitSQLQueryResults;
            command = new Command(dsController.Dictionary);
            string searchPath = docController.SearchPath;
            if (!String.IsNullOrEmpty(baseUri) && !String.IsNullOrEmpty(searchPath))
                searchPath = baseUri + ";" + searchPath;
            command.SearchPath = searchPath;
            command.CommandText = queryText;
            command.BeforeExecute += new QueryExecuteDelegate(command_BeforeExecute);
        }

        private void command_BeforeExecute(Command source, Notation notation, Optimizer optimizer, QueryContext context)
        {
            this.context = context;
            Notation.Record[] recs = notation.Select(Descriptor.Root, 1);
            if (recs.Length > 0)
            {
                Notation.Record[] recsd = notation.Select(recs[0].Arg0, Descriptor.Binding, 1);
                if (recsd.Length > 0)
                    throw new ESQLException("Query parameters is not supported in XQueryConsole", null);
            }
            if (context.UseSampleData)
            {
                String path = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");
                if (Directory.Exists(path))
                    context.DatabaseDictionary.SearchPath = path;                
            }
        }

        public GridCellGroup Execute()
        {
            reader = (DataReader)command.ExecuteReader();
            ResultsetGridBuilder builder = new ResultsetGridBuilder();
            if (limitResults)
                builder.TableLimit = 1000;
            GridCellGroup res = builder.Parse(reader.Source);
            canExportDS = builder.CanExportDS;
            IsTruncated = builder.IsTruncated;
            return res;
        }

        public DbDataReader ExecuteReader()
        {
            return command.ExecuteReader();
        }

        public void Terminate()
        {
            if (context != null)
                context.Cancel();
        }

        public void CloseQuery()
        {
            if (reader != null)
            {
                reader.Close();
                reader = null;
                command = null;
                context = null;
            }
        }

        public bool IsQueryException(Exception ex)
        {
            return ex is ESQLException;
        }

        public bool CanExportDS(GridCellGroup rootCell)
        {
            return rootCell is ResultsetGridBuilder.RootCell && canExportDS;
        }

        public string GetSourceXML(GridCellGroup rootCell)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            StringBuilder sb = new StringBuilder();
            ResultsetGridBuilder builder = new ResultsetGridBuilder();
            Resultset rs = builder.CreateResultset((ResultsetGridBuilder.RootCell)rootCell);
            new XmlFileWriter(XmlWriter.Create(sb, settings)).Write(rs);
            return sb.ToString();
        }

        public int ExportTo(string fileName, ExportTarget target)
        {
            string ext = Path.GetExtension(fileName);
            AbstractWriter writer = null;
            switch (target)
            {
                case ExportTarget.Xml:
                    writer = new XmlFileWriter(fileName);
                    break;

                case ExportTarget.Csv:
                    writer = new CsvWriter(fileName,
                        CultureInfo.CurrentCulture.TextInfo.ListSeparator, true);
                    break;

                case ExportTarget.TabDelimited:
                    writer = new CsvWriter(fileName, "\t", true);
                    break;

                case ExportTarget.FixedLength:
                    writer = new FlvWriter(fileName, true);
                    break;

                case ExportTarget.AdoNet:
                    writer = new AdoNetWriter(fileName);
                    break;

                case ExportTarget.Xls:
                    writer = new XlFileWriter(fileName);
                    break;
            }
            if (writer == null)
                throw new ArgumentException("target");
            DataReader reader = (DataReader)command.ExecuteReader();
            try
            {
                writer.Write(reader.Source);
                return writer.RowCount;
            }
            finally
            {
                reader.Close();
            }
        }

        public bool IsTruncated { get; private set; }

        public string EngineName
        {
            get
            {
                return "SQLX";
            }
        }

        public string DefaultExt
        {
            get { return ".xsql"; }
        }

        #endregion
    }
}
