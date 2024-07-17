using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.App.ServiceImp
{
    public class AppAuthenticationService : IAuthenticationService
    {
        #region Base

        /// <summary>Constructor.</summary>
        public AppAuthenticationService()
        {
            ReleaseConfig.ReleaseConfig releaseConfig = Services.ReleaseConfigFileService.ReleaseConfig;
            this.CognitoUrl = new Uri($"https://naturalfacade.auth.ap-southeast-2.amazoncognito.com/login?response_type=code&scope=openid&client_id={releaseConfig.Cognito.ClientId}&redirect_uri={releaseConfig.Cognito.CallbackUrl}");
        }

        /// <summary>Authenticates the user with the given code, running in a background thread.</summary>
        public void OnAuthenticateWithCognitoCodeInBackgorundThread(string code)
        {
            ReleaseConfig.ReleaseConfig releaseConfig = Services.ReleaseConfigFileService.ReleaseConfig;

            AuthStateApiAccess appAccess = GetApiAccessFromCognito(releaseConfig, code);
            AuthStateUserDetails userDetails = GetUserDetailsFromApi(releaseConfig, appAccess);

            AuthenticationModel authModel = Services.AuthenticationService.AuthModel;
            authModel.AuthState = new AuthState
            {
                UserDetails = userDetails,
                ApiAccess = appAccess
            };
            authModel.OnDataChanged();
        }

        private class AuthResponseJson
        {
            public string id_token { get; set; }
            public string refresh_token { get; set; }
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }

        /// <summary>Getter for the API access tokens from Cognito.</summary>
        private AuthStateApiAccess GetApiAccessFromCognito(ReleaseConfig.ReleaseConfig releaseConfig, string code)
        {
            string url = $"https://naturalfacade.auth.ap-southeast-2.amazoncognito.com/oauth2/token?grant_type=authorization_code&client_id={releaseConfig.Cognito.ClientId}&code={code}&redirect_uri={releaseConfig.Cognito.CallbackUrl}";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent(string.Empty, Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage response = httpClient.Send(requestMessage);
                string responseString = response.Content.ReadAsStringAsync().Result;

                AuthResponseJson responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthResponseJson>(responseString);
                
                return new AuthStateApiAccess
                {
                    IdToken = responseObject.id_token,
                    RefreshToken = responseObject.refresh_token,
                    AccessToken = responseObject.access_token
                };
            }
        }

        /// <summary>Getter for the API access tokens from Cognito.</summary>
        private AuthStateUserDetails GetUserDetailsFromApi(ReleaseConfig.ReleaseConfig releaseConfig, AuthStateApiAccess apiAccess)
        {
            ApiModel.UserDetailsResponse apiResponse = Services.ApiService.GetUserDetails(apiAccess);
            return new AuthStateUserDetails
            {
                UserId = apiResponse.UserId,
                DisplayName = apiResponse.Name,
                Email = apiResponse.Email
            };
        }

        #endregion

        #region IAuthenticationService implementation

        /// <summary>The authentication model.</summary>
        public AuthenticationModel AuthModel { get; private set; } = new AuthenticationModel();

        /// <summary>The URL to use for Cognito authentication.</summary>
        public Uri CognitoUrl { get; private set; }

        /// <summary>Checks if the given URL is a callback URL.</summary>
        public bool IsCallbackUrl(Uri callbackUrl)
        {
            string domain = Services.ReleaseConfigFileService.ReleaseConfig.Cognito.Domain;
            return callbackUrl.Host == domain && callbackUrl.AbsolutePath == "/login";
        }

        /// <summary>Authenticates the user with the given code.</summary>
        public void AuthenticateWithCognitoCode(string code)
        {
            Thread backgroundThread = new Thread(() => { OnAuthenticateWithCognitoCodeInBackgorundThread(code); });
            backgroundThread.Name = "CognitoAuthThread";
            backgroundThread.Start();
        }

        /// <summary>Logs the user out.</summary>
        public void Logout()
        {
            this.AuthModel.AuthState = null;
            this.AuthModel.OnDataChanged();
        }

        #endregion
    }
}
