using System;
using System.Collections.Generic;

namespace skrilla_api.Models.Budget
{
    public class BudgetSummary
    {
        public double Amount { get; set; }
        public double TotalSpent { get; set; }
        public List<BudgetCategorySummaryItem> CategoryItems { get; set; }

        public BudgetSummary(double amount, double totalSpent, List<BudgetCategorySummaryItem> items)
        {
            Amount = amount;
            TotalSpent = totalSpent;
            CategoryItems = items;
        }
    }

}
