using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig
{
    public static class Config2Layout
    {
        public static object Convert(LayoutConfig layoutConfig)
        {
            // Create overlay
            ApiDto.OverlayDto overlay = new ApiDto.OverlayDto
            {
                canvasSize = new int[]
                {
                    layoutConfig.Width ?? 1920,
                    layoutConfig.Height ?? 1080
                }
            };

            // Read specific type
            switch (layoutConfig.LayoutType)
            {
                case Raw.RawLayoutConfig.TYPENAME:
                    Raw.RawLayout2Overlay.Convert(overlay, layoutConfig.Raw);
                    break;
                default:
                    throw new Exception($"Cannot convert layout of type '{layoutConfig.LayoutType}'.");
            }

            // Return
            return overlay;
        }
    }
}
