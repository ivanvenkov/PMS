using API.Database.Enums;
using System.Text.Json.Serialization;

namespace API.Application.Models
{
    public class AdmitVehicleRequest
    {
        [JsonPropertyName("regNumber")]
        public string RegNumber { get; set; }

        [JsonPropertyName("timeOfAdmission")]
        public DateTime TimeOfEntry { get; set;}

        public VehicleType VehicleType { get; set; }

        public DiscountTypes? Discount { get; set; }
    }
}