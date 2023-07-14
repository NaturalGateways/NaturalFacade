using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal static class BooleanHandler
    {
        #region Tag handler

        /// <summary>Handle the prop tag.</summary>
        public static Natural.Xml.ITagHandler HandleTag(Natural.Xml.ITagAttributes attributes, RawXmlReferenceTracking tracking, Action<object> addDataAction)
        {
            string propType = attributes.GetString("type");
            switch (propType)
            {
                case "Prop":
                    HandlePropTag(attributes, tracking, addDataAction);
                    return null;
                case "StringEquals":
                    return new StringEqualsBooleanHandler(tracking, addDataAction);
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

        #endregion
    }
}
