using System;
using Microsoft.EntityFrameworkCore;
using skrilla_api.Models;

namespace SkrillaApi.Tests.Configuration
{
    public class SqliteContext : DbContext
    {
        public DbSet<Consumption> Consumptions { get; set; }
        public SqliteContext()
        {

        }

        public SqliteContext(DbContextOptions<SqliteContext> options) : base(options)
         {
         }
    }
}
