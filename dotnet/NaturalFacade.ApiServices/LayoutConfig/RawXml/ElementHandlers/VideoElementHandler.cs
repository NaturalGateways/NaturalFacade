using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class VideoElementHandler : BaseElementHandler
    {
        #region Base

        /// <summary>Constructor.</summary>
        public VideoElementHandler(RawXmlReferenceTracking tracking, Natural.Xml.ITagAttributes attributes)
            : base(tracking)
        {
            // Get image index
            string url = attributes.GetString("url");
            int propIndex = tracking.GetPropertyUsedIndex(attributes.GetString("prop"));
            int resIndex = tracking.CreateVideoResource(url, propIndex);

            // Create data
            this.Data = new Dictionary<string, object>
            {
                { "elTyp", "Video" },
                { "res", resIndex }
            };
        }

        #endregion
    }
}
