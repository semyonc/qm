//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Diagnostics;
using System.Data.Common;

using DataEngine.Export;
using WmHelp.XmlGrid;

namespace XQueryConsole
{
    public interface IQueryEngineFacade
    {
        void OpenQuery(string queryText, string baseUri);
        
        GridCellGroup Execute();
        DbDataReader ExecuteReader();

        void Terminate();
        void CloseQuery();
        bool IsQueryException(Exception ex);
        bool IsFlatTable(GridCellGroup rootCell);
        string GetSourceXML(GridCellGroup rootCell);
        int ExportTo(string fileName, ExportTarget target);

        bool IsTruncated { get; }
        string EngineName { get; }
        string DefaultExt { get; }        
    }
}
