using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    public class RawXmlLayout2Overlay : Xml.IXmlReader, Xml.IXmlHandler
    {
        #region Static facade

        /// <summary>Converts the data.</summary>
        public static Config2LayoutOverlayOutput Convert(object layoutConfig)
        {
            RawXmlLayout2Overlay instance = new RawXmlLayout2Overlay();
            string layoutConfigXmlString = layoutConfig as string;
            if (string.IsNullOrEmpty(layoutConfigXmlString) == false)
                Xml.XmlReader.ReadFromString(layoutConfigXmlString, instance);
            return instance.CreateOutput();
        }

        #endregion

        #region Base

        /// <summary>Resource tracking.</summary>
        private RawXmlReferenceTracking m_tracking = new RawXmlReferenceTracking();

        /// <summary>Check for a root handler.</summary>
        private IBranchElementHandler m_rootElementHandler = null;

        /// <summary>Creates an output object.</summary>
        public Config2LayoutOverlayOutput CreateOutput()
        {
            Config2LayoutOverlayOutput output = new Config2LayoutOverlayOutput
            {
                ImageResources = m_tracking.ImageResourcesUsedList.Select(x => x.Url).ToArray(),
                RootElement = m_rootElementHandler?.Data
            };
            return output;
        }

        #endregion

        #region Xml.IXmlReader implementation

        /// <summary>Called when the root tag is hit to get the initial handler.</summary>
        public Xml.IXmlHandler HandleRootTag(string tagName, Xml.ITagAttributes attributes)
        {
            if (tagName == "layout")
                return this;
            return null;
        }

        #endregion

        #region Xml.IXmlHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public Xml.IXmlHandler HandleStartChildTag(string tagName, Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "resource":
                    ReadResourceTag(attributes);
                    break;
            }
            if (m_rootElementHandler == null)
            {
                m_rootElementHandler = RawXmlElementFactory.CheckBranchTag(m_tracking, tagName, attributes);
                if (m_rootElementHandler != null)
                {
                    return m_rootElementHandler;
                }
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public void HandleEndTag()
        {
            //
        }

        #endregion

        #region Tag handlers

        /// <summary>Reads a tag attributes into an object</summary>
        private void ReadResourceTag(Xml.ITagAttributes attributes)
        {
            string name = attributes.GetString("name");
            switch (attributes.GetString("type"))
            {
                case "Image":
                    {
                        string url = attributes.GetString("url");
                        m_tracking.AddImageResource(name, url);
                        break;
                    }
            }
        }

        #endregion
    }
}
