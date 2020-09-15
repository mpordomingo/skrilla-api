using System;
using skrilla_api.Configuration;
using skrilla_api.Models;
using Xunit;

using System.Linq;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Data.Sqlite;
using SkrillaApi.Tests.Configuration;

namespace SkrillaApi.Tests
{
    public class ConsumptionClassTest
    {
        private  DbConnection _connection;
        private SqliteContext context;

        protected DbContextOptions<SqliteContext> ContextOptions { get; }

        public ConsumptionClassTest() {
            ContextOptions = new DbContextOptionsBuilder<SqliteContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options;
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
            context = new SqliteContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            return connection;
        }

        [Fact]
        public void ConsumptionInstanceCreatedSuccessfully()
        {
            Consumption aConsumption = new Consumption("Example",50.5);
            context.Consumptions.Add(aConsumption);
            context.SaveChanges();

            Consumption consumptionFound = context.Consumptions
                .Where(c => c.Title == "Example" && c.Amount==50.5).FirstOrDefault();
            Assert.NotNull(consumptionFound);
            Assert.Equal(aConsumption, consumptionFound);
        }

        [Fact]
        public void ConsumptionInstanceCreationFailsDueToNullTitle()
        {
            Consumption invalidConsumption = new Consumption(null, 10.23);
            Assert.Throws<DbUpdateException>(()=> {
                context.Add(invalidConsumption);
                context.SaveChanges();
             });
        }

        [Fact]
        public void Dispose() => _connection.Dispose();

    }
}
