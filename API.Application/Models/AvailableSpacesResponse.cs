using System.Text.Json.Serialization;

namespace API.Application.Models
{
    public class AvailableSpacesResponse
    {
        [JsonPropertyName("availableSpaces")]
        public int AvailableSpaces { get; set; }
    }
}
