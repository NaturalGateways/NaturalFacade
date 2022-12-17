using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig
{
    public class Config2LayoutResult
    {
        public ApiDto.OverlayDto Overlay { get; set; }

        public ApiDto.PropertyDto[] Properties { get; set; }

        public object[] PropertyValues { get; set; }

        public ItemModel.ItemLayoutControlsData[] ControlsArray { get; set; }
    }
}
