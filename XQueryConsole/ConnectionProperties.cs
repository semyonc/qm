//        Copyright (c) 2012, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MongoDB.Driver;

namespace XQueryConsole
{
    class MongoServerAddressConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(String))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is String)
            {
                String[] servers = ((String)value).Split(',');
                MongoServerAddress[] addr = new MongoServerAddress[servers.Length];
                for (int k = 0; k < servers.Length; k++)
                    addr[k] = MongoServerAddress.Parse(servers[k]);
                return addr;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                if (value == null)
                    return "";
                StringBuilder sb = new StringBuilder();
                IEnumerable<MongoServerAddress> servers = 
                    (IEnumerable<MongoServerAddress>)value;
                foreach (MongoServerAddress addr in servers)
                {
                    if (sb.Length > 0)
                        sb.Append(',');
                    if (addr.Port != 0x6989)
                        sb.Append(addr.ToString());
                    else 
                        sb.Append(addr.Host);
                }
                return sb.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }         
    }
   
    class ConnectionProperties : ICustomTypeDescriptor
    {

        static ConnectionProperties()
        {
            TypeDescriptor.AddAttributes(typeof(IEnumerable<MongoServerAddress>),
                new Attribute[] { new TypeConverterAttribute(typeof(MongoServerAddressConverter)) });
            TypeDescriptor.AddAttributes(typeof(IEnumerable<MongoServerAddress>),
                new Attribute[] { new DisplayNameAttribute("Server") });
            TypeDescriptor.AddAttributes(typeof(SafeMode),
                new Attribute[] { new TypeConverterAttribute(typeof(ExpandableObjectConverter)), 
                    new DefaultValueAttribute(new SafeMode(false)), new ReadOnlyAttribute(true) });            
        }

        private Connection _connection;
        private DbConnectionStringBuilder _connectionStringBuilder;

        public ConnectionProperties(Connection conn, string connectionString)
        {
            _connection = conn;
            try
            {
                _connectionStringBuilder = conn.CreateConnectionStringBuilder();
                _connectionStringBuilder.ConnectionString = connectionString;
            }
            catch
            {
                _connectionStringBuilder = null;
            }
        }

        public bool IsGeneralProperty(PropertyDescriptor pd)
        {
            return pd.ComponentType == typeof(Connection);
        }

        public bool IsConnectionBuilderProperty(PropertyDescriptor pd)
        {
            return pd.ComponentType == _connectionStringBuilder.GetType();
        }

        public void Update()
        {
            if (_connectionStringBuilder != null)
                _connection.ConnectionString = _connectionStringBuilder.ConnectionString;
        }

        public bool IsValid
        {
            get
            {
                return _connectionStringBuilder != null;
            }
        }

        private PropertyDescriptor CreateMongoPropertyDescriptor(Type componentType, PropertyDescriptor pd)        
        {
            if (pd.Name == "ConnectionString" || pd.Name == "Server")
                return null;
            if (pd.Name == "ConnectionMode")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(ConnectionMode.Direct));
            if (pd.Name == "ConnectTimeout")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(MongoDefaults.ConnectTimeout));
            if (pd.Name == "GuidRepresentation")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(MongoDefaults.GuidRepresentation));
            if (pd.Name == "IPv6")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(false));
            if (pd.Name == "MaxConnectionIdleTime")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(MongoDefaults.MaxConnectionIdleTime));
            if (pd.Name == "MaxConnectionLifeTime")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(MongoDefaults.MaxConnectionLifeTime));
            if (pd.Name == "MaxConnectionPoolSize")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(MongoDefaults.MaxConnectionPoolSize));
            if (pd.Name == "MinConnectionPoolSize")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(MongoDefaults.MinConnectionPoolSize));
            if (pd.Name == "SlaveOk")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(false));
            if (pd.Name == "SocketTimeout")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(MongoDefaults.SocketTimeout));
            if (pd.Name == "WaitQueueMultiple")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(MongoDefaults.WaitQueueMultiple));
            if (pd.Name == "WaitQueueSize")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(MongoDefaults.WaitQueueSize));
            if (pd.Name == "WaitQueueTimeout")
                return TypeDescriptor.CreateProperty(componentType, pd, new DefaultValueAttribute(MongoDefaults.WaitQueueTimeout));
            return pd;
        }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            List<PropertyDescriptor> res = new List<PropertyDescriptor>();
            PropertyDescriptorCollection connectionProps = TypeDescriptor.GetProperties(_connection, attributes);            
            for (int k = 0; k < connectionProps.Count; k++)
                res.Add(connectionProps[k]);
            if (_connectionStringBuilder != null)
            {
                bool isMongoBuilder = _connectionStringBuilder.GetType().IsAssignableFrom(typeof(MongoConnectionStringBuilder));
                PropertyDescriptorCollection connectionStringProps =
                    TypeDescriptor.GetProperties(_connectionStringBuilder, attributes, isMongoBuilder);
                for (int k = 0; k < connectionStringProps.Count; k++)
                {
                    PropertyDescriptor pd = connectionStringProps[k];
                    if (isMongoBuilder)
                    {
                        pd = CreateMongoPropertyDescriptor(typeof(MongoConnectionStringBuilder), pd);
                        if (pd == null)
                            continue;
                    }
                    res.Add(pd);
                }
            }
            return new PropertyDescriptorCollection(res.ToArray());
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(new Attribute[0]);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {     
            if (pd != null)
            {
                if (pd.ComponentType == _connection.GetType())
                    return _connection;
                if (_connectionStringBuilder != null && 
                    pd.ComponentType.IsAssignableFrom(_connectionStringBuilder.GetType()))
                    return _connectionStringBuilder;              
            }
            return this;
        }

        #endregion
        
    }
}
