﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig
{
    public static class Config2Layout
    {
        public static Config2LayoutResult Convert(LayoutConfig layoutConfig, ApiDto.PropertyDto[] properties)
        {
            // Create result
            Config2LayoutResult result = new Config2LayoutResult
            {
                Overlay = new ApiDto.OverlayDto
                {
                    canvasSize = new int[]
                    {
                        layoutConfig.Width ?? 1920,
                        layoutConfig.Height ?? 1080
                    }
                },
                Properties = properties
            };

            // Read specific type
            switch (layoutConfig.LayoutType)
            {
                case Raw.RawLayoutConfig.TYPENAME:
                    Raw.RawLayout2Overlay.Convert(result, layoutConfig.Raw);
                    break;
                default:
                    throw new Exception($"Cannot convert layout of type '{layoutConfig.LayoutType}'.");
            }

            // Compile property values
            result.PropertyValues = Properties2Values.GetValuesFromProperties(result.Properties);

            // Return
            return result;
        }
    }
}
