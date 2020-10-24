using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using skrilla_api.Configuration;
using skrilla_api.Models;
using skrilla_api.Services;
using Xunit;

namespace SkrillaApi.Tests.Tests
{
    public class ConsumptionServiceTest
    {
        private readonly DbConnection _connection;
        private readonly SkrillaDbContext dbContext;
        private readonly ConsumptionRequest consumptionRequest;
        private readonly ConsumptionService consumptionService;
        private Consumption consumption;
        private Category category;

        protected DbContextOptions<SkrillaDbContext> ContextOptions { get; }

        public ConsumptionServiceTest()
        {
            ContextOptions = new DbContextOptionsBuilder<SkrillaDbContext>()
                   .UseSqlite(CreateInMemoryDatabase())
                   .Options;

            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;

            dbContext = new SkrillaDbContext(ContextOptions);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            consumptionRequest = new ConsumptionRequest
            {
                Title = "Example",
                Amount = 123.4,
                Category = "ExampleCategory",
                Date = new DateTime(2020, 05, 12)
            };

            category = new Category("ExampleCategory",false, "mockUser", "exampleIcon");
            dbContext.Add(category);
            dbContext.SaveChanges();

            var loggerMock = new Mock<ILogger<ConsumptionService>>();

            this.consumptionService = new ConsumptionService(loggerMock.Object, dbContext, GetMockHttpAccesor());

        }

        [Fact]
        public void CreateConsumptionSuccessful()
        {

            Consumption newConsumption = consumptionService
                .CreateConsumption(consumptionRequest);
            Assert.NotNull(newConsumption);

            int id = newConsumption.Id;

            Consumption foundConsumption = dbContext.Consumptions
                .Where(c => c.Id == id).FirstOrDefault<Consumption>();

            Assert.Equal(foundConsumption, newConsumption);
            dbContext.Remove(newConsumption);

        }

        [Fact]
        public void ConsumptionModificationIsSuccessful()
        {

            SetUpConsumption();
            Assert.NotNull(consumption);

            int id = consumption.Id;

            ConsumptionRequest modificationRequest = new ConsumptionRequest
            {
                Title = "Example2",
                Amount = 103.4,
                Category = "DifferentCategory",
                Date = new DateTime(2019, 04, 24)
            };

            consumptionService.ModifyConsumption(modificationRequest, id);

            Consumption foundConsumption = dbContext.Consumptions
                .Where(c => c.Id == id).FirstOrDefault<Consumption>();


            Assert.Equal("DifferentCategory", foundConsumption.Category.Name);
            Assert.Equal(103.4, foundConsumption.Amount);
            Assert.Equal("Example2", foundConsumption.Title);
            Assert.Equal(2019, foundConsumption.Date.Year);
            Assert.Equal(04, foundConsumption.Date.Month);
            Assert.Equal(24, foundConsumption.Date.Day);

            dbContext.Remove(consumption);
        }


        [Fact]
        public void ConsumptionDeletionIsSuccessul()
        {

            SetUpConsumption();
            Assert.NotNull(consumption);

            int id = consumption.Id;

           consumptionService.DeleteConsumption(id);

           Consumption foundConsumption = dbContext.Consumptions
                .Where(c => c.Id == id).FirstOrDefault<Consumption>();


            Assert.Null(foundConsumption);
        }

        [Fact]
        public void GetConsumptionsSuccessful()
        {

            SetUpConsumption();
            Assert.NotNull(consumption);

            int id = consumption.Id;

            List<Consumption> results = consumptionService.GetConsumptions("");

            Assert.Single<Consumption>(results);
            Assert.Equal(consumption, results.First());
        }


        [Fact]
        public void GetAConsumptionIsSuccessful()
        {

            SetUpConsumption();
            Assert.NotNull(consumption);

            int id = consumption.Id;

            Consumption result = consumptionService.GetConsumption(1);

            Assert.Equal(result, consumption);
        }

        private void SetUpConsumption()
        {

            consumption = new Consumption("TestConsumption", 123.78, category, "mockUser", new LocalDate(2020,06,5));
            dbContext.Add(consumption);
            dbContext.SaveChanges();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            return connection;
        }

        private HttpContextAccessor GetMockHttpAccesor()
        {
            HttpContextAccessor httpAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };

            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity claimsId = new ClaimsIdentity();
            claimsId.AddClaim(new Claim("userId", "mockUser"));
            principal.AddIdentity(claimsId);

            httpAccessor.HttpContext.User = principal;

            return httpAccessor; 
        }

        [Fact]
        public void Dispose() => _connection.Dispose();
    }
}
