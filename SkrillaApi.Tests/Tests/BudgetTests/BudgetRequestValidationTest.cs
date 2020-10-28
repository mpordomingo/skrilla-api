using System;
using FluentValidation.Results;
using skrilla_api.Models.Budget;
using skrilla_api.Validation;
using Xunit;

namespace SkrillaApi.Tests.Tests.BudgetTests
{
    public class BudgetRequestValidationTest
    {
        public BudgetRequest request;

        public BudgetValidation validator;

        public BudgetRequestValidationTest()
        {
            request = new BudgetRequest();
            validator = new BudgetValidation();
        }

        [Fact]
        public void BugetRequestValidationPassesSuccessfully()
        {
            request.StartDate = new DateTime(2019, 09, 16);
            request.EndDate = new DateTime(2020, 05, 20);
            request.Amount = 1232.6;

            ValidationResult result = validator.Validate(request);

            Assert.Empty(result.Errors);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void BugetRequestValidationFailsDueToNullStartDate()
        {
            request.StartDate = null;
            request.EndDate = new DateTime(2020, 06, 12);
            request.Amount = 1232.6;

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("La fecha de comienzo no puede ser nula.", summary.Messages);

        }

        [Fact]
        public void BugetRequestValidationFailsDueToNullEnd()
        {
            
            request.StartDate = new DateTime(2020, 06, 12);
            request.EndDate = null;
            request.Amount = 1232.6;

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("La fecha de finalizacion no puede ser nula.", summary.Messages);
        }

        [Fact]
        public void BugetRequestValidationFailsDueToNullDates()
        {
            request.StartDate = null;
            request.EndDate = null;
            request.Amount = 1232.6;

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count);
            Assert.Contains("La fecha de finalizacion no puede ser nula.", summary.Messages);
            Assert.Contains("La fecha de comienzo no puede ser nula.", summary.Messages);
        }

        [Fact]
        public void BugetRequestValidationFailesDueToEndDateIsEarlierThanStartDate()
        {
            request.StartDate = new DateTime(2020, 05, 06);
            request.EndDate = new DateTime(2019, 06, 12);
            request.Amount = 1232.6;

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("La fecha de comienzo debe ser anterior a la fecha de finalizacion.", summary.Messages);

            request.StartDate = new DateTime(2020, 05, 06);
            request.EndDate = new DateTime(2020, 03, 12);

            result = validator.Validate(request);
            summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("La fecha de comienzo debe ser anterior a la fecha de finalizacion.", summary.Messages);

            request.StartDate = new DateTime(2020, 05, 06);
            request.EndDate = new DateTime(2020, 05, 02);

            result = validator.Validate(request);
            summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("La fecha de comienzo debe ser anterior a la fecha de finalizacion.", summary.Messages);
        }


    }
}
