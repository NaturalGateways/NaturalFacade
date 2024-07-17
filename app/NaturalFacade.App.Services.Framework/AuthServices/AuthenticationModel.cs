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
        /// <summary>Information about the logged in user.</summary>
        public AuthStateUserDetails UserDetails { get; set; }

        /// <summary>The access variables to use with the API.</summary>
        public AuthStateApiAccess ApiAccess { get; set; }
    }

    public class AuthStateUserDetails
    {
        /// <summary>The ID of the user.</summary>
        public string UserId { get; set; }

        /// <summary>The name for the user to show.</summary>
        public string DisplayName { get; set; }

        /// <summary>The email.</summary>
        public string Email { get; set; }
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
