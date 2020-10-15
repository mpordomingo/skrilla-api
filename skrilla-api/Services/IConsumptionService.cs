using System;
using System.Collections.Generic;
using skrilla_api.Models;

namespace skrilla_api.Services
{
    public interface IConsumptionService
    {

        public Consumption ModifyConsumption(ConsumptionRequest consumptionRequest, int id);
        public bool DeleteConsumption(int id);
        public Consumption CreateConsumption(ConsumptionRequest request);
        public Consumption GetConsumption(int id);
        public List<Consumption> GetConsumptions(string category);
        public List<ConsumptionMonth> GetConsumptionsPerMonth();

    }
}
