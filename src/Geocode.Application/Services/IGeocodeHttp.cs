using Geocode.Application.Abstractions;
using Geocode.Application.Models;

namespace Geocode.Application.Services;

public interface IGeocodeHttp
{
    Task<Result<GoogleGeocodeModel?>> FetchGeocodeDataAsync(string address, string apiKey, CancellationToken token = default);
}
