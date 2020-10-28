using System;
namespace skrilla_api.Models.Budget
{
    [Serializable]
    public class BudgetItemRequest
    {
        public int category;
        public double amount;

        public BudgetItemRequest()
        {
        }
    }
}
