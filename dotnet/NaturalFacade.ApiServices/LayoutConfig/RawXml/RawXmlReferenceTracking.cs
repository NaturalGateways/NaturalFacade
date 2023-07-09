using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    public class RawXmlReferenceTracking
    {
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
