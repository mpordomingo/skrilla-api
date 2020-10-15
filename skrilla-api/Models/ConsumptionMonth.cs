using System;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NodaTime;
namespace skrilla_api.Models
{
    public class ConsumptionMonth
    {
        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public double Amount { get; set; }

        public ConsumptionMonth(int month,int year,double amount)
        {
            this.Month = month;
            this.Year = year;
            this.Amount = amount;
        }
    }
}
