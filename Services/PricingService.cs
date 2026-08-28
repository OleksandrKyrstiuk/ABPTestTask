using TeskTask.Models;

namespace TeskTask.Services
{
    public class PricingService
    {
        public decimal CalculatePrice(Hall hall, DateTime startTime, int duration, List<Guid> serviceIds, AppData data)
        {
            decimal total = 0;

            // Розраховуємо вартість кожної години окремо, оскільки ціна залежить від часу доби.
            for (int i = 0; i < duration; i++)
            {
                var hour = startTime.AddHours(i);

                decimal multiplier = 1.0m;

                // Пікові години мають найвищий пріоритет, тому перевіряємо їх перед іншими часовими тарифами.
                if (hour.Hour >= 12 && hour.Hour < 14)
                {
                    multiplier = 1.15m;
                }
                // Вечірні години: знижка 20%.
                else if (hour.Hour >= 18 && hour.Hour < 23)
                {
                    multiplier = 0.80m;
                }
                // Ранкові години: знижка 10%.
                else if (hour.Hour >= 6 && hour.Hour < 9)
                {
                    multiplier = 0.90m;
                }

                total += hall.BasePricePerHour * multiplier;
            }

            // Додаткові послуги оплачуються один раз за бронювання, незалежно від тривалості оренди.
            var servicesPrice = data.Services.Where(service => serviceIds.Contains(service.Id)).Sum(service => service.Price);

            return total + servicesPrice;
        }
    }
}