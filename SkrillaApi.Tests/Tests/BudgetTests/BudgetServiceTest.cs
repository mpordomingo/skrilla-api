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
using skrilla_api.Configuration;
using skrilla_api.Models;
using skrilla_api.Models.Budget;
using skrilla_api.Services;
using Xunit;

namespace SkrillaApi.Tests.Tests.BudgetTests
{
    public class BudgetServiceTest
    {
        private readonly DbConnection _connection;
        private readonly SkrillaDbContext dbContext;
        private readonly BudgetRequest budgetRequest;
        private readonly BudgetService budgetService;
        private Category category;

        protected DbContextOptions<SkrillaDbContext> ContextOptions { get; }

        public BudgetServiceTest()
        {
            ContextOptions = new DbContextOptionsBuilder<SkrillaDbContext>()
                   .UseSqlite(CreateInMemoryDatabase())
                   .Options;

            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;

            dbContext = new SkrillaDbContext(ContextOptions);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            category = new Category("ExampleCategory", false, "mockUser", "exampleIcon");
            dbContext.Add(category);
            dbContext.SaveChanges();

            List<BudgetItemRequest> budgetItems = new List<BudgetItemRequest>();

            budgetItems.Add(new BudgetItemRequest {
                category = category.CategoryId,
                amount = 23.5
            });

            budgetRequest = new BudgetRequest
            {
                StartDate = new DateTime(2019,05,06),
                EndDate = new DateTime(2020,04,17),
                Amount = 123.5,
                BudgetItems = budgetItems
            };

            var loggerMock = new Mock<ILogger<BudgetService>>();

            budgetService = new BudgetService(loggerMock.Object, 
                dbContext,
                GetMockHttpAccesor());
        }

        [Fact]
        public void BudgetIsCreatedSuccessfully()
        {
            BudgetRequest invalidBudgetRequest = new BudgetRequest
            {
                StartDate = new DateTime(2019, 05, 06),
                EndDate = new DateTime(2020, 04, 17),
                Amount = 123.5,
                BudgetItems = new List<BudgetItemRequest>
                {
                    new BudgetItemRequest
                    {
                        category = 56563,
                        amount = 569.3
                    }
                }
            };

            SkrillaApiException ex = Assert.Throws<SkrillaApiException>(() =>
            {
                Budget budget = budgetService.CreateBudget(invalidBudgetRequest);
            });
            Assert.Equal("not_found", ex.Code);
            Assert.Equal("One or more categories were not found.", ex.Message);
        }

        [Fact]
        public void BudgetCreationFailsDueToMissingCategory()
        {

            Budget budget = budgetService.CreateBudget(budgetRequest);

            Assert.Single(budget.BudgetItems);
            int id = budget.BudgetId;

            budget = dbContext.Budgets.Where(b => b.BudgetId == id).FirstOrDefault();
            Assert.NotNull(budget);

            BudgetItem item = budget.BudgetItems.First();

            Assert.Equal(budgetRequest.Amount, budget.Amount);
            Assert.Equal(budgetRequest.BudgetItems.First().amount, item.BudgetedAmount);
        }

        [Fact]
        public void GetBudgetIsSuccessful()
        {
            Budget budget = budgetService.CreateBudget(budgetRequest);

            int id = budget.BudgetId;

            budget = dbContext.Budgets.Where(b => b.BudgetId == id).FirstOrDefault();
            Assert.NotNull(budget);

            budget = budgetService.GetBudget();
            BudgetItem item = budget.BudgetItems.First();
            Assert.Equal(budgetRequest.Amount, budget.Amount);
            Assert.Equal(budgetRequest.BudgetItems.First().amount, item.BudgetedAmount);
        }


        [Fact]
        public void GetBudgetReturnsLastAvailableBudget()
        {
            Budget budget = budgetService.CreateBudget(budgetRequest);

            BudgetRequest budgetRequest2 = new BudgetRequest
            {
                StartDate = new DateTime(2019, 05, 06),
                EndDate = new DateTime(2020, 12, 17),
                Amount = 123.5,
                BudgetItems = new List<BudgetItemRequest>()
            };

            Budget budget2 = budgetService.CreateBudget(budgetRequest2);

            int id_budget1 = budget.BudgetId;
            int id_budget2 = budget2.BudgetId;

            budget = dbContext.Budgets.Where(b => b.BudgetId == id_budget1).FirstOrDefault();
            budget2 = dbContext.Budgets.Where(b => b.BudgetId == id_budget2).FirstOrDefault();
            Assert.NotNull(budget);
            Assert.NotNull(budget2);

            Budget budgetFromService = budgetService.GetBudget();
            Assert.Equal(budget2, budgetFromService);
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
