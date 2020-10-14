﻿using System;
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
    public class ConsumptionService : IConsumptionService
    {
        private readonly SkrillaDbContext dbContext;

        private readonly ILogger<ConsumptionService> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConsumptionService(ILogger<ConsumptionService> logger,
            SkrillaDbContext context, IHttpContextAccessor httpContextAccesor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccesor;
            dbContext = context;
        }

        public Consumption ModifyConsumption(ConsumptionRequest consumptionRequest, int id)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            Consumption consumption = dbContext
                   .Consumptions
                   .Where(s => s.Id == id && s.PersonId.Equals(loggedUser))
                   .Include(c => c.Category).FirstOrDefault();

            if (consumption == null)
            {
                _logger.LogDebug("Consumption not found: " + id + " for user: " + loggedUser);
                throw new SkrillaApiException("not_found", "Consumption not found");
            }

            UpdateValues(consumption, consumptionRequest);
            dbContext.SaveChanges();
            return consumption;
        }

        public bool DeleteConsumption(int id)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            Consumption consumption = dbContext
                    .Consumptions
                    .Where(s => s.Id == id && s.PersonId.Equals(loggedUser))
                    .Include(c => c.Category).FirstOrDefault();

            if (consumption == null)
            {
                _logger.LogDebug("Consumption not found: " + id + " for user: " + loggedUser);
                throw new SkrillaApiException("not_found","Consumption not found");
            }

            dbContext.Remove<Consumption>(consumption);
            dbContext.SaveChanges();

            return true;
        }

        public Consumption CreateConsumption(ConsumptionRequest request)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            Category category = GetOrCreateCategory(request.Category);

            Consumption consumption = new Consumption(request.Title,
                request.Amount,
                category,
                loggedUser,
                LocalDate.FromDateTime(request.Date));

            dbContext.Add(consumption);
            dbContext.SaveChanges();
            return consumption;
        }

        public List<Consumption> GetConsumptions(string category)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            if (category != null)
            {
                List<Consumption> result = new List<Consumption>();
                result = dbContext
                    .Consumptions
                    .Where(s => s.Category.Name.Contains(category) && s.PersonId.Equals(loggedUser))
                    .Include(c => c.Category).ToList();

                return result;
            }
            else
            {
                return dbContext.Consumptions
                    .Where(s => s.PersonId.Equals(loggedUser))
                    .Include(c => c.Category)
                    .ToList();
            }
        }

        public Consumption GetConsumption(int id)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            Consumption result = dbContext
                   .Consumptions
                   .Where(s => s.Id == id && s.PersonId.Equals(loggedUser))
                   .Include(c => c.Category).FirstOrDefault();

            return result;
            
        }

        private void UpdateValues(Consumption consumption, ConsumptionRequest request)
        {
            consumption.Title = request.Title;
            consumption.Amount = request.Amount;
            consumption.Category = GetOrCreateCategory(request.Category);
            consumption.Date =  LocalDate.FromDateTime(request.Date);

        }

        private Category GetOrCreateCategory(string category)
        {
            string loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

            List<Category> categories = dbContext
                .Categories
                .Where(s => s.Name == category)
                .ToList<Category>();

            Category aCategory;
            if (categories.Count == 0)
            {

                aCategory = new Category(
                    category,
                    true,
                    loggedUser);

                dbContext.Add(aCategory);
                dbContext.SaveChanges();
            }
            else
            {
                aCategory = categories.First<Category>();
            }


            return aCategory;
        }
    }
}