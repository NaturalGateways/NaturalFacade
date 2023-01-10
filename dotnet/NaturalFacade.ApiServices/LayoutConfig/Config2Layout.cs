using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig
{
    public static class Config2Layout
    {
        public static Config2LayoutResult Convert(LayoutConfig layoutConfig, ApiDto.PropertyDto[] properties)
        {
            // Create result
            Config2LayoutResult result = new Config2LayoutResult
            {
                Overlay = new ApiDto.OverlayDto
                {
                    canvasSize = new int[]
                    {
                        layoutConfig.Width ?? 1920,
                        layoutConfig.Height ?? 1080
                    }
                },
                Properties = properties
            };

            // Read specific type
            switch (layoutConfig.LayoutType)
            {
                case Raw.RawLayoutConfig.TYPENAME:
                    Raw.RawLayout2Overlay.Convert(result, layoutConfig.Raw);
                    break;
                default:
                    throw new Exception($"Cannot convert layout of type '{layoutConfig.LayoutType}'.");
            }

            // Compile property values
            result.PropertyValues = Properties2Values.GetValuesFromProperties(result.Properties);

            // Return
            return result;
        }

        /// <summary>Converts a property value.</summary>
        public static object ConvertPropValue(ApiDto.PropertyTypeDto valueType, object rawLayoutValue)
        {
            switch (valueType)
            {
                case ApiDto.PropertyTypeDto.String:
                    return Natural.Json.JsonHelper.JsonFromObject(rawLayoutValue).AsString;
                case ApiDto.PropertyTypeDto.Boolean:
                    {
                        bool? boolValue = Natural.Json.JsonHelper.JsonFromObject(rawLayoutValue).AsBoolean;
                        if (boolValue.HasValue == false)
                            throw new Exception($"Value is not a boolean.");
                        return boolValue.Value;
                    }
                case ApiDto.PropertyTypeDto.Timer:
                    {
                        Natural.Json.IJsonObject jsonValue = Natural.Json.JsonHelper.JsonFromObject(rawLayoutValue);
                        switch (jsonValue.ObjectType)
                        {
                            case Natural.Json.JsonObjectType.Long:
                                return new Dictionary<string, object>
                                {
                                    { "Secs", jsonValue.AsLong.Value }
                                };
                            case Natural.Json.JsonObjectType.Dictionary:
                                {
                                    long? seconds = jsonValue.GetDictionaryLong("Secs");
                                    string startDateTime = jsonValue.GetDictionaryString("StartDateTime");
                                    Dictionary<string, object> returnObject = new Dictionary<string, object>
                                    {
                                        { "Secs", seconds ?? 0 }
                                    };
                                    if (string.IsNullOrEmpty(startDateTime) == false)
                                        returnObject.Add("StartDateTime", startDateTime);
                                    return returnObject;
                                }
                            default:
                                throw new Exception($"Value is not a timer type: {jsonValue.ObjectType.ToString()}");
                        }
                    }
                default:
                    return rawLayoutValue;
            }
        }
    }
}
