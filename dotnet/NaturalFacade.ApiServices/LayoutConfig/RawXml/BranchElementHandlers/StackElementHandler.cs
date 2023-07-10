using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Natural.Xml;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class StackElementHandler : IBranchElementHandler
    {
        #region Base

        /// <summary>Resource tracking.</summary>
        private RawXmlReferenceTracking m_tracking = null;

        /// <summary>The list of children.</summary>
        private List<object> m_childList = new List<object>();

        /// <summary>Constructor.</summary>
        public StackElementHandler(RawXmlReferenceTracking tracking)
        {
            m_tracking = tracking;
            this.Data = new Dictionary<string, object>
            {
                { "elTyp", "Stack" },
                { "children", m_childList }
            };
        }

        /// <summary>Gets the parent-stack specific attributes and applies them to the child object.</summary>
        private void ApplyStackChildAttributes(ITagAttributes attributes, Dictionary<string, object> data)
        {
            // Get attributes
            Raw.RawLayoutConfigElementStackHAlignment hAlign = attributes.GetNullableEnum<Raw.RawLayoutConfigElementStackHAlignment>("stack.HAlign") ?? Raw.RawLayoutConfigElementStackHAlignment.Fill;
            Raw.RawLayoutConfigElementStackVAlignment vAlign = attributes.GetNullableEnum<Raw.RawLayoutConfigElementStackVAlignment>("stack.VAlign") ?? Raw.RawLayoutConfigElementStackVAlignment.Fill;
            long widthPixels = attributes.GetNullableLong("stack.WidthPixels") ?? 0;
            long heightPixels = attributes.GetNullableLong("stack.HeightPixels") ?? 0;
            long margin = attributes.GetNullableLong("stack.Margin") ?? 0;
            long marginHorizontal = attributes.GetNullableLong("stack.MarginHorizontal") ?? margin;
            long marginVertical = attributes.GetNullableLong("stack.MarginVertical") ?? margin;
            long marginLeft = attributes.GetNullableLong("stack.MarginLeft") ?? marginHorizontal;
            long marginRight = attributes.GetNullableLong("stack.MarginRight") ?? marginHorizontal;
            long marginTop = attributes.GetNullableLong("stack.MarginTop") ?? marginVertical;
            long marginBottom = attributes.GetNullableLong("stack.MarginBottom") ?? marginVertical;

            // Apply attributes
            if (hAlign != Raw.RawLayoutConfigElementStackHAlignment.Fill)
                data.Add("halign", hAlign.ToString());
            if (vAlign != Raw.RawLayoutConfigElementStackVAlignment.Fill)
                data.Add("valign", vAlign.ToString());
            if (widthPixels != 0)
                data.Add("width", widthPixels);
            if (heightPixels != 0)
                data.Add("height", heightPixels);
            if (marginLeft != 0)
                data.Add("stackMarginLeft", marginLeft);
            if (marginRight != 0)
                data.Add("stackMarginRight", marginRight);
            if (marginTop != 0)
                data.Add("stackMarginTop", marginTop);
            if (marginBottom != 0)
                data.Add("stackMarginBottom", marginBottom);
        }

        #endregion

        #region IBranchElementHandler implementation

        /// <summary>The data of the branch.</summary>
        public Dictionary<string, object> Data { get; private set; }

        #endregion

        #region IBranchElementHandler - ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public ITagHandler HandleStartChildTag(string tagName, ITagAttributes attributes)
        {
            {
                IBranchElementHandler branchElementHandler = RawXmlElementFactory.CheckBranchTag(m_tracking, tagName, attributes);
                if (branchElementHandler != null)
                {
                    ApplyStackChildAttributes(attributes, branchElementHandler.Data);
                    m_childList.Add(branchElementHandler.Data);
                    return branchElementHandler;
                }
            }
            {
                Dictionary<string, object> leafData = RawXmlElementFactory.CheckLeafTag(m_tracking, tagName, attributes);
                if (leafData != null)
                {
                    ApplyStackChildAttributes(attributes, leafData);
                    m_childList.Add(leafData);
                    return null;
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
    }
}
