using System;
namespace skrilla_api.Services
{
    public class SkrillaApiException : Exception
    {
        public string Code { get; }

        public SkrillaApiException(string code, string message):
            base(message)
        {
            this.Code = code;
        }
    }
}
