using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using skrilla_api.Configuration;
using skrilla_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using skrilla_api.Services;
using skrilla_api.Middlewares;

namespace skrilla_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<SkrillaDbContext>(options => options
                .UseMySQL(Configuration.GetConnectionString("mysqlConnection")));

            services.AddCors();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<SkrillaDbContext>()
                .AddDefaultTokenProviders();
            services.AddControllers();

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
                options.DefaultForbidScheme = "Identity.Application";
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://skrilla-auth-server.herokuapp.com";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IConsumptionService, ConsumptionService>();
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<IBudgetService, BudgetService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOptions();

            app.UseCors(builder => builder
                .WithOrigins("https://skrilla-ui.herokuapp.com")
                .WithMethods("GET", "OPTIONS", "POST", "DELETE", "PUT")
                .WithHeaders("Origin", "Authorization")
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
