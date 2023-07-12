using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class VSplitElementHandler : BaseElementHandler
    {
        #region Base

        /// <summary>Resource tracking.</summary>
        private RawXmlReferenceTracking m_tracking = null;

        /// <summary>Constructor.</summary>
        public VSplitElementHandler(RawXmlReferenceTracking tracking, Natural.Xml.ITagAttributes attributes)
        {
            long spacing = attributes.GetNullableLong("spacing") ?? 0;

            m_tracking = tracking;
            this.Data = new Dictionary<string, object>
            {
                { "elTyp", "VFloat" }
            };
            if (0 < spacing)
                this.Data.Add("spacing", spacing);
        }

        #endregion

        #region BaseElementHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public override Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            {
                BaseElementHandler branchElementHandler = BaseElementHandler.CreateForTag(m_tracking, tagName, attributes);
                if (branchElementHandler != null)
                {
                    StoreChild(attributes, branchElementHandler.Data);
                    return branchElementHandler;
                }
            }
            return base.HandleStartChildTag(tagName, attributes);
        }

        #endregion

        #region Tag

        /// <summary>Stores a child in a location found from attributes.</summary>
        private void StoreChild(Natural.Xml.ITagAttributes attributes, object childData)
        {
            string align = attributes.GetString("vSplit.Align");
            string alignKey = null;
            switch (align)
            {
                case "Top":
                case "Middle":
                case "Bottom":
                    alignKey = align.ToLower();
                    break;
            }
            if (this.Data.ContainsKey(alignKey))
            {
                throw new Exception($"VSplit cannot have multiple '{align}' children.");
            }
            this.Data.Add(alignKey, childData);
        }

        #endregion
    }
}
