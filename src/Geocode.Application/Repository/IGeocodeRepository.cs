using Geocode.Application.Abstractions;
using Geocode.Application.Models;

namespace Geocode.Application.Repository;

public interface IGeocodeRepository
{
    Task<Result<GeocodeCache?>> Get(string cacheKey, CancellationToken token = default);
    Task<Result<bool>> Create(GeocodeCache geocode, CancellationToken token = default);
}
