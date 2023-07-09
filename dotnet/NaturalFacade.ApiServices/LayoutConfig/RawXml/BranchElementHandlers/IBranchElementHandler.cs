using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.LayoutConfig.RawXml
{
    internal interface IBranchElementHandler : Xml.IXmlHandler
    {
        /// <summary>The data of the branch.</summary>
        Dictionary<string, object> Data { get; }
    }
}
