using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal class ControlsControlHandler : Natural.Xml.ITagHandler
    {
        #region Base

        /// <summary>The property name.</summary>
        public string Name { get; private set; }

        /// <summary>The list of field handlers.</summary>
        public List<ControlsFieldHandler> FieldHandlerList = new List<ControlsFieldHandler>();

        /// <summary>Constructor.</summary>
        public ControlsControlHandler(Natural.Xml.ITagAttributes attributes)
        {
            this.Name = attributes.GetString("name");
        }

        #endregion

        #region Natural.Xml.ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public Natural.Xml.ITagHandler HandleStartChildTag(string tagName, Natural.Xml.ITagAttributes attributes)
        {
            if (tagName == "field")
            {
                ControlsFieldHandler fieldHandler = new ControlsFieldHandler(attributes);
                this.FieldHandlerList.Add(fieldHandler);
                return fieldHandler;
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public void HandleEndTag()
        {
            //
        }

        #endregion
    }
}
