using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace skrilla_api.Models.Budget
{
    [Serializable]
    public class BudgetRequest
    {

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double Amount { get; set; }
        public List<BudgetItemRequest> BudgetItems { get; set; }

        public BudgetRequest()
        {
        }

    }       
}