using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natural.Facade.LayoutNodes
{
    public static class LayoutNodeLoader
    {
        /// <summary>Loads a root node.</summary>
        public static LayoutRootNode LoadRootNode(LayoutJson.LayoutJson json)
        {
            // Load resources
            Dictionary<string, LayoutResourceNode> resNodesByName = json.resources?.ToDictionary(x => x.Key, y => LoadResourceNode(y.Value));

            // Load root element
            LayoutElementNode rootElementNode = LoadElementNode(json.rootElement, resNodesByName);

            // Return
            return new LayoutRootNode(json, resNodesByName?.Values?.ToArray(), rootElementNode);
        }

        /// <summary>Loads a root node.</summary>
        private static LayoutResourceNode LoadResourceNode(LayoutJson.LayoutResource json)
        {
            // Parse element type
            ResourceType resourceType = ResourceType.image;
            bool parsedResourceType = Enum.TryParse<ResourceType>(json.type, true, out resourceType);
            if (parsedResourceType == false)
            {
                throw new Exception($"Cannot find resource type '{json.type}'.");
            }

            // Return
            return new LayoutResourceNode(json, resourceType);
        }

        /// <summary>Loads a root node.</summary>
        private static LayoutElementNode LoadElementNode(LayoutJson.LayoutElement json, Dictionary<string, LayoutResourceNode> resNodesByName)
        {
            // Parse element type
            ElementType elementType = ElementType.stack;
            bool parsedElementType = Enum.TryParse<ElementType>(json.elTyp, true, out elementType);
            if (parsedElementType == false)
            {
                throw new Exception($"Cannot find element type '{json.elTyp}'.");
            }

            // Create node
            LayoutElementNode elementNode = new LayoutElementNode(json, elementType);

            // Load children
            if (json.children?.Any() ?? false)
            {
                elementNode.ChildArray = json.children.Select(x => LoadElementNode(x, resNodesByName)).ToArray();
            }

            // Link to resource
            if (string.IsNullOrEmpty(json.res) == false)
            {
                if (resNodesByName.ContainsKey(json.res) == false)
                {
                    throw new Exception($"Cannot find resource '{json.res}'.");
                }
                elementNode.Resource = resNodesByName[json.res];
            }

            // Return
            return elementNode;
        }
    }
}
