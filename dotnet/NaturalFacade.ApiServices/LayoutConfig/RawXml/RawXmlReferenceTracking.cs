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
            public ApiDto.PropertyTypeDto Type { get; private  set; }
            public string Name { get; private set; }
            public string DefaultValue { get; private set; }

            public int? PropIndex { get; set; }

            public Property(ApiDto.PropertyTypeDto type, string name, string defaultValue)
            {
                this.Type = type;
                this.Name = name;
                this.DefaultValue = defaultValue;
            }
        }

        /// <summary>The resources indexed.</summary>
        private Dictionary<string, Property> m_propertiesByName = new Dictionary<string, Property>();
        /// <summary>The resources indexed.</summary>
        public List<Property> PropertyUsedList { get; private set; } = new List<Property>();

        /// <summary>Adds a font definition.</summary>
        public void AddProperty(string name, ApiDto.PropertyTypeDto type, string defaultValue)
        {
            m_propertiesByName.Add(name, new Property(type, name, defaultValue));
        }

        /// <summary>Getter for the index of a resource under the context it will be used.</summary>
        public int? GetPropertyIndexIfUsed(string name)
        {
            // Get font def
            if (m_propertiesByName.ContainsKey(name) == false)
                throw new Exception($"Cannot find property with name '{name}'.");
            Property property = m_propertiesByName[name];

            // Get property index
            if (property.PropIndex.HasValue)
                return property.PropIndex.Value;
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
