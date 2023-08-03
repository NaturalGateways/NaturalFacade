using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NaturalFacade.LayoutConfig.RawXml
{
    public class RawXmlReferenceTracking
    {
        #region Property tracking

        /// <summary>An font referenced in the file.</summary>
        public class Property
        {
            public Config2LayoutOverlayOutputPropertyDef PropRef { get; private set; }

            public int? PropIndex { get; set; }

            public Property(Config2LayoutOverlayOutputPropertyDef propRef)
            {
                this.PropRef = propRef;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, Property> m_propertiesByName = new Dictionary<string, Property>();
        /// <summary>The resources indexed.</summary>
        public List<Property> PropertyUsedList { get; private set; } = new List<Property>();

        /// <summary>Adds a font definition.</summary>
        public void AddProperty(Config2LayoutOverlayOutputPropertyDef propRef)
        {
            m_propertiesByName.Add(propRef.Name, new Property(propRef));
        }

        /// <summary>Getter for the index of a resource under the context it will be used.</summary>
        public Property GetPropertyIfUsed(string name)
        {
            // Get font def
            if (m_propertiesByName.ContainsKey(name) == false)
                throw new Exception($"Cannot find property with name '{name}'.");
            Property property = m_propertiesByName[name];

            // Get property index
            if (property.PropIndex.HasValue)
                return property;
            return null;
        }

        /// <summary>Getter for the index of a resource under the context it will be used.</summary>
        public int GetPropertyUsedIndex(string name)
        {
            // Get font def
            if (m_propertiesByName.ContainsKey(name) == false)
                throw new Exception($"Cannot find property with name '{name}'.");
            Property property = m_propertiesByName[name];

            // Get property index
            if (property.PropIndex.HasValue == false)
            {
                property.PropIndex = this.PropertyUsedList.Count;
                this.PropertyUsedList.Add(property);
            }
            return property.PropIndex.Value;
        }

        #endregion

        #region Font definition tracking

        /// <summary>An font referenced in the file.</summary>
        public class FontDefinition
        {
            public string FontResName { get; private set; }
            public int? FontResIndex { get; set; }

            public string Size { get; private set; }
            public string Colour { get; private set; }
            public string Align { get; private set; }

            public int? ResIndex { get; set; }

            public FontDefinition(string fontResName, string size, string colour, string align)
            {
                this.FontResName = fontResName;
                this.Size = size;
                this.Colour = colour;
                this.Align = align;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, FontDefinition> m_fontDefinitionsByName = new Dictionary<string, FontDefinition>();
        /// <summary>The resources indexed.</summary>
        public List<FontDefinition> FontDefinitionsUsedList { get; private set; } = new List<FontDefinition>();

        /// <summary>Adds a font definition.</summary>
        public void AddFontDefinition(string name, string fontResName, string size, string colour, string align)
        {
            m_fontDefinitionsByName.Add(name, new FontDefinition(fontResName, size, colour, align));
        }

        /// <summary>Getter for the index of a resource under the context it will be used.</summary>
        public int GetFontDefinitionUsedIndex(string name)
        {
            // Get font def
            if (m_fontDefinitionsByName.ContainsKey(name) == false)
                throw new Exception($"Cannot find font definition with name '{name}'.");
            FontDefinition fontDef = m_fontDefinitionsByName[name];

            // Get font resource index
            if (fontDef.FontResIndex.HasValue == false)
            {
                fontDef.FontResIndex = GetFontResourceUsedIndex(fontDef.FontResName);
            }

            // Get font definition index
            if (fontDef.ResIndex.HasValue == false)
            {
                fontDef.ResIndex = this.FontDefinitionsUsedList.Count;
                this.FontDefinitionsUsedList.Add(fontDef);
            }
            return fontDef.ResIndex.Value;
        }

        #endregion

        #region Audio definition tracking

        /// <summary>An audio player referenced in the file.</summary>
        public class AudioDefinition
        {
            public int ResIndex { get; set; }
            public int PropIndex { get; set; }

            public int SavedIndex { get; set; }

            public AudioDefinition(int savedIndex, int resIndex, int propIndex)
            {
                this.SavedIndex = savedIndex;
                this.ResIndex = resIndex;
                this.PropIndex = propIndex;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, AudioDefinition> m_audioDefinitionsByName = new Dictionary<string, AudioDefinition>();
        /// <summary>The resources indexed.</summary>
        public List<AudioDefinition> AudioDefinitionsUsedList { get; private set; } = new List<AudioDefinition>();

        /// <summary>Adds a font definition.</summary>
        public void AddAudioDefinition(string name, int resIndex, int propIndex)
        {
            int savedIndex = this.AudioDefinitionsUsedList.Count;
            AudioDefinition audioDef = new AudioDefinition(savedIndex, resIndex, propIndex);
            m_audioDefinitionsByName.Add(name, audioDef);
            this.AudioDefinitionsUsedList.Add(audioDef);
        }

        #endregion

        #region Audio resource tracking

        /// <summary>An audio referenced in the file.</summary>
        public class AudioResource
        {
            public string Url { get; private set; }

            public int? ResIndex { get; set; }

            public AudioResource(string url)
            {
                this.Url = url;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, AudioResource> m_audioResourcesByName = new Dictionary<string, AudioResource>();
        /// <summary>The resources indexed.</summary>
        public List<AudioResource> AudioResourcesUsedList { get; private set; } = new List<AudioResource>();

        /// <summary>Adds an audio resource.</summary>
        public void AddAudioResource(string name, string url)
        {
            m_audioResourcesByName.Add(name, new AudioResource(url));
        }

        /// <summary>Getter for the index of a resource under the context it will be used.</summary>
        public int GetAudioResourceUsedIndex(string name)
        {
            if (m_audioResourcesByName.ContainsKey(name) == false)
            {
                throw new Exception($"Cannot find audio resource with name '{name}'.");
            }
            AudioResource res = m_audioResourcesByName[name];
            if (res.ResIndex.HasValue == false)
            {
                res.ResIndex = this.FontResourcesUsedList.Count;
                this.AudioResourcesUsedList.Add(res);
            }
            return res.ResIndex.Value;
        }

        #endregion

        #region Font resource tracking

        /// <summary>An font referenced in the file.</summary>
        public class FontResource
        {
            public string Url { get; private set; }

            public int? ResIndex { get; set; }

            public FontResource(string url)
            {
                this.Url = url;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, FontResource> m_fontResourcesByName = new Dictionary<string, FontResource>();
        /// <summary>The resources indexed.</summary>
        public List<FontResource> FontResourcesUsedList { get; private set; } = new List<FontResource>();

        /// <summary>Adds a font resource.</summary>
        public void AddFontResource(string name, string url)
        {
            m_fontResourcesByName.Add(name, new FontResource(url));
        }

        /// <summary>Getter for the index of a resource under the context it will be used.</summary>
        public int GetFontResourceUsedIndex(string name)
        {
            if (m_fontResourcesByName.ContainsKey(name) == false)
            {
                throw new Exception($"Cannot find font resource with name '{name}'.");
            }
            FontResource res = m_fontResourcesByName[name];
            if (res.ResIndex.HasValue == false)
            {
                res.ResIndex = this.FontResourcesUsedList.Count;
                this.FontResourcesUsedList.Add(res);
            }
            return res.ResIndex.Value;
        }

        #endregion

        #region Image resource tracking

        /// <summary>An image referenced in the file.</summary>
        public class ImageResource
        {
            public string Url { get; private set; }

            public int? ResIndex { get; set; }

            public ImageResource(string url)
            {
                this.Url = url;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, ImageResource> m_imageResourcesByName = new Dictionary<string, ImageResource>();
        /// <summary>The resources indexed.</summary>
        public List<ImageResource> ImageResourcesUsedList { get; private set; } = new List<ImageResource>();

        /// <summary>Adds an image resource.</summary>
        public void AddImageResource(string name, string url)
        {
            m_imageResourcesByName.Add(name, new ImageResource(url));
        }

        /// <summary>Getter for the index of a resource under the context it will be used.</summary>
        public int GetImageResourceUsedIndex(string name)
        {
            if (m_imageResourcesByName.ContainsKey(name) == false)
            {
                throw new Exception($"Cannot find image resource with name '{name}'.");
            }
            ImageResource res = m_imageResourcesByName[name];
            if (res.ResIndex.HasValue == false)
            {
                res.ResIndex = this.ImageResourcesUsedList.Count;
                this.ImageResourcesUsedList.Add(res);
            }
            return res.ResIndex.Value;
        }

        #endregion
    }
}
