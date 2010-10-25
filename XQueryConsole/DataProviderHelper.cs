/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2009  Semyon A. Chertkov (semyonc@gmail.com)

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data;
using System.Data.Common;

using System.IO;
using System.Reflection;
using System.Globalization;

namespace DataEngine
{
    public class DataProviderHelper
    {
        private class ProviderInfo
        {
            public string ProviderInvariantName;
            public string DataSourceProductName;
            public string IdentifierPattern;
            public IdentifierCase IdentifierCase;
            public bool OrderByColumnsInSelect;
            public string ParameterMarkerFormat;
            public string QuotedIdentifierPattern;
            public IdentifierCase QuotedIdentifierCase;
            public string StringLiteralPattern;
            public SupportedJoinOperators SupportedJoinOperators;
            public string DateFormat;
            public string RowCountQuery;

            public char Qualifer;
            public char StringSeparator;
            public char LeftQuote;
            public char RightQuote;

            public int UpdateBatchSize;
            public bool NormalizeColumnName;
            
            public Dictionary<String, int> keywords;

            public ProviderInfo()
            {
                keywords = new Dictionary<string, int>();
                UpdateBatchSize = 1;
            }
        }

        private static Dictionary<String, ProviderInfo> _cached_info;
        
        private ProviderInfo _providerInfo;

        static DataProviderHelper()
        {
            _cached_info = new Dictionary<string, ProviderInfo>();
        }

        public DataProviderHelper()
        {
            _providerInfo = new ProviderInfo();
            SetDefaultProperties();
            List<string> reservedWords = new List<string>();
        }

        public DataProviderHelper(string providerInvariantName, string connectionString)
        {
            lock (_cached_info)
                if (!_cached_info.TryGetValue(providerInvariantName, out _providerInfo))
                {
                    _providerInfo = new ProviderInfo();

                    DbProviderFactory f = DbProviderFactories.GetFactory(providerInvariantName);
                    using (DbConnection connection = f.CreateConnection())
                    {
                        connection.ConnectionString = connectionString;
                        connection.Open();

                        DataTable dt = connection.GetSchema("DataSourceInformation");
                        DataRow r = dt.Rows[0];

                        _providerInfo.ProviderInvariantName = providerInvariantName;
                        _providerInfo.DataSourceProductName = (string)r["DataSourceProductName"];
                        _providerInfo.IdentifierCase = (IdentifierCase)Convert.ToInt32(r["IdentifierCase"]);
                        _providerInfo.IdentifierPattern = (String)r["IdentifierPattern"];
                        _providerInfo.OrderByColumnsInSelect = (bool)r["OrderByColumnsInSelect"];
                        _providerInfo.ParameterMarkerFormat = (string)r["ParameterMarkerFormat"];
                        _providerInfo.QuotedIdentifierCase = (IdentifierCase)Convert.ToInt32(r["QuotedIdentifierCase"]);
                        _providerInfo.QuotedIdentifierPattern = (string)r["QuotedIdentifierPattern"];
                        if (!r.IsNull("StringLiteralPattern"))
                            _providerInfo.StringLiteralPattern = (string)r["StringLiteralPattern"];
                        if (!r.IsNull("SupportedJoinOperators"))
                            _providerInfo.SupportedJoinOperators = (SupportedJoinOperators)Convert.ToInt32(r["SupportedJoinOperators"]);

                        dt = connection.GetSchema("ReservedWords");
                        foreach (DataRow r0 in dt.Rows)
                        {
                            String kw = ((string)r0[0]).ToUpperInvariant();
                            if (!_providerInfo.keywords.ContainsKey(kw)) // MySQL Bug
                                _providerInfo.keywords.Add(kw, 0);
                        }

                        _cached_info.Add(providerInvariantName, _providerInfo);
                        connection.Close();
                    }
                }
            ReadProperties();
        }
        

        public static Stream GetConfigurationStream()
        {
            string configPath = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), "SQLX.Config.xml");
            FileInfo fi = new FileInfo(configPath);
            if (fi.Exists)
                return new FileStream(configPath, FileMode.Open, FileAccess.Read);
            else
            {
                return typeof(DataProviderHelper).Assembly
                    .GetManifestResourceStream("XQueryConsole.SQLX.Config.xml");
            }
        }

        private void ReadProperties()
        {
            XmlDocument xmldoc = new XmlDocument();
            Stream stream = GetConfigurationStream();
            xmldoc.Load(stream);
            stream.Close();
            XmlNode node = xmldoc.SelectSingleNode(
                String.Format("//add[@invariant='{0}']/providerHelper", _providerInfo.ProviderInvariantName));
            if (node != null)
            {
                XmlNode item = node.SelectSingleNode("parameterMarkerFormat");
                if (item != null)
                    _providerInfo.ParameterMarkerFormat = item.InnerText;

                item = node.SelectSingleNode("qualifer");
                if (item != null)
                    _providerInfo.Qualifer = item.InnerText[0];

                item = node.SelectSingleNode("stringSeparator");
                if (item != null)
                    _providerInfo.StringSeparator = item.InnerText[0];

                item = node.SelectSingleNode("leftQuote");
                if (item != null)
                    _providerInfo.LeftQuote = item.InnerText[0];

                item = node.SelectSingleNode("rightQuote");
                if (item != null)
                    _providerInfo.RightQuote = item.InnerText[0];

                item = node.SelectSingleNode("dateFormat");
                if (item != null)
                    _providerInfo.DateFormat = item.InnerText;

                item = node.SelectSingleNode("rowCountQuery");
                if (item != null)
                    _providerInfo.RowCountQuery = item.InnerText;

                item = node.SelectSingleNode("updateBatchSize");
                if (item != null)
                    _providerInfo.UpdateBatchSize = Convert.ToInt32(item.InnerText);

                item = node.SelectSingleNode("normalizeColumnName");
                if (item != null)
                {
                    if (item.InnerXml == "1" || item.InnerXml == "T" || item.InnerXml == "True" ||
                        item.InnerXml == "t" || item.InnerXml == "true")
                        _providerInfo.NormalizeColumnName = true;
                }
            }
            else
                SetDefaultProperties();
        }

        private void SetDefaultProperties()
        {
            _providerInfo.ParameterMarkerFormat = "{0}";
            _providerInfo.Qualifer = '.';
            _providerInfo.StringSeparator = '\'';
            _providerInfo.LeftQuote = '\"';
            _providerInfo.RightQuote = '\"';
            _providerInfo.IdentifierCase = IdentifierCase.Sensitive;            
        }

        public bool RequiresQuoting(string identifierPart)
        {
            if (String.IsNullOrEmpty(identifierPart))
                return false;

            if (RequiresQuotingDefault(identifierPart, 
                _providerInfo.IdentifierCase == IdentifierCase.Sensitive))
                return true;

            if (!String.IsNullOrEmpty(_providerInfo.IdentifierPattern) &&
                !Regex.IsMatch(identifierPart, _providerInfo.IdentifierPattern))
                return true;

            if (_providerInfo.keywords.ContainsKey(identifierPart.ToUpperInvariant()))
                return true;

            return false;
        }

        public bool IsKeyword(string identifier)
        {
            if (_providerInfo.keywords != null &&
                _providerInfo.keywords.ContainsKey(identifier.ToUpperInvariant()))
                return true;
            else
                return false;
        }

        private static bool IsValidIdentChar(char c)
        {
            return c == '_' || c == '@';
        }

        private static bool IsIdentifierFirstChar(char c, bool caseSensitive)
        {
            if (caseSensitive)
                return Char.IsLetter(c) || IsValidIdentChar(c);
            else
                return (Char.IsLetter(c) && Char.IsUpper(c)) || IsValidIdentChar(c);
        }

        private static bool IsIdentifierChar(char c, bool caseSensitive)
        {
            if (caseSensitive)
                return Char.IsLetterOrDigit(c) || IsValidIdentChar(c);
            else
                return (Char.IsLetter(c) && Char.IsUpper(c)) || Char.IsDigit(c) || IsValidIdentChar(c);
        }

        public static bool RequiresQuotingDefault(string identifierPart, bool caseSensitive)
        {
            if (identifierPart.Length == 0)
            {
                return false;
            }

            if ( !IsIdentifierFirstChar(identifierPart[0], caseSensitive))
            {
                return true;
            }

            for (int i = 1; i < identifierPart.Length; i++)
            {
                if (!IsIdentifierChar(identifierPart[i], caseSensitive))
                    return true;
            }
            
            return false;
        }

        public String FormatParameter(String parameterName)
        {
            return String.Format(_providerInfo.ParameterMarkerFormat, parameterName);
        }

        public bool NativeRequiresQuoting(string identifierPart)
        {
            if (identifierPart.Length == 0)
            {
                return false;
            }

            if (_providerInfo.keywords.ContainsKey(identifierPart.ToUpperInvariant()))
                return true;

            if (!(Char.IsLetter(identifierPart[0]) || IsValidIdentChar(identifierPart[0])))
            {
                return true;
            }

            for (int i = 1; i < identifierPart.Length; i++)
            {
                if (!(Char.IsLetter(identifierPart[i]) || IsValidIdentChar(identifierPart[i]) ||
                      Char.IsDigit(identifierPart[i])))
                    return true;
            }

            return false;
        }

        public String NativeFormatIdentifier(String identifier)
        {
            if (NativeRequiresQuoting(identifier))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                sb.Append(identifier.Replace("\"", "\"\""));
                sb.Append("\"");
                return sb.ToString();
            }
            else
                if (IdentifierCase == IdentifierCase.Insensitive && !RequiresQuoting(identifier))
                    return identifier.ToLower();
                else
                    return identifier;
        }

        public String FormatIdentifier(String[] identifier)
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < identifier.Length; k++)
            {
                if (k > 0)
                    sb.Append(".");
                sb.Append(FormatIdentifier(identifier[k]));
            }
            return sb.ToString();
        }

        public String FormatIdentifier(String identifier)
        {            
            if (Util.IsQuotedName(identifier))
                identifier = Util.UnquoteName(identifier);
            else
            {
                if (_providerInfo.IdentifierCase == IdentifierCase.Insensitive)
                    identifier = identifier.ToUpperInvariant();
            }
            if (RequiresQuoting(identifier))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(LeftQuote);
                sb.Append(identifier);
                sb.Append(RightQuote);
                return sb.ToString();
            }
            else
                return identifier;
        }

        public String NormalizeIdentifier(List<string> fieldNames, String identifier)
        {
            if (NormalizeColumnName)
                return FormatIdentifier(Util.CreateUniqueName(fieldNames, 
                    identifier.ToUpper().Replace(" ", "_")));
            else
                return FormatIdentifier(
                    Util.CreateUniqueName(fieldNames, identifier));
        }

        //public String FormatIdentifier(String identifier)
        //{
        //    if (identifier.StartsWith("\"") && identifier.EndsWith("\""))
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(LeftQuote);
        //        sb.Append(identifier.Substring(1, identifier.Length -2).Replace("\"\"", "\""));
        //        sb.Append(RightQuote);
        //        return sb.ToString();
        //    }
        //    else
        //    {
        //        if (_providerInfo.IdentifierCase == IdentifierCase.Insensitive)
        //            return identifier.ToUpperInvariant();
        //        else
        //            return identifier;
        //    }
        //}

        public String FormatLiteral(String literal)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(StringSeparator);
            sb.Append(literal.Replace(new String(new char[] { StringSeparator }),
                new String(new char[] { StringSeparator, StringSeparator })));
            sb.Append(StringSeparator);
            return sb.ToString();
        }

        public String FormatDateTime(DateTime dateTime)
        {
            string sdate = dateTime.ToString("u");
            sdate = sdate.Substring(0, sdate.Length - 1);
            if (String.IsNullOrEmpty(DateFormat))
                return FormatLiteral(sdate);
            else
                return String.Format(DateFormat, sdate);
        }

        public String[] SplitIdentifier(String identifier)
        {
            List<String> arrIdentifier = new List<String>();
            
            if (identifier != null)
            {
                int arrIndex = 0;
                int startIndex = 0;
                int endIndex = 0;
                char quote = '\0';
                while (endIndex < identifier.Length)
                {
                    if (identifier[endIndex] == LeftQuote && quote == '\0')
                    {
                        // We entered a quoted identifier part using '"'
                        quote = RightQuote;
                    }
                    else if (identifier[endIndex] == RightQuote && quote == RightQuote)
                    {
                        if (endIndex < identifier.Length - 1 &&
                            identifier[endIndex + 1] == RightQuote)
                        {
                            // We encountered an embedded quote in a quoted
                            // identifier part; skip it
                            endIndex++;
                        }
                        else
                        {
                            // We left a quoted identifier part using '"'
                            quote = '\0';
                        }
                    }
                    else if (identifier[endIndex] == Qualifer && quote == '\0')
                    {
                        arrIdentifier.Add(identifier.Substring(
                            startIndex, endIndex - startIndex));
                        arrIndex++;
                        startIndex = endIndex + 1;
                    }
                    endIndex++;
                }
                if (identifier.Length > 0)
                    arrIdentifier.Add(identifier.Substring(startIndex));
            }

            return arrIdentifier.ToArray();
        }

        public String GetEstimateRowCountQuery(string tableName, int MaxRows)
        {
            return String.Format(_providerInfo.RowCountQuery, MaxRows, tableName);
        }

        public String ProviderInvariantName 
        { 
            get 
            { 
                return _providerInfo.ProviderInvariantName; 
            } 
        }
        
        public Char Qualifer 
        { 
            get 
            { 
                return _providerInfo.Qualifer; 
            } 
        }
        
        public Char StringSeparator 
        { 
            get 
            { 
                return _providerInfo.StringSeparator; 
            } 
        }
        
        public Char LeftQuote 
        { 
            get 
            { 
                return _providerInfo.LeftQuote; 
            } 
        }
        
        public Char RightQuote 
        { 
            get 
            { 
                return _providerInfo.RightQuote; 
            } 
        }

        public Boolean OrderByColumnsInSelect
        {
            get
            {
                return _providerInfo.OrderByColumnsInSelect;
            }
        }

        public IdentifierCase IdentifierCase
        {
            get
            {
                return _providerInfo.IdentifierCase;
            }
        }

        public String DateFormat
        {
            get
            {
                return _providerInfo.DateFormat;
            }
        }

        public int UpdateBatchSize
        {
            get
            {
                return _providerInfo.UpdateBatchSize;
            }
        }

        public bool NormalizeColumnName
        {
            get
            {
                return _providerInfo.NormalizeColumnName;
            }
        }

        public bool Smart
        {
            get
            {
                return !String.IsNullOrEmpty(_providerInfo.RowCountQuery);
            }
        }
    }
}
