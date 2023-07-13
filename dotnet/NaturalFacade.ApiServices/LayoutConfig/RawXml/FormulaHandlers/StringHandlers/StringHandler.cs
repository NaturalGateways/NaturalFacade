using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class StringHandler : Natural.Xml.ITagHandler
    {
        #region Base

        /// <summary>The tracking.</summary>
        private RawXmlReferenceTracking m_tracking = null;

        /// <summary>The data to add to.</summary>
        private Dictionary<string, object> m_parentData = null;
        /// <summary>The data to add to.</summary>
        private object m_childData = null;

        /// <summary>The JSON name.</summary>
        private string m_jsonName = null;

        /// <summary>Constructor.</summary>
        public StringHandler(RawXmlReferenceTracking tracking, Dictionary<string, object> data, string jsonName)
        {
            m_tracking = tracking;
            m_parentData = data;
            m_jsonName = jsonName;
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public virtual Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "prop":
                    HandlePropTag(attributes);
                    break;
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public virtual void HandleEndTag()
        {
            if (m_childData != null)
            {
                m_parentData[m_jsonName] = m_childData;
            }
        }

        #endregion

        #region Tag handler

        /// <summary>Handle the prop tag.</summary>
        private void HandlePropTag(Natural.Xml.ITagAttributes attributes)
        {
            string propName = attributes.GetString("prop_name");
            int propIndex = m_tracking.GetPropertyUsedIndex(propName);
            m_childData = new Dictionary<string, object>
            {
                { "op", "Prop" },
                { "index", propIndex }
            };
        }

        #endregion
    }
}
