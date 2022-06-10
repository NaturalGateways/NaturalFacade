using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natural.Facade.LayoutNodes
{
    public class LayoutRootNode
    {
        /// <summary>The JSON.</summary>
        public LayoutJson.LayoutJson Json { get; private set; }

        /// <summary>The resources.</summary>
        public LayoutResourceNode[] ResNodeArray { get; private set; }

        /// <summary>The root node.</summary>
        public LayoutElementNode RootElementNode { get; private set; }

        /// <summary>Constructor.</summary>
        public LayoutRootNode(LayoutJson.LayoutJson json, LayoutResourceNode[] resNodeArray, LayoutElementNode rootElementNode)
        {
            this.Json = json;
            this.ResNodeArray = resNodeArray;
            this.RootElementNode = rootElementNode;
        }
    }
}
