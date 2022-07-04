using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.SandboxConsole.Helpers
{
    public static class MockRequestGenerator
    {
        public static ApiDto.ContextDto CreateContext()
        {
            return new ApiDto.ContextDto
            {
                userId = "MockUser",
                email = "test@test.com",
                sourceIp = "0.0.0.0",
                userAgent = "Sandbox",
                requestId = Guid.NewGuid().ToString()
            };
        }

        public static ApiDto.AuthRequestDto CreateGetCurrentUserRequest()
        {
            return new ApiDto.AuthRequestDto
            {
                payload = new ApiDto.AuthRequestPayloadDto
                {
                    RequestType = ApiDto.AuthRequestType.GetCurrentUser.ToString()
                },
                context = CreateContext()
            };
        }

        public static ApiDto.AuthRequestDto CreatePutLayoutRequest(LayoutConfig.Raw.RawLayoutConfig layoutConfig)
        {
            return new ApiDto.AuthRequestDto
            {
                payload = new ApiDto.AuthRequestPayloadDto
                {
                    RequestType = ApiDto.AuthRequestType.PutLayout.ToString(),
                    PutLayout = new ApiDto.AuthPutLayoutRequestDto
                    {
                        Config = new LayoutConfig.LayoutConfig
                        {
                            LayoutType = LayoutConfig.Raw.RawLayoutConfig.TYPENAME,
                            Name = "Test Layout",
                            Raw = layoutConfig
                        }
                    }
                },
                context = CreateContext()
            };
        }

        public static ApiDto.AuthRequestDto CreateGetLayoutOverlayRequest(string layoutId)
        {
            return new ApiDto.AuthRequestDto
            {
                payload = new ApiDto.AuthRequestPayloadDto
                {
                    RequestType = ApiDto.AuthRequestType.GetLayoutOverlay.ToString(),
                    LayoutId = layoutId
                },
                context = CreateContext()
            };
        }
    }
}
