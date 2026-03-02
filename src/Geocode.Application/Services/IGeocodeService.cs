using Geocode.Application.Models;

namespace Geocode.Application.Services;

public interface IGeocodeService
{
    Task<GoogleGeocodeModel> Get(Address address, CancellationToken token = default);
}
