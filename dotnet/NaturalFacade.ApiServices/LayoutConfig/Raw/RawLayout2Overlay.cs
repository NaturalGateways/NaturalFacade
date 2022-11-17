﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.Raw
{
    public class RawLayout2Overlay
    {
        /// <summary>Converts the data.</summary>
        public static ApiDto.OverlayDto Convert(RawLayoutConfig layoutConfig)
        {
            // Create an instance
            RawLayout2Overlay instance = new RawLayout2Overlay(layoutConfig);

            // Get root element
            object rootElement = instance.ConvertElement(layoutConfig.RootElement);

            // Return
            ApiDto.OverlayDto overlay = new ApiDto.OverlayDto
            {
                rootElement = rootElement
            };
            if (instance.m_imageResourcesUsedList.Any())
                overlay.imageResources = instance.m_imageResourcesUsedList.Select(x => x.ResConfig.Url).ToArray();
            if (instance.m_fontResourcesUsedList.Any())
                overlay.fontResources = instance.m_fontResourcesUsedList.Select(x => x.ResConfig.Url).ToArray();
            if (instance.m_fontObjUsedList.Any())
                overlay.fonts = instance.m_fontObjUsedList.Select(x => new ApiDto.OverlayDtoFont
                {
                    res = x.ResIndex.Value,
                    size = x.Font.Size,
                    colour = x.Font.Colour,
                    align = x.Font.Align
                }).ToArray();
            return overlay;
        }

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
            m_imageResourcesByName = layoutConfig.Resources.Where(x => x.Type == "Image").Select(x => new ResourceRef(x)).ToDictionary(x => x.ResConfig.Name);
            m_fontResourcesByName = layoutConfig.Resources.Where(x => x.Type == "Font").Select(x => new ResourceRef(x)).ToDictionary(x => x.ResConfig.Name);
            m_fontObjsByName = layoutConfig.Fonts.Select(x => new FontObjResource(x)).ToDictionary(x => x.Font.Name);
        }

        /// <summary>A resource with a boolean flag for whether it is used or not.</summary>
        private class Resource
        {
            public RawLayoutConfigResource ResConfig { get; private set; }

            public bool IsUsed { get; set; } = false;

            public Resource(RawLayoutConfigResource resConfig)
            {
                this.ResConfig = resConfig;
            }
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
                { "fit", ConvertStringToImageFit(layoutImage.Fit).ToString() },
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

            // Return object
            return new Dictionary<string, object>
            {
                { "elTyp", "Text" },
                { "font", fontObj.FontIndex.Value },
                { "text", layoutText.Text }
            };
        }
    }
}
