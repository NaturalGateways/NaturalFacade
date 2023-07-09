using NaturalFacade.LayoutConfig.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal static class RawXmlElementFactory
    {
        /// <summary>Checks if the tag is an element that needs a handler.</summary>
        public static IBranchElementHandler CheckBranchTag(RawXmlReferenceTracking tracking, string tagName, Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "stack":
                    return new StackElementHandler(tracking);
            }
            return null;
        }

        /// <summary>Checks if the tag is an element that needs a handler.</summary>
        public static Dictionary<string, object> CheckLeafTag(RawXmlReferenceTracking tracking, string tagName, Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "image":
                    return ReadImageTag(tracking, attributes);
            }
            return null;
        }

        /// <summary>Checks if the tag is an element that needs a handler.</summary>
        public static Dictionary<string, object> ReadImageTag(RawXmlReferenceTracking tracking, Xml.ITagAttributes attributes)
        {
            // Get fit
            RawLayoutConfigElementImageFit fit = attributes.GetNullableEnum<RawLayoutConfigElementImageFit>("fit") ?? RawLayoutConfigElementImageFit.None;
            RawLayoutConfigElementImageFit hFit = attributes.GetNullableEnum<RawLayoutConfigElementImageFit>("hFit") ?? fit;
            RawLayoutConfigElementImageFit vFit = attributes.GetNullableEnum<RawLayoutConfigElementImageFit>("vFit") ?? fit;

            // Get image index
            int imageIndex = tracking.GetImageResourceUsedIndex(attributes.GetString("res"));

            // Create data
            Dictionary<string, object>  data = new Dictionary<string, object>
            {
                { "elTyp", "Image" },
                { "hfit", hFit.ToString() },
                { "vfit", vFit.ToString() },
                { "res", imageIndex }
            };

            // Return
            return data;
        }
    }
}
