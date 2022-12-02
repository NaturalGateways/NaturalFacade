using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.Services
{
    public static class ApiService
    {
        /// <summary>Handle the request.</summary>
        public static async Task<ApiDto.ApiResponseDto> HandleAnonRequestAsync(DynamoService dynamoService, ApiDto.AnonRequestPayloadDto requestDto)
        {
            try
            {
                // Parse enum
                if (string.IsNullOrEmpty(requestDto.RequestType))
                {
                    return ApiDto.ApiResponseDto.CreateError($"Missing request type");
                }
                switch (Enum.Parse<ApiDto.AnonRequestType>(requestDto.RequestType))
                {
                    case ApiDto.AnonRequestType.GetInfo:
                        return HandleAnonGetInfo();
                    case ApiDto.AnonRequestType.GetLayoutOverlay:
                        return await HandleAnonGetLayoutOverlayAsync(dynamoService, requestDto);
                    default:
                        return ApiDto.ApiResponseDto.CreateError($"Unrecognised request type: {requestDto.RequestType}");
                }
            }
            catch (Exception ex)
            {
                return ApiDto.ApiResponseDto.CreateError(ex);
            }
        }

        /// <summary>Handle the request.</summary>
        private static ApiDto.ApiResponseDto HandleAnonGetInfo()
        {
            // Get the config resource stream
            System.IO.Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NaturalFacade.AppConfig.json");
            if (resourceStream == null)
            {
                throw new Exception("Cannot retrieve version text: " + string.Join(", ", System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames().Select(x => $"'{x}'")));
            }
            // Read the config
            Dictionary<string, string> configStringsByName = null;
            using (System.IO.StreamReader configStream = new System.IO.StreamReader(resourceStream))
            {
                string configText = configStream.ReadToEnd();
                configStringsByName = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(configText);
            }
            // Return
            return ApiDto.ApiResponseDto.CreateSuccess(new Dictionary<string, string>
            {
                { "Project", "Natural Façade" },
                { "Environment", configStringsByName["environment"] },
                { "Version", configStringsByName["version"] }
            });
        }

        /// <summary>Handle the request.</summary>
        private static async Task<ApiDto.ApiResponseDto> HandleAnonGetLayoutOverlayAsync(DynamoService dynamoService, ApiDto.AnonRequestPayloadDto requestDto)
        {
            object overlayObject = await dynamoService.GetLayoutOverlayAsync(requestDto.UserId, requestDto.LayoutId);
            return ApiDto.ApiResponseDto.CreateSuccess(overlayObject);
        }

        /// <summary>Handle the request.</summary>
        public static async Task<ApiDto.ApiResponseDto> HandleAuthRequestAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            try
            {
                // Parse enum
                switch (Enum.Parse<ApiDto.AuthRequestType>(requestDto.payload.RequestType))
                {
                    case ApiDto.AuthRequestType.GetCurrentUser:
                        return ApiDto.ApiResponseDto.CreateSuccess(await HandleAuthGetCurrentUserAsync(dynamoService, requestDto));
                    case ApiDto.AuthRequestType.UpdateCurrentUser:
                        return ApiDto.ApiResponseDto.CreateSuccess(await HandleAuthUpdateCurrentUserAsync(dynamoService, requestDto));
                    default:
                        return ApiDto.ApiResponseDto.CreateError($"Unrecognised request type: {requestDto.payload.RequestType}");
                }
            }
            catch (Exception ex)
            {
                return ApiDto.ApiResponseDto.CreateError(ex);
            }
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAuthGetCurrentUserAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get existing layout ID
            string userId = $"User-{requestDto.context.userId}";

            // Get user
            ItemModel.ItemUser userItem = await dynamoService.GetUserAsync(userId);

            // Check return existing
            if (userItem != null)
            {
                return userItem;
            }

            // Run action
            ActionModel.Action action = new ActionModel.Action
            {
                AuthType = ActionModel.ActionType.CreateUser,
                CreateUser = new ActionModel.ActionCreateUser
                {
                    UserId = userId,
                    Email = requestDto.context.email
                }
            };
            await dynamoService.PutActionAsync(userId, userId, action);
            return await ActionService.ProcessActionAsync(dynamoService, action, true);
        }

        /// <summary>Handle the request.</summary>
        private static async Task<ApiDto.ApiResponseDto> HandleAuthUpdateCurrentUserAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get user
            string userId = $"User-{requestDto.context.userId}";

            // Run action
            ActionModel.Action action = new ActionModel.Action
            {
                AuthType = ActionModel.ActionType.UpdateUser,
                UpdateUser = new ActionModel.ActionUpdateUser
                {
                    UserId = userId,
                    Name = requestDto.payload.UpdateCurrentUser.Name
                }
            };
            await dynamoService.PutActionAsync(userId, userId, action);
            return ApiDto.ApiResponseDto.CreateSuccess(await ActionService.ProcessActionAsync(dynamoService, action, true));
        }
    }
}
