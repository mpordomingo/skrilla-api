using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using skrilla_api.Models;

namespace skrilla_api.Validation
{
    public class CategoryValidation : AbstractValidator<CategoryRequest>
    {

        public CategoryValidation()
        {
            RuleFor(c => c.Name).Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("La categoria no puede ser nula.")
               .NotEmpty().WithMessage("La categoria no puede estar vacia.")
               .DependentRules(() =>
               {
                   RuleFor(c => c.Name).Must(category => category.Length < 100).Unless(c => c.Name == null)
                   .WithMessage("La categoria no debe superar los 100 caracteres.");
               });
        }
    }
}