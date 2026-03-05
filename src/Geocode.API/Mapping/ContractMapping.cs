using Geocode.Application.Models;
using Geocode.Contracts.Responses;

namespace Geocode.API.Mapping;

public static class ContractMapping
{
    public static GeocodeResponse MapToResponse(this GeocodeCache geocode)
    {
        return new GeocodeResponse
        {
            Status = geocode.GoogleResponse.Status,
            Results = geocode.GoogleResponse.Results
                .Select(x => x.ToGeocodeResultResponse())
                .ToList()
        };
    }

    #region Geocode Response Helpers
    private static GeocodeResultResponse ToGeocodeResultResponse(this GeocodeResult source)
    {
        return new GeocodeResultResponse
        {
            PlaceId = source.PlaceId,
            FormattedAddress = source.FormattedAddress,
            Types = source.Types,
            AddressComponents = source.AddressComponents
                .Select(x => x.ToAddressComponentResponse())
                .ToList(),
            Geometry = source.Geometry.ToGeometryResponse()
        };
    }

    private static AddressComponentResponse ToAddressComponentResponse(this AddressComponent source)
    {
        return new AddressComponentResponse
        {
            LongName = source.LongName,
            ShortName = source.ShortName,
            Types = source.Types
        };
    }

    private static GeometryResponse ToGeometryResponse(this Geometry source)
    {
        return new GeometryResponse
        {
            LocationType = source.LocationType,
            Location = source.Location.ToLatLngResponse(),
            Viewport = source.Viewport.ToViewportResponse()
        };
    }

    private static ViewportResponse ToViewportResponse(this Viewport source)
    {
        return new ViewportResponse
        {
            Northeast = source.Northeast.ToLatLngResponse(),
            Southwest = source.Southwest.ToLatLngResponse()
        };
    }

    private static LatLngResponse ToLatLngResponse(this LatLng source)
    {
        return new LatLngResponse
        {
            Lat = source.Lat,
            Lng = source.Lng
        };
    }
    #endregion
}
