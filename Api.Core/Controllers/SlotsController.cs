using Api.Core.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using System;

namespace Api.Core.Controllers
{
    [ApiController]
    [Route("slots")]
    public class SlotsController(ISlotsService _service) : ControllerBase
    {
        // TODO: add commentaries 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("/weekly/{date}")]
        [SwaggerOperation(Tags = ["user"])]
        public async Task<IActionResult> GetWeekAvailability(string date)
        {
            try
            {
                // TODO: set format @ config level
                if(DateOnly.TryParseExact(date, "yyyyMMdd", out var parsedDate))
                {
                    // TODO: change message!
                    // TODO: set messages @ config level

                    if (parsedDate < DateOnly.FromDateTime(DateTime.Now))
                        return BadRequest("date cannot be set in the past");

                    if (parsedDate.DayOfWeek != DayOfWeek.Monday)
                        return BadRequest("specified day is not monday");

                    var response = await _service.GetWeekFreeSlotsAsync(parsedDate);
                    // TODO: change return value
                    return Ok(response);
                }

                // TODO: use here date @ format level
                return BadRequest("couldn't parse specified date. please check it's a valid date with yyyymmdd format");
            }
            catch (Exception ex)
            {
                // TODO: improve message
                return BadRequest("error");
            }
        }

    }
}
