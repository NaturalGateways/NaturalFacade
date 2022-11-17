using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.SandboxConsole.Helpers
{
    public static class MockRawLayoutFactory
    {
        private class PostBody { public PostBodyLayout PutLayout { get; set; } }
        private class PostBodyLayout { public PostBodyLayoutRaw Config { get; set; } }
        private class PostBodyLayoutRaw { public LayoutConfig.Raw.RawLayoutConfig Raw { get; set; } }

        public static LayoutConfig.Raw.RawLayoutConfig CreateLayoutFromPostBody(string json)
        {
            PostBody data = Newtonsoft.Json.JsonConvert.DeserializeObject<PostBody>(json);
            return data.PutLayout.Config.Raw;
        }
    }
}
