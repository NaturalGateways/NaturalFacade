using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.SandboxConsole.Helpers
{
    public static class MockRawLayoutFactory
    {
        private class AllPostBody { public AllPostBodyLayout PutLayout { get; set; } }
        private class AllPostBodyLayout { public LayoutConfig.LayoutConfig Config { get; set; } }

        public static LayoutConfig.LayoutConfig CreateAllLayoutFromPostBody(string json)
        {
            AllPostBody data = Newtonsoft.Json.JsonConvert.DeserializeObject<AllPostBody>(json);
            return data.PutLayout.Config;
        }

        private class RawPostBody { public RawPostBodyLayout PutLayout { get; set; } }
        private class RawPostBodyLayout { public RawPostBodyLayoutRaw Config { get; set; } }
        private class RawPostBodyLayoutRaw { public LayoutConfig.Raw.RawLayoutConfig Raw { get; set; } }

        public static LayoutConfig.Raw.RawLayoutConfig CreateRawLayoutFromPostBody(string json)
        {
            RawPostBody data = Newtonsoft.Json.JsonConvert.DeserializeObject<RawPostBody>(json);
            return data.PutLayout.Config.Raw;
        }
    }
}
