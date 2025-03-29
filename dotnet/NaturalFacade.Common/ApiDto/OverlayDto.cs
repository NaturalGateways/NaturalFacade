using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalFacade.ApiDto
{
    public class OverlayDto
    {
        public int[] canvasSize { get; set; }

        public int? redrawMillis { get; set; }

        public int? apiFetchMillis { get; set; }

        public object[] properties { get; set; }

        public string[] imageResources { get; set; }

        public string[] fontResources { get; set; }

        public string[] audioResources { get; set; }

        public string[] videoResources { get; set; }

        public OverlayDtoFont[] fonts { get; set; }

        public OverlayDtoAudio[] audios { get; set; }

        public OverlayDtoVideo[] videos { get; set; }

        public object rootElement { get; set; }
    }

    public class OverlayDtoFont
    {
        public long res { get; set; }

        public string size { get; set; }

        public string colour { get; set; }

        public string align { get; set; }
    }

    public class OverlayDtoAudio
    {
        public long res { get; set; }
        public long prop { get; set; }
    }

    public class OverlayDtoVideo
    {
        public long res { get; set; }
        public long prop { get; set; }
    }
}
