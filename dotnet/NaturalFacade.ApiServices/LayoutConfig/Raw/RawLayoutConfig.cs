﻿using System;
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

    public class RawLayoutConfigElement
    {
        // Layouts
        public RawLayoutConfigElementHFloat HFloat { get; set; }
        public RawLayoutConfigElementRows Rows { get; set; }
        public RawLayoutConfigElementStack Stack { get; set; }
        public RawLayoutConfigElementVFloat VFloat { get; set; }

        // Elements
        public RawLayoutConfigElementColouredQuad ColouredQuad { get; set; }
        public RawLayoutConfigElementImage Image { get; set; }
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
}
