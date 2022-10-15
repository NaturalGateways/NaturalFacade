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
            // Get version
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            // Return
            return ApiDto.ApiResponseDto.CreateSuccess(new Dictionary<string, string>
            {
                { "Project", "Natural Façade" },
                { "Version", fvi.FileVersion }
            });
        }

        /// <summary>Handle the request.</summary>
        private static async Task<ApiDto.ApiResponseDto> HandleAnonGetLayoutOverlayAsync(DynamoService dynamoService, ApiDto.AnonRequestPayloadDto requestDto)
        {
            string userItemId = $"User-{requestDto.UserId}";
            string layoutItemId = $"Layout-{requestDto.LayoutId}";
            object overlayObject = await dynamoService.GetLayoutOverlayAsync(userItemId, layoutItemId);
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
