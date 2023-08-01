using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Natural.Xml;

namespace NaturalFacade.LayoutConfig.RawXml
{
    public class RawXmlLayout2Overlay : IXmlReader, ITagHandler
    {
        #region Static facade

        /// <summary>Converts the data.</summary>
        public static Config2LayoutOverlayOutput Convert(object layoutConfig)
        {
            RawXmlLayout2Overlay instance = new RawXmlLayout2Overlay();
            string layoutConfigXmlString = layoutConfig.ToString();
            if (string.IsNullOrEmpty(layoutConfigXmlString) == false)
                XmlReader.ReadFromString(layoutConfigXmlString, instance);
            return instance.CreateOutput();
        }

        #endregion

        #region Base

        /// <summary>Resource tracking.</summary>
        private RawXmlReferenceTracking m_tracking = new RawXmlReferenceTracking();

        /// <summary>The root layout element.</summary>
        private BaseElementHandler m_rootElementHandler = null;

        /// <summary>The list of control handlers.</summary>
        private List<ControlsControlHandler> m_controlsHandlerList = new List<ControlsControlHandler>();

        /// <summary>Creates an output object.</summary>
        public Config2LayoutOverlayOutput CreateOutput()
        {
            // TEMP HACK: Until prop indexes are refactored, we force audio fields to have a string reference
            foreach (ControlsFieldHandler controlField in m_controlsHandlerList.SelectMany(x => x.FieldHandlerList).Where(x => x.FieldModel.AudioWalkman != null))
            {
                m_tracking.GetPropertyUsedIndex(controlField.PropName);
            }

            // Write output
            Config2LayoutOverlayOutput output = new Config2LayoutOverlayOutput
            {
                PropertyDefs = m_tracking.PropertyUsedList.Select(x => x.PropRef).ToArray(),
                ImageResources = m_tracking.ImageResourcesUsedList.Select(x => x.Url).ToArray(),
                FontResources = m_tracking.FontResourcesUsedList.Select(x => x.Url).ToArray(),
                AudioResources = m_tracking.AudioResourcesUsedList.Select(x => x.Url).ToArray(),
                Fonts = m_tracking.FontDefinitionsUsedList.Select(x => ConvertFontDefinition(x)).Where(x => x != null).ToArray(),
                Audios = m_tracking.AudioDefinitionsUsedList.Select(x => ConvertAudioDefinition(x)).Where(x => x != null).ToArray(),
                RootElement = m_rootElementHandler?.Data,
                ControlsArray = m_controlsHandlerList.Select(x => ConvertControls(x)).Where(x => x != null).ToArray()
            };
            return output;
        }

        /// <summary>converts a font to the API DTO.</summary>
        private ApiDto.OverlayDtoFont ConvertFontDefinition(RawXmlReferenceTracking.FontDefinition fontDefinition)
        {
            if (fontDefinition.ResIndex.HasValue && fontDefinition.FontResIndex.HasValue)
            {
                return new ApiDto.OverlayDtoFont
                {
                    res = fontDefinition.FontResIndex.Value,
                    size = fontDefinition.Size,
                    colour = fontDefinition.Colour,
                    align = fontDefinition.Align
                };
            }
            return null;
        }

        /// <summary>converts an audio to the API DTO.</summary>
        private ApiDto.OverlayDtoAudio ConvertAudioDefinition(RawXmlReferenceTracking.AudioDefinition audioDefinition)
        {
            return new ApiDto.OverlayDtoAudio
            {
                res = audioDefinition.ResIndex
            };
        }

        /// <summary>converts a font to the API DTO.</summary>
        private Config2LayoutOverlayOutputControlsDef ConvertControls(ControlsControlHandler controlsHandler)
        {
            Config2LayoutOverlayOutputControlsFieldDef[] fieldArray = controlsHandler.FieldHandlerList.Select(x => ConvertControlsField(x)).Where(x => x != null).ToArray();
            if (fieldArray.Any())
            {
                return new Config2LayoutOverlayOutputControlsDef
                {
                    Name = controlsHandler.Name,
                    SaveAll = false,
                    Fields = fieldArray
                };
            }
            return null;
        }

        /// <summary>converts a font to the API DTO.</summary>
        private Config2LayoutOverlayOutputControlsFieldDef ConvertControlsField(ControlsFieldHandler fieldHandler)
        {
            Config2LayoutOverlayOutputControlsFieldDef fieldDef = fieldHandler.FieldModel;
            // Get prop index
            RawXmlReferenceTracking.Property property = m_tracking.GetPropertyIfUsed(fieldHandler.PropName);
            if (property == null)
                return null;
            fieldDef.PropIndex = property.PropIndex.Value;
            // Use prob name for label if there is no field label
            if (string.IsNullOrEmpty(fieldDef.Label))
                fieldDef.Label = property.PropRef.Name;
            // Check there is at least one field
            if (fieldDef.AudioWalkman == null &&
                fieldDef.TextField == null &&
                fieldDef.Integer == null &&
                fieldDef.Switch == null &&
                fieldDef.SelectOptions == null &&
                fieldDef.Timer == null)
            {
                return null;
            }
            // Return field
            return fieldDef;
        }

        #endregion

        #region IXmlReader implementation

        /// <summary>Called when the root tag is hit to get the initial handler.</summary>
        public ITagHandler HandleRootTag(string tagName, ITagAttributes attributes)
        {
            if (tagName == "layout")
                return this;
            return null;
        }

        #endregion

        #region ITagHandler implementation

        /// <summary>Called when a child tag is hit. Return a handler for a child tag, or null to skip further children.</summary>
        public ITagHandler HandleStartChildTag(string tagName, ITagAttributes attributes)
        {
            switch (tagName)
            {
                case "controls":
                    {
                        ControlsControlHandler handler = new ControlsControlHandler(m_tracking, attributes);
                        m_controlsHandlerList.Add(handler);
                        return handler;
                    }
                case "font":
                    ReadFontTag(attributes);
                    break;
                case "audio":
                    ReadAudioTag(attributes);
                    break;
                case "property":
                    ReadPropertyTag(attributes);
                    break;
                case "resource":
                    ReadResourceTag(attributes);
                    break;
            }
            if (m_rootElementHandler == null)
            {
                m_rootElementHandler = BaseElementHandler.CreateForTag(m_tracking, tagName, attributes);
                if (m_rootElementHandler != null)
                {
                    return m_rootElementHandler;
                }
            }
            return null;
        }

        /// <summary>Called when this handler is done with.</summary>
        public void HandleEndTag()
        {
            //
        }

        #endregion

        #region Tag handlers

        /// <summary>Reads a tag attributes into an object</summary>
        private void ReadPropertyTag(ITagAttributes attributes)
        {
            Config2LayoutOverlayOutputPropertyDef propDef = new Config2LayoutOverlayOutputPropertyDef
            {
                Name = attributes.GetString("name"),
                ValueType = attributes.GetEnum<ApiDto.PropertyTypeDto>("type")
            };
            switch (propDef.ValueType)
            {
                case ApiDto.PropertyTypeDto.String:
                    propDef.DefaultValue = attributes.GetString("default_value");
                    break;
                case ApiDto.PropertyTypeDto.Boolean:
                    propDef.DefaultValue = string.Equals(attributes.GetString("default_value"), "true", StringComparison.InvariantCultureIgnoreCase);
                    break;
                case ApiDto.PropertyTypeDto.Timer:
                    {
                        long? secsDefaultValue = attributes.GetNullableLong("default_value");
                        if (secsDefaultValue.HasValue)
                            propDef.DefaultValue = new Dictionary<string, object> { { "Secs", secsDefaultValue.Value } };
                        propDef.TimerMinValue = attributes.GetNullableLong("min_value");
                        propDef.TimerMaxValue = attributes.GetNullableLong("max_value");
                        propDef.TimerDirection = attributes.GetNullableLong("direction");
                        break;
                    }
            }
            m_tracking.AddProperty(propDef);
        }

        /// <summary>Reads a tag attributes into an object</summary>
        private void ReadResourceTag(ITagAttributes attributes)
        {
            string name = attributes.GetString("name");
            switch (attributes.GetString("type"))
            {
                case "Audio":
                    {
                        string url = attributes.GetString("url");
                        m_tracking.AddAudioResource(name, url);
                        break;
                    }
                case "Font":
                    {
                        string url = attributes.GetString("url");
                        m_tracking.AddFontResource(name, url);
                        break;
                    }
                case "Image":
                    {
                        string url = attributes.GetString("url");
                        m_tracking.AddImageResource(name, url);
                        break;
                    }
            }
        }

        /// <summary>Reads a tag attributes into an object</summary>
        private void ReadFontTag(ITagAttributes attributes)
        {
            string name = attributes.GetString("name");
            string fontRes = attributes.GetString("font_res");
            string size = attributes.GetString("size");
            string colour = attributes.GetString("colour");
            string align = attributes.GetString("align");
            m_tracking.AddFontDefinition(name, fontRes, size, colour, align);
        }

        /// <summary>Reads a tag attributes into an object</summary>
        private void ReadAudioTag(ITagAttributes attributes)
        {
            string name = attributes.GetString("name");
            string resName = attributes.GetString("res");
            int resIndex = m_tracking.GetAudioResourceUsedIndex(resName);
            m_tracking.AddAudioDefinition(name, resIndex);
        }

        #endregion
    }
}
