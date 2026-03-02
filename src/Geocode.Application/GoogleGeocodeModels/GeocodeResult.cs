using System.Text.Json.Serialization;

namespace Geocode.Application.Models;

public class GeocodeResult
{
    [JsonPropertyName("place_id")]
    public string PlaceId { get; set; }

    [JsonPropertyName("formatted_address")]
    public string FormattedAddress { get; set; }

    [JsonPropertyName("types")]
    public List<string> Types { get; set; } = new();

    [JsonPropertyName("address_components")]
    public List<AddressComponent> AddressComponents { get; set; } = new();

    [JsonPropertyName("geometry")]
    public Geometry Geometry { get; set; }
}
