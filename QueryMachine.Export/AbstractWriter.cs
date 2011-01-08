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
using DataEngine.CoreServices.Data;

namespace DataEngine.Export
{

    public abstract class AbstractWriter
    {
        public abstract void Write(Resultset rs);

        protected void RowProceded()
        {
            if (OnRowProceded != null)
                OnRowProceded(this, EventArgs.Empty);
        }

        public event EventHandler OnRowProceded;
    }
}
