using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.Xml.XPath;

using DataEngine;
using DataEngine.CoreServices;
using DataEngine.XQuery;

namespace DataEngine.XQuery
{
    public class XQueryDBContext : XQueryContext
    {
        private DatabaseDictionary m_dictionary;
        private Dictionary<string, XQueryDocument> m_cache;

        public XQueryDBContext(DatabaseDictionary dictionary, XmlNameTable nameTable)
            : base(nameTable)
        {
            m_dictionary = dictionary;
            m_cache = new Dictionary<string, XQueryDocument>();
        }

        public override XQueryNodeIterator CreateCollection(string collection_name)
        {
            lock (m_cache)
            {
                XQueryDocument doc;
                if (m_cache.TryGetValue(collection_name, out doc))
                    return XQueryNodeIterator.Create(doc.CreateNavigator());
                string prefix;
                string[] identifierPart;
                DataEngine.CoreServices.Util.ParseCollectionName(collection_name, out prefix, out identifierPart);
                TableType tableType = m_dictionary.GetTableType(prefix, identifierPart);
                DataProviderTableAccessor accessor = new DataProviderTableAccessor(tableType);
                ResultsetReader reader = new ResultsetReader(accessor.Get(null, null), 
                    XmlConvert.EncodeName(identifierPart[identifierPart.Length -1]), GetSettings());
                doc = new XQueryDocument(reader);
                m_cache.Add(collection_name, doc);
                return XQueryNodeIterator.Create(doc.CreateNavigator());
            }
        }

        public override void Close()
        {
            base.Close();
            foreach (XQueryDocument doc in m_cache.Values)
                doc.Close();
            m_cache.Clear();
        }

        public DatabaseDictionary Dictionary
        {
            get
            {
                return m_dictionary;
            }            
        }
    }
}