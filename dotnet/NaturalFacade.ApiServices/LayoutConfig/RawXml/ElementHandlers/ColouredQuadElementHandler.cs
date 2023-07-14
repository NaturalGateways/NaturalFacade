using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class ColouredQuadElementHandler : BaseElementHandler
    {
        #region Base

        /// <summary>Constructor.</summary>
        public ColouredQuadElementHandler(RawXmlReferenceTracking tracking, Natural.Xml.ITagAttributes attributes)
            : base(tracking)
        {
            // Get attributes
            long? width = attributes.GetNullableLong("Width");
            long? height = attributes.GetNullableLong("Height");

            // Create data
            this.Data = new Dictionary<string, object>
            {
                { "elTyp", "ColouredQuad" },
                { "hex", attributes.GetString("hex") }
            };
            if (width.HasValue)
                this.Data.Add("width", width.Value);
            if (height.HasValue)
                this.Data.Add("height", height.Value);
        }

        #endregion
    }
}
