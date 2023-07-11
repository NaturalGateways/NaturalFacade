using Natural.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class RowsElementHandler : IBranchElementHandler
    {
        #region Base

        /// <summary>Resource tracking.</summary>
        private RawXmlReferenceTracking m_tracking = null;

        /// <summary>The list of children.</summary>
        private List<object> m_childList = new List<object>();

        /// <summary>Constructor.</summary>
        public RowsElementHandler(RawXmlReferenceTracking tracking, ITagAttributes attributes)
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
                    m_childList.Add(branchElementHandler.Data);
                    return branchElementHandler;
                }
            }
            {
                Dictionary<string, object> leafData = RawXmlElementFactory.CheckLeafTag(m_tracking, tagName, attributes);
                if (leafData != null)
                {
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
