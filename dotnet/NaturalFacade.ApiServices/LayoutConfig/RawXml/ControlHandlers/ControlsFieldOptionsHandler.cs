using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml.ControlHandlers
{
    internal class ControlsFieldOptionsHandler : Natural.Xml.ITagHandler
    {
        #region Base

        /// <summary>The field.</summary>
        private Config2LayoutOverlayOutputControlsFieldDef m_field = null;

        /// <summary>The value list.</summary>
        private List<string> m_valueList = new List<string>();

        /// <summary>Constructor.</summary>
        public ControlsFieldOptionsHandler(Config2LayoutOverlayOutputControlsFieldDef field)
        {
            m_field = field;
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "option":
                    HandleOptionTag(attributes);
                    break;
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public void HandleEndTag()
        {
            if (m_valueList.Any())
            {
                m_field.SelectOptions = m_valueList;
            }
        }

        #endregion

        #region Tag handler

        /// <summary>Handles a child tag.</summary>
        private void HandleOptionTag(Natural.Xml.ITagAttributes attributes)
        {
            string value = attributes.GetString("value");
            m_valueList.Add(value);
        }

        #endregion
    }
}
