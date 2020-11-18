using System;
using System.ComponentModel.DataAnnotations;

namespace skrilla_api.Models
{
    [Serializable]
    public class SkrillaGenericResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public SkrillaGenericResponse(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
