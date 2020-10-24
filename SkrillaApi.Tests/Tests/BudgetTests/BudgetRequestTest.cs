using System;
using System.Collections.Generic;
using skrilla_api.Models.Budget;
using Xunit;

namespace SkrillaApi.Tests.Tests.BudgetTests
{
    public class BudgetRequestTest
    {
        private readonly List<BudgetItemRequest> items;

        public BudgetRequestTest()
        {
            items = new List<BudgetItemRequest>
            {
                new BudgetItemRequest
                {
                    category = 123,
                    amount = 563.2
                }
            };
        }

        [Fact]
        public void BudgetRequestIsCreatedSuccessfully()
        {
            DateTime start = new DateTime(2019, 05, 10);
            DateTime end = new DateTime(2020, 04, 15);

            BudgetRequest request = new BudgetRequest
            {
                StartDate = start,
                EndDate = end,
                Amount = 123.5,
                BudgetItems = items
            };

            Assert.Equal(start, request.StartDate);
            Assert.Equal(end, request.EndDate);
            Assert.Equal(123.5, request.Amount);
            Assert.Single(request.BudgetItems);
        }
    }
}
