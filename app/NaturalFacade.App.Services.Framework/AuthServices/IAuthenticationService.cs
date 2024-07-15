using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.App
{
    public interface IAuthenticationService
    {
        /// <summary>The authentication model.</summary>
        AuthenticationModel AuthModel { get; }

        /// <summary>The URL to use for Cognito authentication.</summary>
        Uri CognitoUrl { get; }

        /// <summary>Checks if the given URL is a callback URL.</summary>
        bool IsCallbackUrl(Uri callbackUrl);

        /// <summary>Authenticates the user with the given code.</summary>
        void AuthenticateWithCognitoCode(string code);

        /// <summary>Logs the user out.</summary>
        void Logout();
    }
}
