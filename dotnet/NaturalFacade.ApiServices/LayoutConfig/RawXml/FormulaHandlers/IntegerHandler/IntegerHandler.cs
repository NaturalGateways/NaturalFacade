using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class IntegerHandler
    {
        /// <summary>Handle the prop tag.</summary>
        public static Natural.Xml.ITagHandler HandleTag(Natural.Xml.ITagAttributes attributes, RawXmlReferenceTracking tracking, Action<object> addDataAction)
        {
            string propType = attributes.GetString("type");
            switch (propType)
            {
                case "AudioDurationSecs":
                case "AudioPositionSecs":
                    HandleAudioTag(attributes, tracking, propType, addDataAction);
                    return null;
                case "Prop":
                    HandlePropTag(attributes, tracking, addDataAction);
                    return null;
                case "Value":
                    HandleValueTag(attributes, addDataAction);
                    return null;
                default:
                    throw new Exception($"Unrecognised integer prop type '{propType}'.");
            }
        }

        /// <summary>Handle the integer tag.</summary>
        private static void HandleAudioTag(Natural.Xml.ITagAttributes attributes, RawXmlReferenceTracking tracking, string opType, Action<object> addDataAction)
        {
            string audioName = attributes.GetString("audio");
            int audioIndex = tracking.GetAudioResourceUsedIndex(audioName);
            addDataAction.Invoke(new Dictionary<string, object>
            {
                { "op", opType },
                { "audio", audioIndex }
            });
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

        /// <summary>Handle the integer tag.</summary>
        private static void HandleValueTag(Natural.Xml.ITagAttributes attributes, Action<object> addDataAction)
        {
            addDataAction.Invoke(new Dictionary<string, object>
            {
                { "op", "Value" },
                { "value", attributes.GetNullableLong("value") ?? 0 }
            });
        }
    }
}
