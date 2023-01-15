using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NaturalFacade.LayoutConfig.Raw
{
    public class RawLayout2Overlay
    {
        /// <summary>Converts the data.</summary>
        public static Config2LayoutOverlayOutput Convert(RawLayoutConfig layoutConfig)
        {
            // Create an instance
            RawLayout2Overlay instance = new RawLayout2Overlay(layoutConfig);
            Config2LayoutOverlayOutput output = new Config2LayoutOverlayOutput();

            // Get root element
            output.RootElement = instance.ConvertElement(layoutConfig.RootElement);

            // Set resources
            if (instance.m_propertyUsedList.Any())
                output.PropertyDefs = instance.m_propertyUsedList.Select(x => GetOverlayPropertyDef(x)).ToArray();
            if (instance.m_imageResourcesUsedList.Any())
                output.ImageResources = instance.m_imageResourcesUsedList.Select(x => x.ResConfig.Url).ToArray();
            if (instance.m_fontResourcesUsedList.Any())
                output.FontResources = instance.m_fontResourcesUsedList.Select(x => x.ResConfig.Url).ToArray();
            if (instance.m_fontObjUsedList.Any())
                output.Fonts = instance.m_fontObjUsedList.Select(x => new ApiDto.OverlayDtoFont
                {
                    res = x.ResIndex.Value,
                    size = x.Font.Size,
                    colour = x.Font.Colour,
                    align = x.Font.Align
                }).ToArray();

            // Convert controls
            output.ControlsArray = layoutConfig.Controls?.Select(x => instance.ConvertControls(x))?.ToArray();

            // Return
            return output;
        }

        /// <summary>Getter for the property type of a property.</summary>
        private static Config2LayoutOverlayOutputPropertyDef GetOverlayPropertyDef(PropertyRef propertyRef)
        {
            ApiDto.PropertyTypeDto typeDto = GetTypeOfProperty(propertyRef);
            Config2LayoutOverlayOutputPropertyDef output = new Config2LayoutOverlayOutputPropertyDef
            {
                ValueType = typeDto,
                Name = propertyRef.PropName,
                DefaultValue = Config2Layout.ConvertPropValue(typeDto, propertyRef.DefaultValueJson)
            };
            if (propertyRef.PropType == "Timer")
            {
                output.TimerDirection = (int)propertyRef.PropJson.GetDictionaryLong("Direction").Value;
                output.TimerMinValue = propertyRef.PropJson.GetDictionaryLong("MinValue");
                output.TimerMaxValue = propertyRef.PropJson.GetDictionaryLong("MaxValue");
            }
            return output;
        }

        /// <summary>Getter for the property type of a property.</summary>
        private static ApiDto.PropertyTypeDto GetTypeOfProperty(PropertyRef propertyRef)
        {
            ApiDto.PropertyTypeDto parsedEnum = ApiDto.PropertyTypeDto.String;
            Enum.TryParse<ApiDto.PropertyTypeDto>(propertyRef.PropType, out parsedEnum);
            return parsedEnum;
        }

        /// <summary>The properties and their indexes.</summary>
        private Dictionary<string, PropertyRef> m_propertyByName = new Dictionary<string, PropertyRef>();
        /// <summary>The properties and their indexes.</summary>
        private List<PropertyRef> m_propertyUsedList = new List<PropertyRef>();

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, ResourceRef> m_imageResourcesByName = null;
        /// <summary>The resources indexed.</summary>
        private List<ResourceRef> m_imageResourcesUsedList = new List<ResourceRef>();

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, ResourceRef> m_fontResourcesByName = null;
        /// <summary>The resources indexed.</summary>
        private List<ResourceRef> m_fontResourcesUsedList = new List<ResourceRef>();
        
        /// <summary>The font definitions.</summary>
        private Dictionary<string, FontObjResource> m_fontObjsByName = null;
        /// <summary>The font definitions.</summary>
        private List<FontObjResource> m_fontObjUsedList = new List<FontObjResource>();

        /// <summary>Constructor.</summary>
        private RawLayout2Overlay(RawLayoutConfig layoutConfig)
        {
            if (layoutConfig.Properties?.Any() ?? false)
            {
                m_propertyByName = layoutConfig.Properties.Select(x => new PropertyRef(x)).ToDictionary(x => x.PropName);
            }
            else
            {
                m_propertyByName = new Dictionary<string, PropertyRef>();
            }
            if (layoutConfig.Resources?.Any() ?? false)
            {
                m_imageResourcesByName = layoutConfig.Resources.Where(x => x.Type == "Image").Select(x => new ResourceRef(x)).ToDictionary(x => x.ResConfig.Name);
                m_fontResourcesByName = layoutConfig.Resources.Where(x => x.Type == "Font").Select(x => new ResourceRef(x)).ToDictionary(x => x.ResConfig.Name);
            }
            else
            {
                m_imageResourcesByName = new Dictionary<string, ResourceRef>();
                m_fontResourcesByName = new Dictionary<string, ResourceRef>();
            }
            if (layoutConfig.Fonts?.Any() ?? false)
            {
                m_fontObjsByName = layoutConfig.Fonts.Select(x => new FontObjResource(x)).ToDictionary(x => x.Font.Name);
            }
            else
            {
                m_fontObjsByName = new Dictionary<string, FontObjResource>();
            }
        }

        /// <summary>A property with a boolean flag for whether it is used or not.</summary>
        private class PropertyRef
        {
            public Natural.Json.IJsonObject PropJson { get; private set; }
            public string PropName { get; private set; }
            public string PropType { get; private set; }
            public Natural.Json.IJsonObject DefaultValueJson { get; private set; }

            public int? PropIndex { get; set; }

            public PropertyRef(object propConfig)
            {
                this.PropJson = Natural.Json.JsonHelper.JsonFromObject(propConfig);
                this.PropName = this.PropJson.GetDictionaryString("Name");
                this.PropType = this.PropJson.GetDictionaryString("Type");
                this.DefaultValueJson = this.PropJson.GetDictionaryObject("DefaultValue");
            }
        }

        /// <summary>Getter for the property with the given name, marking it as used.</summary>
        private PropertyRef GetPropertyFromName(string name)
        {
            if (m_propertyByName.ContainsKey(name) == false)
            {
                throw new Exception($"Undefined property '{name}'.");
            }
            PropertyRef property = m_propertyByName[name];
            if (property.PropIndex.HasValue == false)
            {
                property.PropIndex = m_propertyUsedList.Count;
                m_propertyUsedList.Add(property);
            }
            return property;
        }

        /// <summary>A resource with a boolean flag for whether it is used or not.</summary>
        private class ResourceRef
        {
            public RawLayoutConfigResource ResConfig { get; private set; }

            public int? ResIndex { get; set; }

            public ResourceRef(RawLayoutConfigResource resConfig)
            {
                this.ResConfig = resConfig;
            }
        }

        /// <summary>A resource with a boolean flag for whether it is used or not.</summary>
        private class FontObjResource
        {
            public RawLayoutConfigFont Font { get; private set; }

            public int? FontIndex { get; set; }

            public int? ResIndex { get; set; }

            public FontObjResource(RawLayoutConfigFont font)
            {
                this.Font = font;
            }
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertElement(RawLayoutConfigElement layoutElement)
        {
            // Convert for specific type
            Dictionary<string, object> overlayObject = ConvertElementType(layoutElement);

            // Check rules
            if (layoutElement.IsVisibleOp != null)
                overlayObject.Add("isVisible", ConvertBooleanCondition(layoutElement.IsVisibleOp));

            // Return
            return overlayObject;
        }
        
        /// <summary>Creates an overlay element from a layout element of a specfic type.</summary>
        private Dictionary<string, object> ConvertElementType(RawLayoutConfigElement layoutElement)
        {
            if (layoutElement.HFloat != null)
                return ConvertHFloatElement(layoutElement.HFloat);
            if (layoutElement.Rows != null)
                return ConvertRowsElement(layoutElement.Rows);
            if (layoutElement.Stack != null)
                return ConvertStackElement(layoutElement.Stack);
            if (layoutElement.VFloat != null)
                return ConvertVFloatElement(layoutElement.VFloat);
            if (layoutElement.ColouredQuad != null)
                return ConvertColouredQuadElement(layoutElement.ColouredQuad);
            if (layoutElement.Image != null)
                return ConvertImageElement(layoutElement.Image);
            if (layoutElement.Text != null)
                return ConvertTextElement(layoutElement.Text);
            throw new Exception("Unrecognized element type.");
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertHFloatElement(RawLayoutConfigElementHFloat layoutHFloat)
        {
            int spacing = layoutHFloat.Spacing ?? 0;
            int marginLeft = layoutHFloat.MarginLeft ?? layoutHFloat.MarginHorizontal ?? layoutHFloat.Margin ?? 0;
            int marginRight = layoutHFloat.MarginRight ?? layoutHFloat.MarginHorizontal ?? layoutHFloat.Margin ?? 0;
            int marginTop = layoutHFloat.MarginTop ?? layoutHFloat.MarginVertical ?? layoutHFloat.Margin ?? 0;
            int marginBottom = layoutHFloat.MarginBottom ?? layoutHFloat.MarginVertical ?? layoutHFloat.Margin ?? 0;
            Dictionary<string, object> overlayObject = new Dictionary<string, object>
            {
                { "elTyp", "HFloat" }
            };
            if (spacing != 0)
                overlayObject.Add("spacing", spacing);
            if (marginLeft != 0)
                overlayObject.Add("marginLeft", marginLeft);
            if (marginRight != 0)
                overlayObject.Add("marginRight", marginRight);
            if (marginTop != 0)
                overlayObject.Add("marginTop", marginTop);
            if (marginBottom != 0)
                overlayObject.Add("marginBottom", marginBottom);
            if (layoutHFloat.Left != null)
                overlayObject.Add("left", ConvertElement(layoutHFloat.Left));
            if (layoutHFloat.Middle != null)
                overlayObject.Add("middle", ConvertElement(layoutHFloat.Middle));
            if (layoutHFloat.Right != null)
                overlayObject.Add("right", ConvertElement(layoutHFloat.Right));
            return overlayObject;
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertRowsElement(RawLayoutConfigElementRows layoutRows)
        {
            // Get defaults
            int spacing = layoutRows.Spacing ?? 0;
            // Create data
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "elTyp", "Rows" },
                { "children", layoutRows.Children.Select(x => ConvertElement(x)).ToArray() }
            };
            if (spacing != 0)
                data.Add("spacing", spacing);
            return data;
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertStackElement(RawLayoutConfigElementStack layoutStack)
        {
            return new Dictionary<string, object>
            {
                { "elTyp", "Stack" },
                { "children", layoutStack.Children.Select(x => ConvertStackChildElement(x)).ToArray() }
            };
        }

        /// <summary>Converts a string to a stack alignment.</summary>
        private RawLayoutConfigElementStackHAlignment ConvertStringToStackHAlign(string attValue, RawLayoutConfigElementStackHAlignment defaultValue)
        {
            if (string.IsNullOrEmpty(attValue))
            {
                return defaultValue;
            }
            RawLayoutConfigElementStackHAlignment enumValue = defaultValue;
            if (Enum.TryParse<RawLayoutConfigElementStackHAlignment>(attValue, out enumValue))
            {
                return enumValue;
            }
            throw new Exception($"Unrecognised stack halign: '{attValue}'");
        }

        /// <summary>Converts a string to a stack alignment.</summary>
        private RawLayoutConfigElementStackVAlignment ConvertStringToStackVAlign(string attValue, RawLayoutConfigElementStackVAlignment defaultValue)
        {
            if (string.IsNullOrEmpty(attValue))
            {
                return defaultValue;
            }
            RawLayoutConfigElementStackVAlignment enumValue = defaultValue;
            if (Enum.TryParse<RawLayoutConfigElementStackVAlignment>(attValue, out enumValue))
            {
                return enumValue;
            }
            throw new Exception($"Unrecognised stack halign: '{attValue}'");
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertStackChildElement(RawLayoutConfigElementStackChild layoutStackChild)
        {
            // Get element
            Dictionary<string, object> overlayObject = ConvertElement(layoutStackChild.Element);

            // Add stack attributes
            RawLayoutConfigElementStackHAlignment hAlign = ConvertStringToStackHAlign(layoutStackChild.HAlign, RawLayoutConfigElementStackHAlignment.Fill);
            RawLayoutConfigElementStackVAlignment vAlign = ConvertStringToStackVAlign(layoutStackChild.VAlign, RawLayoutConfigElementStackVAlignment.Fill);
            int widthPixels = layoutStackChild.WidthPixels ?? 0;
            int heightPixels = layoutStackChild.HeightPixels ?? 0;
            int marginLeft = layoutStackChild.MarginLeft ?? layoutStackChild.MarginHorizontal ?? layoutStackChild.Margin ?? 0;
            int marginRight = layoutStackChild.MarginRight ?? layoutStackChild.MarginHorizontal ?? layoutStackChild.Margin ?? 0;
            int marginTop = layoutStackChild.MarginTop ?? layoutStackChild.MarginVertical ?? layoutStackChild.Margin ?? 0;
            int marginBottom = layoutStackChild.MarginBottom ?? layoutStackChild.MarginVertical ?? layoutStackChild.Margin ?? 0;
            if (hAlign != RawLayoutConfigElementStackHAlignment.Fill)
                overlayObject.Add("halign", hAlign.ToString());
            if (vAlign != RawLayoutConfigElementStackVAlignment.Fill)
                overlayObject.Add("valign", vAlign.ToString());
            if (widthPixels != 0)
                overlayObject.Add("width", widthPixels);
            if (heightPixels != 0)
                overlayObject.Add("height", heightPixels);
            if (marginLeft != 0)
                overlayObject.Add("stackMarginLeft", marginLeft);
            if (marginRight != 0)
                overlayObject.Add("stackMarginRight", marginRight);
            if (marginTop != 0)
                overlayObject.Add("stackMarginTop", marginTop);
            if (marginBottom != 0)
                overlayObject.Add("stackMarginBottom", marginBottom);
            return overlayObject;
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertVFloatElement(RawLayoutConfigElementVFloat layoutVFloat)
        {
            int spacing = layoutVFloat.Spacing ?? 0;
            int marginLeft = layoutVFloat.MarginLeft ?? layoutVFloat.MarginHorizontal ?? layoutVFloat.Margin ?? 0;
            int marginRight = layoutVFloat.MarginRight ?? layoutVFloat.MarginHorizontal ?? layoutVFloat.Margin ?? 0;
            int marginTop = layoutVFloat.MarginTop ?? layoutVFloat.MarginVertical ?? layoutVFloat.Margin ?? 0;
            int marginBottom = layoutVFloat.MarginBottom ?? layoutVFloat.MarginVertical ?? layoutVFloat.Margin ?? 0;
            Dictionary<string, object> overlayObject = new Dictionary<string, object>
            {
                { "elTyp", "VFloat" }
            };
            if (spacing != 0)
                overlayObject.Add("spacing", spacing);
            if (marginLeft != 0)
                overlayObject.Add("marginLeft", marginLeft);
            if (marginRight != 0)
                overlayObject.Add("marginRight", marginRight);
            if (marginTop != 0)
                overlayObject.Add("marginTop", marginTop);
            if (marginBottom != 0)
                overlayObject.Add("marginBottom", marginBottom);
            if (layoutVFloat.Top != null)
                overlayObject.Add("top", ConvertElement(layoutVFloat.Top));
            if (layoutVFloat.Middle != null)
                overlayObject.Add("middle", ConvertElement(layoutVFloat.Middle));
            if (layoutVFloat.Bottom != null)
                overlayObject.Add("bottom", ConvertElement(layoutVFloat.Bottom));
            return overlayObject;
        }

        /// <summary>Converts a string to an image fit enum.</summary>
        private RawLayoutConfigElementImageFit ConvertStringToImageFit(string fitString)
        {
            if (string.IsNullOrEmpty(fitString))
            {
                return RawLayoutConfigElementImageFit.None;
            }
            RawLayoutConfigElementImageFit fitEnum = RawLayoutConfigElementImageFit.None;
            if (Enum.TryParse<RawLayoutConfigElementImageFit>(fitString, out fitEnum))
            {
                return fitEnum;
            }
            throw new Exception($"Unrecognised image fit: '{fitString}'");
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertColouredQuadElement(RawLayoutConfigElementColouredQuad layoutColouredQuad)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "elTyp", "ColouredQuad" },
                { "hex", layoutColouredQuad.Hex }
            };
            if (layoutColouredQuad.Width.HasValue)
                data.Add("width", layoutColouredQuad.Width.Value);
            if (layoutColouredQuad.Height.HasValue)
                data.Add("height", layoutColouredQuad.Height.Value);
            return data;
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertImageElement(RawLayoutConfigElementImage layoutImage)
        {
            // Check font res exists
            if (m_imageResourcesByName.ContainsKey(layoutImage.Res) == false)
            {
                throw new Exception($"Image resource '{layoutImage.Res}' is not defined.");
            }
            ResourceRef imageFile = m_imageResourcesByName[layoutImage.Res];
            // Check if we mark font as used
            if (imageFile.ResIndex.HasValue == false)
            {
                imageFile.ResIndex = m_imageResourcesUsedList.Count;
                m_imageResourcesUsedList.Add(imageFile);
            }

            // Return object
            return new Dictionary<string, object>
            {
                { "elTyp", "Image" },
                { "hfit", ConvertStringToImageFit(layoutImage.HFit ?? layoutImage.Fit).ToString() },
                { "vfit", ConvertStringToImageFit(layoutImage.VFit ?? layoutImage.Fit).ToString() },
                { "res", imageFile.ResIndex.Value }
            };
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertTextElement(RawLayoutConfigElementText layoutText)
        {
            // Check font exists
            if (m_fontObjsByName.ContainsKey(layoutText.Font) == false)
            {
                throw new Exception($"Font '{layoutText.Font}' is not defined.");
            }
            FontObjResource fontObj = m_fontObjsByName[layoutText.Font];
            // Check if we mark font as used
            if (fontObj.FontIndex.HasValue == false)
            {
                fontObj.FontIndex = m_fontObjUsedList.Count;
                m_fontObjUsedList.Add(fontObj);
            }

            // Check font res exists
            if (m_fontResourcesByName.ContainsKey(fontObj.Font.FontRes) == false)
            {
                throw new Exception($"Font resource '{fontObj.Font.FontRes}' is not defined.");
            }
            ResourceRef fontFile = m_fontResourcesByName[fontObj.Font.FontRes];
            // Check if we mark font as used
            if (fontFile.ResIndex.HasValue == false)
            {
                fontFile.ResIndex = m_fontResourcesUsedList.Count;
                m_fontResourcesUsedList.Add(fontFile);
            }
            fontObj.ResIndex = fontFile.ResIndex;

            // Create object
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "elTyp", "Text" },
                { "font", fontObj.FontIndex.Value }
            };

            // Add text or text parameter
            if (layoutText.TextOp != null)
            {
                data.Add("text", ConvertStringOperation(layoutText.TextOp));
            }
            else
            {
                data.Add("text", new Dictionary<string, object>
                {
                    { "op", "Text" },
                    { "text", layoutText.Text }
                });
            }

            // Return object
            return data;
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertBooleanCondition(RawLayoutConfigBooleanCondition condition)
        {
            switch (condition.Op)
            {
                case "Prop":
                    {
                        PropertyRef property = GetPropertyFromName(condition.Name);
                        return new Dictionary<string, object>
                        {
                            { "op", "Prop" },
                            { "index", property.PropIndex.Value }
                        };
                    }
                case "And":
                case "Or":
                    if ((condition.Children?.Any() ?? false) == false)
                        throw new Exception($"'{condition.Op}' boolean conditions must have children.");
                    return new Dictionary<string, object>
                    {
                        { "op", condition.Op },
                        { "items", condition.Children.Select(x => ConvertBooleanCondition(x)).ToArray() }
                    };
                case "Not":
                    if (condition.Child == null)
                        throw new Exception("'Not' boolean conditions must have a child.");
                    return new Dictionary<string, object>
                    {
                        { "op", condition.Op },
                        { "item", ConvertBooleanCondition(condition.Child) }
                    };
                default:
                    throw new Exception($"Unknown operation type '{condition.Op}'.");
            }
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertStringOperation(RawLayoutConfigStringOperation operation)
        {
            switch (operation.Op)
            {
                case "Text":
                    return new Dictionary<string, object>
                    {
                        { "op", "Text" },
                        { "text", operation.Text ?? string.Empty }
                    };
                case "Prop":
                    {
                        PropertyRef property = GetPropertyFromName(operation.Name);
                        return new Dictionary<string, object>
                        {
                            { "op", "Prop" },
                            { "index", property.PropIndex.Value }
                        };
                    }
                case "Cat":
                    if ((operation.Children?.Any() ?? false) == false)
                        throw new Exception("'Cat' string operation must have children.");
                    return new Dictionary<string, object>
                    {
                        { "op", "Cat" },
                        { "items", operation.Children.Select(x => ConvertStringOperation(x)).ToArray() }
                    };
                case "If":
                    if (operation.If == null)
                        throw new Exception("'If' string operation must have an 'If' condition.");
                    if (operation.Then == null)
                        throw new Exception("'If' string operation must have an 'Then' operation.");
                    Dictionary<string, object> opOutput = new Dictionary<string, object>
                    {
                        { "op", "If" },
                        { "if", ConvertBooleanCondition(operation.If) },
                        { "then", ConvertStringOperation(operation.Then) }
                    };
                    if (operation.Else != null)
                        opOutput.Add("else", ConvertStringOperation(operation.Else));
                    return opOutput;
                default:
                    throw new Exception($"Unknown operation type '{operation.Op}'.");
            }
        }

        /// <summary>Converts a controls object.</summary>
        private ItemModel.ItemLayoutControlsData ConvertControls(RawLayoutConfigControls srcControls)
        {
            return new ItemModel.ItemLayoutControlsData
            {
                Name = srcControls.Name,
                SaveAll = srcControls.SaveAll,
                Fields = srcControls.Fields.Select(x => ConvertControlsField(x)).Where(x => x != null).ToArray()
            };
        }

        /// <summary>Converts a controls object.</summary>
        private object ConvertControlsField(RawLayoutConfigControlsField srcField)
        {
            // Get property
            if (string.IsNullOrEmpty(srcField.PropName))
            {
                throw new Exception("Prop name for a controls field must be provided.");
            }
            if (m_propertyByName.ContainsKey(srcField.PropName) == false)
            {
                throw new Exception($"Prop name '{srcField.PropName}' not found.");
            }
            PropertyRef propertyRef = m_propertyByName[srcField.PropName];
            if (propertyRef.PropIndex.HasValue == false)
            {
                return null;
            }

            // Create field
            Dictionary<string, object> destField = new Dictionary<string, object>
            {
                { "Label", srcField.Label ?? propertyRef.PropName },
                { "PropIndex", propertyRef.PropIndex.Value },
                { "AllowTextEdit", srcField.AllowTextEdit }
            };
            if (srcField.Options?.Any() ?? false)
                destField.Add("Options", srcField.Options);
            if (srcField.Integer != null)
                destField.Add("Integer", ConvertControlsFieldInteger(srcField.Integer));
            if (srcField.Switch != null)
                destField.Add("Switch", ConvertControlsFieldSwitch(srcField.Switch));
            if (srcField.Timer != null)
                destField.Add("Timer", ConvertControlsFieldTimer(srcField.Timer));
            return destField;
        }

        /// <summary>Converts a controls object.</summary>
        private ItemModel.ItemLayoutControlsFieldInteger ConvertControlsFieldInteger(RawLayoutConfigControlsFieldInteger srcInteger)
        {
            return new ItemModel.ItemLayoutControlsFieldInteger
            {
                MinValue = srcInteger.MinValue,
                MaxValue = srcInteger.MaxValue,
                Step = srcInteger.Step
            };
        }

        /// <summary>Converts a controls object.</summary>
        private ItemModel.ItemLayoutControlsFieldSwitch ConvertControlsFieldSwitch(RawLayoutConfigControlsFieldSwitch srcSwitch)
        {
            return new ItemModel.ItemLayoutControlsFieldSwitch
            {
                FalseLabel = srcSwitch.FalseLabel,
                TrueLabel = srcSwitch.TrueLabel
            };
        }

        /// <summary>Converts a controls object.</summary>
        private object ConvertControlsFieldTimer(RawLayoutConfigControlsFieldTimer srcTimer)
        {
            return new Dictionary<string, object>
            {
                { "AllowClear", srcTimer.AllowClear ?? true }
            };
        }
    }
}
