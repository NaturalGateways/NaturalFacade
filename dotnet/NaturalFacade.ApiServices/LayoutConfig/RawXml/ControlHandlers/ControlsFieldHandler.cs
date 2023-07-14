using NaturalFacade.LayoutConfig.RawXml.ControlHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class ControlsFieldHandler : Natural.Xml.ITagHandler
    {
        #region Base

        /// <summary>The property name.</summary>
        public string PropName { get; private set; }

        /// <summary>The model for the field.</summary>
        public Config2LayoutOverlayOutputControlsFieldDef FieldModel { get; private set; }

        /// <summary>Constructor.</summary>
        public ControlsFieldHandler(Natural.Xml.ITagAttributes attributes)
        {
            this.PropName = attributes.GetString("prop_name");
            this.FieldModel = new Config2LayoutOverlayOutputControlsFieldDef
            {
                Label = attributes.GetNullableString("label"),
                PropIndex = 0
            };
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "integer":
                    HandleIntegerFieldTag(attributes);
                    break;
                case "switch":
                    HandleSwitchFieldTag(attributes);
                    break;
                case "timer":
                    HandleTimerFieldTag(attributes);
                    break;
                case "options":
                    return HandleOptionsFieldTag();
                case "text_field":
                    HandleTextFieldTag();
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
        private void HandleTextFieldTag()
        {
            this.FieldModel.TextField = new object();
        }

        /// <summary>Handles a child tag.</summary>
        private void HandleIntegerFieldTag(Natural.Xml.ITagAttributes attributes)
        {
            this.FieldModel.Integer = new Config2LayoutOverlayOutputControlsFieldIntegerDef
            {
                Step = attributes.GetLong("step"),
                MinValue = attributes.GetNullableLong("min_value"),
                MaxValue = attributes.GetNullableLong("max_value")
            };
        }

        /// <summary>Handles a child tag.</summary>
        private void HandleSwitchFieldTag(Natural.Xml.ITagAttributes attributes)
        {
            this.FieldModel.Switch = new Config2LayoutOverlayOutputControlsFieldSwitchDef
            {
                FalseLabel = attributes.GetNullableString("false_label"),
                TrueLabel = attributes.GetNullableString("true_label")
            };
        }

        /// <summary>Handles a child tag.</summary>
        private void HandleTimerFieldTag(Natural.Xml.ITagAttributes attributes)
        {
            string allowClearString = attributes.GetNullableString("allow_clear") ?? "true";
            this.FieldModel.Timer = new Config2LayoutOverlayOutputControlsFieldTimerDef
            {
                AllowClear = string.Equals(allowClearString, "true", StringComparison.InvariantCultureIgnoreCase)
            };
        }

        /// <summary>Handles a child tag.</summary>
        private Natural.Xml.ITagHandler HandleOptionsFieldTag()
        {
            return new ControlsFieldOptionsHandler(this.FieldModel);
        }

        #endregion
    }
}
