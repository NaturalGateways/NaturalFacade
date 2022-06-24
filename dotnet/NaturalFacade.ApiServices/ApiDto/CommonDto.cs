using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ApiDto
{
    public class CommonDto<PayloadType>
    {
        public ContextDto context { get; set; }

        public PayloadType payload { get; set; }
    }

    public class ContextDto
    {
        public string userId { get; set; }

        public string email { get; set; }

        public string sourceIp { get; set; }

        public string userAgent { get; set; }

        public string requestId { get; set; }
    }
}
