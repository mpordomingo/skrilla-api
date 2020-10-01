using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        private readonly UserManager<ApplicationUser> _userManager;


        public ConsumptionsController(ILogger<ConsumptionsController> logger, MysqlContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this.context = context;
            validator = new ConsumptionValidation();
            _userManager = userManager;
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


            if (category != null) {
                List<Consumption> result = new List<Consumption>();
                result.Add(context
                    .Consumptions
                    .Where(s => s.Category.Name == category && s.PersonId.Equals(loggedUser))
                    .Include(c => c.Category)
                    .FirstOrDefault<Consumption>());

                return result;
            }
            else {
                return context.Consumptions
                    .Where(s => s.PersonId.Equals(loggedUser))
                    .Include(c => c.Category)
                    .ToList();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Consumption> Post(ConsumptionRequest request)
        {
            string loggedUser = User.FindFirstValue("userId");

            if(loggedUser == null)
            {
                return Unauthorized();
            }

            ValidationResult result = validator.Validate(request);

            if (result.IsValid)
            {
                Category category = GetOrCreateCategory(request.Category);

                Consumption consumption = new Consumption(request.Title,
                    request.Amount,
                    category,
                    loggedUser,
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


        private Category GetOrCreateCategory(string category)
        {
            List<Category> categories = context
                .Categories
                .Where(s => s.Name == category)
                .ToList<Category>();

            Category aCategory;
            if (categories.Count == 0){
                aCategory = new Category(category, true);
                context.Add(aCategory);
                context.SaveChanges();
            }
            else {
                aCategory = categories.First<Category>();
             }

            
            return aCategory;
        }

    }
}
