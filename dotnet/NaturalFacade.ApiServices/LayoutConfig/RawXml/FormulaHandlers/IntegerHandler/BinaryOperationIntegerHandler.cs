using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class BinaryOperationIntegerHandler : Natural.Xml.ITagHandler
    {
        #region Base

        /// <summary>The tracking values.</summary>
        private RawXmlReferenceTracking m_tracking = null;
        /// <summary>The action to use to add the data to the parent data.</summary>
        private Action<object> m_addDataAction = null;

        /// <summary>The parent data.</summary>
        private Dictionary<string, object> m_data = null;

        /// <summary>Constructor.</summary>
        public BinaryOperationIntegerHandler(RawXmlReferenceTracking tracking, string opType, Action<object> addDataAction)
        {
            m_tracking = tracking;
            m_addDataAction = addDataAction;

            m_data = new Dictionary<string, object>
            {
                { "op", opType }
            };
        }

        #endregion

        #region Add child actions

        /// <summary>Adds child data to the LHS.</summary>
        private void AddToLhs(object childData)
        {
            m_data.Add("lhs", childData);
        }

        /// <summary>Adds child data to the RHS.</summary>
        private void AddToRhs(object childData)
        {
            m_data.Add("rhs", childData);
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "lhs":
                    return IntegerHandler.HandleTag(attributes, m_tracking, AddToLhs);
                case "rhs":
                    return IntegerHandler.HandleTag(attributes, m_tracking, AddToRhs);
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public void HandleEndTag()
        {
            if (m_data.ContainsKey("lhs") == false)
                throw new Exception("Cannot have a binary integer operation without a 'lhs'.");
            if (m_data.ContainsKey("rhs") == false)
                throw new Exception("Cannot have a binary integer operation without a 'rhs'.");
            m_addDataAction.Invoke(m_data);
        }

        #endregion
    }
}
