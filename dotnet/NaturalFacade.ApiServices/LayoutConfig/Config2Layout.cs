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
            // Read specific type
            Config2LayoutOverlayOutput convertOutput = null;
            switch (layoutConfig.LayoutType)
            {
                case Raw.RawLayoutConfig.TYPENAME:
                    convertOutput = Raw.RawLayout2Overlay.Convert(layoutConfig.Layout);
                    break;
                case RawXml.RawXmlLayoutConfig.TYPENAME:
                    convertOutput = RawXml.RawXmlLayout2Overlay.Convert(layoutConfig.Layout);
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
                    audioResources = convertOutput.AudioResources,
                    fonts = convertOutput.Fonts,
                    audios = convertOutput.Audios,
                    videos = convertOutput.Videos,
                    rootElement = convertOutput.RootElement
                },
                Actions = convertOutput.Actions
            };

            // Check if we write the properties for property editing
            if (convertOutput.PropertyDefs?.Any() ?? false)
            {
                int propertyNum = convertOutput.PropertyDefs.Length;
                result.Overlay.properties = new object[propertyNum];
                result.Properties = new ApiDto.PropertyDto[propertyNum];
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
                            result.Properties[propertyIndex].Value = oldProperty.Value;
                            result.PropertyValues[propertyIndex] = oldProperty.Value;
                        }
                    }
                }
            }
            else
            {
                result.Overlay.properties = Array.Empty<object>();
                result.Properties = Array.Empty<ApiDto.PropertyDto>();
                result.PropertyValues = Array.Empty<object>();
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

            // Convert controls
            result.Controls = convertOutput.ControlsArray?.Select(x => ConvertControlsToDto(convertOutput, x))?.Where(x => x != null)?.ToArray();

            // Return
            return result;
        }

        /// <summary>Converts a prop def to an overlay definition.</summary>
        private static object ConvertPropDefToOverlayDef(Config2LayoutOverlayOutputPropertyDef propertyDef)
        {
            Dictionary<string, object> result = new Dictionary<string, object>
            {
                { "type", propertyDef.ValueType.ToString() }
            };
            switch (propertyDef.ValueType)
            {
                case ApiDto.PropertyTypeDto.Timer:
                    result.Add("direction", propertyDef.TimerDirection ?? 1);
                    if (propertyDef.TimerMinValue.HasValue)
                        result.Add("minValue", propertyDef.TimerMinValue.Value);
                    if (propertyDef.TimerMaxValue.HasValue)
                        result.Add("maxValue", propertyDef.TimerMaxValue.Value);
                    break;
            }
            return result;
        }

        /// <summary>Converts a prop def to an property modified by a control.</summary>
        private static ApiDto.PropertyDto ConvertPropDefToPropDto(Config2LayoutOverlayOutputPropertyDef propertyDef)
        {
            return new ApiDto.PropertyDto
            {
                Type = propertyDef.ValueType,
                Name = propertyDef.Name,
                Value = propertyDef.DefaultValue
            };
        }

        /// <summary>Converts a set of controls.</summary>
        private static Config2LayoutResultControls ConvertControlsToDto(Config2LayoutOverlayOutput convertOutput, Config2LayoutOverlayOutputControlsDef controlsDef)
        {
            Config2LayoutResultControlsField[] fieldArray = controlsDef.Fields?.Select(x => ConvertControlsFieldToDto(convertOutput, x))?.Where(x => x != null)?.ToArray();
            if ((fieldArray?.Any() ?? false) == false)
            {
                return null;
            }
            return new Config2LayoutResultControls
            {
                Name = controlsDef.Name,
                SaveAll = controlsDef.SaveAll,
                Fields = fieldArray
            };
        }

        /// <summary>Converts a control field.</summary>
        private static Config2LayoutResultControlsField ConvertControlsFieldToDto(Config2LayoutOverlayOutput convertOutput, Config2LayoutOverlayOutputControlsFieldDef fieldDef)
        {
            Config2LayoutOverlayOutputPropertyDef propertyDef = convertOutput.PropertyDefs[fieldDef.PropIndex];
            return new Config2LayoutResultControlsField
            {
                PropIndex = fieldDef.PropIndex,
                ValueType = propertyDef.ValueType.ToString(),
                Label = fieldDef.Label,
                FieldDef = ConvertControlsFieldToFieldDefDto(fieldDef, propertyDef),
                DefaultValue = propertyDef.DefaultValue
            };
        }

        /// <summary>Converts a control field.</summary>
        private static object ConvertControlsFieldToFieldDefDto(Config2LayoutOverlayOutputControlsFieldDef fieldDef, Config2LayoutOverlayOutputPropertyDef propertyDef)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            if (fieldDef.AudioWalkman != null)
            {
                output.Add("AudioWalkman", fieldDef.AudioWalkman);
            }
            if (fieldDef.VideoWalkman != null)
            {
                output.Add("VideoWalkman", fieldDef.VideoWalkman);
            }
            if (fieldDef.TextField != null)
            {
                output.Add("TextField", fieldDef.TextField);
            }
            if (fieldDef.SelectOptions != null)
            {
                output.Add("SelectOptions", fieldDef.SelectOptions);
            }
            if (fieldDef.Integer != null)
            {
                Dictionary<string, object> subOutput = new Dictionary<string, object>
                {
                    { "Step", fieldDef.Integer.Step }
                };
                if (fieldDef.Integer.MinValue.HasValue)
                    subOutput.Add("MinValue", fieldDef.Integer.MinValue.Value);
                if (fieldDef.Integer.MaxValue.HasValue)
                    subOutput.Add("MaxValue", fieldDef.Integer.MaxValue.Value);
                output.Add("Integer", subOutput);
            }
            if (fieldDef.Switch != null)
            {
                output.Add("Switch", new Dictionary<string, object>
                {
                    { "FalseLabel", fieldDef.Switch.FalseLabel },
                    { "TrueLabel", fieldDef.Switch.TrueLabel }
                });
            }
            if (fieldDef.Timer != null)
            {
                Dictionary<string, object> subOutput = new Dictionary<string, object>
                {
                    { "AllowClear", fieldDef.Timer.AllowClear },
                    { "Direction", propertyDef.TimerDirection }
                };
                if (propertyDef.TimerMinValue.HasValue)
                    subOutput.Add("MinValue", propertyDef.TimerMinValue.Value);
                if (propertyDef.TimerMaxValue.HasValue)
                    subOutput.Add("MaxValue", propertyDef.TimerMaxValue.Value);
                output.Add("Timer", subOutput);
            }
            return output;
        }

        /// <summary>Getter for the property type of a property.</summary>
        public static ApiDto.PropertyTypeDto GetTypeOfProperty(string propertyType)
        {
            ApiDto.PropertyTypeDto parsedEnum = ApiDto.PropertyTypeDto.String;
            Enum.TryParse<ApiDto.PropertyTypeDto>(propertyType, out parsedEnum);
            return parsedEnum;
        }

        /// <summary>Converts a property value.</summary>
        public static object ConvertPropValue(ApiDto.PropertyTypeDto valueType, Natural.Json.IJsonObject valueJson)
        {
            switch (valueType)
            {
                case ApiDto.PropertyTypeDto.Audio:
                    {
                        return new Dictionary<string, object>
                        {
                            { "State", valueJson.GetDictionaryString("State") ?? "Stopped" }
                        };
                    }
                case ApiDto.PropertyTypeDto.Video:
                    {
                        return new Dictionary<string, object>
                        {
                            { "State", valueJson.GetDictionaryString("State") ?? "Stopped" },
                            { "PlayCount", valueJson.GetDictionaryLong("PlayCount") ?? 0 }
                        };
                    }
                case ApiDto.PropertyTypeDto.Boolean:
                    {
                        bool? boolValue = valueJson.AsBoolean;
                        if (boolValue.HasValue == false)
                            throw new Exception($"Value is not a boolean.");
                        return boolValue.Value;
                    }
                case ApiDto.PropertyTypeDto.String:
                    return valueJson.AsString;
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
