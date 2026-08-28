using Microsoft.AspNetCore.Mvc;
using TeskTask.Services;

namespace TeskTask.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService) => _reportService = reportService;

        /// <summary>
        /// Returns an overview of booking activity and revenue.
        /// </summary>
        /// <param name="from">
        /// Optional start date of the reporting period.
        /// </param>
        /// <param name="to">
        /// Optional end date of the reporting period.
        /// </param>
        /// <returns>
        /// Total number of bookings, total revenue, average booking price
        /// and average booking duration.
        /// </returns>
        /// <response code="200">
        /// Report was successfully generated.
        /// </response>
        /// <response code="400">
        /// The provided date range is invalid.
        /// </response>
        [HttpGet("overview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOverview(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            if (!IsValidDateRange(from, to))
            {
                return BadRequest(new
                {
                    message = "'from' must be earlier than 'to'."
                });
            }

            var report = await _reportService.GetOverviewAsync(from, to);

            return Ok(report);
        }

        /// <summary>
        /// Returns booking and revenue statistics for each conference hall.
        /// </summary>
        /// <param name="from">
        /// Optional start date of the reporting period.
        /// </param>
        /// <param name="to">
        /// Optional end date of the reporting period.
        /// </param>
        /// <returns>
        /// A list of halls containing booking count, revenue and total booked hours.
        /// </returns>
        /// <response code="200">
        /// Hall report was successfully generated.
        /// </response>
        /// <response code="400">
        /// The provided date range is invalid.
        /// </response>
        [HttpGet("halls")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHalls(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            if (!IsValidDateRange(from, to))
            {
                return BadRequest(new
                {
                    message = "'from' must be earlier than 'to'."
                });
            }

            var report = await _reportService.GetHallReportAsync(from, to);

            return Ok(report);
        }

        /// <summary>
        /// Returns usage and revenue statistics for additional services.
        /// </summary>
        /// <param name="from">
        /// Optional start date of the reporting period.
        /// </param>
        /// <param name="to">
        /// Optional end date of the reporting period.
        /// </param>
        /// <returns>
        /// A list of services containing usage count and generated revenue.
        /// </returns>
        /// <response code="200">
        /// Service report was successfully generated.
        /// </response>
        /// <response code="400">
        /// The provided date range is invalid.
        /// </response>
        [HttpGet("services")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetServices(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            if (!IsValidDateRange(from, to))
            {
                return BadRequest(new
                {
                    message = "'from' must be earlier than 'to'."
                });
            }

            var report = await _reportService.GetServiceReportAsync(from, to);

            return Ok(report);
        }

        /// <summary>
        /// Returns booking statistics grouped by starting hour.
        /// </summary>
        /// <param name="from">
        /// Optional start date of the reporting period.
        /// </param>
        /// <param name="to">
        /// Optional end date of the reporting period.
        /// </param>
        /// <returns>
        /// A list of hours ordered by booking frequency.
        /// </returns>
        /// <response code="200">
        /// Peak hour report was successfully generated.
        /// </response>
        /// <response code="400">
        /// The provided date range is invalid.
        /// </response>
        [HttpGet("peak-hours")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPeakHours(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            if (!IsValidDateRange(from, to))
            {
                return BadRequest(new
                {
                    message = "'from' must be earlier than 'to'."
                });
            }

            var report = await _reportService.GetPeakHoursAsync(from, to);

            return Ok(report);
        }

        /// <summary>
        /// Validates the requested reporting period.
        /// </summary>
        private static bool IsValidDateRange(
            DateTime? from,
            DateTime? to)
        {
            return !from.HasValue || !to.HasValue || from.Value < to.Value;
        }
    }
}