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

using DataEngine;
using DataEngine.XQuery;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;


namespace XQueryConsole
{
    public class XQueryAdapterImpl: XQueryAdapter
    {
        static XQueryAdapterImpl()
        {
            Factory = new XQueryAdapterFactoryDelegate((DatabaseDictionary dictionary, XmlNameTable nameTable, string commandText) =>
                new XQueryAdapterImpl(dictionary, nameTable, commandText));
        }

        public static void Init()
        {
        }

        private XQueryCommand _command;

        private XQueryAdapterImpl(DatabaseDictionary dictionary, XmlNameTable nameTable, string commandText)
        {
            _command = new XQueryCommand(new XQueryDsContext(dictionary, nameTable));
            _command.SearchPath = dictionary.SearchPath;
            _command.CommandText = commandText;
        }

        public override void Dispose()
        {
            if (_command != null)
            {
                _command.Dispose();
                _command = null;
            }
        }

        public override void AddParameter(string localName) 
        {
            XQueryParameter parameter = new XQueryParameter();
            parameter.LocalName = localName;
            _command.Parameters.Add(parameter);
        }

        public override object Execute(object context, object[] args, XmlDocument xmlResult)
        {
            if (context != null)
            {
                XmlNode node = context as XmlNode;
                if (node == null)
                {
                    if (context is Resultset)
                    {
                        XmlReader reader = new ResultsetReader((Resultset)context,
                            "context", _command.Context.GetSettings());
                        XQueryDocument tmp = new XQueryDocument(reader);
                        _command.ContextItem = tmp.CreateNavigator();
                    }
                    else
                        throw new ESQLException(
                            String.Format(Properties.Resources.XmlQueryContextMustBeANode, context), null);
                }
                else
                    _command.ContextItem = node.CreateNavigator();
            }
            if (args != null)
            {
                for (int k = 0; k < args.Length; k++)
                {
                    XQueryParameter param = _command.Parameters[k];
                    object val = args[k];
                    if (val is Resultset)
                    {
                        XmlReader reader = new ResultsetReader((Resultset)val,
                            XmlConvert.EncodeName(param.LocalName), _command.Context.GetSettings());
                        XQueryDocument tmp = new XQueryDocument(reader);
                        param.Value = tmp.CreateNavigator();
                    }
                    else if (val is XmlNode)
                        param.Value = ((XmlNode)val).CreateNavigator();
                    else if (val is XmlNodeList)
                        param.Value = new  XmlNodeListIterator((XmlNodeList)val);
                    else
                        param.Value = val;
                }
            }
            XQueryNodeIterator iter = _command.Execute();
            DOMConverter converter = new DOMConverter(xmlResult);
            XmlDataAccessor.NodeList res = new XmlDataAccessor.NodeList();
            while (iter.MoveNext())
                res.Add(converter.ToXmlNode(iter.Current));
            if (res.Count == 1)
                return res[0];
            return res;
        }
    }
}
