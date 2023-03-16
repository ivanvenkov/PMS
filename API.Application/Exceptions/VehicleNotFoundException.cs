namespace API.Application.Exceptions
{
    public class VehicleNotFoundException : Exception
    {
        public VehicleNotFoundException(string message) : base(String.Format(message))
        {
        }
    }
}
