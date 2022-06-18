using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ItemModel
{
    internal class ItemLayoutSummary
    {
        public string UserId { get; set; }

        public string LayoutId { get; set; }

        public string Name { get; set; }
    }

    internal class ItemLayoutConfig
    {
        public string UserId { get; set; }

        public string LayoutId { get; set; }

        public string Name { get; set; }

        public LayoutConfig.LayoutConfig Config { get; set; }
    }
}
