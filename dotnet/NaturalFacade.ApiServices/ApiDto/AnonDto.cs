using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ApiDto
{
    public class AnonRequestDto
    {
        public AnonRequestPayloadDto payload { get; set; }
    }

    public enum AnonRequestType
    {
        GetInfo,
        GetLayoutOverlay,
        GetLayoutOverlayPropValues,
        ConvertLayoutToOverlay,
        PerformAction
    }

    public class AnonRequestPayloadDto
    {
        public string RequestType { get; set; }

        public string UserId { get; set; }

        public string LayoutId { get; set; }

        public string ActionName { get; set; }

        public LayoutConfig.LayoutConfig LayoutConfig { get; set; }
    }
}
