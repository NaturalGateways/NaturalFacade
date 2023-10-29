using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class TextElementHandler : BaseElementHandler
    {
        #region Base

        /// <summary>Resource tracking.</summary>
        private RawXmlReferenceTracking m_tracking = null;

        /// <summary>The list of children.</summary>
        private List<object> m_childList = new List<object>();

        /// <summary>Constructor.</summary>
        public TextElementHandler(RawXmlReferenceTracking tracking, Natural.Xml.ITagAttributes attributes)
            : base(tracking)
        {
            m_tracking = tracking;

            // Get attributes
            string fontName = attributes.GetString("font");
            string text = attributes.GetNullableString("text");
            Raw.RawLayoutConfigElementTextFormat format = attributes.GetNullableEnum<Raw.RawLayoutConfigElementTextFormat>("format") ?? Raw.RawLayoutConfigElementTextFormat.None;

            // Get font index
            int fontIndex = tracking.GetFontDefinitionUsedIndex(fontName);

            // Create data
            this.Data = new Dictionary<string, object>
            {
                { "elTyp", "Text" },
                { "font", fontIndex }
            };
            if (format != Raw.RawLayoutConfigElementTextFormat.None)
            {
                this.Data.Add("format", format.ToString());
            }
            if (string.IsNullOrEmpty(text) == false)
            {
                this.Data.Add("text", text);
            }
        }

        #endregion

        #region Add child actions

        /// <summary>Adds child data to component list.</summary>
        private void AddTextProp(object childData)
        {
            this.Data["text"] = childData;
        }

        #endregion

        #region BaseElementHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public override Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "text_prop":
                    return StringHandler.HandleTag(attributes, m_tracking, AddTextProp);
            }
            return base.HandleStartChildTag(tagName, attributes);
        }

        #endregion
    }
}
