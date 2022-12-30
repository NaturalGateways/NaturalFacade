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

        public bool SaveAll { get; set; } = false;

        public ItemLayoutControlsField[] Fields { get; set; }
    }

    public class ItemLayoutControlsField
    {
        public string Label { get; set; }

        public int PropIndex { get; set; }

        public bool AllowTextEdit { get; set; } = false;

        public string[] Options { get; set; }

        public ItemLayoutControlsFieldInteger Integer { get; set; }

        public ItemLayoutControlsFieldSwitch Switch { get; set; }
    }

    public class ItemLayoutControlsFieldInteger
    {
        public int? MinValue { get; set; }

        public int? MaxValue { get; set; }

        public int Step { get; set; } = 1;
    }

    public class ItemLayoutControlsFieldSwitch
    {
        public string FalseLabel { get; set; }

        public string TrueLabel { get; set; }
    }
}
