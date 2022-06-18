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
        public static async Task<ApiDto.AuthResponseDto> HandleAuthRequestAsync(Natural.Aws.DynamoDB.IDynamoService dynamoDb, string userId, ApiDto.AuthRequestDto requestDto)
        {
            try
            {
                // Parse enum
                switch (Enum.Parse<ApiDto.AuthRequestType>(requestDto.AuthType))
                {
                    case ApiDto.AuthRequestType.PutLayout:
                        return await HandleAuthPutLayoutRequestAsync(dynamoDb, userId, requestDto);
                    default:
                        return ApiDto.AuthResponseDto.CreateError($"Unrecognised request type: {requestDto.AuthType}");
                }
            }
            catch (Exception ex)
            {
                return ApiDto.AuthResponseDto.CreateError(ex.Message);
            }
        }

        /// <summary>Handle the request.</summary>
        private static async Task<ApiDto.AuthResponseDto> HandleAuthPutLayoutRequestAsync(Natural.Aws.DynamoDB.IDynamoService dynamoDb, string userId, ApiDto.AuthRequestDto requestDto)
        {
            // Create services
            DynamoService dynamoService = new DynamoService(dynamoDb);

            // Get existing layout ID
            string layoutId = requestDto.Layout.LayoutId ?? $"Layout-{Guid.NewGuid().ToString()}";

            // Create action
            ActionModel.Action action = new ActionModel.Action
            {
                AuthType = ActionModel.ActionType.PutLayout,
                Layout = new ActionModel.ActionLayout
                {
                    UserId = userId,
                    LayoutId = layoutId,
                    Config = requestDto.Layout.Config
                }
            };

            // Write action
            await dynamoService.PutActionAsync(userId, $"PutLayout-{layoutId}", action);

            // Process action
            await ActionService.ProcessActionAsync(dynamoService, userId, action);

            // Return
            return ApiDto.AuthResponseDto.CreateSuccess(new ApiDto.AuthResponseDtoLayout { LayoutId = layoutId });
        }
    }
}
