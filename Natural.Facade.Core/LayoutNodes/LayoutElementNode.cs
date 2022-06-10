using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natural.Facade.LayoutNodes
{
    public enum ElementType
    {
        stack,
        image
    }

    public class LayoutElementNode
    {
        /// <summary>The JSON.</summary>
        public LayoutJson.LayoutElement Json { get; private set; }

        /// <summary>The element type.</summary>
        public ElementType ElementType { get; private set; }

        /// <summary>The resource node.</summary>
        public LayoutResourceNode Resource { get; set; }

        /// <summary>The array of children.</summary>
        public LayoutElementNode[] ChildArray { get; set; }

        /// <summary>Constructor.</summary>
        public LayoutElementNode(LayoutJson.LayoutElement json, ElementType elementType)
        {
            this.Json = json;
            this.ElementType = elementType;
        }
    }
}
