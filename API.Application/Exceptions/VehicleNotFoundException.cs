namespace API.Application.Exceptions
{
    internal class VehicleNotFoundException : Exception
    {
        public VehicleNotFoundException(string message) : base(String.Format(message))
        {
        }
    }
}
