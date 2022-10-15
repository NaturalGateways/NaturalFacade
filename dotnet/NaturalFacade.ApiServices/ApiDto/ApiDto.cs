using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalFacade.ApiDto
{
    public class ApiResponseDto
    {
        public bool Success { get; private set; }

        public object Payload { get; private set; }

        public string Error { get; private set; }

        public ApiResponseExceptionDto[] Exception { get; private set; }

        private ApiResponseDto() { }

        public static ApiResponseDto CreateSuccess(object payload) { return new ApiResponseDto { Success = true, Payload = payload }; }

        public static ApiResponseDto CreateError(string message) { return new ApiResponseDto { Success = false, Error = message }; }

        public static ApiResponseDto CreateError(Exception ex)
        {
            List<ApiResponseExceptionDto> exceptionList = new List<ApiResponseExceptionDto>();
            while (ex != null)
            {
                exceptionList.Add(new ApiResponseExceptionDto(ex));
                ex = ex.InnerException;
            }
            return new ApiResponseDto { Success = false, Exception = exceptionList.ToArray() };
        }
    }

    public class ApiResponseExceptionDto
    {
        public string ExceptionType { get; private set; }

        public string Message { get; private set; }

        public string StackTrace { get; private set; }

        public ApiResponseExceptionDto(Exception ex)
        {
            this.ExceptionType = ex.GetType().FullName;
            this.Message = ex.Message;
            this.StackTrace = ex.StackTrace;
        }
    }
}
