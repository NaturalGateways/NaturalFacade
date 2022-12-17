using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.SandboxConsole.Helpers
{
    public static class MockRawLayoutFactory
    {
        private class AllPostBody { public LayoutConfig.LayoutConfig Config { get; set; } }

        public static LayoutConfig.LayoutConfig CreateAllLayoutFromPostBody(string json)
        {
            AllPostBody data = Newtonsoft.Json.JsonConvert.DeserializeObject<AllPostBody>(json);
            return data.Config;
        }

        private class RawPostBody { public RawPostBodyLayoutRaw Config { get; set; } }
        private class RawPostBodyLayoutRaw { public LayoutConfig.Raw.RawLayoutConfig Raw { get; set; } }

        public static LayoutConfig.Raw.RawLayoutConfig CreateRawLayoutFromPostBody(string json)
        {
            RawPostBody data = Newtonsoft.Json.JsonConvert.DeserializeObject<RawPostBody>(json);
            return data.Config.Raw;
        }
    }
}
