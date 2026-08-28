using System.Text.Json;
using TeskTask.DTOs;
using TeskTask.Models;

namespace TeskTask.Services
{
    public class BookingService
    {
        private readonly string _filePath;
        private readonly PricingService _pricingService;

        public BookingService( IWebHostEnvironment environment, PricingService pricingService)
        {
            _filePath = Path.Combine(environment.ContentRootPath, "Data", "data.json");
            _pricingService = pricingService;
        }

        public async Task<Booking> CreateAsync(CreateBookingDto dto)
        {
            ValidateRequest(dto);

            var data = await GetDataAsync();

            var hall = data.Halls.FirstOrDefault(h => h.Id == dto.HallId);

            if (hall == null)
            {
                throw new KeyNotFoundException("Hall not found.");
            }

            ValidateServices(dto.ServiceIds, hall);

            var endTime = dto.StartTime.AddHours(dto.DurationHours);

            ValidateWorkingHours(dto.StartTime, endTime);
            ValidateBookingConflict(data.Bookings, dto.HallId, dto.StartTime, endTime);

            var totalPrice = _pricingService.CalculatePrice(hall, dto.StartTime, dto.DurationHours, dto.ServiceIds, data);

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                HallId = dto.HallId,
                StartTime = dto.StartTime,
                DurationHours = dto.DurationHours,
                ServiceIds = dto.ServiceIds,
                TotalPrice = totalPrice
            };

            data.Bookings.Add(booking);

            await SaveAsync(data);

            return booking;
        }

        private static void ValidateRequest(CreateBookingDto dto)
        {
            if (dto.HallId == Guid.Empty)
            {
                throw new ArgumentException("Hall ID is required.");
            }

            if (dto.StartTime == default)
            {
                throw new ArgumentException("Start time is required.");
            }

            if (dto.StartTime < DateTime.Now)
            {
                throw new ArgumentException(
                    "Booking cannot be created in the past.");
            }

            if (dto.DurationHours <= 0)
            {
                throw new ArgumentException(
                    "Duration must be greater than zero.");
            }
        }

        private static void ValidateServices( List<Guid> serviceIds, Hall hall)
        {
            var invalidServiceIds = serviceIds.Where(serviceId => !hall.ServiceIds.Contains(serviceId)).ToList();

            if (invalidServiceIds.Any())
            {
                throw new ArgumentException(
                    "One or more selected services are not available in this hall.");
            }
        }

        private static void ValidateWorkingHours(DateTime startTime, DateTime endTime)
        {
            // Будівля недоступна поза часом з 06:00 до 23:00, тому бронювання має повністю припадати на цей проміжок часу.
            var openingTime = startTime.Date.AddHours(6);
            var closingTime = startTime.Date.AddHours(23);

            if (startTime < openingTime || endTime > closingTime)
            {
                throw new ArgumentException("Bookings are available only between 06:00 and 23:00.");
            }
        }

        private static void ValidateBookingConflict(List<Booking> bookings, Guid hallId, DateTime startTime, DateTime endTime)
        {
            // Два бронювання перекриваються, коли одне починається до того, як закінчиться інше, і закінчується після того, як почнеться інше.
            var hasConflict = bookings.Any(booking =>
                booking.HallId == hallId && startTime < booking.StartTime.AddHours(booking.DurationHours) && endTime > booking.StartTime);

            if (hasConflict)
            {
                throw new InvalidOperationException("The hall is already booked for the selected time.");
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

            return JsonSerializer.Deserialize<AppData>(json)
                   ?? new AppData();
        }

        private async Task SaveAsync(AppData data)
        {
            var directory = Path.GetDirectoryName(_filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(data, options);

            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}