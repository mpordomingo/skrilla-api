using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using skrilla_api.Configuration;
using skrilla_api.Models;

namespace skrilla_api.Controllers
{ 

    [ApiController]
    [Route("/consumptions")]
    public class ConsumptionsController : ControllerBase
    {

        private readonly MysqlContext context;

        private readonly ILogger<ConsumptionsController> _logger;

        public ConsumptionsController(ILogger<ConsumptionsController> logger, MysqlContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [HttpGet]
        public List<Consumption>  Get()
        {

            return context.Consumptions.ToList();

        }
    }
}
