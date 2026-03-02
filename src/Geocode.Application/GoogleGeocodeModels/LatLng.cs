using System.Text.Json.Serialization;

namespace Geocode.Application.Models;

public class LatLng
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}
