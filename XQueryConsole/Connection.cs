using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;

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
            DbProviderFactory factory = DbProviderFactories.GetFactory(InvariantName);
            DbConnectionStringBuilder sb = factory.CreateConnectionStringBuilder();
            sb.ConnectionString = ConnectionString;
            return (String)sb["Data Source"];
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
    }
}
