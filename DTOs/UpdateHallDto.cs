using System.ComponentModel.DataAnnotations;
using TeskTask.Models;

namespace TeskTask.DTOs
{
    public class UpdateHallDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public List<Guid> ServiceIds { get; set; } = new();

        [Range(0.01, double.MaxValue)]
        public decimal BasePricePerHour { get; set; }
    }
}
