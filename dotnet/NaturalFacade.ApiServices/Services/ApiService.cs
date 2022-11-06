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
                        return await HandleAuthGetCurrentUserAsync(dynamoService, requestDto);
                    case ApiDto.AuthRequestType.GetLayoutOverlay:
                        return await HandleAuthGetLayoutOverlayAsync(dynamoService, requestDto);
                    case ApiDto.AuthRequestType.PutLayout:
                        return await HandleAuthPutLayoutAsync(dynamoService, requestDto);
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
        private static async Task<ApiDto.ApiResponseDto> HandleAuthGetCurrentUserAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get existing layout ID
            string userId = $"User-{requestDto.context.userId}";

            // Get user
            ItemModel.ItemUser userItem = await dynamoService.GetUserAsync(userId);

            // Check create user
            if (userItem == null || userItem.Email != requestDto.context.email)
            {
                userItem = new ItemModel.ItemUser
                {
                    UserId = userId,
                    Email = requestDto.context.email,
                    Name = "New User"
                };
                await dynamoService.PutUserAsync(userItem);
            }

            // Return response
            return ApiDto.ApiResponseDto.CreateSuccess(new ApiDto.AuthGetCurrentUserResponseDto
            {
                userId = userItem.UserId,
                email = userItem.Email,
                name = userItem.Name
            });
        }

        /// <summary>Handle the request.</summary>
        private static async Task<ApiDto.ApiResponseDto> HandleAuthGetLayoutOverlayAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            string userId = $"User-{requestDto.context.userId}";
            object overlayObject = await dynamoService.GetLayoutOverlayAsync(userId, requestDto.payload.LayoutId);
            return ApiDto.ApiResponseDto.CreateSuccess(new ApiDto.AuthGetLayoutOverlayResponseDto
            {
                LayoutId = requestDto.payload.LayoutId,
                Overlay = overlayObject
            });
        }

        /// <summary>Handle the request.</summary>
        private static async Task<ApiDto.ApiResponseDto> HandleAuthPutLayoutAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get existing layout ID
            string userId = $"User-{requestDto.context.userId}";
            string layoutId = requestDto.payload.PutLayout.LayoutId ?? $"Layout-{Guid.NewGuid().ToString()}";

            // Create action
            ActionModel.Action action = new ActionModel.Action
            {
                AuthType = ActionModel.ActionType.PutLayout,
                Layout = new ActionModel.ActionLayout
                {
                    UserId = userId,
                    LayoutId = layoutId,
                    Config = requestDto.payload.PutLayout.Config
                }
            };

            // Write action
            await dynamoService.PutActionAsync(userId, $"PutLayout-{layoutId}", action);

            // Process action
            await ActionService.ProcessActionAsync(dynamoService, userId, action);

            // Return
            return ApiDto.ApiResponseDto.CreateSuccess(new ApiDto.AuthPutLayoutResponseDto { LayoutId = layoutId });
        }
    }
}
