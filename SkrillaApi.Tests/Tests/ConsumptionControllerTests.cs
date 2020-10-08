using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using skrilla_api.Controllers;
using skrilla_api.Models;
using skrilla_api.Services;
using Xunit;

namespace SkrillaApi.Tests.Tests
{
    public class ConsumptionControllerTests
    {
        private readonly ConsumptionsController controller;
        private List<Consumption> consumptions;

        public ConsumptionControllerTests()
        {
            var loggerMock = new Mock<ILogger<ConsumptionsController>>();
            var consumptionServiceMock = new Mock<IConsumptionService>();

            consumptions = InitializeConsumptionsList();

            consumptionServiceMock
                .Setup(service => service.GetConsumptions(It.IsAny<string>()))
                .Returns((string cate) => consumptions.FindAll(con => con.Category.Name.Contains(cate)));

            controller = new ConsumptionsController(loggerMock.Object, consumptionServiceMock.Object);

            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity claimsId = new ClaimsIdentity();
            claimsId.AddClaim(new Claim("userId", "mockUser"));
            principal.AddIdentity(claimsId);

            controller.ControllerContext = new ControllerContext{
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

        }

        public DefaultHttpContext HttpContext { get; }

        [Fact]
        public void GetConsumptionsReturnsListWithResults()
        {
            var result = controller.Get("");
            Assert.NotNull(result);

            Assert.Equal(2, result.Value.Count);
            Assert.True(result.Value.TrueForAll(con => consumptions.Contains(con)));
            
        }

        private List<Consumption> InitializeConsumptionsList()
        {
            List<Consumption> consumptions = new List<Consumption>();

            Category category1 = new Category("ExampleCategory", false, "mockUser");
            Category category2 = new Category("ExampleCategory2", false, "mockUser");
            Consumption consumption1 = new Consumption("Example", 345.6, category1,
                "mockUser", new LocalDate(2020, 04, 14));
            Consumption consumption2 = new Consumption("Example", 345.6, category2,
                "mockUser", new LocalDate(2020, 04, 15));

            consumptions.Add(consumption1);
            consumptions.Add(consumption2);
            return consumptions;
        }
    }
}
