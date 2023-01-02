using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.Raw
{
    public class RawLayoutConfigBooleanOperation
    {
        /// <summary>Text or Prop.</summary>
        public string Op { get; set; }

        /// <summary>The name of the property.</summary>
        public string Name { get; set; }

        /// <summary>The hardcoded text.</summary>
        public string Text { get; set; }

        /// <summary>The children of a concatenation operation.</summary>
        public RawLayoutConfigBooleanOperation[] Children { get; set; }
    }

    public class RawLayoutConfigStringOperation
    {
        /// <summary>Text or Prop.</summary>
        public string Op { get; set; }

        /// <summary>The name of the property.</summary>
        public string Name { get; set; }

        /// <summary>The hardcoded text.</summary>
        public string Text { get; set; }

        /// <summary>The children of a concatenation operation.</summary>
        public RawLayoutConfigStringOperation[] Children { get; set; }
    }
}
