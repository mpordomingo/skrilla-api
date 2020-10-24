using System;
using skrilla_api.Models;
using Xunit;

namespace SkrillaApi.Tests.Tests
{
    public class ConsumptionResquestTest
    {
        public ConsumptionResquestTest()
        {
        }

        [Fact]
        public void ConsumptionRequestCreatedSuccessfully()
        {
            ConsumptionRequest request = new ConsumptionRequest();
            request.Title = "Example";
            request.Category = "Otros";
            request.Amount = 234.5;
            request.Date = new DateTime(2020,5,7);

            Assert.NotNull(request);
            Assert.Equal(234.5, request.Amount);
            Assert.Equal("Otros", request.Category);
            Assert.Equal("Example", request.Title);
        }
    }
}
