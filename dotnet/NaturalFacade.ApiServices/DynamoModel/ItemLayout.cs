using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ItemModel
{
    public class ItemLayoutSummary
    {
        public string UserId { get; set; }

        public string LayoutId { get; set; }

        public string Name { get; set; }
    }

    public class ItemLayoutConfig
    {
        public string UserId { get; set; }

        public string LayoutId { get; set; }

        public string Name { get; set; }

        public LayoutConfig.LayoutConfig Config { get; set; }
    }
}
