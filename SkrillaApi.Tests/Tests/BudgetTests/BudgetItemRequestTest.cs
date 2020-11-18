using System;
using skrilla_api.Models.Budget;
using Xunit;

namespace SkrillaApi.Tests.Tests.BudgetTests
{
    public class BudgetItemRequestTest
    {
        public BudgetItemRequestTest()
        {
        }

        [Fact]
        public void BudgetItemRequestIsCreatedSuccessfully()
        {
            BudgetItemRequest item = new BudgetItemRequest
            {
                budgetId = 123,
                category = 1,
                amount = 123.5
            };
            Assert.Equal(123, item.budgetId);
            Assert.Equal(1, item.category);
            Assert.Equal(123.5, item.amount);
        }
    }
}
