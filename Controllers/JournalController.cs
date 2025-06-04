using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TreeAPI.Dto;

namespace TreeAPI.Controllers
{
    [ApiController]
    [Route("api/user/[controller]")]
    [Produces("application/json")]
    public class JournalController : ControllerBase
    {
        /// <summary>
        /// Returns a paginated list of <see cref="MJournalInfo"/>.
        /// </summary>
        /// <param name="filter">All fields of the filter <see cref="VJournalFilter"/> are optional</param>
        /// <param name="skip">Skip means the number of items should be skipped by server</param>
        /// <param name="take">Take means the maximum number items should be returned by server</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of <see cref="MJournalInfo"/>
        /// Returns a 200 OK status with the list of topics if successful
        /// </returns>
        [HttpPost]
        [Route("getRange")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "",
            Description = "Provides the pagination API." +
            "Skip means the number of items should be skipped by server." +
            "Take means the maximum number items should be returned by server." +
            "All fields of the filter are optional.",
            Tags = new[] { "user.journal" }
            )]
        public IActionResult GetRange(
            [FromQuery, Required] int skip,
            [FromQuery, Required] int take,
            [FromBody] VJournalFilter filter,
            CancellationToken cancellationToken)
        {
            return Ok(new List<MJournalInfo>());
        }

        /// <summary>
        /// Returns the information about an particular event by ID. 
        /// </summary>
        /// <param name="id">An particular event ID</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the object of the type <see cref="MJournalInfo"/>
        /// Returns a 200 OK status with the object if successful
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost()]
        [Route("getSingle")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "",
            Description = "Returns the information about an particular event by ID.",
            Tags = new[] { "user.journal" }
            )]
        public IActionResult GetSingle([FromQuery, Required] long id, CancellationToken cancellationToken)
        {
            return Ok(new MJournalInfo());
        }
    }
}
