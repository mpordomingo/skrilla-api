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
        public List<Category> Get()
        {

            return context.Categories.ToList();

        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Category> Post(CategoryRequest request)
        {
            Category cat = new Category(request.Name, request.Photoid, request.Nonedit);

            context.Add(cat);
            context.SaveChanges();

            return CreatedAtAction(nameof(Get), null, cat);
           
        }
    }
}