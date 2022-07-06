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

        string ItemTableName { get; }
    }

    public class DynamoService
    {
        #region Base

        /// <summary>The action table.</summary>
        private Natural.Aws.DynamoDB.IDynamoTable m_actionTable = null;
        /// <summary>The item table.</summary>
        private Natural.Aws.DynamoDB.IDynamoTable m_itemTable = null;

        /// <summary>Constructor.</summary>
        public DynamoService(Natural.Aws.DynamoDB.IDynamoService dynamo, IDynamoServiceTableNames tableNames)
        {
            // Get tables
            m_actionTable = dynamo.GetTable(tableNames?.ActionTableName ?? "Actions", "UserId", "DateTimeUtc");
            m_itemTable = dynamo.GetTable(tableNames?.ItemTableName ?? "Items", "ItemId", "ComponentName");
        }

        #endregion

        #region Generic getters and setters

        /// <summary>Puts an action.</summary>
        public async Task PutActionAsync(string itemId, string description, ActionModel.Action action)
        {
            await m_actionTable.PutItemAsync(itemId, DateTime.UtcNow.ToString("o"), new Natural.Aws.DynamoDB.ItemUpdate
            {
                ObjectAttributes = new Dictionary<string, object>
                {
                    { "DataJson", action }
                },
                StringAttributes = new Dictionary<string, string>
                {
                    { "Description", description }
                }
            });
        }

        /// <summary>Gets an item.</summary>
        public async Task<ItemType> GetItemDataAsync<ItemType>(string userId, string partitionId, string sortId)
        {
            Natural.Aws.DynamoDB.IDynamoItem item = await m_itemTable.GetItemByKeyAsync(partitionId, sortId, "UserId,DataJson");
            if (item == null)
            {
                return default(ItemType);
            }
            string recordUserId = item.GetString("UserId");
            if (userId != recordUserId)
            {
                throw new Exception("Cannot access record by other user.");
            }
            return item.GetStringAsObject<ItemType>("DataJson");
        }

        /// <summary>Puts an item.</summary>
        public async Task PutItemAsync(string userId, string partitionId, string sortId, object data)
        {
            await m_itemTable.PutItemAsync(partitionId, sortId, new Natural.Aws.DynamoDB.ItemUpdate
            {
                ObjectAttributes = new Dictionary<string, object>
                {
                    { "DataJson", data }
                },
                StringAttributes = new Dictionary<string, string>
                {
                    { "UserId", userId },
                    { "LastModifiedUtc", DateTime.UtcNow.ToString("o") }
                }
            });
        }

        #endregion

        #region Typed getters and setters

        /// <summary>Gets a user.</summary>
        public async Task<ItemModel.ItemUser> GetUserAsync(string userId)
        {
            return await GetItemDataAsync<ItemModel.ItemUser>(userId, "Users", userId);
        }

        /// <summary>Puts a user.</summary>
        public async Task PutUserAsync(ItemModel.ItemUser itemUser)
        {
            await PutItemAsync(itemUser.UserId, "Users", itemUser.UserId, itemUser);
        }

        /// <summary>Gets a layout overlay.</summary>
        public async Task<object> GetLayoutOverlayAsync(string userId, string layoutId)
        {
            return await GetItemDataAsync<object>(userId, layoutId, "Overlay");
        }

        /// <summary>Gets a layout overlay.</summary>
        public async Task PutLayoutAsync(string userId, string layoutId, ItemModel.ItemLayoutSummary summaryData, ItemModel.ItemLayoutConfig configData, object overlayObject)
        {
            await PutItemAsync(userId, userId, layoutId, summaryData);
            await PutItemAsync(userId, layoutId, "Summary", summaryData);
            await PutItemAsync(userId, layoutId, "Config", configData);
            await PutItemAsync(userId, layoutId, "Overlay", overlayObject);
        }

        #endregion
    }
}
