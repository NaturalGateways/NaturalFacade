using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.App.ServiceImp
{
    public class AppReleaseConfigFileService : IReleaseConfigFileService
    {
        #region Base

        /// <summary>Constructor.</summary>
        public AppReleaseConfigFileService()
        {
            // Load config file
            Stream configStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NaturalFacade.App.ServiceImp.ConfigFiles.release.json");
            TextReader textReader = new StreamReader(configStream);
            string configString = textReader.ReadToEnd();
            this.ReleaseConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ReleaseConfig.ReleaseConfig>(configString);
        }

        #endregion

        #region IReleaseConfigFileService implementation

        /// <summary>The config read from file.</summary>
        public ReleaseConfig.ReleaseConfig ReleaseConfig { get; private set; }

        #endregion
    }
}
