namespace API.Application.Models.Discounts
{
    public class Gold : IDiscount
    {
        public decimal Calculate(decimal accumulatedCharge) => accumulatedCharge * 0.15m;       
    }
}
