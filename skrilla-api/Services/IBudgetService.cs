using System;
using System.Collections.Generic;
using skrilla_api.Models.Budget;

namespace skrilla_api.Services
{
    public interface IBudgetService
    {
        public Budget CreateBudget(BudgetRequest request);
        public Budget GetCurrentBudget();
        public BudgetSummary GetBudgetSummaryById(int budgetId);
        public BudgetSummary GetBudgetSummary();
        public List<Budget> GetBudgetList();
        public BudgetItem ModifyCategoryBudget(BudgetItemRequest request);
    }
}
