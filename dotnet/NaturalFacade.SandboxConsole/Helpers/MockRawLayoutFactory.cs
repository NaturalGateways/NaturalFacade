﻿using System;
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
                    },
                    new LayoutConfig.Raw.RawLayoutConfigResource
                    {
                        Name = "BgDark",
                        Type = LayoutConfig.Raw.RawLayoutConfigResourceType.Image.ToString(),
                        Url = "https://httpsorigin.s3.amazonaws.com/https/Overlay/UIWoodDark.jpg"
                    },
                    new LayoutConfig.Raw.RawLayoutConfigResource
                    {
                        Name = "GameCover",
                        Type = LayoutConfig.Raw.RawLayoutConfigResourceType.Image.ToString(),
                        Url = "https://httpsorigin.s3.amazonaws.com/https/Overlay/GameCover.jpg"
                    },
                    new LayoutConfig.Raw.RawLayoutConfigResource
                    {
                        Name = "FakeCam",
                        Type = LayoutConfig.Raw.RawLayoutConfigResourceType.Image.ToString(),
                        Url = "https://httpsorigin.s3.amazonaws.com/https/Overlay/FakeCam.png"
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
                                                        Fit = LayoutConfig.Raw.RawLayoutConfigElementImageFit.Tiled.ToString(),
                                                        Res = "BgLight"
                                                    }
                                                }
                                            },
                                            new LayoutConfig.Raw.RawLayoutConfigElementStackChild
                                            {
                                                Element = new LayoutConfig.Raw.RawLayoutConfigElement
                                                {
                                                    VFloat = new LayoutConfig.Raw.RawLayoutConfigElementVFloat
                                                    {
                                                        Spacing = 10,
                                                        Margin = 10,
                                                        Top = new LayoutConfig.Raw.RawLayoutConfigElement
                                                        {
                                                            Image = new LayoutConfig.Raw.RawLayoutConfigElementImage
                                                            {
                                                                Fit = LayoutConfig.Raw.RawLayoutConfigElementImageFit.None.ToString(),
                                                                Res = "GameCover"
                                                            }
                                                        },
                                                        Middle = new LayoutConfig.Raw.RawLayoutConfigElement
                                                        {
                                                            Image = new LayoutConfig.Raw.RawLayoutConfigElementImage
                                                            {
                                                                Fit = LayoutConfig.Raw.RawLayoutConfigElementImageFit.Tiled.ToString(),
                                                                Res = "BgDark"
                                                            }
                                                        },
                                                        Bottom = new LayoutConfig.Raw.RawLayoutConfigElement
                                                        {
                                                            Image = new LayoutConfig.Raw.RawLayoutConfigElementImage
                                                            {
                                                                Fit = LayoutConfig.Raw.RawLayoutConfigElementImageFit.None.ToString(),
                                                                Res = "FakeCam"
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
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
