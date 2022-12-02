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

        public static ApiDto.AnonRequestDto CreateGetInfoRequest()
        {
            return new ApiDto.AnonRequestDto
            {
                payload = new ApiDto.AnonRequestPayloadDto
                {
                    RequestType = ApiDto.AnonRequestType.GetInfo.ToString()
                }
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
    }
}
