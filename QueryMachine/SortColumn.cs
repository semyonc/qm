//        Copyright (c) 2008-2012, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.
using System;

namespace DataEngine
{
    public class SortColumn
    {
        public String ColumnName { get; set; }
        public SortDirection Direction { get; set; }

        public SortColumn()
        {
        }

        public SortColumn(string columnName)
        {
            ColumnName = columnName;
        }

        public SortColumn(string columnName, SortDirection direction)
        {
            ColumnName = columnName;
            Direction = direction;
        }
    }
}