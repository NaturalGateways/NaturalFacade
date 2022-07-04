using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ApiDto
{
    public enum AnonRequestType
    {
        GetLayoutOverlay
    }

    public class AnonRequestDto
    {
        public string RequestType { get; set; }

        public string UserId { get; set; }

        public string LayoutId { get; set; }
    }
}
