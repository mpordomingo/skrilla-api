using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("/categories")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {

        private readonly MysqlContext context;

        private readonly ILogger<CategoriesController> _logger;

    
        public CategoriesController(ILogger<CategoriesController> logger, MysqlContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<List<Category>> Get()
        {
            string loggedUser = User.FindFirstValue("userId");
            if (loggedUser == null)
            {
                return Unauthorized();
            }

            return context.Categories
                   .Where(s => s.PersonId.Equals(loggedUser))
                   .ToList();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Category> Post(CategoryRequest request)
        {
            string loggedUser = User.FindFirstValue("userId");
            if (loggedUser == null)
            {
                return Unauthorized();
            }

            Category cat = new Category(request.Name, request.Nonedit, loggedUser);

            context.Add(cat);
            context.SaveChanges();

            return CreatedAtAction(nameof(Get), null, cat);
           
        }
    }
}