//        Copyright (c) 2010-2012, Semyon A. Chertkov (semyonc@gmail.com)
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Data.Remote;
using Data.Remote.Proxy;

using MongoDB.Driver;

using DataEngine;

namespace XQueryConsole
{    
    public class Connection: INotifyPropertyChanged
    {
        private static ImageSource _img_ds =
            new BitmapImage(new Uri("pack://application:,,,/XQueryConsole;component/Images/DataSource.png"));
        private static ImageSource _img_default_ds =
            new BitmapImage(new Uri("pack://application:,,,/XQueryConsole;component/Images/DefaultDataSource.png"));

        private Guid _uid;
        private string _prefix;
        private string _invariantName;
        private bool _default;
        private string _connectionString;
        private AccessorType _accessorType;
        private bool _x86Connection;
                
        public Connection()
        {
            _uid = Guid.NewGuid();
            Prefix = String.Empty;
            InvariantName = String.Empty;
            ConnectionString = String.Empty;
            _accessorType = AccessorType.DataProvider;
        }

        public Connection(Connection src)
        {
            _uid = src._uid;
            Prefix = src.Prefix;
            InvariantName = src.InvariantName;
            ConnectionString = src.ConnectionString;
            Default = src.Default;
            _accessorType = src.Accessor;
        }

        [Category("(General)")]
        public string Prefix 
        {
            get
            {
                return _prefix;
            }
            set
            {
                _prefix = value;
                OnPropertyChanged("Prefix");
                OnPropertyChanged("DisplayName");
            }
        }

        [Category("(General)")]
        [DisplayName("Provider Name")]
        [ReadOnly(true)]
        public string InvariantName 
        {
            get
            {
                return _invariantName;
            }
            set
            {
                _invariantName = value;
                OnPropertyChanged("InvariantName");
            }
        }

        [Category("(General)")]
        [DisplayName("Use X86 Proxy Connection")]
        public bool X86Connection
        {
            get
            {
                return _x86Connection;
            }
            set
            {
                _x86Connection = value;
                OnPropertyChanged("X86Connection");
            }
        }

        [Browsable(false)]
        public Guid UUID {
            get
            {
                return _uid;

            }
            set
            {
                _uid = value;
            }
        }

        [Browsable(false)]
        public string DisplayName
        {
            get
            {
                return ToString();
            }
        }

        [Browsable(false)]
        public ImageSource StatusImg
        {
            get
            {
                return Default ?
                    _img_default_ds : _img_ds;
            }
        }

        [Browsable(false)]
        public string ConnectionString 
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
                OnPropertyChanged("ConnectionString");
                OnPropertyChanged("DisplayName");
            }
        }

        [Browsable(false)]
        public AccessorType Accessor
        {
            get
            {
                return _accessorType;
            }
            set
            {
                _accessorType = value;
            }
        }

        [Category("(General)")]
        [DisplayName("Is Default")]
        [DefaultValue(false)]
        public bool Default 
        {
            get
            {
                return _default;
            }
            set
            {
                _default = value;
                OnPropertyChanged("Default");
                OnPropertyChanged("StatusImg");
            }
        }

        public void AssignTo(Connection conn)
        {
            conn.Prefix = Prefix;
            conn.InvariantName = InvariantName;
            conn.ConnectionString = ConnectionString;
            conn.Default = Default;
        }

        public override string ToString()
        {
            return String.Format("{1}:{0}", 
                GetConnectionName(), Prefix);
        }

        private String GetDataProviderConnectionName()
        {
            if (RemoteDbProviderFactories.Isx64() &&
                (_x86Connection || InvariantName == "System.Data.OleDb"))
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
                object dsName;
                if (csb.TryGetValue("Data Source", out dsName))
                    return dsName.ToString();
                if (csb.TryGetValue("DataSource", out dsName))
                    return dsName.ToString();
                if (csb.TryGetValue("Dsn", out dsName))
                    return dsName.ToString();
                return "";
            }
        }

        private String GetMongoDbConnectionName()
        {
            MongoConnectionStringBuilder csb = new MongoConnectionStringBuilder();
            csb.ConnectionString = ConnectionString;
            return csb.DatabaseName;
        }

        private String GetConnectionName()
        {
            switch (_accessorType)
            {
                case AccessorType.DataProvider:
                    return GetDataProviderConnectionName();
                case AccessorType.MongoDb:
                    return GetMongoDbConnectionName();
                default:
                    return "";
            }
        }

        private void TestDataProviderConnection()
        {
            if (RemoteDbProviderFactories.Isx64() &&
                (_x86Connection || InvariantName == "System.Data.OleDb"))
            {
                RemoteDbProviderFactory f = RemoteDbProviderFactories.GetFactory(InvariantName);
                using (DbConnection dbcon = f.CreateConnection())
                {
                    dbcon.ConnectionString = ConnectionString;
                    dbcon.Open();
                    dbcon.Close();
                }
            }
            else
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(InvariantName);
                using (DbConnection dbcon = factory.CreateConnection())
                {
                    dbcon.ConnectionString = ConnectionString;
                    dbcon.Open();
                    dbcon.Close();
                }

            }
        }

        private void TestMongoDbConnection()
        {
            MongoConnectionStringBuilder csb = new MongoConnectionStringBuilder();
            csb.ConnectionString = ConnectionString;
            MongoServer mongo = MongoServer.Create(csb);
            mongo.Connect();
            if (!mongo.DatabaseExists(csb.DatabaseName))
                throw new Exception(String.Format("Database '{0}' is not exists", csb.DatabaseName));
            MongoDatabaseSettings settings = mongo.CreateDatabaseSettings(csb.DatabaseName);
            if (csb.Username != null)
                settings.Credentials = new MongoCredentials(csb.Username, csb.Password);
            MongoDatabase database = mongo.GetDatabase(settings);
            database.GetCollectionNames();
            mongo.Disconnect();
        }

        public void TestConnection()
        {
            switch (_accessorType)
            {
                case AccessorType.DataProvider:
                    TestDataProviderConnection();
                    break;
                
                case AccessorType.MongoDb:
                    TestMongoDbConnection();
                    break;
            }
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            switch (_accessorType)
            {
                case AccessorType.DataProvider:
                    {
                        DbProviderFactory factory = DbProviderFactories.GetFactory(InvariantName);
                        DbConnectionStringBuilder res = factory.CreateConnectionStringBuilder();
                        res.BrowsableConnectionString = false;
                        return res;
                    }

                case AccessorType.MongoDb:
                        return new MongoConnectionStringBuilder();
                
                default:
                    return null;                    
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class ConnectionContainer
    {
        [XmlArrayAttribute("Connections")]
        public Connection[] connections;

        public ConnectionContainer()
        {
            connections = new Connection[0];
        }

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
