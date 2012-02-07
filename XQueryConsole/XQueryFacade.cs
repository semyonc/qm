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
using System.Diagnostics;

using DataEngine.XQuery;
using WmHelp.XmlGrid;
using System.IO;
using DataEngine.Export;
using System.Xml.XPath;
using System.Collections.Generic;


namespace XQueryConsole
{
    class XQueryFacade: IQueryEngineFacade
    {
        private XQueryCommand command;
        private bool limitResults;

        public XQueryFacade()
        {
        }

        #region IQueryEngineFacade Members

        public void OpenQuery(string queryText, string baseUri)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            DocumentController controller = mainWindow.Controller;
            limitResults = controller.LimitSQLQueryResults;
            if (command != null)
                command.Dispose();
            command = new XQueryCommand(new XQueryDsContext(mainWindow.DatasourceController.Dictionary));
            command.CommandText = queryText;
            command.SearchPath = controller.SearchPath;
            if (!String.IsNullOrEmpty(baseUri))
                command.BaseUri = baseUri;
            command.OptimizerGoal = QueryPlanTarget.AllItems;
        }

        public GridCellGroup Execute()
        {
#if DEBUG
            DataEngine.CoreServices.PerfMonitor.Global.Clear();
#endif
            IsTruncated = false;
            XQueryNodeIterator iter = command.Execute();
            XPathGridBuilder builder = new XPathGridBuilder();
            GridCellGroup rootCell = new GridCellGroup();
            List<XPathItem> res = new List<XPathItem>();
            while (iter.MoveNext())
            {
                if (limitResults && res.Count == 1000)
                {
                    IsTruncated = true;
                    command.Terminate();
                    break;
                }
                res.Add(iter.Current.Clone());
            }
            builder.ParseNodes(rootCell, res);
#if DEBUG
            DataEngine.CoreServices.PerfMonitor.Global.TraceStats();
            Trace.WriteLine(String.Format("TypeValueCache: Hit = {0}, Miss = {1}, Ratio = {2}",
                TypedValueCache.hitCount, TypedValueCache.missCount, (double)TypedValueCache.hitCount / (TypedValueCache.hitCount + TypedValueCache.missCount)));
#endif
            return rootCell;            
        }

        public System.Data.Common.DbDataReader ExecuteReader()
        {
            throw new NotImplementedException();
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

        public int ExportTo(string fileName, ExportTarget target)
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
                        try
                        {
                            int i = 0;
                            XQueryNodeIterator res = command.Execute();
                            while (res.MoveNext())
                            {
                                if (i++ > 0)
                                    writer.WriteWhitespace("\n");
                                XPathItem item = res.Current;
                                if (item.IsNode)
                                {
                                    XPathNavigator nav = (XPathNavigator)item;
                                    nav.WriteSubtree(writer);
                                }
                                else
                                    writer.WriteString(item.Value);
                            }
                            return i;
                        }
                        finally
                        {
                            writer.Close();
                        }
                    }
                default:
                    throw new ArgumentException("target");
            }
        }

        public bool IsTruncated { get; private set; }

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

        #endregion
    }
}
