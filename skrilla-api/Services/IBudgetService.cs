using System;
using skrilla_api.Models.Budget;

namespace skrilla_api.Services
{
    public interface IBudgetService
    {
        public Budget CreateBudget(BudgetRequest request);
        public Budget GetBudget();
    }
}
