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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public class TreeComparer
    {
        public TreeComparer()
        {
        }

        public TreeComparer(string collation)
        {
        }

        public bool IgnoreWhitespace { get; set; }

        private bool TextEqual(string a, string b)
        {
            if (IgnoreWhitespace)
                return XQueryFuncs.NormalizeSpace(a) == 
                    XQueryFuncs.NormalizeSpace(b);
            else
                return a == b;
        }

        private bool IsWhitespaceNode(XPathNavigator nav)
        {
            if (IgnoreWhitespace && nav.NodeType == XPathNodeType.Text)
                return XmlCharType.Instance.IsOnlyWhitespace(nav.Value);
            return false;
        }

        private bool NodeEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            if (nav1.NodeType != nav2.NodeType)
                return false;
            switch (nav1.NodeType)
            {
                case XPathNodeType.Element:
                    return ElementEqual(nav1, nav2);

                case XPathNodeType.Attribute:
                    return AttributeEqual(nav1, nav2);

                case XPathNodeType.Text:
                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                case XPathNodeType.Comment:                    
                    return TextEqual(nav1.Value, nav2.Value);

                case XPathNodeType.ProcessingInstruction:
                    return ProcessingInstructionEqual(nav1, nav2);

                default:
                    return DeepEqual(nav1, nav2);
            }
        }

        private bool ElementEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            if (nav1.LocalName != nav2.LocalName || nav1.NamespaceURI != nav2.NamespaceURI)
                return false;
            return ElementAttributesEqual(nav1.Clone(), nav2.Clone()) && DeepEqual(nav1, nav2);
        }

        private bool ElementAttributesEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            if (nav1.HasAttributes != nav2.HasAttributes)
                return false;
            if (nav1.HasAttributes)
            {
                bool flag1 = nav1.MoveToFirstAttribute();
                bool flag2 = nav2.MoveToFirstAttribute();
                while (flag1 && flag2)
                {
                    flag1 = nav1.MoveToNextAttribute();
                    flag2 = nav2.MoveToNextAttribute();
                }
                nav1.MoveToParent();
                nav2.MoveToParent();
                if (flag1 != flag2)
                    return false;
                for (bool flag3 = nav1.MoveToFirstAttribute(); flag3; flag3 = nav1.MoveToNextAttribute())
                {
                    bool flag4 = nav2.MoveToFirstAttribute();
                    while (flag4)
                    {
                        if (AttributeEqual(nav1, nav2))
                            break;
                        flag4 = nav2.MoveToNextAttribute();
                    }
                    nav2.MoveToParent();
                    if (!flag4)
                    {
                        nav1.MoveToParent();
                        return false;
                    }
                }
                nav1.MoveToParent();
            }
            return true;
        }

        private bool ProcessingInstructionEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            return nav1.LocalName == nav2.LocalName &&
                nav1.Value == nav2.Value;
        }

        private bool AttributeEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            if (nav1.LocalName != nav2.LocalName || nav1.NamespaceURI != nav2.NamespaceURI)
                return false;
            return nav1.TypedValue.Equals(nav2.TypedValue);
        }

        public bool DeepEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            bool flag1;
            bool flag2;
            XPathNodeIterator iter1 = nav1.SelectChildren(XPathNodeType.All);
            XPathNodeIterator iter2 = nav2.SelectChildren(XPathNodeType.All);
            do
            {
                flag1 = iter1.MoveNext();
            }
            while (flag1 && IsWhitespaceNode(iter1.Current));
            do
            {
                flag2 = iter2.MoveNext();
            }
            while (flag2 && IsWhitespaceNode(iter2.Current));
            while (flag1 && flag2)
            {
                if (!NodeEqual(iter1.Current, iter2.Current))
                    return false;
                do
                {
                    flag1 = iter1.MoveNext();
                }
                while (flag1 && IsWhitespaceNode(iter1.Current));
                do
                {
                    flag2 = iter2.MoveNext();
                }
                while (flag2 && IsWhitespaceNode(iter2.Current));
            }
            return flag1 == flag2;
        }

        public bool DeepEqual(XQueryNodeIterator iter1, XQueryNodeIterator iter2)
        {
            iter1 = iter1.Clone();
            iter2 = iter2.Clone();
            bool flag1;
            bool flag2;
            do
            {
                flag1 = iter1.MoveNext();
                flag2 = iter2.MoveNext();
                if (flag1 != flag2)
                    return false;
                else
                    if (flag1 && flag2)
                    {
                        if (iter1.Current.IsNode != iter2.Current.IsNode)
                            return false;
                        else
                        {
                            if (iter1.Current.IsNode && iter2.Current.IsNode)
                            {
                                return NodeEqual((XPathNavigator)iter1.Current,
                                    (XPathNavigator)iter2.Current);
                            }
                            else
                                return iter1.Current.TypedValue.Equals(iter2.Current.TypedValue);
                        }
                    }
            }
            while (flag1 && flag2);
            return true;
        }        
    }
}
