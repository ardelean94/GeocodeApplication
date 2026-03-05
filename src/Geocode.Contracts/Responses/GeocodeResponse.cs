namespace Geocode.Contracts.Responses;

public class GeocodeResponse
{
    public List<GeocodeResultResponse> Results { get; set; } = new();
    public string Status { get; set; }
}

public class GeocodeResultResponse
{
    public string PlaceId { get; set; }
    public string FormattedAddress { get; set; }
    public List<string> Types { get; set; } = new();
    public List<AddressComponentResponse> AddressComponents { get; set; } = new();
    public GeometryResponse Geometry { get; set; }
}

public class GeometryResponse
{
    public LatLngResponse Location { get; set; }
    public string LocationType { get; set; }
    public ViewportResponse Viewport { get; set; }
}

public class AddressComponentResponse
{
    public string LongName { get; set; }
    public string ShortName { get; set; }
    public List<string> Types { get; set; } = new();
}

public class LatLngResponse
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class ViewportResponse
{
    public LatLngResponse Northeast { get; set; }
    public LatLngResponse Southwest { get; set; }
}