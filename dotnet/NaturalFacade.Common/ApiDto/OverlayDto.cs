using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalFacade.ApiDto
{
    public class OverlayDto
    {
        public Dictionary<string, string> parameters { get; set; }

        public Dictionary<string, string> imageResources { get; set; }

        public Dictionary<string, string> fontResources { get; set; }

        public Dictionary<string, OverlayDtoFont> fonts { get; set; }

        public object rootElement { get; set; }
    }

    public class OverlayDtoFont
    {
        public string fontRes { get; set; }

        public string size { get; set; }

        public string colour { get; set; }
    }
}
