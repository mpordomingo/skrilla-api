using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    [Route("/categories")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryValidation validator;

        private readonly ILogger<CategoriesController> _logger;

        private readonly ICategoriesService categoriesService;

        private readonly IConsumptionService consumptionService;

        public CategoriesController(ILogger<CategoriesController> logger, ICategoriesService categoriesService,
            IConsumptionService consumptionService)
        {
            _logger = logger;
            validator = new CategoryValidation();
            this.categoriesService = categoriesService;
            this.consumptionService = consumptionService;
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

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteCategory(int id)
        {
            string loggedUser = User.FindFirstValue("userId");

            if (loggedUser == null)
            {
                return Unauthorized();
            }

            try
            {
                string categoryName = categoriesService.GetCategoryName(id);

                if (categoryName != null)
                {
                    consumptionService.ChangeCategoryIdToCategoryDefaultIdOfConsumptionsWithSameGroupIds(categoryName);
                }

                categoriesService.DeleteCategory(id);
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
        public ActionResult<Consumption> ModifyCategory(int id, CategoryRequest request)
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

            Category updatedCategory = null;

            try
            {
                updatedCategory = categoriesService
                    .ModifyCategory(request, id);
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

            return Ok(updatedCategory);
        }
    }
}