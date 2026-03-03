using Amazon.DynamoDBv2.DataModel;
using Geocode.Application.Models;

namespace Geocode.Application.Repository;

public class GeocodeRepository : IGeocodeRepository
{
    private readonly IDynamoDBContext context;

    public GeocodeRepository(IDynamoDBContext context)
    {
        this.context = context;
    }

    public async Task Create(GeocodeCache geocode, CancellationToken token = default)
    {
        await context.SaveAsync(geocode, token);
    }

    public async Task<GoogleGeocodeModel?> Get(string cacheKey, CancellationToken token = default)
    {
        var geocode = await context.LoadAsync<GoogleGeocodeModel>(cacheKey, token);
        return geocode;
    }
}
