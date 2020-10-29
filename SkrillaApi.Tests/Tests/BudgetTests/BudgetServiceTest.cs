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
        private Category category2;

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
            category2 = new Category("ExampleCategory2", false, "mockUser", "exampleIcon");

            dbContext.Add(category);
            dbContext.Add(category2);
            dbContext.SaveChanges();

            List<BudgetItemRequest> budgetItems = new List<BudgetItemRequest>();

            budgetItems.Add(new BudgetItemRequest {
                category = category.CategoryId,
                amount = 23.5
            });

            budgetItems.Add(new BudgetItemRequest
            {
                category = category2.CategoryId,
                amount = 101.5
            });

            budgetRequest = new BudgetRequest
            {
                StartDate = new DateTime(2019,05,06),
                EndDate = new DateTime(2020,04,17),
                Amount = 123.5,
                BudgetItems = budgetItems
            };

            var loggerMockBudget = new Mock<ILogger<BudgetService>>();

            budgetService = new BudgetService(loggerMockBudget.Object, 
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
        public void GetBudgetSummaryIsSuccessful()
        {
            Budget budget = budgetService.CreateBudget(budgetRequest);

            Category category3 = new Category("ExampleCategory3", false, "mockUser", "exampleIcon");
            Category category4 = new Category("ExampleCategory4", false, "mockUser", "exampleIcon");
            dbContext.Add(category3);
            dbContext.Add(category4);

            int id = budget.BudgetId;

            budget = dbContext.Budgets.Where(b => b.BudgetId == id).FirstOrDefault();
            Assert.NotNull(budget);

            budget = budgetService.GetBudget();
            Consumption consumption_a = new Consumption("ExampleA", 50.5, category, "mockUser", new LocalDate(2020, 03, 21));
            Consumption consumption_b = new Consumption("ExampleB", 95.3, category2, "mockUser", new LocalDate(2019, 10, 21));
            Consumption consumption_c = new Consumption("ExampleC", 45.6, category, "mockUser", new LocalDate(2016, 05, 21));
            Consumption consumption_d = new Consumption("ExampleD", 46.6, category3, "mockUser", new LocalDate(2020, 01, 21));

            dbContext.Add(consumption_a);
            dbContext.Add(consumption_b);
            dbContext.Add(consumption_c);
            dbContext.Add(consumption_d);

            dbContext.SaveChanges();

            BudgetSummary summary = budgetService.GetBudgetSummary();

            Assert.Equal(192.4, summary.TotalSpent);
            Assert.Equal(budget.Amount, summary.Amount);
            Assert.Equal(3, summary.CategoryItems.Count);
            Assert.Contains(23.5, summary.CategoryItems.Select(c => c.BudgetedAmount));
            Assert.Contains(50.5, summary.CategoryItems.Select(c => c.TotalSpent));
            Assert.Contains(101.5, summary.CategoryItems.Select(c => c.BudgetedAmount));
            Assert.Contains(95.3, summary.CategoryItems.Select(c => c.TotalSpent));
            Assert.Contains(46.6, summary.CategoryItems.Select(c => c.TotalSpent));
        }

        [Fact]
        public void GetBudgetSummaryIsSuccessfulWithNoConsumptions()
        {
            dbContext.Database.ExecuteSqlRaw("DELETE FROM consumptions");

            Budget budget = budgetService.CreateBudget(budgetRequest);

            int id = budget.BudgetId;

            budget = dbContext.Budgets.Where(b => b.BudgetId == id).FirstOrDefault();
            Assert.NotNull(budget);

            budget = budgetService.GetBudget();

            BudgetSummary summary = budgetService.GetBudgetSummary();
           
            Assert.Equal(0, summary.TotalSpent);
            Assert.Equal(budget.Amount, summary.Amount);
            
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
