using Geocode.Application.Models;

namespace Geocode.Application.Services;

public interface IGeocodeService
{
    Task<GoogleGeocodeModel> Get(string address, CancellationToken token = default);
}
