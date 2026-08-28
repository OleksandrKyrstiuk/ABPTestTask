using System.Text.Json;
using TeskTask.DTOs;
using TeskTask.Models;

namespace TeskTask.Services
{
    public class HallService
    {
        private readonly string _filePath;

        public HallService(IWebHostEnvironment environment) => _filePath = Path.Combine(environment.ContentRootPath, "Data", "data.json");

        public async Task<Hall> CreateAsync(CreateHallDto dto)
        {
            ValidateHallData(dto.Name, dto.Capacity, dto.BasePricePerHour);

            var data = await GetDataAsync();

            ValidateServices(dto.ServiceIds, data);

            var hall = new Hall
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Capacity = dto.Capacity,
                ServiceIds = dto.ServiceIds.Distinct().ToList(),
                BasePricePerHour = dto.BasePricePerHour
            };

            data.Halls.Add(hall);

            await SaveAsync(data);

            return hall;
        }

        public async Task<Hall?> UpdateAsync(Guid id, UpdateHallDto dto)
        {
            ValidateHallData(dto.Name, dto.Capacity, dto.BasePricePerHour);

            var data = await GetDataAsync();

            var hall = data.Halls.FirstOrDefault(h => h.Id == id);

            if (hall == null)
            {
                return null;
            }

            ValidateServices(dto.ServiceIds, data);

            hall.Name = dto.Name!.Trim();
            hall.Capacity = dto.Capacity;
            hall.ServiceIds = dto.ServiceIds.Distinct().ToList();
            hall.BasePricePerHour = dto.BasePricePerHour;

            await SaveAsync(data);

            return hall;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var data = await GetDataAsync();

            var hall = data.Halls.FirstOrDefault(h => h.Id == id);

            if (hall == null)
            {
                return false;
            }

            data.Halls.Remove(hall);

            await SaveAsync(data);

            return true;
        }

        public async Task<List<Hall>> SearchAsync(SearchHallDto dto)
        {
            var data = await GetDataAsync();

            var halls = data.Halls.AsEnumerable();

            if (dto.Capacity.HasValue)
            {
                halls = halls.Where(h => h.Capacity >= dto.Capacity.Value);
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                halls = halls.Where(h =>  h.Name != null && h.Name.Contains(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (dto.ServiceIds.Any())
            {
                // Зал повинен надавати всі послуги, які вимагає клієнт.
                halls = halls.Where(h => dto.ServiceIds.All(serviceId => h.ServiceIds.Contains(serviceId)));
            }

            // Зал доступний лише в тому випадку, якщо запитуваний період не перетинається з жодним існуючим бронюванням.
            halls = halls.Where(h => !data.Bookings.Any(booking => booking.HallId == h.Id &&
                    dto.StartTime < booking.StartTime.AddHours(booking.DurationHours) && dto.EndTime > booking.StartTime));

            return halls.ToList();
        }

        private static void ValidateHallData(string? name, int capacity, decimal basePricePerHour)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Hall name is required.");
            }

            if (capacity <= 0)
            {
                throw new ArgumentException("Hall capacity must be greater than zero.");
            }

            if (basePricePerHour <= 0)
            {
                throw new ArgumentException("Base price must be greater than zero.");
            }
        }

        private static void ValidateServices(IEnumerable<Guid> serviceIds,AppData data)
        {
            var requestedServiceIds = serviceIds.Distinct().ToList();

            var invalidServiceIds = requestedServiceIds
                .Where(serviceId => !data.Services.Any(service => service.Id == serviceId)).ToList();

            if (invalidServiceIds.Any())
            {
                throw new ArgumentException("One or more selected services do not exist.");
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

            return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
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