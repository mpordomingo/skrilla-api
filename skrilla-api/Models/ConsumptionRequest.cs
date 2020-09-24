using System;
namespace skrilla_api.Models
{
    public class ConsumptionRequest
    {
        public string Title { get; set; }
        public double Amount { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }

        public ConsumptionRequest() { }
        
    }
}
