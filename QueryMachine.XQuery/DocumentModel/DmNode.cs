//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;


namespace DataEngine.XQuery.DocumentModel
{
    internal abstract class DmNode
    {        
        internal DmNode _parent = null;
        internal int _index = -1;

        public abstract XdmNode CreateNode();

        public abstract XPathNodeType NodeType
        {
            get;
        }

        public DmNode ParentNode
        {
            get
            {
                return _parent;
            }
        }

        public virtual bool HasChildNodes
        {
            get
            {
                return false;
            }
        }

        public virtual bool HasAttributes
        {
            get
            {
                return false;
            }
        }

        public virtual DmNodeList ChildNodes
        {
            get
            {
                return null;
            }
        }

        public virtual DmNodeList ChildAttributes
        {
            get
            {
                return null;
            }
        }

        public virtual String Name
        {
            get
            {
                string prefix = Prefix;
                if (String.IsNullOrEmpty(prefix))
                    return LocalName;
                else
                    return prefix + ':' + LocalName;
            }
        }

        public virtual String Prefix
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual String LocalName
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual String NamespaceURI
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual IXmlSchemaInfo SchemaInfo
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public DmRoot OwnerDocument
        {
            get
            {
                if (_parent != null)
                {
                    if (_parent.NodeType == XPathNodeType.Root)
                        return (DmRoot)_parent;
                    return _parent.OwnerDocument;
                }
                return null;
            }
        }                

#if DEBUG
        public override string  ToString()
        {
            string name = Name;
            if (name == "")
                return GetType().Name;
            return String.Format("{0}:{1}", GetType().Name, name);
        }
#endif

        public bool IsAncestor(DmNode node)
        {
            for (node = node.ParentNode; node != null; node = node.ParentNode)
            {
                if (node == this)
                    return true;
            }
            return false;
        }

        protected class DmSchemaInfo : XmlSchemaInfo
        {
            public DmSchemaInfo(IXmlSchemaInfo xmlSchemaInfo)
            {
                IsDefault = xmlSchemaInfo.IsDefault;
                IsNil = xmlSchemaInfo.IsNil;
                MemberType = xmlSchemaInfo.MemberType;
                SchemaAttribute = xmlSchemaInfo.SchemaAttribute;
                SchemaElement = xmlSchemaInfo.SchemaElement;
                SchemaType = xmlSchemaInfo.SchemaType;
                Validity = xmlSchemaInfo.Validity;
            }
        }
    }
}
