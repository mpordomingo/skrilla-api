using System;
namespace skrilla_api.Models.Budget
{
    [Serializable]
    public class BudgetItemRequest
    {
        public int budgetId { get; set; }
        public int category { get; set; }
        public double amount { get; set; }

        public BudgetItemRequest()
        {
        }
    }
}
