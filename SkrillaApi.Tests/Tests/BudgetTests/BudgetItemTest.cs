using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NodaTime;
using skrilla_api.Configuration;
using skrilla_api.Models;
using skrilla_api.Models.Budget;
using Xunit;

namespace SkrillaApi.Tests.Tests.BudgetTests
{
    public class BudgetItemTest
    {

        private DbConnection _connection;
        private SkrillaDbContext context;

        protected DbContextOptions<SkrillaDbContext> ContextOptions { get; }

        private readonly Category category = new Category("ExampleCategory", true, "examplePerson", "other");
        private List<BudgetItem> budgetItems = new List<BudgetItem>();

        public BudgetItemTest()
        {
            ContextOptions = new DbContextOptionsBuilder<SkrillaDbContext>()
                   .UseSqlite(CreateInMemoryDatabase())
                   .Options;
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
            context = new SkrillaDbContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Add(category);
            context.SaveChanges();

        }

        [Fact]
        public void BudgetItemIsCreatedSuccessfully()
        {
            LocalDate start = new LocalDate(2019, 05, 20);
            LocalDate end = new LocalDate(2029, 05, 20);

            Budget budget = new Budget(start, end, 23.5, "examplePerson");


            context.Add(budget);
            context.SaveChanges();

            Budget foundBudget = context.Budgets
                .Where(b => b.BudgetId == budget.BudgetId).
                FirstOrDefault();


            Assert.NotNull(foundBudget);

            BudgetItem item1 = new BudgetItem(budget, category, 12.5);

            context.Add(item1);
            context.SaveChanges();

            int itemId = item1.BudgetItemId;

            BudgetItem foundBudgetItem = context.BudgetItems
                .Where(b => b.BudgetItemId == itemId).
                FirstOrDefault();

            Assert.NotNull(foundBudgetItem);
            Assert.Equal("ExampleCategory", foundBudgetItem.Category.Name);
            Assert.Equal(12.5, foundBudgetItem.BudgetedAmount);

        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            return connection;
        }

        [Fact]
        public void Dispose() => _connection.Dispose();
    }
}
