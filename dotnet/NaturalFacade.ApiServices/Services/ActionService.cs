using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.Services
{
    public static class ActionService
    {
        public static async Task ProcessActionAsync(DynamoService dynamoService, string userId, ActionModel.Action action)
        {
            switch (action.AuthType)
            {
                case ActionModel.ActionType.PutLayout:
                    {
                        await ProcessPutLayoutActionAsync(dynamoService, userId, action.Layout);
                        break;
                    }
            }
        }

        private static async Task ProcessPutLayoutActionAsync(DynamoService dynamoService, string userId, ActionModel.ActionLayout actionLayout)
        {
            // convert config to layout
            object overlayObject = LayoutConfig.Config2Layout.Convert(actionLayout.Config);

            // Create summary and detail
            ItemModel.ItemLayoutSummary summaryItemData = new ItemModel.ItemLayoutSummary
            {
                UserId = userId,
                LayoutId = actionLayout.LayoutId,
                Name = actionLayout.Config.Name
            };
            ItemModel.ItemLayoutConfig configItemData = new ItemModel.ItemLayoutConfig
            {
                UserId = userId,
                LayoutId = actionLayout.LayoutId,
                Name = actionLayout.Config.Name,
                Config = actionLayout.Config
            };

            // Write
            await dynamoService.PutLayoutAsync(userId, actionLayout.LayoutId, summaryItemData, configItemData, overlayObject);
        }
    }
}
