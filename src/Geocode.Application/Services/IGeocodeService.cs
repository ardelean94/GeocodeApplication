using Geocode.Application.Models;

namespace Geocode.Application.Services;

public interface IGeocodeService
{
    Task<GeocodeCache> Get(string address, CancellationToken token = default);
}
