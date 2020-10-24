using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using skrilla_api.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using skrilla_api.Models.Budget;

namespace skrilla_api.Configuration
{
    public class SkrillaDbContext : IdentityDbContext<ApplicationUser>
    {
        public SkrillaDbContext(DbContextOptions<SkrillaDbContext> options) : base(options)
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

            builder.Entity<Budget>()
                .Property(p => p.StartDate)
                .HasConversion(dateConverter)
                .HasColumnType("date");

            builder.Entity<Budget>()
               .Property(p => p.EndDate)
               .HasConversion(dateConverter)
               .HasColumnType("date");

            builder.Entity<BudgetItem>()
                .HasOne<Budget>(b => b.Budget)
                .WithMany(b => b.BudgetItems);

            builder.Entity<BudgetItem>().HasOne<Category>(b => b.Category);

            builder.Entity<Category>();
            builder.Entity<Consumption>().HasOne<Category>(s => s.Category);

            base.OnModelCreating(builder);
        }

        public virtual DbSet<Consumption> Consumptions { get; set;  }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Budget> Budgets { get; set; }
        public virtual DbSet<BudgetItem> BudgetItems { get; set; }
    }
}
