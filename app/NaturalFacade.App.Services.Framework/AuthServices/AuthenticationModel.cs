using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.App
{
    public class AuthenticationModel : UI.Model
    {
        /// <summary>The current auth state.</summary>
        public AuthState AuthState { get; set; }
    }

    public class AuthState
    {
        /// <summary>The access variables to use with the API.</summary>
        public AuthStateApiAccess ApiAccess { get; set; }
    }

    public class AuthStateApiAccess
    {
        /// <summary>The ID token.</summary>
        public string IdToken { get; set; }

        /// <summary>The refresh token.</summary>
        public string RefreshToken { get; set; }

        /// <summary>The access token.</summary>
        public string AccessToken { get; set; }
    }
}
