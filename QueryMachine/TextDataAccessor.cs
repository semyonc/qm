//        Copyright (c) 2008-2012, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Globalization;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    public enum TextDataFormat
    {
        Auto,
        TabDelimited, 
        Delimited,
        FixedLength
    }

    public struct TextFileDataFormat
    {
        public TextDataFormat TextFormat { get; set; }
        public string Delimiter { get; set; }
        public int[] Width { get; set; }
        public string NullValue { get; set; }
        public bool ColumnNameHeader { get; set; }
        public Encoding Encoding { get; set; }
        public int MaxScanRows { get; set; }
        public string DateTimeFormat { get; set; }
        public string DecimalSymbol { get; set; }
        public int NumberDigits { get; set; }
        public bool NumberLeadingZeros { get; set; }
        public string CurrencySymbol { get; set; }
        public int CurrencyPosFormat { get; set; }
        public int CurrencyDigits { get; set; }
        public int CurrencyNegFormat { get; set; }
        public string CurrencyThousandSymbol { get; set; }
        public string CurrencyDecimalSymbol { get; set; }
        public bool SequentialProcessing { get; set; }
        public char EncapsulatorChar { get; set; }
        public char EscapeChar { get; set; }
        public bool VariableFields { get; set; }
    }

    public class TextDataAccessor: BaseDataAccessor
    {
        public static int MaxTextColumns = 250;
        public static char CommentChar = '#';

        private static string[] CurrencyNegativePatterns = new string[] { 
            "($1)", "-$1 ", "$-1", "$1-", "(1$)", "-1$", "1-$", "1$-", "-1 $", "-$ 1", "1 $-", "$ 1-", "$ -1", "1- $", "($ 1)", "(1 $)" };
        private static string[] CurrencyPositivePatterns = new string[] {
            "$1", "1$", "$ 1", "1 $" };
        
        private static string[] FieldTypeNames = new string[] {
            "Bit", "Byte", "Short", "Long", "Currency", "Single", "Double", "DateTime", "Text", "Memo", 
            "Char", "Float", "Integer", "LongChar", "Date"};
        private static Type[] FieldTypes = new Type[] {
            typeof(System.Boolean),  // Bit
            typeof(System.Byte),     // Byte
            typeof(System.Int16),    // Short
            typeof(System.Int32),    // Long
            typeof(System.Decimal),  // Currency
            typeof(System.Single),   // Single
            typeof(System.Double),   // Double 
            typeof(System.DateTime), // DateTime
            typeof(System.String),   // Text
            typeof(System.String),   // Memo
            typeof(System.String),   // Char
            typeof(System.Double),   // Float 
            typeof(System.Int32),    // Integer
            typeof(System.Int64),    // LongChar
            typeof(System.DateTime)  // Date
        };

        public TextDataAccessor()
            : base()
        {
        }

        private NumberFormatInfo GetNumberFormatInfo(TextFileDataFormat df)
        {
            NumberFormatInfo nfi = new CultureInfo(CultureInfo.CurrentCulture.Name, false).NumberFormat;
            nfi.NumberDecimalSeparator = df.DecimalSymbol;
            nfi.NumberDecimalDigits = df.NumberDigits;
            nfi.CurrencyDecimalSeparator = df.CurrencyDecimalSymbol;
            nfi.CurrencyGroupSeparator = df.CurrencyThousandSymbol;
            nfi.CurrencyDecimalDigits = df.CurrencyDigits;
            nfi.CurrencyPositivePattern = df.CurrencyPosFormat;
            nfi.CurrencyNegativePattern = df.CurrencyNegFormat;
            nfi.CurrencySymbol = df.CurrencySymbol;
            return nfi;
        }

        private DateTimeFormatInfo GetDateTimeFormatInfo(TextFileDataFormat df)
        {
            DateTimeFormatInfo dfi = new CultureInfo(CultureInfo.CurrentCulture.Name, false).DateTimeFormat;
            dfi.SetAllDateTimePatterns(new string[] { df.DateTimeFormat }, 'd');
            return dfi;
        }

        private DataTable CreateRowType(string fileName, ref TextFileDataFormat format)
        {
            string iniFileName = Path.Combine(Path.GetDirectoryName(fileName), "schema.ini");
            if (File.Exists(iniFileName))
            {
                IniFile ini = new IniFile(iniFileName);
                string[] sections = ini.GetSectionNames();
                string name = Path.GetFileName(fileName);
                foreach (string s in sections)
                    if (s.IndexOfAny(new char[] { '?', '*' }) != -1)
                    {
                        String[] files = Directory.GetFiles(Path.GetDirectoryName(fileName), s);
                        for (int k = 0; k < files.Length; k++)
                            files[k] = Path.GetFileName(files[k]);
                        if (Array.IndexOf<String>(files, name) != -1)
                            return CreateRowType(ini, s, ref format);
                    }
                    else
                        if (String.Compare(s, name, true) == 0)
                            return CreateRowType(ini, s, ref format);
            }
            return null;
        }

        private DataTable CreateRowType(IniFile ini, string section, ref TextFileDataFormat df)
        {            
            ParseIniTextDataFormat(ini, section, ref df);
            ParseIniCharacterSet(ini, section, ref df);            
            
            df.DateTimeFormat = ini.GetIniString(section, "DateTimeFormat", df.DateTimeFormat);
            df.DecimalSymbol = ini.GetIniString(section, "DecimalSymbol", df.DecimalSymbol);
            df.CurrencySymbol = ini.GetIniString(section, "CurrencySymbol", df.CurrencySymbol);
            df.CurrencyThousandSymbol = ini.GetIniString(section, "CurrencyThousandSymbol", df.CurrencyThousandSymbol);
            df.CurrencyDecimalSymbol = ini.GetIniString(section, "CurrencyDecimalSymbol", df.CurrencyDecimalSymbol);

            df.NumberDigits = ini.GetIniNumber(section, "NumberDigits", df.NumberDigits);
            df.CurrencyDigits = ini.GetIniNumber(section, "CurrencyDigits", df.CurrencyDigits);

            df.ColumnNameHeader = ini.GetIniBoolean(section, "ColNameHeader", df.ColumnNameHeader);
            df.NumberLeadingZeros = ini.GetIniBoolean(section, "NumberLeadingZeros", df.NumberLeadingZeros);
            df.SequentialProcessing = ini.GetIniBoolean(section, "SequentialProcessing", df.SequentialProcessing);

            ParseCurrencyNegFormat(ini, section, ref df);
            ParseCurrencyPosFormat(ini, section, ref df);

            df.NullValue = ini.GetIniString(section, "NullValue", null);
            df.VariableFields = ini.GetIniBoolean(section, "VariableFields", false);

            string encapsulatorChar = ini.GetIniString(section, "Encapsulator", Convert.ToString(df.EncapsulatorChar));
            if (encapsulatorChar == "#0")
                df.EncapsulatorChar = '\0';
            else
            {
                if (encapsulatorChar.Length > 1)
                    throw new ESQLException(Properties.Resources.InvalidCSVChar);
                df.EncapsulatorChar = encapsulatorChar[0];
            }

            string escapeChar = ini.GetIniString(section, "Escape", Convert.ToString(df.EscapeChar));
            if (escapeChar == "#0")
                df.EscapeChar = '\0';
            else
            {
                if (escapeChar.Length > 1)
                    throw new ESQLException(Properties.Resources.InvalidCSVChar);
                df.EscapeChar = escapeChar[0];
            }

            List<string> names = new List<string>();
            List<int> length = new List<int>();
            DataProviderHelper helper = new DataProviderHelper();
            DataTable dt = RowType.CreateSchemaTable();

            string[] keys = ini.GetEntryNames(section);
            Regex regex = new Regex("^Col[0-9]+$");
            StringBuilder sb = new StringBuilder();
            string[] values = new string[4];
            int ordinal = 0;
            for (int k = 0; k < keys.Length; k++)
                if (regex.IsMatch(keys[k]))
                {
                    CsvParser parser = new CsvParser(' ');
                    if (parser.Get((string)ini.GetValue(section, keys[k]), values) > 0)
                    {
                        DataRow dr = dt.NewRow();
                        dr["ColumnOrdinal"] = ordinal++;
                        dr["ColumnName"] = Util.CreateUniqueName(names, helper.NativeFormatIdentifier(values[0]));
                        dr["DataType"] = DecodeDataType(values[1]);
                        if (values[2] == "Width" && values[3] != null)
                        {
                            int size = Int32.Parse(values[3]);
                            if ((Type)dr["DataType"] == typeof(System.String))
                                dr["ColumnSize"] = size;
                            length.Add(size);
                        }
                        else
                            if (df.TextFormat == TextDataFormat.FixedLength)
                                throw new ESQLException(Properties.Resources.ExpectedWidthInSchemaIni, values[0]);
                        dt.Rows.Add(dr);                        
                    }
                }
            if (df.TextFormat == TextDataFormat.FixedLength)
            {
                if (dt.Rows.Count == 0)
                    throw new ESQLException(Properties.Resources.ColumnDataTypeRequred);
                df.Width = length.ToArray();
            }
            return dt;
        }

        private object DecodeDataType(string name)
        {
            int index = Array.IndexOf<String>(FieldTypeNames, name);
            if (index != -1)
                return FieldTypes[index];
            else
                return typeof(System.String);
        }

        private void ParseIniTextDataFormat(IniFile ini, string section, ref TextFileDataFormat df)
        {
            if (ini.HasEntry(section, "Format"))
            {
                string value = (string)ini.GetValue(section, "Format");
                if (value == "TabDelimited")
                {
                    df.TextFormat = TextDataFormat.TabDelimited;
                    df.Delimiter = "\t";
                }
                else if (value == "CSVDelimited")
                    df.TextFormat = TextDataFormat.Delimited;
                else if (value.StartsWith("Delimited("))
                {
                    df.TextFormat = TextDataFormat.Delimited;
                    string s = value.Substring(value.IndexOf('(') + 1, 1);
                    if (s != "" && s != ")")
                        df.Delimiter = s;
                }
                else if (value == "FixedLength")
                    df.TextFormat = TextDataFormat.FixedLength;
            }
        }

        private void ParseIniCharacterSet(IniFile ini, string section, ref TextFileDataFormat df)
        {
            if (ini.HasEntry(section, "CharacterSet"))
            {
                string value = (string)ini.GetValue(section, "CharacterSet");
                if (value == "ANSI")
                    df.Encoding = Encoding.Default;
                else 
                    df.Encoding = Encoding.GetEncoding(value);
            }
        }

        private void ParseCurrencyPosFormat(IniFile ini, string section, ref TextFileDataFormat df)
        {
            if (ini.HasEntry(section, "CurrencyPosFormat"))
            {
                string value = (string)ini.GetValue(section, "CurrencyPosFormat");
                int index = Array.IndexOf<String>(CurrencyPositivePatterns, value);
                if (index != -1)
                    df.CurrencyPosFormat = index;
            }
        }

        private void ParseCurrencyNegFormat(IniFile ini, string section, ref TextFileDataFormat df)
        {
            if (ini.HasEntry(section, "CurrencyNegFormat"))
            {
                string value = (string)ini.GetValue(section, "CurrencyNegFormat");
                int index = Array.IndexOf<String>(CurrencyNegativePatterns, value);
                if (index != -1)
                    df.CurrencyPosFormat = index;
            }
        }

        public static string GetSchemaDataType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return "Bit";

                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return "Integer";

                case TypeCode.Single:
                    return "Single";

                case TypeCode.Double:
                case TypeCode.Decimal:
                    return "Double";

                case TypeCode.DateTime:
                    return "DateTime";

                default:
                    return "Text";
            }
        }

        public static TextFileDataFormat GetCurrentTextDataFormat()
        {
            TextFileDataFormat df = new TextFileDataFormat();
            df.TextFormat = TextDataFormat.Delimited;
            df.Delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            //df.ColumnNameHeader = true;
            df.Encoding = Encoding.Default;
            df.MaxScanRows = 0;

            df.DateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            df.DecimalSymbol = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            df.NumberDigits = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
            df.CurrencyDecimalSymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            df.CurrencyThousandSymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator;
            df.CurrencyDigits = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits;
            df.CurrencyNegFormat = CultureInfo.CurrentCulture.NumberFormat.CurrencyNegativePattern;
            df.CurrencyPosFormat = CultureInfo.CurrentCulture.NumberFormat.CurrencyPositivePattern;
            df.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            df.EncapsulatorChar = '"';
            df.EscapeChar = '\0';
            df.VariableFields = false;
            return df;
        }

        public static void ExportTextDataFormat(IniFile ini, string section, TextFileDataFormat df)
        {
            ini.SetValue(section, "DateTimeFormat", df.DateTimeFormat);
            ini.SetValue(section, "DecimalSymbol", df.DecimalSymbol);
            ini.SetValue(section, "CurrencySymbol", df.CurrencySymbol);
            ini.SetValue(section, "CurrencyThousandSymbol", df.CurrencyThousandSymbol);
            ini.SetValue(section, "CurrencyDecimalSymbol", df.CurrencyDecimalSymbol);
            ini.SetValue(section, "NumberDigits", df.NumberDigits.ToString());
            ini.SetValue(section, "CurrencyDigits", df.CurrencyDigits.ToString());
            ini.SetValue(section, "NumberLeadingZeros", df.NumberLeadingZeros ? "True" : "False");
            ini.SetValue(section, "CurrencyNegFormat", CurrencyNegativePatterns[df.CurrencyNegFormat]);
            ini.SetValue(section, "CurrencyPosFormat", CurrencyPositivePatterns[df.CurrencyPosFormat]);
        }

        private DataTable CreateRowType(StreamReader reader, char delimiter, bool columnNameHeader, DataTable prototype)
        {
            string[] values = new string[MaxTextColumns];
            StringBuilder sb = new StringBuilder();
            string[] header = null;
            int maxcols = 0;
            CsvParser parser = new CsvParser(delimiter);
            while (!reader.EndOfStream)
            {
                maxcols = parser.Get(reader, values);
                if (maxcols > 0 && columnNameHeader)
                {
                    header = new string[maxcols];
                    Array.Copy(values, header, maxcols);
                    break;
                }
            }
            List<string> names = new List<string>();
            DataProviderHelper helper = new DataProviderHelper();
            DataTable dt = RowType.CreateSchemaTable();
            for (int k = 0; k < maxcols; k++)
            {
                DataRow dr = dt.NewRow();
                if (prototype != null && k < prototype.Rows.Count)
                    dr.ItemArray = prototype.Rows[k].ItemArray;
                else
                {
                    dr["ColumnOrdinal"] = k;
                    if (columnNameHeader && header != null && k < header.Length)
                    {
                        String name = helper.NativeFormatIdentifier(header[k]);
                        if (String.IsNullOrEmpty(name))
                            name = "Col";
                        dr["ColumnName"] = Util.CreateUniqueName(names, name);
                    }
                    else
                        dr["ColumnName"] = String.Format("Col{0}", k + 1);
                    dr["DataType"] = typeof(System.String);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
       
        protected override Resultset CreateResultset(Stream stream, string fileName, QueryContext queryContext)
        {
            DataTable dt = null;
            DataTable prototype = null;
            TextFileDataFormat format = queryContext.GetTextFileDataFormat();
            if (fileName != "")
            {
                dt = CreateRowType(fileName, ref format);
                if (format.VariableFields)
                {
                    prototype = dt;
                    dt = null;
                }
            }
            StreamReader reader = new StreamReader(stream, format.Encoding);
            if (dt == null || dt.Rows.Count == 0)
            {
                switch (format.TextFormat)
                {
                    case TextDataFormat.Delimited:
                        dt = CreateRowType(reader, format.Delimiter[0], format.ColumnNameHeader, prototype);
                        break;

                    case TextDataFormat.TabDelimited:
                        dt = CreateRowType(reader, '\t', format.ColumnNameHeader, prototype);
                        break;

                    default:
                        throw new ArgumentException("Delimiter");
                }
                stream.Seek(0, SeekOrigin.Begin);
                reader = new StreamReader(stream, format.Encoding);
            }
            ProcessingContextBase context;
            if (format.SequentialProcessing || format.EscapeChar != '\0')
            {
                if (format.TextFormat == TextDataFormat.FixedLength)
                    context = new ProcessingContext(reader, format.Width, format.NullValue, format.ColumnNameHeader,
                        GetNumberFormatInfo(format), GetDateTimeFormatInfo(format));
                else
                    context = new ProcessingContext(reader, format.Delimiter[0], format.NullValue, format.ColumnNameHeader,
                        format.EncapsulatorChar, format.EscapeChar, GetNumberFormatInfo(format), GetDateTimeFormatInfo(format));
            }
            else
            {
                if (format.TextFormat == TextDataFormat.FixedLength)
                    context = new ParallelProcessingContext(reader, format.Width, format.NullValue, format.ColumnNameHeader,
                        GetNumberFormatInfo(format), GetDateTimeFormatInfo(format));
                else
                    context = new ParallelProcessingContext(reader, format.Delimiter[0], format.NullValue, format.ColumnNameHeader,
                        format.EncapsulatorChar, format.EscapeChar, GetNumberFormatInfo(format), GetDateTimeFormatInfo(format));
            }
            context.RecordLimit = queryContext.LimitInputQuery;
            return new Resultset(new RowType(dt), context);
        }

        protected abstract class ProcessingContextBase : DemandProcessingContext
        {
            protected StreamReader m_reader;
            protected TextParser m_parser;
            protected bool m_columnNameHeader;
            protected NumberFormatInfo m_numberFormat;
            protected DateTimeFormatInfo m_dateTimeFormat;
            protected String m_nullValue;


            public ProcessingContextBase(StreamReader reader, char delimiter, String nullValue, bool columnNameHeader, char encapsulatorChar, char escapeChar,
                NumberFormatInfo nft, DateTimeFormatInfo dft)   
              : base(null)
            {
                m_reader = reader;
                m_columnNameHeader = columnNameHeader;
                m_numberFormat = nft;
                m_dateTimeFormat = dft;
                m_nullValue = nullValue;
                m_parser = new CsvParser(delimiter, TextDataAccessor.CommentChar, encapsulatorChar, escapeChar);                                
            }

            public ProcessingContextBase(StreamReader reader, int[] length, String nullValue, bool columnNameHeader,
                NumberFormatInfo nft, DateTimeFormatInfo dft)
                : base(null)
            {
                m_reader = reader;
                m_columnNameHeader = columnNameHeader;
                m_numberFormat = nft;
                m_dateTimeFormat = dft;
                m_parser = new FLVParser(length, TextDataAccessor.CommentChar);
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    if (m_reader != null)
                    {
                        m_reader.Close();
                        m_reader = null;
                    }
                }
            }

            protected object ConvertValue(string value, Type type, int line, int col)
            {
                if (String.IsNullOrEmpty(value) || value == m_nullValue)
                    return DBNull.Value;

                try
                {
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Boolean: /* Bit */
                            if (value == "0" || value == "false" || value == "False")
                                return false;
                            else if (value == "1" || value == "true" || value == "True")
                                return true;
                            else
                                throw new InvalidCastException();

                        case TypeCode.Byte:    /* Byte */
                            return Convert.ToByte(value);

                        case TypeCode.Int16:   /* Short */ 
                            return Convert.ToInt16(value);

                        case TypeCode.Int32:   /* Integer */
                            return Convert.ToInt32(value);

                        case TypeCode.Int64:   /* Long */
                            return Convert.ToInt64(value);

                        case TypeCode.Decimal:  /* Currency */
                            return Decimal.Parse(value, m_numberFormat);

                        case TypeCode.Single:   /* Single */
                            return Single.Parse(value, m_numberFormat);

                        case TypeCode.Double:   /* Double, Float */
                            return Double.Parse(value, m_numberFormat);

                        case TypeCode.DateTime: /* DateTime, Date */
                            return DateTime.Parse(value, m_dateTimeFormat);

                        case TypeCode.String:   /* Text, Memo */
                            return value;

                        default:
                            throw new InvalidCastException();
                    }
                }
                catch (Exception)
                {
                    throw new ESQLException(Properties.Resources.ValueConvertException,
                        Util.Substring(value, 0, 10), type.Name, line, col);
                }
            }
        }

        protected class ProcessingContext: ProcessingContextBase
        {
            private string[] values = null;
            private TextParser.ColRow[] pos;

            public ProcessingContext(StreamReader reader, char delimiter, String nullValue,
                bool columnNameHeader, char encapsulatorChar, char escapeChar, NumberFormatInfo nft, DateTimeFormatInfo dft) :
                base(reader, delimiter, nullValue, columnNameHeader, encapsulatorChar, escapeChar, nft, dft)
            {
            }

            public ProcessingContext(StreamReader reader, int[] length, String nullValue, bool columnNameHeader,
                NumberFormatInfo nft, DateTimeFormatInfo dft) :
                    base(reader, length, nullValue, columnNameHeader, nft, dft)
            {
            }                

            public override bool ProcessNextPiece(Resultset rs)
            {
                CheckDisposed();
                while (!m_reader.EndOfStream)
                {
                    if (values == null)
                    {
                        values = new string[rs.RowType.Fields.Length];
                        pos = new TextParser.ColRow[values.Length];
                        if (m_columnNameHeader)
                            while (!m_reader.EndOfStream && m_parser.Get(m_reader, values, pos) == -1)
                                ;
                    }
                    else
                        if (m_parser.Get(m_reader, values, pos) > -1)
                        {
                            Row row = rs.NewRow();
                            for (int k = 0; k < rs.RowType.Fields.Length; k++)
                                if (values[k] != null)
                                    row.SetValue(k, ConvertValue(values[k].TrimEnd(),
                                        rs.RowType.Fields[k].DataType, pos[k].line, pos[k].col));
                            rs.Enqueue(row);
                            return true;
                        }
                }
                m_reader.Close();
                m_reader = null;
                return false;
            }
        }

        protected class ParallelProcessingContext : ProcessingContextBase
        {
            private int m_lineindex = -1;
            private string[] m_lines = new string[300];

            public ParallelProcessingContext(StreamReader reader, char delimiter, String nullValue,
                bool columnNameHeader, char encapsulatorChar, char escapeChar, NumberFormatInfo nft, DateTimeFormatInfo dft) :
                base(reader, delimiter, nullValue, columnNameHeader, encapsulatorChar, escapeChar, nft, dft)
            {
            }

            public ParallelProcessingContext(StreamReader reader, int[] length, String nullValue, bool columnNameHeader,
                NumberFormatInfo nft, DateTimeFormatInfo dft) :
                    base(reader, length, nullValue, columnNameHeader, nft, dft)
            {
            }                

            public override bool ProcessNextPiece(Resultset rs)
            {
                CheckDisposed();
                if (m_lineindex == -1)
                {
                    if (m_columnNameHeader)
                    {
                        string[] values = new string[rs.RowType.Fields.Length];
                        TextParser.ColRow[] pos = new TextParser.ColRow[values.Length];
                        while (!m_reader.EndOfStream && m_parser.Get(m_reader, values, pos) == -1)
                            ;
                    }
                    m_lineindex = m_parser.LineIndex;
                }
                int count = 0;
                for (int s = 0; s < m_lines.Length; s++,count++)
                {
                    if (m_reader.EndOfStream)
                    {
                        m_reader.Close();
                        m_reader = null;
                        break;
                    }
                    m_lines[s] = m_reader.ReadLine();
                }
                Iterator.For(0, count, s =>
                {
                    string[] values = new string[rs.RowType.Fields.Length];
                    TextParser.ColRow[] pos = new TextParser.ColRow[values.Length];
                    if (m_parser.Get(m_lines[s], values, m_lineindex + s, pos) > -1)
                    {
                        Row row = rs.NewRow();
                        for (int k = 0; k < rs.RowType.Fields.Length; k++)
                            if (values[k] != null)
                                row.SetValue(k, ConvertValue(values[k].TrimEnd(),
                                    rs.RowType.Fields[k].DataType, pos[k].line, pos[k].col));
                        rs.Enqueue(row);
                    }
                });
                m_lineindex += rs.Count;
                return m_reader != null;
            }

        }

        public static object OpenFile(QueryNode node, QueryContext context, string fileName)
        {
            FlatFileAccessor fileAccessor = new FlatFileAccessor(fileName);
            TextDataAccessor accessor = new TextDataAccessor();
            accessor.ChildNodes.Add(fileAccessor);
            return accessor.Get(context, null);
        }
    }
}
