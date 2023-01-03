using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.Raw
{
    public class RawLayoutConfigBooleanCondition
    {
        /// <summary>Text or Prop.</summary>
        public string Op { get; set; }

        /// <summary>The name of the property.</summary>
        public string Name { get; set; }

        /// <summary>The hardcoded text.</summary>
        public string Text { get; set; }

        /// <summary>The single child.</summary>
        public RawLayoutConfigBooleanCondition Child { get; set; }
        /// <summary>The children of a concatenation operation.</summary>
        public RawLayoutConfigBooleanCondition[] Children { get; set; }
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

        /// <summary>The condition of an if statement.</summary>
        public RawLayoutConfigBooleanCondition If { get; set; }
        /// <summary>The then result of an if statement.</summary>
        public RawLayoutConfigStringOperation Then { get; set; }
        /// <summary>The else result of an if statement.</summary>
        public RawLayoutConfigStringOperation Else { get; set; }
    }
}
