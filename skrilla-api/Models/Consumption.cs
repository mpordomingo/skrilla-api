using System;
namespace skrilla_api.Models
{
    public class Consumption
    {
        public string Title { get; set; }
        public double Amount { get; set; }
        public int Id { get; set; }


        public Consumption(string title, double amount)
        {
            this.Title = title;
            this.Amount = amount;
        }
    }
}
