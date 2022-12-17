using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.Services
{
    public static class ActionService
    {
        public static async Task<object> ProcessActionAsync(DynamoService dynamoService, ActionModel.Action action, bool createResponse)
        {
            switch (action.AuthType)
            {
                case ActionModel.ActionType.CreateUser:
                    return await ProcessCreateUserActionAsync(dynamoService, action.CreateUser, createResponse);
                case ActionModel.ActionType.UpdateUser:
                    return await ProcessUpdateUserActionAsync(dynamoService, action.UpdateUser, createResponse);
                case ActionModel.ActionType.CreateLayout:
                    return await ProcessCreateLayoutActionAsync(dynamoService, action.CreateLayout, createResponse);
                case ActionModel.ActionType.PutLayout:
                    await ProcessPutLayoutActionAsync(dynamoService, action.PutLayout);
                    return null;
                default:
                    throw new Exception($"Cannot handle action type '{action.AuthType.ToString()}'.");
            }
        }

        private static async Task<object> ProcessCreateUserActionAsync(DynamoService dynamoService, ActionModel.ActionCreateUser action, bool createResponse)
        {
            // Create summary
            ItemModel.ItemUser userItem = new ItemModel.ItemUser
            {
                UserId = action.UserId,
                Email = action.Email,
                Name = "New User"
            };

            // Write
            await dynamoService.PutUserAsync(userItem);

            // Return
            if (createResponse)
                return userItem;
            return null;
        }

        private static async Task<object> ProcessUpdateUserActionAsync(DynamoService dynamoService, ActionModel.ActionUpdateUser action, bool createResponse)
        {
            // Get
            ItemModel.ItemUser userItem = await dynamoService.GetUserAsync(action.UserId);
            // Update
            userItem.Name = action.Name;
            // Set
            await dynamoService.PutUserAsync(userItem);

            // Return
            if (createResponse)
                return userItem;
            return null;
        }

        private static async Task<object> ProcessCreateLayoutActionAsync(DynamoService dynamoService, ActionModel.ActionCreateLayout action, bool createResponse)
        {
            // Create object
            ItemModel.ItemLayoutSummary summary = new ItemModel.ItemLayoutSummary
            {
                CreatorUserId = action.UserId,
                CreatedDateTime = DateTime.UtcNow,
                LayoutId = action.LayoutId,
                Name = action.Name
            };
            // Save
            await dynamoService.PutBlankLayoutAsync(action.UserId, summary);

            // Return
            if (createResponse)
                return summary;
            return null;
        }

        private static async Task ProcessPutLayoutActionAsync(DynamoService dynamoService, ActionModel.ActionPutLayout action)
        {
            // Get existing properties
            ApiDto.PropertyDto[] properties = await dynamoService.GetOverlayPropertiesAsync(action.LayoutId);
            ItemModel.ItemLayoutSummary summary = await dynamoService.GetLayoutSummaryAsync(action.LayoutId);

            // Convert layout
            LayoutConfig.Config2LayoutResult convertResult = LayoutConfig.Config2Layout.Convert(action.LayoutConfig, properties);
            summary.ControlsNameArray = convertResult.ControlsArray?.Select(x => x.Name)?.ToArray();

            // Save
            await dynamoService.PutNewLayoutConfigAsync(action.LayoutId, summary, action.LayoutConfig, convertResult);
        }
    }
}
