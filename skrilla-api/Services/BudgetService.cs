using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NodaTime;
using skrilla_api.Configuration;
using skrilla_api.Models;
using skrilla_api.Models.Budget;

namespace skrilla_api.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly SkrillaDbContext dbContext;

        private readonly ILogger<BudgetService> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public BudgetService(ILogger<BudgetService> logger,
            SkrillaDbContext context, IHttpContextAccessor httpContextAccesor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccesor;
            dbContext = context;
        }

        public Budget CreateBudget(BudgetRequest request)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            Budget budget = new Budget(
                LocalDate.FromDateTime((DateTime)request.StartDate),
                LocalDate.FromDateTime((DateTime)request.EndDate),
                request.Amount,
                loggedUser);
            dbContext.Add(budget);

            List<int> categoryIds = request.BudgetItems.Select(i => i.category).ToList();

            List<Category> categories = dbContext.Categories
                .Where(c => categoryIds.Contains(c.CategoryId))
                .ToList();

            if(categories == null || categoryIds.Count != categories.Count)
            {
                throw new SkrillaApiException("not_found",
                    "One or more categories were not found.");
            }


            request.BudgetItems.ForEach(i =>
            {
                BudgetItem item = new BudgetItem(budget,
                    categories.Find(c => c.CategoryId == i.category),
                    i.amount);
                dbContext.Add(item);
            });
            
            dbContext.SaveChanges();

            return budget;
        }

        public Budget GetBudget()
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            Budget budget = dbContext.Budgets
                .Where(b => b.PersonId.Equals(loggedUser))
                .AsEnumerable()
                .OrderByDescending(b => b.EndDate.Year)
                .ThenByDescending(b => b.EndDate.Month)
                .ThenByDescending(b => b.EndDate.Day)
                .FirstOrDefault();

            return budget;

        }
    }
}
