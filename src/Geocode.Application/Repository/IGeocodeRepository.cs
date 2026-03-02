using Geocode.Application.Models;

namespace Geocode.Application.Repository;

public interface IGeocodeRepository
{
    Task<GoogleGeocodeModel> Get(Address address, CancellationToken token = default);
    Task Create(GoogleGeocodeModel geocode, CancellationToken token = default);
}
