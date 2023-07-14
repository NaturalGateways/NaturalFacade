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
        public static Natural.Xml.ITagHandler HandleTag(Natural.Xml.ITagAttributes attributes, RawXmlReferenceTracking tracking, Action<object> addDataAction)
        {
            string propType = attributes.GetString("type");
            switch (propType)
            {
                case "Cat":
                    return new ConcatenationStringHandler(tracking, addDataAction);
                case "Prop":
                    HandlePropTag(attributes, tracking, addDataAction);
                    return null;
                case "Text":
                    HandleTextTag(attributes, addDataAction);
                    return null;
                default:
                    throw new Exception($"Unrecognised prop type '{propType}'.");
            }
        }

        /// <summary>Handle the prop tag.</summary>
        private static void HandlePropTag(Natural.Xml.ITagAttributes attributes, RawXmlReferenceTracking tracking, Action<object> addDataAction)
        {
            string propName = attributes.GetString("prop_name");
            int propIndex = tracking.GetPropertyUsedIndex(propName);
            addDataAction.Invoke(new Dictionary<string, object>
            {
                { "op", "Prop" },
                { "index", propIndex }
            });
        }

        /// <summary>Handle the text tag.</summary>
        private static void HandleTextTag(Natural.Xml.ITagAttributes attributes, Action<object> addDataAction)
        {
            addDataAction.Invoke(new Dictionary<string, object>
            {
                { "op", "Text" },
                { "text", attributes.GetNullableString("text") ?? string.Empty }
            });
        }

        #endregion
    }
}
