using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.App.ApiModel
{
    public class ApiResponseContainer<PayloadType>
    {
        /// <summary>True if call was successful.</summary>
        public bool Success { get; set; }

        /// <summary>The payload of the API response.</summary>
        public PayloadType Payload { get; set; }

        /// <summary>If unsuccessful, the error message.</summary>
        public string Error { get; set; }

    }
}
