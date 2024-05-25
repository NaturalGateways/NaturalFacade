using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class TransformElementHandler : BaseElementHandler
    {
        #region Base

        /// <summary>Resource tracking.</summary>
        private RawXmlReferenceTracking m_tracking = null;

        /// <summary>The list of children.</summary>
        private List<object> m_transformStepList = new List<object>();
        /// <summary>The list of children.</summary>
        private List<object> m_childList = new List<object>();

        /// <summary>Constructor.</summary>
        public TransformElementHandler(RawXmlReferenceTracking tracking, Natural.Xml.ITagAttributes attributes)
            : base(tracking)
        {
            m_tracking = tracking;
            this.Data = new Dictionary<string, object>
            {
                { "elTyp", "Transform" },
                { "steps", m_transformStepList },
                { "children", m_childList }
            };
        }

        #endregion

        #region BaseElementHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public override Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "rotate":
                    HandleRotateTag(attributes);
                    return null;
            }
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

        #region Create step tag

        /// <summary>The pivot types.</summary>
        private enum PivotType
        {
            Center
        }

        /// <summary>Handles the rotate tag.</summary>
        private void HandleRotateTag(Natural.Xml.ITagAttributes attributes)
        {
            // Get rotation
            double degreesCW = attributes.GetLong("deg_cw");

            // Get other attributes
            PivotType pivotType = attributes.GetNullableEnum<PivotType>("pivot") ?? PivotType.Center;

            // Set data
            m_transformStepList.Add(new Dictionary<string, object>
            {
                { "type", "rot" },
                { "deg_cw", degreesCW },
                { "pivot", pivotType.ToString() }
            });
        }

        #endregion
    }
}
