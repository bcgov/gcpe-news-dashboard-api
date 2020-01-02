using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Gcpe.Hub.API.Helpers;
using Gcpe.Hub.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gcpe.Hub.API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ActivitiesController : BaseController
    {
        private readonly HubDbContext dbContext;
        private readonly IMapper mapper;
        private static DateTime? mostFutureForecastActivity = null;
        static DateTime? lastModified = null;
        static DateTime lastModifiedNextCheck = DateTime.Now;

        public ActivitiesController(HubDbContext dbContext,
            ILogger<ActivitiesController> logger,
            IMapper mapper,
            IHostEnvironment env) : base(logger)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            if (env?.IsProduction() == false && !mostFutureForecastActivity.HasValue)
            {
                mostFutureForecastActivity = Forecast(dbContext).Where(a => a.IsActive).OrderByDescending(a => a.StartDateTime).First().StartDateTime?.Date;
            }
        }

        internal static IQueryable<Activity> QueryAll(HubDbContext dbContext)
        {
            return dbContext.Activity.Include(a => a.ContactMinistry).Include(a => a.City)
                .Include(a => a.ActivityCategories).ThenInclude(ac => ac.Category)
                .Include(a => a.ActivitySharedWith).ThenInclude(sw => sw.Ministry);
        }

        private IQueryable<Activity> Forecast(HubDbContext dbContext)
        {
          return QueryAll(dbContext)
                .Where(a => a.IsConfirmed && !a.IsConfidential && a.ActivityKeywords.Any(ak => ak.Keyword.Name == "HQ-1P"|| ak.Keyword.Name == "DB")
                    || !a.IsConfirmed && !a.IsConfidential && a.StartDateTime.Value.Date == a.EndDateTime.Value.Date && !a.IsAllDay
                    && a.ActivityKeywords.Any(ak => ak.Keyword.Name == "HQ-1P" || ak.Keyword.Name == "DB")
                    || !a.IsConfirmed && !a.IsConfidential && a.StartDateTime.Value.Date == a.EndDateTime.Value.Date && a.IsAllDay
                    && a.ActivityKeywords.Any(ak => ak.Keyword.Name == "HQ-1P" || ak.Keyword.Name == "DB")
                    || a.IsConfirmed && a.IsConfidential && a.HqSection != 4 && a.ActivityKeywords.Any(ak => ak.Keyword.Name == "HQ-1P" || ak.Keyword.Name == "DB")
                    || !a.IsConfirmed && a.IsConfidential && a.HqSection != 4 && a.StartDateTime.Value.Date == a.EndDateTime.Value.Date && !a.IsAllDay
                    && a.ActivityKeywords.Any(ak => ak.Keyword.Name == "HQ-1P" || ak.Keyword.Name == "DB"));
        }

        [HttpGet("Forecast/{numDays}")]
        [Authorize(Policy = "ReadAccess")]
        [Produces(typeof(IEnumerable<Models.Activity>))]
        [ProducesResponseType(304)]
        [ProducesResponseType(400)]
        [ResponseCache(Duration = 5)]
        public IActionResult GetActivityForecast(int numDays)
        {
            try
            {
                IQueryable<Activity> forecast = Forecast(dbContext);
                var today = DateTime.Today;
                if (lastModifiedNextCheck.Date != today)
                {
                    Request.GetTypedHeaders().IfModifiedSince = null; // force refresh after midnight
                }
                forecast = forecast.Where(a => a.StartDateTime >= today && a.StartDateTime < today.AddDays(numDays));

                // remove the check for modification only for 7 days forecast
                // reason: hq amdin does not want to update the LastUpdateTimeStamp in corp calendar for activities with hq-1 tag
                return Ok(forecast.Where(a => a.IsActive).OrderBy(a => a.StartDateTime).Select(a => mapper.Map<Models.Activity>(a)).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to get activities", ex);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ReadAccess")]
        [Produces(typeof(Models.Activity))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetActivity(int id)
        {
            try
            {
                var dbActivity = QueryAll(dbContext).FirstOrDefault(a => a.Id == id);

                if (dbActivity != null)
                {
                    return Ok(mapper.Map<Models.Activity>(dbActivity));
                }
                else return NotFound($"Activity not found with id: {id}");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to get an activity", ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "WriteAccess")]
        [ProducesResponseType(typeof(Models.Activity), 201)]
        [ProducesResponseType(400)]
        public IActionResult AddActivity([FromBody]Models.Activity activity)
        {
            try
            {
                Activity dbActivity = new Activity { CreatedDateTime = DateTime.Now };
                dbActivity.UpdateFromModel(activity, dbContext);
                dbContext.Activity.Add(dbActivity);
                dbContext.SaveChanges();
                return CreatedAtRoute("GetActivity", new { id = activity.Id }, mapper.Map<Models.Activity>(dbActivity));
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to save an activity", ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "WriteAccess")]
        [Produces(typeof(Models.Activity))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateActivity(int id, [FromBody] Models.Activity activity)
        {
            try
            {
                var dbActivity = dbContext.Activity.Find(id);
                if (dbActivity == null)
                {
                    return NotFound($"Could not find an activity with an id of {id}");
                }
                dbActivity.UpdateFromModel(activity, dbContext);
                dbContext.Activity.Update(dbActivity);
                dbContext.SaveChanges();
                return Ok(mapper.Map<Models.Activity>(dbActivity));
            }
            catch (Exception ex)
            {
                return BadRequest("Couldn't update activity", ex);
            }
        }
    }
}

