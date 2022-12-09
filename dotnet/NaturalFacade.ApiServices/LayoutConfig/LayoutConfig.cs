using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig
{
    public class LayoutConfig
    {
        /// <summary>The layout type.</summary>
        public string LayoutType { get; set; }

        /// <summary>The layout name.</summary>
        public string Name { get; set; }

        /// <summary>The width of the canvas.</summary>
        public int? Width { get; set; }

        /// <summary>The height of the canvas.</summary>
        public int? Height { get; set; }

        /// <summary>The raw version of the layout.</summary>
        public Raw.RawLayoutConfig Raw { get; set; }
    }
}
