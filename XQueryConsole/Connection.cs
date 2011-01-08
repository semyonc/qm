//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using System.Diagnostics;

using Data.Remote;
using Data.Remote.Proxy;
using DataEngine;


namespace XQueryConsole
{    
    public class Connection
    {
        public Connection()
        {
            Prefix = String.Empty;
            InvariantName = String.Empty;
            ConnectionString = String.Empty;            
        }

        public Connection(Connection src)
        {
            Prefix = src.Prefix;
            InvariantName = src.InvariantName;
            ConnectionString = src.ConnectionString;
            Default = src.Default;
        }

        public string Prefix { get; set; }

        [DisplayName("Provider Name")]
        public string InvariantName { get; set; }

        [DisplayName("Connection String")]
        //[System.ComponentModel.Editor(typeof(ConnectionStringEditor), 
        //    typeof(System.Drawing.Design.UITypeEditor))]
        public string ConnectionString { get; set; }

        public bool Default { get; set; }

        public override string ToString()
        {
            return String.Format("{1}:{0}", 
                GetConnectionName(), Prefix);
        }

        private String GetConnectionName()
        {
            if (RemoteDbProviderFactories.Isx64() &&
                (DataProviderHelper.HostADOProviders || InvariantName == "System.Data.OleDb"))
            {
                RemoteDbProviderFactory f = RemoteDbProviderFactories.GetFactory(InvariantName);
                ProxyConnectionStringBuilder csb = f.CreateConnectionStringBuilder();
                csb.ConnectionString = ConnectionString;
                return (String)csb["Data Source"];
            }
            else
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(InvariantName);
                DbConnectionStringBuilder csb = factory.CreateConnectionStringBuilder();
                csb.ConnectionString = ConnectionString;
                return (String)csb["Data Source"];
            }
        }
    }

    public class ConnectionContainer
    {
        [XmlArrayAttribute("Connections")]
        public Connection[] connections;

        public void Add(Connection conn)
        {            
            List<Connection> list; 
            if (connections == null)
                list = new List<Connection>();
            else
                list = new List<Connection>(connections);
            list.Add(conn);
            connections = list.ToArray();
        }

        public void Remove(Connection conn)
        {
            List<Connection> list = new List<Connection>(connections);
            list.Remove(conn);
            connections = list.ToArray();
        }

        public void Check()
        {
            foreach (Connection conn in connections)
                try
                {
                    conn.ToString();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    Remove(conn);
                }
        }
    }
}
