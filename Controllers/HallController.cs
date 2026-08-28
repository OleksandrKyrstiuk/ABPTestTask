using Microsoft.AspNetCore.Mvc;
using TeskTask.DTOs;
using TeskTask.Services;

namespace TeskTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HallController : ControllerBase
    {
        private readonly HallService _hallService;

        public HallController(HallService hallService) => _hallService = hallService;

        /// <summary>
        /// Creates a new conference hall.
        /// </summary>
        /// <param name="dto">
        /// Hall information including name, capacity, available services and base hourly price.
        /// </param>
        /// <returns>The ID of the newly created hall.</returns>
        /// <response code="201">Hall was successfully created.</response>
        /// <response code="400">The provided hall data is invalid.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateHallDto dto)
        {
            var hall = await _hallService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(Create),
                new { id = hall.Id },
                new
                {
                    message = "Hall successfully created.",
                    id = hall.Id
                });
        }

        /// <summary>
        /// Updates an existing conference hall.
        /// </summary>
        /// <param name="id">Unique identifier of the hall.</param>
        /// <param name="dto">Updated hall information.</param>
        /// <returns>The updated conference hall.</returns>
        /// <response code="200">Hall was successfully updated.</response>
        /// <response code="400">The provided hall data is invalid.</response>
        /// <response code="404">Hall with the specified ID was not found.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, UpdateHallDto dto)
        {
            var hall = await _hallService.UpdateAsync(id, dto);

            if (hall == null)
            {
                return NotFound(new
                {
                    message = "Hall not found."
                });
            }

            return Ok(new
            {
                message = "Hall successfully updated.",
                hall
            });
        }

        /// <summary>
        /// Deletes an existing conference hall.
        /// </summary>
        /// <param name="id">Unique identifier of the hall.</param>
        /// <returns>A confirmation message and the ID of the deleted hall.</returns>
        /// <response code="200">Hall was successfully deleted.</response>
        /// <response code="404">Hall with the specified ID was not found.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _hallService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Hall not found."
                });
            }

            return Ok(new
            {
                message = "Hall successfully deleted.",
                id
            });
        }

        /// <summary>
        /// Searches for conference halls available during the specified time period.
        /// </summary>
        /// <param name="dto">
        /// Search criteria including time period, minimum capacity,
        /// hall name and required services.
        /// Name and service filters are optional.
        /// </param>
        /// <returns>A list of conference halls matching the specified criteria.</returns>
        /// <response code="200">Returns the list of available halls.</response>
        /// <response code="400">The specified search criteria are invalid.</response>
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search(SearchHallDto dto)
        {
            if (dto.StartTime >= dto.EndTime)
            {
                return BadRequest(new
                {
                    message = "Start time must be earlier than end time."
                });
            }

            if (dto.Capacity.HasValue && dto.Capacity <= 0)
            {
                return BadRequest(new
                {
                    message = "Capacity must be greater than zero."
                });
            }

            var halls = await _hallService.SearchAsync(dto);

            return Ok(halls);
        }
    }
}