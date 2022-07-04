using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ApiDto
{
    public class AuthRequestDto : CommonDto<AuthRequestPayloadDto>
    {
        //
    }

    public enum AuthRequestType
    {
        GetCurrentUser,
        GetLayoutOverlay,
        PutLayout
    }

    public class AuthRequestPayloadDto
    {
        public string RequestType { get; set; }

        public string LayoutId { get; set; }

        public AuthPutLayoutRequestDto PutLayout { get; set; }
    }

    public class AuthResponseDto
    {
        public bool Success { get; private set; }

        public object Payload { get; private set; }

        public string Error { get; private set; }

        private AuthResponseDto() { }

        public static AuthResponseDto CreateSuccess(object payload) { return new AuthResponseDto { Success = true, Payload = payload }; }

        public static AuthResponseDto CreateError(string message) { return new AuthResponseDto { Success = false, Error = message }; }
    }
}
