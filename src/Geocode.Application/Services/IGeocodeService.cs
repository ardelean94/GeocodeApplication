using Geocode.Application.Abstractions;
using Geocode.Application.Models;

namespace Geocode.Application.Services;

public interface IGeocodeService
{
    Task<Result<GeocodeCache>> Get(string address, CancellationToken token = default);
}
