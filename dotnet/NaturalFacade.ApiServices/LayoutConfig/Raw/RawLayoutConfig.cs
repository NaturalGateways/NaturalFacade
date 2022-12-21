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

        public RawLayoutConfigProperty[] Properties { get; set; }

        public RawLayoutConfigControls[] Controls { get; set; }

        public RawLayoutConfigResource[] Resources { get; set; }

        public RawLayoutConfigFont[] Fonts { get; set; }

        public RawLayoutConfigElement RootElement { get; set; }
    }

    public class RawLayoutConfigProperty
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public object DefaultValue { get; set; }
    }

    public class RawLayoutConfigControls
    {
        public string Name { get; set; }

        public RawLayoutConfigControlsField[] Fields { get; set; }
    }

    public class RawLayoutConfigControlsField
    {
        public string Label { get; set; }

        public string PropName { get; set; }

        public bool AllowTextEdit { get; set; } = false;

        public string[] Options { get; set; }
    }

    public class RawLayoutConfigResource
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }
    }

    public class RawLayoutConfigFont
    {
        public string Name { get; set; }

        public string FontRes { get; set; }

        public string Size { get; set; }

        public string Colour { get; set; }

        public string Align { get; set; }
    }

    public class RawLayoutConfigElement
    {
        // Common props
        public RawLayoutConfigOperation IsVisibleOp { get; set; }

        // Layouts
        public RawLayoutConfigElementHFloat HFloat { get; set; }
        public RawLayoutConfigElementRows Rows { get; set; }
        public RawLayoutConfigElementStack Stack { get; set; }
        public RawLayoutConfigElementVFloat VFloat { get; set; }

        // Elements
        public RawLayoutConfigElementColouredQuad ColouredQuad { get; set; }
        public RawLayoutConfigElementImage Image { get; set; }
        public RawLayoutConfigElementText Text { get; set; }
    }

    public class RawLayoutConfigElementHFloat
    {
        public RawLayoutConfigElement Left { get; set; }
        public RawLayoutConfigElement Middle { get; set; }
        public RawLayoutConfigElement Right { get; set; }
        public int? Spacing { get; set; }
        public int? Margin { get; set; }
        public int? MarginHorizontal { get; set; }
        public int? MarginVertical { get; set; }
        public int? MarginLeft { get; set; }
        public int? MarginRight { get; set; }
        public int? MarginTop { get; set; }
        public int? MarginBottom { get; set; }
    }

    public class RawLayoutConfigElementRows
    {
        public int? Spacing { get; set; }
        public RawLayoutConfigElement[] Children { get; set; }
    }

    public class RawLayoutConfigElementStack
    {
        public RawLayoutConfigElementStackChild[] Children { get; set; }
    }

    public enum RawLayoutConfigElementStackHAlignment
    {
        Left,
        Centre,
        Right,
        Fill
    }

    public enum RawLayoutConfigElementStackVAlignment
    {
        Top,
        Centre,
        Bottom,
        Fill
    }

    public class RawLayoutConfigElementStackChild
    {
        public string HAlign { get; set; }
        public int? WidthPixels { get; set; }
        public string VAlign { get; set; }
        public int? HeightPixels { get; set; }
        public int? Margin { get; set; }
        public int? MarginHorizontal { get; set; }
        public int? MarginVertical { get; set; }
        public int? MarginLeft { get; set; }
        public int? MarginRight { get; set; }
        public int? MarginTop { get; set; }
        public int? MarginBottom { get; set; }

        public RawLayoutConfigElement Element { get; set; }
    }

    public class RawLayoutConfigElementVFloat
    {
        public RawLayoutConfigElement Top { get; set; }
        public RawLayoutConfigElement Middle { get; set; }
        public RawLayoutConfigElement Bottom { get; set; }
        public int? Spacing { get; set; }
        public int? Margin { get; set; }
        public int? MarginHorizontal { get; set; }
        public int? MarginVertical { get; set; }
        public int? MarginLeft { get; set; }
        public int? MarginRight { get; set; }
        public int? MarginTop { get; set; }
        public int? MarginBottom { get; set; }
    }

    public class RawLayoutConfigElementColouredQuad
    {
        public int? Width { get; set; }
        public int? Height { get; set; }

        public string Hex { get; set; }
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

    public class RawLayoutConfigElementText
    {
        public string Font { get; set; }

        public string Text { get; set; }

        public RawLayoutConfigOperation TextOp { get; set; }
    }
}
