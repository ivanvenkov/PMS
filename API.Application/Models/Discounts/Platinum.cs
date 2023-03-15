namespace API.Application.Models.Discounts
{
    public class Platinum : IDiscount
    {
        public decimal Calculate(decimal accumulatedCharge) => accumulatedCharge * 0.2m;
    }
}
