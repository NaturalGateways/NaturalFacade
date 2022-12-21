using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.SandboxConsole.Helpers
{
    public static class MockRawLayoutFactory
    {
        public static LayoutConfig.LayoutConfig CreateAllLayoutFromPostBody()
        {
            string jsonString = null;
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NaturalFacade.SandboxConsole.ExamplePayload.json"))
            {
                using (TextReader reader = new StreamReader(stream))
                {
                    jsonString = reader.ReadToEnd();
                }
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<LayoutConfig.LayoutConfig>(jsonString);
        }
    }
}
