using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.App.ReleaseConfig
{
    public class ReleaseConfig
    {
        public ReleaseConfigCognito Cognito { get; set; }

        public string ApiUrl { get; set; }

    }

    public class ReleaseConfigCognito
    {
        public string ClientId { get; set; }

        public string Domain { get; set; }

        public string CallbackUrl { get; set; }
    }
}
