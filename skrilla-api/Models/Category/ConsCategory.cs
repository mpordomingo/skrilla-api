using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NodaTime;
namespace skrilla_api.Models
{
    public class ConsCategory
    {
        [Required]
        public string Category { get; set; }

        [Required]
        public double Amount { get; set; }

        public ConsCategory(string category, double amount)
        {
            this.Category = category;
            this.Amount = amount;
        }
    }
}
