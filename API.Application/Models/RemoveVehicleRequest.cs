using System.Text.Json.Serialization;

namespace API.Application.Models
{
    public class RemoveVehicleRequest
    {
        [JsonPropertyName("vehicleRegistrationNumber")]
        public string RegNumber { get; set; }
    }
}