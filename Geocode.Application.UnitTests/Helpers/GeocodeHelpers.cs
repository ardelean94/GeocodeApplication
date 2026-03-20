using Geocode.Application.Models;

namespace Geocode.Application.UnitTests.Helpers;

public static class GeocodeHelpers
{
    public static GeocodeCache GetGeocodeCacheSample()
    {
        return new GeocodeCache
        {
            AddressCacheKey = "",
            NormalizedAddress = "",
            OriginalAddress = "",
            HouseNumber = 0,
            Street = "",
            City = "",
            StateCode = "",
            Zipcode = 12345,
            Country = "",
            GoogleResponse = new(),
            CreatedAt = "",
            ExpiresAt = 0,
        };
    }
}
