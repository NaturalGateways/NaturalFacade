using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class ConcatenationStringHandler : Natural.Xml.ITagHandler
    {
        #region Base

        /// <summary>The tracking values.</summary>
        private RawXmlReferenceTracking m_tracking = null;
        /// <summary>The action to use to add the data to the parent data.</summary>
        private Action<object> m_addDataAction = null;

        /// <summary>The parent data.</summary>
        private Dictionary<string, object> m_data = null;
        /// <summary>The parent data.</summary>
        private List<object> m_componentList = new List<object>();

        /// <summary>Constructor.</summary>
        public ConcatenationStringHandler(RawXmlReferenceTracking tracking, Action<object> addDataAction)
        {
            m_tracking = tracking;
            m_addDataAction = addDataAction;

            m_data = new Dictionary<string, object>
            {
                { "op", "Cat" },
                { "Children", m_componentList }
            };
        }

        #endregion

        #region Add child actions

        /// <summary>Adds child data to component list.</summary>
        private void AddComponent(object childData)
        {
            m_componentList.Add(childData);
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "component":
                    return StringHandler.HandleTag(attributes, m_tracking, AddComponent);
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public void HandleEndTag()
        {
            if (m_componentList.Any() == false)
                throw new Exception("Concatentation property has no values.");
            m_addDataAction.Invoke(m_data);
        }

        #endregion
    }
}
