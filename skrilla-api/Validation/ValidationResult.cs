using System;
using System.Collections.Generic;

namespace skrilla_api.Validation
{
    public class ValidationResult
    {
        public string Result { get; set; }
        public List<string> Messages { get; set; }
        public ValidationResult()
        {
            this.Result = "passed";
            this.Messages =  new List<string>();
        }

        public bool Passed()
        {
            return this.Result.Equals("passed");
        }

    }
}
