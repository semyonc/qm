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

namespace DataEngine.Export
{
    public enum ExportTarget
    {
        Xml,
        Json,
        AdoNet,
        Csv,
        TabDelimited,
        FixedLength,
        Xls,
        ZJson,
        AdoProvider
    }

    public class CommandLineParser
    {
        public CommandLineParser()
        {
        }

        public bool Parse(string[] args)
        {
            int apos = 0;
            if (apos < args.Length)
                QueryFileName = args[apos++];
            if (apos < args.Length)
            {
                string key = args[apos++];
                if (key.Equals("-xml"))
                    Target = ExportTarget.Xml;
                else if (key.Equals("-ado"))
                    Target = ExportTarget.AdoNet;
                else if (key.Equals("-xls"))
                    Target = ExportTarget.Xls;
                else if (key.Equals("-csv"))
                    Target = ExportTarget.Csv;
                else if (key.Equals("-txt"))
                    Target = ExportTarget.FixedLength;
                else if (key.Equals("-tab"))
                    Target = ExportTarget.TabDelimited;
                else if (key.Equals("-db"))
                    Target = ExportTarget.AdoProvider;
                else if (key.Equals("-json"))
                    Target = ExportTarget.Json;
                else if (key.Equals("-zjson"))
                    Target = ExportTarget.ZJson;
                else
                    return false;
            }
            if (Target == ExportTarget.AdoProvider)
            {
                if (apos < args.Length)
                {
                    string name = args[apos++];
                    string[] part = name.Split(':');
                    if (part.Length == 1)
                        TableName = name;
                    else
                    {
                        DbPrefix = part[0];
                        TableName = part[1];
                    }
                    if (apos < args.Length)
                    {
                        string key = args[apos];
                        if (key.Equals("-clean"))
                            Clean = true;
                        else
                            return false;
                    }
                    return true;
                }
            }
            else
            {
                if (apos < args.Length)
                {
                    OutputFileName = args[apos];
                    return true;
                }
            }
            return false;
        }

        public string QueryFileName { get; private set; }

        public ExportTarget Target { get; private set; }

        public string OutputFileName { get; private set; }

        public string DbPrefix { get; private set; }

        public string TableName { get; private set; }

        public bool Clean { get; private set; }
    }
}
