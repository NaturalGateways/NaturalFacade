using System;
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
            return new ApiDto.OverlayDto
            {
                imageResources = instance.m_resourcesByName.Values.Where(x => x.IsUsed && x.ResConfig.Type == "Image").ToDictionary(x => x.ResConfig.Name, y => y.ResConfig.Url),
                rootElement = rootElement
            };
        }

        /// <summary>The resources indexed by name and with an is-used boolean</summary>
        private Dictionary<string, Resource> m_resourcesByName = null;

        /// <summary>Constructor.</summary>
        private RawLayout2Overlay(RawLayoutConfig layoutConfig)
        {
            m_resourcesByName = layoutConfig.Resources.ToDictionary(x => x.Name, y => new Resource(y));
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

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertElement(RawLayoutConfigElement layoutElement)
        {
            if (layoutElement.Stack != null)
                return ConvertStackElement(layoutElement.Stack);
            if (layoutElement.VFloat != null)
                return ConvertVFloatElement(layoutElement.VFloat);
            if (layoutElement.Image != null)
                return ConvertImageElement(layoutElement.Image);
            throw new Exception("Unrecognized element type.");
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
                overlayObject.Add("marginLeft", marginLeft);
            if (marginRight != 0)
                overlayObject.Add("marginRight", marginRight);
            if (marginTop != 0)
                overlayObject.Add("marginTop", marginTop);
            if (marginBottom != 0)
                overlayObject.Add("marginBottom", marginBottom);
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
            if (layoutVFloat.Top != null)
                overlayObject.Add("middle", ConvertElement(layoutVFloat.Middle));
            if (layoutVFloat.Top != null)
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
        private Dictionary<string, object> ConvertImageElement(RawLayoutConfigElementImage layoutImage)
        {
            // Check resources exists
            if (m_resourcesByName.ContainsKey(layoutImage.Res) == false)
            {
                throw new Exception($"Resource '{layoutImage.Res}' is not defined.");
            }
            m_resourcesByName[layoutImage.Res].IsUsed = true;

            // Return object
            return new Dictionary<string, object>
            {
                { "elTyp", "Image" },
                { "fit", ConvertStringToImageFit(layoutImage.Fit).ToString() },
                { "res", layoutImage.Res }
            };
        }
    }
}
