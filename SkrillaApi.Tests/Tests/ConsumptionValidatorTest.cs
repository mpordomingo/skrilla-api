using System;
using skrilla_api.Models;
using skrilla_api.Validation;
using Xunit;

namespace SkrillaApi.Tests.Tests
{
    public class ConsumptionValidatorTest
    {
        public ConsumptionRequest request;

        public ConsumptionValidation validation;

        public ConsumptionValidatorTest()
        {
            request = new ConsumptionRequest();
        }

        

        [Fact]
        public void ValidationIsSuccessful()
        {
            validation = new ConsumptionValidation();

            request.Title = "Example";
            request.Amount = 1323.4;
            request.Category = "Otros";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validation.ValidateConsumptionRequest(request);

            Assert.Empty(result.Messages);
            Assert.True(result.Passed());
            Assert.Empty(result.Messages);
        }

        [Fact]
        public void ValidationFailsDueToEmptyTitle()
        {
            validation = new ConsumptionValidation();

            request.Title = "";
            request.Amount = 1323.4;
            request.Category = "Otros";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validation.ValidateConsumptionRequest(request);

            Assert.False(result.Passed());
            Assert.Single<string>(result.Messages);
            Assert.Contains("El titulo no puede estar vacio o ser nulo.", result.Messages);
        }

        [Fact]
        public void ValidationFailsDueToNullTitle()
        {
            validation = new ConsumptionValidation();

            request.Title = null;
            request.Amount = 1323.4;
            request.Category = "Otros";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validation.ValidateConsumptionRequest(request);

            Assert.False(result.Passed());
            Assert.Single<string>(result.Messages);
            Assert.Contains("El titulo no puede estar vacio o ser nulo.", result.Messages);
        }

        [Fact]
        public void ValidationFailsDueToNullCategory()
        {
            validation = new ConsumptionValidation();

            request.Title = "Example";
            request.Amount = 1323.4;
            request.Category = null;
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validation.ValidateConsumptionRequest(request);

            Assert.False(result.Passed());
            Assert.Single<string>(result.Messages);
            Assert.Contains("La categoria no puede estar vacia o ser nula.", result.Messages);
        }

        [Fact]
        public void ValidationFailsDueToEmptyCategory()
        {
            validation = new ConsumptionValidation();

            request.Title = "Example";
            request.Amount = 1323.4;
            request.Category = "";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validation.ValidateConsumptionRequest(request);

            Assert.False(result.Passed());
            Assert.Single<string>(result.Messages);
            Assert.Contains("La categoria no puede estar vacia o ser nula.", result.Messages);
        }

        [Fact]
        public void ValidationFailsDueToTitleTooLong()
        {
            validation = new ConsumptionValidation();

            request.Title = new String('a', 201);
            request.Amount = 1323.4;
            request.Category = "Otros";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validation.ValidateConsumptionRequest(request);

            Assert.False(result.Passed());
            Assert.Single<string>(result.Messages);
            Assert.Contains("El titulo no debe superar los 200 caracteres.", result.Messages);
        }

        [Fact]
        public void ValidationFailsDueToCategoryTooLong()
        {
            validation = new ConsumptionValidation();

            request.Title = "Example";
            request.Amount = 1323.4;
            request.Category = new String('a', 101);
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validation.ValidateConsumptionRequest(request);

            Assert.False(result.Passed());
            Assert.Single<string>(result.Messages);
            Assert.Contains("La categoria no debe superar los 100 caracteres.", result.Messages);
        }

        [Fact]
        public void ValidationFailsDueToNegativeAmount()
        {
            validation = new ConsumptionValidation();

            request.Title = "Example";
            request.Amount = -1323.4;
            request.Category = "Otros";
            request.Date = new DateTime(2020,04,05);

            ValidationResult result = validation.ValidateConsumptionRequest(request);

            Assert.False(result.Passed());
            Assert.Single<string>(result.Messages);
            Assert.Contains("El costo debe ser un numero positivo.", result.Messages);
        }

        [Fact]
        public void ValidationFailsDueToMultipleCauses()
        {
            validation = new ConsumptionValidation();

            request.Title = new String('a', 201);
            request.Amount = -1323.4;
            request.Category = "";

            ValidationResult result = validation.ValidateConsumptionRequest(request);

            Assert.False(result.Passed());
            Assert.Contains("El titulo no debe superar los 200 caracteres.", result.Messages);
            Assert.Contains("El costo debe ser un numero positivo.", result.Messages);
            Assert.Contains("La categoria no puede estar vacia o ser nula.", result.Messages);
        }
    }
}
