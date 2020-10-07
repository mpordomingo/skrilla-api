﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using skrilla_api.Configuration;
using skrilla_api.Models;
using skrilla_api.Services;
using skrilla_api.Validation;

namespace skrilla_api.Controllers
{

    [ApiController]
    [Route("/consumptions")]
    [Authorize]
    public class ConsumptionsController : ControllerBase
    {

        private readonly ConsumptionValidation validator;

        private readonly ILogger<ConsumptionsController> _logger;


        private readonly IConsumptionService consumptionService;


        public ConsumptionsController(ILogger<ConsumptionsController> logger,
            IConsumptionService consumptionService)
        {
            _logger = logger;
            validator = new ConsumptionValidation();
            this.consumptionService = consumptionService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<List<Consumption>> Get(string category)
        {
            string loggedUser = User.FindFirstValue("userId");
            if(loggedUser == null)
            {
                return Unauthorized();
            }

            List<Consumption> consumptions = consumptionService.GetConsumptions(category);
            return consumptions;
            
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Consumption> Post(ConsumptionRequest request)
        {
            string loggedUser = User.FindFirstValue("userId");

            if (loggedUser == null)
            {
                return Unauthorized();
            }

            ValidationResult result = validator.Validate(request);

            if (!result.IsValid)
            {
                return BadRequest(new ValidationSummary(result));
            }

            try
            {
                Consumption consumption = consumptionService.CreateConsumption(request);

                if (consumption == null)
                {
                    return StatusCode(500);
                }

                return CreatedAtAction(nameof(Get), null, consumption);
            }
            catch (SkrillaApiException e)
            {
                if ("not_found".Equals(e.Code))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteConsumption(int id)
        {
            string loggedUser = User.FindFirstValue("userId");

            if (loggedUser == null)
            {
                return Unauthorized();
            }

            try
            {
                consumptionService.DeleteConsumption(id);
            }
            catch (SkrillaApiException e)
            {
                if ("not_found".Equals(e.Code))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }
            
            return Accepted();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult ModifyConsumption(int id, ConsumptionRequest request)
        {
            string loggedUser = User.FindFirstValue("userId");

            if (loggedUser == null)
            {
                return Unauthorized();
            }

            ValidationResult result = validator.Validate(request);

            if (!result.IsValid)
            {
                return BadRequest(new ValidationSummary(result));
            }

            try
            {
                consumptionService.ModifyConsumption(request, id);
            }
            catch (SkrillaApiException e)
            {
                if (e.Code == "404")
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(e.Message);
                }
            }

            return Ok();
        }

    }
}
