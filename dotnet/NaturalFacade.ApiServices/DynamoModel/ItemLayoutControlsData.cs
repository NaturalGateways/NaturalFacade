using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ItemModel
{
    public class ItemLayoutControlsData
    {
        public string Name { get; set; }

        public ItemLayoutControlsField[] Fields { get; set; }
    }

    public class ItemLayoutControlsField
    {
        public int PropIndex { get; set; }

        public bool AllowTextEdit { get; set; } = false;

        public string[] Options { get; set; }
    }
}
