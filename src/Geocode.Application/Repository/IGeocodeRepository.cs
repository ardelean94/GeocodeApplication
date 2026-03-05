using Geocode.Application.Models;

namespace Geocode.Application.Repository;

public interface IGeocodeRepository
{
    Task<GeocodeCache?> Get(string cacheKey, CancellationToken token = default);
    Task Create(GeocodeCache geocode, CancellationToken token = default);
}
