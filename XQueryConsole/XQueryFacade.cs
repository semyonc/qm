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
using System.Text;

using DataEngine.XQuery;
using WmHelp.XmlGrid;
using System.IO;
using DataEngine.Export;

namespace XQueryConsole
{
    class XQueryFacade: IQueryEngineFacade
    {
        private XQueryCommand command;

        public XQueryFacade()
        {
        }

        #region IQueryEngineFacade Members

        public void OpenQuery(string queryText, string baseUri)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            DocumentController controller = mainWindow.Controller;
            if (command != null)
                command.Dispose();
            command = new XQueryCommand(new XQueryDsContext(mainWindow.DatasourceController.Dictionary));
            command.CommandText = queryText;
            command.SearchPath = controller.SearchPath;
            if (!String.IsNullOrEmpty(baseUri))
                command.BaseUri = baseUri;
        }

        public GridCellGroup Execute()
        {
            XQueryNodeIterator res = command.Execute();
            XPathGridBuilder builder = new XPathGridBuilder();
            GridCellGroup rootCell = new GridCellGroup();
            builder.ParseNodes(rootCell, res.ToList());
            return rootCell;            
        }

        public void Terminate()
        {
            if (command != null)
                command.Terminate();
        }

        public void CloseQuery()
        {
            if (command != null)
            {
                command.Dispose();
                command = null;
            }
        }

        public bool IsQueryException(Exception ex)
        {
            return ex is XQueryException;
        }

        public bool CanExportDS(GridCellGroup rootCell)
        {
            return false;
        }

        public string GetSourceXML(GridCellGroup rootCell)
        {
            StringBuilder sb = new StringBuilder();
            for (int s = 0; s < rootCell.Table.Height; s++)
            {
                if (s > 0)
                    sb.AppendLine();
                GridCell cell = rootCell.Table[0, s];
                if (cell is XPathGroupCell)
                {
                    XPathGroupCell groupCell = (XPathGroupCell)cell;
                    sb.Append(groupCell.Navigator.OuterXml);
                }
                else
                    sb.Append(cell.Text);
            }
            return sb.ToString();
        }

        public void ExportTo(GridCellGroup rootCell, string fileName, ExportTarget target)
        {
            switch (target)
            {
                case ExportTarget.Xml:
                    {
                        FileStream stream = new FileStream(fileName, FileMode.Create);
                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = true,
                            ConformanceLevel = ConformanceLevel.Auto
                        };
                        XmlWriter writer = XmlWriter.Create(stream, settings);
                        SaveResults(rootCell, writer);
                        writer.Close();
                    }
                    break;
            }
        }

        public void BatchMove(GridCellGroup rootCell, string name)
        {
            return;
        }

        public string EngineName
        {
            get
            {
                return "XQuery";
            }
        }

        public string DefaultExt
        {
            get 
            { 
                return ".xq"; 
            }
        }

        private void SaveResults(GridCellGroup rootCell, XmlWriter writer)
        {
            for (int s = 0; s < rootCell.Table.Height; s++)
            {
                if (s > 0)
                    writer.WriteWhitespace("\n");
                GridCell cell = rootCell.Table[0, s];
                if (cell is XPathGroupCell)
                {
                    XPathGroupCell groupCell = (XPathGroupCell)cell;
                    groupCell.Navigator.WriteSubtree(writer);
                }
                else
                    writer.WriteString(cell.Text);
            }
        }

        #endregion
    }
}
