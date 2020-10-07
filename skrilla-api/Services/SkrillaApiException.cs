using System;
namespace skrilla_api.Services
{
    public class SkrillaApiException : Exception
    {
        public string Code { get; }
        public string Message { get; }

        public SkrillaApiException(string code, string message)
        {
            this.Code = code;
            this.Message = message;
        }
    }
}
