using API.Database.Enums;
using System.Text.Json.Serialization;

namespace API.Application.Models
{
    public class GetAllVehiclesResponse
    {
        [JsonPropertyName("vehicleRegistrationNumber")]
        public string RegistrationNumber { get; set; }

        [JsonPropertyName("discount")]
        public DiscountTypes? Discount { get; set; }

        [JsonPropertyName("vehicleType")]
        public VehicleType? VehicleType { get; set; }
    }
}
