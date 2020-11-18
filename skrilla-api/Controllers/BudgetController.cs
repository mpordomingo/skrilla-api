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

        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<List<Budget>> GetBudgetList()
        {
            string loggedUser = User.FindFirstValue("userId");
            if (loggedUser == null)
            {
                return Unauthorized();
            }

            List<Budget> budgets = budgetService.GetBudgetList();
            return budgets;

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Budget> GetCurrentBudget()
        {
            string loggedUser = User.FindFirstValue("userId");
            if (loggedUser == null)
            {
                return Unauthorized();
            }

            Budget budget = budgetService.GetCurrentBudget();
            return budget;

        }

        [HttpGet("{budgetId:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Budget> GetBudgetById(int budgetId)
        {
            string loggedUser = User.FindFirstValue("userId");
            if (loggedUser == null)
            {
                return Unauthorized();
            }

            try
            {
                Budget budget = budgetService.GetBudgetById(budgetId);
                return budget;

            }
            catch (SkrillaApiException e)
            {
                return NotFound(e.Message);
            }
            
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


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<BudgetItem> ModifyCategoryBudget(BudgetItemRequest request)
        {
            string loggedUser = User.FindFirstValue("userId");

            if (loggedUser == null)
            {
                return Unauthorized();
            }

            /*ValidationResult result = validator.Validate(request);

            if (!result.IsValid)
            {
                return BadRequest(new ValidationSummary(result));
            }*/
            BudgetItem item;

            try
            {
                item = budgetService.ModifyCategoryBudget(request);
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

            return Ok(item);
        }

    }
}
