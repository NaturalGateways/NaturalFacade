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

            // Get font index
            int fontIndex = tracking.GetFontDefinitionUsedIndex(fontName);

            // Create data
            this.Data = new Dictionary<string, object>
            {
                { "elTyp", "Text" },
                { "font", fontIndex }
            };
            if (string.IsNullOrEmpty(text) == false)
            {
                this.Data.Add("text", text);
            }
        }

        #endregion

        #region BaseElementHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public override Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "text_prop":
                    HandleTextPropTag(attributes);
                    break;
            }
            return base.HandleStartChildTag(tagName, attributes);
        }

        #endregion

        #region Tag handling

        /// <summary>Handles a text propery under a text tag.</summary>
        private void HandleTextPropTag(Natural.Xml.ITagAttributes attributes)
        {
            switch (attributes.GetString("op"))
            {
                case "Prop":
                    {
                        string propName = attributes.GetString("name");
                        int propIndex = m_tracking.GetPropertyUsedIndex(propName);
                        this.Data["text"] = new Dictionary<string, object>
                        {
                            { "op", "Prop" },
                            { "index", propIndex }
                        };
                        break;
                    }
            }
        }

        #endregion
    }
}
