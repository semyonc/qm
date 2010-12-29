//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using WmHelp.XmlGrid;

namespace XQueryConsole
{
    public interface IQueryEngineFacade
    {
        void OpenQuery(string queryText, string baseUri);
        GridCellGroup Execute();
        void Terminate();
        void CloseQuery();
        bool IsQueryException(Exception ex);
        bool CanExportDS(GridCellGroup rootCell);
        string GetSourceXML(GridCellGroup rootCell);
        void ExportTo(GridCellGroup rootCell, string fileName);

        string EngineName { get; }
        string DefaultExt { get; }
    }
}
