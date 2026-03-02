using System.Text.Json.Serialization;

namespace Geocode.Application.Models;

public class Viewport
{
    [JsonPropertyName("northeast")]
    public LatLng Northeast { get; set; }

    [JsonPropertyName("southwest")]
    public LatLng Southwest { get; set; }
}