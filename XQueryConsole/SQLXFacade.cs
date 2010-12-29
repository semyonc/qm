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

using DataEngine;
using DataEngine.ADO;
using DataEngine.Parser;
using WmHelp.XmlGrid;
using DataEngine.Export;
using System.IO;
using System.Text;
using DataEngine.CoreServices.Data;
using System.Reflection;

namespace XQueryConsole
{
    class SQLXFacade : IQueryEngineFacade
    {
        private Command command;
        private DataReader reader;
        private QueryContext context;

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
            return builder.Parse(reader.Source);
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
            return false;
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

        public void ExportTo(GridCellGroup rootCell, string fileName)
        {
            throw new NotImplementedException();
        }

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
