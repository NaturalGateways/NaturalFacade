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

        public object[] Fields { get; set; }
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
