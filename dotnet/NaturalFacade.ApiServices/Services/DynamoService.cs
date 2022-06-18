using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.Services
{
    public class DynamoService
    {
        /// <summary>The action table.</summary>
        private Natural.Aws.DynamoDB.IDynamoTable m_actionTable = null;
        /// <summary>The item table.</summary>
        private Natural.Aws.DynamoDB.IDynamoTable m_itemTable = null;

        /// <summary>Constructor.</summary>
        public DynamoService(Natural.Aws.DynamoDB.IDynamoService dynamo)
        {
            // Get tables
            m_actionTable = dynamo.GetTable("Actions", "UserId", "DateTimeUtc");
            m_itemTable = dynamo.GetTable("Items", "ItemId", "ComponentName");
        }

        /// <summary>Puts an action.</summary>
        public async Task PutActionAsync(string itemId, string description, ActionModel.Action action)
        {
            await m_actionTable.PutItemAsync(itemId, DateTime.UtcNow.ToString("o"), new Natural.Aws.DynamoDB.ItemUpdate
            {
                ObjectAttributes = new Dictionary<string, object>
                {
                    { "Data", action }
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
            Natural.Aws.DynamoDB.IDynamoItem item = await m_itemTable.GetItemByKeyAsync(partitionId, sortId, "UserId,Data");
            if (item == null)
            {
                return default(ItemType);
            }
            string recordUserId = item.GetString("UserId");
            if (userId != recordUserId)
            {
                throw new Exception("Cannot access record by other user.");
            }
            return item.GetStringAsObject<ItemType>("Data");
        }

        /// <summary>Puts an item.</summary>
        public async Task PutItemAsync(string userId, string partitionId, string sortId, object data)
        {
            await m_itemTable.PutItemAsync(partitionId, sortId, new Natural.Aws.DynamoDB.ItemUpdate
            {
                ObjectAttributes = new Dictionary<string, object>
                {
                    { "Data", data }
                },
                StringAttributes = new Dictionary<string, string>
                {
                    { "UserId", userId },
                    { "LastModifiedUtc", DateTime.UtcNow.ToString("o") }
                }
            });
        }
    }
}
