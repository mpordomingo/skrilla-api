using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using skrilla_api.Models;

namespace SkrillaApi.Tests.Configuration
{
    public class SqliteContext : DbContext
    {
        public DbSet<Consumption> Consumptions { get; set; }
        public DbSet<Category> Categories { get; set; }

        public SqliteContext()
        {

        }

        public SqliteContext(DbContextOptions<SqliteContext> options) : base(options)
         {
         }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var dateConverter = new ValueConverter<LocalDate, DateTime>
                (l => l.ToDateTimeUnspecified(), d => LocalDate.FromDateTime(d));

            builder.Entity<Category>();

            builder.Entity<Consumption>()
                .Property(p => p.Date)
                .HasConversion(dateConverter)
                .HasColumnType("date");
            
            builder.Entity<Consumption>().HasOne(s => s.Category);
            base.OnModelCreating(builder);
        }
        
    }
}
