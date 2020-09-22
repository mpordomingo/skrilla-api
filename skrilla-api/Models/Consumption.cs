using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

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

        [Required]
        public string Category { get; set; }

        [Required]
        [Column("cdate", TypeName = "date")]
        public LocalDate Date { get; set; }

        public Consumption(string title, double amount, string category, int personId, LocalDate date)
        {
            this.Title = title;
            this.Amount = amount;
            this.Category = category;
            this.PersonId = personId;
            this.Date = date;

        }
    }
}
