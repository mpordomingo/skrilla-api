using System;
namespace skrilla_api.Models.Budget
{
    public class BudgetCategorySummaryItem
    {
        public string Category { get; set; }
        public double BudgetedAmount { get; set; }
        public double TotalSpent { get; set; }

        public BudgetCategorySummaryItem(string category, double budget, double totalSpent)
        {
            Category = category;
            BudgetedAmount = budget;
            TotalSpent = totalSpent;
        }
    }
}
