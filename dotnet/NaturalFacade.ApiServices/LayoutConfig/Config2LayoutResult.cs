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

        public string[] AudioResources { get; set; }

        public ApiDto.OverlayDtoFont[] Fonts { get; set; }

        public ApiDto.OverlayDtoAudio[] Audios { get; set; }

        public object RootElement { get; set; }

        public Config2LayoutOverlayOutputControlsDef[] ControlsArray { get; set; }
    }

    /// <summary>The output of a converstion of a layout type to the common data to convert to an overlay and properties.</summary>
    public class Config2LayoutOverlayOutputPropertyDef
    {
        public ApiDto.PropertyTypeDto ValueType { get; set; }

        public string Name { get; set; }

        public long? TimerDirection { get; set; }

        public long? TimerMinValue { get; set; }

        public long? TimerMaxValue { get; set; }

        public object DefaultValue { get; set; }
    }

    /// <summary>The definition for controls.</summary>
    public class Config2LayoutOverlayOutputControlsDef
    {
        public string Name { get; set; }

        public bool SaveAll { get; set; } = false;

        public Config2LayoutOverlayOutputControlsFieldDef[] Fields { get; set; }
    }

    /// <summary>The definition for a control field.</summary>
    public class Config2LayoutOverlayOutputControlsFieldDef
    {
        public string Label { get; set; }

        public int PropIndex { get; set; }

        public Config2LayoutOverlayOutputControlsFieldAudioWalkmanDef AudioWalkman { get; set; }

        public object TextField { get; set; }

        public object SelectOptions { get; set; }

        public Config2LayoutOverlayOutputControlsFieldIntegerDef Integer { get; set; }

        public Config2LayoutOverlayOutputControlsFieldSwitchDef Switch { get; set; }

        public Config2LayoutOverlayOutputControlsFieldTimerDef Timer { get; set; }
    }

    /// <summary>The definition for an audio walkman control field.</summary>
    public class Config2LayoutOverlayOutputControlsFieldAudioWalkmanDef
    {
        public long AudioIndex { get; set; }
    }

    /// <summary>The definition for an integer control field.</summary>
    public class Config2LayoutOverlayOutputControlsFieldIntegerDef
    {
        public long Step { get; set; } = 1;

        public long? MinValue { get; set; }

        public long? MaxValue { get; set; }
    }

    /// <summary>The definition for an switch control field.</summary>
    public class Config2LayoutOverlayOutputControlsFieldSwitchDef
    {
        public string FalseLabel { get; set; }

        public string TrueLabel { get; set; }
    }

    /// <summary>The definition for an switch control field.</summary>
    public class Config2LayoutOverlayOutputControlsFieldTimerDef
    {
        public bool AllowClear { get; set; } = true;
    }

    public class Config2LayoutResult
    {
        public ApiDto.OverlayDto Overlay { get; set; }

        public ApiDto.PropertyDto[] Properties { get; set; }

        public object[] PropertyValues { get; set; }

        public Config2LayoutResultControls[] Controls { get; set; }
    }

    public class Config2LayoutResultControls
    {
        public string Name { get; set; }

        public bool SaveAll { get; set; } = false;

        public Config2LayoutResultControlsField[] Fields { get; set; }
    }

    public class Config2LayoutResultControlsField
    {
        public int PropIndex { get; set; }

        public string ValueType { get; set; }

        public string Label { get; set; }

        public object FieldDef { get; set; }

        public object DefaultValue { get; set; }
    }
}
