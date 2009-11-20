using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace DataEngine.XQuery.DocumentModel
{
    internal class DmNamespace: DmNode
    {
        private string _name;

        public DmNamespace(string name)
        {
            _name = name;
        }

        public override XdmNode CreateNode()
        {
            throw new InvalidOperationException();
        }

        public override XPathNodeType NodeType
        {
            get 
            {
                return XPathNodeType.Namespace;
            }
        }

        public override string LocalName
        {
            get
            {
                return _name;
            }
        }
    }
}
