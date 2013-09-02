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
    public interface SmartTableAccessor
    {
        TableType TableType { get; }

        int TopRows { get; set; }

        Object FilterPredicate { get; set; }

        SortColumn[] SortColumns { get; set; }

        String[] AccessPredicate { get; set; }

        Object[][] AccessPredicateValues { get; set; }

        int GetTableEstimate(int threshold);
    }
}
