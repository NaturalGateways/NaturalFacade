using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.Raw
{
    public class RawLayoutConfig
    {
        public const string TYPENAME = "Raw";
    }

    public enum RawLayoutConfigElementStackHAlignment
    {
        Left,
        Centre,
        Right,
        Fill
    }

    public enum RawLayoutConfigElementStackVAlignment
    {
        Top,
        Centre,
        Bottom,
        Fill
    }

    public enum RawLayoutConfigElementImageFit
    {
        None,
        Tiled,
        Scaled
    }
}
