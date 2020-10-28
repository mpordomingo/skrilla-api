using System;
namespace skrilla_api.Models.Budget
{
    public class BudgetSummary
    {
        public double Amount { get; set; }
        public double TotalSpent { get; set; }

        public BudgetSummary(double amount, double totalSpent)
        {
            Amount = amount;
            TotalSpent = totalSpent;
        }
    }
}
