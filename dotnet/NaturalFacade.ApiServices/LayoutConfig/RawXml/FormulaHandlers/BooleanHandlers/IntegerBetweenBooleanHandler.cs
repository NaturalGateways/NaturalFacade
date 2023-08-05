using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class IntegerBetweenBooleanHandler : Natural.Xml.ITagHandler
    {
        #region Base

        /// <summary>The tracking values.</summary>
        private RawXmlReferenceTracking m_tracking = null;
        /// <summary>The action to use to add the data to the parent data.</summary>
        private Action<object> m_addDataAction = null;

        /// <summary>The minimum inclusive.</summary>
        private object m_minIncData = null;
        /// <summary>The minimum axclusive.</summary>
        private object m_minExcData = null;
        /// <summary>The integer to compare.</summary>
        private object m_valueData = null;
        /// <summary>The maximum inclusive.</summary>
        private object m_maxIncData = null;
        /// <summary>The maximum axclusive.</summary>
        private object m_maxExcData = null;

        /// <summary>Constructor.</summary>
        public IntegerBetweenBooleanHandler(RawXmlReferenceTracking tracking, Action<object> addDataAction)
        {
            m_tracking = tracking;
            m_addDataAction = addDataAction;
        }

        #endregion

        #region Add child actions

        /// <summary>Adds child data.</summary>
        private void AddToMinInc(object childData)
        {
            m_minIncData = childData;
        }

        /// <summary>Adds child data.</summary>
        private void AddToMinExc(object childData)
        {
            m_minExcData = childData;
        }

        /// <summary>Adds child data.</summary>
        private void AddToValue(object childData)
        {
            m_valueData = childData;
        }

        /// <summary>Adds child data.</summary>
        private void AddToMaxInc(object childData)
        {
            m_maxIncData = childData;
        }

        /// <summary>Adds child data.</summary>
        private void AddToMaxExc(object childData)
        {
            m_maxExcData = childData;
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "min_inc":
                    return IntegerHandler.HandleTag(attributes, m_tracking, AddToMinInc);
                case "min_exc":
                    return IntegerHandler.HandleTag(attributes, m_tracking, AddToMinExc);
                case "value":
                    return IntegerHandler.HandleTag(attributes, m_tracking, AddToValue);
                case "max_inc":
                    return IntegerHandler.HandleTag(attributes, m_tracking, AddToMaxInc);
                case "max_exc":
                    return IntegerHandler.HandleTag(attributes, m_tracking, AddToMaxExc);
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public void HandleEndTag()
        {
            // Check value
            if (m_valueData == null)
                throw new Exception("Cannot have a between comparison without a value to compare.");

            // Check min data
            object minData = null;
            if (m_minIncData != null)
            {
                minData = new Dictionary<string, object> { { "op", "IntLessOrEqual" }, { "lhs", m_minIncData }, { "rhs", m_valueData } };
            }
            else if (m_minExcData != null)
            {
                minData = new Dictionary<string, object> { { "op", "IntLessThan" }, { "lhs", m_minExcData }, { "rhs", m_valueData } };
            }

            // Check max data
            object maxData = null;
            if (m_maxIncData != null)
            {
                maxData = new Dictionary<string, object> { { "op", "IntLessOrEqual" }, { "lhs", m_valueData }, { "rhs", m_maxIncData } };
            }
            else if (m_maxExcData != null)
            {
                maxData = new Dictionary<string, object> { { "op", "IntLessThan" }, { "lhs", m_valueData }, { "rhs", m_maxExcData } };
            }

            // Create or assign final operation
            if (minData != null && maxData != null)
            {
                m_addDataAction.Invoke(new Dictionary<string, object>
                {
                    { "op", "And" },
                    { "items", new object[] { minData, maxData } }
                });
            }
            else if (minData != null)
            {
                m_addDataAction.Invoke(minData);
            }
            else if (maxData != null)
            {
                m_addDataAction.Invoke(maxData);
            }
            else
            {
                throw new Exception("Cannot have a between comparison without a min or max.");
            }
        }

        #endregion
    }
}
