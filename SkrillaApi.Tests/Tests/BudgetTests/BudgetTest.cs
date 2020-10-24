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
    public class BudgetTest
    {

        private DbConnection _connection;
        private SkrillaDbContext context;

        protected DbContextOptions<SkrillaDbContext> ContextOptions { get; }

        private readonly Category category = new Category("ExampleCategory", true, "examplePerson", "other");
      

        public BudgetTest()
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
        public void BudgetIsCreatedSuccessfully()
        {
            LocalDate start = new LocalDate(2019, 05, 20);
            LocalDate end = new LocalDate(2029, 05, 20);

            Budget budget = new Budget(start, end, 123.5, "examplePerson");
           

            context.Add(budget);
            context.SaveChanges();

            int budgetId = budget.BudgetId;

            Budget foundBudget = context.Budgets
                .Where( b => b.BudgetId == budgetId).
                FirstOrDefault();


            Assert.NotNull(foundBudget);
            Assert.Equal(budget, foundBudget);

            BudgetItem item1 = new BudgetItem(budget, category, 12.5);
            BudgetItem item2 = new BudgetItem(budget, category, 24.3);

            context.Add(item1);
            context.Add(item2);
            context.SaveChanges();

            foundBudget = context.Budgets
                .Where(b => b.BudgetId == budgetId).
                FirstOrDefault();

            Assert.Equal(2, foundBudget.BudgetItems.Count);

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
