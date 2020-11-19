using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using skrilla_api.Configuration;
using skrilla_api.Controllers;
using skrilla_api.Models;
using skrilla_api.Models.Budget;
using skrilla_api.Services;
using Xunit;

namespace SkrillaApi.Tests.Tests.BudgetTests
{
    public class BudgetControllerTest
    {
        private readonly DbConnection _connection;
        private readonly SkrillaDbContext dbContext;
        private readonly BudgetRequest budgetRequest;
        private readonly ClaimsPrincipal principal = new ClaimsPrincipal();
        private readonly ClaimsIdentity claimsId = new ClaimsIdentity();
        private readonly BudgetController controller;
        private readonly BudgetService budgetService;
        private Category category;
        private Category category2;

        protected DbContextOptions<SkrillaDbContext> ContextOptions { get; }

        public BudgetControllerTest()
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

            List<BudgetItemRequest> budgetItems = new List<BudgetItemRequest>
            {
                new BudgetItemRequest
                {
                    category = category.CategoryId,
                    amount = 23.5
                },

                new BudgetItemRequest
                {
                    category = category2.CategoryId,
                    amount = 101.5
                }
            };

            budgetRequest = new BudgetRequest
            {
                StartDate = new DateTime(2019, 05, 06),
                EndDate = new DateTime(2030, 04, 17),
                Amount = 123.5,
                BudgetItems = budgetItems
            };

            var budgetLoggerMock = new Mock<ILogger<BudgetService>>();
            var controllerLoggerMock = new Mock<ILogger<BudgetController>>();

            budgetService = new BudgetService(budgetLoggerMock.Object,
                dbContext,
                GetMockHttpAccesor());

            controller = new BudgetController(controllerLoggerMock.Object, budgetService);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [Fact]
        public void CreateBudgetSuccessful()
        {
            var result = controller.Post(budgetRequest).Result as ObjectResult;
            var resultValue = result.Value;

            Assert.Equal(budgetRequest.Amount, ((Budget)resultValue).Amount);
            Assert.Equal(LocalDate.FromDateTime((DateTime)budgetRequest.StartDate), ((Budget)resultValue).StartDate);
            Assert.Equal(LocalDate.FromDateTime((DateTime)budgetRequest.EndDate), ((Budget)resultValue).EndDate);

        }

        [Fact]
        public void ModifyBudgetSuccessful()
        {
            Budget budget = budgetService.CreateBudget(budgetRequest);
            BudgetItemRequest itemRequest = new BudgetItemRequest();

            itemRequest.category = category.CategoryId;
            itemRequest.budgetId = budget.BudgetId;
            itemRequest.amount = 5.5;
            itemRequest.ForceRequest = false;

            var result = controller.ModifyCategoryBudget(itemRequest).Result as ObjectResult;
            SkrillaGenericResponse resultValue = (SkrillaGenericResponse) result.Value;

            Assert.Equal("success", resultValue.Code);
            Assert.Equal("Item de categoria actualizado exitosamente.", resultValue.Message);
           
        }

        [Fact]
        public void ModifyBudgetFailsDueToBudgetOverflow()
        {
            Budget budget = budgetService.CreateBudget(budgetRequest);
            BudgetItemRequest itemRequest = new BudgetItemRequest();

            itemRequest.category = category.CategoryId;
            itemRequest.budgetId = budget.BudgetId;
            itemRequest.amount = 10005.5;
            itemRequest.ForceRequest = false;

            var result = controller.ModifyCategoryBudget(itemRequest).Result as ObjectResult;
            SkrillaGenericResponse resultValue = (SkrillaGenericResponse) result.Value;

            Assert.Equal("budget_overflow", resultValue.Code);
            Assert.Equal("El monto presupuestado total de las categorias no puede superar el moto general del presupuesto. ", resultValue.Message);
           
        }

        [Fact]
        public void ModifyBudgetFailsDueToMissingCategory()
        {
            Budget budget = budgetService.CreateBudget(budgetRequest);
            BudgetItemRequest itemRequest = new BudgetItemRequest();

            itemRequest.category = 123456;
            itemRequest.budgetId = budget.BudgetId;
            itemRequest.amount = 10005.5;
            itemRequest.ForceRequest = false;

            var result = controller.ModifyCategoryBudget(itemRequest).Result as ObjectResult;
            SkrillaGenericResponse resultValue = (SkrillaGenericResponse) result.Value;

            Assert.Equal("not_found", resultValue.Code);
            Assert.Equal("La categoria indicada no fue encontrada. ", resultValue.Message);
           
        }

        [Fact]
        public void ModifyBudgetFailsDueToOldBudget()
        {
            BudgetRequest oldBudgetRequest = new BudgetRequest
            {
                StartDate = new DateTime(2019, 05, 06),
                EndDate = new DateTime(2019, 08, 17),
                Amount = 123.5,
                BudgetItems = new List<BudgetItemRequest>()
            };

            Budget budget = budgetService.CreateBudget(oldBudgetRequest);
            BudgetItemRequest itemRequest = new BudgetItemRequest();

            itemRequest.category = 123456;
            itemRequest.budgetId = budget.BudgetId;
            itemRequest.amount = 10005.5;
            itemRequest.ForceRequest = false;

            var result = controller.ModifyCategoryBudget(itemRequest).Result as ObjectResult;
            SkrillaGenericResponse resultValue = (SkrillaGenericResponse) result.Value;

            Assert.Equal("invalid_request", resultValue.Code);
            Assert.Equal("No se puede alterar presupuestos pasados.", resultValue.Message);
           
        }

        [Fact]
        public void GetBudgetSummaryByIdIsSuccessful()
        {
            Budget budget = budgetService.CreateBudget(budgetRequest);

            var result = controller.GetBudgetSummaryById(budget.BudgetId);
            BudgetSummary resultValue = (BudgetSummary) result.Value;

            Assert.Equal(budget.BudgetId, resultValue.BudgetId);
            Assert.Equal(budget.Amount, resultValue.Amount);
           
        }

        [Fact]
        public void GetBudgetList()
        {
            BudgetRequest secondBudgetRequest = new BudgetRequest
            {
                StartDate = new DateTime(2030, 05, 06),
                EndDate = new DateTime(2040, 04, 17),
                Amount = 123.5,
                BudgetItems = new List<BudgetItemRequest>()
            };

            Budget firstBudget = budgetService.CreateBudget(budgetRequest);
            Budget secondBudget = budgetService.CreateBudget(secondBudgetRequest);

            var result = controller.GetBudgetList();
            List<Budget> budgetList = (List<Budget>) result.Value;

            Assert.True(budgetList.Exists(b => b.BudgetId.Equals(firstBudget.BudgetId) &&
                b.Amount.Equals(firstBudget.Amount)
            ));
            Assert.True(budgetList.Exists(b => b.BudgetId.Equals(secondBudget.BudgetId) &&
                b.Amount.Equals(secondBudget.Amount)));

           
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

            
            claimsId.AddClaim(new Claim("userId", "mockUser"));
            principal.AddIdentity(claimsId);

            httpAccessor.HttpContext.User = principal;

            return httpAccessor;
        }

        [Fact]
        public void Dispose() => _connection.Dispose();
    }
}
