using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class RowsElementHandler : BaseElementHandler
    {
        #region Base

        /// <summary>Resource tracking.</summary>
        private RawXmlReferenceTracking m_tracking = null;

        /// <summary>The list of children.</summary>
        private List<object> m_childList = new List<object>();

        /// <summary>Constructor.</summary>
        public RowsElementHandler(RawXmlReferenceTracking tracking, Natural.Xml.ITagAttributes attributes)
        {
            long spacing = attributes.GetNullableLong("spacing") ?? 0;

            m_tracking = tracking;
            this.Data = new Dictionary<string, object>
            {
                { "elTyp", "Rows" },
                { "children", m_childList }
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
                    m_childList.Add(branchElementHandler.Data);
                    return branchElementHandler;
                }
            }
            return base.HandleStartChildTag(tagName, attributes);
        }

        #endregion
    }
}
