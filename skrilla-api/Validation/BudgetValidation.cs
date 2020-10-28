using System;
using FluentValidation;
using skrilla_api.Models.Budget;

namespace skrilla_api.Validation
{
    public class BudgetValidation : AbstractValidator<BudgetRequest>
    {
        public BudgetValidation() 
        {
            RuleFor(b => b.EndDate).Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("La fecha de finalizacion no puede ser nula.");

            RuleFor(b => b.StartDate).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("La fecha de comienzo no puede ser nula.");

            When(b => b.StartDate != null && b.EndDate != null, () => {
                RuleFor(b => b).Must(b =>
                {
                    return DateTime.Compare((DateTime)b.StartDate, (DateTime)b.EndDate) < 0;
                })
                .WithMessage("La fecha de comienzo debe ser anterior a la fecha de finalizacion.");
            });
        }
    }
}
