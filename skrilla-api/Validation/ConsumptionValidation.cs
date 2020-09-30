using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using skrilla_api.Models;

namespace skrilla_api.Validation
{
    public class ConsumptionValidation : AbstractValidator<ConsumptionRequest>
    {

        public ConsumptionValidation()
        {
            RuleFor(c => c.Title)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("El titulo no puede ser nulo.")
                .NotEmpty().WithMessage("El titulo no puede estar vacio.")
                .DependentRules(() =>
                    {
                        RuleFor(c => c.Title).Must(title => title.Length < 200)
                           .WithMessage("El titulo no debe superar los 200 caracteres.");
                    });
     
            RuleFor(c => c.Amount).Must(amount => amount > 0)
                .WithMessage("El monto debe ser un numero positivo.");
        }

     
    }
}