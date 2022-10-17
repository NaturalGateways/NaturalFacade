using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.Raw
{
    public class RawLayoutConfig
    {
        public const string TYPENAME = "Raw";

        public RawLayoutConfigResource[] Resources { get; set; }

        public RawLayoutConfigElement RootElement { get; set; }
    }

    public enum RawLayoutConfigResourceType
    {
        Image
    }

    public class RawLayoutConfigResource
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }
    }

    public enum RawLayoutConfigElementType
    {
        Stack,
        Image
    }

    public class RawLayoutConfigElement
    {
        public string ElementType { get; set; }

        public RawLayoutConfigElementStack Stack { get; set; }
        public RawLayoutConfigElementImage Image { get; set; }
    }

    public class RawLayoutConfigElementStack
    {
        public RawLayoutConfigElementStackChild[] Children { get; set; }
    }

    public enum RawLayoutConfigElementStackSizeType
    {
        Max,
        Min,
        Fixed
    }

    public class RawLayoutConfigElementStackChild
    {
        public RawLayoutConfigElementStackSizeType? WidthType { get; set; }
        public int? WidthPixels { get; set; }
        public RawLayoutConfigElementStackSizeType? HeightType { get; set; }
        public int? HeightPixels { get; set; }
        public RawLayoutConfigElementStackSizeType? MarginLeftType { get; set; }
        public int? MarginLeftPixels { get; set; }
        public RawLayoutConfigElementStackSizeType? MarginRightType { get; set; }
        public int? MarginRightPixels { get; set; }
        public RawLayoutConfigElementStackSizeType? MarginTopType { get; set; }
        public int? MarginTopPixels { get; set; }
        public RawLayoutConfigElementStackSizeType? MarginBottomType { get; set; }
        public int? MarginBottomPixels { get; set; }

        public RawLayoutConfigElement Element { get; set; }
    }

    public enum RawLayoutConfigElementImageFit
    {
        None,
        Tiled,
        Scaled
    }

    public class RawLayoutConfigElementImage
    {
        public string Fit { get; set; }

        public string Res { get; set; }
    }
}
