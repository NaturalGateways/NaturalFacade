using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class StringEqualsBooleanHandler : Natural.Xml.ITagHandler
    {
        #region Base

        /// <summary>The tracking values.</summary>
        private RawXmlReferenceTracking m_tracking = null;
        /// <summary>The parent data.</summary>
        private Dictionary<string, object> m_parentData = null;
        /// <summary>The JSON name for where to add the data to the parent.</summary>
        private string m_jsonName = null;

        /// <summary>The parent data.</summary>
        private Dictionary<string, object> m_childData = null;

        /// <summary>Constructor.</summary>
        public StringEqualsBooleanHandler(RawXmlReferenceTracking tracking, Dictionary<string, object> data, string jsonName)
        {
            m_tracking = tracking;
            m_parentData = data;
            m_jsonName = jsonName;

            m_childData = new Dictionary<string, object>
            {
                { "op", "Equals" }
            };
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "lhs":
                    return StringHandler.HandleTag(attributes, m_tracking, m_childData, "lhs");
                case "rhs":
                    return StringHandler.HandleTag(attributes, m_tracking, m_childData, "rhs");
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public void HandleEndTag()
        {
            if (m_childData.ContainsKey("lhs") == false)
                throw new Exception("Cannot have an equals boolean attribute without a 'lhs'.");
            if (m_childData.ContainsKey("rhs") == false)
                throw new Exception("Cannot have an equals boolean attribute without a 'rhs'.");
            m_parentData.Add(m_jsonName, m_childData);
        }

        #endregion
    }
}
