using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalFacade.ApiDto
{
    public class OverlayDto
    {
        public Dictionary<string, string> parameters { get; set; }

        public string[] imageResources { get; set; }

        public string[] fontResources { get; set; }

        public OverlayDtoFont[] fonts { get; set; }

        public object rootElement { get; set; }
    }

    public class OverlayDtoFont
    {
        public int res { get; set; }

        public string size { get; set; }

        public string colour { get; set; }

        public string align { get; set; }
    }
}
