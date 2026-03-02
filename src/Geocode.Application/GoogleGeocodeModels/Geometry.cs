using System.Text.Json.Serialization;

namespace Geocode.Application.Models;

public class Geometry
{
    [JsonPropertyName("location")]
    public LatLng Location { get; set; }

    [JsonPropertyName("location_type")]
    public string LocationType { get; set; }

    [JsonPropertyName("viewport")]
    public Viewport Viewport { get; set; }
}
