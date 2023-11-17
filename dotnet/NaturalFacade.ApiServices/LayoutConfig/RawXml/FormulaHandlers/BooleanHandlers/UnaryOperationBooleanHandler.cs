using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class UnaryOperationBooleanHandler : Natural.Xml.ITagHandler
    {
        #region Base

        /// <summary>The tracking values.</summary>
        private RawXmlReferenceTracking m_tracking = null;

        /// <summary>The tag name of the child.</summary>
        private string m_childTagName = null;
        /// <summary>The action to use to add the data to the parent data.</summary>
        private Action<object> m_addDataAction = null;

        /// <summary>The parent data.</summary>
        private Dictionary<string, object> m_data = null;

        /// <summary>Constructor.</summary>
        public UnaryOperationBooleanHandler(RawXmlReferenceTracking tracking, string opType, string childTagName, Action<object> addDataAction)
        {
            m_tracking = tracking;
            m_childTagName = childTagName;
            m_addDataAction = addDataAction;

            m_data = new Dictionary<string, object>
            {
                { "op", opType }
            };
        }

        #endregion

        #region Add child actions

        /// <summary>Adds child data to the RHS.</summary>
        private void AddChild(object childData)
        {
            m_data.Add("item", childData);
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            if (tagName == m_childTagName)
            {
                return BooleanHandler.HandleTag(attributes, m_tracking, AddChild);
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public void HandleEndTag()
        {
            if (m_data.ContainsKey("item") == false)
                throw new Exception($"Cannot have a unary boolean operation without '{m_childTagName}'.");
            m_addDataAction.Invoke(m_data);
        }

        #endregion
    }
}
