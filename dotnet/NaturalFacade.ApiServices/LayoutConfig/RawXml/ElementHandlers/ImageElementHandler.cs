using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class ImageElementHandler : BaseElementHandler
    {
        #region Base

        /// <summary>Constructor.</summary>
        public ImageElementHandler(RawXmlReferenceTracking tracking, Natural.Xml.ITagAttributes attributes)
        {
            // Get fit
            Raw.RawLayoutConfigElementImageFit fit = attributes.GetNullableEnum<Raw.RawLayoutConfigElementImageFit>("fit") ?? Raw.RawLayoutConfigElementImageFit.None;
            Raw.RawLayoutConfigElementImageFit hFit = attributes.GetNullableEnum<Raw.RawLayoutConfigElementImageFit>("hFit") ?? fit;
            Raw.RawLayoutConfigElementImageFit vFit = attributes.GetNullableEnum<Raw.RawLayoutConfigElementImageFit>("vFit") ?? fit;

            // Get image index
            int imageIndex = tracking.GetImageResourceUsedIndex(attributes.GetString("res"));

            // Create data
            this.Data = new Dictionary<string, object>
            {
                { "elTyp", "Image" },
                { "hfit", hFit.ToString() },
                { "vfit", vFit.ToString() },
                { "res", imageIndex }
            };
        }

        #endregion
    }
}
