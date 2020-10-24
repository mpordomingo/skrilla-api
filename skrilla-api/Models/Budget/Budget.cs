using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NodaTime;

namespace skrilla_api.Models.Budget
{
    [Serializable]
    public class Budget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BudgetId { get; set; }

        [Required]
        [Column("start_date", TypeName = "date")]
        public LocalDate StartDate { get; set; }

        [Required]
        [Column("end_date", TypeName = "date")]
        public LocalDate EndDate { get; set; }

        [Required]
        [JsonIgnore]
        public string PersonId { get; set; }

        [Required]
        public double Amount { get; set; }

        public List<BudgetItem> BudgetItems { get; set; }


        public Budget(LocalDate startDate, LocalDate endDate, double amount, string personId)
        {
            StartDate = startDate;
            EndDate = endDate;
            PersonId = personId;
            Amount = amount;
            BudgetItems = new List<BudgetItem>();

        }
    }
}
