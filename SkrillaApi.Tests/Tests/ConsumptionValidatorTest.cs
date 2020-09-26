using System;
using FluentValidation.Results;
using skrilla_api.Models;
using skrilla_api.Validation;
using Xunit;

namespace SkrillaApi.Tests.Tests
{
    public class ConsumptionValidatorTest
    {
        public ConsumptionRequest request;

        public ConsumptionValidation validator;

        public ConsumptionValidatorTest()
        {
            request = new ConsumptionRequest();
            validator = new ConsumptionValidation();
        }

        

        [Fact]
        public void ValidationIsSuccessful()
        {
            request.Title = "Example";
            request.Amount = 1323.4;
            request.Category = "Otros";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validator.Validate(request);

            Assert.Empty(result.Errors);
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidationFailsDueToEmptyTitle()
        {

            request.Title = "";
            request.Amount = 1323.4;
            request.Category = "Otros";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("El titulo no puede estar vacio o ser nulo.", summary.Messages);
        }

        [Fact]
        public void ValidationFailsDueToNullTitle()
        {

            request.Title = null;
            request.Amount = 1323.4;
            request.Category = "Otros";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("El titulo no puede estar vacio o ser nulo.", summary.Messages);
        }

        [Fact]
        public void ValidationFailsDueToNullCategory()
        {
            
            request.Title = "Example";
            request.Amount = 1323.4;
            request.Category = null;
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("La categoria no puede estar vacia o ser nula.", summary.Messages);
        }

        [Fact]
        public void ValidationFailsDueToEmptyCategory()
        {
            
            request.Title = "Example";
            request.Amount = 1323.4;
            request.Category = "";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("La categoria no puede estar vacia o ser nula.", summary.Messages);
        }

        [Fact]
        public void ValidationFailsDueToTitleTooLong()
        {

            request.Title = new String('a', 201);
            request.Amount = 1323.4;
            request.Category = "Otros";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("El titulo no debe superar los 200 caracteres.", summary.Messages);
        }

        [Fact]
        public void ValidationFailsDueToCategoryTooLong()
        {

            request.Title = "Example";
            request.Amount = 1323.4;
            request.Category = new String('a', 101);
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("La categoria no debe superar los 100 caracteres.", summary.Messages);
        }

        [Fact]
        public void ValidationFailsDueToNegativeAmount()
        {

            request.Title = "Example";
            request.Amount = -1323.4;
            request.Category = "Otros";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("El monto debe ser un numero positivo.", summary.Messages);
        }

        [Fact]
        public void ValidationFailsDueToMultipleCauses()
        {

            request.Title = new String('a', 201);
            request.Amount = -1323.4;
            request.Category = "";

            ValidationResult result = validator.Validate(request);
            ValidationSummary summary = new ValidationSummary(result);

            Assert.False(result.IsValid);
            Assert.Contains("El titulo no debe superar los 200 caracteres.", summary.Messages);
            Assert.Contains("El monto debe ser un numero positivo.", summary.Messages);
            Assert.Contains("La categoria no puede estar vacia o ser nula.", summary.Messages);
        }
    }
}
