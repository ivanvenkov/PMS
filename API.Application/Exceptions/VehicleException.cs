namespace API.Application.Exceptions
{
    public class VehicleException : Exception
    {
        public VehicleException(string message) : base(String.Format(message))
        {
        }
    }
}
