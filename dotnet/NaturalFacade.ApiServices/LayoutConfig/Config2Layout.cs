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
            switch (layoutConfig.LayoutType)
            {
                case Raw.RawLayoutConfig.TYPENAME:
                    return Raw.RawLayout2Overlay.Convert(layoutConfig.Raw);
                default:
                    throw new Exception($"Cannot convert layout of type '{layoutConfig.LayoutType}'.");
            }
        }
    }
}
