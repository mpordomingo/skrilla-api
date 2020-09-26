using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;
using skrilla_api.Configuration;
using skrilla_api.Models;
using skrilla_api.Validation;

namespace skrilla_api.Controllers
{

    [ApiController]
    [Route("/consumptions")]
    [Authorize]
    public class ConsumptionsController : ControllerBase
    {

        private readonly MysqlContext context;

        private readonly ConsumptionValidation validator;

        private readonly ILogger<ConsumptionsController> _logger;

        public ConsumptionsController(ILogger<ConsumptionsController> logger, MysqlContext context)
        {
            _logger = logger;
            this.context = context;
            validator = new ConsumptionValidation();
        }

        [HttpGet]
        public List<Consumption>  Get()
        {

            return context.Consumptions.ToList();

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Consumption> Post(ConsumptionRequest request)
        {
            ValidationResult result = validator.Validate(request);

            if (result.IsValid)
            {
                Consumption consumption = new Consumption(request.Title,
                    request.Amount,
                    request.Category,
                    1,
                    LocalDate.FromDateTime(request.Date));

                context.Add(consumption);
                context.SaveChanges();

                return CreatedAtAction(nameof(Get), null, consumption);
            }
            else
            {
                return BadRequest(new ValidationSummary(result));
            }
            
        }
    }
}
