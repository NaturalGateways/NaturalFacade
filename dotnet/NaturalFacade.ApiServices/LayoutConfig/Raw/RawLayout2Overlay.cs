using System;
using System.Collections.Generic;
using System.Linq;

namespace NaturalFacade.LayoutConfig.Raw
{
    public class RawLayout2Overlay
    {
        #region Static facade

        /// <summary>Converts the data.</summary>
        public static Config2LayoutOverlayOutput Convert(object layoutConfig)
        {
            RawLayout2Overlay instance = new RawLayout2Overlay();
            string jsonString = Natural.Json.JsonHelper.SerialiseObject(layoutConfig);
            Natural.Json.IJsonObject jsonObject = Natural.Json.JsonHelper.JsonFromString(jsonString);
            instance.ReadJson(jsonObject);
            return instance.CreateOutput();
        }

        #endregion

        #region Base

        /// <summary>The read root element.</summary>
        private object m_rootElement = null;

        /// <summary>The array of controls.</summary>
        private Config2LayoutOverlayOutputControlsDef[] m_controlsArray = null;

        /// <summary>Creates an output object.</summary>
        public Config2LayoutOverlayOutput CreateOutput()
        {
            Config2LayoutOverlayOutput output = new Config2LayoutOverlayOutput
            {
                PropertyDefs = m_propertyRefsUsedList.Select(x => ConvertPropertyDef(x)).ToArray(),
                ImageResources = m_imageResourcesUsedList.Select(x => x.Url).ToArray(),
                FontResources = m_fontResourcesUsedList.Select(x => x.Url).ToArray(),
                Fonts = m_fontRefsUsedList.Select(x => ConvertFontReference(x)).Where(x => x != null).ToArray(),
                RootElement = m_rootElement,
                ControlsArray = m_controlsArray
            };
            return output;
        }

        /// <summary>converts a font to the API DTO.</summary>
        private Config2LayoutOverlayOutputPropertyDef ConvertPropertyDef(PropertyRef propertyRef)
        {
            return new Config2LayoutOverlayOutputPropertyDef
            {
                ValueType = propertyRef.Type,
                Name = propertyRef.Name,
                DefaultValue = propertyRef.DefaultValue,
                TimerMinValue = propertyRef.IntMinValue,
                TimerMaxValue = propertyRef.IntMaxValue,
                TimerDirection = propertyRef.IntDirection
            };
        }

        /// <summary>converts a font to the API DTO.</summary>
        private ApiDto.OverlayDtoFont ConvertFontReference(FontRef fontRef)
        {
            if (fontRef.FontRefIndex.HasValue && fontRef.FontResourceIndex.HasValue)
            {
                return new ApiDto.OverlayDtoFont
                {
                    res = fontRef.FontResourceIndex.Value,
                    size = fontRef.Size,
                    colour = fontRef.Colour,
                    align = fontRef.Align
                };
            }
            return null;
        }

        #endregion

        #region Reference tracking

        #region Property tracking

        /// <summary>A property referenced in the file.</summary>
        private class PropertyRef
        {
            public string Name { get; set; }
            public ApiDto.PropertyTypeDto Type { get; set; }

            public object DefaultValue { get; set; }

            public long? IntMinValue { get; set; }
            public long? IntMaxValue { get; set; }
            public long? IntDirection { get; set; }

            public int? PropIndex { get; set; }

            public PropertyRef(string name, ApiDto.PropertyTypeDto type, object defaultValue)
            {
                this.Name = name;
                this.Type = type;
                this.DefaultValue = defaultValue;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, PropertyRef> m_propertyRefsByName = new Dictionary<string, PropertyRef>();
        /// <summary>The resources indexed.</summary>
        private List<PropertyRef> m_propertyRefsUsedList = new List<PropertyRef>();

        /// <summary>Getter for the property with the given name, marking it as used.</summary>
        private PropertyRef GetPropertyFromName(string name)
        {
            if (m_propertyRefsByName.ContainsKey(name) == false)
            {
                throw new Exception($"Undefined property '{name}'.");
            }
            PropertyRef property = m_propertyRefsByName[name];
            if (property.PropIndex.HasValue == false)
            {
                property.PropIndex = m_propertyRefsUsedList.Count;
                m_propertyRefsUsedList.Add(property);
            }
            return property;
        }

        #endregion

        #region Font reference tracking

        /// <summary>An font referenced in the file.</summary>
        private class FontRef
        {
            public string FontRes { get; set; }

            public string Size { get; set; }

            public string Colour { get; set; }

            public string Align { get; set; }

            public int? FontRefIndex { get; set; }
            public int? FontResourceIndex { get; set; }

            public FontRef(string fontRes, string size, string colour, string align)
            {
                this.FontRes = fontRes;
                this.Size = size;
                this.Colour = colour;
                this.Align = align;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, FontRef> m_fontRefsByName = new Dictionary<string, FontRef>();
        /// <summary>The resources indexed.</summary>
        private List<FontRef> m_fontRefsUsedList = new List<FontRef>();

        #endregion

        #region Font resource tracking

        /// <summary>An font referenced in the file.</summary>
        private class FontResource
        {
            public string Url { get; private set; }

            public int? ResIndex { get; set; }

            public FontResource(string url)
            {
                this.Url = url;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, FontResource> m_fontResourcesByName = new Dictionary<string, FontResource>();
        /// <summary>The resources indexed.</summary>
        private List<FontResource> m_fontResourcesUsedList = new List<FontResource>();

        #endregion

        #region Image resource tracking

        /// <summary>An image referenced in the file.</summary>
        private class ImageResource
        {
            public string Url { get; private set; }

            public int? ResIndex { get; set; }

            public ImageResource(string url)
            {
                this.Url = url;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, ImageResource> m_imageResourcesByName = new Dictionary<string, ImageResource>();
        /// <summary>The resources indexed.</summary>
        private List<ImageResource> m_imageResourcesUsedList = new List<ImageResource>();

        #endregion

        #endregion

        #region JSON Reading

        /// <summary>Reads the layout JSON.</summary>
        public void ReadJson(Natural.Json.IJsonObject layoutJson)
        {
            // Get property references
            {
                Natural.Json.IJsonObject resourcesElement = layoutJson.GetDictionaryObject("Properties");
                if (resourcesElement.ObjectType == Natural.Json.JsonObjectType.Array)
                {
                    ReadPropertyReferences(resourcesElement);
                }
            }

            // Get resource references
            {
                Natural.Json.IJsonObject resourcesElement = layoutJson.GetDictionaryObject("Resources");
                if (resourcesElement.ObjectType == Natural.Json.JsonObjectType.Array)
                {
                    ReadResourceReferences(resourcesElement);
                }
            }

            // Get font references
            {
                Natural.Json.IJsonObject fontsJson = layoutJson.GetDictionaryObject("Fonts");
                if (fontsJson.ObjectType == Natural.Json.JsonObjectType.Array)
                {
                    ReadFontReferences(fontsJson);
                }
            }

            // Get the root element
            {
                Natural.Json.IJsonObject rootElement = layoutJson.GetDictionaryObject("RootElement");
                if (rootElement.ObjectType == Natural.Json.JsonObjectType.Dictionary)
                {
                    m_rootElement = ReadElement(rootElement);
                }
            }

            // Get the controls
            {
                Natural.Json.IJsonObject controlsElement = layoutJson.GetDictionaryObject("Controls");
                m_controlsArray = controlsElement.AsObjectArray.Select(x => ConvertControls(x)).ToArray();
            }
        }

        #endregion

        #region Reference reading

        /// <summary>Reads the image resources.</summary>
        private void ReadPropertyReferences(Natural.Json.IJsonObject arrayJson)
        {
            foreach (Natural.Json.IJsonObject resourceJson in arrayJson.AsObjectArray)
            {
                ReadPropertyReference(resourceJson);
            }
        }

        /// <summary>Reads the image resources.</summary>
        private void ReadPropertyReference(Natural.Json.IJsonObject resourceJson)
        {
            string propName = resourceJson.GetDictionaryString("Name");
            ApiDto.PropertyTypeDto type = ApiDto.PropertyTypeDto.String;
            object defaultValue = null;
            switch (resourceJson.GetDictionaryString("Type"))
            {
                case "String":
                    type = ApiDto.PropertyTypeDto.String;
                    defaultValue = resourceJson.GetDictionaryString("DefaultValue");
                    break;
                case "Boolean":
                    type = ApiDto.PropertyTypeDto.Boolean;
                    defaultValue = resourceJson.GetDictionaryBoolean("DefaultValue");
                    break;
                case "Timer":
                    {
                        type = ApiDto.PropertyTypeDto.Timer;
                        long? secsDefaultValue = resourceJson.GetDictionaryLong("DefaultValue");
                        if (secsDefaultValue.HasValue)
                            defaultValue = new Dictionary<string, object> { { "Secs", secsDefaultValue.Value } };
                        break;
                    }
            }
            PropertyRef propertyRef = new PropertyRef(propName, type, defaultValue);
            propertyRef.IntMinValue = resourceJson.GetDictionaryLong("MinValue");
            propertyRef.IntMaxValue = resourceJson.GetDictionaryLong("MaxValue");
            propertyRef.IntDirection = resourceJson.GetDictionaryLong("Direction");
            m_propertyRefsByName.Add(propName, propertyRef);
        }

        /// <summary>Reads the image resources.</summary>
        private void ReadResourceReferences(Natural.Json.IJsonObject arrayJson)
        {
            foreach (Natural.Json.IJsonObject resourceJson in arrayJson.AsObjectArray)
            {
                ReadResourceReference(resourceJson);
            }
        }

        /// <summary>Reads the image resources.</summary>
        private void ReadResourceReference(Natural.Json.IJsonObject resourceJson)
        {
            string resName = resourceJson.GetDictionaryString("Name");
            switch (resourceJson.GetDictionaryString("Type"))
            {
                case "Font":
                    {
                        string url = resourceJson.GetDictionaryString("Url");
                        m_fontResourcesByName.Add(resName, new FontResource(url));
                        break;
                    }
                case "Image":
                    {
                        string url = resourceJson.GetDictionaryString("Url");
                        m_imageResourcesByName.Add(resName, new ImageResource(url));
                        break;
                    }
            }
        }

        /// <summary>Reads the image resources.</summary>
        private void ReadFontReferences(Natural.Json.IJsonObject arrayJson)
        {
            foreach (Natural.Json.IJsonObject resourceJson in arrayJson.AsObjectArray)
            {
                ReadFontReference(resourceJson);
            }
        }

        /// <summary>Reads the image resources.</summary>
        private void ReadFontReference(Natural.Json.IJsonObject resourceJson)
        {
            string resName = resourceJson.GetDictionaryString("Name");
            string fontRes = resourceJson.GetDictionaryString("FontRes");
            string size = resourceJson.GetDictionaryString("Size");
            string colour = resourceJson.GetDictionaryString("Colour");
            string align = resourceJson.GetDictionaryString("Align");
            m_fontRefsByName.Add(resName, new FontRef(fontRes, size, colour, align));
        }

        #endregion

        #region Operation reading

        #region String operation reading

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertStringOperation(Natural.Json.IJsonObject conditionJson)
        {
            string op = conditionJson.GetDictionaryString("Op");
            switch (op)
            {
                case "Text":
                    {
                        string text = conditionJson.GetDictionaryString("Text");
                        return new Dictionary<string, object>
                        {
                            { "op", op },
                            { "text", text ?? string.Empty }
                        };
                    }
                case "Prop":
                    {
                        string name = conditionJson.GetDictionaryString("Name");
                        PropertyRef property = GetPropertyFromName(name);
                        return new Dictionary<string, object>
                        {
                            { "op", "Prop" },
                            { "index", property.PropIndex.Value }
                        };
                    }
                case "Cat":
                    {
                        Natural.Json.IJsonObject[] childJsonArray = conditionJson.GetDictionaryObject("Children").AsObjectArray;
                        if ((childJsonArray?.Any() ?? false) == false)
                            throw new Exception("'Cat' string operation must have children.");
                        return new Dictionary<string, object>
                        {
                            { "op", op },
                            { "items", childJsonArray.Select(x => ConvertStringOperation(x)).ToArray() }
                        };
                    }
                case "If":
                    {
                        Natural.Json.IJsonObject ifJson = conditionJson.GetDictionaryObject("If");
                        Natural.Json.IJsonObject thenJson = conditionJson.GetDictionaryObject("Then");
                        Natural.Json.IJsonObject elseJson = conditionJson.GetDictionaryObject("Else");
                        if (ifJson.ObjectType == Natural.Json.JsonObjectType.Null)
                            throw new Exception("'If' string operation must have an 'If' condition.");
                        if (thenJson.ObjectType == Natural.Json.JsonObjectType.Null)
                            throw new Exception("'If' string operation must have an 'Then' operation.");
                        Dictionary<string, object> opOutput = new Dictionary<string, object>
                        {
                            { "op", op },
                            { "if", ConvertBooleanCondition(ifJson) },
                            { "then", ConvertStringOperation(thenJson) }
                        };
                        if (elseJson.ObjectType != Natural.Json.JsonObjectType.Null)
                            opOutput.Add("else", ConvertStringOperation(elseJson));
                        return opOutput;
                    }
                default:
                    throw new Exception($"Unknown operation type '{op}'.");
            }
        }

        #endregion

        #region Integer operation reading

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertIntegerOperation(Natural.Json.IJsonObject conditionJson)
        {
            string op = conditionJson.GetDictionaryString("Op");
            switch (op)
            {
                case "Value":
                    {
                        long value = conditionJson.GetDictionaryLong("Value") ?? 0;
                        return new Dictionary<string, object>
                        {
                            { "op", "Value" },
                            { "value", value }
                        };
                    }
                case "Prop":
                    {
                        string name = conditionJson.GetDictionaryString("Name");
                        PropertyRef property = GetPropertyFromName(name);
                        return new Dictionary<string, object>
                        {
                            { "op", op },
                            { "index", property.PropIndex.Value }
                        };
                    }
                case "Add":
                case "Subtract":
                case "Multiply":
                case "Divide":
                case "Modulo":
                    {
                        Natural.Json.IJsonObject lhsJson = conditionJson.GetDictionaryObject("Lhs");
                        Natural.Json.IJsonObject rhsJson = conditionJson.GetDictionaryObject("Rhs");
                        if (lhsJson.ObjectType == Natural.Json.JsonObjectType.Null)
                            throw new Exception($"'{op}' integer conditions must have a lhs.");
                        if (rhsJson.ObjectType == Natural.Json.JsonObjectType.Null)
                            throw new Exception($"'{op}' integer conditions must have a rhs.");
                        return new Dictionary<string, object>
                        {
                            { "op", op },
                            { "lhs", ConvertIntegerOperation(lhsJson) },
                            { "rhs", ConvertIntegerOperation(rhsJson) }
                        };
                    }
                default:
                    throw new Exception($"Unknown operation type '{op}'.");
            }
        }

        #endregion

        #region Boolean operation reading

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertBooleanCondition(Natural.Json.IJsonObject conditionJson)
        {
            string op = conditionJson.GetDictionaryString("Op");
            switch (op)
            {
                case "Prop":
                    {
                        string name = conditionJson.GetDictionaryString("Name");
                        PropertyRef property = GetPropertyFromName(name);
                        return new Dictionary<string, object>
                        {
                            { "op", op },
                            { "index", property.PropIndex.Value }
                        };
                    }
                case "And":
                case "Or":
                    {
                        Natural.Json.IJsonObject[] childJsonArray = conditionJson.GetDictionaryObject("Children").AsObjectArray;
                        if ((childJsonArray?.Any() ?? false) == false)
                            throw new Exception($"'{op}' boolean conditions must have children.");
                        return new Dictionary<string, object>
                        {
                            { "op", op },
                            { "items", childJsonArray.Select(x => ConvertBooleanCondition(x)).ToArray() }
                        };
                    }
                case "Not":
                    {
                        Natural.Json.IJsonObject childJson = conditionJson.GetDictionaryObject("Child");
                        if (childJson.ObjectType != Natural.Json.JsonObjectType.Dictionary)
                            throw new Exception("'Not' boolean conditions must have a child.");
                        return new Dictionary<string, object>
                        {
                            { "op", op },
                            { "item", ConvertBooleanCondition(childJson) }
                        };
                    }
                case "IntLessThan":
                case "IntLessThanEquals":
                case "IntGreaterThan":
                case "IntGreaterThanEquals":
                    {
                        Natural.Json.IJsonObject lhsJson = conditionJson.GetDictionaryObject("Lhs");
                        Natural.Json.IJsonObject rhsJson = conditionJson.GetDictionaryObject("Rhs");
                        if (lhsJson.ObjectType == Natural.Json.JsonObjectType.Null)
                            throw new Exception($"'{op}' boolean conditions must have a lhs.");
                        if (rhsJson.ObjectType == Natural.Json.JsonObjectType.Null)
                            throw new Exception($"'{op}' boolean conditions must have a rhs.");
                        return new Dictionary<string, object>
                    {
                        { "op", op },
                        { "lhs", ConvertIntegerOperation(lhsJson) },
                        { "rhs", ConvertIntegerOperation(rhsJson) }
                    };
                    }
                default:
                    throw new Exception($"Unknown operation type '{op}'.");
            }
        }

        #endregion

        #endregion

        #region JSON Element Reading

        /// <summary>Reads an element object.</summary>
        public Dictionary<string, object> ReadElement(Natural.Json.IJsonObject elementJson)
        {
            // Convert for specific type
            Dictionary<string, object> overlayObject = ReadElementType(elementJson);

            // Check rules
            Natural.Json.IJsonObject isVisibleJson = elementJson.GetDictionaryObject("IsVisibleOp");
            if (isVisibleJson.ObjectType == Natural.Json.JsonObjectType.Dictionary)
                overlayObject.Add("isVisible", ConvertBooleanCondition(isVisibleJson));

            // Return
            return overlayObject;
        }

        /// <summary>Reads an element object.</summary>
        public Dictionary<string, object> ReadElementType(Natural.Json.IJsonObject elementJson)
        {
            string elementType = elementJson.GetDictionaryString("ElementType");
            switch (elementType)
            {
                // Element types
                case "ColouredQuad":
                    return ReadElementColouredQuad(elementJson);
                case "Image":
                    return ReadElementImage(elementJson);
                case "Text":
                    return ReadElementText(elementJson);
                // Layout types
                case "HFloat":
                    return ReadElementHFloat(elementJson);
                case "Rows":
                    return ReadElementRows(elementJson);
                case "Stack":
                    return ReadElementStack(elementJson);
                case "VFloat":
                    return ReadElementVFloat(elementJson);
            }
            return null;
        }

        /// <summary>Reads an element object.</summary>
        public Dictionary<string, object> ReadElementHFloat(Natural.Json.IJsonObject elementJson)
        {
            long spacing = elementJson.GetDictionaryLong("Spacing") ?? 0;
            long margin = elementJson.GetDictionaryLong("Margin") ?? 0;
            long marginHorizontal = elementJson.GetDictionaryLong("MarginHorizontal") ?? margin;
            long marginVertical = elementJson.GetDictionaryLong("MarginVertical") ?? margin;
            long marginLeft = elementJson.GetDictionaryLong("MarginLeft") ?? marginHorizontal;
            long marginRight = elementJson.GetDictionaryLong("MarginRight") ?? marginHorizontal;
            long marginTop = elementJson.GetDictionaryLong("MarginTop") ?? marginVertical;
            long marginBottom = elementJson.GetDictionaryLong("MarginBottom") ?? marginVertical;
            Natural.Json.IJsonObject leftElement = elementJson.GetDictionaryObject("Left");
            Natural.Json.IJsonObject middleElement = elementJson.GetDictionaryObject("Middle");
            Natural.Json.IJsonObject rightElement = elementJson.GetDictionaryObject("Right");
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
            if (leftElement.ObjectType == Natural.Json.JsonObjectType.Dictionary)
                overlayObject.Add("left", ReadElement(leftElement));
            if (middleElement.ObjectType == Natural.Json.JsonObjectType.Dictionary)
                overlayObject.Add("middle", ReadElement(middleElement));
            if (rightElement.ObjectType == Natural.Json.JsonObjectType.Dictionary)
                overlayObject.Add("right", ReadElement(rightElement));
            return overlayObject;
        }

        /// <summary>Reads an element object.</summary>
        public Dictionary<string, object> ReadElementRows(Natural.Json.IJsonObject elementJson)
        {
            // Get defaults
            long spacing = elementJson.GetDictionaryLong("Spacing") ?? 0;

            // Get list of children
            List<object> childList = new List<object>();
            foreach (Natural.Json.IJsonObject childJson in elementJson.GetDictionaryObject("Children").AsObjectArray)
            {
                object childObject = ReadElement(childJson);
                if (childObject != null)
                {
                    childList.Add(childObject);
                }
            }

            // Create data
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "elTyp", "Rows" },
                { "children", childList }
            };
            if (spacing != 0)
                data.Add("spacing", spacing);
            return data;
        }

        /// <summary>Reads an element object.</summary>
        public Dictionary<string, object> ReadElementStack(Natural.Json.IJsonObject elementJson)
        {
            // Get list of children
            List<object> childList = new List<object>();
            foreach (Natural.Json.IJsonObject childJson in elementJson.GetDictionaryObject("Children").AsObjectArray)
            {
                object childObject = ReadElementStackChild(childJson);
                if (childObject != null)
                {
                    childList.Add(childObject);
                }
            }

            // Return
            return new Dictionary<string, object>
            {
                { "elTyp", "Stack" },
                { "children", childList }
            };
        }

        /// <summary>Reads an element object.</summary>
        public Dictionary<string, object> ReadElementStackChild(Natural.Json.IJsonObject stackChildJson)
        {
            // Get element
            Dictionary<string, object> overlayObject = ReadElement(stackChildJson);
            if (overlayObject == null)
            {
                return null;
            }

            // Add stack attributes
            RawLayoutConfigElementStackHAlignment hAlign = ConvertStringToStackHAlign(stackChildJson.GetDictionaryString("Stack.HAlign"), RawLayoutConfigElementStackHAlignment.Fill);
            RawLayoutConfigElementStackVAlignment vAlign = ConvertStringToStackVAlign(stackChildJson.GetDictionaryString("Stack.VAlign"), RawLayoutConfigElementStackVAlignment.Fill);
            long widthPixels = stackChildJson.GetDictionaryLong("Stack.WidthPixels") ?? 0;
            long heightPixels = stackChildJson.GetDictionaryLong("Stack.HeightPixels") ?? 0;
            long? margin = stackChildJson.GetDictionaryLong("Stack.Margin");
            long? marginHorizontal = stackChildJson.GetDictionaryLong("Stack.MarginHorizontal") ?? margin;
            long? marginVertical = stackChildJson.GetDictionaryLong("Stack.MarginVertical") ?? margin;
            long marginLeft = stackChildJson.GetDictionaryLong("Stack.MarginLeft") ?? marginHorizontal ?? 0;
            long marginRight = stackChildJson.GetDictionaryLong("Stack.MarginRight") ?? marginHorizontal ?? 0;
            long marginTop = stackChildJson.GetDictionaryLong("Stack.MarginTop") ?? marginVertical ?? 0;
            long marginBottom = stackChildJson.GetDictionaryLong("Stack.MarginBottom") ?? marginVertical ?? 0;
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

        /// <summary>Reads an element object.</summary>
        public Dictionary<string, object> ReadElementVFloat(Natural.Json.IJsonObject elementJson)
        {
            long spacing = elementJson.GetDictionaryLong("Spacing") ?? 0;
            long margin = elementJson.GetDictionaryLong("Margin") ?? 0;
            long marginHorizontal = elementJson.GetDictionaryLong("MarginHorizontal") ?? margin;
            long marginVertical = elementJson.GetDictionaryLong("MarginVertical") ?? margin;
            long marginLeft = elementJson.GetDictionaryLong("MarginLeft") ?? marginHorizontal;
            long marginRight = elementJson.GetDictionaryLong("MarginRight") ?? marginHorizontal;
            long marginTop = elementJson.GetDictionaryLong("MarginTop") ?? marginVertical;
            long marginBottom = elementJson.GetDictionaryLong("MarginBottom") ?? marginVertical;
            Natural.Json.IJsonObject topElement = elementJson.GetDictionaryObject("Top");
            Natural.Json.IJsonObject middleElement = elementJson.GetDictionaryObject("Middle");
            Natural.Json.IJsonObject bottomElement = elementJson.GetDictionaryObject("Bottom");
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
            if (topElement.ObjectType == Natural.Json.JsonObjectType.Dictionary)
                overlayObject.Add("top", ReadElement(topElement));
            if (middleElement.ObjectType == Natural.Json.JsonObjectType.Dictionary)
                overlayObject.Add("middle", ReadElement(middleElement));
            if (bottomElement.ObjectType == Natural.Json.JsonObjectType.Dictionary)
                overlayObject.Add("bottom", ReadElement(bottomElement));
            return overlayObject;
        }

        /// <summary>Reads an element object.</summary>
        public Dictionary<string, object> ReadElementColouredQuad(Natural.Json.IJsonObject elementJson)
        {
            long? width = elementJson.GetDictionaryLong("Width");
            long? height = elementJson.GetDictionaryLong("Height");
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "elTyp", "ColouredQuad" },
                { "hex", elementJson.GetDictionaryString("Hex") }
            };
            if (width.HasValue)
                data.Add("width", width.Value);
            if (height.HasValue)
                data.Add("height", height.Value);
            return data;
        }

        /// <summary>Reads an element object.</summary>
        public Dictionary<string, object> ReadElementImage(Natural.Json.IJsonObject elementJson)
        {
            // Get data
            string fit = elementJson.GetDictionaryString("Fit");
            string hFit = elementJson.GetDictionaryString("HFit");
            string vFit = elementJson.GetDictionaryString("VFit");

            // Check image res exists
            string imageName = elementJson.GetDictionaryString("Res");
            if (m_imageResourcesByName.ContainsKey(imageName) == false)
            {
                throw new Exception($"Image resource '{imageName}' is not defined.");
            }
            ImageResource imageFile = m_imageResourcesByName[imageName];
            // Check if we mark font as used
            if (imageFile.ResIndex.HasValue == false)
            {
                imageFile.ResIndex = m_imageResourcesUsedList.Count;
                m_imageResourcesUsedList.Add(imageFile);
            }

            // Return
            return new Dictionary<string, object>
            {
                { "elTyp", "Image" },
                { "hfit", ConvertStringToImageFit(hFit ?? fit).ToString() },
                { "vfit", ConvertStringToImageFit(vFit ?? fit).ToString() },
                { "res", imageFile.ResIndex.Value }
            };
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

        /// <summary>Reads an element object.</summary>
        public Dictionary<string, object> ReadElementText(Natural.Json.IJsonObject elementJson)
        {
            // Check font reference exists
            string fontName = elementJson.GetDictionaryString("Font");
            if (m_fontRefsByName.ContainsKey(fontName) == false)
            {
                throw new Exception($"Font reference '{fontName}' is not defined.");
            }
            FontRef fontRef = m_fontRefsByName[fontName];
            // Check if we mark font as used
            if (fontRef.FontRefIndex.HasValue == false)
            {
                fontRef.FontRefIndex = m_fontRefsUsedList.Count;
                m_fontRefsUsedList.Add(fontRef);
            }

            // Check font res exists
            if (m_fontResourcesByName.ContainsKey(fontRef.FontRes) == false)
            {
                throw new Exception($"Font resource '{fontRef.FontRes}' is not defined.");
            }
            FontResource fontResource = m_fontResourcesByName[fontRef.FontRes];
            // Check if we mark font as used
            if (fontResource.ResIndex.HasValue == false)
            {
                fontResource.ResIndex = m_fontResourcesUsedList.Count;
                m_fontResourcesUsedList.Add(fontResource);
            }
            // Set resource index on font config
            fontRef.FontResourceIndex = fontResource.ResIndex;

            // Create object
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "elTyp", "Text" },
                { "font", fontRef.FontRefIndex.Value }
            };

            // Add text or text parameter
            Natural.Json.IJsonObject textOp = elementJson.GetDictionaryObject("TextOp");
            if (textOp.ObjectType == Natural.Json.JsonObjectType.Dictionary)
            {
                data.Add("text", ConvertStringOperation(textOp));
            }
            else
            {
                data.Add("text", new Dictionary<string, object>
                {
                    { "op", "Text" },
                    { "text", elementJson.GetDictionaryString("Text") }
                });
            }

            // Return object
            return data;
        }

        #endregion

        #region Controls reading

        /// <summary>Converts a controls object.</summary>
        private Config2LayoutOverlayOutputControlsDef ConvertControls(Natural.Json.IJsonObject controlsElement)
        {
            return new Config2LayoutOverlayOutputControlsDef
            {
                Name = controlsElement.GetDictionaryString("Name"),
                SaveAll = controlsElement.GetDictionaryBoolean("SaveAll") ?? false,
                Fields = controlsElement.GetDictionaryObject("Fields").AsObjectArray.Select(x => ConvertControlsField(x)).Where(x => x != null).ToArray()
            };
        }

        /// <summary>Converts a controls object.</summary>
        private Config2LayoutOverlayOutputControlsFieldDef ConvertControlsField(Natural.Json.IJsonObject fieldElement)
        {
            // Get property
            string propName = fieldElement.GetDictionaryString("PropName");
            if (string.IsNullOrEmpty(propName))
            {
                throw new Exception("Prop name for a controls field must be provided.");
            }
            if (m_propertyRefsByName.ContainsKey(propName) == false)
            {
                throw new Exception($"Prop name '{propName}' not found.");
            }
            PropertyRef propertyRef = m_propertyRefsByName[propName];
            if (propertyRef.PropIndex.HasValue == false)
            {
                return null;
            }

            // Create field
            string label = fieldElement.GetDictionaryString("Label");
            Config2LayoutOverlayOutputControlsFieldDef destField = new Config2LayoutOverlayOutputControlsFieldDef
            {
                Label = label ?? propName,
                PropIndex = propertyRef.PropIndex.Value
            };
            bool allowTextEdit = fieldElement.GetDictionaryBoolean("AllowTextEdit") ?? false;
            if (allowTextEdit)
                destField.TextField = new object();
            destField.SelectOptions = ConvertControlsFieldSelectOptions(fieldElement.GetDictionaryObject("Options"));
            destField.Integer = ConvertControlsFieldInteger(fieldElement.GetDictionaryObject("Integer"));
            destField.Switch = ConvertControlsFieldSwitch(fieldElement.GetDictionaryObject("Switch"));
            destField.Timer = ConvertControlsFieldTimer(fieldElement.GetDictionaryObject("Timer"));
            return destField;
        }

        /// <summary>Converts a controls object.</summary>
        private object ConvertControlsFieldSelectOptions(Natural.Json.IJsonObject fieldJson)
        {
            Natural.Json.IJsonObject[] optionJsonArray = fieldJson.AsObjectArray;
            if (optionJsonArray?.Any() ?? false)
            {
                return optionJsonArray.Select(x => x.AsString).ToArray();
            }
            return null;
        }

        /// <summary>Converts a controls object.</summary>
        private Config2LayoutOverlayOutputControlsFieldIntegerDef ConvertControlsFieldInteger(Natural.Json.IJsonObject fieldJson)
        {
            if (fieldJson.ObjectType != Natural.Json.JsonObjectType.Dictionary)
                return null;
            return new Config2LayoutOverlayOutputControlsFieldIntegerDef
            {
                Step = fieldJson.GetDictionaryLong("Step") ?? 1,
                MinValue = fieldJson.GetDictionaryLong("MinValue"),
                MaxValue = fieldJson.GetDictionaryLong("MaxValue")
            };
        }

        /// <summary>Converts a controls object.</summary>
        private Config2LayoutOverlayOutputControlsFieldSwitchDef ConvertControlsFieldSwitch(Natural.Json.IJsonObject fieldJson)
        {
            if (fieldJson.ObjectType != Natural.Json.JsonObjectType.Dictionary)
                return null;
            return new Config2LayoutOverlayOutputControlsFieldSwitchDef
            {
                FalseLabel = fieldJson.GetDictionaryString("FalseLabel"),
                TrueLabel = fieldJson.GetDictionaryString("TrueLabel")
            };
        }

        /// <summary>Converts a controls object.</summary>
        private Config2LayoutOverlayOutputControlsFieldTimerDef ConvertControlsFieldTimer(Natural.Json.IJsonObject fieldJson)
        {
            if (fieldJson.ObjectType != Natural.Json.JsonObjectType.Dictionary)
                return null;
            return new Config2LayoutOverlayOutputControlsFieldTimerDef
            {
                AllowClear = fieldJson.GetDictionaryBoolean("AllowClear") ?? true
            };
        }

        #endregion
    }
}
