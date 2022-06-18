using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ApiDto
{
    public enum AuthRequestType
    {
        PutLayout
    }

    public class AuthRequestDto
    {
        public string AuthType { get; set; }

        public AuthRequestDtoLayout Layout { get; set; }
    }

    public class AuthRequestDtoLayout
    {
        public string LayoutId { get; set; }

        public LayoutConfig.LayoutConfig Config { get; set; }
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

    public class AuthResponseDtoLayout
    {
        public string LayoutId { get; set; }
    }
}
