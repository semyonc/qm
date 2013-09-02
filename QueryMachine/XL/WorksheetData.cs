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

namespace DataEngine.XL
{
    public delegate bool WorksheetDataVisitor(int row, int column, Object value);

    public class WorksheetData
    {
        private List<SparseRow> srows = new List<SparseRow>();

        public WorksheetData()
        {
        }

        public Object this[int row, int col]
        {
            get
            {
                SparseRow entry = GetRow(row, false);
                if (entry != null)
                    return entry[col];
                return null;
            }
            set
            {
                SparseRow entry = GetRow(row, true);
                entry[col] = value;
            }
        }

        public void ForEach(WorksheetDataVisitor visitor)
        {
            ForEach(0, 0, Int32.MaxValue, Int32.MaxValue, visitor);
        }

        public void ForEach(int startRow, int startCol, int endRow, int endCol, WorksheetDataVisitor visitor)
        {
            int startRowIndex = GetRowIndex(startRow);
            if (startRowIndex != -1)
            {
                for (int i = startRowIndex; i < srows.Count; i++)
                {
                    SparseRow srow = srows[i];
                    if (srow.Row > endRow)
                        break;
                    List<int> col = srow.columns;
                    List<Object> value = srow.values;
                    for (int j = 0; j < col.Count && col[j] <= endCol; j++)
                        if (startCol <= col[j])
                            if (!visitor(srow.Row, col[j], value[j]))
                                return;
                }
            }
        }

        public IEnumerable<SparseRow> GetRows()
        {
            return srows;
        }

        public SparseRow GetRow(int row)
        {
            return GetRow(row, false);
        }

        public SparseRow GetRow(int row, bool create)
        {
            int low = 0;
            int high = srows.Count - 1;
            int mid = 0;

            while (low <= high)
            {
                mid = (low + high) / 2;
                SparseRow srow = srows[mid];
                if (srow.Row < row)
                    low = mid + 1;
                else if (srow.Row > row)
                    high = mid - 1;
                else
                    return srow;
            }

            if (create)
            {
                SparseRow srow = new SparseRow(row);
                if (srows.Count > 0 && srows[mid].Row < row)
                    srows.Insert(mid + 1, srow);
                else
                    srows.Insert(mid, srow);
                return srow;
            }

            return null;           
        }

        private int GetRowIndex(int row)
        {
            int low = 0;
            int high = srows.Count - 1;
            int mid = 0;

            while (low <= high)
            {
                mid = (low + high) / 2;
                SparseRow srow = srows[mid];
                if (srow.Row < row)
                    low = mid + 1;
                else if (srow.Row > row)
                    high = mid - 1;
                else
                    return mid;
            }

            return -1;		
        }

        public void Clear()
        {
            srows.Clear();
        }

        public int MaxRowCapacity()
        {
            int maxCapacity = 0;
            foreach (SparseRow entry in srows)
                if (entry.Count > maxCapacity)
                    maxCapacity = entry.Count;
            return maxCapacity;
        }

        public int MinRow
        {
            get
            {
                if (srows.Count == 0)
                    return 0;
                return srows[0].Row;
            }
        }

        public int MaxRow
        {
            get
            {
                if (srows.Count == 0)
                    return 0;
                return srows[srows.Count -1].Row;
            }
        }

        public class SparseRow
        {
            internal List<int> columns;
            internal List<Object> values;

            internal SparseRow(int row)
                : this(row, 1)
            {

            }

            internal SparseRow(int row, int capacity)
            {
                Row = row;
                columns = new List<int>(capacity);
                values = new List<Object>(capacity);
            }

            public SparseRow(SparseRow bucket)
                : this(bucket.Row, bucket.Count)
            {
                for (int k = 0; k < bucket.Count; k++)
                {
                    columns.Add(bucket.columns[k]);
                    values.Add(bucket.values[k]);
                }
            }

            private Object Get(int column)
            {
                int low = 0;
                int high = Count - 1;
                int mid = 0;
                while (low <= high)
                {
                    mid = (low + high) / 2;
                    if (columns[mid] < column)
                        low = mid + 1;
                    else if (columns[mid] > column)
                        high = mid - 1;
                    else
                        return values[mid];
                }
                return null;
            }

            private void Set(int column, Object value)
            {
                int low = 0;
                int high = Count - 1;
                int mid = 0;

                while (low <= high)
                {
                    mid = (low + high) / 2;
                    if (columns[mid] < column)
                        low = mid + 1;
                    else if (columns[mid] > column)
                        high = mid - 1;
                    else
                    {
                        if (value != null)
                            values[mid] = value;
                        else
                            values.RemoveAt(mid); // 28.02.2013 Need check for errors
                        return;
                    }
                }

                if (value != null)
                {
                    int index = Count > 0 && columns[mid] < column 
                        ? mid + 1 : mid;
                    columns.Insert(index, column);
                    values.Insert(index, value);                    
                }
            }

            public void ForEach(WorksheetDataVisitor visitor)
            {
                ForEach(0, Int32.MaxValue, visitor);
            }

            public void ForEach(int startCol, int endCol, WorksheetDataVisitor visitor)
            {
                for (int j = 0; j < columns.Count && columns[j] <= endCol; j++)
                    if (startCol <= columns[j])
                        if (!visitor(Row, columns[j], values[j]))
                            return;
            }

            public int Row { get; private set; }

            public int MinColumn
            {
                get
                {
                    if (columns.Count == 0)
                        return 0;
                    return columns[0];
                }
            }

            public int MaxColumn
            {
                get
                {
                    if (columns.Count == 0)
                        return 0;
                    return columns[columns.Count -1];
                }
            }

            public Object this[int col]
            {
                get
                {
                    return Get(col);
                }
                set
                {
                    Set(col, value);
                }
            }

            public int Count
            {
                get
                {
                    return columns.Count;
                }
            }            
        }
    }
}
