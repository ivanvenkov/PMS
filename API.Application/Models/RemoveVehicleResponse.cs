using System.Text.Json.Serialization;

namespace API.Application.Models
{
    public class RemoveVehicleResponse
    {
        [JsonPropertyName("vehicleRegistrationNumber")]
        public string RegistrationNumber { get; set; }

        [JsonPropertyName("totalAccumulatedCharge")]
        public decimal AccumulatedCharge { get; set; }

        [JsonPropertyName("timeOfEntry")]
        public DateTime? TimeOfEntry { get; set; }
    }
}
