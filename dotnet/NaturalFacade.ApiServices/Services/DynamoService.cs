using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.Services
{
    public interface IDynamoServiceTableNames
    {
        string ActionTableName { get; }

        string ItemDataTableName { get; }

        string ItemLinkTableName { get; }
    }

    public class DynamoService
    {
        #region Base

        /// <summary>The item table.</summary>
        private Natural.Aws.DynamoDB.IDynamoTable m_actionTable = null;
        /// <summary>The item table.</summary>
        private Natural.Aws.DynamoDB.IDynamoTable m_itemDataTable = null;
        /// <summary>The link table.</summary>
        private Natural.Aws.DynamoDB.IDynamoTable m_itemLinkTable = null;

        /// <summary>Constructor.</summary>
        public DynamoService(Natural.Aws.DynamoDB.IDynamoService dynamo, IDynamoServiceTableNames tableNames)
        {
            // Get tables
            m_actionTable = dynamo.GetTable(tableNames?.ActionTableName ?? "Actions", "UserId", "DateTimeUtc");
            m_itemDataTable = dynamo.GetTable(tableNames?.ItemDataTableName ?? "Items", "ItemId", "ComponentId");
            m_itemLinkTable = dynamo.GetTable(tableNames?.ItemLinkTableName ?? "Links", "ParentId", "ChildId");
        }

        #endregion

        #region Generic getters and setters

        /// <summary>Puts an action.</summary>
        public async Task PutActionAsync(string userId, string itemId, ActionModel.Action action)
        {
            await m_actionTable.PutItemAsync(userId, DateTime.UtcNow.ToString("o"), new Natural.Aws.DynamoDB.ItemUpdate
            {
                ObjectAttributes = new Dictionary<string, object>
                {
                    { "ModelJson", action.AsMinimalObject() }
                },
                StringAttributes = new Dictionary<string, string>
                {
                    { "ItemId", itemId },
                    { "ActionType", action.AuthType.ToString() }
                }
            });
        }

        /// <summary>Gets an item.</summary>
        public async Task<ItemType> GetItemDataAsync<ItemType>(string itemId, string componentId)
        {
            Natural.Aws.DynamoDB.IDynamoItem item = await m_itemDataTable.GetItemByKeyAsync(itemId, componentId, "DataJson");
            if (item == null)
            {
                return default(ItemType);
            }
            return item.GetStringAsObject<ItemType>("DataJson");
        }

        /// <summary>Puts an item.</summary>
        public async Task PutItemAsync(string itemId, string componentId, object data)
        {
            await m_itemDataTable.PutItemAsync(itemId, componentId, new Natural.Aws.DynamoDB.ItemUpdate
            {
                ObjectAttributes = new Dictionary<string, object>
                {
                    { "DataJson", data }
                },
                StringAttributes = new Dictionary<string, string>
                {
                    { "LastModifiedUtc", DateTime.UtcNow.ToString("o") }
                }
            });
        }

        /// <summary>Puts a link.</summary>
        public async Task PutLinkAsync(string parentItemId, string childItemId, object data)
        {
            await m_itemLinkTable.PutItemAsync(parentItemId, childItemId, new Natural.Aws.DynamoDB.ItemUpdate
            {
                ObjectAttributes = new Dictionary<string, object>
                {
                    { "DataJson", data }
                },
                StringAttributes = new Dictionary<string, string>
                {
                    { "LastModifiedUtc", DateTime.UtcNow.ToString("o") }
                }
            });
        }

        #endregion

        #region Typed getters and setters

        /// <summary>Gets a user.</summary>
        public async Task<ItemModel.ItemUser> GetUserAsync(string userId)
        {
            return await GetItemDataAsync<ItemModel.ItemUser>(userId, "Summary");
        }

        /// <summary>Puts a user.</summary>
        public async Task PutUserAsync(ItemModel.ItemUser itemUser)
        {
            await PutItemAsync(itemUser.UserId, "Summary", itemUser);
            await PutLinkAsync("App", itemUser.UserId, itemUser);
        }

        /// <summary>Gets a set of layout IDs.</summary>
        public async Task<List<string>> GetLayoutIdsAsync(string userId)
        {
            List<string> layoutIdList = new List<string>();
            foreach (Natural.Aws.DynamoDB.IDynamoItem item in await m_itemLinkTable.GetItemsAsync(userId, "Layout-", "ChildId"))
            {
                layoutIdList.Add(item.GetString("ChildId"));
            }
            return layoutIdList;
        }

        /// <summary>Gets a layout summary.</summary>
        public async Task<ItemModel.ItemLayoutSummary> GetLayoutSummaryAsync(string layoutId)
        {
            return await GetItemDataAsync<ItemModel.ItemLayoutSummary>(layoutId, "Summary");
        }

        /// <summary>Gets a layout summary.</summary>
        public async Task<LayoutConfig.LayoutConfig> GetLayoutConfigAsync(string layoutId)
        {
            return await GetItemDataAsync<LayoutConfig.LayoutConfig>(layoutId, "LayoutConfig");
        }
        
        /// <summary>Gets a layout overlay.</summary>
        public async Task<object> GetLayoutOverlayAsync(string layoutId)
        {
            return await GetItemDataAsync<object>(layoutId, "Overlay");
        }

        /// <summary>Gets a layout overlay.</summary>
        public async Task PutBlankLayoutAsync(string userId, ItemModel.ItemLayoutSummary summaryData)
        {
            await PutItemAsync(summaryData.LayoutId, "Summary", summaryData);
            await PutLinkAsync(userId, summaryData.LayoutId, null);
        }

        /// <summary>Puts a layout config and overlay.</summary>
        public async Task PutNewLayoutConfigAsync(string layoutId, LayoutConfig.LayoutConfig layoutConfig, object overlay)
        {
            await PutItemAsync(layoutId, "LayoutConfig", layoutConfig);
            await PutItemAsync(layoutId, "Overlay", overlay);
        }

        #endregion
    }
}
