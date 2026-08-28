using Microsoft.AspNetCore.Mvc;
using TeskTask.DTOs;
using TeskTask.Services;

namespace TeskTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService) =>
            _bookingService = bookingService;

        /// <summary>
        /// Creates a new booking for a conference hall.
        /// </summary>
        /// <param name="dto">
        /// Booking information including hall ID, start time, duration
        /// and selected additional services.
        /// </param>
        /// <returns>
        /// The created booking with its calculated total price.
        /// </returns>
        /// <response code="201">
        /// Booking was successfully created.
        /// </response>
        /// <response code="400">
        /// The provided booking data is invalid, the hall does not exist,
        /// selected services are unavailable, or the hall is already booked
        /// for the selected time.
        /// </response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            var booking = await _bookingService.CreateAsync(dto);

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message = "Booking successfully created.",
                    booking
                });
        }
    }
}