using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public BudgetSummary GetBudgetSummary()
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            Budget budget = dbContext.Budgets
                .Where(b => b.PersonId.Equals(loggedUser))
                .Include(b => b.BudgetItems)
                .AsEnumerable()
                .OrderByDescending(b => b.EndDate.Year)
                .ThenByDescending(b => b.EndDate.Month)
                .ThenByDescending(b => b.EndDate.Day)
                .FirstOrDefault();


            var consumptionsSet = dbContext.Consumptions
                .Where(c => c.PersonId.Equals(loggedUser) &&
                        budget.StartDate.CompareTo(c.Date) < 0 &&
                        budget.EndDate.CompareTo(c.Date) > 0)
                .Include(c => c.Category);


            List<BudgetItem> budgetItems = budget.BudgetItems.ToList();
            List<Category> categories = dbContext.Categories
                .Where(c => c.PersonId.Equals(loggedUser))
                .ToList();

            List<BudgetCategorySummaryItem> summaryItems = new List<BudgetCategorySummaryItem>();

            categories.ForEach(category =>
            {
                BudgetCategorySummaryItem item = consumptionsSet
                .Where(c => c.Category.Equals(category))
                .AsEnumerable()
                .GroupBy(c => c.Category)
                .Select(g =>
                {
                    var budgetItem = budgetItems.Where(b => b.Category.Equals(g.Key)).FirstOrDefault();
                    double budgetAmount = (budgetItem == null) ? -1 : budgetItem.BudgetedAmount;
                    return new BudgetCategorySummaryItem(g.Key, budgetAmount, g.Sum(c => c.Amount));
                }).FirstOrDefault();

                if (item != null)
                {
                    summaryItems.Add(item);
                }
                else
                {
                    summaryItems.Add(new BudgetCategorySummaryItem(category, -1, 0));
                }
            });

            var totalRes = consumptionsSet
                .GroupBy(c => c.PersonId)
                .Select(g =>  new { total = g.Sum(c => c.Amount) })
                .FirstOrDefault();

            double totalSpent = (totalRes == null) ? 0 : totalRes.total;

            totalSpent = Math.Round(totalSpent, 2);
            summaryItems = summaryItems.OrderByDescending(i => i.TotalSpent).ToList();

            BudgetSummary summary = new BudgetSummary(budget.Amount, (double)totalSpent, summaryItems);

            return summary;
        }

        public BudgetItem ModifyCategoryBudget(BudgetItemRequest request)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            Budget budget = dbContext.Budgets
                .Where(b => b.PersonId.Equals(loggedUser))
                .Include(b => b.BudgetItems)
                .ThenInclude(i => i.Category)
                .AsEnumerable()
                .OrderByDescending(b => b.EndDate.Year)
                .ThenByDescending(b => b.EndDate.Month)
                .ThenByDescending(b => b.EndDate.Day)
                .FirstOrDefault();

            if (budget == null)
            {
                throw new SkrillaApiException("not_found", "El usuario no tiene un presupuesto aun.");
            }

            Category category = dbContext.Categories
                .Where(c => c.PersonId.Equals(loggedUser) && c.CategoryId.Equals(request.category))
                .FirstOrDefault();

            if (category == null)
            {
                throw new SkrillaApiException("not_found", "La categoria indicada no fue encontrada. ");
            }

            BudgetItem item = budget.BudgetItems
                .Where(i => category.Equals(i.Category))
                .FirstOrDefault();

            if (item != null)
            {
                item.BudgetedAmount = request.amount;
                dbContext.SaveChanges();
                return item;
            }

            item = new BudgetItem(budget, category, request.amount);
            dbContext.Add(item);
            dbContext.SaveChanges();
            return item;
        }
    }
}
