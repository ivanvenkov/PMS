namespace API.Application.Models.Discounts
{
    public interface IDiscount
    {
        public decimal Calculate(decimal accumulatedCharge); 
    }
}
