using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.SandboxConsole.Helpers
{
    public static class MockRawLayoutFactory
    {
        public static LayoutConfig.Raw.RawLayoutConfig CreateTestLayout()
        {
            return new LayoutConfig.Raw.RawLayoutConfig
            {
                Resources = new LayoutConfig.Raw.RawLayoutConfigResource[]
                {
                    new LayoutConfig.Raw.RawLayoutConfigResource
                    {
                        Name = "StaticBacking",
                        Type = LayoutConfig.Raw.RawLayoutConfigResourceType.Image.ToString(),
                        Url = "https://httpsorigin.s3.amazonaws.com/https/Overlay/StaticBacking.webp"
                    },
                    new LayoutConfig.Raw.RawLayoutConfigResource
                    {
                        Name = "BgLight",
                        Type = LayoutConfig.Raw.RawLayoutConfigResourceType.Image.ToString(),
                        Url = "https://httpsorigin.s3.amazonaws.com/https/Overlay/UIWoodLight.jpg"
                    }
                },
                RootElement = new LayoutConfig.Raw.RawLayoutConfigElement
                {
                    Stack = new LayoutConfig.Raw.RawLayoutConfigElementStack
                    {
                        Children = new LayoutConfig.Raw.RawLayoutConfigElementStackChild[]
                        {
                            new LayoutConfig.Raw.RawLayoutConfigElementStackChild
                            {
                                Element = new LayoutConfig.Raw.RawLayoutConfigElement
                                {
                                    Image = new LayoutConfig.Raw.RawLayoutConfigElementImage
                                    {
                                        Fit = LayoutConfig.Raw.RawLayoutConfigElementImageFit.None.ToString(),
                                        Res = "StaticBacking"
                                    }
                                }
                            },
                            new LayoutConfig.Raw.RawLayoutConfigElementStackChild
                            {
                                HAlign = LayoutConfig.Raw.RawLayoutConfigElementStackHAlignment.Right.ToString(),
                                WidthPixels = 500,
                                Element = new LayoutConfig.Raw.RawLayoutConfigElement
                                {
                                    Image = new LayoutConfig.Raw.RawLayoutConfigElementImage
                                    {
                                        Fit = LayoutConfig.Raw.RawLayoutConfigElementImageFit.Tiled.ToString(),
                                        Res = "BgLight"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
