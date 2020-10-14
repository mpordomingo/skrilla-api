using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using skrilla_api.Configuration;
using skrilla_api.Models;
using skrilla_api.Services;

namespace skrilla_api.Controllers
{

    [ApiController]
    [Route("/categories")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> _logger;

        private readonly ICategoriesService categoriesService;


        public CategoriesController(ILogger<CategoriesController> logger, ICategoriesService categoriesService)
        {
            _logger = logger;
            this.categoriesService = categoriesService;
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

            List<Category> categories = categoriesService.GetCategories();
            return categories;
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

            try
            {
                Category category = categoriesService.CreateCategory(request);

                if (category == null)
                {
                    return StatusCode(500);
                }

                return CreatedAtAction(nameof(Get), null, category);
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