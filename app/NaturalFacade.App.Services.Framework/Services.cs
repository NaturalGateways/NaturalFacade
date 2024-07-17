using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.App
{
    public static class Services
    {
        public static IApiService ApiService { get; set; }

        public static IAuthenticationService AuthenticationService { get; set; }

        public static IReleaseConfigFileService ReleaseConfigFileService { get; set; }
    }
}
