using System.Text.Json;
using TeskTask.DTOs.Report;
using TeskTask.Models;

namespace TeskTask.Services
{
    public class ReportService
    {
        private readonly string _filePath;

        public ReportService(IWebHostEnvironment environment) => _filePath = Path.Combine(environment.ContentRootPath, "Data", "data.json");
        
        public async Task<OverviewReportDto> GetOverviewAsync(DateTime? from, DateTime? to)
        {
            ValidateDateRange(from, to);

            var data = await GetDataAsync();
            var bookings = FilterBookings(data.Bookings, from, to);

            return new OverviewReportDto
            {
                TotalBookings = bookings.Count,

                TotalRevenue = bookings.Sum(b => b.TotalPrice),

                AverageBookingPrice = bookings.Count > 0 ? bookings.Average(b => b.TotalPrice) : 0,
                AverageBookingDuration = bookings.Count > 0 ? bookings.Average(b => b.DurationHours) : 0
            };
        }

        public async Task<List<HallReportDto>> GetHallReportAsync(DateTime? from, DateTime? to)
        {
            ValidateDateRange(from, to);

            var data = await GetDataAsync();
            var bookings = FilterBookings(data.Bookings, from, to);

            return data.Halls.Select(hall =>
                {
                    var hallBookings = bookings.Where(b => b.HallId == hall.Id).ToList();

                    return new HallReportDto
                    {
                        HallId = hall.Id,
                        HallName = hall.Name,
                        BookingCount = hallBookings.Count,
                        Revenue = hallBookings.Sum(b => b.TotalPrice),
                        BookedHours = hallBookings.Sum(b => b.DurationHours)
                    };
                })
                .OrderByDescending(h => h.Revenue)
                .ToList();
        }

        public async Task<List<ServiceReportDto>> GetServiceReportAsync(DateTime? from, DateTime? to)
        {
            ValidateDateRange(from, to);

            var data = await GetDataAsync();
            var bookings = FilterBookings(data.Bookings, from, to);

            return data.Services.Select(service =>
                {
                    var serviceBookings = bookings
                        .Where(b => b.ServiceIds.Contains(service.Id))
                        .ToList();

                    return new ServiceReportDto
                    {
                        ServiceId = service.Id,
                        ServiceName = service.Name,
                        UsageCount = serviceBookings.Count,

                        // Послуга оплачується один раз за кожне бронювання.
                        Revenue = serviceBookings.Count * service.Price
                    };
                })
                .OrderByDescending(s => s.UsageCount)
                .ToList();
        }

        public async Task<List<PeakHourReportDto>> GetPeakHoursAsync(DateTime? from, DateTime? to)
        {
            ValidateDateRange(from, to);

            var data = await GetDataAsync();
            var bookings = FilterBookings(data.Bookings, from, to);

            // Групуємо бронювання за годиною початку, щоб визначити години з найбільшою кількістю бронювань.
            return bookings.GroupBy(b => b.StartTime.Hour).Select(group => new PeakHourReportDto
                {
                    Hour = group.Key,
                    BookingCount = group.Count()
                })
                .OrderByDescending(x => x.BookingCount)
                .ToList();
        }

        private static List<Booking> FilterBookings(List<Booking> bookings, DateTime? from, DateTime? to)
        {
            return bookings.Where(b =>
                    (!from.HasValue || b.StartTime >= from.Value) &&
                    (!to.HasValue || b.StartTime <= to.Value))
                .ToList();
        }

        private static void ValidateDateRange(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                throw new ArgumentException("The 'from' date must be earlier than or equal to the 'to' date.");
            }
        }

        private async Task<AppData> GetDataAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new AppData();
            }

            var json = await File.ReadAllTextAsync(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new AppData();
            }

            try
            {
                return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
            }
            catch (JsonException)
            {
                throw new InvalidOperationException("The data file contains invalid JSON.");
            }
        }
    }
}