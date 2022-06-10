using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natural.Facade.LayoutNodes
{
    public enum ResourceType
    {
        image
    }

    public class LayoutResourceNode
    {
        /// <summary>The JSON.</summary>
        public LayoutJson.LayoutResource Json { get; private set; }

        /// <summary>The resource type.</summary>
        public ResourceType ResourceType { get; private set; }

        /// <summary>Constructor.</summary>
        public LayoutResourceNode(LayoutJson.LayoutResource json, ResourceType resourceType)
        {
            this.Json = json;
            this.ResourceType = resourceType;
        }
    }
}
