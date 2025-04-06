using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.Services
{
    public class ActionEffectService
    {
        /// <summary>The array of values.</summary>
        private object[] m_propValueArray = null;

        /// <summary>Executes the effects of an action.</summary>
        public static async Task ExecuteActionAsync(DynamoService dynamoService, string layoutId, LayoutConfig.Config2LayoutResultAction action)
        {
            // Run effects
            ActionEffectService actionEffectService = new ActionEffectService();
            if (action.Effects?.Any() ?? false)
            {
                foreach (LayoutConfig.Config2LayoutResultActionEffect effect in action.Effects)
                {
                    await actionEffectService.ExecuteActionEffectAsync(dynamoService, layoutId, effect);
                }
            }

            // Check for saves
            if (actionEffectService.m_propValueArray != null)
            {
                await dynamoService.PutLayoutPropertyValuesAsync(layoutId, actionEffectService.m_propValueArray);
            }
        }

        /// <summary>Executes the effects of an action.</summary>
        private async Task ExecuteActionEffectAsync(DynamoService dynamoService, string layoutId, LayoutConfig.Config2LayoutResultActionEffect effect)
        {
            switch (effect.Type)
            {
                case "PropSet":
                    await ExecuteSetPropEffectAsync(dynamoService, layoutId, effect);
                    break;
                case "PlayVideo":
                    await ExecutePlayVideoEffectAsync(dynamoService, layoutId, effect);
                    break;
                default:
                    throw new Exception($"Unrecognised action type '{effect.Type}'.");
            }
        }

        /// <summary>Executes an effect.</summary>
        private async Task ExecuteSetPropEffectAsync(DynamoService dynamoService, string layoutId, LayoutConfig.Config2LayoutResultActionEffect effect)
        {
            if (effect.PropIndex.HasValue == false)
            {
                throw new Exception("Need a prop index when setting a property.");
            }
            if (m_propValueArray == null)
            {
                m_propValueArray = await dynamoService.GetOverlayPropValuesAsync(layoutId);
            }
            m_propValueArray[effect.PropIndex.Value] = effect.Value;
        }

        /// <summary>Executes an effect.</summary>
        private async Task ExecutePlayVideoEffectAsync(DynamoService dynamoService, string layoutId, LayoutConfig.Config2LayoutResultActionEffect effect)
        {
            if (effect.PropIndex.HasValue == false)
            {
                throw new Exception("Need a prop index when setting a property.");
            }
            if (m_propValueArray == null)
            {
                m_propValueArray = await dynamoService.GetOverlayPropValuesAsync(layoutId);
            }
            long playCount = 1;
            object oldValue = m_propValueArray[effect.PropIndex.Value];
            if (oldValue != null)
            {
                long? oldPlayCount = Natural.Json.JsonHelper.JsonFromObject(oldValue).GetDictionaryLong("PlayCount");
                if (oldPlayCount.HasValue)
                    playCount = oldPlayCount.Value + 1;
            }
            m_propValueArray[effect.PropIndex.Value] = new Dictionary<string, object>
            {
                { "State", "Playing" },
                { "PlayCount", playCount }
            };
        }
    }
}
