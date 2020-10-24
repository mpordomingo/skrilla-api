using System;
using System.Collections.Generic;

namespace skrilla_api.Models.Budget
{
    public class BudgetRequest
    {
        public DateTime? StartDate;
        public DateTime? EndDate;
        public double Amount;
        public List<BudgetItemRequest> BudgetItems;

        public BudgetRequest()
        {
        }
    }
}
