using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.Services
{
    public static class ApiService
    {
        #region Anon endpoint

        /// <summary>Handle the request.</summary>
        public static async Task<object> HandleAnonRequestAsync(DynamoService dynamoService, ApiDto.AnonRequestPayloadDto requestDto)
        {
            // Parse enum
            if (string.IsNullOrEmpty(requestDto.RequestType))
            {
                throw new FacadeApiException($"Missing request type");
            }
            switch (Enum.Parse<ApiDto.AnonRequestType>(requestDto.RequestType))
            {
                case ApiDto.AnonRequestType.GetInfo:
                    return HandleAnonGetInfo();
                case ApiDto.AnonRequestType.GetLayoutOverlay:
                    return await HandleAnonGetLayoutOverlayAsync(dynamoService, requestDto);
                case ApiDto.AnonRequestType.GetLayoutOverlayPropValues:
                    return await HandleAnonGetLayoutOverlayPropValuesAsync(dynamoService, requestDto);
                case ApiDto.AnonRequestType.ConvertLayoutToOverlay:
                    return HandleAnonConvertLayoutToOverlay(requestDto);
                case ApiDto.AnonRequestType.PerformAction:
                    return await HandleAnonPerformActionAsync(dynamoService, requestDto);
                default:
                    throw new FacadeApiException($"Unrecognised request type: {requestDto.RequestType}");
            }
        }

        /// <summary>Handle the request.</summary>
        private static object HandleAnonGetInfo()
        {
            // Get the config resource stream
            System.IO.Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NaturalFacade.AppConfig.json");
            if (resourceStream == null)
            {
                throw new Exception("Cannot retrieve version text.");
            }
            // Read the config
            Dictionary<string, string> configStringsByName = null;
            using (System.IO.StreamReader configStream = new System.IO.StreamReader(resourceStream))
            {
                string configText = configStream.ReadToEnd();
                configStringsByName = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(configText);
            }
            // Return
            return new Dictionary<string, string>
            {
                { "Project", "Natural Façade" },
                { "Environment", Environment.GetEnvironmentVariable("Environment") ?? "None" },
                { "Version", configStringsByName["version"] }
            };
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAnonGetLayoutOverlayAsync(DynamoService dynamoService, ApiDto.AnonRequestPayloadDto requestDto)
        {
            // Check params
            if (string.IsNullOrEmpty(requestDto.LayoutId))
                throw new Exception("Layout ID is needed for fetching a layout overlay.");
            // Fetch
            object layoutOverlay = await dynamoService.GetLayoutOverlayAsync(requestDto.LayoutId);
            if (layoutOverlay == null)
                throw new Exception($"Layout ID '{requestDto.LayoutId}' doesn't match a saved overlay.");
            return layoutOverlay;
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAnonGetLayoutOverlayPropValuesAsync(DynamoService dynamoService, ApiDto.AnonRequestPayloadDto requestDto)
        {
            // Check params
            if (string.IsNullOrEmpty(requestDto.LayoutId))
                throw new Exception("Layout ID is needed for fetching a layout overlay.");
            // Fetch
            object layoutOverlay = await dynamoService.GetOverlayPropValuesAsync(requestDto.LayoutId);
            if (layoutOverlay == null)
                throw new Exception($"Layout ID '{requestDto.LayoutId}' doesn't match a saved overlay.");
            return layoutOverlay;
        }

        /// <summary>Handle the request.</summary>
        private static object HandleAnonConvertLayoutToOverlay(ApiDto.AnonRequestPayloadDto requestDto)
        {
            if (requestDto.LayoutConfig == null)
            {
                throw new FacadeApiException("LayoutConfig missing from layout convert.");
            }
            LayoutConfig.Config2LayoutResult result = LayoutConfig.Config2Layout.Convert(requestDto.LayoutConfig, null);
            Dictionary<string, object> resultData = new Dictionary<string, object>
            {
                { "overlay", result.Overlay },
                { "propValues", result.PropertyValues }
            };
            if (result.Controls?.Any() ?? false)
            {
                resultData.Add("controls", result.Controls);
            }
            return resultData;
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAnonPerformActionAsync(DynamoService dynamoService, ApiDto.AnonRequestPayloadDto requestDto)
        {
            // Validate
            if (string.IsNullOrEmpty(requestDto.LayoutId))
            {
                throw new FacadeApiException("Layout ID missing from perform action request.");
            }
            if (string.IsNullOrEmpty(requestDto.ActionName))
            {
                throw new FacadeApiException("Action name missing from perform action request.");
            }
            LayoutConfig.Config2LayoutResultAction action = await dynamoService.GetActionAsync(requestDto.LayoutId, requestDto.ActionName);
            if (action == null)
            {
                throw new FacadeApiException("Cannot find action.");
            }
            if (action.ApiAccess != "Anon")
            {
                throw new FacadeApiException("Action is not anonymous.");
            }

            // Create and perform API action
            await ActionEffectService.ExecuteActionAsync(dynamoService, requestDto.LayoutId, action);
            return null;
        }

        #endregion

        #region Auth endpoint

        /// <summary>Handle the request.</summary>
        public static async Task<object> HandleAuthRequestAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Parse enum
            switch (Enum.Parse<ApiDto.AuthRequestType>(requestDto.payload.RequestType))
            {
                case ApiDto.AuthRequestType.GetCurrentUser:
                    return await HandleAuthGetCurrentUserAsync(dynamoService, requestDto);
                case ApiDto.AuthRequestType.UpdateCurrentUser:
                    return await HandleAuthUpdateCurrentUserAsync(dynamoService, requestDto);
                case ApiDto.AuthRequestType.GetLayoutSummaryPage:
                    return await HandleAuthGetLayoutSummaryPageAsync(dynamoService, requestDto);
                case ApiDto.AuthRequestType.CreateLayout:
                    return await HandleAuthCreateLayoutAsync(dynamoService, requestDto);
                case ApiDto.AuthRequestType.GetLayout:
                    return await HandleAuthGetLayoutAsync(dynamoService, requestDto);
                case ApiDto.AuthRequestType.GetLayoutControls:
                    return await HandleAuthGetLayoutControlsAsync(dynamoService, requestDto);
                case ApiDto.AuthRequestType.PutLayout:
                    return await HandleAuthPutLayoutAsync(dynamoService, requestDto);
                case ApiDto.AuthRequestType.PutLayoutPropertyValues:
                    return await HandleAuthPutLayoutPropertyValuesAsync(dynamoService, requestDto);
                default:
                    throw new FacadeApiException($"Unrecognised request type: {requestDto.payload.RequestType}");
            }
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAuthGetCurrentUserAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get existing layout ID
            string userId = requestDto.context.userId;

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
        private static async Task<object> HandleAuthUpdateCurrentUserAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get IDs
            string userId = requestDto.context.userId;

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
            return await ActionService.ProcessActionAsync(dynamoService, action, true);
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAuthGetLayoutSummaryPageAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get IDs
            string userId = requestDto.context.userId;

            // Get IDs
            List<string> layoutIdList = await dynamoService.GetLayoutIdsAsync(userId);

            // Get summaries
            List<ItemModel.ItemLayoutSummary> summaryList = new List<ItemModel.ItemLayoutSummary>();
            foreach (string layoutId in layoutIdList)
            {
                ItemModel.ItemLayoutSummary summary = await dynamoService.GetLayoutSummaryAsync(layoutId);
                summaryList.Add(summary);
            }

            // Return
            return summaryList.OrderBy(x => x.CreatedDateTime);
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAuthCreateLayoutAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get IDs
            string userId = requestDto.context.userId;
            string layoutId = $"Layout-{Guid.NewGuid().ToString()}";

            // Run action
            ActionModel.Action action = new ActionModel.Action
            {
                AuthType = ActionModel.ActionType.CreateLayout,
                CreateLayout = new ActionModel.ActionCreateLayout
                {
                    UserId = userId,
                    LayoutId = layoutId,
                    Name = requestDto.payload.CreateLayout.Name
                }
            };
            await dynamoService.PutActionAsync(userId, layoutId, action);
            return await ActionService.ProcessActionAsync(dynamoService, action, true);
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAuthGetLayoutAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get IDs
            string userId = requestDto.context.userId;
            string layoutId = requestDto.payload.GetLayout.LayoutId;

            // Fetch summary
            ItemModel.ItemLayoutSummary summary = await dynamoService.GetLayoutSummaryAsync(layoutId);
            // Check creator
            if (summary.CreatorUserId != userId)
            {
                throw new Exception("User is not authorised to view layout.");
            }

            // Get layout config
            return await dynamoService.GetLayoutConfigAsync(layoutId);
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAuthGetLayoutControlsAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get IDs
            string userId = requestDto.context.userId;
            string layoutId = requestDto.payload.GetLayoutControls.LayoutId;
            int controlsIndex = requestDto.payload.GetLayoutControls.ControlsIndex;

            // Fetch summary
            ItemModel.ItemLayoutSummary summary = await dynamoService.GetLayoutSummaryAsync(layoutId);
            // Check creator
            if (summary.CreatorUserId != userId)
            {
                throw new Exception("User is not authorised to view layout.");
            }

            // Fetch controls
            ItemModel.ItemLayoutControlsData controls = await dynamoService.GetOverlayControlsAsync(layoutId, controlsIndex);
            if (controls == null)
            {
                throw new Exception($"Controls at index {controlsIndex} don't exist.");
            }

            // Return controls
            return controls;
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAuthPutLayoutAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get IDs
            string userId = requestDto.context.userId;
            string layoutId = requestDto.payload.PutLayout.LayoutId;

            // Fetch summary
            ItemModel.ItemLayoutSummary summary = await dynamoService.GetLayoutSummaryAsync(layoutId);
            // Check creator
            if (summary.CreatorUserId != userId)
            {
                throw new Exception("User is not authorised to view layout.");
            }

            // Run action
            ActionModel.Action action = new ActionModel.Action
            {
                AuthType = ActionModel.ActionType.PutLayout,
                PutLayout = new ActionModel.ActionPutLayout
                {
                    LayoutId = layoutId,
                    LayoutConfig = requestDto.payload.PutLayout.LayoutConfig
                }
            };
            await dynamoService.PutActionAsync(userId, layoutId, action);
            return await ActionService.ProcessActionAsync(dynamoService, action, true);
        }

        /// <summary>Handle the request.</summary>
        private static async Task<object> HandleAuthPutLayoutPropertyValuesAsync(DynamoService dynamoService, ApiDto.AuthRequestDto requestDto)
        {
            // Get IDs
            string userId = requestDto.context.userId;
            string layoutId = requestDto.payload.PutLayoutPropertyValues.LayoutId;

            // Fetch summary
            ItemModel.ItemLayoutSummary summary = await dynamoService.GetLayoutSummaryAsync(layoutId);
            // Check creator
            if (summary.CreatorUserId != userId)
            {
                throw new Exception("User is not authorised to view layout.");
            }

            // Get existing properties
            ApiDto.PropertyDto[] properties = await dynamoService.GetOverlayPropertiesAsync(layoutId);
            if (properties == null)
            {
                throw new Exception("Cannot find properties.");
            }
            foreach (ApiDto.AuthPutLayoutPropertyValueDataRequestDto propertyValue in requestDto.payload.PutLayoutPropertyValues.Values)
            {
                if ((properties?.Length ?? 0) <= propertyValue.PropertyIndex)
                {
                    throw new Exception("Cannot find property.");
                }
            }

            // Apply changes
            foreach (ApiDto.AuthPutLayoutPropertyValueDataRequestDto propertyValue in requestDto.payload.PutLayoutPropertyValues.Values)
            {
                ApiDto.PropertyDto existingProperty = properties[propertyValue.PropertyIndex];
                Natural.Json.IJsonObject valueJson = Natural.Json.JsonHelper.JsonFromObject(propertyValue.Value);
                existingProperty.Value = LayoutConfig.Config2Layout.ConvertPropValue(existingProperty.Type, valueJson);
            }

            // Create new values
            object[] propValues = null;
            if (properties?.Any() ?? false)
            {
                propValues = properties.Select(x => x.Value).ToArray();
            }
            else
            {
                propValues = Array.Empty<object>();
            }

            // Save
            await dynamoService.PutLayoutPropertyValuesAsync(layoutId, properties, propValues);
            return null;
        }

        #endregion
    }
}
