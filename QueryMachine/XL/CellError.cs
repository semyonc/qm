//        Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DataEngine.XL
{
    public class CellError
    {
        public CellError()
        {
        }

        public override string ToString()
        {
            return "#VALUE!";
        }
    }
}
