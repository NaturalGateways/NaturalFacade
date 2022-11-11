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
            switch (Enum.Parse<RawLayoutConfigElementType>(layoutElement.ElementType))
            {
                case RawLayoutConfigElementType.Stack:
                    if (layoutElement.Stack == null)
                        throw new Exception("Stack element missing data.");
                    return ConvertStackElement(layoutElement.Stack);
                case RawLayoutConfigElementType.Image:
                    if (layoutElement.Image == null)
                        throw new Exception("Stack element missing data.");
                    return ConvertImageElement(layoutElement.Image);
                default:
                    throw new Exception("Unrecognized element type.");
            }
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

        /// <summary>Converts a string to an image fit enum.</summary>
        private RawLayoutConfigElementStackSizeType? ConvertStringToStackSizeType(string attName, string sizeTypeString)
        {
            if (string.IsNullOrEmpty(sizeTypeString))
            {
                return null;
            }
            RawLayoutConfigElementStackSizeType sizeTypeEnum = RawLayoutConfigElementStackSizeType.Fixed;
            if (Enum.TryParse<RawLayoutConfigElementStackSizeType>(sizeTypeString, out sizeTypeEnum))
            {
                return sizeTypeEnum;
            }
            throw new Exception($"Unrecognised stack size type for '{attName}': '{sizeTypeString}'");
        }

        /// <summary>Creates an overlay element from a layout element.</summary>
        private Dictionary<string, object> ConvertStackChildElement(RawLayoutConfigElementStackChild layoutStackChild)
        {
            // Get element
            Dictionary<string, object> overlayObject = ConvertElement(layoutStackChild.Element);

            // Add stack attributes
            RawLayoutConfigElementStackSizeType? widthType = ConvertStringToStackSizeType("WidthType", layoutStackChild.WidthType);
            RawLayoutConfigElementStackSizeType? heightType = ConvertStringToStackSizeType("HeightType", layoutStackChild.HeightType);
            RawLayoutConfigElementStackSizeType? marginLeftType = ConvertStringToStackSizeType("MarginLeftType", layoutStackChild.MarginLeftType);
            RawLayoutConfigElementStackSizeType? marginRightType = ConvertStringToStackSizeType("MarginRightType", layoutStackChild.MarginRightType);
            RawLayoutConfigElementStackSizeType? marginTopType = ConvertStringToStackSizeType("MarginTopType", layoutStackChild.MarginTopType);
            RawLayoutConfigElementStackSizeType? marginBottomType = ConvertStringToStackSizeType("MarginBottomType", layoutStackChild.MarginBottomType);
            if (widthType.HasValue && widthType.Value == RawLayoutConfigElementStackSizeType.Fixed)
            {
                overlayObject.Add("width", layoutStackChild.WidthPixels ?? 0);
            }
            else if (widthType.HasValue && widthType.Value != RawLayoutConfigElementStackSizeType.Max)
            {
                overlayObject.Add("width", widthType.Value.ToString());
            }
            if (heightType.HasValue && heightType.Value == RawLayoutConfigElementStackSizeType.Fixed)
            {
                overlayObject.Add("height", layoutStackChild.HeightPixels ?? 0);
            }
            else if (heightType.HasValue && heightType.Value != RawLayoutConfigElementStackSizeType.Max)
            {
                overlayObject.Add("height", heightType.Value.ToString());
            }
            if (marginLeftType.HasValue)
            {
                if (marginLeftType.Value != RawLayoutConfigElementStackSizeType.Fixed)
                {
                    overlayObject.Add("marginLeft", marginLeftType.Value.ToString());
                }
                else if (layoutStackChild.MarginLeftPixels.HasValue)
                {
                    overlayObject.Add("marginLeft", layoutStackChild.MarginLeftPixels.Value);
                }
            }
            if (marginRightType.HasValue)
            {
                if (marginRightType.Value != RawLayoutConfigElementStackSizeType.Fixed)
                {
                    overlayObject.Add("marginRight", marginRightType.Value.ToString());
                }
                else if (layoutStackChild.MarginRightPixels.HasValue)
                {
                    overlayObject.Add("marginRight", layoutStackChild.MarginRightPixels.Value);
                }
            }
            if (marginTopType.HasValue)
            {
                if (marginTopType.Value != RawLayoutConfigElementStackSizeType.Fixed)
                {
                    overlayObject.Add("marginTop", marginTopType.Value.ToString());
                }
                else if (layoutStackChild.MarginTopPixels.HasValue)
                {
                    overlayObject.Add("marginTop", layoutStackChild.MarginTopPixels.Value);
                }
            }
            if (marginBottomType.HasValue)
            {
                if (marginBottomType.Value != RawLayoutConfigElementStackSizeType.Fixed)
                {
                    overlayObject.Add("marginBottom", marginBottomType.Value.ToString());
                }
                else if (layoutStackChild.MarginBottomPixels.HasValue)
                {
                    overlayObject.Add("marginBottom", layoutStackChild.MarginBottomPixels.Value);
                }
            }
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
