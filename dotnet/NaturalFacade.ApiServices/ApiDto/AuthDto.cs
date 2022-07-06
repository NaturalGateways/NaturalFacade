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

        public AuthResponseExceptionDto[] Exception { get; private set; }

        private AuthResponseDto() { }

        public static AuthResponseDto CreateSuccess(object payload) { return new AuthResponseDto { Success = true, Payload = payload }; }

        public static AuthResponseDto CreateError(string message) { return new AuthResponseDto { Success = false, Error = message }; }

        public static AuthResponseDto CreateError(Exception ex)
        {
            List<AuthResponseExceptionDto> exceptionList = new List<AuthResponseExceptionDto>();
            while (ex != null)
            {
                exceptionList.Add(new AuthResponseExceptionDto(ex));
                ex = ex.InnerException;
            }
            return new AuthResponseDto { Success = false, Exception = exceptionList.ToArray() };
        }
    }

    public class AuthResponseExceptionDto
    {
        public string ExceptionType { get; private set; }

        public string Message { get; private set; }

        public string StackTrace { get; private set; }

        public AuthResponseExceptionDto(Exception ex)
        {
            this.ExceptionType = ex.GetType().FullName;
            this.Message = ex.Message;
            this.StackTrace = ex.StackTrace;
        }
    }
}
