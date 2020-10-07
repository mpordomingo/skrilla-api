using System;
using System.Collections.Generic;
using skrilla_api.Models;

namespace skrilla_api.Services
{
    public interface IConsumptionService
    {

        public bool ModifyConsumption(ConsumptionRequest consumptionRequest, int id);
        public bool DeleteConsumption(int id);
        public Consumption CreateConsumption(ConsumptionRequest request);
        public List<Consumption> GetConsumptions(string category);

    }
}
