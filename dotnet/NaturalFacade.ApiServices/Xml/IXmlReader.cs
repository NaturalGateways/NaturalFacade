using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.Xml
{
    /// <summary>Interface for a reader, handles ensuring the root tag is correct.</summary>
    public interface IXmlReader
    {
        /// <summary>Called when the root tag is hit to get the initial handler.</summary>
        IXmlHandler HandleRootTag(string tagName, ITagAttributes attributes);
    }
}
