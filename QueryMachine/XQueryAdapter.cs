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
using System.Xml;

namespace DataEngine
{
    public abstract class XQueryAdapter: IDisposable
    {
        protected delegate XQueryAdapter XQueryAdapterFactoryDelegate (DatabaseDictionary dictionary, XmlNameTable nameTable, string commandText);

        protected static XQueryAdapterFactoryDelegate Factory { get; set; }

        public static XQueryAdapter Create(DatabaseDictionary dictionary, XmlNameTable nameTable, string commandText)
        {
            if (Factory == null)
                throw new ESQLException(Properties.Resources.XQueryEngineNotImplemented);
            return Factory(dictionary, nameTable, commandText);
        }

        public virtual void Dispose()
        {
        }

        public abstract void AddParameter(string localName);

        public abstract object Execute(object context, object[] args, XmlDocument xmlResult);
    }
}
