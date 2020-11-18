using System;
using System.Collections.Generic;
using skrilla_api.Models.Budget;

namespace skrilla_api.Services
{
    public interface IBudgetService
    {
        public Budget CreateBudget(BudgetRequest request);
        public Budget GetCurrentBudget();
        public Budget GetBudgetById(int budgetId);
        public BudgetSummary GetBudgetSummary();
        public List<Budget> GetBudgetList();
        public BudgetItem ModifyCategoryBudget(BudgetItemRequest request);
    }
}
