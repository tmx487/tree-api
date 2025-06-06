using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace TreeAPI.Controllers
{
    [ApiController]
    [Route("api/user/[controller]")]
    [Produces("application/json")]
    public class PartnerController : ControllerBase
    {
        /// <summary>
        /// </summary>
        [HttpPost]
        [Route("rememberMe")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "",
            Description = "",
            Tags = new[] { "user.partner" }
            )]
        public IActionResult RememberMe([FromQuery, Required] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("Code is required.");

            return Ok("Successful response");
        }
    }
}
