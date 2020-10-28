using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using skrilla_api.Models.Budget;
using skrilla_api.Services;
using skrilla_api.Validation;

namespace skrilla_api.Controllers
{

    [ApiController]
    [Route("/budget")]
    [Authorize]
    public class BudgetController : ControllerBase
    {

        private readonly BudgetValidation validator;

        private readonly ILogger<BudgetController> _logger;

        private readonly IBudgetService budgetService;


        public BudgetController(ILogger<BudgetController> logger,
            IBudgetService budgetService)
        {
            _logger = logger;
            validator = new BudgetValidation();
            this.budgetService = budgetService;
        }

        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<BudgetSummary> GetBudgetSummary()
        {
            string loggedUser = User.FindFirstValue("userId");
            if (loggedUser == null)
            {
                return Unauthorized();
            }

            BudgetSummary budget = budgetService.GetBudgetSummary();
            return budget;

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Budget> GetBudget()
        {
            string loggedUser = User.FindFirstValue("userId");
            if (loggedUser == null)
            {
                return Unauthorized();
            }

            Budget budget = budgetService.GetBudget();
            return budget;

        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Budget> Post([FromBody] BudgetRequest request)
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
                Budget budget = budgetService.CreateBudget(request);

                if (budget == null)
                {
                    return StatusCode(500);
                }

                return CreatedAtAction(nameof(Post), null, budget);
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
    }
}
