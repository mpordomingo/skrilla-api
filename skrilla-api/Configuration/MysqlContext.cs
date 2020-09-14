using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using skrilla_api.Models;
 
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
            base.OnModelCreating(builder);
        }

        public virtual DbSet<Consumption> Consumptions { get; set;  }
    }
}
