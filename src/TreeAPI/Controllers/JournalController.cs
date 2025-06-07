using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TreeAPI.Application.Abstractions;
using TreeAPI.Dto;
using TreeAPI.Mapping;

namespace TreeAPI.Controllers
{
    [ApiController]
    [Route("api/user/[controller]")]
    [Produces("application/json")]
    public class JournalController : ControllerBase
    {
        private readonly IJournalService _journalService;
        public JournalController(IJournalService journalService)
        {
            _journalService = journalService;
        }
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
        public async Task<IActionResult> GetRange(
            [FromQuery, Required] int skip,
            [FromQuery, Required] int take,
            [FromBody] VJournalFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await _journalService.GetRangeAsync(skip, take, filter.From, filter.To, filter.Search, cancellationToken);

            return Ok(JournalInfoMapper.MapToMRange_MJournalInfo(result));
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "",
            Description = "Returns the information about an particular event by ID.",
            Tags = new[] { "user.journal" }
            )]
        public async Task<IActionResult> GetSingle([FromQuery, Required] long eventId, CancellationToken cancellationToken)
        {
            var result = await _journalService.GetSingleAsync(eventId, cancellationToken);
            if (result is null)
            {
                return NotFound("Journal entry not found.");
            }
            return Ok(JournalInfoMapper.MapToMJournalInfo(result));
        }
    }
}
