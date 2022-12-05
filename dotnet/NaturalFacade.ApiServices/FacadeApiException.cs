using System;

namespace NaturalFacade
{
    public class FacadeApiException : Exception
    {
        public string UserMessage { get; private set; }

        public FacadeApiException(string message)
            : base(message)
        {
            this.UserMessage = message;
        }

        public FacadeApiException(string userMessage, string devMessage)
            : base(devMessage)
        {
            this.UserMessage = userMessage;
        }

        public FacadeApiException(string message, Exception ex)
            : base(message, ex)
        {
            this.UserMessage = message;
        }

        public FacadeApiException(string userMessage, string devMessage, Exception ex)
            : base(devMessage, ex)
        {
            this.UserMessage = userMessage;
        }
    }
}
