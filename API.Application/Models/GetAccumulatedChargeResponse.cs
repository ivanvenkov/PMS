using API.Database.Enums;
using System.Text.Json.Serialization;

namespace API.Application.Models
{
    public class GetAccumulatedChargeResponse
    {
        [JsonPropertyName("vehicleRegistrationNumber")]
        public string RegistrationNumber { get; set; }

        [JsonPropertyName("timeOfEntry")]
        public DateTime? TimeOfEntry { get; set; }

        [JsonPropertyName("discountType")]
        public string DiscountType { get; set; }

        [JsonPropertyName("accumulatedCharge")]
        public decimal AccumulatedCharge { get; set; }       

        [JsonPropertyName("discount")]
        public decimal Discount { get; set; }

        [JsonPropertyName("totalCharge")]
        public decimal TotalCharge { get; set; }       
    }   
}