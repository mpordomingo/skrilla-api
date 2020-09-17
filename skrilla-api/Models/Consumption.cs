using System;
using System.ComponentModel.DataAnnotations;

namespace skrilla_api.Models
{
    public class Consumption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public int PersonId { get; set; }

        public Consumption(string title, double amount)
        {
            this.Title = title;
            this.Amount = amount;
        }
    }
}
