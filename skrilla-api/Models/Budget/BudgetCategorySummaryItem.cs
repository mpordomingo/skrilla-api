using System;
namespace skrilla_api.Models.Budget
{
    public class BudgetCategorySummaryItem
    {
        public Category Category { get; set; }
        public double BudgetedAmount { get; set; }
        public double TotalSpent { get; set; }

        public BudgetCategorySummaryItem(Category category, double budget, double totalSpent)
        {
            Category = category;
            BudgetedAmount = budget;
            TotalSpent = totalSpent;
        }
    }
}
