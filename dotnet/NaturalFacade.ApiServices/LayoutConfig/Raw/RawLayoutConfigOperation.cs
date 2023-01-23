using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.Raw
{
    public class RawLayoutConfigBooleanCondition
    {
        /// <summary>Operation type.</summary>
        public string Op { get; set; }

        /// <summary>The name of the property.</summary>
        public string Name { get; set; }

        /// <summary>The hardcoded text.</summary>
        public string Text { get; set; }

        /// <summary>The single child.</summary>
        public RawLayoutConfigBooleanCondition Child { get; set; }
        /// <summary>The children of a concatenation operation.</summary>
        public RawLayoutConfigBooleanCondition[] Children { get; set; }

        /// <summary>The left-hand-side of integer comparisons.</summary>
        public RawLayoutConfigIntegerOperation IntLhs { get; set; }
        /// <summary>The right-hand-side of integer comparisons.</summary>
        public RawLayoutConfigIntegerOperation IntRhs { get; set; }
    }

    public class RawLayoutConfigStringOperation
    {
        /// <summary>Operation type.</summary>
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

    public class RawLayoutConfigIntegerOperation
    {
        /// <summary>Operation type.</summary>
        public string Op { get; set; }

        /// <summary>The name of the property.</summary>
        public string Name { get; set; }

        /// <summary>The constant value.</summary>
        public long Value { get; set; } = 0;

        /// <summary>The lhs of any int operation.</summary>
        public RawLayoutConfigIntegerOperation IntLhs { get; set; }
        /// <summary>The rhs of any int operation.</summary>
        public RawLayoutConfigIntegerOperation IntRhs { get; set; }
    }
}
