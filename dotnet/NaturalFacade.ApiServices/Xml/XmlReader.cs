using System;
using System.Collections.Generic;
using System.Linq;

namespace NaturalFacade.Xml
{
    public static class XmlReader
    {
        /// <summary>Reads XML from an XML string.</summary>
        public static void ReadFromString(string xmlString, IXmlReader reader)
        {
            // Create reader
            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(new StringReader(xmlString));
            InternalObjects.XmlAttributes attributes = new InternalObjects.XmlAttributes(xmlReader);
            Stack<IXmlHandler> handlerStack = new Stack<IXmlHandler>();

            // Search for root tag
            while (true)
            {
                // Check next
                if (xmlReader.Read() == false)
                    return;
                // Check type
                if (xmlReader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    IXmlHandler rootHandler = reader.HandleRootTag(xmlReader.Name, attributes);
                    if (rootHandler == null)
                        return;
                    handlerStack.Push(rootHandler);
                    break;
                }
            }

            // Rest of tags
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case System.Xml.XmlNodeType.Element:
                        {
                            IXmlHandler handler = handlerStack.Peek()?.HandleStartChildTag(xmlReader.Name, attributes);
                            if (xmlReader.IsEmptyElement == false)
                                handlerStack.Push(handler);
                            break;
                        }
                    case System.Xml.XmlNodeType.EndElement:
                        {
                            IXmlHandler handler = handlerStack.Pop();
                            handler?.HandleEndTag();
                            break;
                        }
                }
            }
        }
    }
}
