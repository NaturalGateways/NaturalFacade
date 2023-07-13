using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal static class StringHandler
    {
        #region Tag handler

        /// <summary>Handle the prop tag.</summary>
        public static Natural.Xml.ITagHandler HandleTag(Natural.Xml.ITagAttributes attributes, RawXmlReferenceTracking tracking, Dictionary<string, object> data, string jsonName)
        {
            string propType = attributes.GetString("type");
            switch (propType)
            {
                case "Prop":
                    HandlePropTag(attributes, tracking, data, jsonName);
                    return null;
                case "Text":
                    HandleTextTag(attributes, data, jsonName);
                    return null;
                default:
                    throw new Exception($"Unrecognised prop type '{propType}'.");
            }
        }

        /// <summary>Handle the prop tag.</summary>
        private static void HandlePropTag(Natural.Xml.ITagAttributes attributes, RawXmlReferenceTracking tracking, Dictionary<string, object> data, string jsonName)
        {
            string propName = attributes.GetString("prop_name");
            int propIndex = tracking.GetPropertyUsedIndex(propName);
            data[jsonName] = new Dictionary<string, object>
            {
                { "op", "Prop" },
                { "index", propIndex }
            };
        }

        /// <summary>Handle the text tag.</summary>
        private static void HandleTextTag(Natural.Xml.ITagAttributes attributes, Dictionary<string, object> data, string jsonName)
        {
            data[jsonName] = new Dictionary<string, object>
            {
                { "op", "Text" },
                { "text", attributes.GetNullableString("text") ?? string.Empty }
            };
        }

        #endregion
    }
}
