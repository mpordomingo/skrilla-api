using System;
using System.ComponentModel.DataAnnotations;

namespace skrilla_api.Models
{
    public class Consumption
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }
        [Required]
        public double Amount { get; set; }
        

        public Consumption(string title, double amount)
        {
            this.Title = title;
            this.Amount = amount;
        }
    }
}
