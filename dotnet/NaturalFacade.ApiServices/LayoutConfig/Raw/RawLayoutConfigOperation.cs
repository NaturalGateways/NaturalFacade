using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.Raw
{
    public class RawLayoutConfigOperation
    {
        /// <summary>Text or Prop.</summary>
        public string Op { get; set; }

        /// <summary>The name of the property.</summary>
        public string Name { get; set; }

        /// <summary>The hardcoded text.</summary>
        public string Text { get; set; }
    }
}
