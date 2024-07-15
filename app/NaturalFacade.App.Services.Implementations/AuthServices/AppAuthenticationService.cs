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
            this.CognitoUrl = new Uri("https://naturalfacade.auth.ap-southeast-2.amazoncognito.com/login?response_type=code&scope=openid&client_id=TODO&redirect_uri=https://dev.naturalfacade.com/login");
        }

        private class AuthResponseJson
        {
            public string id_token { get; set; }
            public string refresh_token { get; set; }
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }

        /// <summary>Authenticates the user with the given code, running in a background thread.</summary>
        public void OnAuthenticateWithCognitoCodeInBackgorundThread(string code)
        {
            string url = $"https://naturalfacade.auth.ap-southeast-2.amazoncognito.com/oauth2/token?grant_type=authorization_code&client_id=TODO&code={code}&redirect_uri=https://dev.naturalfacade.com/login";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent(string.Empty, Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage response = httpClient.Send(requestMessage);
                string responseString = response.Content.ReadAsStringAsync().Result;

                AuthResponseJson responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthResponseJson>(responseString);

                AuthenticationModel authModel = Services.AuthenticationService.AuthModel;
                authModel.AuthState = new AuthState
                {
                    ApiAccess = new AuthStateApiAccess
                    {
                        IdToken = responseObject.id_token,
                        RefreshToken = responseObject.refresh_token,
                        AccessToken = responseObject.access_token
                    }
                };
                authModel.OnDataChanged();
            }
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
            return callbackUrl.Host == "dev.naturalfacade.com" && callbackUrl.AbsolutePath == "/login";
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
