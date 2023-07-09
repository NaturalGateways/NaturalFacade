using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Natural.Xml;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal interface IBranchElementHandler : ITagHandler
    {
        /// <summary>The data of the branch.</summary>
        Dictionary<string, object> Data { get; }
    }
}
