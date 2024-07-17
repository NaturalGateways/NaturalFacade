using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.App
{
    public interface IReleaseConfigFileService
    {
        /// <summary>The config read from file.</summary>
        ReleaseConfig.ReleaseConfig ReleaseConfig { get; }
    }
}
