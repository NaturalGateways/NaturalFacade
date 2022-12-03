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

        public DateTime CreatedDateTime { get; set; }

        public string LayoutId { get; set; }

        public string Name { get; set; }

        public bool HasDraft { get; set; } = false;

        public bool HasRelease { get; set; } = false;
    }

    public class ItemLayoutSecurity
    {
        public List<string> PermittedUserIds { get; set; }
    }
}
