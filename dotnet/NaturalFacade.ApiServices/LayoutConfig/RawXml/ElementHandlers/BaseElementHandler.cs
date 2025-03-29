using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal abstract class BaseElementHandler : Natural.Xml.ITagHandler
    {
        #region Static facade

        /// <summary>The data created by the subclass.</summary>
        public static BaseElementHandler CreateForTag(RawXmlReferenceTracking tracking, string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "coloured_quad":
                    return new ColouredQuadElementHandler(tracking, attributes);
                case "hSplit":
                    return new HSplitElementHandler(tracking, attributes);
                case "image":
                    return new ImageElementHandler(tracking, attributes);
                case "rows":
                    return new RowsElementHandler(tracking, attributes);
                case "stack":
                    return new StackElementHandler(tracking);
                case "text":
                    return new TextElementHandler(tracking, attributes);
                case "transform":
                    return new TransformElementHandler(tracking, attributes);
                case "video":
                    return new VideoElementHandler(tracking, attributes);
                case "vSplit":
                    return new VSplitElementHandler(tracking, attributes);
            }
            return null;
        }

        #endregion

        #region Base

        /// <summary>Resource tracking.</summary>
        protected RawXmlReferenceTracking Tracking { get; private set; }

        /// <summary>The data created by the subclass.</summary>
        public Dictionary<string, object> Data { get; protected set; } = null;

        /// <summary>Constructor.</summary>
        public BaseElementHandler(RawXmlReferenceTracking tracking)
        {
            this.Tracking = tracking;
        }

        #endregion

        #region Add data handlers

        /// <summary>Used as an action for reading in properties.</summary>
        private void AddIsVisibleData(object data)
        {
            this.Data.Add("isVisible", data);
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public virtual Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "is_visible":
                    return BooleanHandler.HandleTag(attributes, this.Tracking, AddIsVisibleData);
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public virtual void HandleEndTag()
        {
            //
        }

        #endregion
    }
}
