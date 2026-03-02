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

    public async Task Create(GoogleGeocodeModel geocode, CancellationToken token = default)
    {
        await context.SaveAsync(geocode, token);
    }

    public async Task<GoogleGeocodeModel> Get(Address address, CancellationToken token = default)
    {
        //  To be defined
        string hashKey = string.Empty;
        string sortKey = string.Empty;

        var geocode = await context.LoadAsync<GoogleGeocodeModel>(hashKey, sortKey, token);
        return geocode;
    }
}
