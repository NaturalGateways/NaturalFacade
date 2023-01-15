using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig
{
    /// <summary>The output of a converstion of a layout type to the common data to convert to an overlay and properties.</summary>
    public class Config2LayoutOverlayOutput
    {
        public Config2LayoutOverlayOutputPropertyDef[] PropertyDefs { get; set; }

        public string[] ImageResources { get; set; }

        public string[] FontResources { get; set; }

        public ApiDto.OverlayDtoFont[] Fonts { get; set; }

        public object RootElement { get; set; }

        public ItemModel.ItemLayoutControlsData[] ControlsArray { get; set; }
    }

    /// <summary>The output of a converstion of a layout type to the common data to convert to an overlay and properties.</summary>
    public class Config2LayoutOverlayOutputPropertyDef
    {
        public ApiDto.PropertyTypeDto ValueType { get; set; }

        public string Name { get; set; }

        public int? TimerDirection { get; set; }

        public long? TimerMinValue { get; set; }

        public long? TimerMaxValue { get; set; }

        public object DefaultValue { get; set; }
    }

    public class Config2LayoutResult
    {
        public ApiDto.OverlayDto Overlay { get; set; }

        public Dictionary<string, object>[] Properties { get; set; }

        public object[] PropertyValues { get; set; }

        public ItemModel.ItemLayoutControlsData[] ControlsArray { get; set; }
    }
}
