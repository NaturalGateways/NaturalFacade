using NaturalFacade.App.ReleaseConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.App.ServiceImp
{
    public class AppApiService : IApiService
    {
        #region API functions

        /// <summary>Common code to execute a request against the API.</summary>
        public PayloadType ExecuteAuthRequest<PayloadType>(HttpClient httpClient, AuthStateApiAccess apiAccess, object requestPayload)
        {
            // Get request as a string
            string requestPayloadString = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload);

            // Get URL
            string url = $"{Services.ReleaseConfigFileService.ReleaseConfig.ApiUrl}/auth";

            // Make request
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Content = new StringContent(requestPayloadString, Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(apiAccess.IdToken);
            HttpResponseMessage response = httpClient.Send(requestMessage);
            string responseString = response.Content.ReadAsStringAsync().Result;
            ApiModel.ApiResponseContainer<PayloadType> responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiModel.ApiResponseContainer<PayloadType>>(responseString);

            // Handle error
            if (responseObject.Success == false)
            {
                if (string.IsNullOrEmpty(responseObject.Error))
                    throw new Exception("Unknown error making API call.");
                else
                    throw new Exception($"Error making API call: {responseObject.Error}");
            }

            // Return
            return responseObject.Payload;
        }

        #endregion

        #region IApiService implementation

        /// <summary>Getter for the user details after authenticating.</summary>
        public ApiModel.UserDetailsResponse GetUserDetails(AuthStateApiAccess apiAccess)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                Dictionary<string, object> requestPayload = new Dictionary<string, object>
                {
                    { "RequestType", "GetCurrentUser" }
                };
                return ExecuteAuthRequest<ApiModel.UserDetailsResponse>(httpClient, apiAccess, requestPayload);
            }
        }

        #endregion
    }
}
