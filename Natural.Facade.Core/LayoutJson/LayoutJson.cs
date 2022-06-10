using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natural.Facade.LayoutJson
{
    public class LayoutJson
    {
        public Dictionary<string, LayoutResource> resources { get; set; }

        public LayoutElement rootElement { get; set; }
    }

    public class LayoutResource
    {
        public string type { get; set; }

        public string url { get; set; }
    }

    public class LayoutElement
    {
        public string elTyp { get; set; }

        public string res { get; set; }

        public string fit { get; set; }

        public LayoutElement[] children { get; set; }
    }
}
