using Natural.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class HSplitElementHandler : IBranchElementHandler
    {
        #region Base

        /// <summary>Resource tracking.</summary>
        private RawXmlReferenceTracking m_tracking = null;

        /// <summary>Constructor.</summary>
        public HSplitElementHandler(RawXmlReferenceTracking tracking, ITagAttributes attributes)
        {
            long spacing = attributes.GetNullableLong("spacing") ?? 0;

            m_tracking = tracking;
            this.Data = new Dictionary<string, object>
            {
                { "elTyp", "HFloat" }
            };
            if (0 < spacing)
                this.Data.Add("spacing", spacing);
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
                    StoreChild(attributes, branchElementHandler.Data);
                    return branchElementHandler;
                }
            }
            {
                Dictionary<string, object> leafData = RawXmlElementFactory.CheckLeafTag(m_tracking, tagName, attributes);
                if (leafData != null)
                {
                    StoreChild(attributes, leafData);
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

        #region Tag

        /// <summary>Stores a child in a location found from attributes.</summary>
        private void StoreChild(ITagAttributes attributes, object childData)
        {
            string align = attributes.GetString("hSplit.Align");
            string alignKey = null;
            switch (align)
            {
                case "Left":
                case "Middle":
                case "Right":
                    alignKey = align.ToLower();
                    break;
            }
            if (this.Data.ContainsKey(alignKey))
            {
                throw new Exception($"HSplit cannot have multiple '{align}' children.");
            }
            this.Data.Add(alignKey, childData);
        }

        #endregion
    }
}
