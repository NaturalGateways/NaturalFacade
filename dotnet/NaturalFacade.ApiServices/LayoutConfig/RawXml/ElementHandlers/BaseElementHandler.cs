﻿using System;
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

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public virtual Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "is_visible":
                    return new BooleanHandler(this.Tracking, this.Data, "isVisible");
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