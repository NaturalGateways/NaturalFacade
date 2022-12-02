using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ItemModel
{
    public class ItemLayoutSummary
    {
        public string CreatorUserId { get; set; }

        public string LayoutId { get; set; }

        public string Name { get; set; }

        public List<string> UserIds { get; set; }
    }
}
