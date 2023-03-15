namespace API.Application.Models.Discounts
{
    public class Silver : IDiscount
    {
        public decimal Calculate(decimal accumulatedCharge) => accumulatedCharge * 0.1m;
    }
}
