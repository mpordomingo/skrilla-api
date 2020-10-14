using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using skrilla_api.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;

namespace skrilla_api.Configuration
{
    public class MysqlContext : IdentityDbContext<ApplicationUser>
    {
        public MysqlContext(DbContextOptions<MysqlContext> options) : base(options)
        {
            //this.Database.EnsureCreated();

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var dateConverter = new ValueConverter<LocalDate, DateTime>
                (l => l.ToDateTimeUnspecified(), d => LocalDate.FromDateTime(d));

            builder.Entity<Consumption>()
                .Property(p => p.Date)
                .HasConversion(dateConverter)
                .HasColumnType("date");

            builder.Entity<Category>();
            builder.Entity<Consumption>().HasOne<Category>(s => s.Category);
            builder.Entity<ConsCategory>();

            base.OnModelCreating(builder);
        }

        public virtual DbSet<Consumption> Consumptions { get; set;  }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ConsCategory> ConsCategories { get; set; }
    }
}
