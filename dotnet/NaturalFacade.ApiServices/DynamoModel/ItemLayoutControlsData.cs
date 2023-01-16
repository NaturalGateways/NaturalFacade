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
}
