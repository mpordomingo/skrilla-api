using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace skrilla_api.Models.Budget
{
    [Serializable]
    public class BudgetItem
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BudgetItemId { get; set; }

        [Required]
        [JsonIgnore]
        public Budget Budget { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        public double BudgetedAmount { get; set; }

        public BudgetItem(Budget budget, Category category, double budgetedAmount)
        {
            Budget = budget;
            Category = category;
            BudgetedAmount = budgetedAmount;
        }

        public BudgetItem()
        {

        }
    }
}
