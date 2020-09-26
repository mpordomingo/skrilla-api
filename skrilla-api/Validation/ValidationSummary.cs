using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace skrilla_api.Validation
{
    public class ValidationSummary
    {
        public string Result  { get; set; }
        public List<string> Messages  { get; set; }

         public ValidationSummary(ValidationResult result)
        {
            Result = (result.IsValid) ? "passed" : "error";
            Messages = new List<string>();
            foreach (var failure in result.Errors)
            {
                Messages.Add(failure.ErrorMessage);
            }
        }
    }
}
