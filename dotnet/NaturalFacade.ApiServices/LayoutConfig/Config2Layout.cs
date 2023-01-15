using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace NaturalFacade.LayoutConfig
{
    public static class Config2Layout
    {
        public static Config2LayoutResult Convert(LayoutConfig layoutConfig, ApiDto.PropertyDto[] properties)
        {
            // Read specific type
            Config2LayoutOverlayOutput convertOutput = null;
            switch (layoutConfig.LayoutType)
            {
                case Raw.RawLayoutConfig.TYPENAME:
                    convertOutput = Raw.RawLayout2Overlay.Convert(layoutConfig.Raw);
                    break;
                default:
                    throw new Exception($"Cannot convert layout of type '{layoutConfig.LayoutType}'.");
            }

            // Create result
            Config2LayoutResult result = new Config2LayoutResult
            {
                Overlay = new ApiDto.OverlayDto
                {
                    canvasSize = new int[]
                    {
                        layoutConfig.Width ?? 1920,
                        layoutConfig.Height ?? 1080
                    },
                    imageResources = convertOutput.ImageResources,
                    fontResources = convertOutput.FontResources,
                    fonts = convertOutput.Fonts,
                    rootElement = convertOutput.RootElement
                },
                ControlsArray = convertOutput.ControlsArray
            };

            // Check if we write the properties for property editing
            if (convertOutput.PropertyDefs?.Any() ?? false)
            {
                int propertyNum = convertOutput.PropertyDefs.Length;
                result.Overlay.properties = new object[propertyNum];
                result.Properties = new Dictionary<string, object>[propertyNum];
                result.PropertyValues = new object[propertyNum];
                Dictionary<string, int> indexByName = new Dictionary<string, int>();
                for (int propertyIndex = 0; propertyIndex != propertyNum; ++propertyIndex)
                {
                    Config2LayoutOverlayOutputPropertyDef propertyDef = convertOutput.PropertyDefs[propertyIndex];
                    result.Overlay.properties[propertyIndex] = ConvertPropDefToOverlayDef(propertyDef);
                    result.Properties[propertyIndex] = ConvertPropDefToPropDto(propertyDef);
                    result.PropertyValues[propertyIndex] = propertyDef.DefaultValue;
                    indexByName.Add(propertyDef.Name, propertyIndex);
                }
                if (properties?.Any() ?? false)
                {
                    foreach (ApiDto.PropertyDto oldProperty in properties)
                    {
                        if (indexByName.ContainsKey(oldProperty.Name))
                        {
                            int propertyIndex = indexByName[oldProperty.Name];
                            result.Properties[propertyIndex]["UpdatedValue"] = oldProperty.UpdatedValue;
                            result.PropertyValues[propertyIndex] = oldProperty.UpdatedValue;
                        }
                    }
                }
            }

            // Set timing defaults
            if (layoutConfig.RedrawMillis.HasValue)
            {
                if (0 < layoutConfig.RedrawMillis.Value)
                    result.Overlay.redrawMillis = layoutConfig.RedrawMillis.Value;
            }
            else if (convertOutput.PropertyDefs?.Any(x => x.ValueType == ApiDto.PropertyTypeDto.Timer) ?? false)
            {
                result.Overlay.redrawMillis = 250;
            }
            if (layoutConfig.ApiFetchMillis.HasValue)
            {
                if (0 < layoutConfig.ApiFetchMillis.Value)
                    result.Overlay.apiFetchMillis = layoutConfig.ApiFetchMillis.Value;
            }
            else if (convertOutput.PropertyDefs?.Any() ?? false)
            {
                result.Overlay.apiFetchMillis = 2500;
            }

            // Return
            return result;
        }

        /// <summary>Converts a prop def to an overlay definition.</summary>
        private static object ConvertPropDefToOverlayDef(Config2LayoutOverlayOutputPropertyDef propertyDef)
        {
            Dictionary<string, object> result = new Dictionary<string, object>
            {
                { "ValueType", propertyDef.ValueType.ToString() }
            };
            if (propertyDef.TimerDirection.HasValue)
                result.Add("TimerDirection", propertyDef.TimerDirection.Value);
            if (propertyDef.TimerMinValue.HasValue)
                result.Add("TimerMinValue", propertyDef.TimerMinValue.Value);
            if (propertyDef.TimerMaxValue.HasValue)
                result.Add("TimerMaxValue", propertyDef.TimerMaxValue.Value);
            return result;
        }

        /// <summary>Converts a prop def to an property modified by a control.</summary>
        private static Dictionary<string, object> ConvertPropDefToPropDto(Config2LayoutOverlayOutputPropertyDef propertyDef)
        {
            Dictionary<string, object> output = new Dictionary<string, object>
            {
                { "ValueType", propertyDef.ValueType.ToString() },
                { "Name", propertyDef.Name },
                { "DefaultValue", propertyDef.DefaultValue }
            };
            if (propertyDef.TimerDirection.HasValue)
                output.Add("TimerDirection", propertyDef.TimerDirection.Value);
            if (propertyDef.TimerMinValue.HasValue)
                output.Add("TimerMinValue", propertyDef.TimerMinValue.Value);
            if (propertyDef.TimerMaxValue.HasValue)
                output.Add("TimerMaxValue", propertyDef.TimerMaxValue.Value);
            return output;
        }

        /// <summary>Converts a property value.</summary>
        public static object ConvertPropValue(ApiDto.PropertyTypeDto valueType, Natural.Json.IJsonObject valueJson)
        {
            switch (valueType)
            {
                case ApiDto.PropertyTypeDto.String:
                    return valueJson.AsString;
                case ApiDto.PropertyTypeDto.Boolean:
                    {
                        bool? boolValue = valueJson.AsBoolean;
                        if (boolValue.HasValue == false)
                            throw new Exception($"Value is not a boolean.");
                        return boolValue.Value;
                    }
                case ApiDto.PropertyTypeDto.Timer:
                    {
                        switch (valueJson.ObjectType)
                        {
                            case Natural.Json.JsonObjectType.Long:
                                return new Dictionary<string, object>
                                {
                                    { "Secs", valueJson.AsLong.Value }
                                };
                            case Natural.Json.JsonObjectType.Dictionary:
                                {
                                    long? seconds = valueJson.GetDictionaryLong("Secs");
                                    string startDateTime = valueJson.GetDictionaryString("StartDateTime");
                                    Dictionary<string, object> returnObject = new Dictionary<string, object>
                                    {
                                        { "Secs", seconds ?? 0 }
                                    };
                                    if (string.IsNullOrEmpty(startDateTime) == false)
                                    {
                                        returnObject.Add("StartDateTime", startDateTime);
                                    }
                                    return returnObject;
                                }
                            default:
                                throw new Exception($"Value is not a timer type: {valueJson.ObjectType.ToString()}");
                        }
                    }
                default:
                    return null;
            }
        }
    }
}
