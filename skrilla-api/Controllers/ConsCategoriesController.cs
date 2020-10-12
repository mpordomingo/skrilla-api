using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using skrilla_api.Configuration;
using skrilla_api.Models;

namespace skrilla_api.Controllers
{

    [ApiController]
    [Route("/conspercat")]
    [Authorize]
    public class ConsCategories : ControllerBase
    {

        private readonly SkrillaDbContext context;


        private readonly ILogger<ConsCategories> _logger;


        public ConsCategories(ILogger<ConsCategories> logger, SkrillaDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<List<ConsCategory>> Get()
        {
            string loggedUser = User.FindFirstValue("userId");
            if (loggedUser == null)
            {
                return Unauthorized();
            }
            return context.Consumptions
                   .Where(s => s.PersonId.Equals(loggedUser))
                   .GroupBy(c => c.Category.CategoryId)
                   .Select(c => new ConsCategory
                   (
                        c.Key.ToString(),
                        c.Sum(x => x.Amount)
                   ))
                   .ToList();
        }
    }
}