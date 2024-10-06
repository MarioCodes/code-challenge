﻿using Api.Core.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using Api.Core.Configuration;

namespace Api.Core.Controllers
{
    [ApiController]
    [Route("slots")]
    public class SlotsController(ISlotsService _service,
        IOptions<CoreConfig> _iOptionsCoreConfig) : ControllerBase
    {

        private CoreConfig _coreConfig => _iOptionsCoreConfig.Value;

        // TODO: add commentaries as the following

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
                string inputDateFormat = _coreConfig.InputDateFormat;
                if(DateOnly.TryParseExact(date, inputDateFormat, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.Now))
                        return BadRequest(_coreConfig.ErrorMessages.InputDateSetInPast);

                    if (parsedDate.DayOfWeek != DayOfWeek.Monday)
                        return BadRequest(_coreConfig.ErrorMessages.InputDateNotMonday);

                    var response = await _service.GetWeekFreeSlotsAsync(parsedDate);
                    return Ok(response);
                }

                return BadRequest($"{_coreConfig.ErrorMessages.InputDateWrongFormat}: '{inputDateFormat}'");
            }
            catch (Exception ex)
            {
                return BadRequest($"{_coreConfig.ErrorMessages.InputDateGeneralError} more info: {ex}");
            }
        }

    }
}
