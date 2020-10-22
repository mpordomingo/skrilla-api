using System;
using System.Linq;

using Microsoft.Extensions.Logging;
using skrilla_api.Configuration;
using skrilla_api.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using NodaTime;
using Microsoft.EntityFrameworkCore;

namespace skrilla_api.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly SkrillaDbContext dbContext;

        private readonly ILogger<CategoriesService> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoriesService(ILogger<CategoriesService> logger,
            SkrillaDbContext context, IHttpContextAccessor httpContextAccesor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccesor;
            dbContext = context;
        }

        public Category CreateCategory(CategoryRequest request)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            List<Category> categories = dbContext
                .Categories
                .Where(s => s.Name == request.Name)
                .ToList<Category>();

            Category aCategory;
            if (categories.Count == 0)
            {

                aCategory = new Category(
                    request.Name,
                    true,
                    loggedUser,
                    request.Icon);

                dbContext.Add(aCategory);
                dbContext.SaveChanges();
            }
            else
            {
                aCategory = categories.First<Category>();
            }


            return aCategory;
        }

        public bool DeleteCategory(int id)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            Category category = dbContext
                    .Categories
                    .Where(s => s.CategoryId == id && s.PersonId.Equals(loggedUser))
                    .FirstOrDefault();

            if (category == null)
            {
                _logger.LogDebug("Category not found: " + id + " for user: " + loggedUser);
                throw new SkrillaApiException("not_found", "Category not found");
            }

            dbContext.Remove<Category>(category);
            dbContext.SaveChanges();

            return true;
        }

        public List<Category> GetCategories()
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            
            return dbContext.Categories
                   .Where(s => s.PersonId.Equals(loggedUser))
                   .ToList();
        }

        public string GetCategoryName(int id)
        {
            return GetCategory(id).Name;
        }

        public Category ModifyCategory(CategoryRequest categoryRequest, int CategoryId)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            Category category = dbContext
                   .Categories
                   .Where(s => s.CategoryId == CategoryId && s.PersonId.Equals(loggedUser))
                   .FirstOrDefault();

            if (category == null)
            {
                _logger.LogDebug("Category not found: " + CategoryId + " for user: " + loggedUser);
                throw new SkrillaApiException("not_found", "Category not found");
            }

            UpdateValues(category, categoryRequest);
            dbContext.SaveChanges();
            return category;
        }

        private void UpdateValues(Category category, CategoryRequest request)
        {
            category.Name = request.Name;
        }

        private Category GetCategory(int id)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            return dbContext
                   .Categories
                   .Where(s => s.CategoryId == id && s.PersonId.Equals(loggedUser))
                   .FirstOrDefault();
        }
    }
}
