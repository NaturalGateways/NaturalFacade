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
        private object ConvertElement(RawLayoutConfigElement layoutElement)
        {
            switch (Enum.Parse<RawLayoutConfigElementType>(layoutElement.ElementType))
            {
                case RawLayoutConfigElementType.Stack:
                    if (layoutElement.Stack == null)
                        return new Exception("Stack element missing data.");
                    return ConvertStackElement(layoutElement.Stack);
                case RawLayoutConfigElementType.Image:
                    if (layoutElement.Image == null)
                        return new Exception("Stack element missing data.");
                    return ConvertImageElement(layoutElement.Image);
                default:
                    return new Exception("Unrecognized element type.");
            }
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private object ConvertStackElement(RawLayoutConfigElementStack layoutStack)
        {
            return new Dictionary<string, object>
            {
                { "elTyp", "Stack" },
                { "children", layoutStack.Children.Select(x => ConvertStackChildElement(x)).ToArray() }
            };
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private object ConvertStackChildElement(RawLayoutConfigElementStackChild layoutStackChild)
        {
            Dictionary<string, object> overlayObject = new Dictionary<string, object>();
            if (layoutStackChild.WidthType.HasValue && layoutStackChild.WidthType.Value == RawLayoutConfigElementStackSizeType.Fixed)
            {
                overlayObject.Add("width", layoutStackChild.WidthPixels ?? 0);
            }
            else if (layoutStackChild.WidthType.HasValue && layoutStackChild.WidthType.Value != RawLayoutConfigElementStackSizeType.Max)
            {
                overlayObject.Add("width", layoutStackChild.WidthType.Value.ToString());
            }
            if (layoutStackChild.HeightType.HasValue && layoutStackChild.HeightType.Value == RawLayoutConfigElementStackSizeType.Fixed)
            {
                overlayObject.Add("height", layoutStackChild.HeightPixels ?? 0);
            }
            else if (layoutStackChild.HeightType.HasValue && layoutStackChild.HeightType.Value != RawLayoutConfigElementStackSizeType.Max)
            {
                overlayObject.Add("height", layoutStackChild.HeightType.Value.ToString());
            }
            if (layoutStackChild.MarginLeftType.HasValue)
            {
                if (layoutStackChild.MarginLeftType.Value != RawLayoutConfigElementStackSizeType.Fixed)
                {
                    overlayObject.Add("marginLeft", layoutStackChild.MarginLeftType.Value.ToString());
                }
                else if (layoutStackChild.MarginLeftPixels.HasValue)
                {
                    overlayObject.Add("marginLeft", layoutStackChild.MarginLeftPixels.Value);
                }
            }
            if (layoutStackChild.MarginRightType.HasValue)
            {
                if (layoutStackChild.MarginRightType.Value != RawLayoutConfigElementStackSizeType.Fixed)
                {
                    overlayObject.Add("marginRight", layoutStackChild.MarginRightType.Value.ToString());
                }
                else if (layoutStackChild.MarginRightPixels.HasValue)
                {
                    overlayObject.Add("marginRight", layoutStackChild.MarginRightPixels.Value);
                }
            }
            if (layoutStackChild.MarginTopType.HasValue)
            {
                if (layoutStackChild.MarginTopType.Value != RawLayoutConfigElementStackSizeType.Fixed)
                {
                    overlayObject.Add("marginTop", layoutStackChild.MarginTopType.Value.ToString());
                }
                else if (layoutStackChild.MarginTopPixels.HasValue)
                {
                    overlayObject.Add("marginTop", layoutStackChild.MarginTopPixels.Value);
                }
            }
            if (layoutStackChild.MarginBottomType.HasValue)
            {
                if (layoutStackChild.MarginBottomType.Value != RawLayoutConfigElementStackSizeType.Fixed)
                {
                    overlayObject.Add("marginBottom", layoutStackChild.MarginBottomType.Value.ToString());
                }
                else if (layoutStackChild.MarginBottomPixels.HasValue)
                {
                    overlayObject.Add("marginBottom", layoutStackChild.MarginBottomPixels.Value);
                }
            }
            overlayObject.Add("element", ConvertElement(layoutStackChild.Element));
            return overlayObject;
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private object ConvertImageElement(RawLayoutConfigElementImage layoutImage)
        {
            // Check resources exists
            if (m_resourcesByName.ContainsKey(layoutImage.Res) == false)
            {
                throw new Exception($"Resource '{layoutImage.Res}' is not defined.");
            }
            m_resourcesByName[layoutImage.Res].IsUsed = true;

            // Return object
            return new Dictionary<string, string>
            {
                { "elTyp", "Image" },
                { "fit", (layoutImage.Fit ?? RawLayoutConfigElementImageFit.None).ToString() },
                { "res", layoutImage.Res }
            };
        }
    }
}
