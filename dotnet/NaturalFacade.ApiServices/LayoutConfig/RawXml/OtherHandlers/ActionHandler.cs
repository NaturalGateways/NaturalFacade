using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class ActionHandler : Natural.Xml.ITagHandler
    {
        #region Base

        /// <summary>Resource tracking.</summary>
        private RawXmlReferenceTracking m_tracking = null;

        /// <summary>The list of effects.</summary>
        public string ActionName { get; private set; }
        /// <summary>The list of effects.</summary>
        public Config2LayoutResultActionApiAccess ApiAccess { get; private set; }

        /// <summary>The list of effects.</summary>
        public List<Config2LayoutResultActionEffect> EffectList { get; set; } = new List<Config2LayoutResultActionEffect>();

        /// <summary>Constructor.</summary>
        public ActionHandler(RawXmlReferenceTracking tracking, Natural.Xml.ITagAttributes attributes)
        {
            m_tracking = tracking;
            this.ActionName = attributes.GetString("name");
            this.ApiAccess = attributes.GetEnum<Config2LayoutResultActionApiAccess>("api_access");
        }

        /// <summary>Creates the action from the read data.</summary>
        public Config2LayoutResultAction CreateAction()
        {
            return new Config2LayoutResultAction
            {
                ApiAccess = this.ApiAccess.ToString(),
                Effects = this.EffectList.ToArray()
            };
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "effect":
                    HandleTextFieldTag(attributes);
                    break;
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public void HandleEndTag()
        {
            //
        }

        #endregion

        #region Tag handler

        /// <summary>Handles a child tag.</summary>
        private void HandleTextFieldTag(Natural.Xml.ITagAttributes attributes)
        {
            Config2LayoutResultActionEffect effect = new Config2LayoutResultActionEffect
            {
                Type = attributes.GetString("type")
            };
            switch (effect.Type)
            {
                case "PropSet":
                    effect.PropIndex = m_tracking.GetPropertyUsedIndex(attributes.GetString("prop_name"));
                    effect.Value = attributes.GetString("value");
                    break;
                default:
                    throw new Exception($"Unrecognised effect type: '{effect.Type}'");
            }
            this.EffectList.Add(effect);
        }

        #endregion
    }
}
