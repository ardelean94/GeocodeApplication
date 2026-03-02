using System.Text.Json.Serialization;

namespace Geocode.Application.Models;

public class GoogleGeocodeModel
{
    [JsonPropertyName("results")]
    public List<GeocodeResult> Results { get; set; } = new();

    [JsonPropertyName("status")]
    public string Status { get; set; }
}