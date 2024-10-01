using Api.Core.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using System;

namespace Api.Core.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        [HttpGet("/beat")]
        [SwaggerOperation(Tags = ["user"])]
        public async Task<IActionResult> Beat([FromServices] IUserService userService)
        {
            try
            {
                return Ok($"beat {DateTime.Now}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"error in beat: {ex.Message}");
            }
        }

    }
}
