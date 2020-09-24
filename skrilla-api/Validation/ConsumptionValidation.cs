using System;
using skrilla_api.Models;

namespace skrilla_api.Validation
{
    public class ConsumptionValidation
    {
        public ConsumptionValidation()
        {
        }

        public ValidationResult ValidateConsumptionRequest(ConsumptionRequest request)
        {
            ValidationResult result = new ValidationResult();
            

            if (request.Title == null || request.Title.Equals(""))
            {
                result.Messages.Add("El titulo no puede estar vacio o ser nulo.");
            }

            if (request.Title != null && request.Title.Length > 200)
            {
                result.Messages.Add("El titulo no debe superar los 200 caracteres.");
            }

            if (request.Category == null || request.Category.Equals(""))
            {
                result.Messages.Add("La categoria no puede estar vacia o ser nula.");
            }

            if (request.Category != null && request.Category.Length > 100)
            {
                result.Messages.Add("La categoria no debe superar los 100 caracteres.");
            }
            if (request.Amount < 0)
            {
                result.Messages.Add("El costo debe ser un numero positivo.");
            }


            if (result.Messages.Count > 0) { result.Result = "error"; }

            return result;
        }
    }
}
