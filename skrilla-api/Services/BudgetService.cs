using System;
using System.Collections;
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

            LocalDate newStartDate = LocalDate.FromDateTime((DateTime)request.StartDate);
            LocalDate newEndDate = LocalDate.FromDateTime((DateTime)request.EndDate);


            List<Budget> foundBudgets = dbContext.Budgets.Where(b =>
                    (b.StartDate.CompareTo(newStartDate) < 0 && b.EndDate.CompareTo(newStartDate) > 0)||
                    (b.StartDate.CompareTo(newEndDate) < 0 && b.EndDate.CompareTo(newStartDate) > 0))
                .ToList();

            if(foundBudgets.Count > 0){
                   throw new SkrillaApiException("conflict","There is a budget for that time period already");
             }


            Budget budget = new Budget(
                LocalDate.FromDateTime((DateTime)request.StartDate),
                LocalDate.FromDateTime((DateTime)request.EndDate),
                request.Amount,
                loggedUser);
            dbContext.Add(budget);

            List<int> categoryIds = request.BudgetItems.Select(i => i.category).ToList();

            List<Category> categories = dbContext.Categories
                .Where(s => s.PersonId.Equals(loggedUser))
                .ToList();

            if(categories == null)
            {
                throw new SkrillaApiException("not_found",
                    "One or more categories were not found.");
            }

            categories.ForEach(category =>
            {
                BudgetItem item = new BudgetItem(budget,
                    category,
                    0);
                dbContext.Add(item);
            });

            request.BudgetItems.ForEach(i =>
            {
                BudgetItem item = budget.BudgetItems.Where(bi => i.category.Equals(bi.Category.CategoryId)).FirstOrDefault();
                if(item != null)
                {
                    item.BudgetedAmount = i.amount;
                }

            });
            
            dbContext.SaveChanges();

            return budget;
        }

        public Budget GetCurrentBudget()
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

        public BudgetSummary GetBudgetSummaryById(int budgetId)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            Budget budget = dbContext.Budgets
                .Where(b => b.PersonId.Equals(loggedUser) && b.BudgetId.Equals(budgetId))
                .Include(b => b.BudgetItems)
                .FirstOrDefault();

            if (budget == null)
            {
                throw new SkrillaApiException("not_found", "Budget not found.");
            }


            List<Consumption> consumptionsSet = dbContext.Consumptions
                .Where(c => c.PersonId.Equals(loggedUser) &&
                        budget.StartDate.CompareTo(c.Date) <= 0 &&
                        budget.EndDate.CompareTo(c.Date) >= 0).
                        Include(c => c.Category).ToList();


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
                    var budgetItem = budgetItems.Where(b => b.Category.Equals(category)).FirstOrDefault();
                    double budgetAmount = -1;
                    if (budgetItem != null && budgetItem.BudgetedAmount != 0) {
                        budgetAmount = budgetItem.BudgetedAmount;
                     }

                    summaryItems.Add(new BudgetCategorySummaryItem(category, budgetAmount, 0));
                }
            });

            var totalRes = consumptionsSet
                .GroupBy(c => c.PersonId)
                .Select(g => new { total = g.Sum(c => c.Amount) })
                .FirstOrDefault();

            double totalSpent = (totalRes == null) ? 0 : totalRes.total;

            totalSpent = Math.Round(totalSpent, 2);
            summaryItems = summaryItems.OrderByDescending(i => i.TotalSpent).ToList();

            BudgetSummary summary = new BudgetSummary(budget.Amount, (double)totalSpent, summaryItems, budget.BudgetId);

            return summary;

            
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

            if (budget == null)
                return null;

            List<Consumption> consumptionsSet = dbContext.Consumptions
                .Where(c => c.PersonId.Equals(loggedUser) &&
                        budget.StartDate.CompareTo(c.Date) <= 0 &&
                        budget.EndDate.CompareTo(c.Date) >= 0).
                        Include(c => c.Category).ToList();


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

            BudgetSummary summary = new BudgetSummary(budget.Amount, (double)totalSpent, summaryItems, budget.BudgetId);

            return summary;
        }

        public List<Budget> GetBudgetList()
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            List<Budget> budgets = dbContext.Budgets
                .Where(b => b.PersonId.Equals(loggedUser))
                .Select(n => new Budget(n.StartDate, n.EndDate, n.Amount, n.PersonId)
                {
                    BudgetId = n.BudgetId
                })
                .ToList();

            return budgets;
        }

        public BudgetItem ModifyCategoryBudget(BudgetItemRequest request)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            Budget budget = dbContext.Budgets
                .Where(b => b.PersonId.Equals(loggedUser) && b.BudgetId.Equals(request.budgetId))
                .Include(b => b.BudgetItems)
                .ThenInclude(i => i.Category)
                .AsEnumerable()
                .FirstOrDefault();

            if (budget == null)
            {
                throw new SkrillaApiException("not_found", "El presupuesto indicado no fue encontrada.");
            }
            if (budget.EndDate.CompareTo( LocalDate.FromDateTime(DateTime.Today)) < 0)
            {
                throw new SkrillaApiException("invalid_request", "No se puede alterar presupuestos pasados.");
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
