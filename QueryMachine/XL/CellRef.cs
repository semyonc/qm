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
using System.IO;

namespace DataEngine.XL
{
    public class CellRef
    {
        // http://office.microsoft.com/en-us/excel-help/excel-specifications-and-limits-HP010342495.aspx
        
        public const int MaxColumn = 16384;
        public const int MaxRow = 1048576;

        public CellRef()
        {
            C1 = 1;
            R1 = 1;
            C2 = MaxColumn; 
            R2 = MaxRow;
        }

        public CellRef(int col, int row)
        {
            C1 = C2 = col;
            R1 = R2 = row;
        }

        public CellRef(int col1, int row1, int col2, int row2)
        {
            C1 = col1;
            R1 = row1;
            C2 = col2;
            R2 = row2;
        }

        public CellRef(CellRef src)
        {
            FileName = src.FileName;
            WorksheetName = src.WorksheetName;
            
            C1 = src.C1;
            R1 = src.R1;
            C2 = src.C2;
            R2 = src.R2;
        }

        public bool IsRange
        {
            get
            {
                return C1 != C2 || R1 != R2;
            }
        }

        public string FileName { get; set; }

        public string WorksheetName { get; set; }

        public int C1 { get; private set; }

        public int R1 { get; private set; }

        public int C2 { get; private set; }

        public int R2 { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(FileName))
            {
                string pathString = Path.GetDirectoryName(FileName);
                if (pathString != "")
                {
                    sb.Append(pathString);
                    sb.Append('\\');
                }
                sb.AppendFormat("[{0}]", Path.GetFileName(FileName));
            }
            if (!String.IsNullOrEmpty(WorksheetName))
                sb.Append(WorksheetName);
            if (!String.IsNullOrEmpty(FileName) || !String.IsNullOrEmpty(WorksheetName))
                sb.Append('!');
            sb.AppendFormat("{0}{1}", GetColumnLiteralName(C1), R1);
            if (IsRange)
                sb.AppendFormat(":{0}{1}", GetColumnLiteralName(C2), R2);
            return sb.ToString();
        }        

        public static CellRef Parse(string address)
        {
            CellRef res = new CellRef();
            string[] p;
            char c = address[0];
            if (c == '\'')
            {
                StringBuilder sb = new StringBuilder();
                int k = 1;
                for (; k < address.Length; k++)
                {
                    c = address[k];
                    if (c == '\'')
                        break;
                    sb.Append(c);
                }
                c = address[++k];
                if (c == '!' || k < address.Length - 1)
                {
                    p = new string[2];
                    p[0] = sb.ToString();
                    p[1] = address.Substring(k + 1);
                }
                else
                    return null;
            }
            else
                p = address.Split('!');
            string[] range;
            if (p.Length == 1)
            {
                range = p[0].Split(':');
                if (range.Length == 1)
                {
                    string[] p1 = range[0].Split(']');
                    if (p1.Length == 2)
                    {
                        res.WorksheetName = p1[1];
                        string[] p2 = p1[0].Split('[');
                        if (p2.Length != 2)
                            return null;
                        res.FileName = p2[0] + p2[1];
                        return res;
                    }
                }
            }
            else
                if (p.Length == 2)
                {
                    string[] p1 = p[0].Split(']');
                    if (p1.Length == 1)
                        res.WorksheetName = p1[0];
                    else
                    {
                        if (p1.Length > 2)
                            return null;
                        res.WorksheetName = p1[1];
                        string[] p2 = p1[0].Split('[');
                        if (p2.Length != 2)
                            return null;
                        res.FileName = p2[0] + p2[1];
                    }
                    range = p[1].Split(':');
                }
                else
                    return null;
            if (range.Length == 1)
            {
                if (range[0] != "")
                {
                    int col;
                    int row;
                    if (!ParseRef(range[0], out col, out row))
                        return null;
                    res.C1 = res.C2 = col;
                    res.R1 = res.R2 = row;
                }
            }
            else
            {
                int col1;
                int row1;
                int col2;
                int row2;
                if (!ParseRef(range[0], out col1, out row1) ||
                    !ParseRef(range[1], out col2, out row2))
                    return null;
                res.C1 = col1;
                res.R1 = row1;
                res.C2 = col2;
                res.R2 = row2;
            }
            return res;
        }

        public static CellRef ParseRef(string address)
        {
            int c;
            int r;
            CellRef res = new CellRef();
            if (!ParseRef(address, out c, out r))
                return null;
            res.C1 = res.C2 = c;
            res.R1 = res.R2 = r;
            return res;
        }

        private static bool ParseRef(string text, out int col, out int row)
        {
            int index = 0;            
            col = 0;
            row = 0;
            while (index < text.Length && Char.IsLetter(text[index]))
            {
                int digit = (int)Char.ToUpperInvariant(text[index++]) - (int)'A' + 1;
                if (digit < 0 || digit > alphabet.Length)
                    return false;
                col = col * alphabet.Length + digit;               
            }
            if (index < text.Length)
            {
                while (index < text.Length)
                {
                    if (!Char.IsDigit(text[index]))
                        return false;
                    int digit = (int)text[index++] - (int)'0';
                    row = row * 10 + digit;
                }
                if (col > 0 && col <= MaxColumn && row > 0 && row <= MaxRow)
                    return true;
            }
            return false;
        }
        
        private static readonly char[] alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
                'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public static String GetColumnLiteralName(int index)
        {
            int digitBase = alphabet.Length;
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Insert(0, alphabet[(index -1) % digitBase]);
                index = (index -1) / digitBase;
            } while (index > 0);
            return sb.ToString();
        }
    }
}
